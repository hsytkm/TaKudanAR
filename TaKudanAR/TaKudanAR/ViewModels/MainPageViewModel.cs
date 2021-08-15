using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TaKudanAR.Interfaces;
using TaKudanAR.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TaKudanAR.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public override string Title => "Main";

        public ObservableCollection<AssetImageSource> NodeImageSources { get; }
        public IReactiveProperty<AssetImageSource> SelectedNodeImage { get; }
        public IReadOnlyReactiveProperty<ImageSource?> MarkerImageSource { get; }

        public BusyNotifier BusyNotifier { get; } = new();
        public AsyncReactiveCommand TakeMarkerPhotoCommand { get; }
        public AsyncReactiveCommand StartMarkerArCommand { get; }
        public AsyncReactiveCommand StartMarkerlessArFloorCommand { get; }
        public AsyncReactiveCommand StartMarkerlessArWallCommand { get; }

        public MainPageViewModel()
        {
            var assetStore = Xamarin.Forms.DependencyService.Get<IAssetStore>();
            var kudanARService = Xamarin.Forms.DependencyService.Get<IKudanARService>();

            var markerImageSource = new ReactivePropertySlim<IKudanImageSource>(assetStore.MarkerAssets[0]).AddTo(_disposables);

            NodeImageSources = new ObservableCollection<AssetImageSource>(
                assetStore!.NodeAssets.Select(x => new AssetImageSource(x, assetStore.GetImageSource(x)!)));

            SelectedNodeImage = new ReactivePropertySlim<AssetImageSource>().AddTo(_disposables);

            MarkerImageSource = markerImageSource.Select(image => ToImageSource(image, assetStore))
                .ToReadOnlyReactivePropertySlim().AddTo(_disposables);

            TakeMarkerPhotoCommand = BusyNotifier.Inverse().ToAsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    using var busyToken = BusyNotifier.ProcessStart();
                    var image = await TakePhotoAsync();
                    if (image != null) markerImageSource.Value = image;
                }, _disposables.Add);

            StartMarkerArCommand = BusyNotifier.Inverse().ToAsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    using var busyToken = BusyNotifier.ProcessStart();
                    await kudanARService.StartMarkerARActivityAsync(markerImageSource.Value, SelectedNodeImage.Value.KudanImage);
                }, _disposables.Add);

            StartMarkerlessArFloorCommand = BusyNotifier.Inverse().ToAsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    using var busyToken = BusyNotifier.ProcessStart();
                    await kudanARService.StartMarkerlessARFloorActivityAsync(SelectedNodeImage.Value.KudanImage);
                }, _disposables.Add);

            StartMarkerlessArWallCommand = BusyNotifier.Inverse().ToAsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    using var busyToken = BusyNotifier.ProcessStart();
                    await kudanARService.StartMarkerlessARWallActivityAsync(SelectedNodeImage.Value.KudanImage);
                }, _disposables.Add);

        }

        private static async Task<IKudanImageSource?> TakePhotoAsync()
        {
            var imagePath = await Smapho.TakePhotoAsync();

            if (imagePath is null || !File.Exists(imagePath))
                return null;

            return KudanImageSource.CreateFromFile(imagePath);
        }

        private static ImageSource? ToImageSource(IKudanImageSource kudanImage, IAssetStore assetStore) =>
            kudanImage.IsAsset
                ? assetStore.GetImageSource(kudanImage)
                : ImageSource.FromFile(kudanImage.Key);
    }

    public class AssetImageSource
    {
        public IKudanImageSource KudanImage { get; }
        public ImageSource ImageSource { get; }
        public AssetImageSource(IKudanImageSource kudanImage, ImageSource imageSource)
            => (KudanImage, ImageSource) = (kudanImage, imageSource);
    }
}

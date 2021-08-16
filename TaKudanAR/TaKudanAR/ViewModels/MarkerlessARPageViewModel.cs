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
    public class MarkerlessARPageViewModel : ViewModelBase
    {
        public override string Title => "MarkerlessAR";

        public ImageSource? TargetImageSource { get; }
        public ImageSource? TrackingImageSource { get; }

        public BusyNotifier BusyNotifier { get; } = new();
        public AsyncReactiveCommand StartMarkerlessArFloorCommand { get; }
        public AsyncReactiveCommand StartMarkerlessArWallCommand { get; }

        public MarkerlessARPageViewModel()
        {
            var assetStore = Xamarin.Forms.DependencyService.Get<IAssetStore>();
            var kudanARService = Xamarin.Forms.DependencyService.Get<IKudanARService>();

            var targetKudanImage = assetStore.MarkerlessTargetNodeAsset;
            var trackingKudanImage = assetStore.MarkerlessTrackingNodeAsset;

            TargetImageSource = assetStore.GetImageSource(targetKudanImage);
            TrackingImageSource = assetStore.GetImageSource(trackingKudanImage);

            StartMarkerlessArFloorCommand = BusyNotifier.Inverse().ToAsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    using var busyToken = BusyNotifier.ProcessStart();
                    //await kudanARService.StartMarkerlessARFloorActivityAsync(SelectedNodeImage.Value.KudanImage);

                    await kudanARService.StartMarkerlessARFloorActivityAsync(targetKudanImage, trackingKudanImage);
                }, _disposables.Add);

            StartMarkerlessArWallCommand = BusyNotifier.Inverse().ToAsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    using var busyToken = BusyNotifier.ProcessStart();
                    //await kudanARService.StartMarkerlessARWallActivityAsync(SelectedNodeImage.Value.KudanImage);

                    await kudanARService.StartMarkerlessARWallActivityAsync(targetKudanImage, trackingKudanImage);
                }, _disposables.Add);

        }
    }
}

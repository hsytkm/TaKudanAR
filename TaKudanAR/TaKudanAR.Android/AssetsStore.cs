#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TaKudanAR.Droid;
using TaKudanAR.Interfaces;
using TaKudanAR.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(AssetsStore))]
namespace TaKudanAR.Droid
{
    public class AssetsStore : IAssetStore
    {
        private readonly static IReadOnlyList<IKudanImageSource> _markerAssets = new[]
        {
            KudanImageSource.CreateAsset("KudanMarker.jpg"),
        };

        private readonly static IReadOnlyList<IKudanImageSource> _nodeAssets = new[]
        {
            KudanImageSource.CreateAsset("KudanNode.png"),
            KudanImageSource.CreateAsset("NodePistols1.png"),
            KudanImageSource.CreateAsset("NodePistols2.png"),
            KudanImageSource.CreateAsset("NodePistols3.png"),
            KudanImageSource.CreateAsset("NodePistols5.png"),
            KudanImageSource.CreateAsset("NodePistols6.png"),
            KudanImageSource.CreateAsset("NodePistols7.png"),
        };

        public IReadOnlyList<IKudanImageSource> MarkerAssets => _markerAssets;
        public IReadOnlyList<IKudanImageSource> NodeAssets => _nodeAssets;

        public ImageSource? GetImageSource(IKudanImageSource source)
        {
            if (!source.IsAsset)
                throw new NotImplementedException();

            var assets = MainActivity.Instance?.Assets;
            if (assets is null) return null;

            // Stream の Dispose してない。 (Stream)ImageSource がやってくれる雰囲気がある。
            return ImageSource.FromStream(() => assets.Open(source.Key));
        }
    }
}

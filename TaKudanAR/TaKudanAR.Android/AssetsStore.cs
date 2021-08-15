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
           "KudanMarker.jpg",
           "Marker1.jpg",
        }
        .Select(x => KudanImageSource.CreateFromAsset(x)).ToArray();

        private readonly static IReadOnlyList<IKudanImageSource> _nodeAssets = new[]
        {
           "KudanNode.png",
           "NodePistols1.png", "NodePistols2.png", "NodePistols3.png",
           "NodePistols5.png", "NodePistols6.png", "NodePistols7.png",
           "NodeHarvest1.png", "NodeHarvest2.png", "NodeHarvest3.png",
        }
        .Select(x => KudanImageSource.CreateFromAsset(x)).ToArray();

        public IReadOnlyList<IKudanImageSource> MarkerAssets => _markerAssets;
        public IReadOnlyList<IKudanImageSource> NodeAssets => _nodeAssets;

        public ImageSource? GetImageSource(IKudanImageSource source)
        {
            if (!source.IsAsset) throw new NotImplementedException();

            var assets = MainActivity.Instance?.Assets;
            if (assets is null) return null;

            // Stream の Dispose してない。 (Stream)ImageSource がやってくれる雰囲気がある。
            return ImageSource.FromStream(() => assets.Open(source.Key));
        }
    }
}

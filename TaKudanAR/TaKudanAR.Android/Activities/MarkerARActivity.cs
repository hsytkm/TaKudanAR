#nullable enable
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EU.Kudan.Kudan;
using System;
using TaKudanAR.Droid.Extensions;
using TaKudanAR.Interfaces;
using TaKudanAR.Models;

// see also https://github.com/XLsoft-Corporation/Public-Samples-Android/blob/master/app/src/main/java/com/xlsoft/publicsamples/MarkerActivity.java
namespace TaKudanAR.Droid.Activities
{
    [Activity(Label = "MarkerAR")]
    public class MarkerARActivity : MarkerARActivityBase
    {
        internal const string MARKER_IMAGE_KEY = nameof(MARKER_IMAGE_KEY);
        internal const string MARKER_ASSET_FLAG_KEY = nameof(MARKER_ASSET_FLAG_KEY);
        internal const string NODE_IMAGE_KEY = nameof(NODE_IMAGE_KEY);
        internal const string NODE_ASSET_FLAG_KEY = nameof(NODE_ASSET_FLAG_KEY);

        private IKudanImageSource? _markerImageSource;
        private IKudanImageSource? _nodeImageSource;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ARAPIKey.Instance.SetAPIKey(KudanLicense.Key);

            _markerImageSource = GetKudanImageSource(Intent, MARKER_IMAGE_KEY, MARKER_ASSET_FLAG_KEY);
            _nodeImageSource = GetKudanImageSource(Intent, NODE_IMAGE_KEY, NODE_ASSET_FLAG_KEY);
        }

        public override void Setup()
        {
            base.Setup();

            _ = _markerImageSource ?? throw new NullReferenceException(nameof(_markerImageSource));
            _ = _nodeImageSource ?? throw new NullReferenceException(nameof(_nodeImageSource));

            // Get the trackable Manager singleton
            var tracker = ARImageTracker.Instance;
            tracker.Initialise();

            // Add image trackable to the image tracker manager
            using (var trackable = CreateTrackable("MainMarker", _markerImageSource, _nodeImageSource))
            {
                tracker.AddTrackable(trackable);
                trackable.AddListener(this);
            }

            // 2個目のマーカーとノードを追加（UI側を実装してないので無効化）
            //var assetStore = Xamarin.Forms.DependencyService.Get<IAssetStore>();
            //using (var trackable = CreateTrackable("Marker1", assetStore.MarkerAssets[1], assetStore.NodeAssets[1]))
            //{
            //    tracker.AddTrackable(trackable);
            //    trackable.AddListener(this);
            //}

            // Add listener methods that are defined in the ARImageTrackableListener interface
            foreach (var trackable in tracker.Trackables)
            {
                trackable.AddListener(this);
            }
        }

        private static ARImageTrackable CreateTrackable(string trackableName, IKudanImageSource markerImage, IKudanImageSource nodeImage)
        {
            // Create our trackable with an image
            var trackable = markerImage.ToARImageTrackable(trackableName);

            // Create an image node using an image of the kudan cow
            using var nodeTexture = nodeImage.ToARTexture2D();
            using var imageNode = new ARImageNode(nodeTexture);

            // Node のサイズを Trackable のサイズに合わせる
            var scale = Math.Min(trackable.Width / (float)nodeTexture.Width, trackable.Height / (float)nodeTexture.Height);
            imageNode.ScaleByUniform(scale);

            // Add the image node as a child of the trackable's world
            trackable.World.AddChild(imageNode);

            return trackable;
        }
    }
}

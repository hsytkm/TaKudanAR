#nullable enable
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EU.Kudan.Kudan;
using System;
using TaKudanAR.Interfaces;
using TaKudanAR.Models;

namespace TaKudanAR.Droid.Activities
{
    public abstract class ARActivityBase : ARActivity, IARImageTrackableListener
    {
        internal const string MARKER_IMAGE_KEY = nameof(MARKER_IMAGE_KEY);
        internal const string MARKER_ASSET_FLAG_KEY = nameof(MARKER_ASSET_FLAG_KEY);
        internal const string NODE_IMAGE_KEY = nameof(NODE_IMAGE_KEY);
        internal const string NODE_ASSET_FLAG_KEY = nameof(NODE_ASSET_FLAG_KEY);

        protected IKudanImageSource? _markerImageSource;
        protected IKudanImageSource? _nodeImageSource;

        private static IKudanImageSource? GetKudanImageSource(Intent intent, string imageKey, string assetFlagKey)
        {
            var image = intent.GetStringExtra(imageKey);
            if (image is null) return null;

            return intent.GetBooleanExtra(assetFlagKey, default)
                ? KudanImageSource.CreateAsset(image)
                : KudanImageSource.CreateFile(image);
        }

        protected static IKudanImageSource? GetMarkerImageSource(Intent? intent)
        {
            if (intent is null) return null;
            return GetKudanImageSource(intent, MARKER_IMAGE_KEY, MARKER_ASSET_FLAG_KEY);
        }

        protected static IKudanImageSource? GetNodeImageSource(Intent? intent)
        {
            if (intent is null) return null;
            return GetKudanImageSource(intent, NODE_IMAGE_KEY, NODE_ASSET_FLAG_KEY);
        }

        public void DidDetect(ARImageTrackable? p0) => System.Diagnostics.Debug.WriteLine("Did Detect");
        public void DidLose(ARImageTrackable? p0) => System.Diagnostics.Debug.WriteLine("Did Lose");
        public void DidTrack(ARImageTrackable? p0) => System.Diagnostics.Debug.WriteLine("Did Track");
    }
}

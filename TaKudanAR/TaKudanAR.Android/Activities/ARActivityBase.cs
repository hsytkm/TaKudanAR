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
    public abstract class ARActivityBase : ARActivity
    {
        protected static IKudanImageSource? GetKudanImageSource(Intent? intent, string imageKey, string assetFlagKey)
        {
            if (intent is null) return null;

            var image = intent.GetStringExtra(imageKey);
            if (image is null) return null;

            return intent.GetBooleanExtra(assetFlagKey, default)
                ? KudanImageSource.CreateFromAsset(image)
                : KudanImageSource.CreateFromFile(image);
        }
    }

    public abstract class MarkerARActivityBase : ARActivityBase, IARImageTrackableListener
    {
        public void DidDetect(ARImageTrackable? p0) =>
            System.Diagnostics.Debug.WriteLine($"Did Detect : {p0?.Name}");

        public void DidLose(ARImageTrackable? p0) =>
            System.Diagnostics.Debug.WriteLine($"Did Lose : {p0?.Name}");

        public void DidTrack(ARImageTrackable? p0) =>
            System.Diagnostics.Debug.WriteLine($"Did Track : {p0?.Name}");
    }

    public abstract class MarkerlessARActivityBase : ARActivityBase
    {
        internal const string TARGET_IMAGE_KEY = nameof(TARGET_IMAGE_KEY);
        internal const string TARGET_ASSET_FLAG_KEY = nameof(TARGET_ASSET_FLAG_KEY);
        internal const string TRACKING_IMAGE_KEY = nameof(TRACKING_IMAGE_KEY);
        internal const string TRACKING_ASSET_FLAG_KEY = nameof(TRACKING_ASSET_FLAG_KEY);

        protected IKudanImageSource? _targetImageSource;
        protected IKudanImageSource? _trackingImageSource;
    }
}

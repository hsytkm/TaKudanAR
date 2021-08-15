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
        internal const string NODE_IMAGE_KEY = nameof(NODE_IMAGE_KEY);
        internal const string NODE_ASSET_FLAG_KEY = nameof(NODE_ASSET_FLAG_KEY);

        protected IKudanImageSource? _nodeImageSource;

        protected static IKudanImageSource? GetKudanImageSource(Intent? intent, string imageKey, string assetFlagKey)
        {
            if (intent is null) return null;

            var image = intent.GetStringExtra(imageKey);
            if (image is null) return null;

            return intent.GetBooleanExtra(assetFlagKey, default)
                ? KudanImageSource.CreateFromAsset(image)
                : KudanImageSource.CreateFromFile(image);
        }

        protected static IKudanImageSource? GetNodeImageSource(Intent? intent) =>
            GetKudanImageSource(intent, NODE_IMAGE_KEY, NODE_ASSET_FLAG_KEY);

        public void DidDetect(ARImageTrackable? p0) =>
            System.Diagnostics.Debug.WriteLine($"Did Detect : {p0?.Name}");

        public void DidLose(ARImageTrackable? p0) =>
            System.Diagnostics.Debug.WriteLine($"Did Lose : {p0?.Name}");

        public void DidTrack(ARImageTrackable? p0) =>
            System.Diagnostics.Debug.WriteLine($"Did Track : {p0?.Name}");
    }
}

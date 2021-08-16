#nullable enable
using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using TaKudanAR.Models;
using TaKudanAR.Interfaces;
using TaKudanAR.Droid.Activities;

namespace TaKudanAR.Droid
{
    [Activity(Label = "TaKudanAR", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity? Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;    // MarkerARActivity を StartActivity する際に使用
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void StartMarkerARActivity(IKudanImageSource marker, IKudanImageSource node)
        {
            using var intent = new Intent(this, typeof(MarkerARActivity));

            intent.PutExtra(MarkerARActivity.MARKER_IMAGE_KEY, marker.Key);
            intent.PutExtra(MarkerARActivity.MARKER_ASSET_FLAG_KEY, marker.IsAsset);
            intent.PutExtra(MarkerARActivity.NODE_IMAGE_KEY, node.Key);
            intent.PutExtra(MarkerARActivity.NODE_ASSET_FLAG_KEY, node.IsAsset);

            StartActivity(intent);
        }

        public void StartMarkerlessARActivity<TActivity>(IKudanImageSource target, IKudanImageSource tracking)
            where TActivity : ARActivityBase
        {
            using var intent = new Intent(this, typeof(TActivity));

            intent.PutExtra(MarkerlessARActivityBase.TARGET_IMAGE_KEY, target.Key);
            intent.PutExtra(MarkerlessARActivityBase.TARGET_ASSET_FLAG_KEY, target.IsAsset);
            intent.PutExtra(MarkerlessARActivityBase.TRACKING_IMAGE_KEY, tracking.Key);
            intent.PutExtra(MarkerlessARActivityBase.TRACKING_ASSET_FLAG_KEY, tracking.IsAsset);

            StartActivity(intent);
        }

    }
}

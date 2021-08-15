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
using TaKudanAR.Droid.Activities;
using TaKudanAR.Interfaces;
using TaKudanAR.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(KudanARService))]
namespace TaKudanAR.Droid
{
    public class KudanARService : IKudanARService
    {
        private static readonly IReadOnlyList<Permissions.BasePermission> _kudanARPermissions = new Permissions.BasePermission[]
        {
            new Permissions.Camera(),
            new Permissions.StorageWrite(),
            new Permissions.StorageRead(),
        };

        public async Task StartMarkerARActivityAsync(IKudanImageSource marker, IKudanImageSource node)
        {
            var isGranted = await Smapho.CheckAndRequestPermissionsAsync(_kudanARPermissions);
            if (!isGranted)
                return;

            MainActivity.Instance?.StartMarkerARActivity(marker, node);
        }

        public async Task StartMarkerlessARFloorActivityAsync(IKudanImageSource node)
        {
            var isGranted = await Smapho.CheckAndRequestPermissionsAsync(_kudanARPermissions);
            if (!isGranted)
                return;

            MainActivity.Instance?.StartMarkerlessARActivity<MarkerlessARFloorActivity>(node);
        }

        public async Task StartMarkerlessARWallActivityAsync(IKudanImageSource node)
        {
            var isGranted = await Smapho.CheckAndRequestPermissionsAsync(_kudanARPermissions);
            if (!isGranted)
                return;

            MainActivity.Instance?.StartMarkerlessARActivity<MarkerlessARWallActivity>(node);
        }

    }
}

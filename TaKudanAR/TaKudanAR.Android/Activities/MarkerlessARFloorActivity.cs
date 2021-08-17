#nullable enable
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using Com.Jme3.Math;
using EU.Kudan.Kudan;
using System;
using TaKudanAR.Droid.Extensions;
using TaKudanAR.Interfaces;
using TaKudanAR.Models;

// see also https://github.com/XLsoft-Corporation/Public-Samples-Android/blob/master/app/src/main/java/com/xlsoft/publicsamples/MarkerlessActivity.java
namespace TaKudanAR.Droid.Activities
{
    [Activity(Label = "MarkerlessAR(Floor)")]
    public class MarkerlessARFloorActivity : MarkerlessARActivityBase, GestureDetector.IOnGestureListener
    {
        private GestureDetectorCompat? _gestureDetect;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ARAPIKey.Instance.SetAPIKey(KudanLicense.Key);

            _targetImageSource = GetKudanImageSource(Intent, TARGET_IMAGE_KEY, TARGET_ASSET_FLAG_KEY);
            _trackingImageSource = GetKudanImageSource(Intent, TRACKING_IMAGE_KEY, TRACKING_ASSET_FLAG_KEY);

            // Create gesture recogniser to start and stop arbitrack
            _gestureDetect = new GestureDetectorCompat(this, this);
        }

        public override void Setup()
        {
            base.Setup();

            _ = _targetImageSource ?? throw new NullReferenceException(nameof(_targetImageSource));
            _ = _trackingImageSource ?? throw new NullReferenceException(nameof(_trackingImageSource));

            var assetStore = Xamarin.Forms.DependencyService.Get<IAssetStore>();

            // We choose the orientation of our floor node so that the target node lies flat on the floor.
            // We rotate the node by -90 degrees about the x axis.
            var angles = new[] { -(float)Math.PI / 2f, 0f, 0f };
            using var floorOrientation = new Quaternion(angles);

            // Create a target node. A target node is a node whose position is used
            // to determine the initial position of arbitrack's world when arbitrack is started.
            // The target node in this case is an image node of the Kudan Cow.
            using var floorScale = new Vector3f(0.5f, 0.5f, 0.5f);
            using var floorTargetNode = CreateImageNode(_targetImageSource, floorOrientation, floorScale);

            // Add our target node to the gyroplacemanager's world.
            // The position of the target node is used to determine the initial position of arbitrack's world.
            AddNodeToGyroPlaceManager(floorTargetNode);

            // Create an image node to place in arbitrack's world.
            // We can choose the tracking node to have the same orientation as the target node.
            var trackingScale = Vector3f.UnitXyz;   // =(1f, 1f, 1f);
            using var trackingImageNode = CreateImageNode(_trackingImageSource, floorOrientation, trackingScale);

            // Set up arbitrack.
            SetUpArbiTrack(floorTargetNode, trackingImageNode);
        }

        private static ARImageNode CreateImageNode(IKudanImageSource nodeImage, Quaternion orientation, Vector3f scale)
        {
            using var texture = nodeImage.ToARTexture2D();
            var imageNode = new ARImageNode(texture);
            imageNode.Orientation = orientation;
            imageNode.Scale = scale;
            return imageNode;
        }

        private static void AddNodeToGyroPlaceManager(ARNode node)
        {
            // The gyroplacemanager positions it's world on a plane that represents the floor.
            // You can adjust the floor depth (The distance between the device and the floor) using ARGyroPlaceManager's floor depth variable.
            // The default floor depth is -150
            var gyroPlaceManager = ARGyroPlaceManager.Instance;
            gyroPlaceManager.Initialise();
            gyroPlaceManager.World.AddChild(node);
        }

        private void SetUpArbiTrack(ARNode targetNode, ARNode childNode)
        {
            // Get the arbitrack manager and initialise it.
            var arbiTrack = ARArbiTrack.Instance;
            arbiTrack.Initialise();

            // Set it's target node.
            arbiTrack.TargetNode = targetNode;

            // Add the tracking image node to the arbitrack world.
            arbiTrack.World.AddChild(childNode);

            // Add this activity as a listener of arbitrack
            //arbiTrack.AddListener(this);
        }

        public override bool OnTouchEvent(MotionEvent? e)
        {
            _ = _gestureDetect?.OnTouchEvent(e);
            return base.OnTouchEvent(e);
        }

        #region GestureDetector.IOnGestureListener
        public bool OnDown(MotionEvent? e) => true;

        public bool OnFling(MotionEvent? e1, MotionEvent? e2, float velocityX, float velocityY) => false;

        public void OnLongPress(MotionEvent? e) { }

        public bool OnScroll(MotionEvent? e1, MotionEvent? e2, float distanceX, float distanceY) => false;

        public void OnShowPress(MotionEvent? e) { }

        public bool OnSingleTapUp(MotionEvent? e)
        {
            var arbiTrack = ARArbiTrack.Instance;

            if (arbiTrack.IsTracking)
            {
                // トラッキングを停止して TargetNode を再表示
                arbiTrack.Stop();
                arbiTrack.TargetNode.Visible = true;
            }
            else
            {
                // トラッキングを開始して TargetNode を非表示
                arbiTrack.Start();
                arbiTrack.TargetNode.Visible = false;
            }
            return false;
        }
        #endregion

    }
}

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
using System.Linq;
using TaKudanAR.Droid.Extensions;
using TaKudanAR.Interfaces;
using TaKudanAR.Models;

// see also https://github.com/XLsoft-Corporation/Public-Samples-Android/blob/master/app/src/main/java/com/xlsoft/publicsamples/Markerless_Wall.java
namespace TaKudanAR.Droid.Activities
{
    [Activity(Label = "MarkerlessAR(Wall)")]
    public class MarkerlessARWallActivity : MarkerlessARActivityBase, GestureDetector.IOnGestureListener, IARArbiTrackListener
    {
        private GestureDetectorCompat? _gestureDetect;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ARAPIKey.Instance.SetAPIKey(KudanLicense.Key);

            _targetImageSource = GetKudanImageSource(Intent, TARGET_IMAGE_KEY, TARGET_ASSET_FLAG_KEY);
            _trackingImageSource = GetKudanImageSource(Intent, TRACKING_IMAGE_KEY, TRACKING_ASSET_FLAG_KEY);

            // Create gesture recogniser to start and stop arbitrack.
            _gestureDetect = new GestureDetectorCompat(this, this);
        }

        public override void Setup()
        {
            base.Setup();

            _ = _targetImageSource ?? throw new NullReferenceException(nameof(_targetImageSource));
            _ = _trackingImageSource ?? throw new NullReferenceException(nameof(_trackingImageSource));

            // We choose the orientation of the wall node to depend on the device orientation.
            var wallOrientation = WallOrientationForDeviceOrientation();

            // Create a target node. A target node is a node whose position is used
            // to determine the initial position of arbitrack's world when arbitrack is started.
            // The target node in this case is an image of the Kudan Cow.
            // Place the target node a distance of 1000 behind the screen.
            var wallTargetNode = CreateImageNode(_targetImageSource, wallOrientation, new Vector3f(0.5f, 0.5f, 0.5f), new Vector3f(0f, 0f, -1000f));

            // Add our target node as a child of the camera node associated with the content view port.
            ARView.ContentViewPort.Camera.AddChild(wallTargetNode);

            // Create an image node to place in arbitrack's world.
            var trackingImageNode = CreateImageNode(_trackingImageSource, Quaternion.Identity, Vector3f.UnitXyz, new Vector3f());

            // Set up arbitrack
            SetUpArbiTrack(wallTargetNode, trackingImageNode);
        }

        private static ARImageNode CreateImageNode(IKudanImageSource nodeImage, Quaternion orientation, Vector3f scale, Vector3f posision)
        {
            using var texture = nodeImage.ToARTexture2D();
            var imageNode = new ARImageNode(texture);
            imageNode.Orientation = orientation;
            imageNode.Scale = scale;
            imageNode.Position = posision;
            return imageNode;
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

            // Add this activity as a listener of arbitrack.
            arbiTrack.AddListener(this);
        }

        // Returns the correct orientation for the wall target node for various device orientations.
        private static Quaternion WallOrientationForDeviceOrientation()
        {
            var activity = ARRenderer.Instance.Activity;
            var windowManager = activity.GetSystemService(Context.WindowService)?.JavaCast<IWindowManager>();
            var rotation = windowManager?.DefaultDisplay?.Rotation ?? SurfaceOrientation.Rotation0;

            // The angles we will rotate our wall node by.
            // The components are {x,y,z} in radians.
            return rotation switch
            {
                SurfaceOrientation.Rotation0 => new Quaternion(new float[] { 0f, 0f, (float)Math.PI / 2f }),
                SurfaceOrientation.Rotation90 => Quaternion.Identity,
                SurfaceOrientation.Rotation180 => new Quaternion(new float[] { 0f, 0f, -(float)Math.PI / 2f }),
                SurfaceOrientation.Rotation270 => new Quaternion(new float[] { 0f, 0f, (float)Math.PI }),
                _ => Quaternion.Identity,
            };
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
                // ArbiTrack がトラッキング中の場合は、トラッキングを停止してワールド空間がレンダリングされないようにし、ターゲット ノードを表示
                arbiTrack.Stop();
                arbiTrack.TargetNode.Visible = true;
            }
            else
            {
                // トラッキング中でない場合は、トラッキングを開始してターゲット ノードを非表示にする
                arbiTrack.Start();
                arbiTrack.TargetNode.Visible = false;
            }
            return false;
        }
        #endregion

        #region IARArbiTrackListener
        public void ArbiTrackStarted()
        {
            var arbiTrack = ARArbiTrack.Instance;

            // Rotate the tracking node so that it has the same full orientation as the target node
            // As the target node is a child of the camera world and the tracking node is a child of arbitrack's world, we must first rotate the tracking node by the inverse of arbitrack's world orientation.
            // This is so to the eye it has the same orientation as the target node
            var trackingNode = arbiTrack.World.Children.FirstOrDefault();
            if (trackingNode is null) return;

            // At this point we can update the orientation of the tracking node as arbitrack will have updated it's orientation
            trackingNode.Orientation = arbiTrack.World.Orientation.Inverse().Mult(arbiTrack.TargetNode.Orientation);
        }
        #endregion

    }
}

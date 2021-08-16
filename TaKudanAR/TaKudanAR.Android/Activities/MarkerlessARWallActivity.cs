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

// see also https://github.com/XLsoft-Corporation/Public-Samples-Android/blob/master/app/src/main/java/com/xlsoft/publicsamples/Markerless_Wall.java
namespace TaKudanAR.Droid.Activities
{
    [Activity(Label = "MarkerlessAR(Wall)")]
    public class MarkerlessARWallActivity : MarkerlessARActivityBase //, GestureDetector.IOnGestureListener
    {
#if false
        private GestureDetectorCompat? _gestureDetect;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ARAPIKey.Instance.SetAPIKey(KudanLicense.Key);

            // Create gesture recogniser to start and stop arbitrack
            _gestureDetect = new GestureDetectorCompat(this, this);

            _markerImageSource = GetMarkerImageSource(Intent);
            _nodeImageSource = GetNodeImageSource(Intent);
        }

        public override void Setup()
        {
            base.Setup();

            // We choose the orientation of the wall node to depend on the device orientation
            var wallOrientation = WallOrientationForDeviceOrientation();

            // Create a target node. A target node is a node whose position is used to determine the initial position of arbitrack's world when arbitrack is started
            // The target node in this case is an image of the Kudan Cow
            // Place the target node a distance of 1000 behind the screen
            Vector3f targetPosition = new Vector3f(0, 0, -1000);
            Vector3f wallScale = new Vector3f(0.5f, 0.5f, 0.5f);
            wallTargetNode = CreateImageNode("cowtarget.png", wallOrientation, wallScale, targetPosition);

            // Add our target node as a child of the camera node associated with the content view port
            getARView().getContentViewPort().getCamera().addChild(wallTargetNode);

            // Create an image node to place in arbitrack's world
            ARImageNode trackingImageNode = CreateImageNode("cowtracking.png", Quaternion.IDENTITY, Vector3f.UNIT_XYZ, Vector3f.ZERO);

            // Set up arbitrack
            SetUpArbiTrack(this.wallTargetNode, trackingImageNode);
        }

        private static ARImageNode CreateImageNode(string imageName, Quaternion orientation, Vector3f scale)
        {
            var imageNode = new ARImageNode(imageName);
            imageNode.SetOrientation(orientation.GetX(), orientation.GetY(), orientation.GetZ(), orientation.GetW());
            imageNode.SetScale(scale.X, scale.Y, scale.Z);
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

        private static void SetUpArbiTrack(ARNode targetNode, ARNode childNode)
        {
            // Get the arbitrack manager and initialise it
            var arbiTrack = ARArbiTrack.Instance;
            arbiTrack.Initialise();

            // Set it's target node
            arbiTrack.TargetNode = targetNode;

            // Add the tracking image node to the arbitrack world
            arbiTrack.World.AddChild(childNode);
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
#endif
    }
}

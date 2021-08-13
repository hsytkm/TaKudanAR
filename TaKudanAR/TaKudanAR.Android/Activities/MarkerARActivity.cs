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

// see also https://github.com/XLsoft-Corporation/Public-Samples-Android/blob/master/app/src/main/java/com/xlsoft/publicsamples/MarkerActivity.java
namespace TaKudanAR.Droid.Activities
{
    [Activity(Label = "MarkerARActivity")]
    public class MarkerARActivity : ARActivityBase
    {
        private ARImageTrackable? _imageTrackable;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ARAPIKey.Instance.SetAPIKey(KudanLicense.Key);

            _markerImageSource = GetMarkerImageSource(Intent);
            _nodeImageSource = GetNodeImageSource(Intent);
        }

        public override void Setup()
        {
            base.Setup();

            if (_markerImageSource is null) throw new NullReferenceException(nameof(_markerImageSource));
            if (_nodeImageSource is null) throw new NullReferenceException(nameof(_nodeImageSource));

            // Create our trackable with an image
            var trackable = CreateTrackable("MyMarker", _markerImageSource);

            // Get the trackable Manager singleton
            // 画像トラッカーの 1 つのインスタンスを取得
            var trackableManager = ARImageTracker.Instance;
            trackableManager.Initialise();

            // Add image trackable to the image tracker manager
            // 画像トラッカブルを画像トラッカーに追加
            trackableManager.AddTrackable(trackable);

            // Create an image node using an image of the kudan cow
            // 画像で画像ノードを初期化(ビルドアクションがAndroidAssetのファイル名を指定)
            var node = CreateARImageNode(_nodeImageSource);

            // imageNode のサイズを Trackable のサイズに合わせる
            var textureMaterial = (ARTextureMaterial)node.Material;
            var scale = trackable.Width / textureMaterial.Texture.Width;
            node.ScaleByUniform(scale);

            // Add the image node as a child of the trackable's world
            // 画像ノードをトラッカブルのワールド空間の子として追加
            trackable.World.AddChild(node);

            // Add listener methods that are defined in the ARImageTrackableListener interface
            // リスナー登録
            trackable.AddListener(this);

            _imageTrackable = trackable;
        }

        private static ARImageTrackable CreateTrackable(string trackableName, IKudanImageSource imageSource)
        {
            // Create a new trackable instance with a name
            // 画像トラッカブルを初期化して画像をロード
            var trackable = new ARImageTrackable(trackableName);

            // Load the image for this marker
            if (imageSource.IsAsset)
            {
                trackable.LoadFromAsset(imageSource.Key);
            }
            else
            {
                trackable.LoadFromPath(imageSource.Key);
            }
            return trackable;
        }

        private ARImageNode CreateARImageNode(IKudanImageSource imageSource)
        {
            var texture = new ARTexture2D();
            if (imageSource.IsAsset)
            {
                texture.LoadFromAsset(imageSource.Key);
            }
            else
            {
                texture.LoadFromPath(imageSource.Key);
            }
            return new ARImageNode(texture);
        }

        public new void Dispose()
        {
            _imageTrackable?.Dispose();
            base.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;

namespace TaKudanAR.Interfaces
{
    public interface IAssetStore
    {
        IReadOnlyList<IKudanImageSource> MarkerAssets { get; }
        IReadOnlyList<IKudanImageSource> NodeAssets { get; }

        IKudanImageSource MarkerlessTargetNodeAsset { get; }
        IKudanImageSource MarkerlessTrackingNodeAsset { get; }

        Xamarin.Forms.ImageSource? GetImageSource(IKudanImageSource source);

    }
}

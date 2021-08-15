#nullable enable
using EU.Kudan.Kudan;
using TaKudanAR.Interfaces;

namespace TaKudanAR.Droid.Extensions
{
    internal static class IKudanImageSourceExtension
    {
        public static ARImageTrackable ToARImageTrackable(this IKudanImageSource imageSource, string trackableName = "")
        {
            // Create a new trackable instance with a name
            var trackable = string.IsNullOrWhiteSpace(trackableName)
                ? new ARImageTrackable()
                : new ARImageTrackable(trackableName);

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

        public static ARTexture2D ToARTexture2D(this IKudanImageSource imageSource)
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
            return texture;
        }

    }
}

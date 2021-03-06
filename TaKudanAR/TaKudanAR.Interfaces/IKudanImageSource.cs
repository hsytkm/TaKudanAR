using System;

namespace TaKudanAR.Interfaces
{
    public interface IKudanImageSource
    {
        bool IsAsset { get; }
        string Key { get; }
    }

    public class KudanImageSource : IKudanImageSource
    {
        public bool IsAsset { get; }
        public string Key { get; }

        private KudanImageSource(bool isAsset, string key) => (IsAsset, Key) = (isAsset, key);
        public static IKudanImageSource CreateFromAsset(string asset) => new KudanImageSource(true, asset);
        public static IKudanImageSource CreateFromFile(string path) => new KudanImageSource(false, path);
    }
}

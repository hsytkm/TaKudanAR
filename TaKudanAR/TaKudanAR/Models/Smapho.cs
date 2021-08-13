using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace TaKudanAR.Models
{
    public static class Smapho
    {
        private static readonly IReadOnlyList<Permissions.BasePermission> _imagePermissions = new Permissions.BasePermission[]
        {
            new Permissions.StorageWrite(),
            new Permissions.StorageRead(),
        };

        private static readonly IReadOnlyList<Permissions.BasePermission> _takePhotoPermissions = new Permissions.BasePermission[]
        {
            new Permissions.Camera(),
            new Permissions.StorageWrite(),
            new Permissions.StorageRead(),
        };

        public static async ValueTask<bool> CheckAndRequestPermissionsAsync(IEnumerable<Permissions.BasePermission> permissions)
        {
            foreach (var permission in permissions)
            {
                var status = await CheckAndRequestPermissionAsync(permission);
                if (status != PermissionStatus.Granted)
                {
                    // Notify user permission was denied
                    return false;
                }
            }
            return true;
        }

        public static async ValueTask<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
            where T : Permissions.BasePermission
        {
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                status = await permission.RequestAsync();
            }
            return status;
        }

        /// <summary>
        /// 画像を選択してファイルパスを取得
        /// </summary>
        /// <returns></returns>
        public static async ValueTask<string?> GetImagePathAsync()
        {
            // パーミッションチェック
            var grantedFlag = await CheckAndRequestPermissionsAsync(_imagePermissions);
            if (!grantedFlag)
                return null;

            // Pluginの初期化
            _ = await CrossMedia.Current.Initialize();

            // 画像選択可能か判定
            if (!CrossMedia.Current.IsPickPhotoSupported)
                return null;

            // 画像選択画面を表示
            using var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                SaveMetaData = false,
                PhotoSize = PhotoSize.Full,
            });

            // 画像を選択しなかった場合は終了
            if (file is null)
            {
                System.Diagnostics.Debug.WriteLine("画像選択なし");
                return null;
            }

            // 画像ファイルをリネームして、不要な一時ファイルが溜まらないよう削除
            return CopyFile(file.Path);
        }

        /// <summary>
        /// カメラで撮影してファイルパスを取得
        /// </summary>
        /// <returns></returns>
        public static async ValueTask<string?> TakePhotoAsync()
        {
            // パーミッションチェック
            var grantedFlag = await CheckAndRequestPermissionsAsync(_takePhotoPermissions);
            if (!grantedFlag)
                return null;

            // Pluginの初期化
            _ = await CrossMedia.Current.Initialize();

            // 撮影可能か判定
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                return null;

            // カメラが起動し写真を撮影する。撮影した写真はストレージに保存され、ファイルの情報が return される
            using var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                // ストレージに保存するファイル情報
                // すでに同名ファイルがある場合は、temp_1.jpg などの様に連番がつけられ名前の衝突が回避される
                Directory = "TempPhotos",
                Name = "temp.jpg",
                SaveMetaData = false,
                PhotoSize = PhotoSize.Full,
            });

            // カメラ撮影しなかった場合は終了
            if (file is null)
            {
                System.Diagnostics.Debug.WriteLine("カメラ撮影なし");
                return null;
            }

            // 画像ファイルをリネームして、不要な一時ファイルが溜まらないよう削除
            return CopyFile(file.Path);
        }

        // 画像ファイルをリネームして、不要な一時ファイルが溜まらないよう削除（◆いる？？）
        private static string CopyFile(string sourcePath)
        {
            var directory = Path.GetDirectoryName(sourcePath);
            var extension = Path.GetExtension(sourcePath);
            var newPath = Path.Combine(directory, "temp" + extension);
            File.Copy(sourcePath, newPath, true);
            File.Delete(sourcePath);
            return newPath;
        }
    }
}

using Factories;
using System.IO;
using UnityEngine;

namespace Globals
{
    public static class CacheManager
    {

        public static Sprite GetSprite(ImageType type, string url)
        {
            string cacheName = "ImagesCache";
            string objectId = BuildObjectIdFromUrl(url);
            return LoadSpriteFromFile(Application.persistentDataPath + "/" + cacheName + "/" + objectId);

        }

        public static void PutSprite(Sprite sprite, ImageType type, string url)
        {
            string cacheName = "ImagesCache";
            string objectId = BuildObjectIdFromUrl(url);
            SaveSpriteToFile(sprite, Application.persistentDataPath + "/" + cacheName + "/" + objectId);
        }

        private static void SaveSpriteToFile(Sprite sprite, string path)
        {
            Save(sprite.texture.EncodeToPNG(), path);
        }

        private static string BuildObjectIdFromUrl(string url)
        {
            int lastIndexOfBackSlash = url.LastIndexOf('/');
            int secondLastIndex = lastIndexOfBackSlash > 0 ? url.LastIndexOf('/', lastIndexOfBackSlash - 1) : -1;

            return url.Substring(secondLastIndex, url.Length - secondLastIndex);
        }

        private static void Save(byte[] data, string path)
        {
            var file = new FileInfo(path);
            file.Directory.Create();

            File.WriteAllBytes(path, data);

        }

        private static Sprite LoadSpriteFromFile(string path)
        {
            var f = new FileInfo(path);
            if (f.Exists)
            {
                Texture2D texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
                texture.LoadImage(File.ReadAllBytes(path));
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                
            }
            return null;
        }
    }
}
using Factories;
using Factories.Config;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Globals
{
    public static class CacheManager
    {

        private static Dictionary<string,Dictionary<string, System.Object>> caches = new Dictionary<string, Dictionary<string, System.Object>>();

        private static Dictionary<string,System.Object> GetCache(string cacheName)
        {
            if (!caches.ContainsKey(cacheName))
            {
                caches.Add(cacheName, new Dictionary<string, object>());
            }
            return caches[cacheName];
        }
        public static Sprite GetSprite(ImageType type, string url)
        {
            Sprite returnValue = null;
            string cacheName = "ImagesCache";
            string objectId = BuildObjectIdFromUrl(url);
            bool useMemory = false;
            bool useFile = false;
            switch (type)
            {
                case ImageType.Small:
                    useMemory = true;
                    useFile = true;
                    break;
                case ImageType.Large:
                    break;
                default:
                    break;
            }
            if (useMemory)
            {
                if (GetCache(cacheName).ContainsKey(objectId))
                {
                    return (Sprite)GetCache(cacheName)[objectId];
                }
            }
            if (useFile)
            {
                returnValue = LoadSpriteFromFile(Application.persistentDataPath + "/" + cacheName + "/" + objectId);
                if (returnValue != null && useMemory)
                { // first time loaded
                    GetCache(cacheName).Add(objectId, returnValue);
                }
            }
            return returnValue;

        }

        public static void PutSprite(Sprite sprite, ImageType type, string url)
        {
            string cacheName = "ImagesCache";
            string objectId = BuildObjectIdFromUrl(url);
            bool useMemory = false;
            bool useFile = false;
            switch (type)
            {
                case ImageType.Small:
                    useMemory = true;
                    useFile = true;
                    break;
                case ImageType.Large:
                    break;
                default:
                    break;
            }
            if (useMemory)
            {
                GetCache(cacheName).Add(objectId, sprite);
            }
            if (useFile)
            {
                SaveSpriteToFile(sprite, Application.persistentDataPath + "/" + cacheName + "/" + objectId);
            }
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
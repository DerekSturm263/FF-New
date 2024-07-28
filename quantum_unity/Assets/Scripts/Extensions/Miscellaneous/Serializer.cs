using Quantum;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FusionFighters
{
    public static class Serializer
    {
        public static void Save<T>(T item, AssetGuid fileName, string directory) where T : struct => SaveInternal(item, $"{directory}/{fileName}.json", directory);
        public static void Save<T>(T item, string fileName, string directory) where T : struct => SaveInternal(item, $"{directory}/{fileName}.json", directory);

        public static void SaveAs<T>(T item, string filePath, string dataPath) where T : struct => SaveInternal(item, filePath, dataPath);
        private static void SaveInternal<T>(T item, string filePath, string dataPath) where T : struct
        {
            string json = ToJSON(item);

            CreateDirectory(dataPath);

            FileMode writeMode = File.Exists(filePath) ? FileMode.Truncate : FileMode.Create;
            using StreamWriter writer = new(File.Open(filePath, writeMode));
            writer.Write(json);
            writer.Flush();

            Debug.Log($"{nameof(T)} has been successfully saved as: {json}");
        }

        public static T LoadAs<T>(string filePath, string directory) where T : struct
        {
            TryLoadInternal(filePath, directory, out T item);
            return item;
        }
        public static bool TryLoadAs<T>(string filePath, string directory, out T item) where T : struct => TryLoadInternal(filePath, directory, out item);
        private static bool TryLoadInternal<T>(string filePath, string directory, out T item) where T : struct
        {
            item = default;

            CreateDirectory(directory);

            if (File.Exists(filePath))
            {
                using StreamReader reader = new(File.Open(filePath, FileMode.Open));

                string json = reader.ReadToEnd();
                item = FromJSON<T>(json);

                Debug.Log($"{nameof(T)} has been successfully loaded as: {json}");
                return true;
            }
            else
            {
                Debug.Log($"{nameof(T)} could not be loaded. Returning the default: {ToJSON(default(T))}");
                return false;
            }
        }

        public static void Delete(string filePath, string directory)
        {
            CreateDirectory(directory);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Item was successfully deleted");
            }
            else
            {
                Debug.Log($"Item could not be deleted, as it was not found");
            }
        }

        private static string ToJSON<T>(T item) where T : struct => JsonUtility.ToJson(item, true);
        private static T FromJSON<T>(string json) where T : struct => JsonUtility.FromJson<T>(json);

        public static IEnumerable<T> LoadAllFromDirectory<T>(string directory) where T : struct
        {
            CreateDirectory(directory);
            return Directory.EnumerateFiles(directory, "*.json").Select(filePath => LoadAs<T>(filePath, directory));
        }

        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
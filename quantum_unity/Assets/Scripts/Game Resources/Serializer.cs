using Quantum;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class Serializer
{
    public static void Save<T>(T item, AssetGuid guid, string directory) => SaveInternal(item, $"{directory}/{guid}.json", directory);
    public static void Save<T>(T item, string fileName, string directory) => SaveInternal(item, $"{directory}/{fileName}.json", directory);

    public static void SaveAs<T>(T item, string filePath, string dataPath) => SaveInternal(item, filePath, dataPath);
    private static void SaveInternal<T>(T item, string filePath, string dataPath)
    {
        string json = ToJSON(item);

        CreateDirectory(dataPath);

        FileMode writeMode = File.Exists(filePath) ? FileMode.Truncate : FileMode.Create;
        using StreamWriter writer = new(File.Open(filePath, writeMode));
        writer.Write(json);
        writer.Flush();

        Debug.Log($"{nameof(T)} has been successfully saved as: {item}");
    }

    public static T LoadAs<T>(string filePath, string directory)
    {
        TryLoadInternal(filePath, directory, out T item);
        return item;
    }
    public static bool TryLoadAs<T>(string filePath, string directory, out T item) => TryLoadInternal(filePath, directory, out item);
    private static bool TryLoadInternal<T>(string filePath, string directory, out T item)
    {
        item = default;

        CreateDirectory(directory);

        if (File.Exists(filePath))
        {
            using StreamReader reader = new(File.Open(filePath, FileMode.Open));

            string json = reader.ReadToEnd();
            item = FromJSON<T>(json);

            Debug.Log($"{nameof(T)} has been successfully loaded as: {item}");
            return true;
        }
        else
        {
            Debug.Log($"{nameof(T)} could not be loaded. Returning the default: {item}");
            return false;
        }
    }

    private static string ToJSON<T>(T item) => JsonUtility.ToJson(item, true);
    private static T FromJSON<T>(string json) => JsonUtility.FromJson<T>(json);

    public static IEnumerable<T> LoadAllFromDirectory<T>(string directory)
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

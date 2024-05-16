using Quantum;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class Serializer
{
    public static void Save<T>(T item, AssetGuid guid, string directory) => SaveInternal(item, $"{directory}/{guid}.json", directory);

    public static void SaveAs<T>(T item, string filePath, string dataPath) => SaveInternal(item, filePath, dataPath);
    private static void SaveInternal<T>(T item, string filePath, string dataPath)
    {
        //_metaSettings.SetLastEdittedDate(System.DateTime.Now.Ticks);
        string json = ToJSON(item);

        CreateDirectory(dataPath);

        FileMode writeMode = File.Exists(filePath) ? FileMode.Truncate : FileMode.Create;
        using StreamWriter writer = new(File.Open(filePath, writeMode));
        writer.Write(json);
        writer.Flush();
    }

    public static T LoadAs<T>(string filePath, string dataPath) => LoadInternal<T>(filePath, dataPath);
    private static T LoadInternal<T>(string filePath, string dataPath)
    {
        T item = default;

        CreateDirectory(dataPath);

        if (File.Exists(filePath))
        {
            using StreamReader reader = new(File.Open(filePath, FileMode.Open));

            string json = reader.ReadToEnd();
            item = FromJSON<T>(json);
        }
        
        return item;
    }

    private static string ToJSON<T>(T item) => JsonUtility.ToJson(item, true);
    private static T FromJSON<T>(string json) => JsonUtility.FromJson<T>(json);

    public static IEnumerable<T> LoadAllFromDirectory<T>(string directory) => Directory.EnumerateFiles(directory, "*.json").Select(filePath => LoadAs<T>(filePath, directory));

    public static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }
}

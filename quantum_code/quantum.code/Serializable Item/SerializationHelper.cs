using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Quantum
{
    public static unsafe partial class SerializationHelper
    {
        private static string ApplicationFilePath() => string.Empty;

        public static string BuildPath => $"{ApplicationFilePath()}/Builds";

        public static void Save(Build* build) => SaveInternal(build, $"{BuildPath}/{build->SerializableData.Guid}.json");
        public static void SaveAs(Build* build, string filePath) => SaveInternal(build, filePath);
        private static void SaveInternal(Build* build, string filePath)
        {
            build->SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;
            string json = ToJSON(*build);

            CreateBuildDirectory();

            FileMode writeMode = File.Exists(filePath) ? FileMode.Truncate : FileMode.Create;
            using StreamWriter writer = new(File.Open(filePath, writeMode));
            writer.Write(json);
            writer.Flush();

            Log.Debug($"Saved Build: {build->SerializableData.Name}");
        }

        public static void Load(Build* build) => LoadInternal(build, $"{BuildPath}/{build->SerializableData.Guid}.json");
        public static void LoadAs(Build* build, string filePath) => LoadInternal(build, filePath);
        private static void LoadInternal(Build* build, string filePath)
        {
            CreateBuildDirectory();

            if (File.Exists(filePath))
            {
                using StreamReader reader = new(File.Open(filePath, FileMode.Open));

                string json = reader.ReadToEnd();
                *build = FromJSON<Build>(json);
            }

            Log.Debug($"Loaded Build: {build->SerializableData.Name}");
        }

        private static T FromJSON<T>(string json) => throw new System.NotImplementedException();
        private static string ToJSON<T>(T item) => throw new System.NotImplementedException();

        public static Build BuildFromFile(string filePath)
        {
            Build newBuild = default;

            LoadAs(&newBuild, filePath);
            return newBuild;
        }

        public static IEnumerable<Build> AllFromDirectory() => Directory.EnumerateFiles(BuildPath, "*.json").Select(BuildFromFile);

        public static void CreateBuildDirectory()
        {
            if (!Directory.Exists(BuildPath))
                Directory.CreateDirectory(BuildPath);
        }
    }
}

using System.IO;
using System.Linq;

namespace Quantum
{
        public static unsafe class Serializer
        {
                public static void Save(Build build) => SaveInternal();
                public static void SaveAs(Build build, string filePath) => SaveInternal(build, filePath);
                private static void SaveInternal(Build build, string filePath)
                {
                        build.SerializableData.LastEdittedDate = System.DateTime.Now.Ticks;
                        string json = ToJSON(build);

                        CreateBuildDirectory();

                        FileMode writeMode = File.Exists(filePath) ? FileMode.Truncate : FileMode.Create;
                        using StreamWriter writer = new(File.Open(filePath, writeMode));
                        writer.Write(json);
                        writer.Flush();

                        Log.Debug($"Saved Build: {build.SerializableData.Name}");
                }

                public static Build Load() => LoadInternal();
                public static Build LoadAs(string filePath) => LoadInternal(filePath);
                private static Build LoadInternal(string filePath)
                {
                        Build build = default;

                        CreateBuildDirectory();

                        if (File.Exists(filePath))
                        {
                                using StreamReader reader = new(File.Open(filePath, FileMode.Open));

                                string json = reader.ReadToEnd();
                                build = FromJSON(json);
                        }

                        Log.Debug($"Loaded Build: {build.SerializableData.Name}");
                }

                public static Build BuildFromFile(string filePath)
                {
                        Build build = default;
                        build.LoadAs(filePath);

                        return build;
                }

                public static IEnumerable<T> AllFromDirectory() => Directory.EnumerateFiles(new T().FilePath(), "*.json").Select(filePath => FromFile(filePath));

                public static void CreateBuildDirectory()
                {
                        if (!Directory.Exists($"{Application.persistentDataPath}/Builds"))
                                Directory.CreateDirectory(t$"{Application.persistentDataPath}/Builds");
                }
        }
}

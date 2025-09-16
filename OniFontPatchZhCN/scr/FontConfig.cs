using System.IO;
using Newtonsoft.Json;

namespace TetsuRemo.OniFontPatchZhCN
{
    public class FontConfig
    {
        public string Title { get; set; }
        public string Head { get; set; }
        public string Description { get; set; } 
        public bool Debug { get; set; } = false;
        public bool EnableTranslationFix { get; set; } = true;

        public static FontConfig Load(string rootPath)
        {
            string path = Path.Combine(rootPath, "config.json");
            FontConfig config = null;

            try
            {
                string json = File.ReadAllText(path);
                config = JsonConvert.DeserializeObject<FontConfig>(json);
            }
            catch { }

            if (config == null)
            {
                config = new FontConfig
                {
                    Title = null,
                    Head = null,
                    Description = null,
                    Debug = false,
                    EnableTranslationFix = true
                };
            }

            // 清理空字符串或 "null" → null
            if (string.IsNullOrWhiteSpace(config.Title) || config.Title == "null")
                config.Title = null;
            if (string.IsNullOrWhiteSpace(config.Head) || config.Head == "null")
                config.Head = null;
            if (string.IsNullOrWhiteSpace(config.Description) || config.Description == "null")
                config.Description = null;

            UnityEngine.Debug.Log($"[OniFontPatchZhCN] FontConfig loaded: " +
                $"Title={config.Title ?? "default"}, " +
                $"Head={config.Head ?? "default"}, " +
                $"Description={config.Description ?? "default"}, " +
                $"Debug={config.Debug}");

            return config;
        }
    }
}

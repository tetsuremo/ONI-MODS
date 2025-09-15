using System.IO;
using Newtonsoft.Json;

namespace TetsuRemo.OniFontPatchZhCN
{
    public class FontConfig
    {
        public string Title { get; set; }
        public string Head { get; set; }
        public string Description { get; set; }
        public bool DescriptionReplace { get; set; }
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
                config = new FontConfig { Title = "", Head = "", Description = "", DescriptionReplace = false, Debug = false };

            UnityEngine.Debug.Log($"[OniFontPatchZhCN] FontConfig loaded: Title={config.Title}, Head={config.Head}, Description={config.Description}, DescriptionReplace={config.DescriptionReplace}, Debug={config.Debug}");

            return config;
        }
    }
}
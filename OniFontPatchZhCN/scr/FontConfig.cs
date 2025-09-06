using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace TetsuRemo.OniFontPatchZhCN
{
    public class FontConfig
    {
        public string Title { get; set; }
        public string Head { get; set; }
        public string Description { get; set; }

        public FontConfig(string Title, string Head, string Description)
        {
            this.Title = Title;
            this.Head = Head;
            this.Description = Description;
        }

        public static FontConfig Load(string rootPath)
        {
            string path = Path.Combine(rootPath, "config.json");
            string json = File.ReadAllText(path);
            var config = JsonConvert.DeserializeObject<FontConfig>(json);
            Debug.Log($"[OniFontPatchZhCN] Font config loaded: Title={config.Title}, Head={config.Head}, Description={config.Description}");
            return config;
        }
    }
}

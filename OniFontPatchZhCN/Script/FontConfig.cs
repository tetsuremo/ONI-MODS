using System;

namespace OniFontPatchZhCN
{
    [Serializable]
    public class FontConfig
    {
        public string Title = "";
        public string Head = "";
        public string Description = "";
        public bool Debug = false;
        public bool EnableTranslationFix = true;
    }
}
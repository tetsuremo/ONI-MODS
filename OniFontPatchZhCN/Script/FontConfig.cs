using System;

namespace OniFontPatchZhCN
{
    [Serializable]
    public class FontConfig
    {
        // 这里的变量名必须和 JSON 里的 Key 完全一致
        public string Title = "";
        public string Head = "";
        public string Description = "";

        // 以下两个虽然目前代码没用到，但留在 JSON 里不会报错
        public bool Debug = false;
        public bool EnableTranslationFix = true;
    }
}
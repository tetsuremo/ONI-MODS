// CustomFontAssets.cs
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace TetsuRemo.OniFontPatchZhCN
{
    public class CustomFontAssets
    {
        public TMP_FontAsset Title { get; private set; }
        public TMP_FontAsset Head { get; private set; }
        public TMP_FontAsset Description { get; private set; }
        public bool DescriptionReplace { get; private set; }

        public CustomFontAssets(TMP_FontAsset title, TMP_FontAsset head, TMP_FontAsset desc, bool descReplace)
        {
            Title = title;
            Head = head;
            Description = desc;
            DescriptionReplace = descReplace;
        }

        public static CustomFontAssets Load(string rootPath, FontConfig config)
        {
            string subdir = Application.platform == RuntimePlatform.WindowsPlayer ? "win" : "other";
            string bundlePath = Path.Combine(rootPath, "assets", subdir, "fonts.bundle");

            var bundle = AssetBundle.LoadFromFile(bundlePath);
            var fonts = bundle.LoadAllAssets().OfType<TMP_FontAsset>().ToList();
            bundle.Unload(false);

            TMP_FontAsset LoadFont(string name)
            {
                if (string.IsNullOrEmpty(name) || name == "null")
                    return null;
                return fonts.FirstOrDefault(f => f.name == name);
            }

            var defaultFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
                .FirstOrDefault(f => f.name == "NotoSansCJKsc-Regular");

            TMP_FontAsset titleFont = LoadFont(config.Title) ?? defaultFont;
            TMP_FontAsset headFont = LoadFont(config.Head) ?? defaultFont;
            TMP_FontAsset descFont = LoadFont(config.Description) ?? defaultFont;

            // 只有当Description不为null且config.DescriptionReplace为true时才真正替换描述字体
            bool descReplace = config.DescriptionReplace && descFont != null;

            UnityEngine.Debug.Log($"[OniFontPatchZhCN] Loaded fonts: Title={titleFont?.name}, Head={headFont?.name}, Desc={descFont?.name}, DescReplace={descReplace}");
            return new CustomFontAssets(titleFont, headFont, descFont, descReplace);
        }
    }
}
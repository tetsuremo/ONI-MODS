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

        public CustomFontAssets(TMP_FontAsset title, TMP_FontAsset head, TMP_FontAsset description)
        {
            Title = title;
            Head = head;
            Description = description;
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
                if (string.IsNullOrEmpty(name))
                    return null;
                return fonts.FirstOrDefault(f => f.name == name);
            }

            // 默认字体：兜底使用中文 NotoSans
            var defaultFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
                .FirstOrDefault(f => f.name == "NotoSansCJKsc-Regular");

            TMP_FontAsset titleFont = LoadFont(config.Title) ?? defaultFont;
            TMP_FontAsset headFont = LoadFont(config.Head) ?? defaultFont;
            TMP_FontAsset descFont = LoadFont(config.Description) ?? defaultFont;

            UnityEngine.Debug.Log($"[OniFontPatchZhCN] Loaded fonts: " +
                $"Title={titleFont?.name}, " +
                $"Head={headFont?.name}, " +
                $"Description={descFont?.name}");

            return new CustomFontAssets(titleFont, headFont, descFont);
        }
    }
}
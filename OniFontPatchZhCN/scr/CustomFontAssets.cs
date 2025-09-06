using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace TetsuRemo.OniFontPatchZhCN
{
    public class CustomFontAssets
    {
        public TMP_FontAsset Title { get; set; }
        public TMP_FontAsset Head { get; set; }
        public TMP_FontAsset Description { get; set; }

        public CustomFontAssets(TMP_FontAsset Title, TMP_FontAsset Head, TMP_FontAsset Description)
        {
            this.Title = Title;
            this.Head = Head;
            this.Description = Description;
        }

        public static CustomFontAssets Load(string rootPath, FontConfig config)
        {
            string subdir = Application.platform == RuntimePlatform.WindowsPlayer ? "win" : "other";
            string bundlePath = Path.Combine(rootPath, "assets", subdir, "fonts.bundle");

            var bundle = AssetBundle.LoadFromFile(bundlePath);
            var fonts = bundle.LoadAllAssets().OfType<TMP_FontAsset>().ToList();
            bundle.Unload(false);

            TMP_FontAsset LoadFont(string name) => string.IsNullOrEmpty(name) ? null : fonts.FirstOrDefault(f => f.name == name);

            var titleFont = LoadFont(config.Title);
            var headFont = LoadFont(config.Head);
            var descFont = LoadFont(config.Description);

            // fallback
            var defaultFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(f => f.name == "NotoSansCJKsc-Regular");
            titleFont ??= defaultFont;
            headFont ??= defaultFont;
            descFont ??= defaultFont;

            Debug.Log($"[OniFontPatchZhCN] Loaded custom fonts: Title={titleFont?.name}, Head={headFont?.name}, Desc={descFont?.name}");
            return new CustomFontAssets(titleFont, headFont, descFont);
        }
    }
}

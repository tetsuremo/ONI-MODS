using System;
using System.IO;
using TMPro;
using UnityEngine;

namespace OniFontPatchZhCN
{
    public static class CustomFontAssets
    {
        public static TMP_FontAsset Title { get; private set; }
        public static TMP_FontAsset Head { get; private set; }
        public static TMP_FontAsset Description { get; private set; }

        public static void Load(string rootPath)
        {
            Debug.Log("[OniFontPatchZhCN] Initializing Font Load...");

            // 1. 检测平台并确定路径
            string platformSuffix = "_win";
            if (Application.platform == RuntimePlatform.OSXPlayer) platformSuffix = "_mac";
            else if (Application.platform == RuntimePlatform.LinuxPlayer) platformSuffix = "_linux";

            string bundleName = "fonts_bundle" + platformSuffix;
            string bundlePath = Path.Combine(rootPath, "assets", bundleName);

            if (!File.Exists(bundlePath))
            {
                Debug.LogError($"[OniFontPatchZhCN] Bundle missing at: {bundlePath}");
                return;
            }

            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null) return;

            try
            {
                Font titleFont = bundle.LoadAsset<Font>("assets/myfonts/mytitlefont.otf");
                Font headFont = bundle.LoadAsset<Font>("assets/myfonts/myheaderfont.otf");
                // 尝试加载描述字体，没有也不要紧
                Font descFont = bundle.LoadAsset<Font>("assets/myfonts/mydescriptionfont.otf");

                if (titleFont != null)
                {
                    Title = TMP_FontAsset.CreateFontAsset(titleFont);
                    Title.name = "CustomTitle";
                }
                if (headFont != null)
                {
                    Head = TMP_FontAsset.CreateFontAsset(headFont);
                    Head.name = "CustomHead";
                }
                if (descFont != null)
                {
                    Description = TMP_FontAsset.CreateFontAsset(descFont);
                    Description.name = "CustomDesc";
                }

                // 如果没有专门的描述字体，统一使用 Head 保证不报错
                if (Description == null) Description = Head ?? Title;
                if (Head == null) Head = Title;

                Debug.Log($"[OniFontPatchZhCN] Fonts Loaded: Title:{Title != null}, Head:{Head != null}");
            }
            finally
            {
                bundle.Unload(false);
            }
        }
    }
}
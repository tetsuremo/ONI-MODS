using System.IO;
using System.Linq;
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
            // 1. 读取配置文件
            string configPath = Path.Combine(rootPath, "config.json");
            FontConfig config = new FontConfig();
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                config = JsonUtility.FromJson<FontConfig>(json) ?? new FontConfig();
            }

            // 2. 运行时检测平台后缀
            string platformSuffix = "_win"; // 默认为 win

            // 使用 Application.platform 在运行时判断
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                platformSuffix = "_mac";
            }
            else if (Application.platform == RuntimePlatform.LinuxPlayer)
            {
                platformSuffix = "_linux";
            }
            // Windows 环境通常不需要额外判断，保持默认即可

            string bundleName = "fonts_bundle" + platformSuffix;
            string bundlePath = Path.Combine(rootPath, "assets", bundleName);

            Debug.Log($"[OniFontPatchZhCN] Current Platform: {Application.platform}, Loading: {bundleName}");

            if (!File.Exists(bundlePath))
            {
                Debug.LogError($"[OniFontPatchZhCN] Bundle missing at: {bundlePath}");
                return;
            }

            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null) return;

            // 1. 加载原始字体
            // 路径必须和你在 BuildMap 里写的一致（小写）
            Font titleFont = bundle.LoadAsset<Font>("assets/myfonts/mytitlefont.otf");
            Font headFont = bundle.LoadAsset<Font>("assets/myfonts/myheaderfont.otf");

            if (titleFont != null)
            {
                // 2. 动态创建资产
                // 这会在游戏运行时根据 OTF 数据生成一个“本地血统”的 TMP_FontAsset
                Title = TMP_FontAsset.CreateFontAsset(titleFont);
                Title.name = "CustomTitle";
                Debug.Log("[OniFontPatchZhCN] Successfully created Title font asset.");
            }
            else
            {
                Debug.LogError("[OniFontPatchZhCN] Failed to load Title font from bundle! Check path: assets/myfonts/mytitlefont.otf");
            }

            if (headFont != null)
            {
                Head = TMP_FontAsset.CreateFontAsset(headFont);
                Head.name = "CustomHead";
                Debug.Log("[OniFontPatchZhCN] Successfully created Head font asset.");
            }

            Debug.Log($"[OniFontPatchZhCN] Dynamic Font Generation: Title:{(Title != null)}, Head:{(Head != null)}");
            bundle.Unload(false); // 释放 bundle，但保留生成的资源
        }
    }
}
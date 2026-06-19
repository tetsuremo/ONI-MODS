using System;
using System.IO;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace OniFontPatchZhCN
{
    public static class CustomFontAssets
    {
        public static AssetBundle FontBundle { get; private set; }
        public static TMP_FontAsset MyHeaderFont { get; private set; }
        public static TMP_FontAsset MyTitleFont { get; private set; }
        public static TMP_FontAsset MyDescriptionFont { get; private set; }
        private static TMP_FontAsset cachedVanillaFont = null;

        // 🌟 核心性能优化：用静态变量缓存状态，杜绝每帧反射带来的卡顿与日志暴击
        public static bool IsZhCN { get; private set; } = false;

        public static void LoadFontBundle(string modPath)
        {
            string bundlePath = Path.Combine(modPath, "assets", FontConfig.GetPlatformBundleName());

            try
            {
                if (!File.Exists(bundlePath))
                {
                    Debug.LogWarning($"[OniFontPatchZhCN] 找不到目标字体包文件: {bundlePath}");
                    return;
                }

                FontBundle = AssetBundle.LoadFromFile(bundlePath);
                if (FontBundle != null)
                {
                    MyHeaderFont = FontBundle.LoadAsset<TMP_FontAsset>(FontConfig.HeaderFontAssetName);
                    MyTitleFont = FontBundle.LoadAsset<TMP_FontAsset>(FontConfig.TitleFontAssetName);
                    MyDescriptionFont = FontBundle.LoadAsset<TMP_FontAsset>(FontConfig.DescriptionFontAssetName);

                    Debug.Log($"[OniFontPatchZhCN] AssetBundle 内部资产加载体检报告:");
                    Debug.Log($" -> HeaderFont 状态: {(MyHeaderFont != null ? $"成功 (真实名称: {MyHeaderFont.name})" : "❌ 失败为 NULL")}");
                    Debug.Log($" -> TitleFont 状态: {(MyTitleFont != null ? $"成功 (真实名称: {MyTitleFont.name})" : "❌ 失败为 NULL")}");
                    Debug.Log($" -> DescriptionFont 状态: {(MyDescriptionFont != null ? $"成功 (真实名称: {MyDescriptionFont.name})" : "❌ 失败为 NULL")}");

                    if (MyHeaderFont == null || MyTitleFont == null || MyDescriptionFont == null)
                    {
                        Debug.LogWarning("[OniFontPatchZhCN] 检测到部分资产未命中，正在为您倾倒 AssetBundle 内所有资产的真实名称列表...");
                        foreach (string assetName in FontBundle.GetAllAssetNames())
                        {
                            Debug.Log($"   -> 发现 Bundle 内部包含资产: {assetName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OniFontPatchZhCN] 加载资产包时发生未知异常: {ex.Message}");
            }
        }

        public static TMP_FontAsset GetVanillaChineseFont()
        {
            if (cachedVanillaFont != null) return cachedVanillaFont;

            try
            {
                var sFontAssetField = AccessTools.Field(typeof(Localization), "sFontAsset");
                if (sFontAssetField != null)
                {
                    cachedVanillaFont = sFontAssetField.GetValue(null) as TMP_FontAsset;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OniFontPatchZhCN] 反射官方原版中文字体失败: {ex.Message}");
            }

            return cachedVanillaFont ?? TMP_Settings.defaultFontAsset;
        }

        // 仅在语言初始化时被触发一次更新
        public static void UpdateLanguageStatus()
        {
            try
            {
                var sLocaleField = AccessTools.Field(typeof(Localization), "sLocale");
                if (sLocaleField != null)
                {
                    var currentLocale = sLocaleField.GetValue(null);
                    if (currentLocale != null)
                    {
                        var langProp = AccessTools.Property(currentLocale.GetType(), "Lang");
                        var codeProp = AccessTools.Property(currentLocale.GetType(), "Code");

                        if (langProp != null && codeProp != null)
                        {
                            var langValue = langProp.GetValue(currentLocale, null).ToString();
                            var codeValue = codeProp.GetValue(currentLocale, null).ToString();

                            Debug.Log($"[OniFontPatchZhCN] 核心语言状态刷新 -> Lang: {langValue}, Code: {codeValue}");

                            IsZhCN = langValue.Contains("Chinese") || codeValue.Equals("zh");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OniFontPatchZhCN] 更新语言状态时发生错误: {ex.Message}");
            }
            IsZhCN = false;
        }
    }
}
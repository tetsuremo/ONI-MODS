using System;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace OniFontPatchZhCN
{
    public static class FinalFontPatcher
    {
        // 挂钩点 1：语言切换或初始化时
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class LocalizationInitializePatch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                CustomFontAssets.UpdateLanguageStatus();

                if (!CustomFontAssets.IsZhCN) return;

                Debug.Log("[OniFontPatchZhCN] [时机锚点 1: Localization.Initialize] 语言初始化，预洗地当前内存资产...");
                ApplyCustomFontSettings();
            }
        }

        // ====================================================================
        // 🚀 挂钩点 2：地毯式无损拦截所有实例化的 LocText 组件
        // 将时机换为安全的 Start()，此时 UI 管道已就绪，绝不触发生命周期崩溃
        // ====================================================================
        [HarmonyPatch(typeof(LocText), "Start")]
        public static class LocTextStartPatch
        {
            [HarmonyPostfix]
            public static void Postfix(LocText __instance)
            {
                // 如果不是中文状态，或者组件本身就是空的，直接放行
                if (!CustomFontAssets.IsZhCN || __instance == null) return;

                try
                {
                    // 安全注入新字体
                    AssignFontToComponent(__instance);
                }
                catch (Exception ex)
                {
                    // 极致防崩兜底
                    Debug.LogError($"[OniFontPatchZhCN] 拦截单个 LocText.Start 时发生异常: {ex.Message}");
                }
            }
        }

        //核心重定向器：根据组件当前的字号，强行注入对应的自定义字体
        public static void AssignFontToComponent(TextMeshProUGUI tmpComponent)
        {
            if (tmpComponent == null) return;

            TMP_FontAsset vanillaFallback = CustomFontAssets.GetVanillaChineseFont();
            TMP_FontAsset chosenFont = null;

            // 字号分流逻辑
            if (tmpComponent.fontSize >= FontConfig.TitleSizeThreshold)
            {
                chosenFont = (CustomFontAssets.MyTitleFont != null) ? CustomFontAssets.MyTitleFont : vanillaFallback;
            }
            else if (tmpComponent.fontSize >= FontConfig.HeaderSizeThreshold)
            {
                chosenFont = (CustomFontAssets.MyHeaderFont != null) ? CustomFontAssets.MyHeaderFont : vanillaFallback;
            }
            else
            {
                // 正文字体未打包，这里会优雅地返回原版 fallback 字体
                chosenFont = (CustomFontAssets.MyDescriptionFont != null) ? CustomFontAssets.MyDescriptionFont : vanillaFallback;
            }

            // 只有当指针不一致时才写入，避免无意义的重复赋值
            if (chosenFont != null && tmpComponent.font != chosenFont)
            {
                tmpComponent.font = chosenFont;
            }
        }

        // 静态资产洗地（保留此方法以兼容游戏其他依赖 TextStyleSetting 的地方）
        public static void ApplyCustomFontSettings()
        {
            try
            {
                TextStyleSetting[] allSettings = Resources.FindObjectsOfTypeAll<TextStyleSetting>();
                if (allSettings == null || allSettings.Length == 0) return;

                TMP_FontAsset vanillaFallback = CustomFontAssets.GetVanillaChineseFont();

                foreach (TextStyleSetting setting in allSettings)
                {
                    if (setting == null) continue;

                    TMP_FontAsset chosenFont = null;

                    if (setting.fontSize >= FontConfig.TitleSizeThreshold)
                    {
                        chosenFont = (CustomFontAssets.MyTitleFont != null) ? CustomFontAssets.MyTitleFont : vanillaFallback;
                    }
                    else if (setting.fontSize >= FontConfig.HeaderSizeThreshold)
                    {
                        chosenFont = (CustomFontAssets.MyHeaderFont != null) ? CustomFontAssets.MyHeaderFont : vanillaFallback;
                    }
                    else
                    {
                        chosenFont = (CustomFontAssets.MyDescriptionFont != null) ? CustomFontAssets.MyDescriptionFont : vanillaFallback;
                    }

                    setting.sdfFont = chosenFont;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OniFontPatchZhCN] 静态重构字体样式时发生未知崩溃: {ex.Message}");
            }
        }
    }
}
using HarmonyLib;
using TMPro;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OniFontPatchZhCN
{
    public static class FinalFontPatcher
    {
        private static bool isPatching = false;

        // --- 双重保险逻辑保持不变 ---
        private static TMP_FontAsset SelectBestFont(TMP_FontAsset originalFont, TextStyleSetting setting)
        {
            if (CustomFontAssets.Description == null) return null;

            string fontName = originalFont != null ? originalFont.name : "";
            string styleName = setting != null ? setting.name.ToLower() : "";

            if (fontName.StartsWith("Graystroke", StringComparison.OrdinalIgnoreCase))
                return CustomFontAssets.Title;

            if (fontName.StartsWith("Economica", StringComparison.OrdinalIgnoreCase))
                return CustomFontAssets.Head;

            if (styleName.Contains("title") || styleName.Contains("header") || styleName.Contains("hover"))
                return CustomFontAssets.Title;

            if (styleName.Contains("body") || styleName.Contains("content") || styleName.Contains("desc"))
                return CustomFontAssets.Description;

            return CustomFontAssets.Description;
        }

        // --- 核心：协程异步刷新 ---
        // 这样可以避开 Unity 6 切换语言时的瞬间内存峰值
        public static IEnumerator ApplyGlobalStylePatchCoroutine()
        {
            if (isPatching) yield break;
            isPatching = true;

            yield return null;

            if (CustomFontAssets.Description == null)
            {
                Debug.LogError("[OniFontPatchZhCN] Fonts are NULL, aborting patch.");
                isPatching = false;
                yield break;
            }

            // --- 新增：在这里调用动态字符修正 ---
            DynamicTranslationFix.ApplyInCodeFixes();

            // 2. 修改样式定义（保持原样）
            TextStyleSetting[] allSettings = Resources.FindObjectsOfTypeAll<TextStyleSetting>();
            foreach (var setting in allSettings)
            {
                if (setting == null) continue;
                setting.sdfFont = SelectBestFont(setting.sdfFont, setting);
            }

            // 3. 分段刷新（保持原样）
            LocText[] allTexts = Resources.FindObjectsOfTypeAll<LocText>();
            int batchCount = 0;
            foreach (var text in allTexts)
            {
                if (text == null || text.gameObject == null) continue;
                if (text.textStyleSetting != null)
                    text.font = text.textStyleSetting.sdfFont;
                else
                    text.font = SelectBestFont(text.font, null);

                batchCount++;
                if (batchCount % 100 == 0) yield return null;
            }

            isPatching = false;
        }

        [HarmonyPatch(typeof(Localization), "SwapToLocalizedFont", new Type[] { typeof(string) })]
        public static class Patch_SwapToLocalizedFont
        {
            public static void Postfix()
            {
                // 使用游戏自带的协程管理器或创建一个临时对象来启动协程
                // 在《缺氧》中，我们可以挂载到主管理器上
                if (Global.Instance != null)
                {
                    Global.Instance.StartCoroutine(ApplyGlobalStylePatchCoroutine());
                }
            }
        }
    }
}
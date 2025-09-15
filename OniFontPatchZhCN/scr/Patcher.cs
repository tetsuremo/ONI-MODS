using HarmonyLib;
using TMPro;
using UnityEngine;
using System.Linq;

namespace TetsuRemo.OniFontPatchZhCN
{
    [HarmonyPatch]
    public static class Patcher
    {
        public static CustomFontAssets Fonts;
        public static bool DebugMode = false;

        // Hook 游戏字体切换
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Localization), nameof(Localization.SwapToLocalizedFont), new System.Type[] { typeof(string) })]
        public static bool SwapLocalizedFontGracefully(string fontname)
        {
            var locale = Localization.GetLocale();
            Log($"SwapLocalizedFontGracefully triggered. Current locale: {locale?.Lang}");

            if (locale == null || locale.Lang > Localization.Language.Chinese)
            {
                Log("Not Chinese locale, skipping font swap.");
                return true; // 调用原方法
            }

            // 替换 TextStyleSetting
            var styles = Resources.FindObjectsOfTypeAll<TextStyleSetting>()
                .Where(s => s?.sdfFont != null)
                .ToList();
            foreach (var style in styles)
            {
                var oldFontName = style.sdfFont.name;
                style.sdfFont = SelectFont(oldFontName);
                Log($"Replaced TextStyleSetting '{style.name}' font: {oldFontName} -> {style.sdfFont?.name}");
            }

            // 替换 LocText
            RefreshLocText();

            Log("Font swap finished.");
            return false; // 跳过原方法
        }

        // 遍历 LocText 并替换字体
        public static void RefreshLocText()
        {
            if (Fonts == null) return;

            var locs = Resources.FindObjectsOfTypeAll<LocText>()
                .Where(t => t?.font != null)
                .ToList();

            Log($"Refreshing {locs.Count} LocText instances...");

            foreach (var text in locs)
            {
                var oldFontName = text.font.name;
                var newFont = SelectFont(oldFontName);
                if (newFont != null)
                {
                    text.font = newFont;
                    Log($"Replaced LocText '{text.name}' font: {oldFontName} -> {text.font?.name}");
                }
            }
        }

        // 根据原字体名选择替换字体
        private static TMP_FontAsset SelectFont(string fontname)
        {
            if (string.IsNullOrEmpty(fontname)) return null;

            if (fontname.StartsWith("GRAYSTROKE") || fontname == Fonts.Title?.name)
                return Fonts.Title;

            if (fontname.StartsWith("Economica") || fontname == Fonts.Head?.name)
                return Fonts.Head;

            // 只有 DescriptionReplace 为 true 才替换 Description
            return Fonts.DescriptionReplace ? Fonts.Description : null;
        }

        // 日志辅助方法
        private static void Log(string message)
        {
            if (DebugMode)
                UnityEngine.Debug.Log("[OniFontPatchZhCN] " + message);
        }
    }
}
using HarmonyLib;
using System.Linq;
using TMPro;
using UnityEngine;

namespace OniFontPatchZhCN
{
    [HarmonyPatch]
    public static class FontPatcher
    {
        public static CustomFontAssets Fonts;
        public static bool DebugMode = false;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Localization), nameof(Localization.SwapToLocalizedFont), new System.Type[] { typeof(string) })]
        public static bool SwapLocalizedFontGracefully(string fontname)
        {
            var locale = Localization.GetLocale();
            Log($"SwapLocalizedFontGracefully triggered. Current locale: {locale?.Lang}");

            if (locale == null || locale.Lang > Localization.Language.Chinese)
            {
                Log("Not Chinese locale, skipping font swap.");
                return true;
            }

            var styles = Resources.FindObjectsOfTypeAll<TextStyleSetting>()
                .Where(s => s?.sdfFont != null)
                .ToList();

            foreach (var style in styles)
            {
                var oldFontName = style.sdfFont.name;
                style.sdfFont = SelectFont(oldFontName);
                Log($"Replaced TextStyleSetting '{style.name}' font: {oldFontName} -> {style.sdfFont?.name}");
            }

            RefreshLocText();
            Log("Font swap finished.");
            return false;
        }

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

        private static TMP_FontAsset SelectFont(string fontname)
        {
            if (string.IsNullOrEmpty(fontname)) return Fonts.Description;
            if (fontname.StartsWith("GRAYSTROKE") || fontname == Fonts.Title?.name) return Fonts.Title;
            if (fontname.StartsWith("Economica") || fontname == Fonts.Head?.name) return Fonts.Head;
            return Fonts.Description;
        }

        private static void Log(string message)
        {
            if (DebugMode) Debug.Log("[OniFontPatchZhCN] " + message);
        }
    }
}

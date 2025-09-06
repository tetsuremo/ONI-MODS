using System;
using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace TetsuRemo.OniFontPatchZhCN
{
    [HarmonyPatch]
    public static class Patcher
    {
        public static CustomFontAssets Fonts;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Localization), "SwapToLocalizedFont", new Type[] { typeof(string) })]
        public static bool SwapLocalizedFontGracefully(string fontname)
        {
            try
            {
                var locale = Localization.GetLocale();
                Debug.Log($"[OniFontPatchZhCN] SwapLocalizedFontGracefully triggered. Current locale: {locale?.Lang}");
                if (locale == null || locale.Lang != Localization.Language.Chinese)
                {
                    Debug.Log("[OniFontPatchZhCN] Not Chinese locale, skipping font swap.");
                    return true;
                }

                foreach (var style in Resources.FindObjectsOfTypeAll<TextStyleSetting>().Where(s => s?.sdfFont != null))
                {
                    style.sdfFont = SelectFont(style.sdfFont.name);
                }

                foreach (var text in Resources.FindObjectsOfTypeAll<LocText>().Where(t => t?.font != null))
                {
                    text.font = SelectFont(text.font.name);
                }

                return false; // skip original method
            }
            catch (Exception e)
            {
                Debug.LogError($"[OniFontPatchZhCN] Error in SwapLocalizedFontGracefully: {e}");
                return true;
            }
        }

        private static TMP_FontAsset SelectFont(string fontname)
        {
            if (string.IsNullOrEmpty(fontname)) return null;
            if (fontname.StartsWith("GRAYSTROKE") && Fonts.Title != null) return Fonts.Title;
            if (fontname.StartsWith("Economica") && Fonts.Head != null) return Fonts.Head;
            return Fonts.Description;
        }
    }
}

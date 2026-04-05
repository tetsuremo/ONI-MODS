using HarmonyLib;
using TMPro;

namespace OniFontPatchZhCN
{
    [HarmonyPatch(typeof(LocText), "OnEnable")]
    public static class FinalFontPatcher
    {
        [HarmonyPrefix]
        public static void Prefix(LocText __instance)
        {
            if (__instance == null) return;

            string styleName = (__instance.textStyleSetting != null)
                ? __instance.textStyleSetting.name.ToLower()
                : "no_style";

            TMP_FontAsset target = null;

            if (styleName.Contains("body") || styleName.Contains("content") || styleName.Contains("desc"))
                target = CustomFontAssets.Description;
            else if (styleName.Contains("title") || styleName.Contains("header") || styleName.Contains("hover"))
                target = CustomFontAssets.Title;
            else
                target = CustomFontAssets.Head;

            if (target != null)
            {
                __instance.font = target;
            }
        }
    }
}
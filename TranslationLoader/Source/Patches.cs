using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TranslationLoaderLite
{
    [HarmonyPatch]
    public static class OptionsDialogPatches
    {
        [HarmonyPatch("PeterHan.PLib.Options.OptionsDialog", "OnManualConfig")]
        [HarmonyPrefix]
        public static bool OnManualConfigPrefix(object __instance)
        {
            if (IsOurModOptionsDialog(__instance))
            {
                Utils.OpenPoDirectory();
                return false;
            }
            return true;
        }

        [HarmonyPatch("PeterHan.PLib.Options.OptionsDialog", "OnResetConfig")]
        [HarmonyPrefix]
        public static bool OnResetConfigPrefix(object __instance)
        {
            if (IsOurModOptionsDialog(__instance))
            {
                Utils.GenerateBlankTemplates(force: true);
                return false;
            }
            return true;
        }

        private static bool IsOurModOptionsDialog(object dialogInstance)
        {
            try
            {
                var optionsField = dialogInstance.GetType().GetField("options", BindingFlags.Instance | BindingFlags.NonPublic);
                if (optionsField != null)
                {
                    var optionsValue = optionsField.GetValue(dialogInstance);
                    return optionsValue is TranslationLoaderOptions;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TranslationLoaderLite] Error checking options dialog: {ex}");
            }
            return false;
        }
    }

    // Harmony Patch 拦截 PLib 注册翻译
    [HarmonyPatch(typeof(Localization), "RegisterForTranslation")]
    public static class Localization_RegisterForTranslation_Patch
    {
        [HarmonyPriority(-2147483648)] // 最低优先级，确保在其他 mod 注册后执行
        public static void Postfix()
        {
            if (Utils.translations != null && Utils.translations.Count > 0)
            {
                Debug.Log("[TranslationLoaderLite] Overriding translations after RegisterForTranslation");
                Localization.OverloadStrings(Utils.translations);
            }
        }
    }

    // Harmony Patch 拦截 OverloadStrings 调用，确保翻译覆盖
    [HarmonyPatch(typeof(Localization), "OverloadStrings")]
    [HarmonyPatch(new Type[] { typeof(Dictionary<string, string>) })]
    public static class Localization_OverloadStrings_Patch
    {
        [HarmonyPriority(-2147483648)]
        public static void Prefix(ref Dictionary<string, string> translated_strings)
        {
            if (Utils.translations == null || Utils.translations.Count == 0) return;
            if (Utils.translations == translated_strings) return;
            foreach (var kv in Utils.translations)
                translated_strings[kv.Key] = kv.Value;
        }
    }
}

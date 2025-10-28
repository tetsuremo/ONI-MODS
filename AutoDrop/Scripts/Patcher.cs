using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace AutoDrop
{
    [HarmonyPatch(typeof(Localization), "Initialize")]
    public static class Localization_Initialize_Patch
    {
        public static void Postfix()
        {
            try
            {
                Localization.RegisterForTranslation(typeof(STRINGS));
                LocString.CreateLocStringKeys(typeof(STRINGS), null);

                Debug.Log("[AutoDrop] Localization registered successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AutoDrop] Error in Localization.Initialize postfix: {e}");
            }
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class Db_Initialize_Patch
    {
        private static bool alreadyRegistered = false;

        public static void Postfix()
        {
            if (alreadyRegistered) return;
            alreadyRegistered = true;

            try
            {
                Debug.Log("[AutoDrop] Db.Initialize Postfix running.");

                // 加载翻译并手动覆盖字符串
                LoadAndApplyTranslations();

                Debug.Log("[AutoDrop] Translation strings applied successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[AutoDrop] Error in Db.Initialize postfix: {e}");
            }
        }

        private static void LoadAndApplyTranslations()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string translationsDir = Path.Combine(directoryName, "translations");

            Localization.Locale locale = Localization.GetLocale();
            string localeCode = (locale != null) ? locale.Code : "en";
            string poPath = Path.Combine(translationsDir, localeCode + ".po");

            if (!File.Exists(poPath))
            {
                Debug.Log($"[AutoDrop] Translation file not found: {poPath}");
                return;
            }

            try
            {
                var translations = Localization.LoadStringsFile(poPath, false);

                // 只应用我们需要的翻译
                var modTranslations = new Dictionary<string, string>();

                foreach (var translation in translations)
                {
                    if (translation.Key.Contains("AUTODROP"))
                    {
                        modTranslations[translation.Key] = translation.Value;
                        Debug.Log($"[AutoDrop] Found mod translation: {translation.Key} -> {translation.Value}");
                    }
                }

                if (modTranslations.Count > 0)
                {
                    // 手动覆盖字符串值
                    OverrideLocStrings(modTranslations);
                    Debug.Log($"[AutoDrop] Applied {modTranslations.Count} mod translations from {poPath}");
                }
                else
                {
                    Debug.LogWarning($"[AutoDrop] No AUTODROP translations found in {poPath}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[AutoDrop] Exception while loading translations: {e}");
            }
        }

        private static void OverrideLocStrings(Dictionary<string, string> translations)
        {
            // 手动覆盖 STRINGS 类的值
            if (translations.TryGetValue("STRINGS.UI.USERMENUACTIONS.AUTODROP_ENABLE.NAME", out var enableName))
            {
                STRINGS.UI.USERMENUACTIONS.AUTODROP_ENABLE.NAME = enableName;
                Debug.Log($"[AutoDrop] Overridden AUTODROP_ENABLE.NAME: {enableName}");
            }

            if (translations.TryGetValue("STRINGS.UI.USERMENUACTIONS.AUTODROP_ENABLE.TOOLTIP", out var enableTooltip))
            {
                STRINGS.UI.USERMENUACTIONS.AUTODROP_ENABLE.TOOLTIP = enableTooltip;
                Debug.Log($"[AutoDrop] Overridden AUTODROP_ENABLE.TOOLTIP: {enableTooltip}");
            }

            if (translations.TryGetValue("STRINGS.UI.USERMENUACTIONS.AUTODROP_DISABLE.NAME", out var disableName))
            {
                STRINGS.UI.USERMENUACTIONS.AUTODROP_DISABLE.NAME = disableName;
                Debug.Log($"[AutoDrop] Overridden AUTODROP_DISABLE.NAME: {disableName}");
            }

            if (translations.TryGetValue("STRINGS.UI.USERMENUACTIONS.AUTODROP_DISABLE.TOOLTIP", out var disableTooltip))
            {
                STRINGS.UI.USERMENUACTIONS.AUTODROP_DISABLE.TOOLTIP = disableTooltip;
                Debug.Log($"[AutoDrop] Overridden AUTODROP_DISABLE.TOOLTIP: {disableTooltip}");
            }
        }
    }
    // 气体装瓶器补丁
    [HarmonyPatch(typeof(GasBottlerConfig))]
    public static class GasBottlerPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static void AddAutoDropComponent(GameObject go)
        {
            // 原有逻辑保持不变
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateBuildingDef")]
        public static void AdjustBuildingDef(ref BuildingDef __result)
        {
            __result.Floodable = false;
            __result.Entombable = false;
        }
    }

    // 液体装瓶器补丁
    [HarmonyPatch(typeof(LiquidBottlerConfig))]
    public static class LiquidBottlerPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("ConfigureBuildingTemplate")]
        public static void AddAutoDropComponent(GameObject go)
        {
            // 原有逻辑保持不变
        }

        [HarmonyPostfix]
        [HarmonyPatch("CreateBuildingDef")]
        public static void AdjustBuildingDef(ref BuildingDef __result)
        {
            __result.Floodable = false;
            __result.Entombable = false;
        }

        [HarmonyPatch(typeof(Assets))]
        [HarmonyPatch("AddBuildingDef")]
        public static void Prefix(ref BuildingDef def)
        {
            GameObject buildingComplete = def.BuildingComplete;
            bool flag = buildingComplete.GetComponent<LiquidBottlerConfig>() == null && buildingComplete.GetComponent<Bottler>() == null && buildingComplete.GetComponent<GasBottlerConfig>() == null;
            if (!flag)
            {
                buildingComplete.AddComponent<AutoDropToggle>();
            }
        }
    }
}
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace BatchModifyStyles
{
    public static class BatchSkinPatches
    {
        private static bool alreadyRegistered = false;
        private static Dictionary<string, string> translationDict = new Dictionary<string, string>();

        public static void ApplyAll(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(Localization), "Initialize"),
                postfix: new HarmonyMethod(typeof(BatchSkinPatches), nameof(Localization_Initialize_Patch_Postfix))
            );

            harmony.Patch(
                AccessTools.Method(typeof(PlayerController), "OnPrefabInit"),
                postfix: new HarmonyMethod(typeof(BatchSkinPatches), nameof(PlayerController_OnPrefabInit))
            );

            harmony.Patch(
                AccessTools.Method(typeof(Db), "Initialize"),
                postfix: new HarmonyMethod(typeof(BatchSkinPatches), nameof(Db_Initialize_Patch_Postfix))
            );

            // 应用蓝图按钮补丁
            harmony.Patch(
                AccessTools.Method(typeof(DetailsScreen), "SelectSideScreenTab"),
                postfix: new HarmonyMethod(typeof(BlueprintButtonPatch), nameof(BlueprintButtonPatch.Postfix))
            );
        }

        #region 翻译补丁

        public static void Localization_Initialize_Patch_Postfix()
        {
            try
            {
                Localization.RegisterForTranslation(typeof(STRINGS));
                LocString.CreateLocStringKeys(typeof(STRINGS), null);
                Log.Info("[BatchModifyStyles] Default English LocStrings registered.");
            }
            catch (Exception e)
            {
                Log.Error($"[BatchModifyStyles] Error in Localization.Initialize postfix: {e}");
            }
        }

        public static void Db_Initialize_Patch_Postfix()
        {
            if (alreadyRegistered) return;
            alreadyRegistered = true;

            try
            {
                Log.Info("[BatchModifyStyles] Loading translations");

                var translations = LoadTranslations();
                RefreshLocStrings(new string[]
                {
            "STRINGS.UI.BATCH_MODIFY_SKINS.BATCH_MODIFY_SKIN",
            "STRINGS.UI.BATCH_MODIFY_SKINS.BATCH_SKIN_TOOLTIP",
            "STRINGS.UI.BATCH_MODIFY_SKINS.APPLIED_TO_TARGETS"
                }, translations);

            }
            catch (Exception e)
            {
                Log.Error($"[BatchModifyStyles] Translation load failed: {e}");
            }
        }

        private static Dictionary<string, string> LoadTranslations()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string translationsDir = Path.Combine(directoryName, "translations");

            Localization.Locale locale = Localization.GetLocale();
            string localeCode = locale != null ? locale.Code : "en";
            string poPath = Path.Combine(translationsDir, localeCode + ".po");

            var dict = new Dictionary<string, string>();

            if (!File.Exists(poPath))
            {
                Log.Info($"[BatchModifyStyles] Translation file not found: {poPath}");
                return dict;
            }

            try
            {
                var translations = Localization.LoadStringsFile(poPath, false);
                Localization.OverloadStrings(translations);

                foreach (var kvp in translations)
                {
                    dict[kvp.Key] = kvp.Value;
                    translationDict[kvp.Key] = kvp.Value;
                }

                Log.Info($"[BatchModifyStyles] OverloadStrings executed for {localeCode}. Loaded {translations.Count} entries.");
            }
            catch (Exception e)
            {
                Log.Error($"[BatchModifyStyles] Exception while loading translations: {e}");
            }

            return dict;
        }

        private static void RefreshLocStrings(string[] keys, Dictionary<string, string> translationDict)
        {
            int refreshedCount = 0;
            foreach (var key in keys)
            {
                if (Strings.TryGet(key, out var entry))
                {
                    if (translationDict.TryGetValue(key, out var translated))
                    {
                        entry.String = translated;
                        refreshedCount++;
                        Log.Info($"[BatchModifyStyles] Refreshed LocString for: {key} -> {translated}");
                    }
                    else
                    {
                        Log.Warn($"[BatchModifyStyles] Translation not found for key: {key}");
                    }
                }
                else
                {
                    Log.Warn($"[BatchModifyStyles] LocString key not found: {key}");
                }
            }
            Log.Info($"[BatchModifyStyles] Total refreshed LocStrings: {refreshedCount}/{keys.Length}");
        }

        #endregion

        #region 工具注册

        public static void PlayerController_OnPrefabInit(PlayerController __instance)
        {
            if (BatchSkinTool.Instance == null)
            {
                var go = new GameObject("BatchSkinTool");
                var tool = go.AddComponent<BatchSkinTool>();

                if (__instance.tools != null)
                {
                    var toolsList = new List<InterfaceTool>(__instance.tools);
                    toolsList.Add(tool);
                    __instance.tools = toolsList.ToArray();
                    Log.Info($"[BatchModifyStyles] Tool registered. Total tools: {__instance.tools.Length}");
                }
                else
                {
                    Log.Error("[BatchModifyStyles] PlayerController tools array is null");
                }

                Log.Info("[BatchModifyStyles] Batch skin tool instance created and registered");
            }
            else
            {
                Log.Info("[BatchModifyStyles] Tool instance already exists");
            }
        }

        // 辅助方法：获取翻译后的字符串
        private static string GetTranslatedString(string key, string fallback)
        {
            if (translationDict.TryGetValue(key, out var translated))
            {
                return translated;
            }
            return fallback;
        }

        #endregion
    }
}

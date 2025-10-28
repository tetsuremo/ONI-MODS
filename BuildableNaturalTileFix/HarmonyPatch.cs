using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BuildableNaturalTileFix
{
    // ============================
    // 材料选择器扩展
    // ============================
    [HarmonyPatch(typeof(MaterialSelector), "GetValidMaterials")]
    public static class MaterialSelector_GetValidMaterials_Patch
    {
        public static void Postfix(Tag _materialTypeTag, bool omitDisabledElements, ref List<Tag> __result)
        {
            // 为我们的建筑扩展材料选择（针对 BuildableAny 标签）
            if (_materialTypeTag == "BuildableAny")
            {
                int originalCount = __result.Count;

                // 获取所有固体元素
                var allSolidElements = new List<Tag>();
                foreach (var element in ElementLoader.elements)
                {
                    if (element.IsSolid && (!element.disabled || !omitDisabledElements))
                    {
                        allSolidElements.Add(element.tag);
                    }
                }

                // 找出缺失的材料并添加
                int addedCount = 0;
                foreach (var tag in allSolidElements)
                {
                    if (!__result.Contains(tag))
                    {
                        __result.Add(tag);
                        addedCount++;
                    }
                }

                // 只记录总结信息，不逐个列出
                if (addedCount > 0)
                {
                    Debug.Log($"[BNTFix] Extended BuildableAny: {originalCount} -> {__result.Count} materials (+{addedCount})");
                }
            }
        }
    }

    // ============================
    // 语言初始化
    // ============================
    [HarmonyPatch(typeof(Localization), "Initialize")]
    public static class Localization_Initialize_Patch
    {
        public static void Postfix()
        {
            try
            {
                Localization.RegisterForTranslation(typeof(STRINGS));
                LocString.CreateLocStringKeys(typeof(STRINGS), null);
            }
            catch (Exception e)
            {
                Debug.LogError($"[BNTFix] Error in Localization.Initialize postfix: {e}");
            }
        }
    }

    // ============================
    // 数据库初始化（注册建筑 + 翻译加载）
    // ============================
    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class Db_Initialize_Patch
    {
        private static bool alreadyRegistered = false;
        private const string BUILDING_ID = NaturalTileConfig.ID;

        public static void Postfix()
        {
            if (alreadyRegistered) return;
            alreadyRegistered = true;

            try
            {
                Debug.Log("[BNTFix] Db.Initialize Postfix running.");

                // 注册建筑
                ModUtil.AddBuildingToPlanScreen("Base", BUILDING_ID, "MeshTile");
                Tech tech = Db.Get().Techs.Get("FarmingTech");
                if (tech != null && !tech.unlockedItemIDs.Contains(BUILDING_ID))
                    tech.unlockedItemIDs.Add(BUILDING_ID);

                Debug.Log("[BNTFix] Building registered successfully (Db.Initialize).");

                // 翻译加载
                var translationDict = LoadTranslations();

                // 创建 LocString
                LocString.CreateLocStringKeys(typeof(STRINGS), null);
                Debug.Log("[BNTFix] LocString keys created after OverloadStrings.");

                RefreshLocStrings(new string[]
                {
                    "STRINGS.BUILDINGS.PREFABS.BUILDABLENATURALTILEFIX_NATURALTILE.NAME",
                    "STRINGS.BUILDINGS.PREFABS.BUILDABLENATURALTILEFIX_NATURALTILE.DESC",
                    "STRINGS.BUILDINGS.PREFABS.BUILDABLENATURALTILEFIX_NATURALTILE.EFFECT"
                }, translationDict);
            }
            catch (Exception e)
            {
                Debug.LogError($"[BNTFix] Error in Db.Initialize postfix: {e}");
            }
        }

        private static Dictionary<string, string> LoadTranslations()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string translationsDir = Path.Combine(directoryName, "translations");

            Localization.Locale locale = Localization.GetLocale();
            string localeCode = (locale != null) ? locale.Code : "en";
            string poPath = Path.Combine(translationsDir, localeCode + ".po");

            var dict = new Dictionary<string, string>();
            if (!File.Exists(poPath))
                return dict;

            try
            {
                var translations = Localization.LoadStringsFile(poPath, false);
                Localization.OverloadStrings(translations);

                foreach (var kvp in translations)
                    dict[kvp.Key] = kvp.Value;

                Debug.Log($"[BNTFix] OverloadStrings executed.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[BNTFix] Exception while loading translations: {e}");
            }

            return dict;
        }

        private static void RefreshLocStrings(string[] keys, Dictionary<string, string> translationDict)
        {
            foreach (var key in keys)
            {
                if (Strings.TryGet(key, out var entry))
                {
                    if (translationDict.TryGetValue(key, out var translated))
                        entry.String = translated;
                    else
                        Debug.LogWarning($"[BNTFix] Translation not found for key: {key}");
                }
                else
                {
                    Debug.LogWarning($"[BNTFix] LocString key not found: {key}");
                }
            }
        }
    }

    // ============================
    // 建筑生成时（核心逻辑）
    // ============================
    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public static class BuildingComplete_OnSpawn_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(BuildingComplete __instance)
        {
            if (__instance == null) return;

            try
            {
                var kpid = __instance.GetComponent<KPrefabID>();
                if (kpid == null || kpid.PrefabTag.ToString() != NaturalTileConfig.ID)
                    return;

                int cell = Grid.PosToCell(__instance.gameObject);
                var primary = __instance.GetComponent<PrimaryElement>();
                if (primary == null || primary.Element == null || !Grid.IsValidCell(cell))
                    return;

                // 获取建造时选用的材料、温度
                SimHashes elementID = primary.ElementID;
                float temperature = primary.Temperature;

                // ✅ 使用配置文件中的 BlockMass 替换最终生成的方块质量
                float targetMass = ConfigOptions.Instance.BlockMass;

                // 替换格子内的元素
                SimMessages.ReplaceAndDisplaceElement(
                    cell,
                    elementID,
                    null,
                    targetMass,
                    temperature,
                    byte.MaxValue,
                    0,
                    -1
                );

                // 删除临时建筑
                UnityEngine.Object.Destroy(__instance.gameObject);

                Debug.Log($"[BNTFix] Natural tile placed (Element={elementID}, Mass={targetMass}).");
            }
            catch (Exception e)
            {
                Debug.LogError($"[BNTFix] Error in BuildingComplete.OnSpawn: {e}");
            }
        }
    }
}
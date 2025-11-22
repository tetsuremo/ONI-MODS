using HarmonyLib;
using Klei.CustomSettings;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro; // 必须引用 Unity.TextMeshPro.dll

namespace OniFontPatchZhCN
{
    public static class GameTranslationFixer
    {
        private static bool initialized = false;
        private static Dictionary<string, string> translationDictionary;

        public static void Initialize(string poFilePath)
        {
            if (initialized) return;

            try
            {
                translationDictionary = TranslationLoader.LoadPoFile(poFilePath);

                // 1. 应用 Harmony 补丁 (视觉层拦截)
                // 这是解决 Link 显示英文和 Min/Max 的核心
                try
                {
                    var harmony = new Harmony("OniFontPatchZhCN.FinalVisualFix");
                    harmony.PatchAll();
                    Debug.Log("[OniFontPatchZhCN] Visual Intercept patches applied.");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[OniFontPatchZhCN] Harmony patching error: {ex}");
                }

                // 2. 启动协程处理 Db (解决生物材料、房间等需要等待数据库加载的内容)
                var go = new GameObject("TranslationFixer");
                UnityEngine.Object.DontDestroyOnLoad(go);
                var fixer = go.AddComponent<TranslationFixerMono>();
                fixer.Initialize(translationDictionary);

                initialized = true;
                Debug.Log("[OniFontPatchZhCN] Fixer Initialized (Full Version).");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OniFontPatchZhCN] Initialize Critical Failure: {ex}");
            }
        }

        // ----------------------------------------------------------------------
        // 1. 视觉层终极拦截：TMPro.TMP_Text
        // ----------------------------------------------------------------------
        [HarmonyPatch]
        public static class TMProInterceptPatches
        {
            // 拦截 text 属性的赋值。这是游戏显示的最后一公里。
            [HarmonyPatch(typeof(TMP_Text), "text", MethodType.Setter)]
            [HarmonyPrefix]
            public static void Intercept_TMP_SetText(ref string value)
            {
                if (string.IsNullOrEmpty(value)) return;

                // --- 1. 修复复制人相关 (链接/文本) ---
                if (value.Contains("Duplicant") || value.Contains("Species") || value.Contains("Standard") || value.Contains("Bionic"))
                {
                    // 修复完整短语
                    if (value.Contains("Standard Duplicant")) value = value.Replace("Standard Duplicant", "标准复制人");
                    if (value.Contains("Bionic Duplicant")) value = value.Replace("Bionic Duplicant", "仿生复制人");

                    // 修复带链接的单独单词
                    if (value.Contains("<link=\"DUPLICANTS\">Standard</link>"))
                        value = value.Replace("<link=\"DUPLICANTS\">Standard</link>", "<link=\"DUPLICANTS\">标准</link>");
                    if (value.Contains("<link=\"DUPLICANTS\">Bionic</link>"))
                        value = value.Replace("<link=\"DUPLICANTS\">Bionic</link>", "<link=\"DUPLICANTS\">仿生</link>");

                    // 修复前缀 "Species: "
                    if (value.Contains("Species: ")) value = value.Replace("Species: ", "物种：");
                }

                // --- 2. 修复 Minimum/Maximum ---
                if (value.Contains("Minimum")) value = value.Replace("Minimum", "最小");
                if (value.Contains("Maximum")) value = value.Replace("Maximum", "最大");
            }

            // 拦截 SetText 方法 (防止漏网)
            [HarmonyPatch(typeof(TMP_Text), "SetText", new Type[] { typeof(string), typeof(bool) })]
            [HarmonyPrefix]
            public static void Intercept_TMP_SetTextMethod(ref string text)
            {
                Intercept_TMP_SetText(ref text);
            }
        }

        // ----------------------------------------------------------------------
        // 2. 延迟修复逻辑 (生物材料、房间、设置)
        // ----------------------------------------------------------------------
        private class TranslationFixerMono : MonoBehaviour
        {
            private Dictionary<string, string> translations;

            public void Initialize(Dictionary<string, string> translationDict)
            {
                translations = translationDict;
                StartCoroutine(DelayedFixRoutine());
            }

            private IEnumerator DelayedFixRoutine()
            {
                // 等待几帧确保环境就绪
                yield return null;
                yield return null;

                // 等待数据库加载完成
                while (Db.Get() == null || Db.Get().traits == null) yield return null;

                Debug.Log("[OniFontPatchZhCN] Applying database-dependent fixes...");

                // 执行各个子系统的修复
                ApplyMaterialHardcodedTranslations(); // 生物材料 (你特别要求的)
                FixGameSettings();                    // 游戏设置
                FixRoomConstraints();                 // 房间要求
                FixTraits();                          // 特质
            }

            // --- 恢复：生物生长率硬编码翻译 ---
            private void ApplyMaterialHardcodedTranslations()
            {
                try
                {
                    // 1. 生长类型名称
                    if (CREATURES.STATS.SCALEGROWTH.DISPLAYED_NAME != null)
                    {
                        var dName = CREATURES.STATS.SCALEGROWTH.DISPLAYED_NAME;
                        dName["Drecko"] = "鳞片生长";
                        dName["DreckoPlastic"] = "鳞片生长";
                        dName["WoodDeer"] = "鹿角生长";
                        dName["GlassDeer"] = "鹿角生长";
                        dName["IceBelly"] = "毛发生长";
                        dName["GoldBelly"] = "冠羽生长";
                        dName["Raptor"] = "羽毛生长";
                    }

                    // 2. 产物提示前缀
                    if (CREATURES.STATS.SCALEGROWTH.TOOLTIP_PREFIX != null)
                    {
                        var tPrefix = CREATURES.STATS.SCALEGROWTH.TOOLTIP_PREFIX;
                        tPrefix["Drecko"] = "芦苇纤维";
                        tPrefix["DreckoPlastic"] = "塑料";
                        tPrefix["WoodDeer"] = "木材";
                        tPrefix["GlassDeer"] = "玻璃";
                        tPrefix["IceBelly"] = "芦苇纤维";
                        tPrefix["GoldBelly"] = "皇犸兔头冠";
                        tPrefix["Raptor"] = "羽毛纤维";
                    }
                    Debug.Log("[OniFontPatchZhCN] Creature material translations applied.");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[OniFontPatchZhCN] Failed to fix materials: {ex}");
                }
            }

            // --- 游戏设置修复 ---
            private void FixGameSettings()
            {
                try
                {
                    var fields = typeof(CustomGameSettingConfigs).GetFields(BindingFlags.Static | BindingFlags.Public);
                    foreach (var field in fields) { try { UpdateSettingConfig(field.GetValue(null)); } catch { } }
                }
                catch { }
            }

            private bool UpdateSettingConfig(object setting)
            {
                if (setting == null) return false;
                try
                {
                    var traverse = Traverse.Create(setting);
                    string label = traverse.Field("<label>k__BackingField")?.GetValue<string>();
                    string tooltip = traverse.Field("<tooltip>k__BackingField")?.GetValue<string>();
                    if (!string.IsNullOrEmpty(label) && translations.TryGetValue(label, out string tLabel)) traverse.Field("<label>k__BackingField").SetValue(tLabel);
                    if (!string.IsNullOrEmpty(tooltip) && translations.TryGetValue(tooltip, out string tTooltip)) traverse.Field("<tooltip>k__BackingField").SetValue(tTooltip);

                    if (setting is ToggleSettingConfig toggleConfig)
                    {
                        UpdateSettingLevel(toggleConfig.on_level);
                        UpdateSettingLevel(toggleConfig.off_level);
                    }
                    if (setting is ListSettingConfig listConfig)
                    {
                        var levels = listConfig.GetLevels();
                        if (levels != null) foreach (var level in levels) UpdateSettingLevel(level);
                    }
                    return true;
                }
                catch { return false; }
            }

            private void UpdateSettingLevel(SettingLevel level)
            {
                if (level == null) return;
                try
                {
                    var traverse = Traverse.Create(level);
                    string name = traverse.Field("<name>k__BackingField")?.GetValue<string>();
                    string tooltip = traverse.Field("<tooltip>k__BackingField")?.GetValue<string>();
                    if (!string.IsNullOrEmpty(name) && translations.TryGetValue(name, out string tName)) traverse.Field("<name>k__BackingField").SetValue(tName);
                    if (!string.IsNullOrEmpty(tooltip) && translations.TryGetValue(tooltip, out string tTooltip)) traverse.Field("<tooltip>k__BackingField").SetValue(tTooltip);
                }
                catch { }
            }

            // --- 房间要求修复 ---
            private void FixRoomConstraints()
            {
                try
                {
                    var fields = typeof(RoomConstraints).GetFields(BindingFlags.Static | BindingFlags.Public);
                    foreach (var field in fields)
                    {
                        var constraint = field.GetValue(null) as RoomConstraints.Constraint;
                        if (constraint != null)
                        {
                            if (!string.IsNullOrEmpty(constraint.name) && translations.TryGetValue(constraint.name, out string tName)) constraint.name = tName;
                            if (!string.IsNullOrEmpty(constraint.description) && translations.TryGetValue(constraint.description, out string tDesc)) constraint.description = tDesc;
                        }
                    }
                }
                catch { }
            }

            // --- 特质修复 ---
            private void FixTraits()
            {
                try
                {
                    var traits = Db.Get().traits;
                    foreach (var trait in traits.resources)
                    {
                        if (trait == null) continue;
                        if (translations.TryGetValue(trait.Name, out string tName)) trait.Name = tName;
                        if (!string.IsNullOrEmpty(trait.description) && translations.TryGetValue(trait.description, out string tDesc))
                            trait.description = tDesc;
                    }
                }
                catch { }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using HarmonyLib;
using System.IO;

namespace TModLib
{
    public static class TranslationHub
    {
        // 存储强制覆盖的词条：Key (如 "STRINGS.BUILDINGS.PREFABS.TILE.NAME") -> Value
        private static readonly Dictionary<string, string> priorityTable = new Dictionary<string, string>();

        /// <summary>
        /// 供其他 Mod 调用，注册需要暴力覆盖的词条
        /// </summary>
        public static void RegisterPriorityTranslation(Dictionary<string, string> translations)
        {
            foreach (var kvp in translations)
            {
                priorityTable[kvp.Key] = kvp.Value;
            }
        }

        internal static void ApplyPriorityOverrides()
        {
            if (priorityTable.Count == 0) return;

            foreach (var kvp in priorityTable)
            {
                // 暴力强制写入 StringTable
                // 即使其他 Mod 已经写过了，这里也会因为 Key 相同而覆盖
                Strings.Add(new StringEntry(kvp.Key, kvp.Value));
            }

            Debug.Log($"[TModLib] 已暴力覆盖 {priorityTable.Count} 条翻译条目。");
        }
    }

    [HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
    public static class Localization_Initialize_Patch
    {
        // 在游戏完成标准初始化（包括加载所有 Mod 的 po 文件）后执行
        public static void Postfix()
        {
            TranslationHub.ApplyPriorityOverrides();
        }
    }
}
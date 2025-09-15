using HarmonyLib;
using Klei.AI;
using Klei.CustomSettings;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TranslateFixMod
{

    // 模拟自定义游戏设置
    public class MockCustomGameSettingConfigs
    {
        public static SeedSettingConfig WorldgenSeed = new SeedSettingConfig(
            "WorldgenSeed",
            UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLDGEN_SEED.NAME,
            UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLDGEN_SEED.TOOLTIP,
            false, false
        );

        public static ListSettingConfig ClusterLayout = new ListSettingConfig(
            "ClusterLayout",
            UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CLUSTER_CHOICE.NAME,
            UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CLUSTER_CHOICE.TOOLTIP,
            null, null, null, -1L, false, false, null, "", true
        );

        public static SettingConfig SandboxMode = new ToggleSettingConfig(
            "SandboxMode",
            UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.NAME,
            UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.TOOLTIP,
            new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.LEVELS.DISABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.LEVELS.DISABLED.TOOLTIP, 0L, null),
            new SettingLevel("Enabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.LEVELS.ENABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.LEVELS.ENABLED.TOOLTIP, 0L, null),
            "Disabled", "Disabled", -1L, false, true, null, ""
        );

        // 这里可以继续添加其他游戏设置字段（FastWorkersMode, SaveToCloud 等）
    }

    // 模拟房间约束
    public class MockRoomConstraints
    {
        public static RoomConstraints.Constraint CEILING_HEIGHT_4 = new RoomConstraints.Constraint(
            null,
            (Room room) => 1 + room.cavity.maxY - room.cavity.minY >= 4,
            1,
            string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.NAME, "4"),
            string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.DESCRIPTION, "4"),
            null, null
        );

        public static RoomConstraints.Constraint CEILING_HEIGHT_6 = new RoomConstraints.Constraint(
            null,
            (Room room) => 1 + room.cavity.maxY - room.cavity.minY >= 6,
            1,
            string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.NAME, "6"),
            string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.DESCRIPTION, "6"),
            null, null
        );

        // 可以继续添加 MINIMUM_SIZE、MAXIMUM_SIZE、DECORATIVE_ITEM 等
    }

    // 读取 .po 文件并处理翻译
    public class PoFileReader
    {
        public static string FixupString(string result)
        {
            result = result.Replace("\\n", "\n");
            result = result.Replace("\\\"", "\"");
            result = result.Replace("<style=“", "<style=\"");
            result = result.Replace("”>", "\">");
            result = result.Replace("<color=^p", "<color=#");
            return result;
        }

        public static string GetParameter(string key, int idx, string[] all_lines)
        {
            if (!all_lines[idx].StartsWith(key)) return null;
            List<string> list = new List<string>();
            string text = all_lines[idx].Substring(key.Length + 1);
            list.Add(text);
            for (int i = idx + 1; i < all_lines.Length; i++)
            {
                string text2 = all_lines[i];
                if (!text2.StartsWith("\"")) break;
                list.Add(text2);
            }

            string text3 = "";
            foreach (string text4 in list)
            {
                string str = FixupString(text4.Substring(1, text4.Length - 2));
                text3 += str;
            }

            return text3;
        }

        public static Dictionary<string, string> TranslatedStringsEnCn(string[] lines)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            Entry entry = default;
            for (int i = 0; i < lines.Length; i++)
            {
                string text = lines[i];
                if (string.IsNullOrEmpty(text))
                {
                    entry = default;
                    continue;
                }

                string parameter = GetParameter("msgctxt", i, lines);
                if (parameter != null) entry.msgctxt = parameter;
                parameter = GetParameter("msgid", i, lines);
                if (parameter != null) entry.msgid = parameter;
                parameter = GetParameter("msgstr", i, lines);
                if (parameter != null) entry.msgstr = parameter;

                if (entry.IsPopulated)
                {
                    dictionary[entry.msgid] = entry.msgstr;
                    entry = default;
                }
            }
            return dictionary;
        }

        public struct Entry
        {
            public bool IsPopulated => msgctxt != null && msgstr != null && msgstr.Length > 0;
            public string msgctxt;
            public string msgstr;
            public string msgid;
        }
    }

    // 翻译修复逻辑
    [HarmonyPatch(typeof(PressureDoorConfig), "CreateBuildingDef")]
    public class TestTranslatePath
    {
        public static bool inited = false;
        public static Dictionary<string, string> translateDictionary = null;
        public static BindingFlags staticflags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        public static void updateLabelAndTooltip<T>(T obj)
        {
            Traverse traverse = Traverse.Create(obj).Field("<label>k__BackingField");
            Traverse traverse2 = Traverse.Create(obj).Field("<tooltip>k__BackingField");
            if (translateDictionary.ContainsKey((string)traverse.GetValue()) && translateDictionary.ContainsKey((string)traverse2.GetValue()))
            {
                traverse.SetValue(translateDictionary[(string)traverse.GetValue()]);
                traverse2.SetValue(translateDictionary[(string)traverse2.GetValue()]);
            }
            else
            {
                Debug.LogWarning($"---> 无翻译 {obj} {traverse}");
            }
        }

        public static void fixGameSettingTranslate()
        {
            FieldInfo[] fields = typeof(CustomGameSettingConfigs).GetFields(staticflags);
            foreach (FieldInfo fieldInfo in fields)
            {
                try
                {
                    object value = fieldInfo.GetValue(null);
                    if (value is SettingConfig sc)
                    {
                        updateLabelAndTooltip(sc);
                    }
                    else if (value is ToggleSettingConfig tsc)
                    {
                        updateLabelAndTooltip(tsc.on_level);
                        updateLabelAndTooltip(tsc.off_level);
                    }
                    else if (value is ListSettingConfig lsc)
                    {
                        foreach (SettingLevel level in lsc.GetLevels())
                            if (level != null) updateLabelAndTooltip(level);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("-----> 出了一异常 --<" + fieldInfo?.ToString());
                    Debug.LogWarning(ex);
                }
            }
        }

        public static void fixRoomTranslate()
        {
            FieldInfo[] fields = typeof(RoomConstraints).GetFields(staticflags);
            foreach (FieldInfo fieldInfo in fields)
            {
                if (fieldInfo.GetValue(null) is RoomConstraints.Constraint constraint)
                {
                    FieldInfo mockField = typeof(MockRoomConstraints).GetField(fieldInfo.Name, staticflags);
                    if (mockField != null)
                    {
                        RoomConstraints.Constraint mockConstraint = (RoomConstraints.Constraint)mockField.GetValue(null);
                        constraint.name = mockConstraint.name;
                        constraint.description = mockConstraint.description;
                    }
                }
            }
        }

        public static void fixTraitTranslate()
        {
            ResourceSet<Trait> traits = Db.Get().traits;
            foreach (Trait trait in traits.resources)
            {
                if (translateDictionary.TryGetValue(trait.Name, out string translatedName))
                {
                    trait.Name = translatedName + "(" + trait.Name + ")";
                }
                if (translateDictionary.TryGetValue(trait.description, out string translatedDesc))
                {
                    trait.description = translatedDesc + " " + trait.description;
                }
            }
        }

        public static void Postfix()
        {
            if (inited) return;
            inited = true;

            string currentLanguageCode = Localization.GetCurrentLanguageCode();
            if (currentLanguageCode != "zh")
                return;

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "strings.po");
            if (!File.Exists(path)) return;

            translateDictionary = PoFileReader.TranslatedStringsEnCn(File.ReadAllLines(path, Encoding.UTF8));

            fixGameSettingTranslate();
            fixRoomTranslate();
            fixTraitTranslate();

            Debug.Log("---> 完成翻译修复");
        }
    }
}

using HarmonyLib;
using TMPro;
using System;

namespace OniFontPatchZhCN
{
    /// <summary>
    /// 专门处理那些不在 .po 文件里、由代码生成的动态字符串
    /// </summary>
    public static class DynamicTranslationFix
    {
            public static void ApplyInCodeFixes()
            {
                // 暂时留空，解决编译错误
                // 以后如果有需要强制修改 STRINGS.UI 的代码可以写在这里
            }

            [HarmonyLib.HarmonyPatch(typeof(TMPro.TMP_Text), "text", HarmonyLib.MethodType.Setter)]
            public static class TMProInterceptPatches
            {
                [HarmonyLib.HarmonyPrefix]
                public static void Intercept_TMP_SetText(ref string value)
                {
                    if (string.IsNullOrEmpty(value)) return;

                    // 1. 处理复制人、物种等相关动态文本
                    if (value.Contains("Duplicant") || value.Contains("Species") || value.Contains("Standard") || value.Contains("Bionic"))
                    {
                        if (value.Contains("Standard Duplicant"))
                        {
                            value = value.Replace("Standard Duplicant", "标准复制人");
                        }
                        if (value.Contains("Bionic Duplicant"))
                        {
                            value = value.Replace("Bionic Duplicant", "仿生复制人");
                        }
                        if (value.Contains("<link=\"DUPLICANTS\">Standard</link>"))
                        {
                            value = value.Replace("<link=\"DUPLICANTS\">Standard</link>", "<link=\"DUPLICANTS\">标准</link>");
                        }
                        if (value.Contains("<link=\"DUPLICANTS\">Bionic</link>"))
                        {
                            value = value.Replace("<link=\"DUPLICANTS\">Bionic</link>", "<link=\"DUPLICANTS\">仿生</link>");
                        }
                        if (value.Contains("Species: "))
                        {
                            value = value.Replace("Species: ", "物种：");
                        }
                    }

                    // 2. 处理尺寸限制相关的动态文本（原本房间部分逻辑的平替）
                    if (value.Contains("Minimum"))
                    {
                        value = value.Replace("Minimum", "最小");
                    }
                    if (value.Contains("Maximum"))
                    {
                        value = value.Replace("Maximum", "最大");
                    }

                    // 在这里可以继续添加其他漏翻的关键字
                }
            }
        }
    }
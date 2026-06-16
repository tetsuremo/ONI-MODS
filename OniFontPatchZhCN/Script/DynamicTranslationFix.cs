using HarmonyLib;
using TMPro;

namespace OniFontPatchZhCN
{
    public static class DynamicTranslationFix
    {
        [HarmonyPatch(typeof(TMP_Text), "text", MethodType.Setter)]
        public static class TMProInterceptPatches
        {
            [HarmonyPrefix]
            public static void Intercept_TMP_SetText(ref string value)
            {
                if (string.IsNullOrEmpty(value)) return;

                // 🌟 使用只读布尔变量拦截，杜绝原本每帧反射导致的性能地狱与日志刷屏
                if (!CustomFontAssets.IsZhCN) return;

                // 1. 处理复制人、物种等相关动态文本
                if (value.Contains("Duplicant") || value.Contains("Species") || value.Contains("Standard") || value.Contains("Bionic"))
                {
                    if (value.Contains("Standard Duplicant")) value = value.Replace("Standard Duplicant", "标准复制人");
                    if (value.Contains("Bionic Duplicant")) value = value.Replace("Bionic Duplicant", "仿生复制人");
                    if (value.Contains("<link=\"DUPLICANTS\">Standard</link>")) value = value.Replace("<link=\"DUPLICANTS\">Standard</link>", "<link=\"DUPLICANTS\">标准</link>");
                    if (value.Contains("<link=\"DUPLICANTS\">Bionic</link>")) value = value.Replace("<link=\"DUPLICANTS\">Bionic</link>", "<link=\"DUPLICANTS\">仿生</link>");
                    if (value.Contains("Species: ")) value = value.Replace("Species: ", "物种：");
                }

                // 2. 处理尺寸限制相关的动态文本
                if (value.Contains("Minimum")) value = value.Replace("Minimum", "最小");
                if (value.Contains("Maximum")) value = value.Replace("Maximum", "最大");
            }
        }
    }
}
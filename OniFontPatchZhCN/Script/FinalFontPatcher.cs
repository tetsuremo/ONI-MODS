using System;
using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace OniFontPatchZhCN
{
    public static class OniFontPatchZhCN
    {
        public enum FontFamily
        {
            Description, // 对应 思源黑体 类 (原版默认正文)
            Title,       // 对应 Economica 家族 (细长粗体标题/弹窗)
            Header       // 对应 GRAYSTROKE 家族 (硬核栏目头/主标题/看板)
        }

        // 隐形标记组件：用于永久锁死『轨道 B』无样式组件的阵营，防止回滚
        public class FontTrackTag : MonoBehaviour
        {
            public FontFamily TrackedFamily;
        }

        // ⚡ 轨道 B 的 GameObject 名字高效全词拦截哈希表
        public static readonly HashSet<string> GraystrokeNodesFull = new HashSet<string>
        {
            "developmentbuildlabel", "supertext", "usermessagelabel", "unopeneditemcountlabel",
            "headerlabel", "quantitylabel", "largecostlabel", "namelabel", "costlabel",
            "filamentlabel", "currentkj", "powerlabel", "fg", "titletext", "totallabel",
            "reservedlabel", "areavisualizertext", "maintoolheader", "othertoolsheader",
            "loadingmessage", "healthlabel", "impacttimelabel", "availablecount", "subtitletext",
            "categorylabel", "nosearchresultmessage", "prioritylabel", "breakertext", "value",
            "header", "titlelabel", "valuelabel", "buttontext", "progressmessage", "savefileinfo",
            "statuslabel", "locationtext", "coordinate", "placeholder", "cancel", "infotext",
            "errormessage", "loadingtext", "savetext", "datetext", "outfittitle", "infoworld",
            "infocycles", "infodupes", "nametext", "save", "imagelabel", "resumetext",
            "savenametext", "timeline", "expcount", "distancelabel", "rocketiconlabel",
            "rocketiconprogress", "destinationname", "colonyname", "cyclecount", "errortext",
            "quantitytext", "basename", "worldseed"
        };

        // 🛠️ 唯一核心翻译清洗管线 (直接继承自你原本 100% 灵验的代码)
        public static string CleanDynamicText(string value)
        {
            if (string.IsNullOrEmpty(value) || !CustomFontAssets.IsZhCN) return value;

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

            return value;
        }

        // 🧠 双轨道智能字体检测引擎
        public static FontFamily GetFamily(TextMeshProUGUI tmp, string style)
        {
            var existingTag = tmp.GetComponent<FontTrackTag>();
            if (existingTag != null) return existingTag.TrackedFamily;

            string styleLower = style?.ToLowerInvariant() ?? "";
            string goNameLower = tmp.gameObject.name.ToLowerInvariant();

            // 【轨道 B】无样式硬编码节点的降维打击
            if (string.IsNullOrEmpty(styleLower) || styleLower.Contains("无样式") || styleLower.Contains("null"))
            {
                if (GraystrokeNodesFull.Contains(goNameLower))
                {
                    var tag = tmp.gameObject.AddComponent<FontTrackTag>();
                    tag.TrackedFamily = FontFamily.Header;
                    return FontFamily.Header;
                }

                if (goNameLower.Contains("description") || goNameLower.Contains("errortext"))
                {
                    var tag = tmp.gameObject.AddComponent<FontTrackTag>();
                    tag.TrackedFamily = FontFamily.Description;
                    return FontFamily.Description;
                }

                if (goNameLower.Contains("title") || goNameLower.Contains("header") ||
                    goNameLower.Contains("label") || goNameLower.Contains("message") ||
                    goNameLower.Contains("date") || goNameLower.Contains("world") ||
                    goNameLower.Contains("status"))
                {
                    var tag = tmp.gameObject.AddComponent<FontTrackTag>();
                    tag.TrackedFamily = FontFamily.Header;
                    return FontFamily.Header;
                }

                if (goNameLower == "text" || goNameLower == "gameobject")
                {
                    if (tmp.transform.parent != null &&
                        (tmp.transform.parent.name.Contains("PopFX") || tmp.transform.parent.name.Contains("Tute") ||
                         tmp.transform.parent.name.Contains("Dialog") || tmp.transform.parent.name.Contains("Container") ||
                         tmp.transform.parent.name.Contains("Screen") || tmp.transform.parent.name.Contains("Button") ||
                         tmp.transform.parent.name.Contains("Navigator") || tmp.transform.parent.name.Contains("Menu") ||
                         tmp.transform.parent.name.Contains("Panel")))
                    {
                        var tag = tmp.gameObject.AddComponent<FontTrackTag>();
                        tag.TrackedFamily = FontFamily.Title;
                        return FontFamily.Title;
                    }
                }
                return FontFamily.Description;
            }

            // 【轨道 A】基于扫描报告的标准样式预设白名单精确分流
            if (styleLower.Contains("popfx") || styleLower.Contains("tooltip_title") || styleLower.Contains("titletextsmall"))
                return FontFamily.Title;

            if (styleLower.Contains("buttontitle") || styleLower.Contains("codex") || styleLower.Contains("namelabel") ||
                styleLower.Contains("tableheader") || styleLower.Contains("columnheader") || styleLower.Contains("titletext"))
                return FontFamily.Header;

            return FontFamily.Description;
        }

        public static TMP_FontAsset Resolve(FontFamily family)
        {
            TMP_FontAsset vanillaFallback = CustomFontAssets.GetVanillaChineseFont();
            return family switch
            {
                FontFamily.Title => (CustomFontAssets.MyTitleFont != null) ? CustomFontAssets.MyTitleFont : vanillaFallback,
                FontFamily.Header => (CustomFontAssets.MyHeaderFont != null) ? CustomFontAssets.MyHeaderFont : vanillaFallback,
                _ => (CustomFontAssets.MyDescriptionFont != null) ? CustomFontAssets.MyDescriptionFont : vanillaFallback
            };
        }

        public static void Apply(TextMeshProUGUI tmp)
        {
            if (!tmp || !CustomFontAssets.IsZhCN) return;

            // 文本预先洗地
            string currentText = tmp.text;
            string cleanedText = CleanDynamicText(currentText);
            if (currentText != cleanedText)
            {
                tmp.text = cleanedText;
            }

            string style = (tmp is LocText loc && loc.textStyleSetting != null) ? loc.textStyleSetting.name : "";
            var family = GetFamily(tmp, style);
            var expected = Resolve(family);

            if (!expected || tmp.font == expected) return;

            tmp.font = expected;
            tmp.fontSharedMaterial = expected.material;
            tmp.SetAllDirty();
        }
    }

    // ============================================================
    // 🧷 Harmony 独立顶级拦截闸口
    // ============================================================

    // 锚点时机 1：游戏原版语言初始化时，彻底刷醒并同步 IsZhCN 状态
    [HarmonyPatch(typeof(Localization), "Initialize")]
    public static class Patch_Localization_Initialize
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            CustomFontAssets.UpdateLanguageStatus();
            Debug.Log($"[OniFontPatchZhCN] [锚点 1: Localization.Initialize] 状态刷醒完成，当前中文化判定 = {CustomFontAssets.IsZhCN}");
        }
    }

    // 锚点时机 2：接管文本组件 setter，实行无损文本清洗
    [HarmonyPatch(typeof(TMP_Text), "text", MethodType.Setter)]
    public static class Patch_TMP_Text_Setter
    {
        [HarmonyPrefix]
        public static void Prefix(ref string value)
        {
            value = OniFontPatchZhCN.CleanDynamicText(value);
        }
    }

    // 锚点时机 3：LocText 字体现渲染
    [HarmonyPatch(typeof(LocText), "SwapFont")]
    public static class Patch_SwapFont
    {
        [HarmonyPostfix]
        public static void Postfix(LocText __instance)
        {
            if (__instance != null) OniFontPatchZhCN.Apply(__instance);
        }
    }

    // 锚点时机 4：Style 系统预设切入
    [HarmonyPatch(typeof(SetTextStyleSetting), "ApplyStyle")]
    public static class Patch_ApplyStyle
    {
        [HarmonyPostfix]
        public static void Postfix(TextMeshProUGUI sdfText, TextStyleSetting style)
        {
            OniFontPatchZhCN.Apply(sdfText);
        }
    }

    // 锚点时机 5：地毯式无损初始生命周期安全注入
    [HarmonyPatch(typeof(LocText), "Start")]
    public static class Patch_LocText_Start
    {
        [HarmonyPostfix]
        public static void Postfix(LocText __instance)
        {
            if (__instance != null) OniFontPatchZhCN.Apply(__instance);
        }
    }

    // 锚点时机 6：全屏强制收敛定时器（500ms 高效轮询，给动态无样式节点兜底）
    [HarmonyPatch(typeof(Game), "Update")]
    public static class Patch_Update
    {
        private static float _timer;

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!CustomFontAssets.IsZhCN) return;

            _timer += Time.unscaledDeltaTime;
            if (_timer < 0.5f) return;
            _timer = 0f;

            var all = UnityEngine.Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
            foreach (var tmp in all)
            {
                OniFontPatchZhCN.Apply(tmp);
            }
        }
    }
}
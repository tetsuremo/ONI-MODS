using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace TranslationLoaderLite
{
    public static class TranslationManager
    {
        public static Dictionary<string, string> LocalTranslations { get; private set; } = new Dictionary<string, string>();
        public static string ConfigFolderPath { get; private set; } = "";
        public static string CurrentLangCode { get; private set; } = "en"; // 纯净的 ISO 规范语言代码 (如 zh, de, ja, ko)
        private static string targetPoPath = "";

        public static void Initialize()
        {
            try
            {
                // 1. 获取并彻底净化当前的语言代码，确保完全回归标准规范 (zh_klei -> zh)
                CurrentLangCode = GetPurifiedLanguageCode();

                // 2. 追溯三层公共 config 路径
                string dllPath = Assembly.GetExecutingAssembly().Location;
                string dllFolder = Path.GetDirectoryName(dllPath);
                string localFolder = Path.GetDirectoryName(dllFolder);
                string modsFolder = Path.GetDirectoryName(localFolder);

                ConfigFolderPath = Path.Combine(modsFolder, "config", "TranslationLoader");

                if (!Directory.Exists(ConfigFolderPath))
                {
                    Directory.CreateDirectory(ConfigFolderPath);
                }

                // 🎯 进化：严格符合社区潜规则，生成标准的 zh.po, de.po, ja.po 等
                targetPoPath = Path.Combine(ConfigFolderPath, $"{CurrentLangCode}.po");

                // 3. 注册 Mod 内嵌的多语言矩阵（作为底稿防御）
                RegisterEmbeddedTranslations();

                // 4. 如果外置文件不存在，自动生成对应语言的纯净模板
                if (!File.Exists(targetPoPath))
                {
                    GenerateTemplateFile();
                }

                // 5. 载入并应用外置 PO
                LoadAndExpandPoFile();
            }
            catch (Exception ex)
            {
                global::Debug.LogError("[TranslationLoaderLite] Manager Initialize failed: " + ex);
            }
        }

        // 🎯 核心净化函数：在这里干净利落地剁掉所有冗余的后缀
        private static string GetPurifiedLanguageCode()
        {
            string rawCode = "en";
            try
            {
                string code = Localization.GetCurrentLanguageCode();
                if (!string.IsNullOrEmpty(code))
                {
                    rawCode = code.Trim().ToLower();
                }
            }
            catch
            {
                try
                {
                    string prefsLang = KPlayerPrefs.GetString("SelectedLanguage", "en");
                    if (!string.IsNullOrEmpty(prefsLang))
                    {
                        rawCode = prefsLang.Trim().ToLower();
                    }
                }
                catch { }
            }

            // 🌟 物理切除：如果是以 zh_ 开头（如 zh_klei, zh_amazon, zh_cn 等），直接一刀切到最纯净的 zh
            if (rawCode.StartsWith("zh"))
            {
                return "zh";
            }

            // 如果其他语言有类似冗余后缀，顺带一起净化
            if (rawCode.Contains("_"))
            {
                rawCode = rawCode.Split('_')[0];
            }
            if (rawCode.Contains("-"))
            {
                rawCode = rawCode.Split('-')[0];
            }

            return rawCode;
        }

        // 🎯 内嵌多语言矩阵：判断最规范的纯净代码即可
        private static void RegisterEmbeddedTranslations()
        {
            try
            {
                // 现在只需要优雅地匹配 "zh" 即可，干净利落
                if (CurrentLangCode == "zh")
                {
                    InjectString("TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_NAME", "点击“手动配置”打开翻译。");
                    InjectString("TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_TOOLTIP", "此启动选项毫无意义，仅用于说明。");
                    InjectString("TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.OPEN_FOLDER_NAME", "打开翻译文件夹");
                    InjectString("TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.OPEN_FOLDER_TOOLTIP", "点击此按钮打开当前语言 .po 文件所在的配置文件夹。");
                    InjectString("TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.RESET_TEMPLATE_NAME", "用干净模板重置/覆盖文件");
                    InjectString("TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.RESET_TEMPLATE_TOOLTIP", "点击此按钮将强制用纯净的英文模板覆盖当前的 .po 文件。");
                }
            }
            catch (Exception ex)
            {
                global::Debug.LogError("[TranslationLoaderLite] RegisterEmbeddedTranslations failed: " + ex);
            }
        }

        private static void InjectString(string path, string text)
        {
            Strings.Add(new string[] { path, text });
            int stringsIndex = path.IndexOf(".STRINGS.", StringComparison.OrdinalIgnoreCase);
            if (stringsIndex > 0)
            {
                Strings.Add(new string[] { path.Substring(stringsIndex + 1), text });
            }
        }

        private static void GenerateTemplateFile()
        {
            try
            {
                string templateContent = GetTemplateContent();
                File.WriteAllText(targetPoPath, templateContent, System.Text.Encoding.UTF8);
                global::Debug.Log($"[TranslationLoaderLite] Initial clean template for [{CurrentLangCode}] generated.");
            }
            catch (Exception ex)
            {
                global::Debug.LogError("[TranslationLoaderLite] GenerateTemplateFile failed: " + ex);
            }
        }

        public static void ForceGenerateTemplate()
        {
            try
            {
                if (string.IsNullOrEmpty(targetPoPath)) return;
                string templateContent = GetTemplateContent();
                File.WriteAllText(targetPoPath, templateContent, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                global::Debug.LogError("[TranslationLoaderLite] ForceGenerateTemplate failed: " + ex);
            }
        }

        private static string GetTemplateContent()
        {
            string timestamp = System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            return $"# Translation template for {CurrentLangCode}\r\n" +
                   $"# Language: {CurrentLangCode}\r\n" +
                   $"# Generated: {timestamp}\r\n\r\n" +
                   $"# TranslationLoaderLite\r\n" +
                   $"# msgctxt \"TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_NAME\"\r\n" +
                   $"# msgid \"Click「MANUAL CONFIG」to open the translation.\"\r\n" +
                   $"# msgstr \"\"\r\n\r\n" +
                   $"# TranslationLoaderLite_Tooltip\r\n" +
                   $"# msgctxt \"TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_TOOLTIP\"\r\n" +
                   $"# msgid \"This startup option is meaningless and is for illustration purposes only.\"\r\n" +
                   $"# msgstr \"\"\r\n";
        }

        private static void LoadAndExpandPoFile()
        {
            LocalTranslations.Clear();
            if (!File.Exists(targetPoPath)) return;

            try
            {
                var rawDict = Localization.LoadStringsFile(targetPoPath, false);
                foreach (var kvp in rawDict)
                {
                    if (string.IsNullOrEmpty(kvp.Key)) continue;

                    LocalTranslations[kvp.Key] = kvp.Value;

                    int stringsIndex = kvp.Key.IndexOf(".STRINGS.", StringComparison.OrdinalIgnoreCase);
                    if (stringsIndex > 0)
                    {
                        string shortKey = kvp.Key.Substring(stringsIndex + 1);
                        LocalTranslations[shortKey] = kvp.Value;
                    }
                }
                global::Debug.Log($"[TranslationLoaderLite] External [{CurrentLangCode}.po] active. Loaded {LocalTranslations.Count} override keys.");
            }
            catch (Exception ex)
            {
                global::Debug.LogError("[TranslationLoaderLite] LoadAndExpandPoFile error: " + ex);
            }
        }
    }
}
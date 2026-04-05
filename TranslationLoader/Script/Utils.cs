using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TranslationLoaderLite
{
    public static class Utils
    {
        public static readonly string ConfigFolderPath;
        public static Dictionary<string, string> translations = new Dictionary<string, string>();

        static Utils()
        {
            string dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string modsRoot = Path.GetFullPath(Path.Combine(dllPath, "..", ".."));
            ConfigFolderPath = Path.Combine(modsRoot, "config", "TranslationLoaderLite");
            Directory.CreateDirectory(ConfigFolderPath);
        }

        public static string GetPoPath(string langCode) =>
            Path.Combine(ConfigFolderPath, langCode + ".po");

        public static void LoadTranslations()
        {
            try
            {
                // ====================================================================
                //  【时机优化】 使用 GetCurrentLanguageCode 获取最准确的当前语言
                // ====================================================================
                string langCode = Localization.GetCurrentLanguageCode();
                string poLoadCode = langCode; // 用于查找文件

                if (string.IsNullOrEmpty(poLoadCode))
                {
                    poLoadCode = "en";
                    Debug.LogWarning("[TranslationLoaderLite] GetCurrentLanguageCode returned empty. Falling back to 'en'.");
                }

                // ====================================================================
                //  【语言映射】 将 zh_klei 映射为 zh，因为文件通常命名为 zh.po
                // ====================================================================
                if (poLoadCode.EndsWith("_klei"))
                {
                    poLoadCode = poLoadCode.Substring(0, poLoadCode.Length - 5); // 变为 'zh' 或 'ko' 等
                    Debug.Log($"[TranslationLoaderLite] Mapped language code from '{langCode}' to '{poLoadCode}'");
                }

                Dictionary<string, string> dict = null;

                // ====================================================================
                // 1. 先尝试从外部 config 文件夹读取 (最高优先级)
                // ====================================================================
                string poPath = GetPoPath(poLoadCode);
                if (File.Exists(poPath))
                {
                    dict = ParsePoFileFromFile(poPath);
                    Debug.Log($"[TranslationLoaderLite] Loaded {dict?.Count ?? 0} translations from external file: {poPath}");
                }

                // ====================================================================
                // // 2. 如果文件不存在或未加载到翻译，再尝试嵌入资源
                // ====================================================================
                if (dict == null || (dict.Count == 0 && !File.Exists(poPath)))
                {
                    dict = LoadTranslationsFromEmbeddedResource(poLoadCode);
                }

                // ====================================================================
                // 3. 如果还没有加载到翻译，就生成空模板 (使用映射后的代码)
                // ====================================================================
                if (dict == null || dict.Count == 0)
                {
                    // GenerateBlankTemplates 需要使用 GetLocale 确保生成路径正确，且我们已经知道它会返回 'zh' 或 'en'
                    GenerateBlankTemplates(force: false);

                    // 再次尝试从 config 文件夹读取（模板可能已创建）
                    poPath = GetPoPath(poLoadCode);
                    if (File.Exists(poPath))
                    {
                        dict = ParsePoFileFromFile(poPath);
                        Debug.Log($"[TranslationLoaderLite] Loaded blank template translations from file: {poPath}");
                    }
                }

                translations = dict ?? new Dictionary<string, string>();
                Debug.Log($"[TranslationLoaderLite] Successfully loaded {translations.Count} translations for {poLoadCode}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TranslationLoaderLite] Failed to load translations: {ex}");
            }
        }

        private static Dictionary<string, string> LoadTranslationsFromEmbeddedResource(string langCode)
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();

                // ====================================================================
                //  【核心修复】 使用空字符串前缀，实现无命名空间的资源查找
                // ====================================================================
                string resourcePrefix = "";

                string resourceName = resourcePrefix + langCode + ".po";
                string fallbackResourceName = resourcePrefix + "en.po";

                // 尝试查找当前语言资源
                if (asm.GetManifestResourceStream(resourceName) == null && langCode != "en")
                {
                    // 如果找不到，尝试回退到英文
                    resourceName = fallbackResourceName;
                    Debug.LogWarning($"[TranslationLoaderLite] Embedded resource for {langCode} not found. Attempting fallback to {resourceName}.");
                }

                Debug.Log($"[TranslationLoaderLite] Attempting to load embedded resource: {resourceName}");

                using (var stream = asm.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            var dict = ParsePoFileFromString(reader.ReadToEnd());
                            Debug.Log($"[TranslationLoaderLite] Loaded {dict.Count} translations from embedded resource: {resourceName}");
                            return dict;
                        }
                    }
                    else
                    {
                        Debug.LogError($"[TranslationLoaderLite] Embedded resource not found: {resourceName}");
                        // 打印所有资源，以便您最终确认 resourcePrefix 是否应该为空
                        Debug.Log("[TranslationLoaderLite] Available resources:");
                        foreach (var name in asm.GetManifestResourceNames())
                        {
                            Debug.Log($"  - {name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 捕获异常，防止崩溃
                Debug.LogError($"[TranslationLoaderLite] Failed to load embedded resource {langCode}: {ex}");
            }

            return null;
        }

        // --- (OpenPoDirectory, GenerateBlankTemplates, PO Parsing 区域保持不变) ---

        public static void OpenPoDirectory()
        {
            try
            {
                System.Diagnostics.Process.Start(ConfigFolderPath);
                Debug.Log($"[TranslationLoaderLite] Opened PO directory: {ConfigFolderPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TranslationLoaderLite] Failed to open PO directory: {ex}");
            }
        }

        public static void GenerateBlankTemplates(bool force)
        {
            try
            {
                // 使用 GetLocale，因为它在模板生成时是安全的，且可能返回 'zh'
                var locale = Localization.GetLocale();
                string langCode = locale?.Code ?? "en";
                string poPath = GetPoPath(langCode);

                if (!File.Exists(poPath) || force)
                {
                    using (var writer = new StreamWriter(poPath, false, Encoding.UTF8))
                    {
                        writer.WriteLine($"# Translation template for {langCode}");
                        writer.WriteLine($"# Language: {langCode}");
                        writer.WriteLine($"# Generated: {System.DateTime.Now}");
                        writer.WriteLine();
                        writer.WriteLine("# TranslationLoaderLite");
                        writer.WriteLine("# msgctxt \"TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_NAME\"");
                        writer.WriteLine("# msgid \"Click「MANUAL CONFIG」to open the translation.\"");
                        writer.WriteLine("# msgstr \"\"");
                        writer.WriteLine();
                        writer.WriteLine("# TranslationLoaderLite_Tooltip");
                        writer.WriteLine("# msgctxt \"TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_TOOLTIP\"");
                        writer.WriteLine("# msgid \"This startup option is meaningless and is for illustration purposes only.\"");
                        writer.WriteLine("# msgstr \"\"");
                    }
                    Debug.Log($"[TranslationLoaderLite] Generated blank PO template for {langCode} (force={force})");
                }
                else
                {
                    Debug.Log($"[TranslationLoaderLite] PO template already exists for {langCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[TranslationLoaderLite] Failed to generate templates: {ex}");
            }
        }

        #region PO Parsing
        private static Dictionary<string, string> ParsePoFileFromFile(string poFilePath)
        {
            var content = File.ReadAllText(poFilePath, Encoding.UTF8);
            return ParsePoFileFromString(content);
        }

        private static Dictionary<string, string> ParsePoFileFromString(string content)
        {
            var dict = new Dictionary<string, string>();
            var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string context = null, msgid = null, msgstr = null;
            bool inMsgId = false, inMsgStr = false;

            void Save()
            {
                if (!string.IsNullOrEmpty(context) && !string.IsNullOrEmpty(msgstr))
                    dict[context] = msgstr;
                context = msgid = msgstr = null;
                inMsgId = inMsgStr = false;
            }

            foreach (var line in lines)
            {
                string l = line.Trim();
                if (string.IsNullOrEmpty(l)) { Save(); continue; }
                if (l.StartsWith("#")) continue;

                if (l.StartsWith("msgctxt")) { Save(); context = ExtractQuotedValue(l); continue; }
                if (l.StartsWith("msgid")) { msgid = ExtractQuotedValue(l); inMsgId = true; inMsgStr = false; continue; }
                if (l.StartsWith("msgstr")) { msgstr = ExtractQuotedValue(l); inMsgId = false; inMsgStr = true; continue; }
                if (l.StartsWith("\"") && l.EndsWith("\""))
                {
                    string val = ExtractQuotedValue(l);
                    if (inMsgId) msgid += val;
                    if (inMsgStr) msgstr += val;
                }
            }
            Save();
            return dict;
        }

        private static string ExtractQuotedValue(string line)
        {
            if (string.IsNullOrEmpty(line)) return "";
            int first = line.IndexOf('"'), last = line.LastIndexOf('"');
            if (first >= 0 && last > first)
                return line.Substring(first + 1, last - first - 1).Replace("\\n", "\n").Replace("\\t", "\t");
            return line;
        }
        #endregion
    }
}

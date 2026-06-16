using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TranslationLoaderLite
{
    public static class Utils
    {
        public static string ConfigFolderPath { get; private set; }
        public static Dictionary<string, string> translations = new Dictionary<string, string>();

        public static void InitPaths()
        {
            try
            {
                string location = Assembly.GetExecutingAssembly().Location;
                if (string.IsNullOrEmpty(location)) return;

                string dllDir = Path.GetDirectoryName(location);
                // 路径：mods/YourMod/config/TranslationLoaderLite
                string modRoot = Path.GetFullPath(Path.Combine(dllDir, "..", ".."));
                ConfigFolderPath = Path.Combine(modRoot, "config", "TranslationLoaderLite");

                if (!Directory.Exists(ConfigFolderPath))
                {
                    Directory.CreateDirectory(ConfigFolderPath);
                }
            }
            catch (Exception ex)
            {
                global::Debug.LogError("[TranslationLoaderLite] Failed to initialize paths: " + ex.Message);
            }
        }

        public static string GetPoPath(string langCode) => Path.Combine(ConfigFolderPath, langCode + ".po");

        public static void LoadTranslations()
        {
            try
            {
                string langCode = Localization.GetCurrentLanguageCode();
                if (string.IsNullOrEmpty(langCode)) langCode = "en";

                string poFileName = langCode;
                if (poFileName.EndsWith("_klei"))
                {
                    poFileName = poFileName.Substring(0, poFileName.Length - 5);
                }

                Dictionary<string, string> loadedDict = null;

                // 优先读取外部
                string externalPath = GetPoPath(poFileName);
                if (File.Exists(externalPath))
                {
                    loadedDict = ParsePoFileFromFile(externalPath);
                }

                // 次选嵌入资源
                if (loadedDict == null || loadedDict.Count == 0)
                {
                    loadedDict = LoadTranslationsFromEmbeddedResource(poFileName);
                }

                // 都没有则生成模板
                if (loadedDict == null || loadedDict.Count == 0)
                {
                    GenerateBlankTemplates(force: false);
                }

                translations = loadedDict ?? new Dictionary<string, string>();
                global::Debug.Log($"[TranslationLoaderLite] Memory ready: {translations.Count} keys.");
            }
            catch (Exception ex)
            {
                global::Debug.LogError($"[TranslationLoaderLite] LoadTranslations error: {ex}");
            }
        }

        // --- 以下代码保持原样，无需修改（LoadTranslationsFromEmbeddedResource, ParsePoFile, 等等） ---
        private static Dictionary<string, string> LoadTranslationsFromEmbeddedResource(string langCode)
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                string resourceName = langCode + ".po";
                using (var stream = asm.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            return ParsePoFileFromString(reader.ReadToEnd());
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public static void GenerateBlankTemplates(bool force)
        {
            try
            {
                string langCode = Localization.GetCurrentLanguageCode() ?? "en";
                if (langCode.EndsWith("_klei")) langCode = langCode.Substring(0, langCode.Length - 5);
                string poPath = GetPoPath(langCode);
                if (!File.Exists(poPath) || force)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("# Translation template");
                    sb.AppendLine("msgctxt \"Key.Example\"\nmsgid \"Original\"\nmsgstr \"\"");
                    File.WriteAllText(poPath, sb.ToString(), Encoding.UTF8);
                }
            }
            catch { }
        }

        private static Dictionary<string, string> ParsePoFileFromFile(string path) => ParsePoFileFromString(File.ReadAllText(path, Encoding.UTF8));
        private static Dictionary<string, string> ParsePoFileFromString(string content)
        {
            var dict = new Dictionary<string, string>();
            var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string context = null, msgstr = null; bool inMsgStr = false;
            foreach (var line in lines)
            {
                string l = line.Trim();
                if (string.IsNullOrEmpty(l)) { if (context != null && msgstr != null) dict[context] = msgstr; context = msgstr = null; continue; }
                if (l.StartsWith("#")) continue;
                if (l.StartsWith("msgctxt")) { context = ExtractQuotedValue(l); continue; }
                if (l.StartsWith("msgid")) { inMsgStr = false; continue; }
                if (l.StartsWith("msgstr")) { msgstr = ExtractQuotedValue(l); inMsgStr = true; continue; }
                if (l.StartsWith("\"") && inMsgStr) msgstr += ExtractQuotedValue(l);
            }
            if (context != null && msgstr != null) dict[context] = msgstr;
            return dict;
        }
        private static string ExtractQuotedValue(string line)
        {
            int first = line.IndexOf('"'), last = line.LastIndexOf('"');
            if (first >= 0 && last > first) return line.Substring(first + 1, last - first - 1).Replace("\\n", "\n").Replace("\\t", "\t");
            return string.Empty;
        }
    }
}
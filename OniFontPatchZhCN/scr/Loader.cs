using HarmonyLib;
using KMod;
using UnityEngine;
using System.IO;

namespace TetsuRemo.OniFontPatchZhCN
{
    public class Loader : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            string rootPath = base.mod.file_source.GetRoot();
            var config = FontConfig.Load(rootPath);

            // --- 字体加载 ---
            Patcher.Fonts = CustomFontAssets.Load(rootPath, config);
            Patcher.DebugMode = config.Debug;

            Debug.Log($"[OniFontPatchZhCN] FontConfig loaded:\n- Title={config.Title}\n- Head={config.Head}\n- Debug={config.Debug}\n- EnableTranslationFix={config.EnableTranslationFix}");

            Debug.Log($"[OniFontPatchZhCN] Loaded fonts: Title={Patcher.Fonts?.Title?.name}, Head={Patcher.Fonts?.Head?.name}, Description={Patcher.Fonts?.Description?.name}");
            // --- 翻译修复 ---
            if (config.EnableTranslationFix && IsCurrentLanguageChinese())
            {
                string translationPath = Path.Combine(rootPath, "translations", "strings_preinstalled_zh.po").Replace("\\", "/");

                if (File.Exists(translationPath))
                {
                    try
                    {
                        TranslationFixer.LoadPoFile(translationPath);
                        Debug.Log($"[OniFontPatchZhCN] Loaded PO file: {translationPath} (entries: {TranslationFixer.EntryCount})");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[OniFontPatchZhCN] Failed to load PO file {translationPath}: {ex}");
                    }

                    try
                    {
                        // 安装 Harmony 钩子
                        TranslationFixer.Initialize(harmony);
                        Debug.Log("[OniFontPatchZhCN] TranslationFixer hooks installed for Chinese language.");

                        // 原 ForceFirstLoad 功能：强制初始化 Localization
                        try
                        {
                            Localization.Initialize();
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogWarning($"[OniFontPatchZhCN] Localization.Initialize failed: {ex}");
                        }

                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[OniFontPatchZhCN] Failed to initialize TranslationFixer: {ex}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[OniFontPatchZhCN] PO file not found: {translationPath}");
                }
            }
            else
            {
                Debug.Log("[OniFontPatchZhCN] Translation fix skipped: either disabled in config or current language is not Chinese.");
            }

            // --- Harmony 补丁 ---
            try
            {
                harmony.PatchAll();
                Debug.Log("[OniFontPatchZhCN] Harmony.PatchAll completed.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[OniFontPatchZhCN] Harmony.PatchAll failed: {ex}");
            }

            // --- 刷新 LocText ---
            try
            {
                Patcher.RefreshLocText();
                Debug.Log("[OniFontPatchZhCN] Initial RefreshLocText done.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[OniFontPatchZhCN] RefreshLocText failed: {ex}");
            }

            Debug.Log("[OniFontPatchZhCN] Loader finished.");
        }

        // 判断当前语言是否为中文
        private bool IsCurrentLanguageChinese()
        {
            var lang = Application.systemLanguage;
            return lang == SystemLanguage.Chinese || lang == SystemLanguage.ChineseSimplified || lang == SystemLanguage.ChineseTraditional;
        }
    }
}

using HarmonyLib;
using KMod;
using System;
using System.IO;
using UnityEngine;

namespace OniFontPatchZhCN
{
    public class MainLoader : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            string rootPath = mod.file_source.GetRoot();
            var config = FontConfig.Load(rootPath);

            // 初始化字体系统
            FontPatcher.Fonts = CustomFontAssets.Load(rootPath, config);
            FontPatcher.DebugMode = config.Debug;

            // 初始化翻译系统
            if (config.EnableTranslationFix && IsChineseLanguage())
            {
                InitializeTranslationSystem(rootPath);
            }

            // 应用 Harmony 补丁
            harmony.PatchAll();
            FontPatcher.RefreshLocText();

            Debug.Log("[OniFontPatchZhCN] Mod loaded successfully");
        }

        private void InitializeTranslationSystem(string rootPath)
        {
            string translationPath = Path.Combine(rootPath, "translations", "strings_preinstalled_zh.po");

            if (File.Exists(translationPath))
            {
                try
                {
                    GameTranslationFixer.Initialize(translationPath);
                    Debug.Log($"[OniFontPatchZhCN] Translation system initialized with {translationPath}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[OniFontPatchZhCN] Translation system initialization failed: {ex}");
                }
            }
            else
            {
                Debug.LogWarning($"[OniFontPatchZhCN] Translation file not found: {translationPath}");
            }
        }

        private static bool IsChineseLanguage()
        {
            var lang = Application.systemLanguage;
            return lang == SystemLanguage.Chinese ||
                   lang == SystemLanguage.ChineseSimplified ||
                   lang == SystemLanguage.ChineseTraditional;
        }
    }
}

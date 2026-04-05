using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System.Collections.Generic;
using System;

namespace TranslationLoaderLite
{
    public class TranslationLoaderMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary(false);

            // 注册 Mod 设置
            new POptions().RegisterOptions(this, typeof(TranslationLoaderOptions));
            PUtil.LogDebug("[TranslationLoaderLite] Mod options registered.");

            harmony.PatchAll();
            Debug.Log("[TranslationLoaderLite] Mod loaded (OnLoad)");
        }

        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);

            Debug.Log("[TranslationLoaderLite] OnAllModsLoaded called. Loading translation files (no immediate override).");

            // 1. 加载翻译文件（但不立即强制覆盖，覆盖交给 Patch）
            Utils.LoadTranslations();

            // 2. 注册自身的 PLib LocString
            TranslationLoaderLiteStrings.RegisterDelayed();

            Debug.Log("[TranslationLoaderLite] Translation files loaded and own strings registered.");
        }
    }
}

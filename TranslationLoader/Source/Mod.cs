using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System.Collections.Generic;

namespace TranslationLoaderLite
{
    public class TranslationLoaderMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);

            // 初始化 PLib
            PUtil.InitLibrary(false);

            // 注册 Mod 设置
            new POptions().RegisterOptions(this, typeof(TranslationLoaderOptions));
            PUtil.LogDebug("[TranslationLoaderLite] Mod options registered.");

            // Patch 所有 Harmony 修补
            harmony.PatchAll();
            Debug.Log("[TranslationLoaderLite] Mod loaded (OnLoad)");
        }

        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);

            // 1️⃣ 先加载所有翻译（包括自身和其他mod的翻译）
            Utils.LoadTranslations();

            // 2️⃣ 再注册自身 PLib LocString（这样会使用已加载的翻译）
            TranslationLoaderLiteStrings.RegisterDelayed();

            Debug.Log("[TranslationLoaderLite] Loaded translations for all mods and registered own strings.");
        }
    }
}
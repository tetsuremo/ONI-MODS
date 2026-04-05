using HarmonyLib;

namespace TranslationLoaderLite
{
    public static class LocalizationPatches
    {
        public static void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Localization), "Initialize"),
                postfix: new HarmonyMethod(typeof(LocalizationPatches), nameof(Init_Postfix))
            );
        }

        public static void Init_Postfix()
        {
            Debug.Log("[TranslationLoaderLite] Localization.Initialize postfix triggered.");

            Utils.LoadTranslations();

            if (Utils.translations != null && Utils.translations.Count > 0)
            {
                Localization.OverloadStrings(Utils.translations);
                Debug.Log($"[TranslationLoaderLite] OverloadStrings applied. Keys overwritten: {Utils.translations.Count}");
            }
            else
            {
                Debug.LogWarning("[TranslationLoaderLite] No translations loaded, skipping OverloadStrings.");
            }
        }
    }
}

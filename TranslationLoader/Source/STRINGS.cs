using PeterHan.PLib.Database;
using System.Reflection;

namespace TranslationLoaderLite
{
    public static class TranslationLoaderLiteStrings
    {
        public static class OPTIONS
        {
            // English fallback
            public static LocString KEYPLACEHOLDER_NAME = new LocString(
                "Click「MANUAL CONFIG」to open the translation.",
                "TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_NAME"
            );

            public static LocString KEYPLACEHOLDER_TOOLTIP = new LocString(
                "This startup option is meaningless and is for illustration purposes only.",
                "TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_TOOLTIP"
            );
        }

        // 延迟注册方法，在 OnAllModsLoaded 调用
        public static void RegisterDelayed()
        {
            try
            {
                var ploc = new PLocalization();
                ploc.Register(Assembly.GetExecutingAssembly());

                // 检查是否有已加载的自定义翻译
                string nameKey = "TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_NAME";
                string tooltipKey = "TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_TOOLTIP";

                // 如果翻译字典中有我们的键，就使用翻译，否则使用英文fallback
                if (Utils.translations != null && Utils.translations.ContainsKey(nameKey))
                {
                    Strings.Add(nameKey, Utils.translations[nameKey]);
                    Debug.Log($"[TranslationLoaderLite] Used custom translation for name: {Utils.translations[nameKey]}");
                }
                else
                {
                    Strings.Add(nameKey, "Click「MANUAL CONFIG」to open the translation.");
                    Debug.Log("[TranslationLoaderLite] Used fallback English for name");
                }

                if (Utils.translations != null && Utils.translations.ContainsKey(tooltipKey))
                {
                    Strings.Add(tooltipKey, Utils.translations[tooltipKey]);
                    Debug.Log($"[TranslationLoaderLite] Used custom translation for tooltip: {Utils.translations[tooltipKey]}");
                }
                else
                {
                    Strings.Add(tooltipKey, "This startup option is meaningless and is for illustration purposes only.");
                    Debug.Log("[TranslationLoaderLite] Used fallback English for tooltip");
                }

                Debug.Log($"[TranslationLoaderLite] Delayed PLib translation registration finished.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[TranslationLoaderLite] Failed to register delayed translations: {ex}");
            }
        }
    }
}
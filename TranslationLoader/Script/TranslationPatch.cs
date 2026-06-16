using HarmonyLib;
using System;

namespace TranslationLoaderLite
{
    public static class Patch_Strings_Interceptor
    {
        [ThreadStatic]
        private static bool _isProcessing;

        public static void Prefix(string key, ref string text)
        {
            // 如果字典还没准备好，或者正在处理，绝对不运行逻辑
            if (_isProcessing || Utils.translations == null || Utils.translations.Count == 0 || key == null)
                return;

            try
            {
                _isProcessing = true;

                // 极简 Key 转换，避开繁琐的字符串操作
                string lookupKey = key.StartsWith("STRINGS.") ? key.Substring(8) : key;

                if (Utils.translations.TryGetValue(lookupKey, out string translated))
                {
                    if (!string.IsNullOrEmpty(translated))
                        text = translated;
                }
            }
            catch { /* 屏蔽补丁内所有异常，防止炸掉游戏 */ }
            finally
            {
                _isProcessing = false;
            }
        }
    }
    [HarmonyPatch(typeof(MainMenu), "OnPrefabInit")]
    public static class Patch_MainMenu_LoadTrigger
    {
        public static void Postfix()
        {
            // 如果还没加载，就执行加载
            if (Utils.translations == null || Utils.translations.Count == 0)
            {
                global::Debug.Log("[TranslationLoaderLite] Safety trigger: Loading translations now.");
                Utils.LoadTranslations();

                // 加载完后，手动覆盖一次游戏已经加载好的文本
                if (Utils.translations.Count > 0)
                {
                    Localization.OverloadStrings(Utils.translations);
                }
            }
        }
    }
}
using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace TranslationLoaderLite
{
    [RestartRequired]
    [ConfigFile("config.json", false, true)] 
    [Serializable]
    public class TranslationLoaderOptions : SingletonOptions<TranslationLoaderOptions>
    {
        [Option(
            "TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_NAME",
            "TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_TOOLTIP"
        )]
        [JsonProperty]
        public bool Key { get; set; } = false;
    }
}

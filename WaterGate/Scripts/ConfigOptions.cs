using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace WaterGateKai
{
    [JsonObject(MemberSerialization.OptIn)]
    [ConfigFile("config.json", true, false)]
    public sealed class ConfigOptions
    {
        public static ConfigOptions Instance { get; private set; }

        static ConfigOptions()
        {
            Instance = new ConfigOptions();
        }

        [JsonProperty]
        [Option("WaterGateKai.STRINGS.CONFIG.BUILDSPEED.NAME",
                "WaterGateKai.STRINGS.CONFIG.BUILDSPEED.DESC",
                "WaterGateKai.STRINGS.CONFIG.GENERAL")]
        [Limit(10, 120)]
        public float BuildSpeed { get; set; } = 30f;

        [JsonProperty]
        [Option("WaterGateKai.STRINGS.CONFIG.BOTTOMLIQUIDMASS.NAME",
                "WaterGateKai.STRINGS.CONFIG.BOTTOMLIQUIDMASS.DESC",
                "WaterGateKai.STRINGS.CONFIG.GENERAL")]
        [Limit(0.01f, 100f)]
        public float BottomLiquidMass { get; set; } = 0.111f;
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PeterHan.PLib.Options;
using System;

namespace WaterGateKai
{
    [JsonObject(MemberSerialization.OptIn)]
    [ConfigFile("config.json", true, false)]
    [RestartRequired]
    public sealed class ConfigOptions
    {
        public static ConfigOptions Instance { get; internal set; }

        static ConfigOptions()
        {
            Instance = new ConfigOptions();
        }

        // --------------------------
        // 原来的 float 配置
        // --------------------------
        [JsonProperty]
        [Option("WaterGateKai.STRINGS.CONFIG.BUILDSPEED.NAME",
                "WaterGateKai.STRINGS.CONFIG.BUILDSPEED.DESC",
                "WaterGateKai.STRINGS.CONFIG.GENERAL")]
        [Limit(10f, 120f)]
        public float BuildSpeed { get; set; } = 30f;

        [JsonProperty]
        [Option("WaterGateKai.STRINGS.CONFIG.BOTTOMLIQUIDMASS.NAME",
                "WaterGateKai.STRINGS.CONFIG.BOTTOMLIQUIDMASS.DESC",
                "WaterGateKai.STRINGS.CONFIG.GENERAL")]
        [Limit(0.01f, 100f)]
        public float BottomLiquidMass { get; set; } = 0.03f;

        // --------------------------
        // 液体下拉菜单 - 强制序列化为字符串
        // --------------------------
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))] // 添加这个属性
        [Option("WaterGateKai.STRINGS.CONFIG.GRANITE_BOTTOM.NAME",
                "WaterGateKai.STRINGS.CONFIG.GRANITE_BOTTOM.DESC",
                "WaterGateKai.STRINGS.CONFIG.GENERAL")]
        public LiquidType GraniteBottom { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))] // 添加这个属性
        [Option("WaterGateKai.STRINGS.CONFIG.GRANITE_TOP.NAME",
                "WaterGateKai.STRINGS.CONFIG.GRANITE_TOP.DESC",
                "WaterGateKai.STRINGS.CONFIG.GENERAL")]
        public LiquidType GraniteTop { get; set; }

        public ConfigOptions()
        {
            // 只有在属性没有被反序列化设置时才设置默认值
            if (GraniteBottom == default(LiquidType))
                GraniteBottom = LiquidType.Ethanol;

            if (GraniteTop == default(LiquidType))
                GraniteTop = LiquidType.Brine;
        }

        public SimHashes GetGraniteBottomSimHash()
        {
            string enumName = GraniteBottom.ToString();
            Debug.Log($"[WaterGateKai] Converting GraniteBottom: {GraniteBottom} -> {enumName}");

            if (Enum.TryParse<SimHashes>(enumName, out var result))
                return result;

            Debug.LogWarning($"[WaterGateKai] Failed to parse GraniteBottom: {enumName}, using default Ethanol");
            return SimHashes.Ethanol;
        }

        public SimHashes GetGraniteTopSimHash()
        {
            string enumName = GraniteTop.ToString();
            Debug.Log($"[WaterGateKai] Converting GraniteTop: {GraniteTop} -> {enumName}");

            if (Enum.TryParse<SimHashes>(enumName, out var result))
                return result;

            Debug.LogWarning($"[WaterGateKai] Failed to parse GraniteTop: {enumName}, using default Brine");
            return SimHashes.Brine;
        }

        public static void Load()
        {
            Instance = POptions.ReadSettings<ConfigOptions>() ?? new ConfigOptions();
        }
    }
}

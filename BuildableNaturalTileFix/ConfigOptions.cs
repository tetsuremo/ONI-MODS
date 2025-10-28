using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace BuildableNaturalTileFix
{
    [JsonObject(MemberSerialization.OptIn)]
    [ConfigFile("config.json", true, false)]
    public sealed class ConfigOptions
    {
        public static ConfigOptions Instance { get; internal set; }

        static ConfigOptions()
        {
            Instance = new ConfigOptions();
        }

        [JsonProperty]
        [Option("BuildableNaturalTileFix.NaturalTileStrings.CONFIG.BUILDMASS.NAME",
                "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.BUILDMASS.DESC",
                "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.GENERAL")]
        [Limit(1f, 500f)]
        public float BuildMass { get; set; } = 50f;

        [JsonProperty]
        [Option("BuildableNaturalTileFix.NaturalTileStrings.CONFIG.BLOCKMASS.NAME",
                "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.BLOCKMASS.DESC",
                "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.GENERAL")]
        [Limit(1f, 1000f)]
        public float BlockMass { get; set; } = 10f;

        [JsonProperty]
        [Option("BuildableNaturalTileFix.NaturalTileStrings.CONFIG.BUILDSPEED.NAME",
                "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.BUILDSPEED.DESC",
                "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.GENERAL")]
        [Limit(0.1f, 50f)]
        public float BuildSpeed { get; set; } = 3f;

        public static void Load()
        {
            Instance = PeterHan.PLib.Options.POptions.ReadSettings<ConfigOptions>() ?? new ConfigOptions();
        }
    }
}

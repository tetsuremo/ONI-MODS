namespace BuildableNaturalTileFix
{
    public class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class BUILDABLENATURALTILEFIX_NATURALTILE
                {
                    public static LocString NAME = "Natural Tile";
                    public static LocString DESC = "Fill that hole you dug out back in with any solid element.";
                    public static LocString EFFECT = "Creates a natural tile from the selected material.";
                }
            }
        }
    }
    public static class NaturalTileStrings
    {
        public static class CONFIG
        {
            public static class BUILDMASS
            {
                public static LocString NAME = new LocString(
                    "Build Mass",
                    "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.BUILDMASS.NAME"
                );
                public static LocString DESC = new LocString(
                    "Mass required to build the natural tile.",
                    "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.BUILDMASS.DESC"
                );
            }

            public static class BLOCKMASS
            {
                public static LocString NAME = new LocString(
                    "Block Mass",
                    "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.BLOCKMASS.NAME"
                );
                public static LocString DESC = new LocString(
                    "Mass of the resulting natural tile.",
                    "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.BLOCKMASS.DESC"
                );
            }

            public static class BUILDSPEED
            {
                public static LocString NAME = new LocString(
                    "Build Speed",
                    "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.BUILDSPEED.NAME"
                );
                public static LocString DESC = new LocString(
                    "Construction time for the natural tile.",
                    "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.BUILDSPEED.DESC"
                );
            }

            public static LocString GENERAL = new LocString(
                "General Settings",
                "BuildableNaturalTileFix.NaturalTileStrings.CONFIG.GENERAL"
            );
        }
    }
}

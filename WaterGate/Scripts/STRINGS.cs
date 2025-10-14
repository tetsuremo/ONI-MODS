namespace WaterGateKai
{
    public class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class PIAPIWATERGATE1X2
                {
                    public static LocString NAME = "2-Tile Water Gate";
                    public static LocString DESC = "The most commonly used Liquid Locks!";
                    public static LocString EFFECT = "Igneous Rock → Brine + Water\nGranite → Ethanol + Concentrated Brine\nOther Materials → Crude Oil + Petroleum";
                }

                public class PIAPIWATERGATE1X3
                {
                    public static LocString NAME = "3-Tile Water Gate";
                    public static LocString DESC = "3-Tile versions of liquid locks.";
                    public static LocString EFFECT = "From top to bottom: Water, Petroleum, Crude Oil.";
                }

                public class PIAPIWATERGATE1X4
                {
                    public static LocString NAME = "4-Tile Water Gate";
                    public static LocString DESC = "4-Tile versions of liquid locks.";
                    public static LocString EFFECT = "From top to bottom: Water, Brine, Ethanol, Crude Oil.";
                }

                public class PIAPIGELGATE
                {
                    public static LocString NAME = "Gel Gate";
                    public static LocString DESC = "Why does Oil become ViscoGel?";
                    public static LocString EFFECT = "Produces 101 kg of gel upon activation.";
                }
            }
        }
        public static class CONFIG
        {
            public static LocString GENERAL = "General Settings";

            public static class BUILDSPEED
            {
                public static LocString NAME = "Build Time";
                public static LocString DESC = "Base construction time (seconds). Lower = faster.";
            }

            public static class BOTTOMLIQUIDMASS
            {
                public static LocString NAME = "Bottom Liquid Mass (kg)";
                public static LocString DESC = "Mass of the bottom liquid for 2-tile water gates.";
            }
        }
    }
}


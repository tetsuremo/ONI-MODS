namespace PeterHan.FoodTooltip
{
    public static class FoodTooltipStrings
    {
        public static LocString CRITTER_PER_CYCLE = "{0}: {1}";
        public static LocString CRITTER_PER_CYCLE_TOOLTIP = "This critter's {2} will provide {1} if used to make {0}";
        public static LocString CRITTER_INFERTILE = "{0}: Insufficient reproduction";
        public static LocString CRITTER_INFERTILE_TOOLTIP = "This critter is not reproducing enough to provide this item";

        public static LocString PLANT_PER_CYCLE = "{0}: {1}";
        public static LocString PLANT_PER_CYCLE_TOOLTIP = "This plant will provide {1} if used to make {0}";
        public static LocString PLANT_WILTING = "{0}: Not growing";
        public static LocString PLANT_WILTING_TOOLTIP = "This plant is not currently growing";

        private const string PRE_CONSUMPTION = "<color=#f44a47>";
        private const string PRE_PRODUCTION = "<color=#57b95e>";
        private const string PST = "</color>";

        public static LocString FOOD_RATE_CURRENT = "This Cycle: " + PRE_PRODUCTION + "+{0}" + PST + " " + PRE_CONSUMPTION + "-{1}" + PST;
        public static LocString FOOD_RATE_LAST1 = "Last Cycle: " + PRE_PRODUCTION + "+{0}" + PST + " " + PRE_CONSUMPTION + "-{1}" + PST;
        public static LocString FOOD_RATE_LAST5 = "5 Cycle Average: " + PRE_PRODUCTION + "+{0}" + PST + " " + PRE_CONSUMPTION + "-{1}" + PST;
    }

    internal static class FoodDescriptorTexts
    {
        internal static readonly FoodDescriptorText CRITTERS = new FoodDescriptorText(
            FoodTooltipStrings.CRITTER_PER_CYCLE, FoodTooltipStrings.CRITTER_PER_CYCLE_TOOLTIP,
            FoodTooltipStrings.CRITTER_INFERTILE, FoodTooltipStrings.CRITTER_INFERTILE_TOOLTIP
        );

        internal static readonly FoodDescriptorText PLANTS = new FoodDescriptorText(
            FoodTooltipStrings.PLANT_PER_CYCLE, FoodTooltipStrings.PLANT_PER_CYCLE_TOOLTIP,
            FoodTooltipStrings.PLANT_WILTING, FoodTooltipStrings.PLANT_WILTING_TOOLTIP
        );
    }

    internal sealed class FoodDescriptorText
    {
        public string PerCycle { get; }
        public string PerCycleTooltip { get; }
        public string Stifled { get; }
        public string StifledTooltip { get; }
        internal FoodDescriptorText(string perCycle, string perCycleTooltip, string stifled, string stifledTooltip)
        {
            PerCycle = perCycle;
            PerCycleTooltip = perCycleTooltip;
            Stifled = stifled;
            StifledTooltip = stifledTooltip;
        }
    }
}

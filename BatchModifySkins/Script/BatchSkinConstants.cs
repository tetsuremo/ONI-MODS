using System.Collections.Generic;
using UnityEngine;

namespace BatchModifyStyles
{
    public enum TemplateType { Building, Minion }

    public static class BatchSkinConstants
    {
        public static readonly Color TOOL_COLOR = new Color(59f / 255f, 184f / 255f, 219f / 255f, 1f);
        public static readonly Color AREA_COLOR = new Color(59f / 255f, 184f / 255f, 219f / 255f, 0.5f);
        public static readonly float ICON_SCALE = 0.5f;
        public static readonly float GRID_SIZE = 1f;
    }

    public class SavedOutfitData
    {
        public Dictionary<WearableAccessorizer.WearableType, WearableAccessorizer.Wearable> wearables;
        public Dictionary<ClothingOutfitUtility.OutfitType, List<ResourceRef<Database.ClothingItemResource>>> customClothing;
    }
}

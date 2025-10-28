using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace BatchModifyStyles
{
    public class BatchSkinMod : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            Log.Info("[BatchModifyStyles] Batch skin mod loaded");
        }

        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);

            // 加载资源
            Log.Info("[BatchModifyStyles] Starting asset loading...");
            Assets.LoadAssets();

            // 应用补丁
            BatchSkinPatches.ApplyAll(harmony);
            
        }
    }
}

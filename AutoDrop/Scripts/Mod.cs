using HarmonyLib;
using KMod;
using System.Collections.Generic;

namespace AutoDrop
{
    public class MyUserMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            Debug.Log("[AutoDrop] OnLoad: Before patches!");
            base.OnLoad(harmony);
            Debug.Log("[AutoDrop] OnLoad: After patches!");
        }

        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods)
        {
            Debug.Log("[AutoDrop] All mods loaded");
        }
    }
}
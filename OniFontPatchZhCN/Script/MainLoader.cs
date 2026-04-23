using HarmonyLib;
using KMod;
using System.Reflection;
using System.IO;
using UnityEngine;
using System;

namespace OniFontPatchZhCN
{
    public class MainLoader : UserMod2
    {
        public static FontConfig Config = new FontConfig(); // 全局静态配置

        public override void OnLoad(Harmony harmony)
        {
            Debug.Log("[OniFontPatchZhCN] Loading with Unity 6 Stability Patch...");
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            try
            {
                // 加载字体
                CustomFontAssets.Load(modPath);

                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Debug.Log("[OniFontPatchZhCN] Harmony patches (including TMPro interceptor) applied.");

                base.OnLoad(harmony);
            }
            catch (Exception e)
            {
                Debug.LogError($"[OniFontPatchZhCN] Load Error: {e.Message}");
            }
        }

        public override void OnAllModsLoaded(Harmony harmony, System.Collections.Generic.IReadOnlyList<Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);
            if (Global.Instance != null)
                Global.Instance.StartCoroutine(FinalFontPatcher.ApplyGlobalStylePatchCoroutine());
        }
    }
}
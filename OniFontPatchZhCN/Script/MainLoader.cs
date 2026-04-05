using HarmonyLib;
using KMod;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace OniFontPatchZhCN
{
    public class MainLoader : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            Debug.Log("[OniFontPatchZhCN] Starting MainLoader...");

            // 获取 DLL 所在物理路径
            string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            try
            {
                // 1. 加载资源 (AssetBundle + JSON 配置)
                CustomFontAssets.Load(modPath);

                // 2. 挂载补丁 (执行 FinalFontPatcher)
                base.OnLoad(harmony);

                Debug.Log("[OniFontPatchZhCN] Mod loaded and patches applied successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[OniFontPatchZhCN] Critical Load Error: {e.Message}\n{e.StackTrace}");
            }
        }
    }
}
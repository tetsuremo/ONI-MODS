using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using System;
using System.IO;
using System.Reflection;

namespace BuildableNaturalTileFix
{
    public sealed class BuildableNaturalTileFixMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            try
            {
                base.OnLoad(harmony);

                Debug.Log("[BNTFix] Mod OnLoad started.");

                // =========================
                // 初始化 PLib（仅用于设置界面）
                // =========================
                PUtil.InitLibrary();
                Debug.Log("[BNTFix] PLib initialized.");

                // 注册 PLib 设置面板翻译
                new PLocalization().Register(Assembly.GetExecutingAssembly());
                PUtil.LogDebug("[BNTFix] PLib localization registered");

                // 注册 Mod 设置
                new POptions().RegisterOptions(this, typeof(ConfigOptions));
                Debug.Log("[BNTFix] PLib options registered.");

                // =========================
                // Harmony 补丁
                // =========================
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Debug.Log("[BNTFix] Harmony patches applied.");

                Debug.Log("[BNTFix] Mod loaded successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[BNTFix] Exception in OnLoad: {e}");
            }
        }
    }
}

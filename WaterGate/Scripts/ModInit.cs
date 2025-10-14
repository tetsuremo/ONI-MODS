using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using System;
using System.Reflection;

namespace WaterGateKai
{
    public sealed class ModInit : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            try
            {
                base.OnLoad(harmony);

                Debug.Log("[WaterGateKai] Mod OnLoad started.");

                // =========================
                // 初始化 PLib（用于设置界面）
                // =========================
                PUtil.InitLibrary();
                Debug.Log("[WaterGateKai] PLib initialized.");

                // 注册 PLib 设置面板翻译
                new PLocalization().Register(Assembly.GetExecutingAssembly());
                PUtil.LogDebug("[WaterGateKai] PLib localization registered");

                // 注册 Mod 设置
                new POptions().RegisterOptions(this, typeof(ConfigOptions));
                Debug.Log("[WaterGateKai] PLib options registered.");

                // =========================
                // Harmony 补丁
                // =========================
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Debug.Log("[WaterGateKai] Harmony patches applied.");

                Debug.Log("[WaterGateKai] Mod loaded successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[WaterGateKai] Exception in OnLoad: {e}");
            }
        }
    }
}
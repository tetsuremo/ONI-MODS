using KMod;
using HarmonyLib;
using UnityEngine;

namespace TModLib.Options
{
    [HarmonyPatch(typeof(Mod), "OnButtonsCreated")]
    public static class Mod_OnButtonsCreated_Patch
    {
        public static void Postfix(Mod __instance, GameObject parent)
        {
            // 这里判断是否是需要显示设置按钮的 Mod
            // 如果是你自己的 Mod，则注入一个自定义按钮，点击后打开 TModLib 绘制的窗口

            // TODO: 实例化基于 Unity 6 的 UI Prefab
        }
    }
}
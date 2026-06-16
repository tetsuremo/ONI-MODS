using HarmonyLib;
using KMod;
using UnityEngine;

namespace OniFontPatchZhCN
{
    public class MainLoader : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            Debug.Log("[OniFontPatchZhCN] 模组启动，正在初始化底层核心架构...");

            // 1. 装载对应的 AssetBundle 字体文件
            CustomFontAssets.LoadFontBundle(this.path);

            // 2. 执行所有分流文件的 Harmony 补丁注册
            base.OnLoad(harmony);

            Debug.Log("[OniFontPatchZhCN] 所有文件补丁已成功修补注入。");
        }
    }
}
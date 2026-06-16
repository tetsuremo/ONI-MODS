using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using PeterHan.PLib.PatchManager;
using System;

namespace TranslationLoaderLite
{
    public class TranslationLoaderMod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            try
            {
                // 1. 仅初始化路径，不读文件，不打补丁
                Utils.InitPaths();

                base.OnLoad(harmony);

                // 2. PLib 基础初始化（必须在 OnLoad）
                PUtil.InitLibrary(false);
                new POptions().RegisterOptions(this, typeof(TranslationLoaderOptions));

                // 3. 手动注册拦截补丁（只注册我们最稳的 Strings 补丁）
                // 暂时不 PatchAll，防止自动加载了某些有问题的类
                harmony.Patch(AccessTools.Method(typeof(Strings), "Add", new Type[] { typeof(string), typeof(string) }),
                              new HarmonyMethod(typeof(Patch_Strings_Interceptor), nameof(Patch_Strings_Interceptor.Prefix)));

                global::Debug.Log("[TranslationLoaderLite] Core Interceptor Hooked.");
            }
            catch (Exception ex)
            {
                global::Debug.LogError("[TranslationLoaderLite] OnLoad Crash: " + ex);
            }
        }
    }
}
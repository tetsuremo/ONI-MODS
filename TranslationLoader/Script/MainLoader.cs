using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Reflection;

namespace TranslationLoaderLite
{
    public class TranslationLoaderMod : UserMod2
    {
        public static ModSettings Settings { get; private set; }

        public override void OnLoad(Harmony harmony)
        {
            try
            {
                // 1. 初始化 PLib 环境
                PUtil.InitLibrary();

                // 2. 搜索并自动初始化翻译文件，优先于界面配置注册，确保按钮和标签渲染时能拿到翻译
                TranslationManager.Initialize();

                // 3. 读取并注册本 Mod 自己的设置菜单
                Settings = POptions.ReadSettings<ModSettings>() ?? new ModSettings();
                var options = new POptions();
                options.RegisterOptions(this, typeof(ModSettings));

                if (Settings.EnableOverride)
                {
                    // 4. 完美挂载到原版本地化总闸的后置位
                    var originalInit = AccessTools.Method(typeof(Localization), nameof(Localization.Initialize));
                    if (originalInit != null)
                    {
                        harmony.Patch(originalInit, postfix: new HarmonyMethod(typeof(TranslationLoaderMod), nameof(TranslationLoaderMod.UniversalAbsoluteOverwrite)));
                        global::Debug.Log("[TranslationLoaderLite] Pure Overwrite Core fully armed for production.");
                    }
                }
            }
            catch (Exception ex)
            {
                global::Debug.LogError("[TranslationLoaderLite] Critical load error: " + ex);
            }
        }

        // 👑 绝对统治力的终局清洗核心
        public static void UniversalAbsoluteOverwrite()
        {
            var translations = TranslationManager.LocalTranslations;
            if (translations == null || translations.Count == 0) return;

            try
            {
                // 1. 先发物理直刷：将扩充字典中的所有 Key 直接拍进游戏底层的 StringEntry
                foreach (var kvp in translations)
                {
                    StringKey stringKey = new StringKey(kvp.Key);
                    if (Strings.TryGet(stringKey, out var entry) && entry != null)
                    {
                        entry.String = kvp.Value;
                    }
                }

                // 2. 调用原生 Overload 强行修改全域已注册程序集内的静态 LocString
                Localization.OverloadStrings(translations);

                // 3. 仿 PLib 全量反射同步补刀：把更新后的 LocString 字段再次同步回全局字典，确保彻底消灭缓存
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly == null) continue;
                    RewriteStringsLikePLib(assembly);
                }
            }
            catch (Exception ex)
            {
                global::Debug.LogError("[TranslationLoaderLite] Overwrite workflow encountered error: " + ex);
            }
        }

        private static void RewriteStringsLikePLib(Assembly assembly)
        {
            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type == null || !type.IsClass) continue;

                    foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic))
                    {
                        if (field.FieldType == typeof(LocString))
                        {
                            var locString = (LocString)field.GetValue(null);
                            if (locString != null && !string.IsNullOrEmpty(locString.key.String))
                            {
                                Strings.Add(new string[] { locString.key.String, locString.text });
                            }
                        }
                    }
                }
            }
            catch
            {
                // 隐式忽略受保护系统程序集
            }
        }
    }
}
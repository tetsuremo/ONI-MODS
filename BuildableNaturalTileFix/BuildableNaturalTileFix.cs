using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Options;
using System.Reflection;

namespace BuildableNaturalTileFix
{
    public sealed class Mod : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);

            // 初始化 PLib
            PUtil.InitLibrary();

            // 注册 PLib 翻译
            new PLocalization().Register(Assembly.GetExecutingAssembly());

            // 注册 Mod 设置
            new POptions().RegisterOptions(this, typeof(ConfigOptions));

            // ✅ 从 config.json 读取配置
            ConfigOptions.Load();

            // 应用 Harmony 补丁
            harmony.PatchAll();
        }
    }
}

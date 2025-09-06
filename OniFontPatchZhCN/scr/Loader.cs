using HarmonyLib;
using KMod;

namespace TetsuRemo.OniFontPatchZhCN
{
    public class Loader : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            harmony.PatchAll();
            string root = base.mod.file_source.GetRoot();
            FontConfig config = FontConfig.Load(root);
            Patcher.Fonts = CustomFontAssets.Load(root, config);
        }
    }
}

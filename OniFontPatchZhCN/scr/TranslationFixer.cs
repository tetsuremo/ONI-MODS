using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslateFixMod;

namespace TetsuRemo.OniFontPatchZhCN
{
    public static class TranslationFixer
    {
        public static int EntryCount => translateDictionary?.Count ?? 0;
        private static Dictionary<string, string> translateDictionary;

        public static void LoadPoFile(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException(path);
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            translateDictionary = PoFileReader.LoadPoFile(path);
        }

        public static void Initialize(Harmony harmony)
        {
            if (translateDictionary == null) return;
            // 这里安装 Harmony 钩子，例如 Postfix Patch
            var patchType = typeof(TestTranslatePath); // 之前的 Postfix 类
            harmony.PatchAll(patchType.Assembly);
        }
    }
}

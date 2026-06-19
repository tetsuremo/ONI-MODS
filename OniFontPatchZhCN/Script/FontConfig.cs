using System.IO;
using UnityEngine;

namespace OniFontPatchZhCN
{
    public static class FontConfig
    {
        // 资产包与资产名称定义
        public const string BundleWin = "fonts_bundle_win";
        public const string BundleOther = "fonts_bundle_other";

        public const string HeaderFontAssetName = "myheaderfont SDF.asset";
        public const string TitleFontAssetName = "mytitlefont SDF.asset";
        public const string DescriptionFontAssetName = "mydescriptionfont SDF.asset";

        // 根据当前运行平台获取对应的 AssetBundle 文件名
        public static string GetPlatformBundleName()
        {
            return (Application.platform == RuntimePlatform.WindowsPlayer) ? BundleWin : BundleOther;
        }
    }
}
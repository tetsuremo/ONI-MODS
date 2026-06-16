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

        // 字体样式分流阈值 (可根据排版视觉效果微调)
        public const int TitleSizeThreshold = 22;       // 大于等于 22 像素使用大标题字体
        public const int HeaderSizeThreshold = 14;      // 14 到 21 像素之间使用中标题字体

        //根据当前运行平台获取对应的 AssetBundle 文件名
        public static string GetPlatformBundleName()
        {
            return (Application.platform == RuntimePlatform.WindowsPlayer) ? BundleWin : BundleOther;
        }
    }
}
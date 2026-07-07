using PeterHan.PLib.Options;
using System;
using System.IO;

namespace TranslationLoaderLite
{
    [ConfigFile("translation_loader_lite.json", true, true)]
    [RestartRequired]
    public class ModSettings
    {
        // 1. 开关项：绑定你的第一个 msgctxt
        [Option("TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_NAME",
                "TranslationLoaderLite.TranslationLoaderLiteStrings.OPTIONS.KEYPLACEHOLDER_TOOLTIP",
                "General")]
        public bool EnableOverride { get; set; } = true;

        // 2. 按钮 A：打开 .po 所在的文件夹（这里如果你在 .po 里面也加了对应的翻译，可以用 Key 替换，这里我先为你写上标准规范路径）
        // 如果需要你也可以把它们加进你的 .po 里作为自定义翻译，格式和上面开关一样
        [Option("Open Translation Folder", "Click this button to open the config folder where zh.po resides.", "Actions")]
        public System.Action OpenFolder => () =>
        {
            try
            {
                if (Directory.Exists(TranslationManager.ConfigFolderPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = TranslationManager.ConfigFolderPath,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
            }
            catch (Exception ex)
            {
                global::Debug.LogError("[TranslationLoaderLite] Failed to open folder: " + ex);
            }
        };

        // 3. 按钮 B：用模板覆盖现有文件（重置对应语言 .po）
        [Option("Reset/Overwrite with Template", "Clicking this will FORCE overwrite your existing zh.po with the built-in template.", "Actions")]
        public System.Action OverwriteTemplate => () =>
        {
            try
            {
                TranslationManager.ForceGenerateTemplate();
                // 写入后立即重新初始化加载一次，让热重置立即生效
                TranslationManager.Initialize();
                global::Debug.Log("[TranslationLoaderLite] Option Action: zh.po has been successfully reset to template.");
            }
            catch (Exception ex)
            {
                global::Debug.LogError("[TranslationLoaderLite] Failed to force reset template: " + ex);
            }
        };
    }
}
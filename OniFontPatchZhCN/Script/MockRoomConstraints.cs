using System;
using STRINGS;

namespace OniFontPatchZhCN
{
    public class MockRoomConstraints
    {
        // 使用反射安全地创建约束，避免编译时API不匹配
        public static void InitializeRoomConstraints()
        {
            try
            {
                // 这里只是提供字符串模板，实际的约束对象在运行时通过反射处理
                // 具体的约束修复逻辑在 GameTranslationFixer 中处理
                Debug.Log("[OniFontPatchZhCN] Mock room constraints initialized");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OniFontPatchZhCN] Failed to initialize mock room constraints: {ex}");
            }
        }

        // 提供字符串模板供翻译系统使用
        public static class ConstraintTemplates
        {
            public static string CEILING_HEIGHT_4_NAME = string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.NAME, "4");
            public static string CEILING_HEIGHT_4_DESC = string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.DESCRIPTION, "4");

            public static string CEILING_HEIGHT_6_NAME = string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.NAME, "6");
            public static string CEILING_HEIGHT_6_DESC = string.Format(ROOMS.CRITERIA.CEILING_HEIGHT.DESCRIPTION, "6");

            public static string MINIMUM_SIZE_12_NAME = string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.NAME, "12");
            public static string MINIMUM_SIZE_12_DESC = string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, "12");

            public static string MINIMUM_SIZE_24_NAME = string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.NAME, "24");
            public static string MINIMUM_SIZE_24_DESC = string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, "24");

            public static string MINIMUM_SIZE_32_NAME = string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.NAME, "32");
            public static string MINIMUM_SIZE_32_DESC = string.Format(ROOMS.CRITERIA.MINIMUM_SIZE.DESCRIPTION, "32");

            public static string MAXIMUM_SIZE_64_NAME = string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "64");
            public static string MAXIMUM_SIZE_64_DESC = string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "64");

            public static string MAXIMUM_SIZE_96_NAME = string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "96");
            public static string MAXIMUM_SIZE_96_DESC = string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "96");

            public static string MAXIMUM_SIZE_120_NAME = string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "120");
            public static string MAXIMUM_SIZE_120_DESC = string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "120");

            public static string MAXIMUM_SIZE_240_NAME = string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "240");
            public static string MAXIMUM_SIZE_240_DESC = string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "240");

            public static string MAXIMUM_SIZE_480_NAME = string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.NAME, "480");
            public static string MAXIMUM_SIZE_480_DESC = string.Format(ROOMS.CRITERIA.MAXIMUM_SIZE.DESCRIPTION, "480");

            public static string DECORATIVE_ITEM_1_NAME = string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM.NAME, 1);
            public static string DECORATIVE_ITEM_1_DESC = string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM.DESCRIPTION, 1);

            public static string DECORATIVE_ITEM_2_NAME = string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM.NAME, 2);
            public static string DECORATIVE_ITEM_2_DESC = string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM.DESCRIPTION, 2);

            public static string DECORATIVE_ITEM_20_NAME = string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM.NAME, "20");
            public static string DECORATIVE_ITEM_20_DESC = string.Format(ROOMS.CRITERIA.DECORATIVE_ITEM.DESCRIPTION, "20");
        }
    }
}

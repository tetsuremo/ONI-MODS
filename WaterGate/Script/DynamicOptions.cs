using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace WaterGateKai
{
    /// <summary>
    /// 在运行时动态创建包含所有液体的枚举类型
    /// </summary>
    public static class DynamicEnumCreator
    {
        private static Type dynamicLiquidType = null;

        public static Type GetDynamicLiquidType()
        {
            if (dynamicLiquidType != null)
                return dynamicLiquidType;

            try
            {
                var liquids = ElementLoader.elements
                    .Where(e => e.IsLiquid && e.id != SimHashes.Vacuum)
                    .OrderBy(e => GetElementDisplayName(e.id))
                    .ToArray();

                // 创建动态程序集
                var asmName = new AssemblyName("WaterGateKai_DynamicEnums");
                var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
                var moduleBuilder = asmBuilder.DefineDynamicModule("MainModule");

                // 创建枚举类型
                var enumBuilder = moduleBuilder.DefineEnum(
                    "WaterGateKai.DynamicLiquidType",
                    TypeAttributes.Public,
                    typeof(int));

                // 添加所有液体作为枚举值
                for (int i = 0; i < liquids.Length; i++)
                {
                    string fieldName = liquids[i].id.ToString();
                    enumBuilder.DefineLiteral(fieldName, i);
                }

                dynamicLiquidType = enumBuilder.CreateType();
                Debug.Log($"[WaterGateKai] Created dynamic liquid enum with {liquids.Length} values");

                return dynamicLiquidType;
            }
            catch (Exception e)
            {
                Debug.LogError($"[WaterGateKai] Error creating dynamic liquid enum: {e}");
                return typeof(LiquidType); // 回退到默认枚举
            }
        }

        private static string GetElementDisplayName(SimHashes elementId)
        {
            try
            {
                string key = $"STRINGS.ELEMENTS.{elementId.ToString().ToUpperInvariant()}.NAME";
                string name = Strings.Get(key);

                if (name == key || string.IsNullOrEmpty(name))
                    return elementId.ToString();

                return name;
            }
            catch
            {
                return elementId.ToString();
            }
        }

        /// <summary>
        /// 将动态枚举值转换为 SimHashes
        /// </summary>
        public static SimHashes ConvertToSimHash(object enumValue)
        {
            if (enumValue == null) return SimHashes.Vacuum;

            string enumName = enumValue.ToString();
            if (Enum.TryParse<SimHashes>(enumName, out var result))
                return result;

            return SimHashes.Vacuum;
        }
    }
}
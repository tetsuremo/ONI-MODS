using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BuildingRegistrationTool
{
    /// <summary>
    /// 建筑注册配置
    /// </summary>
    public class BuildingConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Effect { get; set; }
        public int Width { get; set; } = 1;
        public int Height { get; set; } = 1;
        public float ConstructionTime { get; set; } = 3f;
        public string[] ConstructionMaterials { get; set; }
        public float[] ConstructionMass { get; set; }
        public string BuildMenuCategory { get; set; } = "Base";
        public string BuildMenuSubcategory { get; set; } = "Tile";
        public string Tech { get; set; } = "FarmingTech";
        public bool IsSolidTile { get; set; } = false;
    }

    /// <summary>
    /// 建筑注册工具主类
    /// </summary>
    public static class BuildingRegistrar
    {
        private static readonly Dictionary<string, BuildingConfig> buildingConfigs = new Dictionary<string, BuildingConfig>();
        private static bool isInitialized = false;

        /// <summary>
        /// 注册建筑配置
        /// </summary>
        public static void RegisterBuilding(BuildingConfig config)
        {
            try
            {
                if (config == null)
                {
                    PUtil.LogError("[BuildingRegistrar] Building config is null");
                    return;
                }

                if (string.IsNullOrEmpty(config.Id))
                {
                    PUtil.LogError("[BuildingRegistrar] Building ID is null or empty");
                    return;
                }

                string normalizedId = config.Id.ToUpper();
                buildingConfigs[normalizedId] = config;

                PUtil.LogDebug($"[BuildingRegistrar] Registered building: {normalizedId}");
                PUtil.LogDebug($"[BuildingRegistrar] - Name: {config.Name}");
                PUtil.LogDebug($"[BuildingRegistrar] - Category: {config.BuildMenuCategory}/{config.BuildMenuSubcategory}");
            }
            catch (Exception e)
            {
                PUtil.LogError($"[BuildingRegistrar] Error registering building {config?.Id}: {e}");
            }
        }

        /// <summary>
        /// 应用所有建筑翻译
        /// </summary>
        public static void ApplyBuildingTranslations()
        {
            try
            {
                PUtil.LogDebug($"[BuildingRegistrar] Applying translations for {buildingConfigs.Count} buildings");

                foreach (var kvp in buildingConfigs)
                {
                    string buildingId = kvp.Key;
                    var config = kvp.Value;

                    string prefix = $"STRINGS.BUILDINGS.PREFABS.{buildingId}.";

                    // 注册建筑字符串
                    Strings.Add(prefix + "NAME", config.Name);
                    Strings.Add(prefix + "DESC", config.Description);
                    Strings.Add(prefix + "EFFECT", config.Effect);

                    PUtil.LogDebug($"[BuildingRegistrar] Applied translation for: {buildingId}");
                }

                PUtil.LogDebug("[BuildingRegistrar] All building translations applied");
            }
            catch (Exception e)
            {
                PUtil.LogError($"[BuildingRegistrar] Error applying translations: {e}");
            }
        }

        /// <summary>
        /// 获取所有已注册的建筑ID
        /// </summary>
        public static List<string> GetRegisteredBuildingIds()
        {
            return new List<string>(buildingConfigs.Keys);
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        public static BuildingConfig GetBuildingConfig(string buildingId)
        {
            string normalizedId = buildingId.ToUpper();
            return buildingConfigs.ContainsKey(normalizedId) ? buildingConfigs[normalizedId] : null;
        }

        /// <summary>
        /// 检查是否已注册
        /// </summary>
        public static bool IsBuildingRegistered(string buildingId)
        {
            return buildingConfigs.ContainsKey(buildingId.ToUpper());
        }

        /// <summary>
        /// 初始化工具
        /// </summary>
        public static void Initialize()
        {
            if (isInitialized) return;

            try
            {
                PUtil.LogDebug("[BuildingRegistrar] Initializing building registration tool");
                isInitialized = true;
            }
            catch (Exception e)
            {
                PUtil.LogError($"[BuildingRegistrar] Error during initialization: {e}");
            }
        }
    }

    /// <summary>
    /// Harmony 补丁 - 在 Db.Initialize 时应用翻译
    /// </summary>
    [HarmonyPatch(typeof(Db), "Initialize")]
    public static class Db_Initialize_Patch
    {
        public static void Postfix()
        {
            try
            {
                PUtil.LogDebug("[BuildingRegistrar] Db.Initialize detected, applying building translations");
                BuildingRegistrar.ApplyBuildingTranslations();
            }
            catch (Exception e)
            {
                PUtil.LogError($"[BuildingRegistrar] Error in Db.Initialize patch: {e}");
            }
        }
    }

    /// <summary>
    /// Harmony 补丁 - 在 GeneratedBuildings 加载时注册建筑
    /// </summary>
    [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
    public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
    {
        public static void Prefix()
        {
            try
            {
                PUtil.LogDebug("[BuildingRegistrar] GeneratedBuildings.LoadGeneratedBuildings detected");

                // 这里可以添加自动建筑注册逻辑
                // 或者让主 Mod 自己处理建筑注册
            }
            catch (Exception e)
            {
                PUtil.LogError($"[BuildingRegistrar] Error in GeneratedBuildings patch: {e}");
            }
        }
    }

    /// <summary>
    /// 工具 Mod 类
    /// </summary>
    public class BuildingRegistrationToolMod : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            try
            {
                PUtil.InitLibrary();
                PUtil.LogDebug("[BuildingRegistrar] Building registration tool loaded");

                // 初始化工具
                BuildingRegistrar.Initialize();

                // 应用 Harmony 补丁
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                PUtil.LogDebug("[BuildingRegistrar] Harmony patches applied");
            }
            catch (Exception e)
            {
                PUtil.LogError($"[BuildingRegistrar] Error during OnLoad: {e}");
            }
        }
    }
}
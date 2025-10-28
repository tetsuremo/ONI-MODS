using UnityEngine;
using TUNING;

namespace WaterGateKai
{
    /// <summary>
    /// 两格水门
    /// </summary>
    public class WaterGate1x2Config : IBuildingConfig
    {
        public const string ID = "PiapiWaterGate1x2";

        public override BuildingDef CreateBuildingDef()
        {
            float[] construction_mass = new float[] { 100f };
            string[] construction_materials = MATERIALS.RAW_MINERALS;

            BuildingDef def = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 2,
                anim: "piapi_watergate1x2_kanim",
                hitpoints: 100,
                construction_time: ConfigOptions.Instance.BuildSpeed,
                construction_mass: construction_mass,
                construction_materials: construction_materials,
                melting_point: 1600f,
                build_location_rule: BuildLocationRule.Anywhere,
                decor: BUILDINGS.DECOR.NONE,
                noise: NOISE_POLLUTION.NONE
            );

            def.Floodable = false;
            def.Overheatable = false;
            def.Entombable = false;
            def.AudioCategory = "Metal";
            def.AudioSize = "small";
            def.BaseTimeUntilRepair = -1f;
            def.ViewMode = OverlayModes.None.ID;
            def.SceneLayer = Grid.SceneLayer.Building;
            def.ObjectLayer = ObjectLayer.Building;
            def.PermittedRotations = PermittedRotations.Unrotatable;
            def.DefaultAnimState = "on";

            return def;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            go.AddOrGet<WaterGate1x2Behaviour>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<BuildingComplete>();
        }
    }

    /// <summary>
    /// 三格水门
    /// </summary>
    public class WaterGate1x3Config : IBuildingConfig
    {
        public const string ID = "PiapiWaterGate1x3";

        public override BuildingDef CreateBuildingDef()
        {
            float[] construction_mass = new float[] { 200f };
            string[] construction_materials = MATERIALS.RAW_MINERALS;

            BuildingDef def = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 3,
                anim: "piapi_watergate1x3_kanim",
                hitpoints: 100,
                construction_time: ConfigOptions.Instance.BuildSpeed,
                construction_mass: construction_mass,
                construction_materials: construction_materials,
                melting_point: 1600f,
                build_location_rule: BuildLocationRule.Anywhere,
                decor: BUILDINGS.DECOR.NONE,
                noise: NOISE_POLLUTION.NONE
            );

            def.Floodable = false;
            def.Overheatable = false;
            def.Entombable = false;
            def.AudioCategory = "Metal";
            def.AudioSize = "small";
            def.BaseTimeUntilRepair = -1f;
            def.ViewMode = OverlayModes.None.ID;
            def.SceneLayer = Grid.SceneLayer.Building;
            def.ObjectLayer = ObjectLayer.Building;
            def.PermittedRotations = PermittedRotations.Unrotatable;
            def.DefaultAnimState = "on";

            return def;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            go.AddOrGet<WaterGate1x3Behaviour>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<BuildingComplete>();
        }
    }

    /// <summary>
    /// 四格水门
    /// </summary>
    public class WaterGate1x4Config : IBuildingConfig
    {
        public const string ID = "PiapiWaterGate1x4";

        public override BuildingDef CreateBuildingDef()
        {
            float[] construction_mass = new float[] { 300f };
            string[] construction_materials = MATERIALS.RAW_MINERALS;

            BuildingDef def = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 4,
                anim: "piapi_watergate1x4_kanim",
                hitpoints: 100,
                construction_time: ConfigOptions.Instance.BuildSpeed,
                construction_mass: construction_mass,
                construction_materials: construction_materials,
                melting_point: 1600f,
                build_location_rule: BuildLocationRule.Anywhere,
                decor: BUILDINGS.DECOR.NONE,
                noise: NOISE_POLLUTION.NONE
            );

            def.Floodable = false;
            def.Overheatable = false;
            def.Entombable = false;
            def.AudioCategory = "Metal";
            def.AudioSize = "small";
            def.BaseTimeUntilRepair = -1f;
            def.ViewMode = OverlayModes.None.ID;
            def.SceneLayer = Grid.SceneLayer.Building;
            def.ObjectLayer = ObjectLayer.Building;
            def.PermittedRotations = PermittedRotations.Unrotatable;
            def.DefaultAnimState = "on";

            return def;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            go.AddOrGet<WaterGate1x4Behaviour>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<BuildingComplete>();
        }
    }

    /// <summary>
    /// 凝胶门（使用树脂）
    /// </summary>

    public class GelGateConfig : IBuildingConfig
    {
        public const string ID = "PiapiGelGate";

        public override BuildingDef CreateBuildingDef()
        {
            float[] construction_mass = new float[] { 200f };
            string[] construction_materials = new string[] { MATERIALS.TRANSPARENT };

            BuildingDef def = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 1,
                anim: "piapi_gelgate_kanim",
                hitpoints: 100,
                construction_time: 10f,
                construction_mass: construction_mass,
                construction_materials: construction_materials,
                melting_point: 1600f,
                build_location_rule: BuildLocationRule.Anywhere,
                decor: BUILDINGS.DECOR.NONE,
                noise: NOISE_POLLUTION.NONE
            );

            def.Floodable = false;
            def.Overheatable = false;
            def.Entombable = false;
            def.AudioCategory = "Plastic";
            def.AudioSize = "small";
            def.BaseTimeUntilRepair = -1f;
            def.ViewMode = OverlayModes.None.ID;
            def.SceneLayer = Grid.SceneLayer.Building;
            def.ObjectLayer = ObjectLayer.Building;
            def.PermittedRotations = PermittedRotations.Unrotatable;
            def.BuildLocationRule = BuildLocationRule.Anywhere;
            def.DefaultAnimState = "on";

            return def;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            go.AddOrGet<GelGateBehaviour>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<BuildingComplete>();
        }
    }
}

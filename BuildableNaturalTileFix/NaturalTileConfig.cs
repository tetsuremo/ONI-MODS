using UnityEngine;
using TUNING;

namespace BuildableNaturalTileFix
{
    public class NaturalTileConfig : IBuildingConfig
    {
        public const string ID = "BUILDABLENATURALTILEFIX_NATURALTILE";

        public override BuildingDef CreateBuildingDef()
        {
            // 使用配置中的质量
            float[] constructionMass = new float[] { ConfigOptions.Instance.BuildMass };

            // 使用所有固体材料，但建筑菜单会根据当前可用材料过滤
            BuildingDef def = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 1,
                anim: "natural_tile_kanim",
                hitpoints: 100,
                construction_time: ConfigOptions.Instance.BuildSpeed,
                construction_mass: constructionMass,
                construction_materials: MATERIALS.ANY_BUILDABLE,
                melting_point: 1600f,
                build_location_rule: BuildLocationRule.Tile,
                decor: BUILDINGS.DECOR.BONUS.TIER0,
                noise: NOISE_POLLUTION.NONE
            );

            def.Floodable = false;
            def.Entombable = false;
            def.Overheatable = false;
            def.UseStructureTemperature = false;
            def.IsFoundation = true;
            def.TileLayer = ObjectLayer.FoundationTile;
            def.ReplacementLayer = ObjectLayer.ReplacementTile;
            def.AudioCategory = "Metal";
            def.AudioSize = "small";
            def.BaseTimeUntilRepair = -1f;
            def.SceneLayer = Grid.SceneLayer.TileMain;
            def.ConstructionOffsetFilter = BuildingDef.ConstructionOffsetFilter_OneDown;
            def.ShowInBuildMenu = true;
            def.DragBuild = true;

            // 地砖材质设置
            def.isKAnimTile = false;
            def.BlockTileAtlas = Assets.GetTextureAtlas("natural_tile");
            def.BlockTilePlaceAtlas = Assets.GetTextureAtlas("natural_tile_place");
            def.BlockTileShineAtlas = Assets.GetTextureAtlas("tiles_solid_shine");
            def.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
            def.BlockTileIsTransparent = false;

            def.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_solid_tops_info");
            def.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_solid_tops_place_info");

            def.ForegroundLayer = Grid.SceneLayer.TileMain;

            return def;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);

            go.AddOrGet<TileTemperature>();
            go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = TileConfig.BlockTileConnectorID;
            go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            go.AddOrGet<SimCellOccupier>().doReplaceElement = true;
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            go.AddOrGet<SimCellOccupier>().doReplaceElement = true;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            var kpid = go.GetComponent<KPrefabID>();
            if (kpid != null)
                kpid.AddTag(GameTags.FloorTiles, false);

            var primary = go.AddOrGet<PrimaryElement>();
            primary.SetElement(SimHashes.Dirt); // 默认元素，实际建造时会使用选择的材料
            primary.Mass = ConfigOptions.Instance.BlockMass;

            var occupier = go.AddOrGet<SimCellOccupier>();
            occupier.doReplaceElement = true;
            occupier.notifyOnMelt = true;

            go.AddOrGet<BuildingComplete>();
        }
    }
}
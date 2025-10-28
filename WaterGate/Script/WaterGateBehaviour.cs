using UnityEngine;
using System;

namespace WaterGateKai
{
    /// <summary>
    /// 两格水门行为（按建材类型生成不同液体）
    /// </summary>
    public class WaterGate1x2Behaviour : KMonoBehaviour
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();
            Debug.Log($"[WaterGateKai] WaterGate1x2 spawned at {transform.position}");

            int cell = Grid.PosToCell(this);
            var primaryElement = GetComponent<PrimaryElement>();

            if (primaryElement == null)
            {
                Debug.LogWarning("[WaterGateKai] Missing PrimaryElement component.");
                return;
            }

            SimHashes bottomElement, topElement;
            float bottomMass, topMass;

            GetLiquidsForMaterial(primaryElement.ElementID, out bottomElement, out topElement, out bottomMass, out topMass);

            // 生成下层液体
            SimMessages.ReplaceElement(cell, bottomElement, CellEventLogger.Instance.SimCellOccupierDestroySelf,
                bottomMass, primaryElement.Temperature, byte.MaxValue, 0, -1);

            // 生成上层液体 - 固定质量
            SimMessages.ReplaceElement(Grid.CellAbove(cell), topElement, CellEventLogger.Instance.SimCellOccupierDestroySelf,
                topMass, primaryElement.Temperature, byte.MaxValue, 0, -1);

            // 下一帧销毁建筑
            GameScheduler.Instance.ScheduleNextFrame("DestroyWaterGate1x2", _ => DestroyBuilding(), null, null);
        }

        private void GetLiquidsForMaterial(SimHashes material, out SimHashes bottomElement, out SimHashes topElement, out float bottomMass, out float topMass)
        {
            bottomMass = ConfigOptions.Instance.BottomLiquidMass > 0 ? ConfigOptions.Instance.BottomLiquidMass : 0.111f;

            // 上层液体固定质量，不依赖基础质量
            switch (material)
            {
                case SimHashes.IgneousRock:
                    // 火成岩：盐水 + 水
                    bottomElement = SimHashes.SaltWater;
                    topElement = SimHashes.Water;
                    topMass = 0.03f; // 固定质量
                    break;
                case SimHashes.Granite:
                    // 花岗岩：浓盐水 + 乙醇
                    bottomElement = SimHashes.Brine;
                    topElement = SimHashes.Ethanol;
                    topMass = 0.03f; // 固定质量
                    break;
                default:
                    // 其他材料：原油 + 石油
                    bottomElement = SimHashes.CrudeOil;
                    topElement = SimHashes.Petroleum;
                    topMass = 0.111f; // 固定质量
                    break;
            }
        }

        private void DestroyBuilding()
        {
            if (this != null && gameObject != null)
                Util.KDestroyGameObject(gameObject);
        }
    }

    /// <summary>
    /// 三格水门行为
    /// </summary>
    public class WaterGate1x3Behaviour : KMonoBehaviour
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();
            Debug.Log($"[WaterGateKai] WaterGate1x3 spawned at {transform.position}");

            int bottomCell = Grid.PosToCell(this);
            int middleCell = Grid.CellAbove(bottomCell);
            int topCell = Grid.CellAbove(middleCell);

            var primaryElement = GetComponent<PrimaryElement>();
            float temperature = primaryElement?.Temperature ?? 283f;
            float baseMass = ConfigOptions.Instance.BottomLiquidMass > 0 ? ConfigOptions.Instance.BottomLiquidMass : 0.111f;

            // 从上到下：水、石油、原油
            // 上层和中层使用固定质量
            SimMessages.ReplaceElement(topCell, SimHashes.Water, CellEventLogger.Instance.SimCellOccupierDestroySelf,
                0.03f, temperature, byte.MaxValue, 0, -1); // 固定质量
            SimMessages.ReplaceElement(middleCell, SimHashes.Petroleum, CellEventLogger.Instance.SimCellOccupierDestroySelf,
                0.111f, temperature, byte.MaxValue, 0, -1); // 固定质量
            SimMessages.ReplaceElement(bottomCell, SimHashes.CrudeOil, CellEventLogger.Instance.SimCellOccupierDestroySelf,
                baseMass, temperature, byte.MaxValue, 0, -1); // 只有底层受配置影响

            GameScheduler.Instance.ScheduleNextFrame("DestroyWaterGate1x3", _ => DestroyBuilding(), null, null);
        }

        private void DestroyBuilding()
        {
            if (this != null && gameObject != null)
                Util.KDestroyGameObject(gameObject);
        }
    }

    /// <summary>
    /// 四格水门行为
    /// </summary>
    public class WaterGate1x4Behaviour : KMonoBehaviour
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();
            Debug.Log($"[WaterGateKai] WaterGate1x4 spawned at {transform.position}");

            int bottomCell = Grid.PosToCell(this);
            int cell1 = Grid.CellAbove(bottomCell);
            int cell2 = Grid.CellAbove(cell1);
            int topCell = Grid.CellAbove(cell2);

            var primaryElement = GetComponent<PrimaryElement>();
            float temperature = primaryElement?.Temperature ?? 283f;
            float baseMass = ConfigOptions.Instance.BottomLiquidMass > 0 ? ConfigOptions.Instance.BottomLiquidMass : 0.111f;

            // 从上到下：水、盐水、乙醇、原油
            // 上层和中层使用固定质量
            SimMessages.ReplaceElement(topCell, SimHashes.Water, CellEventLogger.Instance.SimCellOccupierDestroySelf,
                0.03f, temperature, byte.MaxValue, 0, -1); // 固定质量
            SimMessages.ReplaceElement(cell2, SimHashes.Brine, CellEventLogger.Instance.SimCellOccupierDestroySelf,
                0.03f, temperature, byte.MaxValue, 0, -1); // 固定质量
            SimMessages.ReplaceElement(cell1, SimHashes.Ethanol, CellEventLogger.Instance.SimCellOccupierDestroySelf,
                0.03f, temperature, byte.MaxValue, 0, -1); // 固定质量
            SimMessages.ReplaceElement(bottomCell, SimHashes.CrudeOil, CellEventLogger.Instance.SimCellOccupierDestroySelf,
                baseMass, temperature, byte.MaxValue, 0, -1); // 只有底层受配置影响

            GameScheduler.Instance.ScheduleNextFrame("DestroyWaterGate1x4", _ => DestroyBuilding(), null, null);
        }

        private void DestroyBuilding()
        {
            if (this != null && gameObject != null)
                Util.KDestroyGameObject(gameObject);
        }
    }

    /// <summary>
    /// 凝胶门行为（保持不变）
    /// </summary>
    public class GelGateBehaviour : KMonoBehaviour
    {
        protected override void OnSpawn()
        {
            base.OnSpawn();
            Debug.Log($"[WaterGateKai] GelGate spawned at {transform.position}");

            int cell = Grid.PosToCell(this);
            var primaryElement = GetComponent<PrimaryElement>();
            float temperature = primaryElement?.Temperature ?? 283f;

            // 固定生成 101kg 的 ViscoGel
            SimMessages.ReplaceElement(cell, SimHashes.ViscoGel, CellEventLogger.Instance.SimCellOccupierDestroySelf,
                101f, temperature, byte.MaxValue, 0, -1);

            GameScheduler.Instance.ScheduleNextFrame("DestroyGelGate", _ => DestroyBuilding(), null, null);
        }

        private void DestroyBuilding()
        {
            if (this != null && gameObject != null)
                Util.KDestroyGameObject(gameObject);
        }
    }
}
using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BatchModifyStyles
{
    public class BatchSkinTemplateManager
    {
        private TemplateType templateType = TemplateType.Building;
        private string lastBuildingTemplateID;
        private string lastBuildingType;
        private SavedOutfitData lastMinionTemplateData;

        private BatchSkinTool parentTool;

        public BatchSkinTemplateManager(BatchSkinTool parent)
        {
            parentTool = parent;
        }

        public TemplateType CurrentTemplateType => templateType;

        public void SetTemplateType(TemplateType type)
        {
            templateType = type;
        }

        public bool HasValidTemplate()
        {
            if (templateType == TemplateType.Building)
                return !string.IsNullOrEmpty(lastBuildingType);
            else
                return lastMinionTemplateData != null;
        }

        public void CheckAndSaveTemplateFromSelection()
        {
            GameObject selected = GetSelectedObject();
            if (selected != null)
            {
                Log.Info($"[BatchModifyStyles] Selected object: {selected.name}");

                // 先检测是否是复制人
                var minionIdentity = selected.GetComponent<MinionIdentity>();
                var wearable = selected.GetComponent<WearableAccessorizer>();

                if (minionIdentity != null && wearable != null)
                {
                    // 设置为复制人模式并保存
                    SetTemplateType(TemplateType.Minion);
                    SaveTemplate(selected);
                    Log.Info($"[BatchModifyStyles] Saved MINION template: {minionIdentity.name}");
                }
                // 再检测是否是建筑
                else if (IsValidBuildingTarget(selected))
                {
                    // 设置为建筑模式并保存
                    SetTemplateType(TemplateType.Building);
                    SaveTemplate(selected);
                    Log.Info($"[BatchModifyStyles] Saved BUILDING template");
                }
                else
                {
                    Log.Info($"[BatchModifyStyles] Object is not a valid template");
                }
            }
            else
            {
                Log.Info("[BatchModifyStyles] No object selected");
            }
        }

        private bool IsValidBuildingTarget(GameObject target)
        {
            var buildingComplete = target.GetComponent<BuildingComplete>();
            var prefabID = target.GetComponent<KPrefabID>();

            if (buildingComplete != null && prefabID != null)
            {
                var buildingDef = buildingComplete.Def;
                return buildingDef != null && buildingDef.AvailableFacades != null && buildingDef.AvailableFacades.Count > 0;
            }
            return false;
        }

        public int ApplyTemplateToArea(Vector3 start, Vector3 end)
        {
            int count = 0;

            if (templateType == TemplateType.Building)
            {
                Log.Info("[BatchModifyStyles] Applying BUILDING template to area");
                count = ApplyBuildingTemplate(start, end);
            }
            else if (templateType == TemplateType.Minion)
            {
                Log.Info("[BatchModifyStyles] Applying MINION template to area");
                count = ApplyMinionTemplate(start, end);
            }

            if (count > 0)
            {
                
            }
            else
            {
                Log.Info("[BatchModifyStyles] No matching targets found");
            }

            return count;
        }

        private GameObject GetSelectedObject()
        {
            try
            {
                if (SelectTool.Instance != null && SelectTool.Instance.selected != null)
                {
                    return SelectTool.Instance.selected.gameObject;
                }
                return null;
            }
            catch (Exception e)
            {
                Log.Error($"[BatchModifyStyles] Error getting selected object: {e}");
                return null;
            }
        }

        private bool IsValidTemplateTarget(GameObject target)
        {
            if (target == null) return false;

            if (templateType == TemplateType.Building)
            {
                var buildingComplete = target.GetComponent<BuildingComplete>();
                var prefabID = target.GetComponent<KPrefabID>();

                if (buildingComplete != null && prefabID != null)
                {
                    var buildingDef = buildingComplete.Def;
                    if (buildingDef != null && buildingDef.AvailableFacades != null && buildingDef.AvailableFacades.Count > 0)
                    {
                        return true;
                    }
                }
                var minionIdentity = target.GetComponent<MinionIdentity>();
                var wearable = target.GetComponent<WearableAccessorizer>();

                if (minionIdentity != null && wearable != null)
                {
                    return true;
                }
            }
            return false;
        }

        private void SaveTemplate(GameObject target)
        {
            if (target == null) return;

            if (templateType == TemplateType.Building)
                SaveBuildingTemplate(target);
            else
                SaveMinionTemplate(target);
        }

        private void SaveBuildingTemplate(GameObject target)
        {
            var buildingFacade = target.GetComponent<BuildingFacade>();
            var prefabID = target.GetComponent<KPrefabID>();

            if (prefabID != null)
            {
                lastBuildingType = prefabID.PrefabTag.Name;

                if (buildingFacade != null && !string.IsNullOrEmpty(buildingFacade.CurrentFacade))
                {
                    if (buildingFacade.CurrentFacade == "DEFAULT_FACADE" || buildingFacade.IsOriginal)
                    {
                        lastBuildingTemplateID = "";
                        Log.Info($"[BatchModifyStyles] Saved building template: Type={lastBuildingType}, Facade=Default");
                    }
                    else
                    {
                        lastBuildingTemplateID = buildingFacade.CurrentFacade;
                        Log.Info($"[BatchModifyStyles] Saved building template: Type={lastBuildingType}, Facade={lastBuildingTemplateID}");
                    }
                }
                else
                {
                    lastBuildingTemplateID = "";
                    Log.Info($"[BatchModifyStyles] Saved building template: Type={lastBuildingType}, Facade=Default");
                }
            }
        }

        private void SaveMinionTemplate(GameObject target)
        {
            var minion = target.GetComponent<MinionIdentity>();
            if (minion != null)
            {
                var wearable = minion.GetComponent<WearableAccessorizer>();
                if (wearable != null)
                {
                    Log.Info($"[BatchModifyStyles] Minion wearables count: {wearable.Wearables.Count}");
                    Log.Info($"[BatchModifyStyles] Custom clothing items count: {wearable.GetCustomClothingItems().Count}");

                    lastMinionTemplateData = new SavedOutfitData
                    {
                        wearables = new Dictionary<WearableAccessorizer.WearableType, WearableAccessorizer.Wearable>(wearable.Wearables),
                        customClothing = new Dictionary<ClothingOutfitUtility.OutfitType, List<ResourceRef<ClothingItemResource>>>(wearable.GetCustomClothingItems())
                    };
                    Log.Info("[BatchModifyStyles] Saved minion template successfully");
                }
            }
        }

        private int ApplyBuildingTemplate(Vector3 start, Vector3 end)
        {
            int count = 0;
            int startX, startY, endX, endY;
            Grid.PosToXY(start, out startX, out startY);
            Grid.PosToXY(end, out endX, out endY);

            if (endX < startX) Util.Swap(ref startX, ref endX);
            if (endY < startY) Util.Swap(ref startY, ref endY);

            Log.Info($"[BatchModifyStyles] Applying facade to area: ({startX},{startY}) to ({endX},{endY})");

            foreach (var building in Components.BuildingCompletes.Items)
            {
                if (building == null) continue;

                Vector3 pos = building.transform.GetPosition();
                int cellX, cellY;
                Grid.PosToXY(pos, out cellX, out cellY);

                if (cellX >= startX && cellX <= endX && cellY >= startY && cellY <= endY)
                {
                    var buildingFacade = building.GetComponent<BuildingFacade>();
                    var prefabID = building.GetComponent<KPrefabID>();

                    if (buildingFacade != null && prefabID != null && prefabID.PrefabTag.Name == lastBuildingType)
                    {
                        if (!string.IsNullOrEmpty(lastBuildingTemplateID))
                        {
                            var facadeResource = Db.GetBuildingFacades().TryGet(lastBuildingTemplateID);
                            if (facadeResource != null)
                            {
                                buildingFacade.ApplyBuildingFacade(facadeResource, false);
                                count++;
                            }
                        }
                        else
                        {
                            buildingFacade.ApplyDefaultFacade(false);
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        private int ApplyMinionTemplate(Vector3 start, Vector3 end)
        {
            int count = 0;
            int startX, startY, endX, endY;
            Grid.PosToXY(start, out startX, out startY);
            Grid.PosToXY(end, out endX, out endY);

            if (endX < startX) Util.Swap(ref startX, ref endX);
            if (endY < startY) Util.Swap(ref startY, ref endY);

            foreach (var minion in Components.LiveMinionIdentities.Items)
            {
                if (minion == null) continue;

                Vector3 pos = minion.transform.GetPosition();
                int cellX, cellY;
                Grid.PosToXY(pos, out cellX, out cellY);

                if (cellX >= startX && cellX <= endX && cellY >= startY && cellY <= endY)
                {
                    var wearable = minion.GetComponent<WearableAccessorizer>();
                    if (wearable != null && lastMinionTemplateData != null)
                    {
                        // 使用正确的 RestoreWearables 方法
                        wearable.RestoreWearables(
                            lastMinionTemplateData.wearables,
                            lastMinionTemplateData.customClothing
                        );
                        count++;
                    }
                }
            }
            return count;
        }

        private void ShowNotification(LocString message)
        {
            if (PopFXManager.Instance != null)
            {
                // 使用正确的 PopFX 方法
                Vector3 worldPos = PlayerController.GetCursorPos(KInputManager.GetMousePos());
                PopFXManager.Instance.SpawnFX(
                    PopFXManager.Instance.sprite_Plus, // 使用加号图标
                    message,
                    null,
                    worldPos,
                    1.5f, // 显示时间
                    false
                );
                Log.Info($"[BatchModifyStyles] Showed notification: {message}");
            }
        }
    }
}

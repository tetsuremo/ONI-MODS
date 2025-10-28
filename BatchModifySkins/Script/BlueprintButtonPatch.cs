using HarmonyLib;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace BatchModifyStyles
{
    [HarmonyPatch(typeof(DetailsScreen), "SelectSideScreenTab")]
    public class BlueprintButtonPatch
    {
        private static GameObject currentButton;

        public static void Postfix(DetailsScreen __instance, DetailsScreen.SidescreenTabTypes tabID)
        {
            if (tabID == DetailsScreen.SidescreenTabTypes.Blueprints)
            {
                __instance.StartCoroutine(CreateButtonInBlueprintTab(__instance));
            }
            else
            {
                HideButton();
            }
        }

        private static IEnumerator CreateButtonInBlueprintTab(DetailsScreen screen)
        {
            yield return new WaitForEndOfFrame();

            Log.Info("[BatchModifyStyles] === Creating blueprint button ===");

            if (currentButton != null)
            {
                currentButton.SetActive(true);
                yield break;
            }

            yield return CopyChangeMaterialButtonProperly(screen);
        }

        private static IEnumerator CopyChangeMaterialButtonProperly(DetailsScreen screen)
        {
            try
            {
                // 获取 Material 标签的按钮作为模板
                var materialTab = screen.GetTabOfType(DetailsScreen.SidescreenTabTypes.Material);
                if (materialTab == null || materialTab.bodyInstance == null)
                    yield break;

                var materialPanel = materialTab.bodyInstance.GetComponentInChildren<DetailsScreenMaterialPanel>();
                if (materialPanel == null)
                    yield break;

                FieldInfo buttonField = typeof(DetailsScreenMaterialPanel)
                    .GetField("openChangeMaterialPanelButton", BindingFlags.NonPublic | BindingFlags.Instance);
                if (buttonField == null)
                    yield break;

                KButton changeMaterialButton = buttonField.GetValue(materialPanel) as KButton;
                if (changeMaterialButton == null)
                    yield break;

                // 获取 Blueprints 标签下的 CosmeticsPanel
                var blueprintTab = screen.GetTabOfType(DetailsScreen.SidescreenTabTypes.Blueprints);
                if (blueprintTab == null || blueprintTab.bodyInstance == null)
                    yield break;

                var cosmeticsPanel = blueprintTab.bodyInstance
                    .GetComponent<HierarchyReferences>()
                    .GetReference<CosmeticsPanel>("CosmeticsPanel");

                if (cosmeticsPanel == null)
                    yield break;

                // 克隆按钮并挂到 CosmeticsPanel 下
                KButton newButton = GameObject.Instantiate(changeMaterialButton, cosmeticsPanel.transform);
                newButton.name = "BatchModifyButton";
                currentButton = newButton.gameObject;

                PlaceButtonAtBottomCenter(newButton);

                CustomizeButton(newButton);

                newButton.gameObject.SetActive(true);
                Log.Info("[BatchModifyStyles] ✓ Button created successfully");
            }
            catch (System.Exception e)
            {
                Log.Error($"[BatchModifyStyles] Error: {e}");
            }
        }

        private static void PlaceButtonAtBottomCenter(KButton button)
        {
            if (button == null) return;

            RectTransform btnRT = button.GetComponent<RectTransform>();
            if (btnRT == null) return;

            // 底部正中，固定大小，和"更换材料"按钮一样
            btnRT.SetParent(button.transform.parent, false);
            btnRT.anchorMin = new Vector2(0.5f, 0f);
            btnRT.anchorMax = new Vector2(0.5f, 0f);
            btnRT.pivot = new Vector2(0.5f, 0f);
            btnRT.sizeDelta = new Vector2(120f, 36f);
            btnRT.anchoredPosition = new Vector2(0f, 8f);
        }

        private static void CustomizeButton(KButton button)
        {
            LocText locText = button.GetComponentInChildren<LocText>();
            if (locText != null)
            {
                // 强制设置翻译文本
                locText.key = "STRINGS.UI.BATCH_MODIFY_SKINS.BATCH_MODIFY_SKIN";
                locText.text = Strings.Get("STRINGS.UI.BATCH_MODIFY_SKINS.BATCH_MODIFY_SKIN");
                Log.Info($"[BatchModifyStyles] Button text set to: {locText.text}");
            }

            ToolTip tooltip = button.GetComponent<ToolTip>();
            if (tooltip != null)
            {
                string tooltipText = Strings.Get("STRINGS.UI.BATCH_MODIFY_SKINS.BATCH_SKIN_TOOLTIP");
                tooltip.SetSimpleTooltip(tooltipText);
                Log.Info($"[BatchModifyStyles] Tooltip set to: {tooltipText}");
            }

            button.ClearOnClick();
            button.onClick += () => HandleBatchModify();
        }

        private static void HandleBatchModify()
        {
            try
            {
                Log.Info("[BatchModifyStyles] Batch modify button clicked");

                // 检查工具实例
                if (BatchSkinTool.Instance == null)
                {
                    Log.Error("[BatchModifyStyles] BatchSkinTool instance is null");
                    ShowErrorToast("Tool not initialized");
                    return;
                }

                // 检查是否有选中的对象
                SelectTool selectTool = SelectTool.Instance;
                if (selectTool == null || selectTool.selected == null)
                {
                    Log.Warn("[BatchModifyStyles] No building selected");
                    return;
                }

                GameObject selected = selectTool.selected.gameObject;
                Log.Info($"[BatchModifyStyles] Selected object: {selected.name}");

                // 检查是否是有效的目标
                if (!IsValidTarget(selected))
                {
                    Log.Warn($"[BatchModifyStyles] Invalid target: {selected.name}");
                    return;
                }

                // 激活工具
                BatchSkinTool.Instance.ActivateFromBlueprint();

                Log.Info("[BatchModifyStyles] Tool activation initiated");
            }
            catch (System.Exception e)
            {
                Log.Error($"[BatchModifyStyles] Error handling batch modify: {e}");
                ShowErrorToast("Error: " + e.Message);
            }
        }

        private static bool IsValidTarget(GameObject target)
        {
            if (target == null) return false;

            // 检查建筑
            var buildingFacade = target.GetComponent<BuildingFacade>();
            var buildingComplete = target.GetComponent<BuildingComplete>();
            if (buildingFacade != null && buildingComplete != null)
            {
                return true;
            }

            // 检查复制人
            var minion = target.GetComponent<MinionIdentity>();
            if (minion != null)
            {
                return true;
            }

            return false;
        }

        private static void ShowErrorToast(string message)
        {
            if (PopFXManager.Instance != null)
                PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, message,
                    null, PlayerController.GetCursorPos(KInputManager.GetMousePos()));

            Log.Warn($"[BatchModifyStyles] {message}");
        }

        private static void HideButton()
        {
            if (currentButton != null)
                currentButton.SetActive(false);
        }
    }
}

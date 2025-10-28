using UnityEngine;

namespace BatchModifyStyles
{
    public class BatchSkinInputHandler
    {
        private BatchSkinTool parentTool;
        private BatchSkinVisualizer visualizer;
        private BatchSkinTemplateManager templateManager;

        public BatchSkinInputHandler(BatchSkinTool parent, BatchSkinVisualizer viz, BatchSkinTemplateManager tm)
        {
            parentTool = parent;
            visualizer = viz;
            templateManager = tm;
        }

        public void Initialize()
        {
            Log.Info("[BatchModifyStyles] Input handler initialized");
        }

        public void OnLeftClickDown(Vector3 cursor_pos)
        {
            if (!parentTool.hasFocus) return;

            if (!templateManager.HasValidTemplate())
            {
                templateManager.CheckAndSaveTemplateFromSelection();
                if (!templateManager.HasValidTemplate()) return;
            }

            visualizer.StartDrag(cursor_pos);
        }

        public void OnLeftClickUp(Vector3 cursor_pos)
        {
            if (visualizer.IsDragging && templateManager.HasValidTemplate())
            {
                var (start, end) = visualizer.GetDragArea();
                visualizer.EndDrag();
                int count = templateManager.ApplyTemplateToArea(start, end);

                if (count > 0)
                    ShowNotification(FormatAppliedNotification(count));
            }
        }

        private string FormatAppliedNotification(int count)
        {
            try
            {
                // 使用 Strings.Get() 获取本地化字符串
                string formatString = Strings.Get("STRINGS.UI.BATCH_MODIFY_SKINS.APPLIED_TO_TARGETS");

                if (!string.IsNullOrEmpty(formatString))
                {
                    string result = string.Format(formatString, count);
                    return result;
                }
                else
                {
                    Log.Warn("[BatchModifyStyles] Format string is null or empty");
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"[BatchModifyStyles] Format error: {e}");
            }

            // 备用英文
            return $"Applied to {count} targets";
        }

        private void ShowNotification(string message)
        {
            if (PopFXManager.Instance != null)
            {
                Vector3 worldPos = PlayerController.GetCursorPos(KInputManager.GetMousePos());
                PopFXManager.Instance.SpawnFX(
                    PopFXManager.Instance.sprite_Plus,
                    message,
                    null,
                    worldPos,
                    2.0f,
                    false
                );
            }
        }
    }
}

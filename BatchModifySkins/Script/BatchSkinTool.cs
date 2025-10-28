using UnityEngine;

namespace BatchModifyStyles
{
    public class BatchSkinTool : InterfaceTool
    {
        public static BatchSkinTool Instance { get; private set; }

        private BatchSkinVisualizer visualizer;
        private BatchSkinTemplateManager templateManager;
        private BatchSkinInputHandler inputHandler;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Instance = this;

            visualizer = new BatchSkinVisualizer(this);
            templateManager = new BatchSkinTemplateManager(this);
            inputHandler = new BatchSkinInputHandler(this, visualizer, templateManager);

            visualizer.Initialize();
            inputHandler.Initialize();

            this.cursor = null;
            this.cursorOffset = Vector2.zero;

            Log.Info("[BatchModifyStyles] Batch skin tool initialized");
        }

        public void ActivateFromBlueprint()
        {
            Log.Info("[BatchModifyStyles] Activating tool from blueprint button");

            GameObject selected = GetSelectedObject();
            if (selected == null)
            {
                Log.Warn("[BatchModifyStyles] No object selected");
                return;
            }

            templateManager.CheckAndSaveTemplateFromSelection();

            if (!templateManager.HasValidTemplate())
            {
                Log.Warn("[BatchModifyStyles] No valid template found");
                return;
            }

            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.ActivateTool(this);
                Log.Info("[BatchModifyStyles] Tool activated successfully");
            }
        }

        private GameObject GetSelectedObject()
        {
            if (SelectTool.Instance != null && SelectTool.Instance.selected != null)
            {
                return SelectTool.Instance.selected.gameObject;
            }
            return null;
        }

        protected override void OnActivateTool()
        {
            base.OnActivateTool();
            hasFocus = true;

            visualizer.OnActivate();
            templateManager.CheckAndSaveTemplateFromSelection();

            
        }

        protected override void OnDeactivateTool(InterfaceTool new_tool)
        {
            base.OnDeactivateTool(new_tool);
            visualizer.OnDeactivate();
            Log.Info("[BatchModifyStyles] Tool deactivated");
        }

        public override void OnMouseMove(Vector3 cursor_pos)
        {
            base.OnMouseMove(cursor_pos);
            visualizer.OnMouseMove(cursor_pos);
        }

        public override void OnLeftClickDown(Vector3 cursor_pos)
        {
            base.OnLeftClickDown(cursor_pos);
            inputHandler.OnLeftClickDown(cursor_pos);
        }

        public override void OnLeftClickUp(Vector3 cursor_pos)
        {
            base.OnLeftClickUp(cursor_pos);
            inputHandler.OnLeftClickUp(cursor_pos);
        }

        public override void OnKeyDown(KButtonEvent e)
        {
            if (e.IsAction(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
            {
                PlayerController.Instance.ActivateTool(SelectTool.Instance);
                e.Consumed = true;
                return;
            }

            base.OnKeyDown(e);
        }
    }
}

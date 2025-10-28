using UnityEngine;

namespace BatchModifyStyles
{
    public class BatchSkinVisualizer
    {
        private BatchSkinTool parentTool;

        private GameObject cursorIcon;
        private GameObject areaVisualizer;
        private SpriteRenderer areaSpriteRenderer;

        private bool isDragging;
        private Vector3 dragStart;
        private Vector3 dragEnd;

        public bool IsDragging => isDragging;

        public BatchSkinVisualizer(BatchSkinTool parent)
        {
            parentTool = parent;
        }

        public void Initialize()
        {
            CreateCursorIcon();
            CreateAreaVisualizer();
            Log.Info("[BatchModifyStyles] Visualizer initialized");
        }

        public void OnActivate()
        {
            ShowCursorIcon(true);
        }

        public void OnDeactivate()
        {
            ShowCursorIcon(false);
            isDragging = false;
            if (areaVisualizer != null)
                areaVisualizer.SetActive(false);
        }

        public void OnMouseMove(Vector3 cursorPos)
        {
            UpdateCursorIcon(cursorPos);
            if (isDragging)
            {
                dragEnd = SnapToGrid(cursorPos);
                UpdateAreaVisualizer();
            }
        }

        public void StartDrag(Vector3 cursorPos)
        {
            isDragging = true;
            dragStart = SnapToGrid(cursorPos);
            dragEnd = dragStart;

            if (areaVisualizer != null)
            {
                areaVisualizer.SetActive(true);
                UpdateAreaVisualizer();
            }
        }

        public void EndDrag()
        {
            isDragging = false;
            if (areaVisualizer != null)
                areaVisualizer.SetActive(false);
        }

        public (Vector3 start, Vector3 end) GetDragArea()
        {
            return (dragStart, dragEnd);
        }

        private void CreateCursorIcon()
        {
            cursorIcon = new GameObject("BatchSkinCursorIcon");
            var sr = cursorIcon.AddComponent<SpriteRenderer>();
            sr.sprite = Assets.VisualIcon;
            sr.color = BatchSkinConstants.TOOL_COLOR;
            sr.sortingLayerName = "Overlay";
            sr.sortingOrder = 10000;
            cursorIcon.transform.localScale = Vector3.one * BatchSkinConstants.ICON_SCALE;
            cursorIcon.SetActive(false);
        }

        private void ShowCursorIcon(bool show)
        {
            if (cursorIcon != null)
                cursorIcon.SetActive(show && parentTool.hasFocus);
        }

        private void UpdateCursorIcon(Vector3 cursorPos)
        {
            if (cursorIcon != null && cursorIcon.activeSelf)
            {
                Vector3 iconPos = cursorPos;
                iconPos.z = -0.1f;
                cursorIcon.transform.position = SnapToGrid(iconPos);
            }
        }

        private void CreateAreaVisualizer()
        {
            areaVisualizer = new GameObject("BatchSkinAreaVisualizer");
            areaSpriteRenderer = areaVisualizer.AddComponent<SpriteRenderer>();

            // 创建正片叠底效果的材质
            var material = new Material(Shader.Find("Sprites/Default"));
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.EnableKeyword("_ALPHABLEND_ON");
            areaSpriteRenderer.material = material;

            areaSpriteRenderer.sortingLayerName = "Overlay";
            areaSpriteRenderer.sortingOrder = 9999;

            areaVisualizer.transform.SetParent(parentTool.transform);
            areaVisualizer.SetActive(false);
        }

        private void UpdateAreaVisualizer()
        {
            if (areaVisualizer == null || !isDragging) return;

            Vector2 max = Vector3.Max(dragStart, dragEnd);
            Vector2 min = Vector3.Min(dragStart, dragEnd);

            // 当前min和max是格子中心点，需要扩展到格子边界
            Vector2 actualMin = new Vector2(
                min.x - BatchSkinConstants.GRID_SIZE / 2f,
                min.y - BatchSkinConstants.GRID_SIZE / 2f
            );

            Vector2 actualMax = new Vector2(
                max.x + BatchSkinConstants.GRID_SIZE / 2f,
                max.y + BatchSkinConstants.GRID_SIZE / 2f
            );

            Vector2 center = (actualMax + actualMin) * 0.5f;
            Vector2 size = actualMax - actualMin;

            areaVisualizer.transform.position = center;

            // 创建纹理时使用修正后的尺寸
            CreateBorderedTexture(size);
        }

        private void CreateBorderedTexture(Vector2 size)
        {
            int textureWidth = Mathf.Max(32, Mathf.RoundToInt(size.x * 32f)); // 更高分辨率
            int textureHeight = Mathf.Max(32, Mathf.RoundToInt(size.y * 32f));

            Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
            Color[] pixels = new Color[textureWidth * textureHeight];

            // 初始化所有像素为透明
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.clear;
            }

            // 亚像素边框 - 只在最外层绘制
            for (int y = 0; y < textureHeight; y++)
            {
                for (int x = 0; x < textureWidth; x++)
                {
                    // 只在最外边缘绘制边框
                    bool isTopBorder = y == 0;
                    bool isBottomBorder = y == textureHeight - 1;
                    bool isLeftBorder = x == 0;
                    bool isRightBorder = x == textureWidth - 1;

                    bool isBorder = isTopBorder || isBottomBorder || isLeftBorder || isRightBorder;

                    if (isBorder)
                    {
                        // 边框：亚像素级别，不透明
                        pixels[y * textureWidth + x] = BatchSkinConstants.TOOL_COLOR;
                    }
                    else if (x == 1 || x == textureWidth - 2 || y == 1 || y == textureHeight - 2)
                    {
                        // 内边缘：稍微透明的边框，创造更细的效果
                        pixels[y * textureWidth + x] = new Color(
                            BatchSkinConstants.TOOL_COLOR.r,
                            BatchSkinConstants.TOOL_COLOR.g,
                            BatchSkinConstants.TOOL_COLOR.b,
                            0.3f
                        );
                    }
                    else
                    {
                        // 内部：半透明填充
                        pixels[y * textureWidth + x] = new Color(
                            BatchSkinConstants.AREA_COLOR.r,
                            BatchSkinConstants.AREA_COLOR.g,
                            BatchSkinConstants.AREA_COLOR.b,
                            0.15f  // 更透明的内部
                        );
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            texture.filterMode = FilterMode.Point;

            areaSpriteRenderer.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, textureWidth, textureHeight),
                new Vector2(0.5f, 0.5f),
                32f  // 更高的PPU
            );

            areaVisualizer.transform.localScale = new Vector3(
                size.x / (textureWidth / 32f),
                size.y / (textureHeight / 32f),
                1f
            );
        }

        private Vector3 SnapToGrid(Vector3 pos)
        {
            return new Vector3(
                Mathf.Floor(pos.x / BatchSkinConstants.GRID_SIZE) * BatchSkinConstants.GRID_SIZE + BatchSkinConstants.GRID_SIZE / 2f,
                Mathf.Floor(pos.y / BatchSkinConstants.GRID_SIZE) * BatchSkinConstants.GRID_SIZE + BatchSkinConstants.GRID_SIZE / 2f,
                0f
            );
        }
    }
}

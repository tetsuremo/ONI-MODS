using System.IO;
using System.Reflection;
using UnityEngine;

namespace BatchModifyStyles
{
    public static class Assets
    {
        public static string VisualName = "BATCHMODIFYSKINS.TOOL.VISUALIZER";
        public static Sprite VisualIcon;

        public static void LoadAssets()
        {
            var path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "assets"
            );

            // 只加载鼠标视觉图标
            VisualIcon = LoadSprite(Path.Combine(path, "batch_skin_visualizer.png"), VisualName);

#if DEBUG
            if (VisualIcon != null)
                Debug.Log($"[BatchModifyStyles] VisualIcon loaded OK: {VisualIcon.texture.width}x{VisualIcon.texture.height}");
            else
                Debug.LogWarning("[BatchModifyStyles] VisualIcon is null after loading.");
#endif
        }

        public static void RegisterToGlobalAssets()
        {
            try
            {
                if (global::Assets.Sprites == null)
                {
#if DEBUG
                    Debug.LogWarning("[BatchModifyStyles] global::Assets.Sprites is null, skipping registration");
#endif
                    return;
                }

                if (VisualIcon != null)
                {
                    if (!global::Assets.Sprites.ContainsKey(VisualName))
                    {
                        global::Assets.Sprites.Add(VisualName, VisualIcon);
#if DEBUG
                        Debug.Log($"[BatchModifyStyles] Registered {VisualName} in global::Assets.Sprites.");
#endif
                    }
#if DEBUG
                    else
                    {
                        Debug.Log($"[BatchModifyStyles] global::Assets.Sprites already contains {VisualName}.");
                    }
#endif
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[BatchModifyStyles] Exception while registering sprites: {e}");
            }
        }

        private static Sprite LoadSprite(string path, string name)
        {
            if (File.Exists(path))
            {
                try
                {
                    var texture = new Texture2D(2, 2);
                    texture.LoadImage(File.ReadAllBytes(path));
                    texture.filterMode = FilterMode.Point;
                    var sprite = Sprite.Create(texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f),
                        100f);
                    sprite.name = name;
#if DEBUG
                    Debug.Log($"[BatchModifyStyles] Created sprite {name} ({texture.width}x{texture.height}) from {Path.GetFileName(path)}");
#endif
                    return sprite;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[BatchModifyStyles] Failed to create sprite {name} from {path}: {e}");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"[BatchModifyStyles] Missing icon file: {path}");
            }
            return null;
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Extensions.Miscellaneous
{
    public static class Helper
    {
        #region Extension Methods

        public static bool IsEmptyOrNull<T>(this ICollection<T> list) => list.All(item => item is null) || list.Count == 0;

        public static Vector2 SnapTo8Slices(this Vector2 vector2)
        {
            Vector2 dir = Vector2.zero;

            if (Vector2.Dot(vector2.normalized, Vector2.up) > 0.5f)
                dir += Vector2.up;
            if (Vector2.Dot(vector2.normalized, Vector2.down) > 0.5f)
                dir += Vector2.down;
            if (Vector2.Dot(vector2.normalized, Vector2.left) > 0.5f)
                dir += Vector2.left;
            if (Vector2.Dot(vector2.normalized, Vector2.right) > 0.5f)
                dir += Vector2.right;

            return dir.normalized;
        }

        public static bool Intersects(this BoundsInt lhs, BoundsInt rhs)
        {
            Bounds lhsNoInt = new(lhs.position, lhs.size);
            Bounds rhsNoInt = new(rhs.position, rhs.size);

            return lhsNoInt.Intersects(rhsNoInt);
        }

        public static bool ContainsBounds(this BoundsInt lhs, BoundsInt rhs)
        {
            return rhs.position.x > lhs.position.x + 1 &&
                   rhs.position.y > lhs.position.y + 1 &&
                   rhs.position.x + rhs.size.x < lhs.position.x + lhs.size.x - 2 &&
                   rhs.position.y + rhs.size.y < lhs.position.y + lhs.size.y - 2;
        }

        public static int FrameCount(this AnimationClip animClip) => (int)(animClip.length * animClip.frameRate);

        public static GameObject FindChildWithTag(this GameObject gameObject, string tag, bool includeInactive)
        {
            foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(includeInactive))
            {
                if (transform.CompareTag(tag))
                    return transform.gameObject;
            }

            return null;
        }

        [System.Flags]
        public enum Direction
        {
            Horizontal = 1 << 0,
            Vertical = 1 << 1
        }

        public static void SetSizeAuto(this RectTransform rectTransform, Direction direction, Vector2 padding = default, Vector2 extraPadding = default, bool useMaxParent = false)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            RectTransform parentTransform = rectTransform.parent.GetComponent<RectTransform>();

            if (direction.HasFlag(Direction.Horizontal))
            {
                float biggestX = 0;
                float width = 0;

                for (int i = rectTransform.childCount - 1; i >= 0; --i)
                {
                    RectTransform rect = rectTransform.GetChild(i).GetComponent<RectTransform>();
                    if (!rect.gameObject.activeSelf)
                        continue;

                    float newX = Mathf.Abs(rect.anchoredPosition.x);
                    if (newX > biggestX)
                    {
                        biggestX = newX;
                        width = rect.sizeDelta.x;

                        break;
                    }
                }

                float newSize = biggestX - width / 2 + width;
                
                if (useMaxParent)
                    sizeDelta.x = Mathf.Max(newSize + padding.x, parentTransform.rect.width);
                else
                    sizeDelta.x = newSize + padding.x;
            }

            if (direction.HasFlag(Direction.Vertical))
            {
                float biggestY = 0;
                float height = 0;

                for (int i = rectTransform.childCount - 1; i >= 0; --i)
                {
                    RectTransform rect = rectTransform.GetChild(i).GetComponent<RectTransform>();
                    if (!rect.gameObject.activeSelf)
                        continue;

                    float newY = Mathf.Abs(rect.anchoredPosition.y);
                    if (newY > biggestY)
                    {
                        biggestY = newY;
                        height = rect.sizeDelta.y;

                        break;
                    }
                }

                float newSize = biggestY - height / 2 + height;
                
                if (useMaxParent)
                    sizeDelta.y = Mathf.Max(newSize + padding.y, parentTransform.rect.height);
                else
                    sizeDelta.y = newSize + padding.y;
            }

            rectTransform.sizeDelta = sizeDelta + extraPadding;
            return;
        }

        public enum ImageType
        {
            PNG,
            JPG,
            EXR,
            TGA
        }

        public static Texture2D RenderToTexture2D(this Camera camera, RenderTexture output, Shader shader = null, string replacementTag = "")
        {
            camera.targetTexture = output;

            var currentRT = RenderTexture.active;
            RenderTexture.active = camera.targetTexture;

            if (shader)
                camera.RenderWithShader(shader, replacementTag);
            else
                camera.Render();

            Texture2D image = new(camera.targetTexture.width, camera.targetTexture.height);
            image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
            image.Apply();

            RenderTexture.active = currentRT;
            return image;
        }

        public static void RenderToScreenshot(this Camera camera, string filePath, RenderTexture output, ImageType type, Shader shader = null, string replacementTag = "")
        {
            Texture2D texture = RenderToTexture2D(camera, output, shader, replacementTag);
            byte[] renderBytes = type switch
            {
                ImageType.PNG => texture.EncodeToPNG(),
                ImageType.JPG => texture.EncodeToJPG(),
                ImageType.EXR => texture.EncodeToEXR(),
                ImageType.TGA => texture.EncodeToTGA(),
                _ => null
            };

            System.IO.File.WriteAllBytes(filePath, renderBytes);
            Debug.Log("Screenshot taken!");
        }

        public static Sprite SpriteFromScreenshot(string filePath, int width, int height)
        {
            Debug.Log("Getting sprite from a screenshot");

            if (File.Exists(filePath))
            {
                byte[] fileData = File.ReadAllBytes(filePath);

                Texture2D iconTexture = new(width, height, TextureFormat.RGBA32, false, true);
                iconTexture.LoadImage(fileData);

                return Sprite.Create(iconTexture, new(0, 0, iconTexture.width, iconTexture.height), Vector2.one);
            }
            
            return Sprite.Create(Texture2D.whiteTexture, new(0, 0, Texture2D.whiteTexture.width, -Texture2D.whiteTexture.height), Vector2.one);
        }

        #endregion

        public static void DrawSprite(Rect position, Sprite sprite)
        {
            if (!sprite)
                return;

            Texture2D texture = sprite.texture;
            Rect spriteRect = new(sprite.textureRect.position + sprite.textureRectOffset, sprite.textureRect.size);
            Rect texCoords = new(spriteRect.x / texture.width, spriteRect.y / texture.height, spriteRect.width / texture.width, spriteRect.height / texture.height);

            GUI.DrawTextureWithTexCoords(position, texture, texCoords);
        }

        public static float Sign(float f)
        {
            if (f != 0)
                return Mathf.Sign(f);
            else
                return 0;
        }

        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
        {
            return new
            (
                Mathf.Clamp(value.x, min.x, max.x),
                Mathf.Clamp(value.y, min.y, max.y)
            );
        }

        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            return new
            (
                Mathf.Clamp(value.x, min.x, max.x),
                Mathf.Clamp(value.y, min.y, max.y),
                Mathf.Clamp(value.z, min.z, max.z)
            );
        }

        public static void Swap<T, U>(ref T a, ref U b)
        {
            T c = a;
            a = (T)(object)b;
            b = (U)(object)c;
        }

        public static async Task Delay(float seconds, System.Action action)
        {
            await Task.Delay((int)(seconds * 1000));
            action.Invoke();
        }

        public static async Task Delay<T>(float seconds, System.Action<T> action, T t)
        {
            await Task.Delay((int)(seconds * 1000));
            action.Invoke(t);
        }

        public static void NoOp() { }
    }
}

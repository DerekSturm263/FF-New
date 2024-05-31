using System.Collections.Generic;
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

        public static GameObject FindChildWithTag(this GameObject gameObject, string tag)
        {
            foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>())
            {
                if (transform.CompareTag(tag))
                    return transform.gameObject;
            }

            return null;
        }

        #endregion

        public static void DrawSprite(Rect position, Sprite sprite)
        {
            if (!sprite)
                return;

            Texture2D texture = sprite.texture;
            Rect spriteRect = new(sprite.textureRect.position + sprite.textureRectOffset, sprite.textureRect.size);
            Rect texCoords = new(spriteRect.x / texture.width, spriteRect.y / texture.height, spriteRect.width / texture.width, spriteRect.height / texture.height);

            UnityEngine.GUI.DrawTextureWithTexCoords(position, texture, texCoords);
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

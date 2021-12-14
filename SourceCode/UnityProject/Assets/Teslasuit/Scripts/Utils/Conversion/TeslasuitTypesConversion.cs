using UnityEngine;


namespace TeslasuitAPI.Utils
{
    public static class TeslasuitTypesConversion
    {
        public static Vector3f Vector3f(this Vector3 v)
        {
            return new Vector3f(v.x, v.y, v.z);
        }

        public static Vector2f Vector2f(this Vector2 v)
        {
            return new Vector2f(v.x, v.y);
        }

        public static Vector3 Vector3(this Vector3f v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static Vector2 Vector2(this Vector2f v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Quaternion Quaternion(this Quat4f v)
        {
            return new Quaternion(v.x, v.y, v.z, v.w);
        }

        public static Quat4f Quat4f(this Quaternion v)
        {
            return new Quat4f(v.x, v.y, v.z, v.w);
        }
    }
}


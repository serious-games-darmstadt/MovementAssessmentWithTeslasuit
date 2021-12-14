using TeslasuitAPI;
using UnityEngine;

namespace TeslasuitAPI.Utils
{
    public static class TransformExtensions
    {

        public static Quaternion Normalized(this Quaternion quat)
        {
            Quaternion result = quat;
            
            float magn = Mathf.Sqrt(quat.x * quat.x + quat.y * quat.y + quat.z * quat.z + quat.w * quat.w);


            result.x = quat.x / magn;
            result.y = quat.y / magn;
            result.z = quat.z / magn;
            result.w = quat.w / magn;
            return result;
        }

        public static Quaternion Inversed(this Quaternion vector)
        {
            Quaternion quaternion = vector;

            return quaternion.Inverse(true, true, true);
        }

        public static Quaternion HeadingOffset(Quaternion a, Quaternion b)
        {
            Quaternion offset = a * b.Inversed();
            offset.x = 0f;
            offset.z = 0f;

            float mag = offset.w * offset.w + offset.y * offset.y;

            offset.w /= Mathf.Sqrt(mag);
            offset.y /= Mathf.Sqrt(mag);

            return offset;
        }

        private static Quaternion Inverse(this Quaternion vector, bool X, bool Y, bool Z)
        {
            vector.x *= X ? -1f : 1f;
            vector.y *= Y ? -1f : 1f;
            vector.z *= Z ? -1f : 1f;
            return vector;
        }

        public static Ray InverseTransformRay(Transform tr, Ray ray)
        {
            ray.direction = tr.InverseTransformDirection(ray.direction);
            ray.origin = tr.InverseTransformPoint(ray.origin);
            return ray;
        }

        public static void InverseTransformRays(Transform tr, ref Ray[] rays)
        {
            for (int i = 0; i < rays.Length; i++)
            {
                rays[i] = InverseTransformRay(tr, rays[i]);
            }
        }

        public static Ray[] CollisionNormalRays(Collision collision, float sign)
        {
            Ray[] rays = new Ray[collision.contacts.Length];

            for (int i = 0; i < rays.Length; i++)
            {
                rays[i] = new Ray(collision.contacts[i].point, Mathf.Sign(sign) * collision.contacts[i].normal);
            }

            return rays;
        }
    }

}

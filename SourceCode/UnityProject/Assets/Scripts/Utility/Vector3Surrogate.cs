using System.Runtime.Serialization;
using UnityEngine;

namespace Thesis
{
    public class Vector3Surrogate: ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3 vector = (Vector3) obj;
            info.AddValue("x", vector.x);
            info.AddValue("y", vector.y);
            info.AddValue("z", vector.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3 vector = (Vector3) obj;
            vector.x = info.GetSingle("x");
            vector.y = info.GetSingle("y");
            vector.z = info.GetSingle("z");
            return vector;
        }
    }
}
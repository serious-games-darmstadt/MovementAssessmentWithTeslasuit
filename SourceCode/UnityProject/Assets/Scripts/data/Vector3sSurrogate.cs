using System.Runtime.Serialization;
using TeslasuitAPI;

namespace Thesis
{
    public class Vector3sSurrogate: ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3s vector = (Vector3s) obj;
            info.AddValue("x", vector.x);
            info.AddValue("y", vector.y);
            info.AddValue("z", vector.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3s vector = (Vector3s) obj;
            vector.x = info.GetInt16("x");
            vector.y = info.GetInt16("y");
            vector.z = info.GetInt16("z");
            return vector;
        }
    }
}
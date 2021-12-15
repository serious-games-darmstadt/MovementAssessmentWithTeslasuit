using System.Runtime.Serialization;
using TeslasuitAPI;

namespace Thesis
{
    public class Quat4fSurrogate: ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Quat4f quat = (Quat4f) obj;
            info.AddValue("w", quat.w);
            info.AddValue("x", quat.x);
            info.AddValue("y", quat.y);
            info.AddValue("z", quat.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Quat4f quat = (Quat4f) obj;
            quat.w = info.GetSingle("w");
            quat.x = info.GetSingle("x");
            quat.y = info.GetSingle("y");
            quat.z = info.GetSingle("z");
            return quat;
        }
    }
}
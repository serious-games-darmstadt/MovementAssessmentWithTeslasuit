using System.Runtime.Serialization;
using TeslasuitAPI;

namespace Thesis
{
    public class TSMocapDataSurrogate: ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            TSMocapData data = (TSMocapData) obj;
            info.AddValue("bone_index", data.mocap_bone_index);
            info.AddValue("quat9x", data.quat9x);
            info.AddValue("quat6x", data.quat6x);
            info.AddValue("gyroscope", data.gyroscope);
            info.AddValue("magnetometer", data.magnetometer);
            info.AddValue("accelerometer", data.accelerometer);
            info.AddValue("linear_accel", data.linear_accel);
            info.AddValue("temperature", data.temperature);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            TSMocapData data = (TSMocapData) obj;
            data.mocap_bone_index = info.GetUInt64("bone_index");
            data.quat9x = (Quat4f) info.GetValue("quat9x", typeof(Quat4f));
            data.quat6x = (Quat4f) info.GetValue("quat6x", typeof(Quat4f));
            data.gyroscope = (Vector3s) info.GetValue("gyroscope", typeof(Vector3s));
            data.magnetometer = (Vector3s) info.GetValue("magnetometer", typeof(Vector3s));
            data.accelerometer = (Vector3s) info.GetValue("accelerometer", typeof(Vector3s));
            data.linear_accel = (Vector3s) info.GetValue("linear_accel", typeof(Vector3s));
            data.temperature = info.GetSByte("temperature");
            return data;
        }
    }
}
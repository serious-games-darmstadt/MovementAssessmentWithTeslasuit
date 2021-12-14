using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using DefaultNamespace;
using TeslasuitAPI;
using UnityEngine;

namespace Thesis
{
    [Serializable]
    public class SuitData
    {
        public TSMocapData[] data;
        public ExerciseLabel label;
        public Vector3[] jointData;
        public double timestamp;
        private String segment;

        public SuitData(TSMocapData[] data, double timestamp, Vector3[] jointData, String segment)
        {
            this.data = data;
            this.timestamp = timestamp;
            this.jointData = jointData;
            this.segment = segment;
        }

        /**
         * filtered: Whether CSV should be filtered for transmission to Python
         */
        public String ToCSV(string seperator, bool filtered = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(timestamp.ToString(CultureInfo.InvariantCulture)).Append(seperator);
            if (!filtered) sb.Append(label.ToString()).Append(seperator);
            if (!filtered) sb.Append(segment).Append(seperator);

            for (int i = 0; i < data.Length; i++)
            {
                TSMocapData tsMocapData = data[i];

                if (!filtered)
                {
                    sb.Append(tsMocapData.mocap_bone_index.ToString()).Append(seperator);
                }
                
                if (Config.StreamedProperties["quat9x"] && (!filtered || Config.FilteredProperties["quat9x"]))
                    sb.Append(quatToString(tsMocapData.quat9x, seperator));
                if (Config.StreamedProperties["quat6x"] && (!filtered || Config.FilteredProperties["quat6x"]))
                    sb.Append(quatToString(tsMocapData.quat6x, seperator));
                if (Config.StreamedProperties["gyroscope"] && (!filtered || Config.FilteredProperties["gyroscope"]))
                    sb.Append(vector3sToString(tsMocapData.gyroscope, seperator));
                if (Config.StreamedProperties["magnetometer"] && (!filtered || Config.FilteredProperties["magnetometer"]))
                    sb.Append(vector3sToString(tsMocapData.magnetometer, seperator));
                if (Config.StreamedProperties["accelerometer"] && (!filtered || Config.FilteredProperties["accelerometer"]))
                    sb.Append(vector3sToString(tsMocapData.accelerometer, seperator));
                if (Config.StreamedProperties["linearAccel"] && (!filtered || Config.FilteredProperties["linearAccel"]))
                    sb.Append(vector3sToString(tsMocapData.linear_accel, seperator));
                if (Config.StreamedProperties["temperature"] && (!filtered || Config.FilteredProperties["temperature"]))
                    sb.Append(tsMocapData.temperature.ToString()).Append(seperator);
            }

            for (int i = 0; i < jointData.Length; i++)
            {
                Vector3 joint = jointData[i];
                sb.Append(Vector3ToString(joint, seperator, endLine: i == jointData.Length - 1));
            }

            return sb.ToString();
        }

        private string quatToString(Quat4f quat4F, string seperator)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(quat4F.w.ToString(CultureInfo.InvariantCulture)).Append(seperator)
                .Append(quat4F.x.ToString(CultureInfo.InvariantCulture)).Append(seperator)
                .Append(quat4F.y.ToString(CultureInfo.InvariantCulture)).Append(seperator)
                .Append(quat4F.z.ToString(CultureInfo.InvariantCulture)).Append(seperator);

            return sb.ToString();
        }

        private string vector3sToString(Vector3s vector3S, string separator)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(vector3S.x.ToString(CultureInfo.InvariantCulture)).Append(separator)
                .Append(vector3S.y.ToString(CultureInfo.InvariantCulture)).Append(separator)
                .Append(vector3S.z.ToString(CultureInfo.InvariantCulture)).Append(separator);

            return sb.ToString();
        }

        private string Vector3ToString(Vector3 vector, String separator, bool endLine = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(vector.x.ToString(CultureInfo.InvariantCulture)).Append(separator)
                .Append(vector.y.ToString(CultureInfo.InvariantCulture)).Append(separator)
                .Append(vector.z.ToString(CultureInfo.InvariantCulture));

            if (!endLine)
            {
                sb.Append(separator);
            }

            return sb.ToString();
        }

        public String GetCsvHeader(string seperator)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("index").Append(seperator).Append("label").Append(seperator).Append("segment").Append(seperator);

            foreach (var tsMocapData in data)
            {
                String nodeName = Enum.GetName(typeof(MocapBone), tsMocapData.mocap_bone_index);
                sb.Append(nodeName + "_boneIndex").Append(seperator);

                foreach (var property in Config.propertyNames)
                {
                    // Property not streamed -> Continue
                    if (!Config.StreamedProperties[property])
                        continue;

                    // Temperature only has a single value
                    if (property.Equals("temperature"))
                    {
                        sb.Append(nodeName + "_" + property).Append(seperator);
                        continue;
                    }

                    // For Quats, add w component
                    if (property.Equals("quat9x") || property.Equals("quat6x"))
                    {
                        sb.Append(nodeName + "_" + property + "_w").Append(seperator);
                    }

                    // Everything else has x, y, z.
                    sb.Append(nodeName + "_" + property + "_x").Append(seperator);
                    sb.Append(nodeName + "_" + property + "_y").Append(seperator);
                    sb.Append(nodeName + "_" + property + "_z").Append(seperator);
                }
            }

            foreach (var joint in MocapJoints.GetInstance().JointNames)
            {
                sb.Append(joint + "_x").Append(seperator);
                sb.Append(joint + "_y").Append(seperator);
                sb.Append(joint + "_z").Append(seperator);
            }

            sb.Append("\n");
            return sb.ToString();
        }
    }
}
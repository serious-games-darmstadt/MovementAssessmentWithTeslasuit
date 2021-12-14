using UnityEngine;
using TeslasuitAPI;
using System.IO;

namespace TeslasuitAPI
{
    public class HapticAsset : ScriptableObject
    {
        [SerializeField]
        private byte[] _bytes;
        public byte[] Bytes
        {
            get { return _bytes; }
            protected set { _bytes = value; }
        }

        protected virtual byte[] ReadBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public virtual HapticAsset Init(string path)
        {
            Bytes = ReadBytes(path);
            return this;
        }
    }
    
}

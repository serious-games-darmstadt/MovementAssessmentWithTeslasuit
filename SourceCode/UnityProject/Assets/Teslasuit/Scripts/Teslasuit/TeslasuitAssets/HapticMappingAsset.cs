
using UnityEngine;

namespace TeslasuitAPI
{
    public class HapticMappingAsset : HapticAsset, IMappingAsset 
    {

        public int Width { get { return _mappingWidth; } set { _mappingWidth = value; } }
        public int Height { get { return _mappingHeight; } set { _mappingHeight = value; } }

        [SerializeField]
        protected int _mappingWidth = 1024;
        [SerializeField]
        protected int _mappingHeight = 1024;

        public byte[] GetBytes()
        {

            return Bytes;
        }
    } 
}

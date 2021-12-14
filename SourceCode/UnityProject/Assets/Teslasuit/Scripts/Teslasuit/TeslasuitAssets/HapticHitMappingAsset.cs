using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeslasuitAPI
{
    public class HapticHitMappingAsset : HapticMappingAsset, IHitMappingAsset
    {

        public override HapticAsset Init(string path)
        {
            var ret = base.Init(path);
            return ret;
        }
    }
}

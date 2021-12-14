using System;
using UnityEngine;
#if UNITY_2018_2_OR_NEWER
using UnityEngine.Rendering;
#endif

namespace TeslasuitAPI
{
    public class TriangleRaycastAsyncCompute : TriangleRaycastCompute
    {

        protected override void RequestBufferData(int count, CGRayCast[] results, ComputeBuffer computeBuffer, Action<CGRayCast[], int, object> callback, object opaque)
        {
#if UNITY_2018_2_OR_NEWER
            if (SystemInfo.supportsAsyncGPUReadback)
            {
                var asyncReq = AsyncGPUReadback.Request(computeBuffer, (request) =>
                {
                    request.GetData<CGRayCast>().CopyTo(results);
                    callback?.Invoke(results, count, opaque);
                });
                asyncReq.Update();
            }
            else
#endif
            {
                computeBuffer.GetData(results);
                callback?.Invoke(results, count, opaque);
            }
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace TeslasuitAPI
{

    [CustomEditor(typeof(HapticMaterialAsset))]
    public class HapticMaterialAssetEditor : Editor
    {
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return createIcon(Color.cyan, 128);
            //return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        Texture2D createIcon(Color color, int size)
        {
            Vector2 center = new Vector2(size / 2, size / 2);
            float radius = size / 2f;
            float circleEnd = 0.65f * radius;
            float circleBegin = 0.4f * radius;

            Texture2D icon = new Texture2D(size, size);
            foreach (var x in Enumerable.Range(0, size))
                foreach (var y in Enumerable.Range(0, size))
                {
                    Vector2 curr = new Vector2(x, y);
                    float dist = Vector2.Distance(curr, center);
                    float mul = (dist > circleBegin ? 1f : 0f) * (dist < circleEnd ? 1f : 0f);
                    Color current = color * mul * (1f - dist / radius);
                    icon.SetPixel(x, y, current);
                }
            return icon;
        }
    }
}
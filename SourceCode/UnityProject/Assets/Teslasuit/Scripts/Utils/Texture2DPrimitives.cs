using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeslasuitAPI.Utils
{
    public static class Texture2DPrimitives
    {

        public static Texture2D RoundIcon(Color color, int size, int offsetX = 0, int offsetY = 0)
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

                    int oX = x + offsetX;
                    int oY = x + offsetY;
                    if (oX < size && oY < size)
                        icon.SetPixel(x, y, current);
                }
            return icon;
        }
    } 
}

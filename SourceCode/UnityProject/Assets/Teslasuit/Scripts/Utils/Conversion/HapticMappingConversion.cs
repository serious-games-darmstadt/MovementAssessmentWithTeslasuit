using UnityEngine;
using System;
using TeslasuitAPI.Utils;

namespace TeslasuitAPI
{
    public class HapticMappingConversion
    {
        private static Vector2f UVToMappingCoordinates(Vector2 uv, int mappingWidth, int mappingHeight)
        {
            return new Vector2f(mappingWidth * (uv.x - 0.5f), mappingHeight * (1f - uv.y - 0.5f));
        }

        public static UnityPolygon[] ToUVPolygons(Polygon[] polygons, int width, int height)
        {
            if (polygons == null) return null;
            UnityPolygon[] polygon2D = new UnityPolygon[polygons.Length];
            for (int i = 0; i < polygons.Length; i++)
            {
                polygon2D[i] = new UnityPolygon(polygons[i], width, height);
            }
            return polygon2D;
        }

        private UnityPolygon[] ToPolygon2D(Polygon[] polygons)
        {
            if (polygons == null) return null;
            UnityPolygon[] polygon2D = new UnityPolygon[polygons.Length];
            for (int i = 0; i < polygons.Length; i++)
            {
                polygon2D[i] = new UnityPolygon(polygons[i]);
            }
            return polygon2D;
        }

    } 
}

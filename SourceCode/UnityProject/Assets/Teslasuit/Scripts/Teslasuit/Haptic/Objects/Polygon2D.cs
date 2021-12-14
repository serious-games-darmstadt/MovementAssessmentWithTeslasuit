using System;
using TeslasuitAPI;
using UnityEngine;

namespace TeslasuitAPI
{
    public class UnityPolygon
    {
        public Vector2[] vertices;


        public UnityPolygon()
        {

        }

        public UnityPolygon(Vector2[] vertices)
        {
            this.vertices = vertices;
        }

        public UnityPolygon(Vector2f[] vertices)
        {
            this.vertices = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                this.vertices[i] = new Vector2(vertices[i].x, vertices[i].y);
            }
        }


        public UnityPolygon(Polygon vertices, int fromTextureWidth, int fromTextureHeight)
        {
            this.vertices = new Vector2[vertices.count];
            for (int i = 0; i < this.vertices.Length; i++)
            {
                this.vertices[i] = new Vector2(vertices.points[i].x, vertices.points[i].y);
            }
            this.ConvertToUVCoordinates(fromTextureWidth, fromTextureHeight);
        }

        public UnityPolygon(Polygon vertices)
        {
            this.vertices = new Vector2[vertices.count];
            for (int i = 0; i < this.vertices.Length; i++)
            {
                this.vertices[i] = new Vector2(vertices.points[i].x, vertices.points[i].y);
            }
        }

        public UnityPolygon ToTextureCoordinates(float width, float height)
        {
            UnityPolygon result = new UnityPolygon(this.vertices);
            result.ConvertToTextureCoordinates(width, height);
            return result;
        }

        public UnityPolygon ToUVCoordinates(float width, float height)
        {
            UnityPolygon result = new UnityPolygon(this.vertices);
            result.ConvertToUVCoordinates(width, height);
            return result;
        }

        public void ConvertToTextureCoordinates(float width, float height)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector2(width * (vertices[i].x - 0.5f), height * (0.5f - vertices[i].y));
            }
        }

        public void ConvertToUVCoordinates(float width, float height)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector2(vertices[i].x / width + 0.5f, 0.5f - vertices[i].y / height);
            }
        }

    }

}
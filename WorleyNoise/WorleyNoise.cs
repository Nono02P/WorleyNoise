using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace WorleyNoise
{
    public class WorleyNoise
    {
        private Random _rnd;
        private Vector3[] _points;

        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public int NbOfPoints { get; set; }

        public WorleyNoise(Random rnd = null)
        {
            if (rnd == null)
                _rnd = new Random();
            else
                _rnd = rnd;
        }

        public void GenerateMap(int nbOfPoints, int width, int height, int depth = 1)
        {
            Width = width;
            Height = height;
            NbOfPoints = nbOfPoints;

            if (depth < 1)
                Depth = 1;
            else
                Depth = depth;

            _points = new Vector3[NbOfPoints];
            for (int i = 0; i < NbOfPoints; i++)
            {
                _points[i] = new Vector3(_rnd.Next(Width), _rnd.Next(Height), _rnd.Next(Depth));
            }
        }

        private float Map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        public float GetValue(int x, int y, int z = 0, int pointIndex = 0)
        {
            if (_points == null)
                throw new Exception("Call the Generate map method first.");

            int max = Math.Max(Math.Max(Width, Height), Depth);

            int index = x + (y * Width) + (z * Width * Height);
            float[] distances = new float[_points.Length];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = Map(Vector3.Distance(new Vector3(x, y, z), _points[i]), 0, max / 2, 0, 1);
            }
            IOrderedEnumerable<float> ordered = distances.OrderBy(d => d);

            return ordered.ElementAt(pointIndex % NbOfPoints);
        }
    }
}
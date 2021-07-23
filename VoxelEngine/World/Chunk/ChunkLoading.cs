﻿using System;
using VoxelEngine.Glm;

namespace VoxelEngine
{
    /// <summary>
    /// Объект чанка, для загрузки прорисовки
    /// </summary>
    public class ChunkLoading : IComparable
    {
        public int X { get; set; } = 0;
        public int Z { get; set; } = 0;

        /// <summary>
        /// Дистанция
        /// </summary>
        public float Distance { get; set; } = 0f;

        /// <summary>
        /// Является ли альфой
        /// </summary>
        public bool IsAlpha { get; set; } = false;

        /// <summary>
        /// Соседний чанк
        /// </summary>
        public vec2i[] Beside { get; set; } = new vec2i[0];

        public ChunkLoading() { }
        public ChunkLoading(int x, int z)
        {
            X = x;
            Z = z;
        }
        public ChunkLoading(int x, int z, float distance)
        {
            X = x;
            Z = z;
            Distance = distance;
        }

        public ChunkLoading(int x, int z, float distance, bool isAlpha)
        {
            X = x;
            Z = z;
            Distance = distance;
            IsAlpha = isAlpha;
        }

        /// <summary>
        /// Метод для сортировки
        /// </summary>
        public int CompareTo(object o)
        {
            if (o is ChunkLoading v)
                return Distance.CompareTo(v.Distance);
            else
                throw new Exception("Невозможно сравнить два объекта");
        }

        public override string ToString()
        {
            return string.Format("({0};{1}) {2:0.00}", X, Z, Distance);
        }
    }
}

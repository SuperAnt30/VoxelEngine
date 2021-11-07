using System;
using VoxelEngine.Glm;

namespace VoxelEngine.Binary
{
    [Serializable]
    public class WorldBin
    {
        /// <summary>
        /// Имя мира
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// Сид мира
        /// </summary>
        public int Seed { get; set; } = 2;
        /// <summary>
        /// Счётчик времени игры
        /// </summary>
        public long TickCount { get; set; } = 0;
        /// <summary>
        /// Позиция камеры
        /// </summary>
        public vec3 Position { get; set; } 
        /// <summary>
        /// Поворот вокруг своей оси
        /// </summary>
        public float RotationYaw { get; set; }
        /// <summary>
        /// Поворот вверх вниз
        /// </summary>
        public float RotationPitch { get; set; }
    }
}

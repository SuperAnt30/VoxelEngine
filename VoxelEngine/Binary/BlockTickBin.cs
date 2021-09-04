using System;
using VoxelEngine.Glm;

namespace VoxelEngine.Binary
{
    [Serializable]
    public class BlockTickBin
    {
        /// <summary>
        /// Позиция блока
        /// </summary>
        public vec3 Position { get; set; }
        /// <summary>
        /// Какой блок запустил проверку
        /// </summary>
        public ushort EBlock { get; set; }
        /// <summary>
        /// Количество тиков ещё
        /// </summary>
        public int CountTick { get; set; }
    }
}

using System;

namespace VoxelEngine.Binary
{
    [Serializable]
    public class RegionBin
    {
        /// <summary>
        /// Массив заархивированных чанков
        /// [x,z][байт архив]
        /// </summary>
        public byte[,][] Chunks { get; set; }
    }
}
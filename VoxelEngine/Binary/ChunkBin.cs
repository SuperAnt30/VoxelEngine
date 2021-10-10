using System;

namespace VoxelEngine.Binary
{
    [Serializable]
    public class ChunkBin
    {
        /// <summary>
        /// Информация блоков
        /// 12 bit индекс блока, 4 бита параметр блока
        /// </summary>
        public ushort[,,] Voxel { get; set; } = new ushort[16, 256, 16];
        /// <summary>
        /// Освещение блоков
        /// 4 бита неба, 4 бита блоков
        /// </summary>
        public byte[,,] Light { get; set; } = new byte[16, 256, 16];
        /// <summary>
        /// Биомы в чанке, столбцы
        /// </summary>
        public byte[,] Biome { get; set; } = new byte[16, 16];
        /// <summary>
        /// Статус генерации чанка
        /// </summary>
        public byte GeterationStatus { get; set; }
        /// <summary>
        /// Массив задач тиков на блоки
        /// </summary>
        public BlockTickBin[] BlockTickBins { get; set; }
        /// <summary>
        /// Массив групповых блоков
        /// </summary>
        public GroupBin[] GroupBins { get; set; }
    }
}

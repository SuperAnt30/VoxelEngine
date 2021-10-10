using System;

namespace VoxelEngine.Binary
{
    [Serializable]
    public class GroupBin
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        /// <summary>
        /// Параметры группы 16 bit
        /// </summary>
        public ushort Properties { get; set; }
        /// <summary>
        /// Тип группы
        /// </summary>
        public byte Type { get; set; }
    }
}

using System;

namespace VoxelEngine
{
    /// <summary>
    /// Объект вокселя
    /// </summary>
    public class VoxelData : IComparable
    {
        public Block Block { get; set; }

        public float Distance { get; set; } = 0f;
        /// <summary>
        /// Буфер сетки блока
        /// </summary>
        public float[] Buffer { get; set; }

        /// <summary>
        /// Метод для сортировки
        /// </summary>
        public int CompareTo(object o)
        {
            if (o is VoxelData v)
                return Distance.CompareTo(v.Distance);
            else
                throw new Exception("Невозможно сравнить два объекта");
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}) {2:0.00}", Block.EBlock, Block.Position, Distance);
        }
    }
}

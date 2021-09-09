namespace VoxelEngine.Util
{
    public class BufferHeir
    {
        /// <summary>
        /// Буфер сетки
        /// </summary>
        public float[] Buffer { get; protected set; } = new float[0];

        /// <summary>
        /// Пустая ли сетка
        /// </summary>
        public bool IsBufferEmpty()
        {
            return Buffer.Length == 0;
        }

        /// <summary>
        /// Очистить сетку
        /// </summary>
        public void ClearBuffer()
        {
            Buffer = new float[0];
        }

        /// <summary>
        /// Количество
        /// </summary>
        public int Count()
        {
            return Buffer.Length;
        }
    }
}

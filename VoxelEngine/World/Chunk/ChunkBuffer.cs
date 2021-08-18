using System.Collections.Generic;

namespace VoxelEngine.World.Chunk
{
    /// <summary>
    /// Объект буфера сеток псевдочанка
    /// </summary>
    public class ChunkBuffer
    {
        /// <summary>
        /// Массив буфера сетки
        /// </summary>
        public float[] Buffer { get; protected set; } = new float[0];
        /// <summary>
        /// Массив альфа буфера сетки 
        /// * Не используется, формируется каждый раз полностью на весь чанк
        /// </summary>
        // public float[] BufferAlpha { get; protected set; } = new float[0];
        /// <summary>
        /// Массив альфа блоков Voxels
        /// </summary>
        public List<VoxelData> Alphas { get; protected set; } = new List<VoxelData>();
        /// <summary>
        /// Пометка изменения
        /// </summary>
        public bool IsModifiedRender { get; protected set; } = true;

        /// <summary>
        /// Пометка псевдо чанка для рендера
        /// </summary>
        public void SetModifiedRender()
        {
            IsModifiedRender = true;
        }

        /// <summary>
        /// Рендер сделан
        /// </summary>
        public void RenderDone(float[] buffer)
        {
            Buffer = buffer;
            IsModifiedRender = false;
        }
    }
}

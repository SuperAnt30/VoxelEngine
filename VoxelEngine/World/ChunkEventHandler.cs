using System.Collections.Generic;

namespace VoxelEngine
{
    public delegate void ChunkEventHandler(object sender, ChunkEventArgs e);

    /// <summary>
    /// Объект для события лога
    /// </summary>
    public class ChunkEventArgs
    {
        /// <summary>
        /// Объект для события чанка
        /// </summary>
        public ChunkEventArgs(float[] b)
        {
            buffer = b;
        }

        public float[] buffer = new float[0];
        /// <summary>
        /// Получить чанк
        /// </summary>
        //public ChunkRender Chunk
        //{
        //    get;
        //    protected set;
        //}
    }
}

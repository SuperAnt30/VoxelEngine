namespace VoxelEngine
{
    public delegate void ChunkEventHandler(object sender, ChunkEventArgs e);

    /// <summary>
    /// Объект для события рендера
    /// </summary>
    public class ChunkEventArgs
    {
        /// <summary>
        /// Объект для события рендера
        /// </summary>
        public ChunkEventArgs(ChunkRender chunk, bool isAlpha)
        {
            Chunk = chunk;
            IsAlpha = isAlpha;
        }

        /// <summary>
        /// Объект прорисовки чанка
        /// </summary>
        public ChunkRender Chunk { get; protected set; }
        /// <summary>
        /// Является ли альфой
        /// </summary>
        public bool IsAlpha { get; protected set; } = false;
    }
}

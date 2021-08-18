using VoxelEngine.Glm;

namespace VoxelEngine
{
    public delegate void BufferEventHandler(object sender, BufferEventArgs e);

    /// <summary>
    /// Объект для события рендера
    /// </summary>
    public class BufferEventArgs
    {
        public BufferEventArgs(int x, int z, float[] bufferAlphe)
        {
            ChunkPos = new vec2i(x, z);
            BufferAlpha = bufferAlphe;
            IsAlpha = true;
        }

        public BufferEventArgs(int x, int z, float[] buffer, float[] bufferAlphe)
        {
            ChunkPos = new vec2i(x, z);
            BufferAlpha = bufferAlphe;
            Buffer = buffer;
            IsAlpha = false;
        }

        /// <summary>
        /// Массив буфера сетки
        /// </summary>
        public float[] Buffer { get; protected set; } = new float[0];
        /// <summary>
        /// Массив альфа буфера сетки 
        /// </summary>
        public float[] BufferAlpha { get; protected set; } = new float[0];

        /// <summary>
        /// Объект прорисовки чанка
        /// </summary>
        public vec2i ChunkPos { get; protected set; }
        /// <summary>
        /// Является ли альфой
        /// </summary>
        public bool IsAlpha { get; protected set; } = false;
    }
}

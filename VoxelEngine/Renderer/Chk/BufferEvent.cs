using VoxelEngine.Entity;
using VoxelEngine.Glm;

namespace VoxelEngine.Renderer.Chk
{
    public delegate void BufferEventHandler(object sender, BufferEventArgs e);

    /// <summary>
    /// Объект для события рендера
    /// </summary>
    public class BufferEventArgs
    {
        /// <summary>
        /// Чанк альфа
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="bufferAlphe"></param>
        public BufferEventArgs(int x, int z, float[] bufferAlphe)
        {
            ChunkPos = new vec2i(x, z);
            BufferAlpha = bufferAlphe;
            Answer = EnumAnswer.ChunkAlpha;
        }

        /// <summary>
        /// Чанк все
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferAlphe"></param>
        public BufferEventArgs(int x, int z, float[] buffer, float[] bufferAlphe)
        {
            ChunkPos = new vec2i(x, z);
            BufferAlpha = bufferAlphe;
            Buffer = buffer;
            Answer = EnumAnswer.ChunkAll;
        }

        /// <summary>
        /// Сущьность
        /// </summary>
        public BufferEventArgs(int index, EnumEntity key, float[] buffer)
        {
            Index = index;
            KeyEntity = key;
            Buffer = buffer;
            Answer = EnumAnswer.Entity;
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
        /// Порядковый номер
        /// </summary>
        public int Index { get; protected set; }
        /// <summary>
        /// Тип сущьности
        /// </summary>
        public EnumEntity KeyEntity { get; protected set; }
        /// <summary>
        /// Объект прорисовки чанка
        /// </summary>
        public vec2i ChunkPos { get; protected set; }

        /// <summary>
        /// Ответ чего
        /// </summary>
        public EnumAnswer Answer { get; protected set; }

        public enum EnumAnswer
        {
            /// <summary>
            /// Чанк, все блоки, прозрачные и нет
            /// </summary>
            ChunkAll,
            /// <summary>
            /// Чанк, только прозрачные блоки
            /// </summary>
            ChunkAlpha,
            /// <summary>
            /// Сущность
            /// </summary>
            Entity
        }
    }
}

namespace VoxelEngine.World.Chunk
{
    public class ChunkHeir
    {
        /// <summary>
        /// Объект кэш чанка
        /// </summary>
        public ChunkD Chunk { get; protected set; }

        protected ChunkHeir() { }

        public ChunkHeir(ChunkD chunk)
        {
            Chunk = chunk;
        }
    }
}

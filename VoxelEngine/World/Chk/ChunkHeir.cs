namespace VoxelEngine.World.Chk
{
    public class ChunkHeir
    {
        /// <summary>
        /// Объект кэш чанка
        /// </summary>
        public ChunkBase Chunk { get; protected set; }

        protected ChunkHeir() { }

        public ChunkHeir(ChunkBase chunk)
        {
            Chunk = chunk;
        }
    }
}

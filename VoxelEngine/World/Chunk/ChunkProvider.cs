using System.Collections;
using VoxelEngine.Util;

namespace VoxelEngine.World.Chunk
{
    /// <summary>
    /// Объект который хранит и отвечает за кэш чанков
    /// </summary>
    public class ChunkProvider
    {
        /// <summary>
        /// Чанк по умолчанию, если нет ни одного в списке
        /// </summary>
        public ChunkD ChunkDefault { get; protected set; }

        /// <summary>
        /// Список чанков
        /// </summary>
        protected ChunkMap chunkMapping = new ChunkMap();
        /// <summary>
        /// Сылка на объект мира
        /// </summary>
        protected WorldD world;

        public ChunkProvider(WorldD worldIn)
        {
            ChunkDefault = new ChunkD(worldIn, 0, 0);
            world = worldIn;
        }

        /// <summary>
        /// Очистка
        /// </summary>
        public void Clear()
        {
            chunkMapping.Clear();
        }
             

        /// <summary>
        /// Выгрузить чанк
        /// </summary>
        public void UnloadChunk(int x, int z)
        {
            ChunkD chunk = ProvideChunk(x, z);
            if (chunk.IsChunkLoaded)
            {
                chunk.OnChunkUnload();
            }
            chunkMapping.Remove(x, z);
        }

        /// <summary>
        /// Загрузить чанк
        /// </summary>
        public void LoadChunk(int x, int z)
        {
            ChunkD chunk = new ChunkD(world, x, z);
            chunkMapping.Set(chunk);
            chunk.OnChunkLoad();
            Debag.GetInstance().CacheChunk = Count();
            //return chunk;
        }

        /// <summary>
        /// Проверить наличие чанка в массиве
        /// </summary>
        public bool IsChunk(int x, int z)
        {
            if (chunkMapping.Contains(x, z))
            {
                ChunkD chunkD = ProvideChunk(x, z);
                return chunkD.IsChunkLoaded;
            }
            return false;
        }

        /// <summary>
        /// Получить чанк по координатам чанка
        /// </summary>
        public ChunkD ProvideChunk(int x, int z)
        {
            ChunkD chunk = chunkMapping.Get(x, z);
            return chunk ?? ChunkDefault;
        }

        /// <summary>
        /// Получить чанк по координате блока
        /// </summary>
        public ChunkD ProvideChunk(BlockPos pos)
        {
            return ProvideChunk(pos.X >> 4, pos.Z >> 4);
        }

        /// <summary>
        /// Количество чанков в кэше
        /// </summary>
        public int Count()
        {
            return chunkMapping.Count;
        }

        /// <summary>
        /// Вернуть коллекцию
        /// </summary>
        public virtual ICollection Values { get { return chunkMapping.Values; } }
    }
}

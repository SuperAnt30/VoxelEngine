using System.Collections;
using VoxelEngine.Util;

namespace VoxelEngine.World.Chk
{
    /// <summary>
    /// Объект который хранит и отвечает за кэш чанков
    /// </summary>
    public class ChunkProvider
    {
        /// <summary>
        /// Чанк по умолчанию, если нет ни одного в списке
        /// </summary>
        public ChunkBase ChunkDefault { get; protected set; }

        /// <summary>
        /// Список чанков
        /// </summary>
        protected ChunkMap chunkMapping = new ChunkMap();
        /// <summary>
        /// Сылка на объект мира
        /// </summary>
        protected WorldBase world;

        public ChunkProvider(WorldBase worldIn)
        {
            ChunkDefault = new ChunkBase(worldIn, 0, 0);
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
            ChunkBase chunk = ProvideChunk(x, z);
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
            ChunkBase chunk = new ChunkBase(world, x, z);
            chunkMapping.Set(chunk);
            chunk.OnChunkLoad();
            Debug.GetInstance().CacheChunk = Count();
            //return chunk;
        }

        /// <summary>
        /// Проверить наличие чанка в массиве
        /// </summary>
        public bool IsChunk(int x, int z)
        {
            if (chunkMapping.Contains(x, z))
            {
                ChunkBase chunkD = ProvideChunk(x, z);
                return chunkD.IsChunkLoaded;
            }
            return false;
        }

        /// <summary>
        /// Получить чанк по координатам чанка
        /// </summary>
        public ChunkBase ProvideChunk(int x, int z)
        {
            ChunkBase chunk = chunkMapping.Get(x, z);
            return chunk ?? ChunkDefault;
        }

        /// <summary>
        /// Получить чанк по координате блока
        /// </summary>
        public ChunkBase ProvideChunk(BlockPos pos)
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

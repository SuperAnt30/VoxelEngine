using VoxelEngine.Util;

namespace VoxelEngine.World.Chunk
{
    public class ChunkMap : Map
    {
        /// <summary>
        /// Добавить или изменить чанк
        /// </summary>
        public void Set(ChunkD chunk)
        {
            base.Set(Key(chunk.X, chunk.Z), chunk);
        }

        /// <summary>
        /// Получить первое значение по дистанции
        /// </summary>
        public new ChunkD Get(string key)
        {
            return base.Get(key) as ChunkD;
        }

        /// <summary>
        /// Получить первое значение по дистанции
        /// </summary>
        public ChunkD Get(int x, int z)
        {
            return base.Get(Key(x, z)) as ChunkD;
        }

        /// <summary>
        /// Проверить по наличию ключа
        /// </summary>
        public bool Contains(ChunkD chunk)
        {
            return _ht.ContainsKey(Key(chunk.X, chunk.Z));
        }
    }
}

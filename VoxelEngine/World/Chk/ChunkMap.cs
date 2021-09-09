using VoxelEngine.Util;

namespace VoxelEngine.World.Chk
{
    public class ChunkMap : Map
    {
        /// <summary>
        /// Добавить или изменить чанк
        /// </summary>
        public void Set(ChunkBase chunk)
        {
            base.Set(Key(chunk.X, chunk.Z), chunk);
        }

        /// <summary>
        /// Получить первое значение по дистанции
        /// </summary>
        public new ChunkBase Get(string key)
        {
            return base.Get(key) as ChunkBase;
        }

        /// <summary>
        /// Получить первое значение по дистанции
        /// </summary>
        public ChunkBase Get(int x, int z)
        {
            return base.Get(Key(x, z)) as ChunkBase;
        }

        /// <summary>
        /// Проверить по наличию ключа
        /// </summary>
        public bool Contains(ChunkBase chunk)
        {
            return _ht.ContainsKey(Key(chunk.X, chunk.Z));
        }
    }
}

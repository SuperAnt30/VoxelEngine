using VoxelEngine.Util;

namespace VoxelEngine.World
{
    public class RegionMap : Map
    {
        /// <summary>
        /// Добавить или изменить регион
        /// </summary>
        public void Set(RegionFile region)
        {
            base.Set(Key(region.X, region.Z), region);
        }

        /// <summary>
        /// Получить значение
        /// </summary>
        public new RegionFile Get(string key)
        {
            return base.Get(key) as RegionFile;
        }
        /// <summary>
        /// Получить значение
        /// </summary>
        public RegionFile Get(int x, int z)
        {
            return base.Get(Key(x, z)) as RegionFile;
        }

        /// <summary>
        /// Получить кэш памяти регионов в байтах 
        /// </summary>
        public int Mem()
        {
            int mem = 0;
            foreach (RegionFile region in _ht.Values)
            {
                mem += region.Mem();
            }
            return mem;
        }
    }
}

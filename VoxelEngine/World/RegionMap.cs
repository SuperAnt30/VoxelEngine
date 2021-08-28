using VoxelEngine.Util;

namespace VoxelEngine.World
{
    public class RegionMap : Map
    {
        /// <summary>
        /// Добавить или изменить регион
        /// </summary>
        public void Set(RegionBinary region)
        {
            base.Set(Key(region.X, region.Z), region);
        }

        /// <summary>
        /// Получить значение
        /// </summary>
        public new RegionBinary Get(string key)
        {
            return base.Get(key) as RegionBinary;
        }
        /// <summary>
        /// Получить значение
        /// </summary>
        public RegionBinary Get(int x, int z)
        {
            return base.Get(Key(x, z)) as RegionBinary;
        }

        /// <summary>
        /// Получить кэш памяти регионов в байтах 
        /// </summary>
        public int Mem()
        {
            int mem = 0;
            foreach (RegionBinary region in _ht.Values)
            {
                mem += region.Mem();
            }
            return mem;
        }
    }
}

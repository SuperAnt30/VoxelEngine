using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Подчанк, 1/16 часть чанка
    /// </summary>
    public class ChunkStorage2
    {
        protected Voxel[,,] _voxels = new Voxel[16, 16, 16];

        /// <summary>
        /// Получить значение вокселя по координатам данного подчанка
        /// </summary>
        public Voxel GetVoxel(int x, int y, int z) => _voxels[y, x, z];

        /// <summary>
        /// Задать воксель
        /// </summary>
        public void SetVoxelId(int x, int y, int z, byte id)
        {
            _voxels[y, x, z].SetIdByte(id);
            //_voxels[y, x, z].SetBlockLightOpacity(Blocks.GetBlockLightOpacity(id));
            //if ((id == 9 || id == 10))
            //{
            //    _voxels[y, x, z].SetBlockLightOpacity((byte)15);
            //}
        }

        /// <summary>
        /// Задать воксель
        /// </summary>
        public void SetVoxel(int x, int y, int z, Voxel voxel) => _voxels[y, x, z] = voxel;

        /// <summary>
        /// Задать яркость блока или неба
        /// </summary>
        public void SetLightFor(int x, int y, int z, EnumSkyBlock type, byte light)
        {
            _voxels[y, x, z].SetLightFor(type, light);
        }

        /// <summary>
        /// Задать яркость блока или неба
        /// </summary>
        public void SetLightsFor(int x, int y, int z, byte light)
        {
            _voxels[y, x, z].SetLightsFor(light);
        }

        /// <summary>
        /// Получить яркость блока или неба
        /// </summary>
        public byte GetLightFor(int x, int y, int z, EnumSkyBlock type)
        {
            return _voxels[y, x, z].GetLightFor(type);
        }
    }
}

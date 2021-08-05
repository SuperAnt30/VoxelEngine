using VoxelEngine.Util;

namespace VoxelEngine
{
    public class ChunkStorage
    {
        /// <summary>
        /// Массив вокселей
        /// </summary>
        protected Voxel[,,] _voxels = new Voxel[16, 16, 16];

        /// <summary>
        /// Массив буфера сетки
        /// </summary>
        protected float[] buffer = new float[0];

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
        }

        /// <summary>
        /// Задать дополнительный параметр блока в 4 бита
        /// </summary>
        public void SetParam4bit(int x, int y, int z, byte param)
        {
            _voxels[y, x, z].SetParam4bit(param);
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
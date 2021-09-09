using VoxelEngine.Vxl;

namespace VoxelEngine.World.Chk
{
    public class ChunkStorage
    {
        /// <summary>
        /// Массив вокселей
        /// </summary>
        protected Voxel[,,] _voxels = new Voxel[16, 16, 16];
        /// <summary>
        /// Объект буфера сеток
        /// </summary>
        public ChunkBuffer Buffer { get; protected set; } = new ChunkBuffer();

        /// <summary>
        /// Пометка псевдо чанка для рендера
        /// </summary>
        public void SetModifiedRender()
        {
            Buffer.SetModifiedRender();
        }

        /// <summary>
        /// Получить значение вокселя по координатам данного подчанка
        /// </summary>
        public Voxel GetVoxel(int x, int y, int z) => _voxels[y, x, z];

        /// <summary>
        /// Задать воксель
        /// </summary>
        public void SetVoxel(int x, int y, int z, Voxel voxel)
        {
            _voxels[y, x, z] = voxel;
            SetModifiedRender();
        }
    }
}
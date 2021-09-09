namespace VoxelEngine.Renderer.Chk
{
    /// <summary>
    /// Прорисовки чанка
    /// </summary>
    public class ChunkMeshs
    {
        // Формула получения вокселя в чанке
        // (y * VE.CHUNK_WIDTH + z) * VE.CHUNK_WIDTH + x

        public int X { get; protected set; }
        public int Z { get; protected set; }

        /// <summary>
        /// Сетка чанка сплошных блоков
        /// </summary>
        public ChunkMesh MeshDense { get; protected set; }
        /// <summary>
        /// Сетка чанка альфа блоков
        /// </summary>
        public ChunkMesh MeshAlpha { get; protected set; }

        public ChunkMeshs(int xpos, int zpos)
        {
            X = xpos;
            Z = zpos;
            MeshDense = new ChunkMesh(X, Z);
            MeshAlpha = new ChunkMesh(X, Z);
        }

        /// <summary>
        /// Количество полигонов
        /// </summary>
        public int CountPoligon
        {
            get
            {
                return MeshAlpha.CountPoligon + MeshDense.CountPoligon;
            }
        }

        /// <summary>
        /// Удалить сетки
        /// </summary>
        public void Delete()
        {
            MeshAlpha.Delete();
            MeshDense.Delete();
        }
    }
}

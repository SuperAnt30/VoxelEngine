namespace VoxelEngine
{
    /// <summary>
    /// Прорисовка чанка
    /// </summary>
    public class ChunkMesh : Render
    {

        protected override int[] _Attrs { get; } = new int[] { 3, 2, 3, 2 };

        // Формула получения вокселя в чанке
        // (y * VE.CHUNK_WIDTH + z) * VE.CHUNK_WIDTH + x

        public int X { get; protected set; }
        public int Z { get; protected set; }


        public ChunkMesh(int xpos, int zpos)
        {
            X = xpos;
            Z = zpos;
        }

        public void Render(float[] buffer)
        {
            Debag.GetInstance().CountPoligonChunk -= CountPoligon;
            _Render(buffer);
            Debag.GetInstance().CountPoligonChunk += CountPoligon;
        }
    }
}

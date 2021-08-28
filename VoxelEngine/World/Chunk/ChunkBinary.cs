using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using VoxelEngine.Binary;

namespace VoxelEngine.World.Chunk
{
    /// <summary>
    /// Запись чанка в файл
    /// </summary>
    public class ChunkBinary: ChunkHeir
    {
        public ChunkBinary(ChunkD chunk) : base(chunk) { }

        /// <summary>
        /// Чтение чанка
        /// </summary>
        public void Read(byte[] buffer)
        {
            ChunkBin chunk = Deserialize(buffer);
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        Voxel voxel = Chunk.GetVoxel(x, y, z);
                        voxel.SetVoxelData(chunk.Voxel[x, y, z]);
                        voxel.SetLightsFor(chunk.Light[x, y, z]);
                        Chunk.SetVoxel(x, y, z, voxel);
                    }
                }
            }
            //Chunk.SetChunkModified();
        }

        protected ChunkBin Deserialize(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms) as ChunkBin;
            }
        }

        /// <summary>
        /// Запись чанка
        /// </summary>
        public byte[] Write()
        {
            ChunkBin chunk = new ChunkBin();
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        Voxel voxel = Chunk.GetVoxel(x, y, z);
                        chunk.Voxel[x, y, z] = voxel.GetVoxelData();
                        chunk.Light[x, y, z] = voxel.GetLightsFor();
                    }
                }
            }

            return Serialize(chunk);
        }

        protected byte[] Serialize(ChunkBin chunk)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, chunk);
                return ms.GetBuffer();
            }
        }
    }
}

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using VoxelEngine.Binary;
using VoxelEngine.Util;

namespace VoxelEngine.World
{
    /// <summary>
    /// Регион запись региона мира в файл
    /// </summary>
    public class RegionBinary : Coords
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string Path { get; protected set; }

        protected static string path = "map";

        /// <summary>
        /// Файловый буфер каждого чанка
        /// </summary>
        protected RegionBin buffer = new RegionBin();

        public RegionBinary(int chunkX, int chunkZ)
        {
            X = chunkX >> 5;
            Z = chunkZ >> 5;
            Path = path + "/r" + X.ToString() + "_" + Z.ToString() + ".dat";
            buffer.Chunks = new byte[32, 32][];

            for (int x = 0; x < 32; x++)
            {
                for (int z = 0; z < 32; z++)
                {
                    buffer.Chunks[x, z] = new byte[0];
                }
            }
        }

        /// <summary>
        /// Получить кэш памяти региона в байтах
        /// </summary>
        public int Mem()
        {
            int mem = 0;
            for (int x = 0; x < 32; x++)
            {
                for (int z = 0; z < 32; z++)
                {
                    mem += buffer.Chunks[x, z].Length;
                }
            }
            return mem;
        }

        /// <summary>
        /// Чтение файла региона
        /// </summary>
        public void ReadFile()
        {
            if (File.Exists(Path))
            {
                using (FileStream ms = new FileStream(Path, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    buffer = formatter.Deserialize(ms) as RegionBin;
                }
            }
        }

        /// <summary>
        /// Запись файла региона
        /// </summary>
        public void WriteFile()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream ms = new FileStream(Path, FileMode.Create))
            {
                formatter.Serialize(ms, buffer);
            }
        }

        /// <summary>
        /// Получить данные чанка
        /// </summary>
        public byte[] GetChunk(int chunkX, int chunkZ)
        {
            int localX = chunkX - (X << 5);
            int localZ = chunkZ - (Z << 5);

            if (buffer.Chunks[localX, localZ].Length == 0) return null;
            return Gzip.Decompress(buffer.Chunks[localX, localZ]);
        }

        /// <summary>
        /// Задачть чанк
        /// </summary>
        public void SetChunk(int chunkX, int chunkZ, byte[] source)
        {
            int localX = chunkX - (X << 5);
            int localZ = chunkZ - (Z << 5);

            buffer.Chunks[localX, localZ] = source.Length == 0 ? source : Gzip.Compress(source);
            return;
        }
    }
}

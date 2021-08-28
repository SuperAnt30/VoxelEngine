using System;
using System.Collections.Generic;
using System.IO;
namespace VoxelEngine.World
{
    /// <summary>
    /// Объект файла региона
    /// * Устарел
    /// </summary>
    public class RegionFile : Coords
    {
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string Path { get; protected set; }

        protected static string path = "map";

        /// <summary>
        /// Файловый буфер каждого чанка
        /// </summary>
        protected byte[][] buffer = new byte[1024][];

        public RegionFile(int chunkX, int chunkZ)
        {
            X = chunkX >> 5;
            Z = chunkZ >> 5;
            Path = path + "/r" + X.ToString() + "_" + Z.ToString() + ".dat";

            for (int i = 0; i < 1024; i++)
            {
                buffer[i] = new byte[0];
            }
        }

        /// <summary>
        /// Получить кэш памяти региона в байтах
        /// </summary>
        public int Mem()
        {
            int mem = 0;
            for(int i = 0; i < buffer.Length; i++)
            {
                mem += buffer[i].Length;
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
                using (FileStream fileStream = new FileStream(Path, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(fileStream))
                    {
                        byte[] array = reader.ReadBytes((int)fileStream.Length);

                        // Собираем расположение
                        //int[] offsets = new int[1024];
                        for (int i = 0; i < 1024; i ++)
                        {
                            int offset = _BytesToInt(array, i * 4);
                            if (offset == 0)
                            {
                                buffer[i] = new byte[0];
                            }
                            else
                            {
                                // Читаем первый инт для узнать длинну его
                                int compressedSize = _BytesToInt(array, offset);
                                buffer[i] = new byte[compressedSize];
                                // Читаем байты чанка
                                offset += 4;
                                //compressedSize += offset;
                                Array.Copy(array, offset, buffer[i], 0, compressedSize);
                                
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Запись файла региона
        /// </summary>
        public void WriteFile()
        {
            //return;
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
            List<byte> array = new List<byte>();
            int offsets = 4096;

            // Собираем информацию смещению чанка
            for (int i = 0; i < 1024; i ++)
            {
                int offset = buffer[i].Length;
                if (offset == 0)
                {
                    array.AddRange(_IntToBytes(0));
                }
                else
                {
                    array.AddRange(_IntToBytes(offsets));
                    offsets += offset + 4;
                }
            }
            // Теперь конкретно массив вокселей чанка
            for (int i = 0; i < 1024; i++)
            {
                if (buffer[i].Length == 0) continue;
                array.AddRange(_IntToBytes(buffer[i].Length));
                array.AddRange(buffer[i]);
            }

            File.WriteAllBytes(Path, array.ToArray());
        }

        /// <summary>
        /// Получить данные чанка
        /// </summary>
        public byte[] GetChunk(int chunkX, int chunkZ)
        {
            int localX = chunkX - (X << 5);
            int localY = chunkZ - (Z << 5);
            int chunkIndex = localY * 32 + localX;

            if (buffer[chunkIndex].Length == 0) return null;

            return _DecompressRLE(buffer[chunkIndex]);
        }

        /// <summary>
        /// Задачть чанк
        /// </summary>
        public void SetChunk(int chunkX, int chunkZ, byte[] source)
        {
            int localX = chunkX - (X << 5);
            int localY = chunkZ - (Z << 5);
            int chunkIndex = localY * 32 + localX;

            buffer[chunkIndex] = source.Length == 0 ? source : _CompressRLE(source);
        }

        /// <summary>
        /// Получить инт из массива байт
        /// </summary>
        /// <param name="src">массив байт</param>
        /// <param name="offset">смещение</param>
        protected int _BytesToInt(byte[] src, int offset)
        {
            return (src[offset] << 24) | (src[offset + 1] << 16) | (src[offset + 2] << 8) | (src[offset + 3]);
        }

        /// <summary>
        /// Отправить инт (4 байта)
        /// </summary>
        /// <param name="value">инт</param>
        protected byte[] _IntToBytes(int value)
        {
            return new byte[] {
                (byte)(value >> 24 & 255),
                (byte)(value >> 16 & 255),
                (byte)(value >> 8 & 255),
                (byte)(value >> 0 & 255)
            };
        }

        #region CompressRLE

        /// <summary>
        /// Компрессия
        /// </summary>
        protected byte[] _CompressRLE(byte[] src)
        {
            List<byte> dst = new List<byte>();
            int counter = 1;
            byte c1 = src[0];
            byte c2 = src[1];
            for (int i = 2; i < src.Length; i += 2)
            {
                byte cnext1 = src[i];
                byte cnext2 = src[i + 1];
                if (cnext1 != c1 || cnext2 != c2 || counter == 256)
                {
                    dst.Add((byte)(counter - 1));
                    dst.Add(c1);
                    dst.Add(c2);
                    c1 = cnext1;
                    c2 = cnext2;
                    counter = 0;
                }
                counter++;
            }
            dst.Add((byte)(counter - 1));
            dst.Add(c1);
            dst.Add(c2);
            return dst.ToArray();
        }

        /// <summary>
        /// Декомпрессия
        /// </summary>
        protected byte[] _DecompressRLE(byte[] src)
        {
            List<byte> dst = new List<byte>();
            for (int i = 0; i < src.Length; i += 3)
            {
                byte counter = src[i];
                byte c1 = src[i + 1];
                byte c2 = src[i + 2];
                for (int j = 0; j <= counter; j++)
                {
                    dst.Add(c1);
                    dst.Add(c2);
                }
            }
            return dst.ToArray();
        }

        #endregion
    }
}

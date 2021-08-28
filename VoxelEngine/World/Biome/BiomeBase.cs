using VoxelEngine.World.Chunk;

namespace VoxelEngine.World.Biome
{
    /// <summary>
    /// Базовый биом, для всех
    /// </summary>
    public class BiomeBase : ChunkHeir
    {
        public BiomeBase(ChunkD chunk) : base(chunk) { }

        /// <summary>
        /// Возращаем сгенерированный столбец
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public virtual void Column(int x, int z, float height)
        {
            //for (int y = 3; y < 256; y++)
            //{
            //    Chunk.SetBlockState(x, y, z, EnumBlock.Air);
            //}
            //return;

            int yh = 64 + (int)(height * 64f);
            int yn = UpLayer(x, z, 4);
            EnumBlock eBlock;

            for (int y = 3; y < 256; y++)
            {
                if (y < yh - yn) eBlock = EnumBlock.Stone;
                else if (y < yh) eBlock = EnumBlock.Dirt;
                else if (y == yh) eBlock = EnumBlock.Grass;
                else eBlock = EnumBlock.Air;
                Chunk.SetBlockState(x, y, z, eBlock);
            }
            // Пещеры
            //Cave(x, z);
        }

        /// <summary>
        /// Слой для генерации верхнего грунта
        /// </summary>
        /// <param name="depth">толщина слоя * 2</param>
        /// <returns>значение от 0 - толщины</returns>
        protected int UpLayer(int x, int z, int depth)
        {
            float scale = 1.0f;
            float[] noise = new float[1];
            Chunk.World.Noise.Down.GenerateNoise2d(noise, Chunk.X * 16 + x, Chunk.Z * 16 + z, 1, 1, scale, scale);
            return 4 + (int)(noise[0] * 4f);
        }

        /// <summary>
        /// Генерация наза 0-3 блока
        /// </summary>
        public void Down()
        {
            float[] noise = new float[256];
            float scale = 1.0f;
            Chunk.World.Noise.Down.GenerateNoise2d(noise, Chunk.X * 16, Chunk.Z * 16, 16, 16, scale, scale);

            int count = 0;
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    float n = noise[count];
                    count++;

                    Chunk.SetBlockState(x, 0, z, EnumBlock.TileDark);
                    Chunk.SetBlockState(x, 1, z, n < .3 ? EnumBlock.TileDark : EnumBlock.Stone);
                    Chunk.SetBlockState(x, 2, z, n < -.3 ? EnumBlock.TileDark : EnumBlock.Stone);
                }
            }
        }

        /// <summary>
        /// Генерация пещер
        /// </summary>
        public void Cave(int x, int z)
        {
            int realX = Chunk.X * 16 + x;
            int realZ = Chunk.Z * 16 + z;
            float[] noise = new float[64];
            Chunk.World.Noise.Cave.GenerateNoise3d(noise, realX, 3, realZ, 1, 64, 1, .05f, .05f, .05f);
            for (int y = 3; y < 67; y++)
            {
                if (noise[y - 3] < -1f)
                {
                    Chunk.SetBlockState(x, y, z, EnumBlock.Air);
                }
            }
        }

        /// <summary>
        /// Генерация пещер
        /// </summary>
        public void Cave()
        {
            int count = 0;
            float[] noise = new float[4096];
            //NoiseGeneratorPerlin noise = new NoiseGeneratorPerlin(random, 2);
            for (int y0 = 0; y0 < 16; y0++)
            {
                Chunk.World.Noise.Cave.GenerateNoise3d(noise, Chunk.X * 16, y0 * 16, Chunk.Z * 16, 16, 16, 16, .05f, .05f, .05f);
                count = 0;
                for (int x = 0; x < 16; x++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        for (int y = 0; y < 16; y++)
                        {
                            if (noise[count] < -1f)
                            {
                                Chunk.SetBlockState(x, y0 * 16 + y, z, EnumBlock.Air);
                            }
                            count++;
                        }
                    }
                }
            }
        }
    }
}

using VoxelEngine.World.Chunk;

namespace VoxelEngine.World.Biome
{
    /// <summary>
    /// Базовый биом, для всех
    /// </summary>
    public class BiomeBase : ChunkHeir
    {
        /// <summary>
        /// Наличие пещер в биоме
        /// </summary>
        public bool IsCave { get; protected set; } = true;
        /// <summary>
        /// Удаление всех блоков
        /// </summary>
        public bool AllCave { get; protected set; } = false;

        public BiomeBase(ChunkD chunk) : base(chunk) { }

        /// <summary>
        /// Возращаем сгенерированный столбец
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public virtual void Column(int x, int z, float height, float wetness)
        {
            _Column(x, z, height, UpLayer(x, z, 4, .1f) + 4, EnumBlock.Grass, EnumBlock.Dirt);
        }

        protected void _Column(int x, int z, float height, int depth, EnumBlock levUp, EnumBlock levDown)
        {
            int yh = 65 + (int)(height * 64f);
            int yl = yh - depth;
            EnumBlock eBlock;

            for (int y = 3; y < 256; y++)
            {
                if (y == yh) eBlock = levUp;
                else if (y < yh && y >= yl) eBlock = levDown;
                else if (y < yl) eBlock = EnumBlock.Stone;
                else eBlock = EnumBlock.Air;

                Chunk.SetBlockState(x, y, z, eBlock);
            }
        }

        /// <summary>
        /// Слой для генерации верхнего грунта
        /// </summary>
        /// <param name="depth">толщина слоя * 2</param>
        /// <returns>значение от 0 - толщины</returns>
        protected int UpLayer(int x, int z, int depth, float scale)
        {
            float[] noise = new float[1];
            Chunk.World.Noise.Down.GenerateNoise2d(noise, Chunk.X * 16 + x, Chunk.Z * 16 + z, 1, 1, scale, scale);
            return depth + (int)(noise[0] * (float)depth);
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

                    Chunk.SetBlockState(x, 0, z, EnumBlock.Bedrock);
                    Chunk.SetBlockState(x, 1, z, n < .3 ? EnumBlock.Bedrock : EnumBlock.Stone);
                    Chunk.SetBlockState(x, 2, z, n < -.3 ? EnumBlock.Bedrock : EnumBlock.Stone);
                }
            }
        }

        /// <summary>
        /// Генерация пещер
        /// </summary>
        public void Cave(int x, int z, bool all)
        {
            int realX = Chunk.X * 16 + x;
            int realZ = Chunk.Z * 16 + z;
            float[] noise = new float[128];
            Chunk.World.Noise.Cave.GenerateNoise3d(noise, realX, 3, realZ, 1, 128, 1, .06f, .06f, .06f);
            for (int y = 3; y < 131; y++)
            {
                if (noise[y - 3] < -1f)
                {
                    if (all)
                    {
                        Chunk.SetBlockState(x, y, z, EnumBlock.Air);
                    }
                    else
                    {
                        Voxel voxel = Chunk.GetVoxel(x, y, z);
                        EnumBlock eBlock = voxel.GetEBlock();
                        if (voxel.IsEmpty || eBlock == EnumBlock.Stone)
                        {
                            Chunk.SetBlockState(x, y, z, EnumBlock.Air);
                        }
                    }
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

        /// <summary>
        /// Генерация облостей
        /// </summary>
        public void Area(int x, int z, EnumBlock eBlock, bool big)
        {
            int realX = Chunk.X * 16 + x;
            int realZ = Chunk.Z * 16 + z;
            float[] noise = new float[128];
            float res;
            if (big)
            {
                Chunk.World.Noise.Area.GenerateNoise3d(noise, realX, 3, realZ, 1, 128, 1, .1f, .1f, .1f);
                res = -.5f;
            } else
            {
                Chunk.World.Noise.Down.GenerateNoise3d(noise, realX, 3, realZ, 1, 128, 1, .1f, .1f, .1f);
                res = -.8f;
            }
            for (int y = 3; y < 131; y++)
            {
                if (noise[y - 3] < res)
                {
                    Voxel voxel = Chunk.GetVoxel(x, y, z);
                    if (voxel.IsEmpty || voxel.GetEBlock() == EnumBlock.Stone)
                    {
                        Chunk.SetBlockState(x, y, z, eBlock);
                    }
                }
            }
        }
    }
}

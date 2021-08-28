using System;
using VoxelEngine.World.Biome;
using VoxelEngine.World.Chunk;

namespace VoxelEngine.Gen
{
    /// <summary>
    /// Генерация чанка
    /// </summary>
    public class ChunkGenerate: ChunkHeir
    {
        protected BiomeBase biomeBase;
        protected BiomeWater water;
        protected BiomeGreenPlain greenPlain;
        protected BiomeDesert desert;
        protected BiomeMountains mountains;
        protected BiomeSwamp swamp;
        protected BiomeBeach beach;
        protected BiomeForest forest;
        protected BiomeMountainsDesert mountainsDesert;

        public ChunkGenerate(ChunkD chunk) : base(chunk)
        {
            biomeBase = new BiomeBase(chunk);
            water = new BiomeWater(chunk);
            greenPlain = new BiomeGreenPlain(chunk);
            desert = new BiomeDesert(chunk);
            mountains = new BiomeMountains(chunk);
            swamp = new BiomeSwamp(chunk);
            beach = new BiomeBeach(chunk);
            forest = new BiomeForest(chunk);
            mountainsDesert = new BiomeMountainsDesert(chunk);
        }
        
        public void Generation()
        {
            Map();
            //ReliefHeight();
            //biomeBase.Cave();
            //GenerationOld();
        }

        protected void Map()
        {
            biomeBase.Down();

            float[] heightNoise = new float[256];
            float[] wetnessNoise = new float[256];
            float scale = 0.2f;
            Chunk.World.Noise.HeightBiome.GenerateNoise2d(heightNoise, Chunk.X * 16, Chunk.Z * 16, 16, 16, scale, scale);
            Chunk.World.Noise.WetnessBiome.GenerateNoise2d(wetnessNoise, Chunk.X * 16, Chunk.Z * 16, 16, 16, scale, scale);

            
            int count = 0;
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    float w = wetnessNoise[count] / 132f;
                    float h = heightNoise[count] / 132f;
                    EnumBiome eBiome = DefineBiome(h, w);
                    BiomeBase biome = BiomeObject(eBiome);
                    biome.Column(x, z, -h);
                    //if (eBiome == EnumBiome.Water)
                    //{
                    //    water.Column(x, z, h);
                    //}
                    //else
                    //{
                    //    EnumBlock eBlock = BiomeBlock(h, w);
                    //    if (eBlock == EnumBlock.Stone) h *= 4f;
                    //    int yh = 64 - (int)(h * 64f);
                    //    if (eBlock == EnumBlock.Stone) yh -= 75;
                    //    //if (h < -3.8f) eBlock = EnumBlock.Stone;
                    //    //else if (h < 0f) eBlock = EnumBlock.Grass;
                    //    //else if (h < 0.2f) eBlock = EnumBlock.Sand;
                    //    //int n = (int)((fn + 16f) * 1f + 50); // 4, .1f, (+16f) * 1f + 50 для равнины хорош
                    //    for (int y = 0; y < 256; y++)
                    //    {
                    //        if (y < 30) Chunk.SetBlockState(x, y, z, EnumBlock.Stone);
                    //        else if (y < yh) Chunk.SetBlockState(x, y, z, eBlock);
                    //        else Chunk.SetBlockState(x, y, z, EnumBlock.Air);
                    //        //int id = n < y ? 0 : 3;
                    //        //Chunk.SetBlockState(x, y, z, eBlock);// (EnumBlock)id);
                    //    }
                    //}
                    count++;
                }
            }
        }

        /// <summary>
        /// Определяем объект для генерации биома
        /// </summary>
        protected BiomeBase BiomeObject(EnumBiome eBiome)
        {
            switch(eBiome)
            {
                case EnumBiome.Water: return water;
                case EnumBiome.GreenPlain: return greenPlain;
                case EnumBiome.Desert: return desert;
                case EnumBiome.Mountains: return mountains;
                case EnumBiome.Swamp: return swamp;
                case EnumBiome.Beach: return beach;
                case EnumBiome.Forest: return forest;
                case EnumBiome.MountainsDesert: return mountainsDesert;
                default: return biomeBase;
            }
        }

        /// <summary>
        /// Определить биом по двум шумам
        /// </summary>
        /// <param name="h">высота -1..+1</param>
        /// <param name="w">влажность -1..+1</param>
        /// <returns></returns>
        protected EnumBiome DefineBiome(float h, float w)
        {
            if (h < -.4f)
            {
                // высоко
                if (w < -.22f) return EnumBiome.MountainsDesert; // горы в пустыне
                return EnumBiome.Mountains; // горы каменные
            }
            if (h < 0f)
            {
                // средняя местность по высоте
                if (w < -.22f) return EnumBiome.Desert; // пустыня
                if (w < .1f) return EnumBiome.GreenPlain; // ровнина
                if (w < .42f) return EnumBiome.Forest; // лес
                return EnumBiome.Swamp; // болото
            }
            if (h < 0.02f)
            {
                // Пляжная высота
                if (w < -.22f) return EnumBiome.Desert; // пустыня
                if (w < .1f) return EnumBiome.Beach; // пляж
                if (w < .42f) return EnumBiome.Forest; // лес
                return EnumBiome.Swamp; // болото
            }
            // вода
            return EnumBiome.Water;
        }


        /// <summary>
        /// Заполняем рельеф
        /// </summary>
        protected void ReliefHeight()
        {
            //4, (0.05f - 0.1f)(диапазон - 8f..+ 8f)
            float[] heightNoise = new float[256];
            float[] wetnessNoise = new float[256];
            float scale = 0.2f;
            Chunk.World.Noise.HeightBiome.GenerateNoise2d(heightNoise, Chunk.X * 16, Chunk.Z * 16, 16, 16, scale, scale);
            Chunk.World.Noise.WetnessBiome.GenerateNoise2d(wetnessNoise, Chunk.X * 16, Chunk.Z * 16, 16, 16, scale, scale);

            int count = 0;
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    float w = wetnessNoise[count] / 132f;
                    float h = heightNoise[count] / 132f;
                    EnumBlock eBlock = BiomeBlock(h, w);
                    if (eBlock == EnumBlock.Stone) h *= 4f;
                    int yh = 64 - (int)(h * 64f);
                    if (eBlock == EnumBlock.Stone) yh -= 76;
                    for (int y = 0; y < 256; y++)
                    {
                        if (y < 30) Chunk.SetBlockState(x, y, z, EnumBlock.Stone);
                        else if (y < yh) Chunk.SetBlockState(x, y, z, eBlock);
                        else Chunk.SetBlockState(x, y, z, EnumBlock.Air);
                    }
                    count++;
                }
            }
        }

        protected EnumBlock BiomeBlock(float h, float w)
        {
            // h высота
            // w влажность
            //h /= 14f;
            //w /= 14f;
            if (h < -.4f)
            {
                // высоко
                if (w < -.22f) return EnumBlock.TileBrown; // горы в пустыне
                return EnumBlock.Stone; // горы каменные
            }
            if (h < 0f)
            {
                // средняя местность по высоте
                if (w < -.22f) return EnumBlock.Sand; // пустыня
                if (w < .1f) return EnumBlock.Grass; // ровнина
                if (w < .42f) return EnumBlock.Dirt; // лес
                return EnumBlock.TileDark; // болото
            }
            if (h < 0.02f)
            {
                // Пляжная высота
                if (w < -.22f) return EnumBlock.Sand; // пустыня
                if (w < .1f) return EnumBlock.TileGray; // пляж
                if (w < .42f) return EnumBlock.Dirt; // лес
                return EnumBlock.TileDark; // болото
            }
            // вода
            return EnumBlock.Water;
        }

        

        public void GenerationOld()
        {
            Random rand = new Random(2);



            //NoiseGeneratorPerlin noiseS = new NoiseGeneratorPerlin(rand, 4);
            //double[] stoneNoise = new double[256];
            //stoneNoise = noiseS.GenerationToArray(stoneNoise, Chunk.X * 16, Chunk.Z * 16, 16, 16, 0.0625f, 0.0625f, 1f);

            float[] stoneNoise = new float[4096]; // 2048
            NoiseGeneratorPerlin noiseO = new NoiseGeneratorPerlin(rand, 1);
            float no = .1f;

            //
            // +Octaves 2, no .05, хорошо для пещер, воздух < -1f
            // Octaves 1, no .04, хорошо для пещер, воздух < -0.5f
            // Octaves 2, no .05, хорошо для больших руд или блюшек земли, песка в камне < -1.5f
            // Octaves 1, no .1, хорошо для больших руд или блюшек земли, песка в камне < -0.5f
            // Octaves 1, no .1, хорошо длядорогих руд в камне < -0.8f

            //noiseO.generateNoiseOctaves(stoneNoise, Chunk.X * 16, 10, Chunk.Z * 16, 16, 8, 16, no, no, no);


            for (int y0 = 0; y0 < 16; y0++)
            {
                noiseO.GenerateNoise3d(stoneNoise, Chunk.X * 16, y0 * 16, Chunk.Z * 16, 16, 16, 16, no, no, no);
                int var6 = 0;
                for (int x = 0; x < 16; x++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        //double fn = stoneNoise[z * 16 + x];
                        //int n = (int)((fn + 7f) * 9f) + 30;
                        // Нижний уровень для бедрока можно
                        //int n = (int)(fn / 3.0f + 3.0f + rand.NextDouble() * 0.25f);


                        //double fn = stoneNoise[x * 16 + z];
                        //int n = (int)(fn + 110f);
                        for (int y = 0; y < 16; y++)
                        {
                            int id = 0;

                            //if (stoneNoise[x * 128 + z * 8 + y] < 0) id = 7;
                            //if (stoneNoise[x * 4096 + z * 256 + y] < 0) id = 7;
                            if (stoneNoise[var6] < -0.8f) id = 7;
                            //if (stoneNoise[var6] > -1f) id = 7;
                            var6++;
                            Chunk.SetBlockState(x, y0 * 16 + y, z, (EnumBlock)id);
                        }
                        continue;
                        for (int y = 8; y < 256; y++)
                        {

                            int id = 0;
                            //if (y < 8)
                            //{
                            //    noiseO.generateNoiseOctaves(stoneNoise, Chunk.X * 16, y, Chunk.Z * 16, 16, 1, 16, no, no, no);
                            //    if (stoneNoise[x * 16 + z] < 0) id = 7;
                            //}
                            //if (y < n) id = 7;
                            //if (y <= n)
                            //{
                            //    // если не воздух
                            //    if (y < 46)
                            //    {
                            //        // принудительно камень
                            //        id = 1;
                            //    }
                            //    else if (y < 67 && n < 67)
                            //    {
                            //        // камень
                            //        if (y < n - 3) id = 1;
                            //        // песок
                            //        else id = 4;
                            //    }
                            //    else if (y > 80)
                            //    {
                            //        // камень
                            //        id = 1;
                            //    }
                            //    else
                            //    {
                            //        // камень
                            //        if (y < n - 3) id = 1;
                            //        // земля
                            //        else if (y < n - 1) id = 2;
                            //        // дёрн
                            //        else id = 3;
                            //    }

                            //}
                            //else
                            //{
                            //    if (y < 65 && n < 67)
                            //    {
                            //        id = 11;
                            //    }
                            //}
                            Chunk.SetBlockState(x, y, z, (EnumBlock)id);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Объект кэш чанка
        /// </summary>
        //public ChunkD Chunk { get; protected set; }

    }
}

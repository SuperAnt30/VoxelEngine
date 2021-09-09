using VoxelEngine.World.Blk;
using VoxelEngine.World.Chk;

namespace VoxelEngine.World.Biome
{
    /// <summary>
    /// Биом Пустяня
    /// </summary>
    public class BiomeDesert : BiomeBase
    {
        public BiomeDesert(ChunkBase chunk) : base(chunk) { }

        /// <summary>
        /// Возращаем сгенерированный столбец
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public override void Column(int x, int z, float height, float wetness)
        {
            int yh = 65 + (int)(height * 64f);
            int yl = yh - UpLayer(x, z, 6, .1f) + 2;
            EnumBlock eBlock;

            for (int y = 3; y < 256; y++)
            {
                if (y <= yh && y >= yl) eBlock = EnumBlock.Sand;
                else if (y < yl) eBlock = EnumBlock.Stone;
                else eBlock = EnumBlock.Air;

                Chunk.SetBlockState(x, y, z, eBlock);

                if (y == yh && Grass(x, z, .6f))
                {
                    y++; Chunk.SetBlockState(x, y, z, EnumBlock.Cactus);
                    y++; Chunk.SetBlockState(x, y, z, EnumBlock.Cactus);
                    if (Grass(x, z, .65f))
                    {
                        y++; Chunk.SetBlockState(x, y, z, EnumBlock.Cactus);
                        y++; Chunk.SetBlockState(x, y, z, EnumBlock.Cactus);
                        if (Grass(x, z, .7f))
                        {
                            y++; Chunk.SetBlockState(x, y, z, EnumBlock.Cactus);
                        }
                    }
                }
            }
        }
    }
}

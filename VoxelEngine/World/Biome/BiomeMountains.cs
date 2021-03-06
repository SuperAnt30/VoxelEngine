using VoxelEngine.World.Blk;
using VoxelEngine.World.Chk;

namespace VoxelEngine.World.Biome
{
    /// <summary>
    /// Биом Горы
    /// </summary>
    public class BiomeMountains : BiomeBase
    {
        public BiomeMountains(ChunkBase chunk) : base(chunk)
        {
            AllCave = true;
        }

        /// <summary>
        /// Возращаем сгенерированный столбец
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public override void Column(int x, int z, float height, float wetness)
        {
            int yh = (int)(height * 256f) - 11;
            int yl = yh - UpLayer(x, z, 12, .5f);
            EnumBlock eBlock;

            for (int y = 3; y < 256; y++)
            {
                if (y == yh) eBlock = yl < 86 ? EnumBlock.Grass : EnumBlock.Stone;
                else if (y < yh && y >= yl) eBlock = yl < 86 ? EnumBlock.Dirt : EnumBlock.Stone;
                else if (y < yl) eBlock = EnumBlock.Stone;
                else eBlock = EnumBlock.Air;

                Chunk.SetBlockState(x, y, z, eBlock);

                if (y == yh && eBlock == EnumBlock.Grass && Grass(x, z, .2f))
                {
                    y++;
                    Chunk.SetBlockState(x, y, z, EnumBlock.TallGrass);
                }
            }
        }
    }
}

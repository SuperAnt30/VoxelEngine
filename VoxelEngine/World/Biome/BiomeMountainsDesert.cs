using VoxelEngine.World.Blk;
using VoxelEngine.World.Chk;

namespace VoxelEngine.World.Biome
{
    /// <summary>
    /// Биом Горы в пустыне
    /// </summary>
    public class BiomeMountainsDesert : BiomeBase
    {
        public BiomeMountainsDesert(ChunkBase chunk) : base(chunk)
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
                if (y <= yh && y >= yl) eBlock = yl < 86 ? EnumBlock.Sand : EnumBlock.Diorite;
                else if (y < yl) eBlock = EnumBlock.Stone;
                else eBlock = EnumBlock.Air;

                Chunk.SetBlockState(x, y, z, eBlock);
            }
        }

    }
}

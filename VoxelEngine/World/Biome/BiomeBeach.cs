using VoxelEngine.World.Chunk;

namespace VoxelEngine.World.Biome
{
    /// <summary>
    /// Биом Пляж
    /// </summary>
    public class BiomeBeach : BiomeBase
    {
        public BiomeBeach(ChunkD chunk) : base(chunk) { }

        /// <summary>
        /// Возращаем сгенерированный столбец
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public override void Column(int x, int z, float height)
        {
            int yh = 64 + (int)(height * 64f);
            int yn = UpLayer(x, z, 4);
            EnumBlock eBlock;

            for (int y = 3; y < 256; y++)
            {
                if (y < yh - yn) eBlock = EnumBlock.Stone;
                else if (y <= yh) eBlock = EnumBlock.Sand;
                else eBlock = EnumBlock.Air;
                Chunk.SetBlockState(x, y, z, eBlock);
            }
            // Пещеры
            //Cave(x, z);
        }
    }
}

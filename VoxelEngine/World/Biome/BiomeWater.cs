using VoxelEngine.World.Chunk;

namespace VoxelEngine.World.Biome
{
    /// <summary>
    /// Биом воды
    /// </summary>
    public class BiomeWater : BiomeBase
    {
        public BiomeWater(ChunkD chunk) : base(chunk) { }

        /// <summary>
        /// Возращаем сгенерированный столбец
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public override void Column(int x, int z, float height)
        {
            //base.Column(x, z, height);
            //return;

            int yn = UpLayer(x, z, 8);
            int yh = 64 + (int)(height * 64f);
            EnumBlock eBlock;
            for (int y = 0; y < 256; y++)
            {
                if (y < yh - yn) eBlock = EnumBlock.Stone;
                else if (y <= yh) eBlock = EnumBlock.Sand;
                else if (y < 64) eBlock = EnumBlock.Water;
                else eBlock = EnumBlock.Air;

                Chunk.SetBlockState(x, y, z, eBlock);
            }
        }
    }
}

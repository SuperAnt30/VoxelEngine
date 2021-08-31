using VoxelEngine.World.Chunk;

namespace VoxelEngine.World.Biome
{
    /// <summary>
    /// Биом Болото
    /// </summary>
    public class BiomeSwamp : BiomeBase
    {
        public BiomeSwamp(ChunkD chunk) : base(chunk) { }

        /// <summary>
        /// Возращаем сгенерированный столбец
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public override void Column(int x, int z, float height, float wetness)
        {
            int h = (int)(height * 64f);
            int yh = 65 + h;
            int yl = yh - UpLayer(x, z, 6, .5f);
            int yl2 = yh;
            int h2 = (int)(height * 50f);
            
            // ближе к воде
            if (h < -1) yl2 += UpLayer(x, z, h2, .2f) - (h2);
            // ближе к берегу
            if (h > 1) yl2 -= UpLayer(x, z, -h2, .2f) + h2;
            EnumBlock eBlock;

            for (int y = 3; y < 256; y++)
            {
                // Верхний слой
                if (y == yl2) eBlock = y < 64 ? EnumBlock.Dirt : EnumBlock.Grass;
                // Прослока между верхней и камнями
                else if (y < yl2 && y > yl) eBlock = EnumBlock.Dirt;
                // Нижний камни
                else if (y <= yl) eBlock = EnumBlock.Stone;
                // Если надо вода
                else if (y <= 64) eBlock = EnumBlock.Water;
                // Остальное воздух
                else eBlock = EnumBlock.Air;

                Chunk.SetBlockState(x, y, z, eBlock);
            }
        }
    }
}

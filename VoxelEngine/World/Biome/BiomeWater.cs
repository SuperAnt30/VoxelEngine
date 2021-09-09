using VoxelEngine.World.Blk;
using VoxelEngine.World.Chk;

namespace VoxelEngine.World.Biome
{
    /// <summary>
    /// Биом воды
    /// </summary>
    public class BiomeWater : BiomeBase
    {
        public BiomeWater(ChunkBase chunk) : base(chunk) { }

        /// <summary>
        /// Возращаем сгенерированный столбец
        /// </summary>
        public override void Column(int x, int z, float height, float wetness)
        {
            int yh = 64 + (int)(height * 64f);
            int yl = yh - UpLayer(x, z, 8, .1f);
            EnumBlock eBlock;       
            for (int y = 3; y < 256; y++)
            {
                // Верхний с прослойкой между верхней и камнями
                if (y <= yh && y >= yl)
                {
                    eBlock = (yl < 48 || wetness > .1f) ? EnumBlock.Dirt : EnumBlock.Sand;
                    // Поближе к глубине и где влажность сильная типа глина, ил
                    if (wetness > .1f && yl < 38) eBlock = EnumBlock.TileDark;
                }
                // Нижний камни
                else if (y < yl) eBlock = EnumBlock.Stone;
                // Вода
                else if (y <= 64) eBlock = EnumBlock.Water;
                // Остальное воздух
                else eBlock = EnumBlock.Air;

                Chunk.SetBlockState(x, y, z, eBlock);
            }
        }
    }
}

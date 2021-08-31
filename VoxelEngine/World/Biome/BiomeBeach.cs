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
        public override void Column(int x, int z, float height, float wetness)
        {
            _Column(x, z, height - 0.008f, UpLayer(x, z, 6, .1f) + 1, EnumBlock.Sand, EnumBlock.Sand);
        }
    }
}

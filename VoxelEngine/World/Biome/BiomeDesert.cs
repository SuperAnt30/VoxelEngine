using VoxelEngine.World.Chunk;

namespace VoxelEngine.World.Biome
{
    /// <summary>
    /// Биом Пустяня
    /// </summary>
    public class BiomeDesert : BiomeBase
    {
        public BiomeDesert(ChunkD chunk) : base(chunk) { }

        /// <summary>
        /// Возращаем сгенерированный столбец
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public override void Column(int x, int z, float height, float wetness)
        {
            _Column(x, z, height, UpLayer(x, z, 6, .1f) + 2, EnumBlock.Sand, EnumBlock.Sand);
        }
    }
}

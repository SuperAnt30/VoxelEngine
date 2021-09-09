using VoxelEngine.Glm;
using VoxelEngine.World.Biome;

namespace VoxelEngine.Renderer.Blk
{
    /// <summary>
    /// Цвет блоков от биома
    /// </summary>
    public class BlockColorBiome
    {
        /// <summary>
        /// Цвет травы
        /// </summary>
        public static vec4 Grass(EnumBiome eBiome)
        {
            switch(eBiome)
            {
                case EnumBiome.Desert:
                case EnumBiome.MountainsDesert: return new vec4(0.96f, 0.73f, 0.35f, 1f);
                case EnumBiome.Forest: return new vec4(0.46f, 0.63f, 0.25f, 1f);
                case EnumBiome.Swamp: return new vec4(0.56f, 0.63f, 0.35f, 1f);
            }
            return new vec4(0.56f, 0.73f, 0.35f, 1f);
        }

        /// <summary>
        /// Цвет листвы
        /// </summary>
        public static vec4 Leaves(EnumBiome eBiome)
        {
            switch (eBiome)
            {
                case EnumBiome.Desert:
                case EnumBiome.MountainsDesert: return new vec4(0.96f, 0.73f, 0.35f, 1f);
                case EnumBiome.Forest: return new vec4(0.46f, 0.63f, 0.25f, 1f);
                case EnumBiome.Swamp: return new vec4(0.56f, 0.63f, 0.35f, 1f);
            }
            return new vec4(0.56f, 0.73f, 0.35f, 1f);
        }

        /// <summary>
        /// Цвет воды
        /// </summary>
        public static vec4 Water(EnumBiome eBiome)
        {
            switch (eBiome)
            {
                case EnumBiome.Swamp: return new vec4(0.36f, 0.63f, 0.68f, 1f);
            }
            return new vec4(0.24f, 0.45f, 0.88f, 1f);
        }
    }
}

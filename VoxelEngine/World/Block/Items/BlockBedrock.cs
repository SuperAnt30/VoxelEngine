using VoxelEngine.Model;

namespace VoxelEngine.World
{
    /// <summary>
    /// Блок Коренная порода
    /// </summary>
    public class BlockBedrock : Block
    {
        /// <summary>
        /// Блок Коренная порода
        /// </summary>
        public BlockBedrock()
        {
            Boxes = new Box[] { new Box(24) };
        }
    }
}

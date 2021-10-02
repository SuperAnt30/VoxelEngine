using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    /// <summary>
    /// Блок Коренная порода
    /// </summary>
    public class BlockBedrock : BlockBase
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

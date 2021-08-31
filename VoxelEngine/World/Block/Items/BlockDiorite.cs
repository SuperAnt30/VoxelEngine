using VoxelEngine.Model;

namespace VoxelEngine.World
{
    /// <summary>
    /// Блок камня Диорит
    /// </summary>
    public class BlockDiorite : Block
    {
        /// <summary>
        /// Блок камня Диорит
        /// </summary>
        public BlockDiorite()
        {
            Boxes = new Box[] { new Box(23) };
        }
    }
}

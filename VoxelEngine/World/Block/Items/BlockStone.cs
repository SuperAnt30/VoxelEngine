using VoxelEngine.Model;

namespace VoxelEngine.World
{
    /// <summary>
    /// Блок камня
    /// </summary>
    public class BlockStone : Block
    {
        /// <summary>
        /// Блок камня
        /// </summary>
        public BlockStone()
        {
            Boxes = new Box[] { new Box(0) };
        }
    }
}

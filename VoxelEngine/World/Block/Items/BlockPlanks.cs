using VoxelEngine.Model;

namespace VoxelEngine.World
{
    
    public class BlockPlanks : Block
    {
        /// <summary>
        /// Блок доски
        /// </summary>
        public BlockPlanks()
        {
            Boxes = new Box[] { new Box(4) };
        }
    }
}

using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    
    public class BlockPlanks : BlockBase
    {
        /// <summary>
        /// Блок доски
        /// </summary>
        public BlockPlanks()
        {
            Boxes = new Box[] { new Box(4) };
            soundBreak = "dig.wood";
            soundPut = "dig.wood";
            soundStep = "step.wood";
        }
    }
}

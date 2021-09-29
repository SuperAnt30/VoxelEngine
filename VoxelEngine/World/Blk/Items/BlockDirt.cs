using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    
    public class BlockDirt : Block
    {   
        /// <summary>
        /// Блок земли
        /// </summary>
        public BlockDirt()
        {
            Boxes = new Box[] { new Box(1) };
            soundBreak = "dig.grass";
            soundPut = "dig.grass";
            soundStep = "step.sand";
        }
    }
}

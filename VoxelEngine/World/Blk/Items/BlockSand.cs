using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    
    public class BlockSand : Block
    {
        /// <summary>
        /// Блок песка
        /// </summary>
        public BlockSand()
        {
            Boxes = new Box[] { new Box(5) };
            soundBreak = "dig.sand";
            soundPut = "dig.sand";
            soundStep = "step.sand";
        }
    }
}

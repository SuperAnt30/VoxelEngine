using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    
    public class BlockDoor : BlockBase
    {
        /// <summary>
        /// Блок двери
        /// </summary>
        public BlockDoor()
        {
            Boxes = new Box[] { new Box(48) };

            AllDrawing = true;
            LightingYourself = true;
            IsGroupModel = true;
            IsCube = false;
            soundBreak = "dig.wood";
            soundPut = "dig.wood";
            soundStep = "step.wood";
        }
    }
}

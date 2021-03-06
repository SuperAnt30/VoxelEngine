using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    public class BlockLeaves : BlockBase
    {
        /// <summary>
        /// Блок листвы
        /// </summary>
        public BlockLeaves()
        {
            Boxes = new Box[] { new Box(34, true) };

            IsCollision = false;
            AllDrawing = true;
            IsLeaves = true;
            IsCube = false;
            soundBreak = "dig.grass";
            soundPut = "dig.grass";
        }
    }
}

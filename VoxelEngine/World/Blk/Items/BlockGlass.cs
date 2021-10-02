using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    public class BlockGlass : BlockBase
    {
        /// <summary>
        /// Блок Стекла
        /// </summary>
        public BlockGlass()
        {
            Boxes = new Box[] { new Box(19) };
            //LightOpacity = 10;
            IsAlphe = true;
            soundBreak = "dig.glass";
            soundBreakCount = 3;
        }
    }
}

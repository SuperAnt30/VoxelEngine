using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    
    public class BlockTileDark : Block
    {
        /// <summary>
        /// Блок плитка темная
        /// </summary>
        public BlockTileDark()
        {
            Boxes = new Box[] { new Box(7) };
        }
    }
}

using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    
    public class BlockTileGray : Block
    {
        /// <summary>
        /// Блок плитка серая
        /// </summary>
        public BlockTileGray()
        {
            Boxes = new Box[] { new Box(6) };
        }
    }
}

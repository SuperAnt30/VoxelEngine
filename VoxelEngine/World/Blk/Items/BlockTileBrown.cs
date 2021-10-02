using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    
    public class BlockTileBrown : BlockBase
    {
        /// <summary>
        /// Блок плитка коричневая
        /// </summary>
        public BlockTileBrown()
        {
            Boxes = new Box[] { new Box(8) };
        }
    }
}

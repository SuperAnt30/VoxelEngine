using VoxelEngine.Model;

namespace VoxelEngine
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

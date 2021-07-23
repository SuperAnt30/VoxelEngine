using VoxelEngine.Model;

namespace VoxelEngine
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

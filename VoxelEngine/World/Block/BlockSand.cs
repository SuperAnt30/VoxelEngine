using VoxelEngine.Model;

namespace VoxelEngine
{
    
    public class BlockSand : Block
    {
        /// <summary>
        /// Блок песка
        /// </summary>
        public BlockSand()
        {
            Boxes = new Box[] { new Box(5) };
        }
    }
}

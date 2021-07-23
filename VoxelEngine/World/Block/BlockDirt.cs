using VoxelEngine.Model;

namespace VoxelEngine
{
    
    public class BlockDirt : Block
    {   
        /// <summary>
        /// Блок земли
        /// </summary>
        public BlockDirt()
        {
            Boxes = new Box[] { new Box(1) };
        }
    }
}

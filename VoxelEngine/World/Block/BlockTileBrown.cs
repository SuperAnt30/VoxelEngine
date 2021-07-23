using VoxelEngine.Model;

namespace VoxelEngine
{
    
    public class BlockTileBrown : Block
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

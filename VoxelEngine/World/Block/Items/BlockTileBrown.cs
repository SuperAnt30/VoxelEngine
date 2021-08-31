using VoxelEngine.Model;

namespace VoxelEngine.World
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

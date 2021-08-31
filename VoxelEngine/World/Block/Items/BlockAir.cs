using VoxelEngine.Model;

namespace VoxelEngine.World
{
    /// <summary>
    /// Блок воздуха, пустота
    /// </summary>
    public class BlockAir : Block
    {
        public BlockAir()
        {
            //LightOpacity = 15;
            Boxes = new Box[] { new Box(33) };

            IsCollision = false;
            IsAction = false;
        }
    }
}

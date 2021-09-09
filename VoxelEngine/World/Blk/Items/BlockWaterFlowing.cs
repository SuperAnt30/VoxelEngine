using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    /// <summary>
    /// Блок текучей воды
    /// </summary>
    public class BlockWaterFlowing : Block
    {
        public BlockWaterFlowing()
        {
            Boxes = new Box[] { new Box(31, true) };
            IsAlphe = true;
            IsCollision = false;
            IsAction = false;
            IsWater = true;
        }
    }
}

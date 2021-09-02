using VoxelEngine.Glm;
using VoxelEngine.Model;
using VoxelEngine.Util;

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

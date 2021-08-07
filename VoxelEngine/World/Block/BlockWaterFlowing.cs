using VoxelEngine.Glm;
using VoxelEngine.Model;
using VoxelEngine.Util;

namespace VoxelEngine
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
            Color = new vec4(0.24f, 0.45f, 0.88f, 1f);
            //Color = new vec4(0.54f, 0.65f, 0.88f, 1f);
            IsCollision = false;
            IsAction = false;
            IsWater = true;
        }
    }
}

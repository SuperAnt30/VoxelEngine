using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

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
            IsCube = false;

            //soundBreak = "";
            //soundPut = "";
            //soundStep = "";
            //soundBreakCount = 0;
            //soundPutCount = 0;
            //soundStepCount = 0;
        }
    }
}

using VoxelEngine.Util;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    public class BlockBrol : Block
    {
        /// <summary>
        /// Блок брол, автор Вероника
        /// </summary>
        public BlockBrol()
        {
            Boxes = new Box[] {
                new Box()
                {
                    Faces = new Face[]
                    {
                        new Face(Pole.Up, 20),
                        new Face(Pole.Down, 21),
                        new Face(Pole.East, 22),
                        new Face(Pole.North, 22),
                        new Face(Pole.South, 22),
                        new Face(Pole.West, 22)
                    }
                }
            };

            LightValue = 14;
        }
    }
}

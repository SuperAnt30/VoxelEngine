using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;
using VoxelEngine.Util;

namespace VoxelEngine.World
{
    
    public class BlockGrass : Block
    {
        /// <summary>
        /// Блок травы
        /// </summary>
        public BlockGrass()
        {
            Boxes = new Box[] {
                new Box()
                {
                    Faces = new Face[]
                    {
                        new Face(Pole.Up, 2, true),
                        new Face(Pole.Down, 1),
                        new Face(Pole.East, 1),
                        new Face(Pole.North, 1),
                        new Face(Pole.South, 1),
                        new Face(Pole.West, 1)
                    }
                },
                new Box()
                {
                    Faces = new Face[]
                    {
                        new Face(Pole.East, 3, true),
                        new Face(Pole.North, 3, true),
                        new Face(Pole.South, 3, true),
                        new Face(Pole.West, 3, true)
                    }
                }
            };

            IsGrass = true;
        }
    }
}

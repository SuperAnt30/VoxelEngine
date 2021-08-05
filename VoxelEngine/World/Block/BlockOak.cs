using VoxelEngine.Model;
using VoxelEngine.Util;

namespace VoxelEngine
{
    public class BlockOak : Block
    {
        /// <summary>
        /// Блок бревна
        /// </summary>
        public BlockOak()
        {
            Boxes = new Box[] {
                new Box()
                {
                    Faces = new Face[]
                    {
                        new Face(Pole.Up, 32),
                        new Face(Pole.Down, 32),
                        new Face(Pole.East, 33),
                        new Face(Pole.North, 33),
                        new Face(Pole.South, 33),
                        new Face(Pole.West, 33)
                    }
                }
            };
        }
    }
}

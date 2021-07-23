using VoxelEngine.Glm;
using VoxelEngine.Model;
using VoxelEngine.Util;

namespace VoxelEngine
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
            Color = new vec4(0.56f, 0.73f, 0.35f, 1f);
            //Color = new vec4(0.96f, 0.73f, 0.35f, 1f);
            //Color = new vec4(0.86f, 0.33f, 0.25f, 1f);
        }
    }
}

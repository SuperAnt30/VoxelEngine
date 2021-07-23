using VoxelEngine.Glm;
using VoxelEngine.Model;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Блок воды
    /// </summary>
    public class BlockWater : Block
    {
        public BlockWater()
        {
            Boxes = new Box[] {
                new Box()
                {
                    From = new vec3(0, 0, 0),
                    To = new vec3(1f, 1f - VE.UV_SIZE, 1f),
                    Faces = new Face[]
                    {
                        new Face(Pole.Up, 15, true),
                        new Face(Pole.Down, 15, true),
                    }
                },
                new Box()
                {
                    From = new vec3(0, 0, 0),
                    To = new vec3(1f, 1f - VE.UV_SIZE, 1f),
                    Faces = new Face[]
                    {
                        new Face(Pole.East, 31, true),
                        new Face(Pole.North, 31, true),
                        new Face(Pole.South, 31, true),
                        new Face(Pole.West, 31, true)
                    }
                }
            };
            //LightOpacity = 14;
            IsAlphe = true;
            Color = new vec4(0.24f, 0.45f, 0.88f, 1f);
        }

        /// <summary>
        /// Второй вариант для прорисовки
        /// </summary>
        public override void BoxesTwo()
        {
            Boxes = new Box[] {
                new Box()
                {
                    Faces = new Face[]
                    {
                        new Face(Pole.Up, 15, true),
                        new Face(Pole.Down, 15, true),
                    }
                },
                new Box()
                {
                    Faces = new Face[]
                    {
                        new Face(Pole.East, 31, true),
                        new Face(Pole.North, 31, true),
                        new Face(Pole.South, 31, true),
                        new Face(Pole.West, 31, true)
                    }
                }
            };
        }
    }
}

using VoxelEngine.Glm;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;
using VoxelEngine.Util;

namespace VoxelEngine.World
{
    /// <summary>
    /// Блок воды
    /// </summary>
    public class BlockWater : BlockBase
    {
        public BlockWater()
        {
            Boxes = new Box[] {
                //new Box()
                //{
                //    From = new vec3(0, 0, 0),
                //    To = new vec3(1f, 1f - VE.UV_SIZE, 1f),
                //    Faces = new Face[]
                //    {
                //        new Face(Pole.Up, 15, true, true),
                //        new Face(Pole.Down, 15, true),
                //    }
                //},
                new Box()
                {
                    From = new vec3(0, 0, 0),
                    //To = new vec3(1f, 1f - VE.UV_SIZE, 1f),
                    To = new vec3(1f, 1f, 1f),
                    Faces = new Face[]
                    {
                        new Face(Pole.Up, 15, true, true),
                        new Face(Pole.Down, 15, true),
                        new Face(Pole.East, 31, true),
                        new Face(Pole.North, 31, true),
                        new Face(Pole.South, 31, true),
                        new Face(Pole.West, 31, true)
                    }
                }
            };
            IsAlphe = true;
            IsAction = false;
            IsCollision = false;
            IsWater = true;
            IsCube = false;
            soundPut = "liquid.swim";
        }

        /// <summary>
        /// Второй вариант для прорисовки
        /// </summary>
        //public override void BoxesTwo()
        //{
        //    Boxes = new Box[] {
        //        new Box()
        //        {
        //            Faces = new Face[]
        //            {
        //                new Face(Pole.Up, 15, true),
        //                new Face(Pole.Down, 15, true),
        //            }
        //        },
        //        new Box()
        //        {
        //            Faces = new Face[]
        //            {
        //                new Face(Pole.East, 31, true),
        //                new Face(Pole.North, 31, true),
        //                new Face(Pole.South, 31, true),
        //                new Face(Pole.West, 31, true)
        //            }
        //        }
        //    };
        //}
    }
}

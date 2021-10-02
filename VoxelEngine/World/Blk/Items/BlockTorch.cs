using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    public class BlockTorch : BlockBase
    {
        /// <summary>
        /// Блок факела
        /// </summary>
        public BlockTorch()
        {
            HitBox = new Box(new vec3(VE.UV_SIZE * 7, 0, VE.UV_SIZE * 7),
                new vec3(1f - VE.UV_SIZE * 7, 1f - VE.UV_SIZE * 6, 1f - VE.UV_SIZE * 7));

            Boxes = new Box[] {
                new Box()
                {
                    //From = new vec3(VE.UV_SIZE * 7, 0, VE.UV_SIZE * 7),
                    From = new vec3(0.4375f, 0, 0.4375f),
                    //To = new vec3(1f - VE.UV_SIZE * 7, 1f - VE.UV_SIZE * 6, 1f - VE.UV_SIZE * 7),
                    To = new vec3(0.5625f, 1f - VE.UV_SIZE * 6, 0.5625f),
                    // VE.UV_SIZE / 16 * 7, VE.UV_SIZE / 16 * 6
                    UVFrom = new vec2(0.02734375f, 0.0234375f),  
                    // VE.UV_SIZE / 16 * 9, VE.UV_SIZE / 16 * 8
                    UVTo = new vec2(0.03515625f, 0.03125f), 
                    Faces = new Face[]
                    {
                        new Face(Pole.Up, 40)
                    }
                },
                new Box()
                {
                    From = new vec3(0.4375f, 0, 0.4375f),
                    To = new vec3(0.5625f, 1f - VE.UV_SIZE * 6, 0.5625f),
                    // VE.UV_SIZE / 16 * 7, VE.UV_SIZE / 16 * 14
                    UVFrom = new vec2(0.02734375f, 0.0546875f), 
                    UVTo = new vec2(0.03515625f, VE.UV_SIZE),
                    Faces = new Face[]
                    {
                        new Face(Pole.Down, 40),
                    }
                },
                new Box()
                {
                    From = new vec3(0, 0, VE.UV_SIZE * 7),
                    To = new vec3(1f, 1f, 1f - VE.UV_SIZE * 7),
                    Faces = new Face[]
                    {
                        new Face(Pole.North, 40),
                        new Face(Pole.South, 40),
                    }
                },
                new Box()
                {
                    From = new vec3(VE.UV_SIZE * 7, 0, 0),
                    To = new vec3(1f - VE.UV_SIZE * 7, 1f, 1f),
                    Faces = new Face[]
                    {
                        new Face(Pole.East, 40),
                        new Face(Pole.West, 40)
                    }
                }
            };
            AllDrawing = true;
            LightingYourself = true;
            IsCube = false;
            IsCollision = false;
            LightValue = 14;
            //soundBreak = "dig.glass";
            //soundBreakCount = 3;
        }
    }
}

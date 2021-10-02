using VoxelEngine.Glm;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;
using VoxelEngine.Util;

namespace VoxelEngine.World
{
    public class BlockSapling : BlockBase
    {
        /// <summary>
        /// Блок саженца
        /// </summary>
        public BlockSapling()
        {
            Boxes = new Box[] {
                new Box()
                {
                    From = new vec3(0, 0, .5f),
                    To = new vec3(1f, 1f, .5f),
                    Faces = new Face[]
                    {
                        new Face(Pole.North, 35),
                        new Face(Pole.South, 35),
                    }
                },
                new Box()
                {
                    From = new vec3(.5f, 0, 0),
                    To = new vec3(.5f, 1f, 1f),
                    Faces = new Face[]
                    {
                        new Face(Pole.East, 35),
                        new Face(Pole.West, 35)
                    }
                }
            };
            IsCollision = false;
            AllDrawing = true;
            LightingYourself = true;
            IsCube = false;
            soundBreak = "dig.grass";
            soundPut = "dig.grass";
        }
    }
}

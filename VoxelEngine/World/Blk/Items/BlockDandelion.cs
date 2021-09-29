using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;

namespace VoxelEngine.World
{
    public class BlockDandelion : Block
    {
        /// <summary>
        /// Блок Одуванчик
        /// </summary>
        public BlockDandelion()
        {
            Boxes = new Box[] {
                new Box()
                {
                    From = new vec3(0, 0, .5f),
                    To = new vec3(1f, 1f, .5f),
                    RotateYaw = glm.pi45,
                    Faces = new Face[]
                    {
                        new Face(Pole.North, 39),
                        new Face(Pole.South, 39),
                    }
                },
                new Box()
                {
                    From = new vec3(.5f, 0, 0),
                    To = new vec3(.5f, 1f, 1f),
                    RotateYaw = glm.pi45,
                    Faces = new Face[]
                    {
                        new Face(Pole.East, 39),
                        new Face(Pole.West, 39)
                    }
                }
            };

            IsLeaves = true;
            IsCollision = false;
            AllDrawing = true;
            IsGrass = true;
            LightingYourself = true;
            IsCube = false;
            soundBreak = "dig.grass";
        }
    }
}

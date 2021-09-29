using VoxelEngine.Glm;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;
using VoxelEngine.Util;

namespace VoxelEngine.World
{
    public class BlockPoppy : Block
    {
        /// <summary>
        /// Блок Мак
        /// </summary>
        public BlockPoppy()
        {
            Boxes = new Box[] {
                new Box()
                {
                    From = new vec3(0, 0, .5f),
                    To = new vec3(1f, 1f, .5f),
                    RotateYaw = glm.pi45,
                    Faces = new Face[]
                    {
                        new Face(Pole.North, 38),
                        new Face(Pole.South, 38),
                    }
                },
                new Box()
                {
                    From = new vec3(.5f, 0, 0),
                    To = new vec3(.5f, 1f, 1f),
                    RotateYaw = glm.pi45,
                    Faces = new Face[]
                    {
                        new Face(Pole.East, 38),
                        new Face(Pole.West, 38)
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
            soundPut = "dig.grass";
        }
    }
}

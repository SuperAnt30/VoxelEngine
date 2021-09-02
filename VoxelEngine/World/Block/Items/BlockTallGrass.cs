using VoxelEngine.Glm;
using VoxelEngine.Model;
using VoxelEngine.Util;

namespace VoxelEngine.World
{
    public class BlockTallGrass : Block
    {
        /// <summary>
        /// Блок высокой травы
        /// </summary>
        public BlockTallGrass()
        {
            Boxes = new Box[] {
                new Box()
                {
                    From = new vec3(0, 0, .5f),
                    To = new vec3(1f, 1f, .5f),
                    RotateYaw = glm.pi45,
                    Faces = new Face[]
                    {
                        new Face(Pole.North, 37, true),
                        new Face(Pole.South, 37, true),
                    }
                },
                new Box()
                {
                    From = new vec3(.5f, 0, 0),
                    To = new vec3(.5f, 1f, 1f),
                    RotateYaw = glm.pi45,
                    Faces = new Face[]
                    {
                        new Face(Pole.East, 37, true),
                        new Face(Pole.West, 37, true)
                    }
                }
            };

            IsLeaves = true;
            IsCollision = false;
            AllDrawing = true;
            IsGrass = true;
            LightingYourself = true;
        }
    }
}

using VoxelEngine.Glm;
using VoxelEngine.Model;
using VoxelEngine.Util;

namespace VoxelEngine.World
{
    public class BlockLeavesApple : Block
    {
        /// <summary>
        /// Блок листвы с яблоком
        /// </summary>
        public BlockLeavesApple()
        {
            Boxes = new Box[]
            {
                new Box(34, true),
                new Box()
                {
                    From = new vec3(0, 0, .5f),
                    To = new vec3(1f, 1f, .5f),
                    Faces = new Face[]
                    {
                        new Face(Pole.North, 36),
                        new Face(Pole.South, 36),
                    }
                },
                new Box()
                {
                    From = new vec3(.5f, 0, 0),
                    To = new vec3(.5f, 1f, 1f),
                    Faces = new Face[]
                    {
                        new Face(Pole.East, 36),
                        new Face(Pole.West, 36)
                    }
                }
            };

            Color = new vec4(0.56f, 0.73f, 0.35f, 1f);
            AllDrawing = true;
            IsLeaves = true;
        }
    }
}

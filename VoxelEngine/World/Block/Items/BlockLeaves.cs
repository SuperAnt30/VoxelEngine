using VoxelEngine.Glm;
using VoxelEngine.Model;

namespace VoxelEngine.World
{
    public class BlockLeaves : Block
    {
        /// <summary>
        /// Блок листвы
        /// </summary>
        public BlockLeaves()
        {
            Boxes = new Box[] { new Box(34, true) };

            AllDrawing = true;
            IsLeaves = true;
        }
    }
}

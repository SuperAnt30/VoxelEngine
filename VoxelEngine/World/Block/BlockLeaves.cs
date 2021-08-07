using VoxelEngine.Glm;
using VoxelEngine.Model;

namespace VoxelEngine
{
    public class BlockLeaves : Block
    {
        /// <summary>
        /// Блок листвы
        /// </summary>
        public BlockLeaves()
        {
            Boxes = new Box[] { new Box(34, true) };

            Color = new vec4(0.56f, 0.73f, 0.35f, 1f);
            AllDrawing = true;
        }
    }
}

using VoxelEngine.Glm;

namespace VoxelEngine
{
    public delegate void VoxelEventHandler(object sender, VoxelEventArgs e);

    /// <summary>
    /// Объект для события рендера
    /// </summary>
    public class VoxelEventArgs
    {
        /// <summary>
        /// Объект для события рендера
        /// </summary>
        public VoxelEventArgs(vec3i position, vec2i[] beside)
        {
            Position = position;
            Beside = beside;
        }

        /// <summary>
        /// Объект прорисовки чанка
        /// </summary>
        public vec3i Position { get; protected set; }
        /// <summary>
        /// Соседний чанк
        /// </summary>
        public vec2i[] Beside { get; protected set; }
    }
}

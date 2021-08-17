using VoxelEngine.Glm;

namespace VoxelEngine.Util
{
    public delegate void CoordEventHandler(object sender, CoordEventArgs e);

    /// <summary>
    /// Объект для события рендера
    /// </summary>
    public class CoordEventArgs
    {
        public CoordEventArgs(vec3i position)
        {
            Position3d = position;
        }

        public CoordEventArgs(vec2i position)
        {
            Position2d = position;
        }

        /// <summary>
        /// Координаты 2д
        /// </summary>
        public vec2i Position2d { get; protected set; }
        /// <summary>
        /// Координаты 3д
        /// </summary>
        public vec3i Position3d { get; protected set; }
    }
}

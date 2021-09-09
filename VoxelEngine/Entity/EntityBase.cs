using VoxelEngine.Glm;

namespace VoxelEngine.Entity
{
    /// <summary>
    /// Базовый объект сущности
    /// </summary>
    public class EntityBase
    {
        /// <summary>
        /// Позиция моба
        /// </summary>
        public vec3 Position { get; protected set; }

        /// <summary>
        /// Поворот
        /// </summary>
        public float Yaw { get; protected set; }

        public EntityBase(vec3 pos, float yaw)
        {
            Position = pos;
            Yaw = yaw;
        }


        public void Tick(long tick)
        {

        }

    }
}

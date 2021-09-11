using VoxelEngine.Glm;

namespace VoxelEngine.Entity
{
    /// <summary>
    /// Базовый объект сущности
    /// </summary>
    public class EntityBase
    {
        /// <summary>
        /// Порядковый номер сущьности
        /// </summary>
        public int Index { get; protected set; }
        /// <summary>
        /// Тип конкретного моба, для текстуры и тп
        /// </summary>
        public EnumEntity Key { get; protected set; }

        /// <summary>
        /// Позиция Сущности
        /// </summary>
        public vec3 Position { get; protected set; }
        /// <summary>
        /// Сущность на предыдущем тике, используемая для вычисления позиции во время процедур рендеринга
        /// </summary>
        public vec3 LastTickPos { get; protected set; }

        /// <summary>
        /// Сколько тиков прошло у этой сущности с тех пор, как она была жива
        /// </summary>
        public int TicksExisted { get; protected set; } = 0;

        /// <summary>
        /// Движение
        /// </summary>
        public bool IsMove { get; protected set; } = false;

        /// <summary>
        /// Поворот
        /// </summary>
        public float Yaw { get; protected set; }

        public EntityBase(int index, vec3 pos, float yaw)
        {
            Index = index;
            Position = pos;
            Yaw = yaw;
        }

        public virtual void Tick(long tick)
        {
            
        }

    }
}

namespace VoxelEngine.Entity
{
    /// <summary>
    /// Объект скоростей сущьности
    /// </summary>
    public class EntitySpeed
    {
        /// <summary>
        /// Скорость шага
        /// </summary>
        public float Step { get; protected set; } = 8.6f;
        /// <summary>
        /// Скорость движения сидя
        /// </summary>
        public float Sneaking { get; protected set; } = 2.6f;
        /// <summary>
        /// Скорость бега
        /// </summary>
        public float Sprinting { get; protected set; } = 11.2f;
        /// <summary>
        /// Скорость прыжка
        /// 20 где-то выше 2 блоков но ниже 3
        /// </summary>
        public float Jamp { get; protected set; } = 20f;
        /// <summary>
        /// Скорость плавание под водой вверх/вниз
        /// </summary>
        public float Swim { get; protected set; } = 2f;

        /// <summary>
        /// Скорость полёта
        /// </summary>
        public float Fly { get; protected set; } = 22f;
        /// <summary>
        /// Скорость быстрого полёта
        /// </summary>
        public float FlyFast { get; protected set; } = 44f;
        /// <summary>
        /// Скорость полёта вертикали
        /// </summary>
        public float FlyVertical { get; protected set; } = 15f;

        public void Set(float step)
        {
            Step = step;
        }

    }
}

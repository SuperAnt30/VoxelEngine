namespace VoxelEngine.Entity
{
    /// <summary>
    /// Ключевой объект перемещения движения
    /// </summary>
    public class MovingKey
    {
        /// <summary>
        /// Перемещение лево право
        /// </summary>
        public EnumMovingKey Horizontal { get; set; } = EnumMovingKey.None;
        /// <summary>
        /// Перемещение вперёд назад
        /// </summary>
        public EnumMovingKey Vertical { get; set; } = EnumMovingKey.None;
        /// <summary>
        /// Перемещение вверх или вниз, прыжок или присел
        /// </summary>
        public EnumMovingKey Height { get; set; } = EnumMovingKey.None;

        public float GetHorizontalValue()
        {
            return (float)Horizontal;
        }
        public float GetVerticalValue()
        {
            return (float)Vertical;
        }
        public float GetHeightValue()
        {
            return (float)Height;
        }
    }
}

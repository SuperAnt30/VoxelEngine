namespace VoxelEngine.Entity
{
    /// <summary>
    /// Нажатие клавиши или направление движения
    /// </summary>
    public enum EnumMovingKey
    {
        /// <summary>
        /// Значение отрицательное, идём назад
        /// </summary>
        Minus = -1,
        /// <summary>
        /// Нет действия
        /// </summary>
        None = 0,
        /// <summary>
        /// Значение положительное, идём вперёд
        /// </summary>
        Plus = 1
    }
}

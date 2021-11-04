namespace VoxelEngine.Gen
{
    /// <summary>
    /// Статус генерации чанка
    /// </summary>
    public enum EnumGeterationStatus
    {
        /// <summary>
        /// Генерация только в одном чанке, без связи с соседними чанками
        /// </summary>
        Chunk = 1,
        /// <summary>
        /// Генерация с соседними чанками 3*3, для генерации деревьев и объектов выходящих за чанк
        /// </summary>
        Area = 2,
        /// <summary>
        /// Генерация с соседними чанками 3*3 * ещё соседние 3*3 итого блок 5*5,
        /// для генерации освещения в тикущем чанке
        /// </summary>
        DoubleArea = 3,
        /// <summary>
        /// Готов для рендера
        /// </summary>
        Ready = 4
    }
}

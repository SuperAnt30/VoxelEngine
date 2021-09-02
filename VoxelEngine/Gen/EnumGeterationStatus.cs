namespace VoxelEngine.Gen
{
    /// <summary>
    /// Статус генерации чанка
    /// </summary>
    public enum EnumGeterationStatus
    {
        /// <summary>
        /// Нет генерации в чанке
        /// </summary>
        None = 0,
        /// <summary>
        /// Генерация только в одном чанке, без связи с соседними чанками
        /// </summary>
        Chunk = 1,
        /// <summary>
        /// Генерация с соседними чанками, деревья освещение
        /// </summary>
        Area = 2
    }
}

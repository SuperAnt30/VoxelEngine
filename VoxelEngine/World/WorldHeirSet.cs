namespace VoxelEngine.World
{
    public class WorldHeirSet
    {
        /// <summary>
        /// Объект кэш чанка
        /// </summary>
        public WorldBase World { get; protected set; }

        /// <summary>
        /// Задать объект мира
        /// </summary>
        public void SetWorld(WorldBase world)
        {
            World = world;
        }
    }
}

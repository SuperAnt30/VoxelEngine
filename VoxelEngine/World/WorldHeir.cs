namespace VoxelEngine.World
{
    public class WorldHeir
    {
        /// <summary>
        /// Объект кэш чанка
        /// </summary>
        public WorldBase World { get; protected set; }

        protected WorldHeir() { }

        public WorldHeir(WorldBase world)
        {
            World = world;
        }
    }
}

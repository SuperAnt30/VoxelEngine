namespace VoxelEngine.World
{
    public class WorldHeir
    {
        /// <summary>
        /// Объект базового мира
        /// </summary>
        public WorldBase World { get; protected set; }

        protected WorldHeir() { }

        public WorldHeir(WorldBase world)
        {
            World = world;
        }
    }
}

namespace VoxelEngine.Entity
{
    public delegate void EntityEventHandler(object sender, EntityEventArgs e);

    /// <summary>
    /// Объект для события сущьности
    /// </summary>
    public class EntityEventArgs
    {
        public EntityEventArgs(EntityBase entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Сущьность
        /// </summary>
        public EntityBase Entity { get; protected set; }
    }
}

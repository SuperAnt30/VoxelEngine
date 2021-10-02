namespace VoxelEngine.Entity
{
    public class EntityDistance
    {
        public EntityLiving Entity { get; protected set; }

        public float Distance { get; protected set; } = 100;

        public bool IsEmpty { get; protected set; } = true;

        public EntityDistance() { }

        public EntityDistance(EntityLiving entity, float dis)
        {
            Entity = entity;
            Distance = dis;
            IsEmpty = false;
        }
    }
}

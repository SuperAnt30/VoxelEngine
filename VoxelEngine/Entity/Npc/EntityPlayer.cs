using VoxelEngine.World;

namespace VoxelEngine.Entity.Npc
{
    /// <summary>
    /// Сущность игрока выживание
    /// </summary>
    public class EntityPlayer : EntityLiving //EntityFly //EntityLiving
    {
        public EntityPlayer(WorldBase world) : base(world)
        {
            InitializePlayer();
        }
    }
}

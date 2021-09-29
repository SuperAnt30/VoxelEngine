using VoxelEngine.World;
using VoxelEngine.World.Blk;

namespace VoxelEngine.Entity.Npc
{
    /// <summary>
    /// Сущность игрока выживание
    /// </summary>
    public class EntityPlayer : EntityLiving
    {
        public EntityPlayer(WorldBase world) : base(world)
        {
            InitializePlayer();
        }

    }
}

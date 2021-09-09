using VoxelEngine.Glm;

namespace VoxelEngine.Entity
{
    /// <summary>
    /// Сущность курочка
    /// </summary>
    public class EntityChicken: EntityBase
    {

        public EntityChicken(vec3 pos, float yaw) : base(pos, yaw) { }

        //this.timeUntilNextEgg = this.rand.nextInt(6000) + 6000;
        //this.tasks.addTask(0, new EntityAISwimming(this)); // Плавание
        //this.tasks.addTask(1, new EntityAIPanic(this, 1.4D)); // Паника
        //this.tasks.addTask(2, new EntityAIMate(this, 1.0D)); // Приятель, брачный период
        //this.tasks.addTask(3, new EntityAITempt(this, 1.0D, Items.wheat_seeds, false)); // Соблазнять
        //this.tasks.addTask(4, new EntityAIFollowParent(this, 1.1D)); // Следуйте за родителем
        //this.tasks.addTask(5, new EntityAIWander(this, 1.0D)); // Бродить
        //this.tasks.addTask(6, new EntityAIWatchClosest(this, EntityPlayer.class, 6.0F)); // Смотреть ближайшие
        //this.tasks.addTask(7, new EntityAILookIdle(this)); // Смотреть без дела

    }
}

using System;
using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World;

namespace VoxelEngine.Entity.Npc
{
    /// <summary>
    /// Сущность курочка
    /// </summary>
    public class EntityChicken: EntityLiving
    {
        public EntityChicken(WorldBase world) : base(world) { }

        /// <summary>
        /// TODO:: временно
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pos"></param>
        /// <param name="yaw"></param>
        public void SetChicken(int index, vec3 pos, float yaw)
        {
            RotationYaw = yaw;

            HitBox = new HitBoxEntity(index, World);
            HitBox.HitBoxChanged += HitBox_Changed;
            HitBox.Size.SetSize(.4f, 0.7f);
            HitBox.SetPos(pos);
        }

        





        //this.timeUntilNextEgg = this.rand.nextInt(6000) + 6000;
        //this.tasks.addTask(0, new EntityAISwimming(this)); // Плавание
        //this.tasks.addTask(1, new EntityAIPanic(this, 1.4D)); // Паника
        //this.tasks.addTask(2, new EntityAIMate(this, 1.0D)); // Приятель, брачный период
        //this.tasks.addTask(3, new EntityAITempt(this, 1.0D, Items.wheat_seeds, false)); // Соблазнять
        //this.tasks.addTask(4, new EntityAIFollowParent(this, 1.1D)); // Следуйте за родителем
        //this.tasks.addTask(5, new EntityAIWander(this, 1.0D)); // Бродить
        //this.tasks.addTask(6, new EntityAIWatchClosest(this, EntityPlayer.class, 6.0F)); // Смотреть ближайшие
        //this.tasks.addTask(7, new EntityAILookIdle(this)); // Смотреть без дела

        public override void UpdateTick(long tick)
        {
            base.UpdateTick(tick);
            int r = random.Next(10);
            IsMove = false;
            if (r == 1)
            {
                // вращаемся
                RotationYaw += (float)random.NextDouble() - .5f;
            }
            else if (r > 5)
            {
                vec3 v = new vec3(0, 0, -1f);
                v = v.rotateYaw(RotationYaw).normalize();
                Moving.Vertical = EnumMovingKey.Plus;
                //HitBox.SetPos(HitBox.Position + v * .1f);
                //HitBox.RefrashDrawHitBox();
                IsMove = true;
            }
        }
    }
}

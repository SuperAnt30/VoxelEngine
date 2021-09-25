using VoxelEngine.Glm;
using VoxelEngine.Graphics;
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
            Key = EnumEntity.Chicken;
            speed.Set(2f);

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


        int pause = 0;
        int pauseJamp = 0;
        public override void UpdateTick(long tick)
        {
            base.UpdateTick(tick);
            int r = random.Next(10);
            pause--;
            pauseJamp--;

            int mov = 1;

            // Блок плавания {
            if (HitBox.IsLegsWater || HitBox.IsDownWater)
            {
                Render();
                mov = 3;
            }
            if (pauseJamp <= 0 && HitBox.IsEyesWater)
            {
                Jump();
                pauseJamp = 7;
                mov = 5;
            }
            else if (IsJumping)
            {
                JumpCancel();
            }
            // Блок плавания }

            if (r == 0)
            {
                // вращаемся
                RotationYaw += ((float)random.NextDouble() - .5f);
                Render();
            }

            // Блок перемещения {
            if (pause <= 0)
            {
                if (r != 0 && r <= mov)
                {
                    Moving.Forward();
                    Render();
                    pause = random.Next(80) + 40;
                }
                else
                {
                    if (!Moving.IsStand())
                    {
                        Render();
                        Moving.VerticalCancel();
                        pause = random.Next(160) + 40;
                    }
                }
            }
            // Блок перемещения }
        }
    }
}

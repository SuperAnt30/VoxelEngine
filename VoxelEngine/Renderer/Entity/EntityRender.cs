using VoxelEngine.Entity;
using VoxelEngine.Glm;

namespace VoxelEngine.Renderer.Entity
{

    public class EntityRender
    {
        /// <summary>
        /// Объект мира рендера
        /// </summary>
        public WorldRender World { get; protected set; }
        /// <summary>
        /// Объект сущьности
        /// </summary>
        public EntityLiving Entity { get; protected set; }

        public EntityRender(WorldRender world, EntityLiving entity)
        {
            World = world;
            Entity = entity;
        }

        //public void getMouseOver(float p_78473_1_)
        //{

        //}

        /// <summary>
        /// Обновление каждый кадр (FPS)
        /// </summary>
        public string UpdateDraw(float timeFrame, float timeAll)
        {
            Entity.Moving.Update(timeAll);
            string str = Entity.UpdateMoving();

            if (Entity.Mode == VEMoving.Survival)
            {
                vec3 move = Entity.MoveTime(timeFrame);
                if (Entity.HitBox.CollisionBodyXZ(move) && Entity.OnGround)
                {
                    // Авто прыжок
                    Entity.SetMotionY(Entity.HitBox.IsLegsWater ? VE.SPEED_WATER_AUTOJAMP : VE.SPEED_AUTOJAMP);
                    vec3 move2 = Entity.MoveTime(timeFrame);
                    move.y = move2.y;
                }
                Entity.CollisionBodyY(move);
            }
            else if (Entity.Mode == VEMoving.ObstacleFlight)
            {
                vec3 move = Entity.MoveTime(timeFrame);
                Entity.HitBox.CollisionBodyXZ(move);
                Entity.CollisionBodyY(move);
            }
            else
            {
                Entity.HitBox.SetPos(Entity.HitBox.Position + Entity.MoveTime(timeFrame));
            }

            return str;
        }

        //#region Event

        //protected void HitBox_Done(object sender, BufferEventArgs e)
        //{
        //    OnHitBoxDone(e);
        //}

        ///// <summary>
        ///// Событие чанк сделано
        ///// </summary>
        //public event BufferEventHandler HitBoxDone;
        ///// <summary>
        ///// Событие чанк сделано
        ///// </summary>
        //protected void OnHitBoxDone(BufferEventArgs e)
        //{
        //    HitBoxDone?.Invoke(this, e);
        //}

        //#endregion
    }
}

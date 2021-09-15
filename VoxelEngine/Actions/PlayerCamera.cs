using System.Diagnostics;
using VoxelEngine.Entity;
using VoxelEngine.Graphics;

namespace VoxelEngine.Actions
{
    /// <summary>
    /// Управление камерой
    /// </summary>
    public class PlayerCamera
    {
        /// <summary>
        /// Объект сущьности игрока
        /// </summary>
        public EntityLiving Entity { get; set; }

        /// <summary>
        /// Объект для точного замера времени
        /// </summary>
        //protected Stopwatch stopwatch = new Stopwatch();
        protected Camera cam;

        /// <summary>
        /// Строка для дебага
        /// </summary>
        public string StrDebug { get; protected set; } = "";

        public PlayerCamera()
        {
            //stopwatch.Start();
            cam = OpenGLF.GetInstance().Cam;
        }

        #region KeyDown

        /// <summary>
        /// Включить или выключить ускорение
        /// </summary>
        public void Sprinting()
        {
            if (!Entity.IsSneaking && !Entity.IsSprinting 
                && Entity.Moving.Vertical == EnumMovingKey.Plus)
            {
                Entity.Sprinting();
                cam.SetFov(Glm.glm.radians(80));
            }
            // TODO:: увеличиваем угол обзора от бега на 10-15%
            // Делаем плавно как старт бега
            //cam.SetFov(Glm.glm.radians(Entity.IsSprinting ? 80 : 70));
        }

        /// <summary>
        /// Шаг влево
        /// </summary>
        public void StepLeft()
        {
            if (Entity.Moving.Horizontal != EnumMovingKey.Minus)
            {
                Entity.Moving.Horizontal = EnumMovingKey.Minus;
                StrDebug = Entity.UpdateMoving();
            }
        }
        /// <summary>
        /// Шаг вправо
        /// </summary>
        public void StepRight()
        {
            if (Entity.Moving.Horizontal != EnumMovingKey.Plus)
            {
                Entity.Moving.Horizontal = EnumMovingKey.Plus;
                StrDebug = Entity.UpdateMoving();
            }
        }
        /// <summary>
        /// Шаг вперёд
        /// </summary>
        public void StepForward()
        {
            if (Entity.Moving.Vertical != EnumMovingKey.Plus)
            {
                Entity.Moving.Vertical = EnumMovingKey.Plus;
                StrDebug = Entity.UpdateMoving();
                
            }
        }
        /// <summary>
        /// Шаг назад
        /// </summary>
        public void StepBack()
        {
            if (Entity.Moving.Vertical != EnumMovingKey.Minus)
            {
                Entity.Moving.Vertical = EnumMovingKey.Minus;
                StrDebug = Entity.UpdateMoving();
            }
        }

        /// <summary>
        /// Перемещение вверх или прыжок
        /// </summary>
        public void Jamp()
        {
            if (Entity.Moving.Height != EnumMovingKey.Plus)
            {
                Entity.Moving.Height = EnumMovingKey.Plus;
                StrDebug = Entity.UpdateMoving();
            }
        }

        /// <summary>
        /// Перемещение вниз или присесть
        /// </summary>
        public void Down()
        {
            if (Entity.Moving.Height != EnumMovingKey.Minus)
            {
                Entity.Moving.Height = EnumMovingKey.Minus;
                StrDebug = Entity.UpdateMoving();
                if (Entity.IsSneaking)
                {
                    Entity.HitBox.Sneaking();
                }
            }
        }
        #endregion

        #region KeyUp

        public void KeyUpSprinting()
        {
            // ускорение сбрасывается
            Entity.SprintingNot();
            // Делаем плавно как старт бега
            cam.SetFov(Glm.glm.radians(70));
        }
        /// <summary>
        /// Отпускаем клавишу A || D
        /// </summary>
        public void KeyUpHorizontal()
        {
            if (Entity.Moving.Horizontal != EnumMovingKey.None)
            {
                Entity.Moving.Horizontal = EnumMovingKey.None;
                StrDebug = Entity.UpdateMoving();
            }
        }
        /// <summary>
        /// Отпускаем клавишу W || S
        /// </summary>
        public void KeyUpVertical()
        {
            if (Entity.Moving.Vertical != EnumMovingKey.None)
            {
                Entity.Moving.Vertical = EnumMovingKey.None;
                StrDebug = Entity.UpdateMoving();
            }
        }
        /// <summary>
        /// Отпускаем клавишу прыгать / вверх
        /// </summary>
        public void KeyUpJamp()
        {
            if (Entity.Moving.Height != EnumMovingKey.None)
            {
                Entity.Moving.Height = EnumMovingKey.None;
                StrDebug = Entity.UpdateMoving();
            }
        }

        /// <summary>
        /// Опускаем клавишу сидеть / вниз
        /// </summary>
        public void KeyUpSneaking()
        {
            if (Entity.Moving.Height != EnumMovingKey.None)
            {
                Entity.Moving.Height = EnumMovingKey.None;
                StrDebug = Entity.UpdateMoving();
            }

            if (VEC.GetInstance().Moving == VEMoving.Survival)
            {
                // Встать
                // Проверка на возможность встать
                //HitBoxEntity hitBox = OpenGLF.GetInstance().Cam.HitBox;
                Entity.SneakingNearly();
                //_isSneaking = EnumSneaking.GetUp;
                if (!Entity.HitBox.IsCollisionUp())
                {
                    // Встаём
                    Uping();
                }
            }
        }

        #endregion
        
        /// <summary>
        /// Начинаем вставать
        /// </summary>
        protected void Uping()
        {
            Entity.Uping();
        }
        /// <summary>
        /// Изменения или точнее перемещение камеры
        /// Находится в кадрах прорисовки (FPS)
        /// </summary>
        public void Update(float time)
        {
            //float time = stopwatch.ElapsedMilliseconds / 1000f;
            //stopwatch.Restart();
            //if (time > 1.5f) time = 1.5f;

            StrDebug = Entity.UpdateDraw(time);
            Entity.HitBox.Size.Update();
            cam.SetPos(Entity.HitBox.Position);
            
        }

        /// <summary>
        /// Добавляем в TPS, чтоб корректировать прыжки
        /// </summary>
        public void Tick()
        {
            Entity.UpdateTick(0);
            
        }
    }
}

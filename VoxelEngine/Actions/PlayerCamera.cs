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
                && Entity.Moving.Vertical.Plus)
            {
                Entity.Sprinting();
                //cam.SetFov(Glm.glm.radians(80));
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
            if (Entity.Moving.Left()) StrDebug = Entity.UpdateMoving();
        }
        /// <summary>
        /// Шаг вправо
        /// </summary>
        public void StepRight()
        {
            if (Entity.Moving.Right()) StrDebug = Entity.UpdateMoving();
        }
        /// <summary>
        /// Шаг вперёд
        /// </summary>
        public void StepForward()
        {
            if (Entity.Moving.Forward()) StrDebug = Entity.UpdateMoving();
        }
        /// <summary>
        /// Шаг назад
        /// </summary>
        public void StepBack()
        {
            if (Entity.Moving.Back()) StrDebug = Entity.UpdateMoving();
        }

        /// <summary>
        /// Перемещение вверх или прыжок
        /// </summary>
        public void Jamp()
        {
            if (Entity.Moving.Up()) StrDebug = Entity.UpdateMoving();
        }

        /// <summary>
        /// Перемещение вниз или присесть
        /// </summary>
        public void Down()
        {
            if (Entity.Moving.Down())
            {
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
            //cam.SetFov(Glm.glm.radians(70));
        }
        /// <summary>
        /// Отпускаем клавишу A || D
        /// </summary>
        public void KeyUpHorizontal()
        {
            if (Entity.Moving.HorizontalCancel()) StrDebug = Entity.UpdateMoving();
        }
        /// <summary>
        /// Отпускаем клавишу W || S
        /// </summary>
        public void KeyUpVertical()
        {
            if (Entity.Moving.VerticalCancel()) StrDebug = Entity.UpdateMoving();
        }
        /// <summary>
        /// Отпускаем клавишу прыгать / вверх
        /// </summary>
        public void KeyUpJamp()
        {
            if (Entity.Moving.HeightCancel()) StrDebug = Entity.UpdateMoving();
        }

        /// <summary>
        /// Опускаем клавишу сидеть / вниз
        /// </summary>
        public void KeyUpSneaking()
        {
            if (Entity.Moving.HeightCancel()) StrDebug = Entity.UpdateMoving();
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
        public void Update(float timeFrame, float timeAll)
        {
            StrDebug = Entity.UpdateDraw(timeFrame, timeAll);
            Entity.HitBox.Size.Update(timeAll);
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

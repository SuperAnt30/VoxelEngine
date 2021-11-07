using System.Diagnostics;
using VoxelEngine.Entity;
using VoxelEngine.Graphics;
using VoxelEngine.Renderer.Entity;

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
        public EntityRender EntityR { get; set; }

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
            if (!EntityR.Entity.IsSneaking && !EntityR.Entity.IsSprinting 
                && EntityR.Entity.Moving.Vertical.Plus)
            {
                EntityR.Entity.Sprinting();
            }
        }

        /// <summary>
        /// Шаг влево
        /// </summary>
        public void StepLeft()
        {
            if (EntityR.Entity.Moving.Left()) StrDebug = EntityR.Entity.UpdateMoving();
        }
        /// <summary>
        /// Шаг вправо
        /// </summary>
        public void StepRight()
        {
            if (EntityR.Entity.Moving.Right()) StrDebug = EntityR.Entity.UpdateMoving();
        }
        /// <summary>
        /// Шаг вперёд
        /// </summary>
        public void StepForward()
        {
            if (EntityR.Entity.Moving.Forward()) StrDebug = EntityR.Entity.UpdateMoving();
        }
        /// <summary>
        /// Шаг назад
        /// </summary>
        public void StepBack()
        {
            if (EntityR.Entity.Moving.Back()) StrDebug = EntityR.Entity.UpdateMoving();
        }

        /// <summary>
        /// Перемещение вверх или прыжок
        /// </summary>
        public void Jamp()
        {
            if (EntityR.Entity.Moving.Up()) StrDebug = EntityR.Entity.UpdateMoving();
        }

        /// <summary>
        /// Перемещение вниз или присесть
        /// </summary>
        public void Down()
        {
            if (EntityR.Entity.Moving.Down())
            {
                StrDebug = EntityR.Entity.UpdateMoving();
                if (EntityR.Entity.IsSneaking)
                {
                    EntityR.Entity.HitBox.Sneaking();
                }
            }
        }
        #endregion

        #region KeyUp

        public void KeyUpSprinting()
        {
            // ускорение сбрасывается
            EntityR.Entity.SprintingNot();
            // Делаем плавно как старт бега
            //cam.SetFov(Glm.glm.radians(70));
        }
        /// <summary>
        /// Отпускаем клавишу A || D
        /// </summary>
        public void KeyUpHorizontal()
        {
            if (EntityR.Entity.Moving.HorizontalCancel()) StrDebug = EntityR.Entity.UpdateMoving();
        }
        /// <summary>
        /// Отпускаем клавишу W || S
        /// </summary>
        public void KeyUpVertical()
        {
            if (EntityR.Entity.Moving.VerticalCancel()) StrDebug = EntityR.Entity.UpdateMoving();
        }
        /// <summary>
        /// Отпускаем клавишу прыгать / вверх
        /// </summary>
        public void KeyUpJamp()
        {
            if (EntityR.Entity.Moving.HeightCancel()) StrDebug = EntityR.Entity.UpdateMoving();
        }

        /// <summary>
        /// Опускаем клавишу сидеть / вниз
        /// </summary>
        public void KeyUpSneaking()
        {
            if (EntityR.Entity.Moving.HeightCancel()) StrDebug = EntityR.Entity.UpdateMoving();
            if (VEC.moving == VEMoving.Survival)
            {
                // Встать
                // Проверка на возможность встать
                //HitBoxEntity hitBox = OpenGLF.GetInstance().Cam.HitBox;
                EntityR.Entity.SneakingNearly();
                //_isSneaking = EnumSneaking.GetUp;
                if (!EntityR.Entity.HitBox.IsCollisionUp())
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
            EntityR.Entity.Uping();
        }
        /// <summary>
        /// Изменения или точнее перемещение камеры
        /// Находится в кадрах прорисовки (FPS)
        /// </summary>
        public void Update(float timeFrame, float timeAll)
        {
            if (EntityR != null)
            {
                StrDebug = EntityR.UpdateDraw(timeFrame, timeAll);
                EntityR.Entity.HitBox.Size.Update(timeAll);
                cam.SetPos(EntityR.Entity.HitBox.Position);
            }
        }

        /// <summary>
        /// Добавляем в TPS, чтоб корректировать прыжки
        /// </summary>
        public void Tick()
        {
            EntityR.Entity.UpdateTick(0);
            
        }
    }
}

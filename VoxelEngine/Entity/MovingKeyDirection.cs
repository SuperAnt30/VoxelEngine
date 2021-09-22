using VoxelEngine.Util;

namespace VoxelEngine.Entity
{
    /// <summary>
    /// Ключевой объект перемещения движения конкретного направления
    /// </summary>
    public class MovingKeyDirection : AnimationHeir
    {
        /// <summary>
        /// Перемещение 0..1
        /// </summary>
        public float Moving { get; protected set; }

        protected bool animationEnd = false;

        protected void SetMoving(float m)
        {
            Moving = m;
            OnLookAtChanged();
        }

        /// <summary>
        /// Задать количество такт шагов от 0 до 1
        /// </summary>
        public void CountStep(float step)
        {
            this.step = step;
        }

        /// <summary>
        /// Значение является положительным
        /// </summary>
        public bool Plus { get { return Moving > 0; } }

        /// <summary>
        /// Значение является отрицательным
        /// </summary>
        public bool Minus { get { return Moving < 0; } }

        /// <summary>
        /// Значение является положительным и активно
        /// </summary>
        public bool PlusAction { get { return Moving > 0 && !animationEnd; } }
        /// <summary>
        /// Значение является отрицательным и активно
        /// </summary>
        public bool MinusAction { get { return Moving < 0 && !animationEnd; } }

        /// <summary>
        /// Плюс
        /// </summary>
        public bool MovingPlus()
        {
            if (Moving <= 0 || animationEnd)
            {
                timeBegin = time;
                SetMoving(.01f);
                animation = true;
                animationEnd = false;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Минус
        /// </summary>
        public bool MovingMinus()
        {
            if (Moving >= 0 || animationEnd)
            {
                timeBegin = time;
                SetMoving(-.01f);
                animation = true;
                animationEnd = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Ноль
        /// </summary>
        public bool Zero()
        {
            if (!IsZero)
            {
                timeBegin = time;
                if (Moving > 0) SetMoving(1f);
                if (Moving < 0) SetMoving(-1f);
                animation = false;
                animationEnd = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Стоим ли мы
        /// </summary>
        public bool IsZero => Moving == 0;

        /// <summary>
        /// Находится в кадрах прорисовки (FPS)
        /// </summary>
        public override void Update(float time)
        {
            base.Update(time);

            if (animation)
            {
                if (Moving > 0)
                {
                    if (timeD >= step) SetMoving(1f);
                    else SetMoving(timeD / step);
                }
                else if (Moving < 0)
                {
                    if (timeD >= step) SetMoving(-1f);
                    else SetMoving(timeD / -step);
                }
                if (Moving >= 1f || Moving <= -1f) animation = false;
            }
            if (animationEnd)
            {
                if (Moving > 0)
                {
                    if (timeD >= step) SetMoving(0);
                    else SetMoving(1f - timeD / step);
                }
                else if (Moving < 0)
                {
                    if (timeD >= step) SetMoving(0);
                    else SetMoving(-(1f - timeD / step));
                }
                if (Moving == 0) animationEnd = false;
            }
        }
    }
}

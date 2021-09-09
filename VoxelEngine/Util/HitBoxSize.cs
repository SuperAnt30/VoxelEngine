using System;

namespace VoxelEngine.Util
{
    /// <summary>
    /// Объект для размера хитбокса
    /// и анимации сесть, встать
    /// </summary>
    public class HitBoxSize
    {
        /// <summary>
        /// Пол ширины
        /// </summary>
        public float Width { get; protected set; } = 0;
        /// <summary>
        /// Высота
        /// </summary>
        public float Heigth { get; protected set; } = 0;
        /// <summary>
        /// Смещение глаз
        /// </summary>
        public float Eyes { get; protected set; } = 0;
        /// <summary>
        /// Начальное смещение глаз
        /// </summary>
        public float EyesBegin { get; protected set; } = 0;
        /// <summary>
        /// Конечное смещение глаз
        /// </summary>
        public float EyesEnd { get; protected set; } = 0;
        /// <summary>
        /// Начальный тик
        /// </summary>
        protected long beginTick = 0;
        protected int time = 0;
        protected bool animation = false;

        public void SetSize(float w, float h)
        {
            Width = w;
            Heigth = h;
        }

        /// <summary>
        /// Задать смещение глаз
        /// </summary>
        /// <param name="eyes"></param>
        /// <param name="time">1 = 10 милли секунд</param>
        public void SetEyes(float eyes, int time)
        {
            EyesBegin = EyesEnd;
            EyesEnd = eyes;
            beginTick = DateTime.Now.Ticks;
            this.time = time * 100000;
            animation = true;
        }

        public void Tick()
        {
            if (animation)
            {
                long tick = DateTime.Now.Ticks;
                
                float t = tick - beginTick;
                if (t > time)
                {
                    Eyes = EyesEnd;
                    animation = false;
                }
                else
                {
                    // дельта смещения
                    float e = EyesEnd - EyesBegin;
                    Eyes = EyesBegin + (t * e / (float)time);
                }
                Debug.GetInstance().CountTest++;
                OnLookAtChanged();
            }
        }

        /// <summary>
        /// Событие изменена позиция камеры
        /// </summary>
        public event EventHandler LookAtChanged;

        /// <summary>
        /// Изменена позиция камеры
        /// </summary>
        protected void OnLookAtChanged()
        {
            LookAtChanged?.Invoke(this, new EventArgs());
        }
    }
}

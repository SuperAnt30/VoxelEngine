namespace VoxelEngine.Util
{
    /// <summary>
    /// Объект для размера хитбокса
    /// и анимации сесть, встать
    /// </summary>
    public class HitBoxSize : AnimationHeir
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
        protected float eyesBegin = 0;
        /// <summary>
        /// Конечное смещение глаз
        /// </summary>
        protected float eyesEnd = 0;

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
        public void SetEyes(float eyes)
        {
            eyesBegin = eyesEnd;
            eyesEnd = eyes;
            timeBegin = time;
            animation = true;
        }

        /// <summary>
        /// Находится в кадрах прорисовки (FPS)
        /// </summary>
        public override void Update(float time)
        {
            base.Update(time);

            if (animation)
            {
                if (timeD >= step)
                {
                    Eyes = eyesEnd;
                    animation = false;
                }
                else
                {
                    // дельта смещения
                    float e = eyesEnd - eyesBegin;
                    Eyes = eyesBegin + (e * (timeD / step));
                }
                OnLookAtChanged();
            }
        }

    }
}

using System;

namespace VoxelEngine.Util
{
    /// <summary>
    /// Объект наследник для мягкой анимации
    /// </summary>
    public class AnimationHeir
    {
        /// <summary>
        /// Запуск анимации
        /// </summary>
        protected bool animation = false;
        /// <summary>
        /// Количество секунд на действие, сек
        /// </summary>
        protected float step = 0.2f;
        /// <summary>
        /// Время с начала запуска проекта, сек
        /// </summary>
        protected float time;
        /// <summary>
        /// Время начало анимации
        /// </summary>
        protected float timeBegin;
        /// <summary>
        /// Время дельты
        /// </summary>
        protected float timeD;

        /// <summary>
        /// Находится в кадрах прорисовки (FPS)
        /// </summary>
        public virtual void Update(float time)
        {
            timeD = time - timeBegin;
            this.time = time;
        }

        #region event

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

        #endregion
    }
}

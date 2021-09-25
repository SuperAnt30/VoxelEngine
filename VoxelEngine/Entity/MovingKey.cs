using System;

namespace VoxelEngine.Entity
{
    /// <summary>
    /// Ключевой объект перемещения движения
    /// </summary>
    public class MovingKey
    {
        protected bool isPlayer = false;
        public MovingKey() { }
        public MovingKey(bool isPlayer)
        {
            this.isPlayer = isPlayer;
            if (isPlayer)
            {
                // Для игрока при изминении ускорения
                Sprinting.CountStep(.1f);
                Sprinting.LookAtChanged += Sprinting_LookAtChanged;
            }
        }

        public float GetHorizontalValue()
        {
            return Horizontal.Moving;
        }
        public float GetVerticalValue()
        {
            return Vertical.Moving;
        }
        public float GetHeightValue()
        {
            return Height.Moving;
        }
        public float GetSprintingValue()
        {
            return Sprinting.Moving;
        }

        /// <summary>
        /// Перемещение лево право
        /// </summary>
        public MovingKeyDirection Horizontal { get; protected set; } = new MovingKeyDirection();
        /// <summary>
        /// Перемещение вперёд назад
        /// </summary>
        public MovingKeyDirection Vertical { get; protected set; } = new MovingKeyDirection();
        /// <summary>
        /// Перемещение вверх или вниз, прыжок или присел
        /// </summary>
        public MovingKeyDirection Height { get; protected set; } = new MovingKeyDirection();
        /// <summary>
        /// Ускорение, только в одну сторону 
        /// </summary>
        public MovingKeyDirection Sprinting { get; protected set; } = new MovingKeyDirection();

        /// <summary>
        /// Вперёд
        /// </summary>
        public bool Forward() => Vertical.MovingPlus();
        /// <summary>
        /// Назад
        /// </summary>
        public bool Back() => Vertical.MovingMinus();
        /// <summary>
        /// Право
        /// </summary>
        public bool Right() => Horizontal.MovingPlus();
        /// <summary>
        /// Лево
        /// </summary>
        public bool Left() => Horizontal.MovingMinus();
        /// <summary>
        /// Вверх
        /// </summary>
        public bool Up() => Height.MovingPlus();
        /// <summary>
        /// Вниз
        /// </summary>
        public bool Down() => Height.MovingMinus();
        /// <summary>
        /// Начать ускорение
        /// </summary>
        public bool SprintingBegin() => Sprinting.MovingPlus();
        /// <summary>
        /// Вперёд и Назад отменено
        /// </summary>
        public bool VerticalCancel() => Vertical.Zero();
        /// <summary>
        /// Лево и право отменено
        /// </summary>
        public bool HorizontalCancel() => Horizontal.Zero();
        /// <summary>
        /// Вверх и Вниз отменено
        /// </summary>
        public bool HeightCancel() => Height.Zero();
        /// <summary>
        /// Отменить ускорение
        /// </summary>
        public bool SprintingCancel() => Sprinting.Zero();

        /// <summary>
        /// Стоим ли мы
        /// </summary>
        public bool IsStand() => Horizontal.IsZero && Vertical.IsZero && Height.IsZero;

        /// <summary>
        /// Остановиться
        /// </summary>
        public void Stand()
        {
            Horizontal.Zero();
            Vertical.Zero();
            Height.Zero();
            //if (isPlayer) Sprinting.Zero();
        }
        /// <summary>
        /// Находится в кадрах прорисовки (FPS)
        /// </summary>
        public void Update(float time)
        {
            Horizontal.Update(time);
            Vertical.Update(time);
            Height.Update(time);
            if (isPlayer) Sprinting.Update(time);
        }

        #region event

        private void Sprinting_LookAtChanged(object sender, System.EventArgs e)
        {
            OnLookAtChanged();
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

        #endregion
    }
}

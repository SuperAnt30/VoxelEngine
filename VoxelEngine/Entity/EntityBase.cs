using System;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;
using VoxelEngine.Renderer;
using VoxelEngine.Renderer.Blk;
using VoxelEngine.Renderer.Chk;
using VoxelEngine.Util;
using VoxelEngine.World;

namespace VoxelEngine.Entity
{
    /// <summary>
    /// Базовый объект сущности
    /// </summary>
    public class EntityBase : WorldHeir
    {
        /// <summary>
        /// Тип конкретного моба, для текстуры и тп
        /// </summary>
        public EnumEntity Key { get; protected set; }
        /// <summary>
        /// Получить хит бокс игрока
        /// </summary>
        public HitBoxEntity HitBox { get; protected set; }
        
        /// <summary>
        /// Поворот вокруг своей оси
        /// </summary>
        public float RotationYaw { get; protected set; }
        /// <summary>
        /// Поворот вверх вниз
        /// </summary>
        public float RotationPitch { get; protected set; }

        /// <summary>
        /// Убит ли
        /// </summary>
        public bool IsKill { get; protected set; } = false;

        /// <summary>
        /// Сущность на предыдущем тике, используемая для вычисления позиции во время процедур рендеринга
        /// </summary>
       // public vec3 LastTickPos { get; protected set; }

        /// <summary>
        /// Сколько тиков прошло у этой сущности с тех пор, как она была жива
        /// </summary>
       // public int TicksExisted { get; protected set; } = 0;

        /// <summary>
        /// Генератор случайных чисел
        /// </summary>
        public Random random = new Random();

        public EntityBase(WorldBase world) : base(world) { }

        /// <summary>
        /// Задать вращение
        /// </summary>
        public void SetRotation(float yaw, float pitch)
        {
            RotationYaw = yaw;
            RotationPitch = pitch;
        }

        /// <summary>
        /// Убить
        /// </summary>
        public void Kill() => IsKill = true;




        #region Event

        protected void HitBox_Changed(object sender, EventArgs e)
        {
            OnHitBoxChanged();
        }

        /// <summary>
        /// Событие изменён хитбокс
        /// </summary>
        public event EntityEventHandler HitBoxChanged;
        /// <summary>
        /// Событие изменён хитбокс
        /// </summary>
        protected void OnHitBoxChanged()
        {
            EntityBase entity = this;
            EntityEventArgs args = new EntityEventArgs(entity);
            HitBoxChanged?.Invoke(this, args);
        }

        protected void HitBox_LookAtChanged(object sender, EventArgs e)
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

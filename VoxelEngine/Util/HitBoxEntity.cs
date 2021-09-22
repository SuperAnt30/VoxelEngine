﻿using System;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;
using VoxelEngine.Renderer;
using VoxelEngine.World;

namespace VoxelEngine.Util
{ 
    /// <summary>
    /// Нитбокс камеры (игрока)
    /// </summary>
    public class HitBoxEntity : WorldHeir
    {
        /// <summary>
        /// Порядковый номер сущьности
        /// </summary>
        public int Index { get; protected set; }
        /// <summary>
        /// Размер
        /// </summary>
        public HitBoxSize Size { get; protected set; } = new HitBoxSize();
        /// <summary>
        /// Положение камеры
        /// </summary>
        public vec3 Position { get; protected set; }
        /// <summary>
        /// В каком чанке находится
        /// </summary>
        public vec2i ChunkPos { get; protected set; } = new vec2i();
        /// <summary>
        /// Позиция псевдо чанка
        /// </summary>
        public int ChunkY { get; protected set; }
        /// <summary>
        /// В каком блоке находится
        /// </summary>
        public vec3i BlockPos { get; protected set; } = new vec3i();
        /// <summary>
        /// В каком блоке находится глаза
        /// </summary>
        public vec3i BlockEyes { get; protected set; } = new vec3i();
        /// <summary>
        /// Направление течения если в воде
        /// </summary>
        public Pole Flow { get; set; } = Pole.Down;

        /// <summary>
        /// Находиться ли глаза игрока в воде
        /// </summary>
        public bool IsEyesWater { get; protected set; } = false;
        /// <summary>
        /// Находиться ли ноги игрок в воде
        /// </summary>
        public bool IsLegsWater { get; protected set; } = false;
        /// <summary>
        /// Находиться ли под ногами вода
        /// </summary>
        public bool IsDownWater { get; protected set; } = false;

        /// <summary>
        /// Размер стоя
        /// x = пол ширины y = высота z = высота глаз
        /// </summary>
        protected vec3 sizeWorth;
        /// <summary>
        /// Размер сидя
        /// x = пол ширины y = высота z = высота глаз
        /// </summary>
        protected vec3 sizeSneaking;

        /// <summary>
        /// Сетка хитбокса
        /// </summary>
        public float[] Buffer { get; protected set; } = new float[0];

        protected HitBoxEntity() { }

        public HitBoxEntity(int index, WorldBase world) : base(world)
        {
            Index = index;
            Size.LookAtChanged += Size_LookAtChanged;
        }

        public void SetPos(vec3 pos)
        {
            if (!Position.Equals(pos))
            {
                Position = pos;
                BlockPos = new vec3i(Position);
                BlockEyes = new vec3i(Position + new vec3(0, Size.Eyes, 0));
                ChunkPos = new vec2i((BlockPos.x) >> 4, (BlockPos.z) >> 4);
                ChunkY = (BlockPos.y) >> 4;

                // Глаза
                UpdateEyes();

                // Ноги
                IsLegsWater = World.GetBlock(BlockPos).IsWater;
                // ниже ног
                IsDownWater = World.GetBlock(BlockPos + new vec3i(0, -1, 0)).IsWater;

                RefrashDrawHitBox();
            }
        }

        /// <summary>
        /// Указываем размер хитбокса стоя и сидя
        /// </summary>
        /// <param name="worthHalfWidth">пол ширины стоя</param>
        /// <param name="worthHeight">высота стоя</param>
        /// <param name="worthEyes">глаза стоя</param>
        /// <param name="sneakingHalfWidth">пол ширины сидя</param>
        /// <param name="sneakingHeight">высота сидя</param>
        /// <param name="sneakingEyes">глаза сидя</param>
        public void SetSizeHitBox(float worthHalfWidth, float worthHeight, float worthEyes, 
            float sneakingHalfWidth, float sneakingHeight, float sneakingEyes)
        {
            sizeWorth = new vec3(worthHalfWidth, worthHeight, worthEyes);
            sizeSneaking = new vec3(sneakingHalfWidth, sneakingHeight, sneakingEyes);
        }

        /// <summary>
        /// Указываем размер хитбокса только стоя
        /// </summary>
        /// <param name="worthHalfWidth">пол ширины стоя</param>
        /// <param name="worthHeight">высота стоя</param>
        /// <param name="worthEyes">глаза стоя</param>
        public void SetSizeHitBox(float worthHalfWidth, float worthHeight, float worthEyes)
        {
            sizeWorth = new vec3(worthHalfWidth, worthHeight, worthEyes);
            sizeSneaking = new vec3();
        }

        /// <summary>
        /// Выбрать положение стоя
        /// </summary>
        public void Worth()
        {
            Size.SetSize(sizeWorth.x, sizeWorth.y);
            Size.SetEyes(sizeWorth.z);
            RefrashDrawHitBox();
        }

        /// <summary>
        /// Выбрать положение сидя
        /// </summary>
        public void Sneaking()
        {
            Size.SetSize(sizeSneaking.x, sizeSneaking.y);
            Size.SetEyes(sizeSneaking.z);
            RefrashDrawHitBox();
        }

        /// <summary>
        /// Обновить глаза
        /// </summary>
        protected bool UpdateEyes()
        {
            bool isEyesWater = World.GetBlock(BlockEyes).IsWater;
            if (IsEyesWater != isEyesWater)
            {
                IsEyesWater = isEyesWater;
                OnLookAtChanged();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Если застряли на блок вверх
        /// </summary>
        public void UpPos()
        {
            SetPos(new vec3(Position.x, Mth.Floor(Position.y + 1f), Position.z));
        }

        /// <summary>
        /// Изменить прорисовку хитбокса
        /// </summary>
        public void RefrashDrawHitBox()
        {
            if (Debug.GetInstance().IsDrawCollisium) BufferHitBox();
        }

        /// <summary>
        /// Проверяем коллизию тела c блоками по XZ
        /// </summary>
        /// <param name="vec">координата позиции</param>
        /// <returns>true - авто прыжок</returns>
        public bool CollisionBodyXZ(vec3 vec)
        {
            EnumCollisionBody cxz = _IsCollisionBody(new vec3(vec.x, 0, vec.z));
            if (cxz == EnumCollisionBody.None)
            {
                // Если в коллизии нет проблемы смещения
                SetPos(new vec3(Position.x + vec.x, Position.y, Position.z + vec.z));
            }
            else
            {
                // проверяем авто прыжок тут
                if (vec.y == 0 && _IsCollisionBody(new vec3(vec.x, 1f, vec.z)) == EnumCollisionBody.None)
                {
                    // Если с прыжком нет колизии то надо прыгать!!!
                    // TODO:: реализовать авто прыжок мягким
                    //SetPos(new vec3(Position.x + vec.x, Position.y, Position.z + vec.z));
                    return true;
                }
                else
                {
                    // одна из сторон не может проходить
                    EnumCollisionBody cx = _IsCollisionBody(new vec3(vec.x, 0, 0));
                    EnumCollisionBody cz = _IsCollisionBody(new vec3(0, 0, vec.z));
                    if (cx == EnumCollisionBody.None || (cx == EnumCollisionBody.None && cz == EnumCollisionBody.None))
                    {
                        // Если обе стороны могут, это лож, будет глюк колизии угла, идём по этой стороне только
                        // TODO:: определить по какой стороне идём можно по Yaw углу
                        SetPos(new vec3(Position.x + vec.x, Position.y, Position.z));
                    }
                    else if (cz == EnumCollisionBody.None)
                    {
                        SetPos(new vec3(Position.x, Position.y, Position.z + vec.z));
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяем коллизию тела c блоками по Y
        /// </summary>
        /// <returns>true - прекращение падения</returns>
        public bool CollisionBodyY(vec3 vec)
        {
            // Коллизия вертикали
            EnumCollisionBody onGround = _IsCollisionBody(new vec3(0, vec.y, 0));
            if (onGround == EnumCollisionBody.None)
            {
                SetPos(new vec3(Position.x, Position.y + vec.y, Position.z));
            }
            else if (onGround == EnumCollisionBody.CollisionDown)
            {
                SetPos(new vec3(Position.x, Position.y - vec.y, Position.z));
            }

            return onGround == EnumCollisionBody.CollisionDown;
        }
        /// <summary>
        /// Проверяем коллизию тела c блоками
        /// </summary>
        /// <param name="vec">координата позиции</param>
        protected EnumCollisionBody _IsCollisionBody(vec3 vec)
        {
            vec3 pos = Position + vec;

            HitBoxSizeUD hbs = SizeUD(pos);
            vec3i vd = hbs.Vdi;
            vec3i vu = hbs.Vui;

            //int y = vd.y;
            for (int y = vd.y; y <= vu.y; y++)
            {
                for (int x = vd.x; x <= vu.x; x++)
                {
                    for (int z = vd.z; z <= vu.z; z++)
                    {
                        if (World.GetBlock(new vec3i(x, y, z)).IsCollision)
                        {
                            if (vec.y < 0)
                            {
                              //  SetPos(new vec3(Position.x, vu.y, Position.z));
                                return EnumCollisionBody.CollisionDown;
                            }
                            return EnumCollisionBody.Collision;
                        }
                            
                    }
                }
            }
            return EnumCollisionBody.None;
        }

        /// <summary>
        /// Проверяем коллизию тела c блоками
        /// </summary>
        public bool IsCollisionBody(vec3 vec)
        {
            vec3 pos = Position + vec;
            HitBoxSizeUD hbs = SizeUD(pos);
            vec3i vd = hbs.Vdi;
            vec3i vu = hbs.Vui;

            for (int y = vd.y; y <= vu.y; y++)
            {
                for (int x = vd.x; x <= vu.x; x++)
                {
                    for (int z = vd.z; z <= vu.z; z++)
                    {
                        if (World.GetBlock(new vec3i(x, y, z)).IsCollision)
                        {
                            return true;
                        }

                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяем коллизию под ногами
        /// </summary>
        public bool IsCollisionDown(vec3 pos)
        {
            HitBoxSizeUD hbs = SizeUD(pos);
            vec3 vd = hbs.Vd;
            vec3 vu = hbs.Vu;
            vd.y -= .01f;
            //vu.y = vd.y;
            vec3i d = new vec3i(vd);
            vec3i d2 = new vec3i(vu);

            for (int x = d.x; x <= d2.x; x++)
            {
                for (int z = d.z; z <= d2.z; z++)
                {
                    if (World.GetBlock(new vec3i(x, d.y, z)).IsCollision)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Проверяем колизию блоков над головой
        /// </summary>
        public bool IsCollisionUp()
        {
            return IsCollisionUp(Position, 0.51f);
        }
        /// <summary>
        /// Проверяем колизию блоков над головой
        /// </summary>
        protected bool IsCollisionUp(vec3 pos, float addY)
        {
            HitBoxSizeUD hbs = SizeUD(pos);
            vec3 vd = hbs.Vd;
            vec3 vu = hbs.Vu;
            vu.y += addY;
            vd.y = vu.y;
            vec3i d = new vec3i(vd);
            vec3i d2 = new vec3i(vu);

            for (int x = d.x; x <= d2.x; x++)
            {
                for (int z = d.z; z <= d2.z; z++)
                {
                    if (World.GetBlock(new vec3i(x, d.y, z)).IsCollision)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяем коллизию тела c позицией хитбокса
        /// изпользуем защиту поставить блок и подобное
        /// </summary>
        /// <param name="pos">координата проверки колизии</param>
        public bool IsVoxelBody(vec3i pos)
        {
            HitBoxSizeUD hbs = SizeUD(Position);
            vec3i vd = hbs.Vdi;
            vec3i vu = hbs.Vui;

            for (int y = vd.y; y <= vu.y; y++)
            {
                for (int x = vd.x; x <= vu.x; x++)
                {
                    for (int z = vd.z; z <= vu.z; z++)
                    {
                        if (pos.Equals(new vec3i(x, y, z)))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Прорисовка хитбокса игрока
        /// </summary>
        public void BufferHitBox()
        {
            // HitBox игрока
            HitBoxSizeUD hbs = SizeUD(Position);
            vec3 vd = hbs.Vd;
            vec2 size = new vec2(Size.Width, Size.Heigth);
            LineRender line = new LineRender();
            line.Box(vd.x + size.x, vd.y + size.y / 2f, vd.z + size.x, size.x * 2f, size.y, size.x * 2f, .0f, .9f, .9f, 1f);
            //line.Box(vd.x + size.x + .5f, vd.y + size.y / 2f, vd.z + size.x + .5f, size.x * 2f, size.y, size.x * 2f, .0f, .9f, .9f, 1f);
            Buffer = line.ToBuffer();
            OnHitBoxChanged();
        }

        /// <summary>
        /// Растояние до объекта от глаз
        /// </summary>
        public float DistanceEyesTo(vec3i vec)
        {
            int x = vec.x - BlockEyes.x;
            int y = vec.y - BlockEyes.y;
            int z = vec.z - BlockEyes.z;
            return Mth.Sqrt(x * x + y * y + z * z);
        }

        #region Event

        private void Size_LookAtChanged(object sender, EventArgs e)
        {
            if (!UpdateEyes()) OnLookAtChanged();
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

        /// <summary>
        /// Событие изменён хитбокс
        /// </summary>
        public event EventHandler HitBoxChanged;
        /// <summary>
        /// Событие изменён хитбокс
        /// </summary>
        protected void OnHitBoxChanged()
        {
            HitBoxChanged?.Invoke(this, new EventArgs());
        }

        #endregion

        protected HitBoxSizeUD SizeUD(vec3 pos)
        {
            return new HitBoxSizeUD(pos, Size);
        }

        protected class HitBoxSizeUD
        {
            public vec3 Vd { get; protected set; } = new vec3();
            public vec3 Vu { get; protected set; } = new vec3();
            public vec3i Vdi { get { return new vec3i(Vd); } }
            public vec3i Vui { get { return new vec3i(Vu); } }

            public HitBoxSizeUD(vec3 pos, HitBoxSize size)
            {
                float w = size.Width;
                float h = size.Heigth;

                Vd = new vec3(pos.x - w, pos.y, pos.z - w);
                Vu = new vec3(pos.x + w, pos.y + h, pos.z + w);
            }
        }


        protected enum EnumCollisionBody
        {
            /// <summary>
            /// Нет коллизии
            /// </summary>
            None = 0,
            /// <summary>
            /// Коллизия
            /// </summary>
            Collision = 1,
            /// <summary>
            /// Коллизия с низу, когда падали вниз, прекращаем падать
            /// </summary>
            CollisionDown = 2
        }
    }
}
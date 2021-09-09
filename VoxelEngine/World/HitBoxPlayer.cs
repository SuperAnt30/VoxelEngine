using VoxelEngine.Actions;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;
using VoxelEngine.Util;

namespace VoxelEngine
{ 
    /// <summary>
    /// Нитбокс камеры (игрока)
    /// </summary>
    public class HitBoxPlayer
    {
        /// <summary>
        /// Размер
        /// </summary>
        public HitBoxSize Size { get; protected set; } = new HitBoxSize();
        /// <summary>
        /// Положение камеры
        /// </summary>
        public vec3 Position { get; protected set; }

        public HitBoxPlayer() { }

        public void SetPos(vec3 pos)
        {
            Position = pos;
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
            vec3i vd = hbs.vdi;
            vec3i vu = hbs.vui;

            //int y = vd.y;
            for (int y = vd.y; y <= vu.y; y++)
            {
                for (int x = vd.x; x <= vu.x; x++)
                {
                    for (int z = vd.z; z <= vu.z; z++)
                    {
                        if (Mouse.GetInstance().World.GetBlock(new vec3i(x, y, z)).IsCollision)
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
            vec3i vd = hbs.vdi;
            vec3i vu = hbs.vui;

            for (int y = vd.y; y <= vu.y; y++)
            {
                for (int x = vd.x; x <= vu.x; x++)
                {
                    for (int z = vd.z; z <= vu.z; z++)
                    {
                        if (Mouse.GetInstance().World.GetBlock(new vec3i(x, y, z)).IsCollision)
                        {
                            return true;
                        }

                    }
                }
            }
            return false;
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

        /// <summary>
        /// Проверяем коллизию под ногами
        /// </summary>
        public bool IsCollisionDown(vec3 pos)
        {
            HitBoxSizeUD hbs = SizeUD(pos);
            vec3 vd = hbs.vd;
            vec3 vu = hbs.vu;
            vd.y -= .01f;
            //vu.y = vd.y;
            vec3i d = new vec3i(vd);
            vec3i d2 = new vec3i(vu);

            for (int x = d.x; x <= d2.x; x++)
            {
                for (int z = d.z; z <= d2.z; z++)
                {
                    if (Mouse.GetInstance().World.GetBlock(new vec3i(x, d.y, z)).IsCollision)
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
            vec3 vd = hbs.vd;
            vec3 vu = hbs.vu;
            vu.y += addY;
            vd.y = vu.y;
            vec3i d = new vec3i(vd);
            vec3i d2 = new vec3i(vu);

            for (int x = d.x; x <= d2.x; x++)
            {
                for (int z = d.z; z <= d2.z; z++)
                {
                    if (Mouse.GetInstance().World.GetBlock(new vec3i(x, d.y, z)).IsCollision)
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
            vec3i vd = hbs.vdi;
            vec3i vu = hbs.vui;

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
        public void DrawHitBox()
        {
            // HitBox игрока
            HitBoxSizeUD hbs = SizeUD(Position);
            vec3 vd = hbs.vd;
            vec2 size = new vec2(Size.Width, Size.Heigth);
            OpenGLF.GetInstance().WorldLineM.Box("HitBoxPlayer",
                    vd.x + size.x, vd.y + size.y / 2f, vd.z + size.x, size.x * 2f, size.y, size.x * 2f, .0f, .9f, .9f, 1f);
        }

        protected HitBoxSizeUD SizeUD(vec3 pos)
        {
            return new HitBoxSizeUD(pos, Size);
        }

        protected class HitBoxSizeUD
        {
            public vec3 vd { get; protected set; } = new vec3();
            public vec3 vu { get; protected set; } = new vec3();
            public vec3i vdi { get { return new vec3i(vd); } }
            public vec3i vui { get { return new vec3i(vu); } }

            public HitBoxSizeUD(vec3 pos, HitBoxSize size)
            {
                float w = size.Width;
                float h = size.Heigth;

                vd = new vec3(pos.x - w, pos.y, pos.z - w);
                vu = new vec3(pos.x + w, pos.y + h, pos.z + w);
            }
        }
    }
}

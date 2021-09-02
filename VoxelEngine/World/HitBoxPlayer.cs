using VoxelEngine.Glm;

namespace VoxelEngine
{ 
    /// <summary>
    /// Нитбокс камеры (игрока)
    /// </summary>
    public class HitBoxPlayer
    {
        /// <summary>
        /// Верхняя точка +++
        /// </summary>
        public vec2 VecUp { get; protected set; }
        /// <summary>
        /// Нижняя точка ---
        /// </summary>
        public vec2 VecDown { get; protected set; }
        /// <summary>
        /// Положение камеры
        /// </summary>
        public vec3 Position { get; protected set; }
        /// <summary>
        /// Размер
        /// </summary>
        public vec2 Size
        {
            get { return VecUp - VecDown; }
        }

        public vec3 _SumVec3Vec2(vec3 v3, vec2 v2)
        {
            return new vec3(v3.x + v2.x, v3.y + v2.y, v3.z + v2.x);
        }

        public HitBoxPlayer(vec3 pos, vec2 vecUp, vec2 vecDown)
        {
            Position = pos;
            VecUp = vecUp;
            VecDown = vecDown;
        }

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
                    else if (cz == 0)
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
        /// 
        protected EnumCollisionBody _IsCollisionBody(vec3 vec)
        {
            vec3 pos = Position + vec;
            vec3i vd = new vec3i(_SumVec3Vec2(pos, VecDown));
            vec3i vu = new vec3i(_SumVec3Vec2(pos, VecUp));

            for (int y = vd.y; y <= vu.y; y++)
            {
                for (int x = vd.x; x <= vu.x; x++)
                {
                    for (int z = vd.z; z <= vu.z; z++)
                    {
                        if (Mouse.GetInstance().World.GetBlock(new vec3i(x, y, z)).IsCollision)
                        {
                            //if (vec.x > 0) SetPos(new vec3(vd.x, Position.y, Position.z));
                            //if (vec.x < 0) SetPos(new vec3(vu.x, Position.y, Position.z));
                            //if (vec.y > 0) SetPos(new vec3(Position.x, vd.y, Position.z));
                            if (vec.y < 0)
                            {
                                SetPos(new vec3(Position.x, vu.y, Position.z));
                                return EnumCollisionBody.CollisionDown;
                            }
                            //if (vec.z > 0) SetPos(new vec3(Position.x, Position.y, vd.z));
                            //if (vec.z < 0) SetPos(new vec3(Position.x, Position.y, vu.z));

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
            vec3i vd = new vec3i(_SumVec3Vec2(pos, VecDown));
            vec3i vu = new vec3i(_SumVec3Vec2(pos, VecUp));

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
        public bool IsCollisionDown(vec3 p)
        {
            vec3 vd = _SumVec3Vec2(p, VecDown);
            vec3 vu = _SumVec3Vec2(p, VecUp);
            vd.y -= 0.01f;
            vu.y = vd.y;
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
        protected bool IsCollisionUp(vec3 p, float addY)
        {
            vec3 vd = _SumVec3Vec2(p, VecDown);
            vec3 vu = _SumVec3Vec2(p, VecUp);
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
            vec3i vd = new vec3i(_SumVec3Vec2(Position, VecDown));
            vec3i vu = new vec3i(_SumVec3Vec2(Position, VecUp));

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
            vec3 vd = _SumVec3Vec2(Position, VecDown);
            vec3 vu = _SumVec3Vec2(Position, VecUp);
            // Position.y += VecDown.y - 0.01f;
            vec2 size = Size;
            OpenGLF.GetInstance().WorldLineM.Box("HitBoxPlayer",
                    vd.x + size.x / 2f, vd.y + size.y * 0.5f, vd.z + size.x / 2f, size.x, size.y - 0.1f, size.x, .0f, .9f, .9f, 1f);
        }
    }
}

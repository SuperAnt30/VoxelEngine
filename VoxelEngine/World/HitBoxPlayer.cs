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
        /// Определение коллизии по направлению движения вектора
        /// </summary>
        /// <param name="vecMove">вектор движения</param>
        /// <returns>возращает значене 0 значит коллизия</returns>
        public vec2i GetCollisionVecMove(vec3 vecMove)
        {
            vec3i vd = new vec3i(_SumVec3Vec2(Position, VecDown));
            vec3i vu = new vec3i(_SumVec3Vec2(Position, VecUp));
            if (vecMove.y > 0) vu.y++;

            if (vecMove.x <= 0 && vecMove.z <= 0) // SW
            {
                return _GetCollisionVecMove(vd, vu, new vec2i(-1, -1), vecMove);
            }
            else if (vecMove.x <= 0 && vecMove.z > 0) // WN
            {
                return _GetCollisionVecMove(vd, vu, new vec2i(-1, 1), vecMove);
            }
            else if (vecMove.x > 0 && vecMove.z <= 0) // SE
            {
                return _GetCollisionVecMove(vd, vu, new vec2i(1, -1), vecMove);
            }
            else if (vecMove.x > 0 && vecMove.z > 0) // EN
            {
                return _GetCollisionVecMove(vd, vu, new vec2i(1, 1), vecMove);
            }

            return new vec2i(0, 0);
        }

        /// <summary>
        /// Определение коллизии по конкретному направлению движения вектора
        /// </summary>
        /// <param name="vec">вектор направления</param>
        /// <param name="vecMove">вектор движения</param>
        /// <returns>возращает вектор куда можно двигаться</returns>
        protected vec2i _GetCollisionVecMove(vec3i vd, vec3i vu, vec2i vec, vec3 vecMove)
        {
            int ix = _IsCollisionWallX(vd, vu, vecMove, new vec2i(vec.x, 0));
            int iz = _IsCollisionWallZ(vd, vu, vecMove, new vec2i(0, vec.y));

            if (ix < 2 && iz < 2)
            {
                int ixz = _IsCollisionWallAngle(vd, vu, vecMove, vec);
                if (ixz < 2)
                {
                    // Проверка на авто прыжок
                    if (ix == 1 || iz == 1 || ixz == 1)
                    {
                        if (ix == 1 || ixz == 1) vec.x = 2;
                        if (iz == 1 || ixz == 1) vec.y = 2;
                    }
                    // нет припятствий
                    return vec;
                } else
                {
                    // препятстие на углу
                    if (ixz == 3 || ixz == 5) vec.x = 0;
                    if (ixz == 2 || ixz == 5) vec.y = 0;
                }
            } else
            {
                // Препятствие с одной стороны
                if (ix > 1) vec.x = 0;
                if (iz > 1) vec.y = 0;
            }

            return vec;
        }

        #region IsCollisionWall

        /// <summary>
        /// Определение коллизии столбца по высоте персонажа
        /// </summary>
        /// <param name="vec">ряд нижнего блока</param>
        protected int _IsCollisionWallAngle(vec3i vd, vec3i vu, vec3 vecMove, vec2i v2)
        {
            bool one = false;
            // Проверка угла
            for (int y = vd.y; y <= vu.y; y++)
            {
                vec3i vr = new vec3i(v2.x == 1 ? vu.x : vd.x, y, v2.y == 1 ? vu.z : vd.z);

                if (Mouse.GetInstance().World.GetBlock(vr).IsCollision)
                {
                    // Если блок есть

                    // Проверяем через блок по X
                    vec3i vrd = vr;
                    vrd.x += v2.x == 1 ? -2 : 2;
                    int idX = Mouse.GetInstance().World.GetBlock(vrd).IsCollision ? 2 : 0;

                    // Проверяем через блок по Z
                    vrd = vr;
                    vrd.z += v2.y == 1 ? -2 : 2;
                    int idZ = Mouse.GetInstance().World.GetBlock(vrd).IsCollision ? 3 : 0;

                    // Если не одинокий блок
                    if (idX + idZ > 0)
                    {
                        if (y == vd.y) one = true;
                        else return idX + idZ;
                    }

                    // Если одинокий блок, надо определить по какой стороне движемся
                    // Методика, от прошлой координаты, по какому блоку шли

                    // Доп проверка по прошлому.
                    vec3 posOld = OpenGLF.GetInstance().Cam.Position;
                    vec3i vd2 = new vec3i(_SumVec3Vec2(posOld, VecDown));
                    vec3i vu2 = new vec3i(_SumVec3Vec2(posOld, VecUp));

                    vr = new vec3i(v2.x == 1 ? vu2.x : vd2.x, y, v2.y == 1 ? vu.z : vd.z);
                    if (Mouse.GetInstance().World.GetBlock(vr).IsCollision)
                    {
                        // Игнорим координату Z
                        if (y == vd.y) one = true;
                        else return 2;
                    }
                    vr = new vec3i(v2.x == 1 ? vu.x : vd.x, y, v2.y == 1 ? vu2.z : vd2.z);
                    if (Mouse.GetInstance().World.GetBlock(vr).IsCollision)
                    {
                        // Игнорим координату X
                        if (y == vd.y) one = true;
                        else return 3;
                    }
                }
            }
            // Нет коллизии
            return one ? 1 : 0;
        }

        /// <summary>
        /// Определение коллизии столбца по высоте персонажа
        /// </summary>
        /// <param name="vec">ряд нижнего блока</param>
        protected int _IsCollisionWallZ(vec3i vd, vec3i vu, vec3 vecMove, vec2i v2)
        {
            // Проверка стороны Z
            vd.x += vecMove.x < 0 ? 1 : 0;
            vu.x += vecMove.x < 0 ? 0 : -1;
            bool one = false;
            for (int y = vd.y; y <= vu.y; y++)
            {
                for (int x = vd.x; x <= vu.x; x++)
                {
                    vec3i vr = new vec3i(x, y, v2.y == 1 ? vu.z : vd.z);
                    if (Mouse.GetInstance().World.GetBlock(vr).IsCollision)
                    {
                        if (y == vd.y) one = true;
                        else return 2;
                    }
                }
            }

            if (one)
            {
                // Доп проверка на один блок выше, так как будет авто блок
                for (int x = vd.x; x <= vu.x; x++)
                {
                    vec3i vr = new vec3i(x, vu.y + 1, v2.y == 1 ? vu.z : vd.z);
                    if (Mouse.GetInstance().World.GetBlock(vr).IsCollision)
                    {
                        return 2;
                    }
                }
                return 1;
            }
            // Нет коллизии
            return 0;
        }

        /// <summary>
        /// Определение коллизии столбца по высоте персонажа
        /// </summary>
        /// <param name="vec">ряд нижнего блока</param>
        protected int _IsCollisionWallX(vec3i vd, vec3i vu, vec3 vecMove, vec2i v2)
        {
            // Проверка стороны X
            vd.z += vecMove.z < 0 ? 1 : 0;
            vu.z += vecMove.z < 0 ? 0 : -1;
            bool one = false;
            for (int y = vd.y; y <= vu.y; y++) 
            {
                for (int z = vd.z; z <= vu.z; z++)
                {
                    vec3i vr = new vec3i(v2.x == 1 ? vu.x : vd.x, y, z);
                    if (Mouse.GetInstance().World.GetBlock(vr).IsCollision)
                    {
                        if (y == vd.y) one = true;
                        else return 2;
                    }
                }
            }

            if (one)
            {
                // Доп проверка на один блок выше, так как будет авто блок
                for (int z = vd.z; z <= vu.z; z++)
                {
                    vec3i vr = new vec3i(v2.x == 1 ? vu.x : vd.x, vu.y + 1, z);
                    if (Mouse.GetInstance().World.GetBlock(vr).IsCollision)
                    {
                        return 2;
                    }
                }
                return 1;
            }
           
            // Нет коллизии
            return 0;
        }

        #endregion

        /// <summary>
        /// Проверяем коллизию под ногами
        /// </summary>
        public bool IsCollisionDown()
        {
            return IsCollisionDown(OpenGLF.GetInstance().Cam.Position);
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

            if (Debag.GetInstance().IsDrawCollisium)
            {
                // HitBox игрока
                p.y += VecDown.y - 0.01f;
                vec2 size = Size;
                OpenGLF.GetInstance().WorldLineM.Box("HitBoxPlayer",
                        p.x, p.y + size.y * 0.5f, p.z, size.x, size.y - 0.1f, size.x, .0f, .9f, .9f, 1f);

                // Диагоналки блоков для колизии низа
                //OpenGLF.GetInstance().WorldLineM.Box("collisionDown2",
                //        d.x + .5f, d.y + 1f, d.z + .5f, 1, 0.2f, 1, .0f, .0f, .9f, 1f);
                //OpenGLF.GetInstance().WorldLineM.Box("collisionDown3",
                //        d2.x + .5f, d2.y + 1f, d2.z + .5f, 1, 0.2f, 1, .0f, .9f, .0f, 1f);
            } else
            {
                OpenGLF.GetInstance().WorldLineM.Remove("HitBoxPlayer");
                //OpenGLF.GetInstance().WorldLineM.Remove("collisionDown2");
                //OpenGLF.GetInstance().WorldLineM.Remove("collisionDown3");
            }

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
            return IsCollisionUp(OpenGLF.GetInstance().Cam.Position, 0.01f);
        }
        /// <summary>
        /// Проверяем колизию блоков над головой
        /// </summary>
        public bool IsCollisionUp(vec3 p, float addY)
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
                    //if (Mouse.GetInstance().World.GetBlock(new vec3i(x, d.y, z)).Id != 0)
                    if (Mouse.GetInstance().World.GetBlock(new vec3i(x, d.y, z)).IsCollision)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяем коллизию тела
        /// </summary>
        public bool IsCollisionBody(vec3 p)
        {
            vec3i vd = new vec3i(_SumVec3Vec2(p, VecDown));
            vec3i vu = new vec3i(_SumVec3Vec2(p, VecUp));

            for (int y = vd.y; y <= vu.y; y++)
            {
                for (int x = vd.x; x <= vu.x; x++)
                {
                    for (int z = vd.z; z <= vu.z; z++)
                    {
                        if (Mouse.GetInstance().World.GetBlock(new vec3i(x, y, z)).IsCollision)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Проверяем коллизию тела
        /// </summary>
        public bool IsVoxelBody(vec3i p)
        {
            vec3i vd = new vec3i(_SumVec3Vec2(Position, VecDown));
            vec3i vu = new vec3i(_SumVec3Vec2(Position, VecUp));

            for (int y = vd.y; y <= vu.y; y++)
            {
                for (int x = vd.x; x <= vu.x; x++)
                {
                    for (int z = vd.z; z <= vu.z; z++)
                    {
                        if (p.Equals(new vec3i(x, y, z)))
                            return true;
                    }
                }
            }
            return false;
        }
    }
}

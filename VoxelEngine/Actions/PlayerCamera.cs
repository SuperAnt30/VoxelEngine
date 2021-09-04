using System.Diagnostics;
using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World;

namespace VoxelEngine
{
    /// <summary>
    /// Управление камерой
    /// </summary>
    public class PlayerCamera
    {
        /// <summary>
        /// Вектор перемещения
        /// </summary>
        protected vec3 _move = new vec3(0);
        /// <summary>
        /// перемещение лево право -1 || 0 || 1
        /// </summary>
        protected Key _horizontal = 0;
        /// <summary>
        /// перемещение вперёд назад -1 || 0 || 1
        /// </summary>
        protected Key _vertical = 0;
        /// <summary>
        /// Перемещение вверх или вниз, вроде -1 || 0 || 1
        /// </summary>
        protected Key _height = 0;
        /// <summary>
        /// На земле ли мы
        /// </summary>
        protected bool _onGround = false;
        /// <summary>
        /// Игрок присел?
        /// </summary>
        protected int _isSneaking = 0;
        /// <summary>
        /// Объект для точного замера времени
        /// </summary>
        protected Stopwatch stopwatch = new Stopwatch();
        protected Camera cam;

        /// <summary>
        /// Активно ли ускорение бега
        /// </summary>
        public bool IsSpeed { get; protected set; } = false;
        /// <summary>
        /// Находиться ли глаза игрока в воде
        /// </summary>
        public bool IsEyesWater { get; protected set; } = false;
        /// <summary>
        /// Находиться ли тело игрока в воде
        /// </summary>
        public bool IsBodyWater { get; protected set; } = false;
        /// <summary>
        /// Находиться ли ноги игрок в воде (-1 блок над глазами)
        /// </summary>
        public bool IsLegsWater { get; protected set; } = false;

        /// <summary>
        /// Строка для дебага
        /// </summary>
        public string StrDebug { get; protected set; } = "";

        public PlayerCamera()
        {
            stopwatch.Start();
            cam = OpenGLF.GetInstance().Cam;
        }

        #region KeyDown

        /// <summary>
        /// Включить или выключить ускорение
        /// </summary>
        public void Speed()
        {
            if (_isSneaking == 0)
            {
                if (_vertical <= 0) IsSpeed = false;
                else IsSpeed = !IsSpeed;
            }
        }

        /// <summary>
        /// Шаг влево
        /// </summary>
        public void StepLeft()
        {
            if (_horizontal != Key.Minus)
            {
                _horizontal = Key.Minus;
                _Refrash();
            }
        }
        /// <summary>
        /// Шаг вправо
        /// </summary>
        public void StepRight()
        {
            if (_horizontal != Key.Plus)
            {
                _horizontal = Key.Plus;
                _Refrash();
            }
        }
        /// <summary>
        /// Шаг вперёд
        /// </summary>
        public void StepForward()
        {
            if (_vertical != Key.Plus)
            {
                _vertical = Key.Plus;
                _Refrash();
            }
            // сбрасываем ускорение
            IsSpeed = false;
        }
        /// <summary>
        /// Шаг назад
        /// </summary>
        public void StepBack()
        {
            if (_vertical != Key.Minus)
            {
                _vertical = Key.Minus;
                _Refrash();
            }
        }

        /// <summary>
        /// Перемещение вверх или прыжок
        /// </summary>
        public void StepJamp()
        {
            if (_height != Key.Plus)
            {
                _height = Key.Plus;
                _Refrash();
            }
        }

        /// <summary>
        /// Перемещение вниз или присесть
        /// </summary>
        public void StepDown()
        {
            if (_height != Key.Minus)
            {
                _height = Key.Minus;
                _Refrash();
            }
        }
        #endregion

        #region KeyUp

        /// <summary>
        /// Отпускаем клавишу A || D
        /// </summary>
        public void KeyUpHorizontal()
        {
            if (_horizontal != Key.None)
            {
                _horizontal = Key.None;
                _Refrash();
            }
        }
        /// <summary>
        /// Отпускаем клавишу W || S
        /// </summary>
        public void KeyUpVertical()
        {
            if (_vertical != Key.None)
            {
                _vertical = Key.None;
                _Refrash();
            }
        }
        /// <summary>
        /// Отпускаем клавишу прыгать / вверх
        /// </summary>
        public void KeyUpJamp()
        {
            if (_height != Key.None)
            {
                _height = Key.None;
                _Refrash();
            }
        }

        /// <summary>
        /// Опускаем клавишу сидеть / вниз
        /// </summary>
        public void KeyUpSneaking()
        {
            if (_height != Key.None)
            {
                _height = Key.None;
                _Refrash();
            }

            if (VEC.GetInstance().Moving == VEC.VEMoving.Survival)
            {
                // Встать
                // Проверка на возможность встать
                HitBoxPlayer hitBox = OpenGLF.GetInstance().Cam.GetHitBox();
                _isSneaking = 2;
                if (hitBox.IsCollisionUp())
                {
                    // Встать не можем
                    _isSneaking = 2;
                }
                else
                {
                    // Встаём
                    _Uping();
                }
            }
        }

        #endregion

        /// <summary>
        /// Обновить координаты перемещения
        /// определение скоростей тут, частично кроме вертикали
        /// </summary>
        protected void _Refrash()
        {
            float h, v, j;
            if (VEC.GetInstance().Moving == VEC.VEMoving.Survival)
            {
                if (_height == Key.Minus)
                {
                    // Присел
                    OpenGLF.GetInstance().Cam.Sneaking();
                    _isSneaking = 1;
                }
                h = IsLegsWater ? VE.SPEED_STEP : _isSneaking != 0 ? VE.SPEED_SNEAKING : VE.SPEED_STEP;
                v = h;
                j = _move.y;
                if (_vertical > 0 && IsSpeed)
                {
                    v = VE.SPEED_RUN;
                }
                if (IsLegsWater && !IsEyesWater)
                {
                    if (_height == Key.Plus)
                    {
                        if (IsBodyWater)
                        {
                            _onGround = false;
                            j = VE.SPEED_JAMP * .12f;
                        } 
                        else if(_onGround)
                        {
                            _onGround = false;
                            j = VE.SPEED_JAMP * .6f;
                        }
                    }
                    //OpenGLF.GetInstance().Cam.Sailing();

                    // Если в воде то замедляем в 2 раза
                    h *= .5f;
                    v *= .5f;
                }
                else if (IsEyesWater)
                {
                    // плывём вверх или низ под водой
                    if (_height != 0) j = (float)_height * VE.SPEED_SWIM;

                    // Если в воде то замедляем в 2,5 раза
                    h *= .4f;
                    v *= .4f;
                }
                else
                {
                    // прыжок
                    if (_height == Key.Plus && _onGround)
                    {
                        _onGround = false;
                        j = VE.SPEED_JAMP;
                    }
                }
            } else
            {
                h = VE.SPEED_STEP;
                v = _vertical > 0
                    ? IsSpeed
                        ? VE.SPEED_FLY_FAST : VE.SPEED_FLY
                    : VE.SPEED_STEP;
                j = VE.SPEED_FLY_VERTICAL * (float)_height;
            }

            float v2 = (float)_vertical * v;
            float h2 = (float)_horizontal * h;

            StrDebug =string.Format("j: {0:0.0}{8}{9} v: {1:0.0} h: {2:0.0} {3}{4}{5}{6}{7}",
                j, v2, h2, _onGround ? "__" : "",
                IsEyesWater ? "[E]" : "", IsBodyWater ? "[B]" : "", IsLegsWater ? "[L]" : "", flow != Pole.Down ? flow.ToString() : "",
                IsSpeed ? "[Sp]" : "", _isSneaking == 1 ? "[Sn]" : "");

            _move.y = j;
            _move.x = glm.sin(cam.Yaw + 1.570796f) * h2;
            _move.z = glm.cos(cam.Yaw + 1.570796f) * h2;
            _move.x -= glm.sin(cam.Yaw) * v2;
            _move.z -= glm.cos(cam.Yaw) * v2;

            // Если есть течение то корректируем от смещения
            if (flow == Pole.East) _move.x += VE.SPEED_FLOW;
            else if (flow == Pole.West) _move.x -= VE.SPEED_FLOW;
            else if (flow == Pole.North) _move.z -= VE.SPEED_FLOW;
            else if (flow == Pole.South) _move.z += VE.SPEED_FLOW;
        }
        
        /// <summary>
        /// Параметр этапа встать, если true мы пытаемся встать
        /// </summary>
        protected bool _isUping = false;
        /// <summary>
        /// Начинаем вставать
        /// </summary>
        protected void _Uping()
        {
            if (_move.y == 0 && _onGround)
            {
                _move.y = VE.SPEED_UPING;
                //if (IsLegsWater) _move.y *= 2f;
                _isUping = true;
            }
        }
        /// <summary>
        /// Встали
        /// </summary>
        protected void _Uped()
        {
            _isUping = false;
            // Встаём
            OpenGLF.GetInstance().Cam.Worth();
            _isSneaking = 0;
        }

        /// <summary>
        /// Обновление каждый такт
        /// </summary>
        protected void _Update()
        {
            if (VEC.GetInstance().Moving == VEC.VEMoving.Survival)
            {
                HitBoxPlayer hitBox = cam.GetHitBox();

                //=== Проверка падения

                // Если не на земле
                if (!_onGround)
                {
                    if (IsEyesWater && _move.y <= 0)
                    {
                        _move.y = -VE.SPEED_FLOW;
                    }
                    if (IsLegsWater)
                    {
                        _move.y -= VE.SPEED_DOWN * 0.1f;
                    }
                    else _move.y -= VE.SPEED_DOWN;
                }
                else
                {
                    // Если мы не прыгаем, и под нагами нет блока, мы начинаем падать вниз
                    if (!hitBox.IsCollisionDown(hitBox.Position)) _onGround = false;
                }

                //=== Проверка упирания вверх

                // Положение сидя
                if (_move.y == 0 && _isSneaking == 2 && !hitBox.IsCollisionUp())
                {
                    // Начинаем прыжок для вставания, если нет прыжка и над головой нет блоков
                    _Uping();
                }
                if (_move.y <= 0 && _isUping)
                {
                    _Uped();
                }
            }
            
            if (VEC.GetInstance().Moving != VEC.VEMoving.FreeFlight)
            {
                HitBoxPlayer hitBox = cam.GetHitBox();

                //=== Проваливания в блок

                if (hitBox.IsCollisionBody(new vec3()))
                {
                    if (!hitBox.IsCollisionBody(new vec3(0, 1f, 0)))
                    {
                        cam.Position = new vec3(cam.Position.x, Mth.Floor(cam.Position.y + 1f), cam.Position.z);
                    }
                }
            }
        }

        /// <summary>
        /// Изменения или точнее перемещение камеры
        /// </summary>
        public void Update()
        {
            float time = stopwatch.ElapsedMilliseconds / 1000f;
            stopwatch.Restart();
            if (time > 1.5f) time = 1.5f;

            _Refrash();
            
            Camera cam = OpenGLF.GetInstance().Cam;
            HitBoxPlayer hitBox = cam.GetHitBox();
            if (Debag.GetInstance().IsDrawCollisium) hitBox.DrawHitBox();
            vec3 move = _move * time;

            // Перемещения
            if (VEC.GetInstance().Moving != VEC.VEMoving.FreeFlight)
            {
                // Коллизия для векторов поверхности
                if (VEC.GetInstance().Moving == VEC.VEMoving.Survival)
                {
                    //if (IsLegsWater)
                    //{
                    //    //cam.Sneaking();
                    //    if (!hitBox.CollisionBodyXZ(move))
                    //    {
                    //        cam.Sneaking();
                    //        hitBox = cam.GetHitBox();
                    //        hitBox.CollisionBodyXZ(move);
                    //        cam.Worth();
                    //    }
                    //    //
                    //}
                    //else
                    {
                        bool isAutoJump = hitBox.CollisionBodyXZ(move);
                        
                        if (isAutoJump && _onGround)
                        {
                            _move.y = IsLegsWater ? VE.SPEED_WATER_AUTOJAMP : VE.SPEED_AUTOJAMP;
                            move.y = _move.y * time;  // под вопросом
                        }
                    }
                } else
                {
                    hitBox.CollisionBodyXZ(move);
                }

                // Коллизия для вектора вертикали
                if (hitBox.CollisionBodyY(move))
                {
                    _onGround = true;
                    // надо упереться в блок, чтоб не падать
                    hitBox.SetPos(new vec3(hitBox.Position.x, Mth.Floor(hitBox.Position.y), hitBox.Position.z)); 
                    _move.y = 0;
                }

            } else
            {
                hitBox.SetPos(hitBox.Position + move);
            }
            // Меняем позицию
            cam.Position = hitBox.Position;
        }

        /// <summary>
        /// Направление течения если в воде
        /// </summary>
        Pole flow = Pole.Down;

        /// <summary>
        /// Добавляем в TPS, чтоб корректировать прыжки
        /// </summary>
        public void Tick()
        {
            // Опрделяем в воде ли я
            vec3i pos = new vec3i(OpenGLF.GetInstance().Cam.Position);
            // глаза
            IsEyesWater = Mouse.GetInstance().World.GetBlock(pos).IsWater;
            // тело
            IsBodyWater = Mouse.GetInstance().World.GetBlock(pos + new vec3i(0, -1, 0)).IsWater;
            // ноги
            IsLegsWater = Mouse.GetInstance().World.GetBlock(pos + new vec3i(0, -3, 0)).IsWater;

            if (IsLegsWater)
            {
                Block block = Mouse.GetInstance().World.GetBlock(pos + new vec3i(0, -3, 0));
                if (block.EBlock == EnumBlock.WaterFlowing)
                {
                    BlockRender blockRender = new BlockRender(new ChunkRender(Mouse.GetInstance().World.GetChunk(block.Position), new WorldRender()), block);
                    vec4 level = blockRender.HeightWater() * VE.WATER_LEVEL;

                    float w = level.x + level.y;
                    float s = level.y + level.z;
                    float e = level.z + level.w;
                    float n = level.w + level.x;

                    if (w > e && w >= s && w >= n)
                    {
                        flow = Pole.West;
                    }
                    else if (s >= w && s >= e && s > n)
                    {
                        flow = Pole.South;
                    }
                    else if (e > w && e >= s && e >= n)
                    {
                        flow = Pole.East;
                    }
                    else
                    {
                        flow = Pole.North;
                    }
                } else
                {
                    flow = Pole.Down;
                }
            }
            else
            {
                flow = Pole.Down;
            }

            _Update();
        }

        /// <summary>
        /// Нажатие клавиши
        /// </summary>
        protected enum Key
        {
            /// <summary>
            /// Значение отрицательное
            /// </summary>
            Minus = -1,
            /// <summary>
            /// Нет действия
            /// </summary>
            None = 0,
            /// <summary>
            /// Значение положительное
            /// </summary>
            Plus = 1
        }
    }
}

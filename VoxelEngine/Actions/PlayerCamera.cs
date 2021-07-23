using VoxelEngine.Glm;

namespace VoxelEngine
{
    /// <summary>
    /// Управление камерой
    /// </summary>
    public class PlayerCamera
    {
        /// <summary>
        /// перемещение лево право м/с
        /// </summary>
        protected float _horizontal = 0;
        /// <summary>
        /// перемещение вперёд назад м/с
        /// </summary>
        protected float _vertical = 0;
        /// <summary>
        /// перемещение для креатива вверх вниз м/с
        /// </summary>
        protected float _jamp = 0;


        /// <summary>
        /// прыжок ли сейчас для выживания
        /// </summary>
        protected bool _isJamp = false;

        /// <summary>
        /// фиксируем тикущее время
        /// </summary>
        protected long _time = System.DateTime.Now.Ticks;

        /// <summary>
        /// За сколько тиков должны закончить прыжок
        /// </summary>
        protected int _tickjamp = 0;

        /// <summary>
        /// Игрок присел?
        /// </summary>
        protected int _isSneaking = 0;

       // protected float _speedStep = VE.SPEED_STEP;

        /// <summary>
        /// Активно ли ускорение бега
        /// </summary>
        public bool IsSpeed { get; protected set; } = false;


        public void StepLeft()
        {
            _TimeNow();
            _horizontal = _isSneaking != 0 ? -VE.SPEED_SNEAKING : -VE.SPEED_STEP;
        }
        public void BeginRight()
        {
            _TimeNow();
            _horizontal = _isSneaking != 0 ? VE.SPEED_SNEAKING : VE.SPEED_STEP;
        }

        public void StepJamp()
        {
            if (VEC.GetInstance().Moving == VEC.VEMoving.Survival)
            {
                if (!_isJamp)
                {
                    _TimeNow();
                    // за сколько тиков достигнет пика
                    _tickjamp = 4;
                    // высота прыжка
                    _jamp = VE.SPEED_JAMP;
                    _isJamp = true;
                }
            }
            else
            {
                _TimeNow();
                _jamp = VE.SPEED_FLY_VERTICAL;
            }
        }
        public void StepDown()
        {
            if (VEC.GetInstance().Moving == VEC.VEMoving.Survival)
            {
                // присесть
                OpenGLF.GetInstance().Cam.Sneaking();
                _isSneaking = 1;
                if (_vertical != 0) _vertical = VE.SPEED_SNEAKING;
            }
            else
            {
                // лететь вниз
                _TimeNow();
                _jamp = -VE.SPEED_FLY_VERTICAL;
            }
        }
        public void StepForward()
        {
            IsSpeed = false; // сбрасываем ускорение
            _TimeNow();
            //vertical = 11f;
            _vertical = VE.SPEED_STEP;
            _vertical = VEC.GetInstance().Moving == VEC.VEMoving.Survival 
                ? (_isSneaking != 0 ? VE.SPEED_SNEAKING : VE.SPEED_STEP) 
                : VE.SPEED_FLY;
        }
        public void StepBack()
        {
            _TimeNow();
            _vertical = _isSneaking != 0 ? -VE.SPEED_SNEAKING : -VE.SPEED_STEP;
        }
        /// <summary>
        /// Отпускаем клавишу A || D
        /// </summary>
        public void KeyUpHorizontal()
        {
            _horizontal = 0;
        }
        /// <summary>
        /// Отпускаем клавишу W || S
        /// </summary>
        public void KeyUpVertical()
        {
            _vertical = 0;
        }
        /// <summary>
        /// Отпускаем клавишу прыгать / вверх
        /// </summary>
        public void KeyUpJamp()
        {
            if (VEC.GetInstance().Moving != VEC.VEMoving.Survival)
            {
                _jamp = 0;
            }
        }

        protected void _Up()
        {
            // Встаём
            OpenGLF.GetInstance().Cam.Worth();
            _TimeNow();
            // за сколько тиков достигнет пика
            _tickjamp = 4;
            // высота прыжка
            _jamp = 4f;// VE.SPEED_JAMP;
            _isSneaking = 0;
            if (_vertical > 0) _vertical = IsSpeed ? VE.SPEED_RUN : VE.SPEED_STEP;
            else if (_vertical < 0) _vertical = IsSpeed ? -VE.SPEED_RUN : -VE.SPEED_STEP;
        }
        /// <summary>
        /// Опускаем клавишу сидеть / вниз
        /// </summary>
        public void KeyUpSneaking()
        {
            if (VEC.GetInstance().Moving == VEC.VEMoving.Survival)
            {
                // Встать

                // Проверка на возможность
                HitBoxPlayer hitBox = OpenGLF.GetInstance().Cam.GetHitBox();
                if (hitBox.IsCollisionUp(hitBox.Position, 0.51f))
                {
                    // Встать не можем
                    _isSneaking = 2;
                }
                else
                {
                    // Встаём
                    _Up();
                }
            } else
            {
                _jamp = 0;
            }
        }

        protected void _TimeNow()
        {
            if (_horizontal == 0 && _vertical == 0 && _jamp == 0)
                _time = System.DateTime.Now.Ticks;
        }

        
        /// <summary>
        /// Включить или выключить ускорение
        /// </summary>
        public void Speed()
        {
            IsSpeed = !IsSpeed;

            // if (vertical > 0) vertical = isSpeed ? 22f: 11f;
            if (_vertical > 0)
            {
                if (VEC.GetInstance().Moving == VEC.VEMoving.Survival)
                {
                    if (_isSneaking == 0)
                    {
                        //_speedStep = IsSpeed ? VE.SPEED_RUN : VE.SPEED_STEP;
                        _vertical = IsSpeed ? VE.SPEED_RUN : VE.SPEED_STEP;
                    } else
                    {
                        _vertical = VE.SPEED_SNEAKING;
                    }
                }
                else
                {
                    _vertical = IsSpeed ? VE.SPEED_FLY_FAST : VE.SPEED_FLY;
                }
            }
            else IsSpeed = false;
        }

        

        /// <summary>
        /// Скорости перемещения для дебага
        /// </summary>
        /// <returns></returns>
        public string jvh()
        {
            return string.Format("j: {0:0.00}\r\nv: {1:0.00}\r\nh: {2:0.00}", _jamp, _horizontal, _vertical);
        }

        /// <summary>
        /// Прорисовка, или точнее перемещение камеры
        /// </summary>
        public void Draw()
        {
            Camera cam = OpenGLF.GetInstance().Cam;
            if (VEC.GetInstance().Moving == VEC.VEMoving.Survival && _jamp == 0)
            {
                // Если мы не прыгаем, и под нагами нет блока, мы начинаем падать вниз
                if (!OpenGLF.GetInstance().Cam.GetHitBox().IsCollisionDown())
                {
                    // запуск падения
                    _TimeNow();
                    _jamp = -VE.SPEED_DOWN;
                    // запрет, чтоб при падении не смог прыгнуть
                    _isJamp = true;
                }
            }
            if (VEC.GetInstance().Moving != VEC.VEMoving.FreeFlight && _isSneaking == 2)
            {
                // Положение сидя
                HitBoxPlayer hitBox = cam.GetHitBox();
                if (_jamp == 0 && !hitBox.IsCollisionUp(hitBox.Position, 0.51f))
                {
                    // Встаём если нет прыжка и над головой нет блоков
                    _Up();
                }
            }

            // Если есть любое смещение
            if (_horizontal != 0 || _vertical != 0 || _jamp != 0)
            {
                
                // Определяем направление движения
                
                vec3 front = glm.normalize(new vec3(cam.Front.x, 0, cam.Front.z)); // фронт без Y, чтоб не терять скорость
                vec3 desiredMove = front * _vertical + cam.Right * _horizontal; // определение направления
                desiredMove.y = _jamp; // вверх вниз
                vec3 moveDir = new vec3(desiredMove.x, desiredMove.y, desiredMove.z); // фиксируем

                // тикущее время
                long currentFrame = System.DateTime.Now.Ticks;
                // сколько секунд прошло от прошлого тика
                float time = (float)(currentFrame - _time) / 10000000f;
                // перезаписываем время для кэша
                _time = currentFrame;
                
                
                if (VEC.GetInstance().Moving != VEC.VEMoving.FreeFlight)
                {
                    // Проверка коллизии

                    HitBoxPlayer hitBox = cam.GetHitBox();
                    // вектор передвижения камеры с учётом прошедшего времени
                    hitBox.SetPos(OpenGLF.GetInstance().Cam.Position + moveDir * time);
                    vec2i vecYaw = hitBox.GetCollisionVecMove(moveDir);

                    // Если упираемся в стенку, убираем движения в ту сторону
                    if (vecYaw.x == 0) moveDir.x = 0;
                    if (vecYaw.y == 0) moveDir.z = 0;

                    // дополнительная проверка будущего положения
                    if (vecYaw.x != 2 && vecYaw.y != 2 
                        && hitBox.IsCollisionBody(OpenGLF.GetInstance().Cam.Position + moveDir * time))
                    {
                        // Если в теле есть блок, то перемещение невозможно
                        moveDir.x = 0;
                        moveDir.z = 0;
                    }
                    

                    // Условия для авто прыжка на 1 блок
                    if ((moveDir.x != 0 || moveDir.z != 0) && (vecYaw.x == 2 || vecYaw.y == 2))
                    {
                        if (!_isJamp)
                        {
                            // Авто прыжок
                            // TODO:: Авто прыжок сделать мягче
                            //OpenGLF.GetInstance().Cam.Position = OpenGLF.GetInstance().Cam.Position + new vec3(0, 1, 0);

                            _TimeNow();
                            _tickjamp = 2; // 4
                            // высота прыжка
                            _jamp = 8;// 4
                            _isJamp = true;
                        }
                    }

                    //if (_jamp != 0 && hitBox.IsCollisionUp(OpenGLF.GetInstance().Cam.Position + moveDir * time, 0.51f))
                    //{
                    //    moveDir.x = 0;
                    //    moveDir.z = 0;
                    //}

                    // Коллизия по вертикали
                    if (moveDir.y != 0)// && vecYaw.y == 0)
                    {
                        // дополнительная проверка без перемещения по вертикали
                        hitBox = cam.GetHitBox();
                        if (moveDir.y > 0 && hitBox.IsCollisionUp())
                        {
                            // упёрлись головой, начинаем падать
                            moveDir.y = 0;
                            _jamp = -0.1f;
                        }
                        else if (moveDir.y < 0 && hitBox.IsCollisionDown())
                        {
                            // проверка на ровное падение, упали
                            _isJamp = false;
                            _jamp = 0;
                            moveDir.y = 0;
                            // Корректировка по высоте
                            OpenGLF.GetInstance().Cam.PositionVertical();
                        }
                    }

                    
                }

                // Меняем позицию
                OpenGLF.GetInstance().Cam.Position = OpenGLF.GetInstance().Cam.Position + moveDir * time;
            }
        }

        /// <summary>
        /// Добавляем в TPS, чтоб корректировать прышки
        /// </summary>
        public void Tick()
        {
            // Определяем амплитуду прыжка
            if (_tickjamp > 0)
            {
                _tickjamp--;
                if (_tickjamp == 0)
                {
                    _jamp = -4.8f;
                }
            }

            // увеличиваем скорость падения
            if (VEC.GetInstance().Moving == VEC.VEMoving.Survival && _jamp < 0)
            {
                _jamp = (_jamp - 0.8f) * .98f;
                //_jamp = (_jamp - 1.4f) * .98f;
            }
        }
    }
}

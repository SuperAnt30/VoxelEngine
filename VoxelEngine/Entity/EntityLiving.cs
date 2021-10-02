using VoxelEngine.Glm;
using VoxelEngine.Renderer;
using VoxelEngine.Renderer.Blk;
using VoxelEngine.Renderer.Chk;
using VoxelEngine.Util;
using VoxelEngine.World;
using VoxelEngine.World.Blk;

namespace VoxelEngine.Entity
{
    /// <summary>
    /// Объект жизни сущьности, отвечает за движение вращение и прочее
    /// </summary>
    public class EntityLiving : EntityBase
    {
        /// <summary>
        /// Видимость сущьности для Frustum Culling
        /// </summary>
        public VisibleDraw Visible { get; set; } = VisibleDraw.See;
        /// <summary>
        /// Режим перемещения
        /// </summary>
        public VEMoving Mode { get; protected set; } = VEMoving.Survival;
        /// <summary>
        /// Объект скоростей
        /// </summary>
        protected EntitySpeed speed = new EntitySpeed();
        /// <summary>
        /// Ключевой объект перемещения движения
        /// </summary>
        public MovingKey Moving { get; protected set; } = new MovingKey();
        /// <summary>
        /// На земле
        /// </summary>
        public bool OnGround { get; protected set; } = false;

        /// <summary>
        /// Бежим
        /// </summary>
        public bool IsSprinting { get; protected set; } = false;
        /// <summary>
        /// Прыгаем
        /// </summary>
        public bool IsJumping { get; protected set; } = false;

        /// <summary>
        /// Нужен ли рендер
        /// </summary>
        public bool IsRender { get; protected set; } = true;

        /// <summary>
        /// Результат сидеть
        /// </summary>
        protected EnumSneaking sneaking = EnumSneaking.DonSit;
        /// <summary>
        /// Вектор движения
        /// </summary>
        protected vec3 motion;
        /// <summary>
        /// счётчик паузы ходьбы
        /// </summary>
        protected int pauseStepSound = 0;
        /// <summary>
        /// счётчик паузы ходьбы в вводе
        /// </summary>
        protected int pauseWaterSound = 0;

        public EntityLiving(WorldBase world) : base(world) { }

        public void SetMode(VEMoving mode)
        {
            this.Mode = mode;
        }
        /// <summary>
        /// Пытается переместить объект на переданное смещение. 
        /// </summary>
        /// <param name="motion">вектор смещения без скорости</param>
        //public virtual void MoveEntity(vec3 motion)
        //{

        //}

        /// <summary>
        /// Начало движения
        /// </summary>    
        //public void StartMovement()
        //{
        //    if (acceleration != EnumMovingKey.Plus)
        //    {
        //        acceleration = EnumMovingKey.Plus;
        //        accelerationValue = 0f;
        //    }
        //}

        //public void MovingCache()
        //{
        //    movingOld = Moving.Clone();
        //}

        /// <summary>
        /// Конец движения
        /// </summary>    
        //public void EndMovement()
        //{
        //    if (acceleration != EnumMovingKey.Minus && Moving.IsStand())
        //    {
        //        acceleration = EnumMovingKey.Minus;
        //        accelerationValue = 1f;
        //        Moving = movingOld;
        //    }
        //}

        /// <summary>
        /// Используется как в воде, так и в летающих объектах 
        /// </summary>
        //public void MoveFlying(float strafe, float forward, float friction)
        //{

        //}

        /// <summary>
        /// Обработка прыжка
        /// </summary>
        public void Jump()
        {
            if (Mode == VEMoving.Survival && !IsJumping)
            {
                IsJumping = true;
                Moving.Up();
            }
        }

        /// <summary>
        /// Прекратить прыжок
        /// </summary>
        public void JumpCancel()
        {
            if (IsJumping)
            {
                Moving.HeightCancel();
                IsJumping = false;
            }
        }


        protected void MotionAngle(float j, float v, float h)
        {
            motion.y = j;
            motion.x = glm.sin(RotationYaw + 1.570796f) * h;
            motion.z = glm.cos(RotationYaw + 1.570796f) * h;
            motion.x -= glm.sin(RotationYaw) * v;
            motion.z -= glm.cos(RotationYaw) * v;
        }

        protected string ToDebug(float j, float v, float h)
        {
            return string.Format("j: {0:0.0}{8}{9} v: {1:0.0} h: {2:0.0} {3}{5}{4}{6}{7}",
                j, v, h, OnGround ? "__" : "",
                HitBox.IsEyesWater ? "[E]" : "", HitBox.IsDownWater ? "[D]" : "", HitBox.IsLegsWater ? "[L]" : "", HitBox.Flow != Pole.Down ? HitBox.Flow.ToString() : "",
                IsSprinting ? "[Sp]" : "", IsSneaking ? "[Sn]" : "");
        }

        public void SetMotionY(float y)
        {
            motion.y = y;
        }

        /// <summary>
        /// Обновление каждый такт (TPS)
        /// </summary>
        public virtual void UpdateTick(long tick)
        {
            //Moving.Tick();

            if (Mode == VEMoving.Survival)
            {
                pauseStepSound--;
                pauseWaterSound--;
                //if (HitBox.IsLegsWaterOn || HitBox.IsLegsWaterOff)
                //{
                //    if (HitBox.IsLegsWaterOn) HitBox.LegsWaterOn();
                //    else  HitBox.LegsWaterOff();
                //    World.Audio.PlaySound("liquid.swim" + (random.Next(4) + 1), new vec3(0), 0.5f, 1f);
                //}

                if (HitBox.IsLegsWater)
                {
                    BlockBase block = World.GetBlock(HitBox.BlockPos);
                    if (block.EBlock == EnumBlock.WaterFlowing)
                    {
                        BlockRender blockRender = new BlockRender(new ChunkRender(World.GetChunk(block.Position),
                            (WorldRender)World), block);
                        vec4 level = blockRender.HeightWater() * VE.WATER_LEVEL;

                        float w = level.x + level.y;
                        float s = level.y + level.z;
                        float e = level.z + level.w;
                        float n = level.w + level.x;

                        if (w > e && w >= s && w >= n)
                        {
                            HitBox.Flow = Pole.West;
                        }
                        else if (s >= w && s >= e && s > n)
                        {
                            HitBox.Flow = Pole.South;
                        }
                        else if (e > w && e >= s && e >= n)
                        {
                            HitBox.Flow = Pole.East;
                        }
                        else
                        {
                            HitBox.Flow = Pole.North;
                        }
                    }
                    else
                    {
                        HitBox.Flow = Pole.Down;
                    }
                    
                }
                else
                {
                    HitBox.Flow = Pole.Down;
                }

                // Если не на земле
                if (!OnGround)
                {
                    if (HitBox.IsEyesWater && motion.y <= 0)
                    {
                        // Целиком в воде
                        motion.y = -VE.SPEED_FLOW;
                    }
                    else if (HitBox.IsDownWater || HitBox.IsLegsWater)
                    {
                        // Под ногами вода, скорее всего плывём
                        motion.y -= VE.SPEED_DOWN * 0.1f;
                    }
                    else motion.y -= VE.SPEED_DOWN;
                }
                else
                {
                    // Если мы не прыгаем, и под нагами нет блока, мы начинаем падать вниз
                    if (!HitBox.IsCollisionDown(HitBox.Position)) OnGround = false;
                }

                //=== Проверка упирания вверх

                // Положение сидя
                if (motion.y == 0 && IsSneakingNearly && !HitBox.IsCollisionUp())
                {
                    // Начинаем прыжок для вставания, если нет прыжка и над головой нет блоков
                    Uping();
                }
                if (motion.y <= 0 && isUping)
                {
                    Uped();
                }
            }
            if (Mode != VEMoving.FreeFlight)
            {
                UpPos();
            }
        }

        protected void UpPos()
        {
            if (HitBox.IsCollisionBody(new vec3(0)) && !HitBox.IsCollisionBody(new vec3(0, 1f, 0)))
            {
                HitBox.UpPos();
            }
        }

        /// <summary>
        /// Коллизия для вектора вертикали
        /// </summary>
        public void CollisionBodyY(vec3 move)
        {
            if (HitBox.CollisionBodyY(move))
            {
                OnGround = true;
                // надо упереться в блок, чтоб не падать
                HitBox.SetPos(new vec3(HitBox.Position.x, Mth.Floor(HitBox.Position.y), HitBox.Position.z));
                motion.y = 0;
            }
        }

        public vec3 MoveTime(float time)
        {
            // Тут надо время умножить на вектор
            return motion * time;
        }

        /// <summary>
        /// Обновить координаты перемещения
        /// определение скоростей тут, частично кроме вертикали
        /// </summary>
        public virtual string UpdateMoving()
        {
            float h, v, j;
            if (Mode == VEMoving.Survival)
            {
                // Звук от перемещения
                if (OnGround && !HitBox.IsLegsWater && !IsSneaking && pauseStepSound <= 0 
                    && (!Moving.Vertical.IsZero || !Moving.Horizontal.IsZero))
                {
                    SoundMoving();
                }
                // Звук перемещения в воде
                if (pauseWaterSound <= 0 && (HitBox.IsLegsWater || HitBox.IsLegsWater)
                    && (!Moving.Vertical.IsZero || !Moving.Horizontal.IsZero))
                {
                    SoundWater();
                }

                h = IsSneaking ? speed.Sneaking : speed.Step;
                v = h;
                j = motion.y;

                if (Moving.Height.MinusAction && OnGround && !IsSneaking)
                {
                    // Присел
                    Sneaking();
                }

                if (Moving.Vertical.Plus && IsSprinting && !IsSneaking)
                {
                    v = speed.Sprinting;
                }
                
                if ((HitBox.IsDownWater || HitBox.IsLegsWater) && !HitBox.IsEyesWater)
                {
                    // Игрок над водой или частично в воде
                    if (Moving.Height.PlusAction)
                    {
                        if (HitBox.IsLegsWater)
                        {
                            // Ноги в воде но нет под ногами дна
                            // скорее всего плывёт
                            OnGround = false;
                            j = speed.Swim;
                        }
                        else
                        if (OnGround)
                        {
                            // мы на земле но рядом блок под нами с водой
                            OnGround = false;
                            // Но так-как определить в воде плывём или падаем с данного положения
                            // высота прыжка уменьшаем, чтоб высота прыжка не привышала требуемую
                            j = speed.Jamp * .5f;
                        }
                    }
                    // Если в воде то замедляем в 2 раза
                    h *= .5f;
                    v *= .5f;
                }
                else if (HitBox.IsEyesWater)
                {
                    // Игрок под водой
                    // плывём вверх или низ под водой
                    if (!Moving.Height.IsZero) j = Moving.GetHeightValue() * speed.Swim;

                    // Если в воде то замедляем в 2,5 раза
                    h *= .4f;
                    v *= .4f;
                }
                else
                {
                    // Прыжок
                    if (Moving.Height.PlusAction && OnGround)
                    {
                        OnGround = false;
                        j = speed.Jamp;
                    }
                }
                
                v *= Moving.GetVerticalValue();
                h *= Moving.GetHorizontalValue();

                if (IsSprinting && !OnGround && v > 0)
                {
                    // Если прыжок с бегом, то скорость увеличивается на 20%
                    v *= 1.2f;
                }

                MotionAngle(j, v, h);

                // Если есть течение то корректируем от смещения
                if (HitBox.Flow == Pole.East) motion.x += VE.SPEED_FLOW;
                else if (HitBox.Flow == Pole.West) motion.x -= VE.SPEED_FLOW;
                else if (HitBox.Flow == Pole.North) motion.z -= VE.SPEED_FLOW;
                else if (HitBox.Flow == Pole.South) motion.z += VE.SPEED_FLOW;
            }
            else
            {
                h = speed.Fly * Moving.GetHorizontalValue();
                v = (IsSprinting ? speed.FlyFast : speed.Fly) * Moving.GetVerticalValue();
                j = speed.FlyVertical * Moving.GetHeightValue();
                MotionAngle(j, v, h);
            }
            return ToDebug(j, v, h);
        }
        
        /// <summary>
        /// Параметр этапа встать, если true мы пытаемся встать
        /// </summary>
        protected bool isUping = false;
        /// <summary>
        /// Начинаем вставать
        /// </summary>
        public void Uping()
        {
            if (motion.y == 0 && OnGround)
            {
                isUping = true;
            }
        }
        /// <summary>
        /// Встали
        /// </summary>
        public void Uped()
        {
            HitBox.Worth();
            isUping = false;
            SneakingNot();
        }

        /// <summary>
        /// Бежим
        /// </summary>
        public void Sprinting()
        {
            IsSprinting = true;
            Moving.SprintingBegin();
        }
        /// <summary>
        /// Не бежим
        /// </summary>
        public void SprintingNot()
        {
            IsSprinting = false;
            Moving.SprintingCancel();
        }
        /// <summary>
        /// Сидим
        /// </summary>
        public void Sneaking()
        {
            sneaking = EnumSneaking.Sit;
            OnHitBoxChanged();
        }
        /// <summary>
        /// Встаём
        /// </summary>
        public void SneakingNearly() => sneaking = EnumSneaking.GetUp;
        /// <summary>
        /// Не сидим
        /// </summary>
        public void SneakingNot()
        {
            sneaking = EnumSneaking.DonSit;
            OnHitBoxChanged();
        }
        /// <summary>
        /// Присел ли
        /// </summary>
        public bool IsSneaking => sneaking != EnumSneaking.DonSit;
        /// <summary>
        /// Встаём ли
        /// </summary>
        public bool IsSneakingNearly => sneaking == EnumSneaking.GetUp;

        /// <summary>
        /// Нужен рендер
        /// </summary>
        public void Render() => IsRender = true;

        /// <summary>
        /// Рендер сделан
        /// </summary>
        public void RenderDone()
        {
            if (Moving.IsStand()) IsRender = false;
        }

        /// <summary>
        /// Стартовый пакет игрока
        /// </summary>
        protected void InitializePlayer()
        {
            HitBox = new HitBoxEntity(-1, World);
            HitBox.SetSizeHitBox(.6f, 3.7f, 3.4f, .6f, 2.6f, 2.4f);
            HitBox.Worth();
            HitBox.HitBoxChanged += HitBox_Changed;
            HitBox.LookAtChanged += HitBox_LookAtChanged;
            Moving = new MovingKey(true);
            Moving.LookAtChanged += HitBox_LookAtChanged;
        }

        /// <summary>
        /// Получить звуковое положение 
        /// </summary>
        public vec3 GetPositionSound()
        {
            vec3 pos = HitBox.Position - World.Entity.HitBox.Position;
            return pos.rotateYaw(-World.Entity.RotationYaw);
        }

        /// <summary>
        /// Звук перемещения
        /// </summary>
        protected virtual void SoundMoving()
        {
            pauseStepSound = IsSprinting ? 5 : 8;
            BlockBase block = World.GetBlock(HitBox.BlockPosDown);
            World.Audio.PlaySound(block.SoundStep(), GetPositionSound(), 0.16f, 1f);
        }

        /// <summary>
        /// Звук перемещения в воду
        /// </summary>
        protected virtual void SoundWater()
        {
            pauseWaterSound = 30;
            World.Audio.PlaySound("liquid.swim" + (random.Next(4) + 1), GetPositionSound(), 0.2f, 1f);
        }

        
    }
}

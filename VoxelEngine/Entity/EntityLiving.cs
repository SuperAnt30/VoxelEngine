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
        //public vec3 NewPosition { get; protected set; }

        ///// <summary>
        ///// Угол по горизонтали в радианах
        ///// </summary>
        //public float NewRotationYaw { get; protected set; } = 0.0f;
        ///// <summary>
        ///// Угол по вертикали в радианах
        ///// </summary>
        //public float NewRotationPitch { get; protected set; } = 0.0f;

        /// <summary>
        /// Режим перемещения
        /// </summary>
        protected VEMoving mode = VEMoving.Survival;
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
        /// Результат сидеть
        /// </summary>
        protected EnumSneaking sneaking = EnumSneaking.DonSit;
        /// <summary>
        /// Вектор движения
        /// </summary>
        protected vec3 motion;

        public EntityLiving(WorldBase world) : base(world) { }

        public void SetMode(VEMoving mode)
        {
            this.mode = mode;
        }
        /// <summary>
        /// Пытается переместить объект на переданное смещение. 
        /// </summary>
        /// <param name="motion">вектор смещения без скорости</param>
        //public virtual void MoveEntity(vec3 motion)
        //{

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
        //protected void Jump()
        //{

        //}

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

        /// <summary>
        /// Обновление каждый такт (TPS)
        /// </summary>
        public virtual void UpdateTick(long tick)
        {
            if (mode == VEMoving.Survival)
            {
                if (HitBox.IsLegsWater)
                {
                    Block block = World.GetBlock(HitBox.BlockPos);
                    if (block.EBlock == EnumBlock.WaterFlowing)
                    {
                        BlockRender blockRender = new BlockRender(new ChunkRender(World.GetChunk(block.Position), new WorldRender()), block);
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
            if (mode != VEMoving.FreeFlight)
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
        /// Обновление каждый кадр (FPS)
        /// </summary>
        public virtual string UpdateDraw(float time)
        {
            string str = UpdateMoving();

            if (mode == VEMoving.Survival)
            {
                vec3 move = MoveTime(time);
                if (HitBox.CollisionBodyXZ(move) && OnGround)
                {
                    // Авто прыжок
                    motion.y = HitBox.IsLegsWater ? VE.SPEED_WATER_AUTOJAMP : VE.SPEED_AUTOJAMP;
                    vec3 move2 = MoveTime(time);
                    move.y = move2.y;
                }
                CollisionBodyY(move);
            }
            else if (mode == VEMoving.ObstacleFlight)
            {
                vec3 move = MoveTime(time);
                HitBox.CollisionBodyXZ(move);
                CollisionBodyY(move);
            }
            else
            {
                HitBox.SetPos(HitBox.Position + MoveTime(time));
            }

            return str;
        }

        /// <summary>
        /// Коллизия для вектора вертикали
        /// </summary>
        protected void CollisionBodyY(vec3 move)
        {
            if (HitBox.CollisionBodyY(move))
            {
                OnGround = true;
                // надо упереться в блок, чтоб не падать
                HitBox.SetPos(new vec3(HitBox.Position.x, Mth.Floor(HitBox.Position.y), HitBox.Position.z));
                motion.y = 0;
            }
        }

        protected vec3 MoveTime(float time)
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
            if (mode == VEMoving.Survival)
            {
                h = IsSneaking ? VE.SPEED_SNEAKING : VE.SPEED_STEP;
                v = h;
                j = motion.y;

                if (Moving.Height == EnumMovingKey.Minus && OnGround && !IsSneaking)
                {
                    // Присел
                    Sneaking();
                }

                if (Moving.Vertical == EnumMovingKey.Plus && IsSprinting && !IsSneaking)
                {
                    v = VE.SPEED_RUN;
                }
                if ((HitBox.IsDownWater || HitBox.IsLegsWater) && !HitBox.IsEyesWater)
                {
                    // Игрок над водой или частично в воде
                    if (Moving.Height == EnumMovingKey.Plus)
                    {
                        if (HitBox.IsLegsWater)
                        {
                            // Ноги в воде но нет под ногами дна
                            // скорее всего плывёт
                            OnGround = false;
                            j = VE.SPEED_SWIM;
                        }
                        else
                        if (OnGround)
                        {
                            // мы на земле но рядом блок под нами с водой
                            OnGround = false;
                            j = VE.SPEED_JAMP;
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
                    if (Moving.Height != EnumMovingKey.None) j = Moving.GetHeightValue() * VE.SPEED_SWIM;

                    // Если в воде то замедляем в 2,5 раза
                    h *= .4f;
                    v *= .4f;
                }
                else
                {
                    // Прыжок
                    if (Moving.Height == EnumMovingKey.Plus && OnGround)
                    {
                        OnGround = false;
                        j = HitBox.IsLegsWater ? VE.SPEED_WATER_JAMP : VE.SPEED_JAMP;
                    }
                }

                v *= Moving.GetVerticalValue();
                h *= Moving.GetHorizontalValue();

                MotionAngle(j, v, h);

                // Если есть течение то корректируем от смещения
                if (HitBox.Flow == Pole.East) motion.x += VE.SPEED_FLOW;
                else if (HitBox.Flow == Pole.West) motion.x -= VE.SPEED_FLOW;
                else if (HitBox.Flow == Pole.North) motion.z -= VE.SPEED_FLOW;
                else if (HitBox.Flow == Pole.South) motion.z += VE.SPEED_FLOW;

                
            }
            else
            {
                h = VE.SPEED_STEP * Moving.GetHorizontalValue();
                v = Moving.Vertical == EnumMovingKey.Plus ? IsSprinting
                        ? VE.SPEED_FLY_FAST : VE.SPEED_FLY
                    : VE.SPEED_STEP;
                v *= Moving.GetVerticalValue();
                j = VE.SPEED_FLY_VERTICAL * Moving.GetHeightValue();

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
        public void Sprinting() => IsSprinting = true;
        /// <summary>
        /// Не бежим
        /// </summary>
        public void SprintingNot() => IsSprinting = false;
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

    }
}

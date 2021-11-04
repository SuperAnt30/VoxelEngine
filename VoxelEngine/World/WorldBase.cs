using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using VoxelEngine.Audio;
using VoxelEngine.Entity;
using VoxelEngine.Entity.Npc;
using VoxelEngine.Gen;
using VoxelEngine.Gen.Group;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;
using VoxelEngine.Util;
using VoxelEngine.Vxl;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Chk;

namespace VoxelEngine.World
{
    /// <summary>
    /// Объект мира, для работы с данными
    /// </summary>
    public class WorldBase
    {
        /// <summary>
        /// Зерно генерации случайных чисел
        /// </summary>
        public int Seed { get; protected set; } = 2;
        /// <summary>
        /// Чанки
        /// </summary>
        public ChunkProvider ChunkPr { get; protected set; }
        /// <summary>
        /// Регионы
        /// </summary>
        public RegionProvider RegionPr { get; protected set; }
        /// <summary>
        /// Объект шумов
        /// </summary>
        public NoiseStorge Noise { get; protected set; }
        /// <summary>
        /// Когда было сделано сохранение мира
        /// </summary>
        protected long timeSave = 0;
        /// <summary>
        /// Список сущностей
        /// </summary>
        public Hashtable Entities { get; protected set; }
        /// <summary>
        /// Объект сущьности игрока
        /// </summary>
        public EntityLiving Entity { get; protected set; }
        /// <summary>
        /// Объект сущности на который смотрит курсор
        /// </summary>
        public EntityDistance EntityDis { get; protected set; }
        /// <summary>
        /// Объект звуков
        /// </summary>
        public AudioBase Audio { get; protected set; }

        /// <summary>
        /// Таймер для фиксации времени c запуска приложения
        /// </summary>
        protected Stopwatch stopwatch = new Stopwatch();

        public WorldBase()
        {
            stopwatch.Start();
            timeSave = VEC.TickCount;
            ChunkPr = new ChunkProvider(this);
            RegionPr = new RegionProvider(this);
            Noise = new NoiseStorge(this);
            Entities = new Hashtable();
            Entity = new EntityPlayer(this);
            Entity.HitBoxChanged += Entity_HitBoxChanged;
            Entity.LookAtChanged += Entity_HitBoxLookAtChanged;
            Audio = new AudioBase(this);
            Audio.Initialize();
        }

        /// <summary>
        /// Обновить режим игрока
        /// </summary>
        public void UpdateModePlayer()
        {
            Entity.SetMode(VEC.moving);
        }

        protected bool isTickStop = false;

        /// <summary>
        /// Остановить такт, закрытие программы
        /// </summary>
        public void TickStop() => isTickStop = true;

        /// <summary>
        /// Пакет такта в потоке
        /// </summary>
        public void PackageTick()
        {
            Task.Factory.StartNew(() => { Tick(); });
        }

        /// <summary>
        /// Такт
        /// </summary>
        protected void Tick()
        {
            stopwatch.Restart();
            long tick = VEC.TickCount;

            Audio.Tick();

            // для записи
            if (timeSave + 6000 < VEC.TickCount)
            {
                timeSave = tick;
                // Сохраняем миры каждые 5 мин
                RegionPr.RegionsWrite();
            }

            // Чанк где стоит игрок
            vec2i c = OpenGLF.GetInstance().Cam.ChunkPos;
            // Массив спирали чанков
            vec2i[] distSqrt = VES.DistSqrt;
            // По спирале такты чанков близ лежащих к игроку
            for (int i = 0; i < VE.CHUNKS_TICK; i++)
            {
                ChunkBase cm = GetChunk(c.x + distSqrt[i].x, c.y + distSqrt[i].y);
                if (cm != null) cm.Tick(tick);
            }

            // Такты всех мобов
            Hashtable hashtable = (Hashtable)Entities.Clone();
            EntityDistance edNew = new EntityDistance();
            foreach (EntityLiving entity in hashtable.Values)
            {
                float dis = Entity.HitBox.DistanceEyesTo(entity.HitBox.BlockPos);
                if (dis >= VE.ENTITY_DISSPAWN)
                {
                    // дисспавн
                    entity.Kill();
                }
                else
                {
                    EntityDistance ed = RayCrossEntity(entity);
                    if (!ed.IsEmpty && ed.Distance < edNew.Distance)
                    {
                        edNew = ed;
                    }
                    entity.UpdateTick(tick);
                    TickEntity(entity, tick);
                }
            }
            if (EntityDis != edNew)
            {
                EntityDis = edNew;
            }

            long ms = stopwatch.ElapsedMilliseconds;
            if (ms < 50)
            {
                System.Threading.Thread.Sleep(50 - (int)ms);
            }
            if (!isTickStop) OnTicked();
        }

        /// <summary>
        /// Такт конкретной сущьности
        /// </summary>
        protected virtual void TickEntity(EntityLiving entity, long tick) { }

        /// <summary>
        /// Получить блок
        /// </summary>
        public BlockBase GetBlock(vec3i pos)
        {
            if (pos.y >= 0 && pos.y <= 255)
            {
                ChunkBase chunk = GetChunk(pos.x >> 4, pos.z >> 4);
                if (chunk != null)
                {
                    return chunk.GetBlock0(new vec3i(pos.x & 15, pos.y, pos.z & 15));
                }
            }
            return Blocks.GetEmpty(new BlockPos(pos));
        }
        /// <summary>
        /// Получить блок
        /// </summary>
        public BlockBase GetBlock(BlockPos pos)
        {
            return GetBlock(pos.ToVec3i());

           // return Blocks.GetBlock(GetVoxel(pos.ToVec3i()), pos);
        }

        /// <summary>
        /// Получить чанк с кэша//, если его там нет, то сгенерировать его
        /// </summary>
        public ChunkBase GetChunk(int x, int z)
        {
            if (IsChunk(x, z)) return ChunkPr.ProvideChunk(x, z);
            return null;
        }

        /// <summary>
        /// Получить чанк с кэша//, если его там нет, то сгенерировать его
        /// </summary>
        public ChunkBase GetChunk(BlockPos pos)
        {
            return GetChunk(pos.X >> 4, pos.Z >> 4);
        }

        /// <summary>
        /// загружен ли чанк
        /// </summary>
        public bool IsChunk(int x, int z)
        {
            return ChunkPr.IsChunk(x, z);
        }

        /// <summary>
        /// Проверка облости загрузки чанков
        /// </summary>
        public bool IsArea(BlockPos pos, int radius)
        {
            int x0 = (pos.X - radius) >> 4;
            int x1 = (pos.X + radius) >> 4;
            int z0 = (pos.Z - radius) >> 4;
            int z1 = (pos.Z + radius) >> 4;

            if (pos.Y < 0 || pos.Y > 255) return false;

            for (int x = x0; x <= x1; x++)
            {
                for (int z = z0; z <= z1; z++)
                {
                    if (!IsChunk(x, z)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Проверка облости загрузки чанков 3*3 c 3*3 генерацией, для рендера
        /// </summary>
        public bool IsGeterationArea(int chx, int chz, EnumGeterationStatus status)
        {
            int x0 = chx - 1;
            int x1 = chx + 1;
            int z0 = chz - 1;
            int z1 = chz + 1;

            for (int x = x0; x <= x1; x++)
            {
                for (int z = z0; z <= z1; z++)
                {
                    ChunkBase chunk = GetChunk(x, z);
                    if (chunk == null || (int)chunk.PreparationStatus < (int)status)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Проверка облости загрузки чанков 3*3
        /// </summary>
        public bool IsArea(int chx, int chz)
        {
            int x0 = chx - 1;
            int x1 = chx + 1;
            int z0 = chz - 1;
            int z1 = chz + 1;

            for (int x = x0; x <= x1; x++)
            {
                for (int z = z0; z <= z1; z++)
                {
                    if (!IsChunk(x, z)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Очистить кэш чанков
        /// TODO:: может вызвать ошибку изменения массива в цикле другого потока
        /// </summary>
        public void ChunksClear()
        {
            ChunkPr.Clear();
        }

        /// <summary>
        /// Получить звуковое положение 
        /// </summary>
        public vec3 GetPositionSound(BlockPos bpos)
        {
            vec3 pos = bpos.ToVec3() - Entity.HitBox.Position;
            return pos.rotateYaw(-Entity.RotationYaw);
        }

        /// <summary>
        /// Задать блок
        /// </summary>
        /// <param name="newBlock"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public void SetBlockState(BlockBase newBlock, bool notTick)//, int flags)
        {
            // Через поток так как при удалении и меняется освещение, подвисает
            Task.Factory.StartNew(() => { SetBlockState0(newBlock, notTick); });
        }

        public void SetBlock(BlockPos pos, EnumBlock eBlock, byte b)
        {
            ChunkBase chunk = GetChunk(pos);
            if (chunk != null)
            {
                chunk.SetBlockState(pos.X & 15, pos.Y, pos.Z & 15, eBlock);
                chunk.SetParam4bit(pos.X & 15, pos.Y, pos.Z & 15, b);
            }
        }

        protected void SetBlockState0(BlockBase newBlock, bool notTick)//, int flags)
        {
            //if (newBlock.EBlock == EnumBlock.Water) newBlock.Voxel.SetParam4bit(2);
            List<vec2i> p = new List<vec2i>();
            int vx = newBlock.Position.X & 15;
            int vz = newBlock.Position.Z & 15;

            vec2i ch = new vec2i(newBlock.Position.X >> 4, newBlock.Position.Z >> 4);
            ChunkBase chunk = GetChunk(ch.x, ch.y);

            BlockBase blockOld = chunk.GetBlock(newBlock.Position);
            if (blockOld.EBlock == EnumBlock.Door)
            {
                GroupDoor door = new GroupDoor(this, blockOld.Position.ToVec3i());
                door.Light();
                door.Remove(blockOld);
            }
            else
            {
                blockOld = chunk.SetBlockState(newBlock, notTick);
            }

            if (blockOld != null)
            {
                if (notTick)
                {
                    // Звук установленного блока или разрушенного
                    Audio.PlaySound(newBlock.IsAir ? blockOld.SoundBreak() : newBlock.SoundPut(),
                        GetPositionSound(newBlock.Position), 1f, 1f);
                }

                // Проверка высот
                
                chunk.Light.CheckLightSetBlock(
                    newBlock.Position, 
                    newBlock.GetBlockLightOpacity(), 
                    blockOld.GetBlockLightOpacity(), 
                    newBlock.LightValue != blockOld.LightValue
                );

                //CheckLight(newBlock.Position);
                //}
                //else
                //{
                //    // Пометить соседний псевдо чанк на рендер
                //    int cy = newBlock.Position.Y >> 4;
                //    int vy = newBlock.Position.Y & 15;
                //    SetModifiedRender(new vec3i(ch.x, cy, ch.y));
                //    if (cy > 0 && vy == 0) SetModifiedRender(new vec3i(ch.x, cy - 1, ch.y));
                //    if (cy < 15 && vy == 15) SetModifiedRender(new vec3i(ch.x, cy + 1, ch.y));
                //    if (vx == 0)
                //    {
                //        SetModifiedRender(new vec3i(ch.x - 1, cy, ch.y));
                //        if (vz == 0) SetModifiedRender(new vec3i(ch.x - 1, cy, ch.y - 1));
                //        if (vz == 15) SetModifiedRender(new vec3i(ch.x - 1, cy, ch.y + 1));
                //    }
                //    if (vx == 15)
                //    {
                //        SetModifiedRender(new vec3i(ch.x + 1, cy, ch.y));
                //        if (vz == 0) SetModifiedRender(new vec3i(ch.x + 1, cy, ch.y - 1));
                //        if (vz == 15) SetModifiedRender(new vec3i(ch.x + 1, cy, ch.y + 1));
                //    }
                //    if (vz == 0) SetModifiedRender(new vec3i(ch.x, cy, ch.y - 1));
                //    if (vz == 15) SetModifiedRender(new vec3i(ch.x, cy, ch.y + 1));
                //}
                //if ((flags & 2) != 0 && (!this.isRemote || (flags & 4) == 0) && var4.isPopulated())
                //{
                //    this.markBlockForUpdate(pos);
                //}

                //if (!this.isRemote && (flags & 1) != 0)
                //{
                //    this.func_175722_b(pos, blockOld.getBlock());

                //    if (newBlock.hasComparatorInputOverride())
                //    {
                //        this.updateComparatorOutputLevel(pos, newBlock);
                //    }
                //}

                //return p.ToArray();
            }

            OnVoxelChanged(newBlock.Position.ToVec3i(), p.ToArray());// SetBlockState(newBlock));
        }

        /// <summary>
        /// Задать изменение для рендера псевдо чанка
        /// </summary>
        /// <param name="c">x,z чанк, y псевдочанк</param>
        public void SetModifiedRender(vec3i c)
        {
            ChunkBase chunk = GetChunk(c.x, c.z);
            if (chunk != null)
            {
                chunk.StorageArrays[c.y].SetModifiedRender();
                chunk.SetChunkModified();
            }
        }

        /// <summary>
        /// Пересечение луча с сущностью
        /// </summary>
        protected virtual EntityDistance RayCrossEntity(EntityLiving entity) => new EntityDistance();

        /// <summary>
        /// Пересечения лучей с визуализируемой поверхностью
        /// </summary>
        /// <param name="a">точка от куда идёт лучь</param>
        /// <param name="dir">вектор луча</param>
        /// <param name="maxDist">максимальная дистания</param>
        public MovingObjectPosition RayCast(vec3 a, vec3 dir, float maxDist)
        {
            float px = a.x;
            float py = a.y;
            float pz = a.z;

            float dx = dir.x;
            float dy = dir.y;
            float dz = dir.z;

            float t = 0.0f;
            int ix = Mth.Floor(px);
            int iy = Mth.Floor(py);
            int iz = Mth.Floor(pz);

            int stepx = (dx > 0.0f) ? 1 : -1;
            int stepy = (dy > 0.0f) ? 1 : -1;
            int stepz = (dz > 0.0f) ? 1 : -1;

            float infinity = 600f;// std::numeric_limits<float>::infinity();

            float txDelta = (dx == 0.0f) ? infinity : Mth.Abs(1.0f / dx);
            float tyDelta = (dy == 0.0f) ? infinity : Mth.Abs(1.0f / dy);
            float tzDelta = (dz == 0.0f) ? infinity : Mth.Abs(1.0f / dz);

            float xdist = (stepx > 0) ? (ix + 1 - px) : (px - ix);
            float ydist = (stepy > 0) ? (iy + 1 - py) : (py - iy);
            float zdist = (stepz > 0) ? (iz + 1 - pz) : (pz - iz);

            float txMax = (txDelta < infinity) ? txDelta * xdist : infinity;
            float tyMax = (tyDelta < infinity) ? tyDelta * ydist : infinity;
            float tzMax = (tzDelta < infinity) ? tzDelta * zdist : infinity;

            int steppedIndex = -1;

            while (t <= maxDist)
            {
                BlockBase block = GetBlock(new vec3i(ix, iy, iz));
                if (block.CollisionRayTrace(a, dir, maxDist))
                {
                    vec3 end;
                    vec3i norm;
                    vec3i iend;

                    end.x = px + t * dx;
                    end.y = py + t * dy;
                    end.z = pz + t * dz;

                    iend.x = ix;
                    iend.y = iy;
                    iend.z = iz;

                    norm.x = norm.y = norm.z = 0;
                    if (steppedIndex == 0) norm.x = -stepx;
                    if (steppedIndex == 1) norm.y = -stepy;
                    if (steppedIndex == 2) norm.z = -stepz;

                    if (EntityDis != null)
                    {
                        if (t < EntityDis.Distance)
                        {
                            return new MovingObjectPosition(block, end, iend, norm);
                        }
                        return new MovingObjectPosition(EntityDis.Entity);
                    }
                }
                if (txMax < tyMax)
                {
                    if (txMax < tzMax)
                    {
                        ix += stepx;
                        t = txMax;
                        txMax += txDelta;
                        steppedIndex = 0;
                    }
                    else
                    {
                        iz += stepz;
                        t = tzMax;
                        tzMax += tzDelta;
                        steppedIndex = 2;
                    }
                }
                else
                {
                    if (tyMax < tzMax)
                    {
                        iy += stepy;
                        t = tyMax;
                        tyMax += tyDelta;
                        steppedIndex = 1;
                    }
                    else
                    {
                        iz += stepz;
                        t = tzMax;
                        tzMax += tzDelta;
                        steppedIndex = 2;
                    }
                }
            }
            return new MovingObjectPosition();
        }

        /// <summary>
        /// TODO:: Временный спавн моба 
        /// </summary>
        public void AddEntity()
        {
            Camera cam = OpenGLF.GetInstance().Cam;
            MovingObjectPosition moving = RayCast(cam.PosPlus(), cam.Front, VE.MAX_DIST);
            if (moving.IsBlock())
            {
                vec3 v = new vec3(moving.IEnd + moving.Norm) + new vec3(.5f, 0, .5f);

                EntityChicken entity = new EntityChicken(this);
                entity.HitBoxChanged += Entity_HitBoxChanged;
                entity.SetChicken(VEC.EntityIndex, v, cam.Yaw - glm.pi);
                if (Entities.ContainsKey(entity.HitBox.Index))
                {
                    Entities[entity.HitBox.Index] = entity;
                }
                else
                {
                    Entities.Add(entity.HitBox.Index, entity);
                }

                VEC.EntityAdd();
                Debug.Entities = Entities.Count;
            }
            //ModelChicken chicken = new ModelChicken();
            //chicken.Render(entity, 0, 0, 0, 0, 0, VE.UV_SIZE);// 0.12f);

            //if (!chicken.IsBufferEmpty())
            //{
            //    OpenGLF.GetInstance().WorldM.RenderEntity(chicken.Buffer);
            //}
        }

        public virtual void RemoveEntity(EntityLiving entity)
        {
            Entities.Remove(entity.HitBox.Index);
            Debug.Entities = Entities.Count;
        }

        #region  Event

        protected void Entity_HitBoxLookAtChanged(object sender, EventArgs e) => OnLookAtChanged();
        private void Entity_HitBoxChanged(object sender, EntityEventArgs e) => OnHitBoxChanged(e);

        /// <summary>
        /// Событие изменена позиция камеры
        /// </summary>
        public event EventHandler LookAtChanged;
        /// <summary>
        /// Событие изменена позиция камеры
        /// </summary>
        protected void OnLookAtChanged() => LookAtChanged?.Invoke(this, new EventArgs());
        
        /// <summary>
        /// Событие изменён хитбокс сущьности
        /// </summary>
        public event EntityEventHandler HitBoxChanged;
        /// <summary>
        /// Событие изменён хитбокс сущьности
        /// </summary>
        protected void OnHitBoxChanged(EntityEventArgs e) => HitBoxChanged?.Invoke(this, e);

        /// <summary>
        /// Событие изменен воксель
        /// </summary>
        public event VoxelEventHandler VoxelChanged;
        /// <summary>
        /// Событие изменен воксель
        /// </summary>
        protected virtual void OnVoxelChanged(vec3i position, vec2i[] beside) 
            => VoxelChanged?.Invoke(this, new VoxelEventArgs(position, beside));

        /// <summary>
        /// Событие законченна обработка тика
        /// </summary>
        public event EventHandler Ticked;
        /// <summary>
        /// Событие законченна обработка тика
        /// </summary>
        protected void OnTicked() => Ticked?.Invoke(this, new EventArgs());

        #endregion
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using VoxelEngine.Entity;
using VoxelEngine.Entity.Npc;
using VoxelEngine.Gen;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;
using VoxelEngine.Renderer;
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
        /// Таймер для фиксации времени c запуска приложения
        /// </summary>
        protected Stopwatch stopwatch = new Stopwatch();

        public WorldBase()
        {
            stopwatch.Start();
            timeSave = VEC.GetInstance().TickCount;
            ChunkPr = new ChunkProvider(this);
            RegionPr = new RegionProvider(this);
            Noise = new NoiseStorge(this);
            Entities = new Hashtable();
            Entity = new EntityPlayer(this);
            Entity.HitBoxChanged += Entity_HitBoxChanged;
            Entity.LookAtChanged += Entity_HitBoxLookAtChanged;
        }

        /// <summary>
        /// Обновить режим игрока
        /// </summary>
        public void UpdateModePlayer()
        {
            Entity.SetMode(VEC.GetInstance().Moving);
        }


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
        protected virtual void Tick()
        {
            stopwatch.Restart();
            long tick = VEC.GetInstance().TickCount;

            // для записи
            if (timeSave + 6000 < VEC.GetInstance().TickCount)
            {
                timeSave = tick;
                // Сохраняем миры каждые 5 мин
                RegionPr.RegionsWrite();
            }

            // Чанк где стоит игрок
            vec2i c = OpenGLF.GetInstance().Cam.ChunkPos;
            // Массив спирали чанков
            ChunkLoading[] spiral = VES.GetInstance().DistSqrt;
            // По спирале такты чанков близ лежащих к игроку
            for (int i = 0; i < VE.CHUNKS_TICK; i++)
            {
                ChunkBase cm = GetChunk(c.x + spiral[i].X, c.y + spiral[i].Z);
                if (cm != null) cm.Tick(tick);
            }

            // Такты всех мобов
            Hashtable hashtable = (Hashtable)Entities.Clone();
            foreach (EntityLiving entity in hashtable.Values)
            {
                if (Entity.HitBox.DistanceEyesTo(entity.HitBox.BlockPos) >= VE.ENTITY_DISSPAWN)
                {
                    // дисспавн
                    entity.Kill();
                }
                else
                {
                    entity.UpdateTick(tick);
                    TickEntity(entity, tick);
                }
            }

            long ms = stopwatch.ElapsedMilliseconds;
            if (ms < 50)
            {
                System.Threading.Thread.Sleep(50 - (int)ms);
            }
            OnTicked();
        }

        /// <summary>
        /// Такт конкретной сущьности
        /// </summary>
        protected virtual void TickEntity(EntityLiving entity, long tick) { }

        /// <summary>
        /// Получить блок
        /// </summary>
        public Block GetBlock(vec3i pos)
        {
            return Blocks.GetBlock(GetVoxel(pos), new BlockPos(pos));
        }
        /// <summary>
        /// Получить блок
        /// </summary>
        public Block GetBlock(BlockPos pos)
        {
            return Blocks.GetBlock(GetVoxel(pos.ToVec3i()), pos);
        }

        /// <summary>
        /// Получить воксель
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Voxel GetVoxel(vec3i pos)
        {
            if (pos.y < 0 || pos.y > 255)
            {
                return new Voxel();
            }
            ChunkBase chunk = GetChunk(pos.x >> 4, pos.z >> 4);
            if (chunk == null) return new Voxel();
            return chunk.GetVoxel(pos.x & 15, pos.y, pos.z & 15);
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
        /// Загружен ли чанк, если нет тут же загружаем
        /// </summary>
        //public bool IsChunkLoaded(int x, int z)
        //{
        //    if (IsChunk(x, z)) return true;

        //    var outer = Task.Factory.StartNew(() => { ChunkPr.LoadChunk(x, z); });
        //    //ChunkPr.LoadChunk(x, z);
        //    //Debag.GetInstance().CacheChunk = ChunkPr.Count();
        //    outer.Wait();
        //    return false;
        //}

        /// <summary>
        /// загружен ли чанк
        /// </summary>
        public bool IsChunk(int x, int z)
        {
            return ChunkPr.IsChunk(x, z);
        }

        /// <summary>
        /// Загрузка области чанков 3*3
        /// </summary>
        public void AreaLoaded(vec2i pos)
        {
            int x0 = pos.x - 1;
            int x1 = pos.x + 1;
            int z0 = pos.y - 1;
            int z1 = pos.y + 1;

            for (int x = x0; x <= x1; x++)
            {
                for (int z = z0; z <= z1; z++)
                {
                    RegionPr.RegionSet(x, z);
                    if (!IsChunk(x, z))
                    {
                        ChunkPr.LoadChunk(x, z);
                    }
                }
            }
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

            if (pos.Y < 1 || pos.Y > 255) return false;

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
        /// Проверка облости загрузки чанков
        /// </summary>
        public bool IsArea(vec2i ch, int radius)
        {
            int x0 = ch.x - radius;
            int x1 = ch.x + radius;
            int z0 = ch.y - radius;
            int z1 = ch.y + radius;

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
        /// TODO::#
        /// Не используется
        /// </summary>
        //public BlockPos GetHorizon(BlockPos pos)
        //{
        //    int y = 0;

        //    if (pos.X >= -30000000 && pos.Z >= -30000000 && pos.X < 30000000 && pos.Z < 30000000)
        //    {
        //        if (IsChunkLoaded(pos.X >> 4, pos.Z >> 4))
        //        {
        //            y = GetChunk(pos.X >> 4, pos.Z >> 4).GetHeight(pos.X & 15, pos.Z & 15);
        //        }
        //    }
        //    else
        //    {
        //        y = 64;
        //    }

        //    return new BlockPos(pos.X, y, pos.Z);
        //}

        ///// <summary>
        ///// Получает наименьшую высоту участка, на который падает прямой солнечный свет.
        ///// Не используется
        ///// </summary>
        //public int GetChunksLowestHorizon(int x, int z)
        //{
        //    if (x >= -30000000 && z >= -30000000 && x < 30000000 && z < 30000000)
        //    {
        //        if (!IsChunkLoaded(x >> 4, z >> 4))
        //        {
        //            return GetChunk(x >> 4, z >> 4).GetLowestHeight();
        //        }
        //    }
        //    return 64;
        //}


        /// <summary>
        /// Очистить кэш чанков
        /// TODO:: может вызвать ошибку изменения массива в цикле другого потока
        /// </summary>
        public void ChunksClear()
        {
            ChunkPr.Clear();
        }

        /// <summary>
        /// Запуск  Удалить дальние чанки
        /// </summary>
        public void PackageCleaning()
        {
            Task.Factory.StartNew(() => { Cleaning(); });
        }



        /// <summary>
        /// Удалить дальние чанки из массива кэша и регионы
        /// </summary>
        public void Cleaning()
        {
            vec2i positionCam = OpenGLF.GetInstance().Cam.ChunkPos;
            List<vec2i> chunks = new List<vec2i>();
            
            // дальность чанков с учётом кэша
            int visiblityCache = VE.CHUNK_VISIBILITY + 4;
            int xMin = positionCam.x - visiblityCache;
            int xMax = positionCam.x + visiblityCache;
            int zMin = positionCam.y - visiblityCache;
            int zMax = positionCam.y + visiblityCache;
            // Собираем массив чанков которые уже не попадают в видимость
            foreach (ChunkBase cr in ChunkPr.Values)
            {
                if (cr.X < xMin || cr.X > xMax || cr.Z < zMin || cr.Z > zMax)
                {
                    chunks.Add(new vec2i(cr.X, cr.Z));
                }
            }
            // Удаляем
            if (chunks.Count > 0)
            {
                foreach (vec2i key in chunks)
                {
                    ChunkPr.UnloadChunk(key.x, key.y);
                }
            }
            Debug.GetInstance().CacheChunk = ChunkPr.Count();

            List<vec2i> regions = new List<vec2i>();
            foreach (RegionBinary rf in RegionPr.Values)
            {
                if (rf.X < xMin >> 5 || rf.X > xMax >> 5 || rf.Z < zMin >> 5 || rf.Z > zMax >> 5)
                {
                    regions.Add(new vec2i(rf.X, rf.Z));
                }
            }
            // Удаляем
            if (regions.Count > 0)
            {
                foreach (vec2i key in regions)
                {
                    RegionPr.RegionRemove(key.x, key.y);
                }
            }

            OnCleaned();
        }

        /// <summary>
        /// Может ли видеть небо (CanSeeSky)
        /// </summary>
        public bool IsAgainstSky(BlockPos pos)
        {
            ChunkBase chunk = GetChunk(pos);
            if (chunk == null) return false;
            return chunk.CanSeeSky(pos);
        }

        /// <summary>
        /// Задать блок
        /// </summary>
        /// <param name="newBlock"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public void SetBlockState(Block newBlock, bool notTick)//, int flags)
        {
            //if (newBlock.EBlock == EnumBlock.Water) newBlock.Voxel.SetParam4bit(2);
            List<vec2i> p = new List<vec2i>();
            int vx = newBlock.Position.X & 15;
            int vz = newBlock.Position.Z & 15;

            vec2i ch = new vec2i(newBlock.Position.X >> 4, newBlock.Position.Z >> 4);
            ChunkBase chunk = GetChunk(ch.x, ch.y);
            
            Block blockOld = chunk.SetBlockState(newBlock, notTick);

            if (blockOld != null)
            {
                if (newBlock.GetBlockLightOpacity() != blockOld.GetBlockLightOpacity() || newBlock.LightValue != blockOld.LightValue)
                {
                    CheckLight(newBlock.Position);
                }

                // Пометить соседний псевдо чанк на рендер
                int cy = newBlock.Position.Y >> 4;
                int vy = newBlock.Position.Y & 15;
                if (cy > 0 && vy == 0) SetModifiedRender(new vec3i(ch.x, cy - 1, ch.y));
                if (cy < 15 && vy == 15) SetModifiedRender(new vec3i(ch.x, cy + 1, ch.y));
                if (vx == 0)
                {
                    SetModifiedRender(new vec3i(ch.x - 1, cy, ch.y));
                    if (vz == 0) SetModifiedRender(new vec3i(ch.x - 1, cy, ch.y - 1));
                    if (vz == 15) SetModifiedRender(new vec3i(ch.x - 1, cy, ch.y + 1));
                }
                if (vx == 15)
                {
                    SetModifiedRender(new vec3i(ch.x + 1, cy, ch.y));
                    if (vz == 0) SetModifiedRender(new vec3i(ch.x + 1, cy, ch.y - 1));
                    if (vz == 15) SetModifiedRender(new vec3i(ch.x + 1, cy, ch.y + 1));
                }
                if (vz == 0) SetModifiedRender(new vec3i(ch.x, cy, ch.y - 1));
                if (vz == 15) SetModifiedRender(new vec3i(ch.x, cy, ch.y + 1));


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
        protected void SetModifiedRender(vec3i c)
        {
            ChunkBase chunk = GetChunk(c.x, c.z);
            if (chunk != null)
            {
                chunk.SetChunkModified();
                chunk.StorageArrays[c.y].SetModifiedRender();
            }
        }

        #region Light minecraft

        /// <summary>
        /// возвращает уровень яркости который будет от соседних блоков
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private int func_175638_a(BlockPos pos, EnumSkyBlock type)
        {
            if (type == EnumSkyBlock.Sky && IsAgainstSky(pos))
            {
                return 15;
            }
            else
            {
                Block block = GetBlock(pos);
                // Количество излучаемого света
                int light = type == EnumSkyBlock.Sky ? 0 : block.LightValue;

                // Сколько света вычитается для прохождения этого блока
                int opacity = block.GetBlockLightOpacity();

                if (opacity >= 15 && block.LightValue > 0)
                {
                    opacity = 1;
                }

                if (opacity < 1)
                {
                    opacity = 1;
                }

                if (opacity >= 15)
                {
                    return 0;
                }
                else
                if (light >= 14)
                {
                    return light;
                }
                else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        BlockPos pos2 = pos.Offset((Pole)i);// new BlockPos(pos.ToVec3i() + EnumFacing.DirectionVec((Pole)i));
                        int var11 = (GetLightFor(type, pos2) - opacity);

                        if (var11 > light)
                        {
                            light = var11;
                        }

                        if (light >= 14)
                        {
                            return light;// + 1);
                        }
                    }
                    return light;
                }
            }
        }

        public bool CheckLight(BlockPos pos)
        {
            bool var2 = false;
            var2 |= CheckLightFor(EnumSkyBlock.Sky, pos);
            var2 |= CheckLightFor(EnumSkyBlock.Block, pos);
            return var2;
        }

        

        /// <summary>
        /// Генерация освещения блока
        /// </summary>
        public bool CheckLightFor(EnumSkyBlock type, BlockPos pos)
        {
            // Проверка загруженности чанков в области 
            if (!IsArea(pos, 17))
            {
                return false;
            }
            else
            {
                uint[] lightUpdateBlockList = new uint[32768];

                int var3 = 0;
                int var4 = 0;
                //this.theProfiler.startSection("getBrightness");
                // var5 возвращает уровень яркости тикущего блока
                int lightVox = GetLightFor(type, pos);
                // var6 возвращает уровень яркости который будет от соседних блоков
                int lightVoxs = func_175638_a(pos, type);

                int x = pos.X;
                int y = pos.Y;
                int z = pos.Z;
                uint var10;
                int x2;
                int y2;
                int z2;
                int var16;
                int x3;
                int y3;
                int z3;

                if (lightVoxs > lightVox)
                {
                    lightUpdateBlockList[var4++] = 133152;
                }
                else if (lightVoxs < lightVox)
                {
                    lightUpdateBlockList[var4++] = (uint)(133152 | lightVox << 18);

                    while (var3 < var4)
                    {
                        var10 = lightUpdateBlockList[var3++];
                        x2 = (int)((var10 & 63) - 32 + x);
                        y2 = (int)((var10 >> 6 & 63) - 32 + y);
                        z2 = (int)((var10 >> 12 & 63) - 32 + z);
                        int var14 = (int)(var10 >> 18 & 15);
                        BlockPos pos2 = new BlockPos(x2, y2, z2);
                        var16 = GetLightFor(type, pos2);

                        if (var16 == var14)
                        {
                            SetLightFor(type, pos2, 0);

                            if (var14 > 0)
                            {
                                x3 = Mth.Abs(x2 - x);
                                y3 = Mth.Abs(y2 - y);
                                z3 = Mth.Abs(z2 - z);

                                if (x3 + y3 + z3 < 17)
                                {
                                    for (int var22 = 0; var22 < 6; var22++)
                                    {
                                        vec3i v = EnumFacing.DirectionVec((Pole)var22);
                                        int x4 = x2 + v.x;
                                        int y4 = y2 + v.y;
                                        int z4 = z2 + v.z;

                                        BlockPos pos3 = new BlockPos(x4, y4, z4);
                                        int var28 = Mth.Max(1, GetBlock(pos3).GetBlockLightOpacity());
                                        var16 = GetLightFor(type, pos3);

                                        if (var16 == var14 - var28 && var4 < lightUpdateBlockList.Length)
                                        {
                                            lightUpdateBlockList[var4++] = (uint)(x4 - x + 32 | y4 - y + 32 << 6 | z4 - z + 32 << 12 | var14 - var28 << 18);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var3 = 0;
                }

                //this.theProfiler.endSection();
                //this.theProfiler.startSection("checkedPosition < toCheckCount");

                while (var3 < var4)
                {
                    var10 = lightUpdateBlockList[var3++];
                    x2 = (int)((var10 & 63) - 32 + x);
                    y2 = (int)((var10 >> 6 & 63) - 32 + y);
                    z2 = (int)((var10 >> 12 & 63) - 32 + z);
                    BlockPos pos3 = new BlockPos(x2, y2, z2);
                    int var30 = GetLightFor(type, pos3);
                    var16 = func_175638_a(pos3, type);

                    if (var16 != var30)
                    {
                        SetLightFor(type, pos3, (byte)var16);

                        if (var16 > var30)
                        {
                            x3 = Mth.Abs(x2 - x);
                            y3 = Mth.Abs(y2 - y);
                            z3 = Mth.Abs(z2 - z);
                            bool var31 = var4 < lightUpdateBlockList.Length - 6;

                            //BlockPos pos2 = pos.Offset((Pole)i);
                            if (x3 + y3 + z3 < 17 && var31)
                            {
                                if (GetLightFor(type, pos3.Offset(Pole.West)) < var16)
                                {
                                    lightUpdateBlockList[var4++] = (uint)(x2 - 1 - x + 32 + (y2 - y + 32 << 6) + (z2 - z + 32 << 12));
                                }

                                if (GetLightFor(type, pos3.Offset(Pole.East)) < var16)
                                {
                                    lightUpdateBlockList[var4++] = (uint)(x2 + 1 - x + 32 + (y2 - y + 32 << 6) + (z2 - z + 32 << 12));
                                }

                                if (GetLightFor(type, pos3.Offset(Pole.Down)) < var16)
                                {
                                    lightUpdateBlockList[var4++] = (uint)(x2 - x + 32 + (y2 - 1 - y + 32 << 6) + (z2 - z + 32 << 12));
                                }

                                if (GetLightFor(type, pos3.Offset(Pole.Up)) < var16)
                                {
                                    lightUpdateBlockList[var4++] = (uint)(x2 - x + 32 + (y2 + 1 - y + 32 << 6) + (z2 - z + 32 << 12));
                                }

                                if (GetLightFor(type, pos3.Offset(Pole.North)) < var16)
                                {
                                    lightUpdateBlockList[var4++] = (uint)(x2 - x + 32 + (y2 - y + 32 << 6) + (z2 - 1 - z + 32 << 12));
                                }

                                if (GetLightFor(type, pos3.Offset(Pole.South)) < var16)
                                {
                                    lightUpdateBlockList[var4++] = (uint)(x2 - x + 32 + (y2 - y + 32 << 6) + (z2 + 1 - z + 32 << 12));
                                }
                            }
                        }
                    }
                }

                // this.theProfiler.endSection();
                return true;
            }
        }


        /// <summary>
        /// Получить уровень яркости тикущего блока
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public byte GetLightFor(EnumSkyBlock type, BlockPos pos)
        {
            if (pos.Y < 0) pos = new BlockPos(pos.X, 0, pos.Z);
            ChunkBase chunk = GetChunk(pos.X >> 4, pos.Z >> 4);
            if (chunk == null) return (byte)type;
            return chunk.GetLightFor(pos.X & 15, pos.Y, pos.Z & 15, type);
        }

        /// <summary>
        /// Задать уровень яркости тикущего блока
        /// </summary>
        public void SetLightFor(EnumSkyBlock type, BlockPos pos, byte lightValue)
        {
            if (pos.Y < 0) pos = new BlockPos(pos.X, 0, pos.Z);
            ChunkBase chunk = GetChunk(pos.X >> 4, pos.Z >> 4);
            if (chunk != null) chunk.SetLightFor(pos.X & 15, pos.Y, pos.Z & 15, type, lightValue);
        }

        /// <summary>
        /// отмечает вертикальную линию блоков как тёмную
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void MarkBlocksDirtyVertical(int x1, int z1, int y1, int y2)
        {
            int var5;

            if (y1 > y2)
            {
                var5 = y2;
                y2 = y1;
                y1 = var5;
            }

            //if (!this.provider.getHasNoSky())
            {
                for (var5 = y1; var5 <= y2; ++var5)
                {
                    CheckLightFor(EnumSkyBlock.Sky, new BlockPos(x1, var5, z1));
                }
            }

            // обновить сетку
            //this.markBlockRangeForRenderUpdate(x1, x2, z1, x1, y2, z1);
        }

        #endregion

        /// <summary>
        /// Пересечения лучей с визуализируемой поверхностью
        /// </summary>
        /// <param name="a">точка от куда идёт лучь</param>
        /// <param name="dir">вектор луча</param>
        /// <param name="maxDist">максимальная дистания</param>
        /// <param name="end">координата пересечения</param>
        /// <param name="norm">нормаль стороны на какую смотрим блока</param>
        /// <param name="iend">позиция ближайшего блока</param>
        /// <returns></returns>
        public Block RayCast(vec3 a, vec3 dir, float maxDist, out vec3 end, out vec3i norm, out vec3i iend)
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
                Block block = GetBlock(new vec3i(ix, iy, iz));
                if (block.IsAction)
                {
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
                    return block;
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
            iend.x = ix;
            iend.y = iy;
            iend.z = iz;

            end.x = px + t * dx;
            end.y = py + t * dy;
            end.z = pz + t * dz;
            norm.x = norm.y = norm.z = 0;
            return null;// new Block();
        }

        /// <summary>
        /// TODO:: Временный спавн моба 
        /// </summary>
        public void AddEntity()
        {
            Camera cam = OpenGLF.GetInstance().Cam;
            VEC config = VEC.GetInstance();
            Block block = RayCast(cam.PosPlus(), cam.Front, 10.0f, out vec3 end, out vec3i norm, out vec3i iend);
            vec3 v = new vec3(iend + norm) + new vec3(.5f, 0, .5f);

            EntityChicken entity = new EntityChicken(this);
            entity.HitBoxChanged += Entity_HitBoxChanged;
            entity.SetChicken(config.EntityIndex, v, cam.Yaw - glm.pi);
            if (Entities.ContainsKey(entity.HitBox.Index))
            {
                Entities[entity.HitBox.Index] = entity;
            }
            else
            {
                Entities.Add(entity.HitBox.Index, entity);
            }

            config.EntityAdd();
            Debug.GetInstance().Entities = Entities.Count;
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
            Debug.GetInstance().Entities = Entities.Count;
        }



        #region  Event

        protected void Entity_HitBoxLookAtChanged(object sender, EventArgs e)
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

        private void Entity_HitBoxChanged(object sender, EntityEventArgs e)
        {
            OnHitBoxChanged(e);
        }

        /// <summary>
        /// Событие изменён хитбокс сущьности
        /// </summary>
        public event EntityEventHandler HitBoxChanged;
        /// <summary>
        /// Событие изменён хитбокс сущьности
        /// </summary>
        protected void OnHitBoxChanged(EntityEventArgs e)
        {
            HitBoxChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Событие изменен воксель
        /// </summary>
        public event VoxelEventHandler VoxelChanged;

        /// <summary>
        /// изменен воксель
        /// </summary>
        protected virtual void OnVoxelChanged(vec3i position, vec2i[] beside)
        {
            VoxelChanged?.Invoke(this, new VoxelEventArgs(position, beside));
        }

        /// <summary>
        /// Событие законченной чистки
        /// </summary>
        public event EventHandler Cleaned;

        protected void OnCleaned()
        {
            Cleaned?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Событие законченна обработка тика
        /// </summary>
        public event EventHandler Ticked;

        protected void OnTicked()
        {
            Ticked?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}

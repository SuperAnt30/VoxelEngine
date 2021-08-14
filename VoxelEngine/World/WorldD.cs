using System.Collections.Generic;
using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World.Chunk;

namespace VoxelEngine.World
{
    /// <summary>
    /// Объект мира, для работы с данными
    /// </summary>
    public class WorldD
    {
        /// <summary>
        /// Чанки
        /// </summary>
        public ChunkProvider ChunkPr { get; protected set; }
        /// <summary>
        /// Регионы
        /// </summary>
        public RegionProvider RegionPr { get; protected set; }
        /// <summary>
        /// Когда было сделано сохранение мира
        /// </summary>
        protected long timeSave = 0;

        /// <summary>
        /// заполнены кусками, которые находятся в пределах 9 кусков от любого игрока
        /// </summary>
        //protected Set activeChunkSet = Sets.newHashSet();

        public WorldD()
        {
            timeSave = Debag.GetInstance().TickCount;
            ChunkPr = new ChunkProvider(this);
            RegionPr = new RegionProvider(this);
        }

        public void Tick()
        {
            // для записи
            if (timeSave + 6000 < Debag.GetInstance().TickCount)
            {
                timeSave = Debag.GetInstance().TickCount;
                // Сохраняем миры каждые 5 мин
                RegionPr.RegionsWrite();
                //RegionsWrite();
            }
        }

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
            ChunkD chunk = GetChunk(pos.x >> 4, pos.z >> 4);
            if (chunk == null) return new Voxel();
            return chunk.GetVoxel(pos.x & 15, pos.y, pos.z & 15);
        }

        /// <summary>
        /// Получить чанк с кэша//, если его там нет, то сгенерировать его
        /// </summary>
        public ChunkD GetChunk(int x, int z)
        {
            if (IsChunk(x, z)) return ChunkPr.ProvideChunk(x, z);
            return null;
        }

        /// <summary>
        /// Получить чанк с кэша//, если его там нет, то сгенерировать его
        /// </summary>
        public ChunkD GetChunk(BlockPos pos)
        {
            return GetChunk(pos.X >> 4, pos.Z >> 4);
        }

        /// <summary>
        /// Загружен ли чанк, если нет тут же загружаем
        /// </summary>
        public bool IsChunkLoaded(int x, int z)
        {
            if (IsChunk(x, z)) return true;
            ChunkPr.LoadChunk(x, z);
            Debag.GetInstance().CacheChunk = ChunkPr.Count();
            return false;
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
        protected bool IsAreaLoaded(int x0, int y0, int z0, int x1, int y1, int z1, bool isLoad)
        {
            if (y0 >= 0 && y1 < 256)
            {
                if (!isLoad)
                {
                    x0 >>= 4;
                    z0 >>= 4;
                    x1 >>= 4;
                    z1 >>= 4;
                }

                for (int x = x0; x <= x1; x++)
                {
                    for (int z = z0; z <= z1; z++)
                    {
                        if (isLoad)
                        {
                            if (!IsChunkLoaded(x, z)) return false;
                        }
                        else
                        {
                            if (!IsChunk(x, z)) return false;
                        }
                    }
                }
                return true;

            }
            return false;
        }

        /// <summary>
        /// Проверка облости загрузки чанков с её загрузкой
        /// </summary>
        public bool IsAreaLoaded(vec2i pos, int radius)
        {
            return IsAreaLoaded(
                pos.x - radius, 64, pos.y - radius,
                pos.x + radius, 64, pos.y + radius, true);
        }

        /// <summary>
        /// Проверка облости загрузки чанков
        /// </summary>
        public bool IsArea(BlockPos pos, int radius)
        {
            return IsAreaLoaded(
                pos.X - radius, pos.Y - radius, pos.Z - radius,
                pos.X + radius, pos.Y + radius, pos.Z + radius, false);
        }

        /// <summary>
        /// Очистить кэш чанков
        /// </summary>
        public void ChunksClear()
        {
            ChunkPr.Clear();
        }

        /// <summary>
        /// Удалить дальние чанки из массива кэша сеток
        /// </summary>
        public void RemoveAway(vec2i positionCam)
        {
            List<vec2i> vs = new List<vec2i>();
            // дальность чанков с учётом кэша
            int visiblityCache = VE.CHUNK_VISIBILITY + 4;
            int xMin = positionCam.x - visiblityCache;
            int xMax = positionCam.x + visiblityCache;
            int zMin = positionCam.y - visiblityCache;
            int zMax = positionCam.y + visiblityCache;
            // Собираем массив чанков которые уже не попадают в видимость
            foreach (ChunkD cr in ChunkPr.Values)
            {
                if (cr.X <= xMin || cr.X >= xMax || cr.Z <= zMin || cr.Z >= zMax)
                {
                    vs.Add(new vec2i(cr.X, cr.Z));
                }
            }

            // Удаляем
            if (vs.Count > 0)
            {
                foreach (vec2i key in vs)
                {
                    ChunkPr.UnloadChunk(key.x, key.y);
                }
            }

            Debag.GetInstance().CacheChunk = ChunkPr.Count();
        }

        /// <summary>
        /// Может ли видеть небо (CanSeeSky)
        /// </summary>
        public bool IsAgainstSky(BlockPos pos)
        {
            ChunkD chunk = GetChunk(pos);
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

            ChunkD chunk = GetChunk(newBlock.Position.X >> 4, newBlock.Position.Z >> 4);
            Block blockOld = chunk.SetBlockState(newBlock, notTick);

            if (blockOld != null)
            {
                if (newBlock.GetBlockLightOpacity() != blockOld.GetBlockLightOpacity() || newBlock.LightValue != blockOld.LightValue)
                {
                    //this.theProfiler.startSection("checkLight");
                    CheckLight(newBlock.Position);
                    p.AddRange(new vec2i[] {
                        new vec2i(-1, -1), new vec2i(-1, 0), new vec2i(-1, 1),
                        new vec2i(0, -1), new vec2i(0, 1),
                        new vec2i(1, -1), new vec2i(1, 0), new vec2i(1, 1)
                    });
                    //this.theProfiler.endSection();
                }
                else
                {
                    if (vx == 0) p.Add(new vec2i(-1, 0));
                    if (vz == 0) p.Add(new vec2i(0, -1));
                    if (vx == 15) p.Add(new vec2i(1, 0));
                    if (vz == 15) p.Add(new vec2i(0, 1));
                }

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
            ChunkD chunk = GetChunk(pos.X >> 4, pos.Z >> 4);
            if (chunk == null) return (byte)type;
            return chunk.GetLightFor(pos.X & 15, pos.Y, pos.Z & 15, type);
        }

        /// <summary>
        /// Задать уровень яркости тикущего блока
        /// </summary>
        public void SetLightFor(EnumSkyBlock type, BlockPos pos, byte lightValue)
        {
            if (pos.Y < 0) pos = new BlockPos(pos.X, 0, pos.Z);
            ChunkD chunk = GetChunk(pos.X >> 4, pos.Z >> 4);
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
            return new Block();
        }

        /// <summary>
        /// Событие изменен воксель
        /// </summary>
        public event VoxelEventHandler VoxelChanged;

        /// <summary>
        /// изменен воксель
        /// </summary>
        protected void OnVoxelChanged(vec3i position, vec2i[] beside)
        {
            VoxelChanged?.Invoke(this, new VoxelEventArgs(position, beside));
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World.Blk;

namespace VoxelEngine.World.Chk.Light
{
    /// <summary>
    /// Алгоритмы обработки освещения
    /// </summary>
    public class WorkingLight : ChunkHeir
    {
        protected Stopwatch stopwatch = new Stopwatch();
        protected RangeModified modified;
        /// <summary>
        /// Массив кэша
        /// </summary>
        protected byte[,,] cacheList;
        protected bool[,,] cacheListD;
        /// <summary>
        /// основная коллекция
        /// </summary>
        protected List<LightStruct> list;
        /// <summary>
        /// Впомогательная коллекция
        /// </summary>
        protected List<LightStruct> listCache;

        public WorkingLight(ChunkBase chunk, BlockPos pos) : base(chunk)
        {
            modified = new RangeModified(chunk.World, pos);
            stopwatch.Start();
            Debug.CountSetBlockLight = 0;
        }
        public WorkingLight(ChunkBase chunk) : base(chunk)
        {
            modified = new RangeModified(chunk.World);
            stopwatch.Start();
        }

        /// <summary>
        /// Запустить запрос рендера
        /// </summary>
        public void ModifiedRender()
        {
            Debug.TimeSetBlockLight = stopwatch.ElapsedMilliseconds;
            modified.ModifiedRender();
        }

        #region SetGetLight

        /// <summary>
        /// Получить уровень яркости тикущего блока, глобальные  координаты
        /// </summary>
        /// <param name="type">тип света</param>
        /// <param name="pos">позиция блока</param>
        /// <returns>яркость</returns>
        public byte GetLight(BlockPos pos, EnumSkyBlock type)
        {
            if (pos.Y < 0) pos = new BlockPos(pos.X, 0, pos.Z);
            ChunkBase chunk = World.GetChunk(pos.X >> 4, pos.Z >> 4);
            if (chunk == null) return (byte)type;
            return chunk.GetLightFor(pos.X & 15, pos.Y, pos.Z & 15, type);
        }

        /// <summary>
        /// Задать уровень яркости тикущего блока
        /// </summary>
        /// <param name="type">тип света</param>
        /// <param name="pos">позиция блока</param>
        /// <param name="lightValue">яркость 0-15</param>
        public void SetLight(BlockPos pos, EnumSkyBlock type, int lightValue)
        {
            if (pos.Y < 0) pos = new BlockPos(pos.X, 0, pos.Z);
            ChunkBase chunk = World.GetChunk(pos.X >> 4, pos.Z >> 4);
            if (chunk != null)
            {
                chunk.SetLightFor(pos.X & 15, pos.Y, pos.Z & 15, type, (byte)lightValue);
                Debug.CountSetBlockLight++;
            }
            // зафиксировать диапазон
            modified.BlockModify(pos);
        }

        /// <summary>
        /// Задать уровень яркости тикущего блока с проверкой небесного неба
        /// </summary>
        /// <param name="type">тип света</param>
        /// <param name="pos">позиция блока</param>
        /// <param name="lightValue">яркость 0-15</param>
        protected void SetLightCheckSky(BlockPos pos, EnumSkyBlock type, int light)
        {
            if (type == EnumSkyBlock.Sky && IsAgainstSky(pos))
            {
                light = 15;
            }
            SetLight(pos, type, light);
        }

        /// <summary>
        /// Возвращает уровень яркости блока, анализируя соседние блоки
        /// </summary>
        /// <param name="pos">позиция</param>
        /// <param name="type">тип света</param>
        /// <returns>яркость</returns>
        private int LevelBright(BlockPos pos, EnumSkyBlock type)
        {
            if (type == EnumSkyBlock.Sky && IsAgainstSky(pos))
            {
                // Если небо, и видим небо, яркость максимальная
                return 15;
            }
            BlockBase block = World.GetBlock(pos);
            // Количество излучаемого света
            int light = type == EnumSkyBlock.Sky ? 0 : block.LightValue;

            // Сколько света вычитается для прохождения этого блока
            int opacity = block.GetBlockLightOpacity();
            if (opacity >= 15 && block.LightValue > 0) opacity = 1;
            if (opacity < 1) opacity = 1;

            // Если блок не проводит свет, значит темно
            if (opacity >= 15) return 0;
            // Если блок яркий выводим значение
            if (light >= 14) return light;

            // обрабатываем соседние блоки, вдруг рядом плафон ярче, чтоб не затемнить
            for (int i = 0; i < 6; i++)
            {
                BlockPos pos2 = pos.Offset((Pole)i);
                int lightNew = GetLight(pos2, type) - opacity;
                // Если соседний блок ярче текущего блока
                if (lightNew > light) light = lightNew;
                // Если блок яркий выводим значение
                if (light >= 14) return light;
            }
            return light;
        }

        /// <summary>
        /// Может ли видеть небо
        /// </summary>
        private bool IsAgainstSky(BlockPos pos)
        {
            ChunkBase chunk = World.GetChunk(pos);
            if (chunk == null) return false;
            return chunk.Light.IsAgainstSky(pos);
        }

        /// <summary>
        /// зафиксировать диапазон
        /// </summary>
        /// <param name="pos">позиция блока</param>
        public void BlockModify(BlockPos pos)
        {
            modified.BlockModify(pos);
        }

        #endregion

        /// <summary>
        /// Проверяем освещение блока и неба
        /// </summary>
        /// <param name="pos">позиция блока</param>
        /// <param name="isSky">надо ли проверять небо</param>
        public void CheckLight(BlockPos pos, bool isSky)
        {
            if (isSky) CheckLightFor(pos.X, pos.Y, pos.Y, pos.Z);
            CheckLightFor(pos.X, pos.Y, 0, pos.Z);
        }

        /// <summary>
        /// Осветляем за счёт блока или неба при старте
        /// </summary>
        /// <param name="x">глобальная X</param>
        /// <param name="yDown">нижняя точка Y</param>
        /// <param name="yUp">Если равен 0, то проверка блоков, если больше, то это верхняя позиция столба, для проверки неба</param>
        /// <param name="z">глобальная Z</param>
        public void CheckLightFor(int x, int yDown, int yUp, int z)
        {
            EnumSkyBlock type = yUp == 0 ? EnumSkyBlock.Block : EnumSkyBlock.Sky;

            BlockPos pos = new BlockPos(x, yDown, z);
            // тикущаяя яркость
            int lightVox = GetLight(pos, type);
            // Планируемая световая яркость
            int light = LevelBright(pos, type);

            if (lightVox < light || lightVox == 15)
            {
                // Осветлить
                BrighterLightFor(pos, type, (byte)light, yUp);
                // Тест столба для визуализации ввиде стекла
                //for (int y = pos.Y; y <= yUp; y++) World.SetBlock(new BlockPos(pos.X, y, pos.Z), EnumBlock.Glass, 0);
            }
            else if (lightVox > light && lightVox > 0)
            {
                // Затемнить
                List<LightStruct> list = DarkenLightFor(pos, type, (byte)lightVox, yUp);
                // Возврат осветления при затемнении
                foreach (LightStruct c in list)
                {
                    BlockPos pos2 = new BlockPos(c.Pos + c.Vec);
                    BrighterLightFor(pos2, type, c.Light, pos2.Y);
                    // Тест столба для визуализации ввиде стекла
                    //World.SetBlock(pos2, EnumBlock.Glass, c.Light);
                }
            }
        }

        /// <summary>
        /// Затемняем неба
        /// </summary>
        /// <param name="x">глобальная X</param>
        /// <param name="yDown">нижняя точка Y</param>
        /// <param name="yUp">Если равен 0, то проверка блоков, если больше, то это верхняя позиция столба, для проверки неба</param>
        /// <param name="z">глобальная Z</param>
        public void CheckDarkenLightFor(int x, int yDown, int yUp, int z)
        {
            // Тест столба для визуализации ввиде стекла
            //for (int y = yDown; y <= yUp; y++) Chunk.SetBlockState(x & 15, y, z & 15, EnumBlock.Glass);

            EnumSkyBlock type = yUp == 0 ? EnumSkyBlock.Block : EnumSkyBlock.Sky;

            BlockPos pos = new BlockPos(x, yDown, z);
            // тикущаяя яркость
            int lightVox = GetLight(pos, type);
            // Затемнить
            List<LightStruct> list = DarkenLightFor(pos, type, (byte)lightVox, yUp);
            // Возврат осветления при затемнении
            foreach (LightStruct c in list)
            {
                BlockPos pos2 = new BlockPos(c.Pos + c.Vec);
                BrighterLightFor(pos2, type, c.Light, pos2.Y);
                // Тест столба для визуализации ввиде стекла
                //World.SetBlock(pos2, EnumBlock.Glass, c.Light);
            }
        }

        #region Brighter

        /// <summary>
        /// Осветляем за счёт блока или неба при старте
        /// </summary>
        /// <param name="pos">позиция блока</param>
        /// <param name="type">тип освещения</param>
        /// <param name="light">планируемая яркость</param>
        /// <param name="yUp">максимальная высота, если столб</param>
        protected void BrighterLightFor(BlockPos pos, EnumSkyBlock type, byte light, int yUp)
        {
            if (yUp > 0)
            {
                for (int y = pos.Y; y <= yUp; y++)
                {
                    SetLightCheckSky(new BlockPos(pos.X, y, pos.Z), type, light);
                }
            }
            else
            {
                SetLightCheckSky(pos, type, light);
            }

            // основная коллекция
            list = new List<LightStruct>() { new LightStruct(pos.ToVec3i(), light) };
            // Впомогательная коллекция
            listCache = new List<LightStruct>();

            // Кэш массива проверки яркости блока, 31*31*31
            int yh = yUp > pos.Y ? 31 + yUp - pos.Y : 31;
            cacheList = new byte[31, yh, 31];
            // Параметр для первого блока
            bool isBegin = true;

            // Цикл обхода по древу, уровневым метод (он же ширину (breadth-first search, BFS))
            while (list.Count > 0)
            {
                foreach (LightStruct c in list)
                {
                    int lightNew = c.Light;
                    if (isBegin || RefreshBrighterLight(pos.Add(c.Vec), c.Light, type, out lightNew))
                    {
                        if (yh > 31 && isBegin)
                        {
                            // для столбца в первом проходе
                            int yMax = yUp - pos.Y;
                            for (int y = 0; y < yMax; y++) // Цикл рядов
                            {
                                for (int i = 0; i < 4; i++) // Цикл сторон
                                {
                                    SetCacheBrighter(pos, EnumFacing.GetHorizontal(i), new vec3i(0, y, 0), (byte)lightNew);
                                }
                            }
                            // добавляем вверхний блок
                            SetCacheBrighter(pos, Pole.Up, new vec3i(0, yMax - 1, 0), (byte)lightNew);
                            // добавляем нижний блок
                            SetCacheBrighter(pos, Pole.Down, new vec3i(0), (byte)lightNew);
                        }
                        else
                        {
                            // для блока
                            for (int i = 0; i < 6; i++) // Цикл сторон
                            {
                                SetCacheBrighter(pos, (Pole)i, c.Vec, (byte)lightNew);
                            }
                        }
                        isBegin = false;
                    }
                }
                list = listCache;
                listCache = new List<LightStruct>();
            }
        }

        /// <summary>
        /// Изменение кэша в момент осветления
        /// </summary>
        protected void SetCacheBrighter(BlockPos pos, Pole pole, vec3i vec, byte lightNew)
        {
            vec3i v = vec + EnumFacing.DirectionVec(pole);
            if (cacheList[v.x + 15, v.y + 15, v.z + 15] > lightNew) return;
            cacheList[v.x + 15, v.y + 15, v.z + 15] = lightNew;
            listCache.Add(new LightStruct(pos.ToVec3i(), v, lightNew));
        }

        /// <summary>
        /// Обновить освещения в момент осветления
        /// </summary>
        /// <param name="pos">позиция блока</param>
        /// <param name="light">освещение</param>
        /// <param name="type">тип света</param>
        /// <param name="lightNew">будущее освещение</param>
        /// <returns>true было изменение, false нет изменения</returns>
        protected bool RefreshBrighterLight(BlockPos pos, int light, EnumSkyBlock type, out int lightNew)
        {
            BlockBase block = World.GetBlock(pos);
            // Определяем тикущую яркость блока
            lightNew = GetLight(pos, type);
            // Определяем яркость, какая должна
            light = light - (block.GetBlockLightOpacity() + 1);
            if (light < 0) light = 0;
            if (lightNew >= light) return false;
            // Если тикущая темнее, осветляем её
            lightNew = light;
            SetLight(pos, type, (byte)lightNew);
            return true;
        }

        #endregion

        #region Darken 

        /// <summary>
        /// Затемнить 
        /// </summary>
        /// <param name="pos">позиция блока</param>
        /// <param name="type">тип освещения</param>
        /// <param name="light">планируемая яркость</param>
        /// <param name="yUp">максимальная высота, если столб</param>
        protected List<LightStruct> DarkenLightFor(BlockPos pos, EnumSkyBlock type, byte light, int yUp)
        {
            if (yUp > 0)
            {
                for (int y = pos.Y; y <= yUp; y++)
                {
                    SetLight(new BlockPos(pos.X, y, pos.Z), type, 0);
                }
            }
            else
            {
                SetLight(pos, type, 0);
            }

            // основная коллекция
            list = new List<LightStruct>() { new LightStruct(pos.ToVec3i(), light) };
            // Впомогательная коллекция
            listCache = new List<LightStruct>();
            // Коллекция для подготовки осветления
            List<LightStruct> listResult = new List<LightStruct>();
            
            // Кэш массива проверки яркости блока, 31*31*31
            int yh = yUp > pos.Y ? 33 + yUp - pos.Y : 33;
            cacheListD = new bool[33, yh, 33];
            // Параметр для первого блока
            bool isBegin = true;

            // Цикл обхода по древу, уровневым метод (он же ширину (breadth-first search, BFS))
            while (list.Count > 0)
            {
                foreach (LightStruct c in list)
                {
                    int lightNew = c.Light;
                    bool b = isBegin; // Первый проход
                    if (!b)
                    {
                        int bb = RefreshDarkenLight(pos.Add(c.Vec), c.Light, type, out lightNew);
                        if (bb == 2)
                        {
                            if (lightNew > 1)// && lightNew < 15)
                            {
                                // Блок ярче нужного, значит готовим блок для осветления
                                listResult.Add(new LightStruct(pos.ToVec3i(), c.Vec, (byte)lightNew));
                            }
                        }
                        else if (bb == 1) b = true;
                    }

                    if (b)
                    {
                        if (yh > 33 && isBegin)
                        {
                            // для столбца в первом проходе
                            int yMax = yUp - pos.Y;
                            for (int y = 0; y < yMax; y++) // Цикл рядов
                            {
                                for (int i = 0; i < 4; i++) // Цикл сторон
                                {
                                    SetCacheDarken(pos, EnumFacing.GetHorizontal(i), new vec3i(0, y, 0), (byte)lightNew);
                                }
                            }
                            // добавляем вверхний блок
                            SetCacheDarken(pos, Pole.Up, new vec3i(0, yMax - 1, 0), (byte)lightNew);
                            // добавляем нижний блок
                            SetCacheDarken(pos, Pole.Down, new vec3i(0), (byte)lightNew);
                        }
                        else
                        {
                            // для блока
                            for (int i = 0; i < 6; i++) // Цикл сторон
                            {
                                SetCacheDarken(pos, (Pole)i, c.Vec, (byte)lightNew);
                            }
                        }
                        isBegin = false;
                    }
                }
                list = listCache;
                listCache = new List<LightStruct>();
            }
            return listResult;
        }

        /// <summary>
        /// Обновить звтемнение
        /// </summary>
        /// <param name="pos">позиция блока</param>
        /// <param name="light">освещение</param>
        /// <param name="type">тип света</param>
        /// <returns>0 - остановить проход, 1 - продолжить проход, 2 - остановить проход с записью для освещения</returns>
        protected int RefreshDarkenLight(BlockPos pos, int light, EnumSkyBlock type, out int lightNew)
        {
            lightNew = 0;
            // Если блок уже равна темноте, останавливаем проход
            if (light == 0) return 0;
            BlockBase block = World.GetBlock(pos);
            // Определяем тикущую яркость блока
            lightNew = GetLight(pos, type);
            // Если фактическая яркость больше уровня прохода,
            // значит зацепили соседний источник света, прерываем без изменения 
            // с будущей пометкой на проход освещения
            if (lightNew > light || (type == EnumSkyBlock.Block && block.LightValue > 0))
            {
                if (type == EnumSkyBlock.Block && block.LightValue > 0) lightNew = block.LightValue;
                return 2;
            }
            // Изменяем яркость в полностью тёмную
            if (type == EnumSkyBlock.Sky && IsAgainstSky(pos)) return 2;
            SetLight(pos, type, 0);
            lightNew = light - 1;
            return 1;
        }

        /// <summary>
        /// Изменение кэша в момент осветления
        /// </summary>
        protected void SetCacheDarken(BlockPos pos, Pole pole, vec3i vec, byte lightNew)
        {
            vec3i v = vec + EnumFacing.DirectionVec(pole);
            if (cacheListD[v.x + 16, v.y + 16, v.z + 16]) return;
            cacheListD[v.x + 16, v.y + 16, v.z + 16] = true;
            listCache.Add(new LightStruct(pos.ToVec3i(), v, lightNew));
        }

        #endregion
    }
}

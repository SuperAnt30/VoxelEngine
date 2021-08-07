using VoxelEngine.Glm;
using System.Collections.Generic;
using VoxelEngine.Util;
using System.Collections;

namespace VoxelEngine
{ 
    public class WorldRender
    {
        /// <summary>
        /// Массив видимых чанков ChunkRender
        /// </summary>
        //protected ChunkRender[] _chunksRender = new ChunkRender[VE.CHUNK_ALL];

        /// <summary>
        /// Массив кэша чанков vec2i, ChunkMeshs
        /// </summary>
        protected Hashtable _chunksRender = new Hashtable();
        /// <summary>
        /// Индекс количества загруженых чанков
        /// </summary>
        //protected int _chunkIndex = 0;


      //  protected ChunkRender[] _chunksRender = new ChunkRender[VE.CHUNK_VISIBILITY + 3];
        

        /// <summary>
        /// Массив загрузки чанков на 4 потока
        /// </summary>
        protected List<ChunkLoading>[] listLoading = new List<ChunkLoading>[8];
        /// <summary>
        /// Пауза для потоков
        /// </summary>
        protected bool[] pause = new bool[8];

        /// <summary>
        /// Массив RegionFile
        /// </summary>
        protected Dictionary<vec2i, RegionFile> _regions = new Dictionary<vec2i, RegionFile>();

        public WorldRender()
        {
            for (int i = 0; i < listLoading.Length; i++)
            {
                listLoading[i] = new List<ChunkLoading>();
                pause[i] = false;
            }

            _timeSave = Debag.GetInstance().TickCount;
        }

        /// <summary>
        /// Запустить паузу
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetPause(int index, bool value)
        {
            pause[index] = value;
        }

        /// <summary>
        /// Количество чанков в загрузке
        /// </summary>
        public int CountLoading(int index)
        {
            return listLoading[index].Count;
        }

        /// <summary>
        /// Проверить все ли потоки чанков на паузе
        /// </summary>
        /// <returns></returns>
        public bool AllPause()
        {
            for (int i = 0; i < listLoading.Length; i++)
            {
                if (pause[i]) return true;
            }
            return false;
        }

        /// <summary>
        /// Когда было сделано сохранение мира
        /// </summary>
        protected long _timeSave = 0;
        /// <summary>
        /// Такт 20 в секунду
        /// </summary>
        public void Tick()
        {
            // для записи
            if (_timeSave + 6000 < Debag.GetInstance().TickCount)
            {
                _timeSave = Debag.GetInstance().TickCount;
                // Сохраняем миры каждые 5 мин
                RegionsWrite();
            }

            // дла чанка, где стоишь и рядом (крест)
            vec2i c = OpenGLF.GetInstance().Cam.ToPositionChunk();
            int d = 2; // дистанция


            ChunkLoading[] spiral = OpenGLF.GetInstance().DistSqrt;

            // тик по спирале
            for (int i = 0; i < VE.CHUNKS_TICK; i++) // spiral.Length
            {
                ChunkRender cm = GetChunk(c.x + spiral[i].X, c.y + spiral[i].Z);
                if (cm != null) cm.Tick();
            }

            //ChunkRender cm = GetChunk(c.x, c.y);

                //if (cm != null)
                //{
                //    cm.Tick();
                //    cm.ChunkEast().Tick();
                //    cm.ChunkNorth().Tick();
                //    cm.ChunkSouth().Tick();
                //    cm.ChunkWest().Tick();
                //}

        }


        /// <summary>
        /// Очистить кэш чанков
        /// </summary>
        public void ChunkRendersClear()
        {
            //_chunksRender = new ChunkRender[VE.CHUNK_ALL];
            //_chunkIndex = 0;
            _chunksRender.Clear();
        }

        protected void AddChunck(ChunkRender cr)
        {
            _chunksRender.Add(WorldMesh.KeyChunk(cr.X, cr.Z), cr); 
        }

        /// <summary>
        /// Удалить дальние чанки из массива кэша сеток
        /// </summary>
        public void RemoveAway(vec2i positionCam)
        {
            List<string> vs = new List<string>();
            // дальность чанков с учётом кэша
            int visiblityCache = VE.CHUNK_VISIBILITY + 4;
            int xMin = positionCam.x - visiblityCache;
            int xMax = positionCam.x + visiblityCache;
            int zMin = positionCam.y - visiblityCache;
            int zMax = positionCam.y + visiblityCache;
            // Собираем массив чанков которые уже не попадают в видимость
            foreach (DictionaryEntry s in _chunksRender)
            {
                ChunkRender cr = s.Value as ChunkRender;
                if (cr.X <= xMin || cr.X >= xMax || cr.Z <= zMin || cr.Z >= zMax)
                {
                    vs.Add(s.Key.ToString());
                }
            }

            // Удаляем
            if (vs.Count > 0)
            {
                foreach (string key in vs)
                {
                    _chunksRender.Remove(key);
                }
            }

            Debag.GetInstance().CacheChunk = _chunksRender.Count;
        }

        /// <summary>
        /// Запуск генерации меша
        /// </summary>
        /// <param name="chunkPos">координаты камеры</param>
        /// <returns>Если пауза выплёвываем с ошибкой, для последуещей попытки</returns>
        public bool Render(vec2i chunkPos, bool isAndAlpha)
        {
            // Пауза для проверки что потоки установились которые прорисовывают чанки
            if (AllPause()) return false;
            // Debag.Log("log", "WorldRender.Render XZ {0}/{1}", chunkPos.X, chunkPos.Y);

            // Собираем массив чанков которые уже не попадают в видимость
            RemoveAway(chunkPos);
            //List<string> vs = new List<string>();


            // Готовим буфер для заполнения
            //ChunkRender[] chunksBuffer = new ChunkRender[VE.CHUNK_ALL];
            //_chunkIndex = 0;

            // Список используемых регионов
            List<vec2i> keyRegions = new List<vec2i>();
            foreach (vec2i s in _regions.Keys) keyRegions.Add(s);

            // Очистить списки загрузки чанков, так как сейчас мы новые списки создадим
            for (int i = 0; i < listLoading.Length; i++)
            {
                listLoading[i].Clear();
            }
            // Debag.GetInstance().CountTest = 0;
            // дальность чанков с учётом кэша
            int visiblityCache = VE.CHUNK_VISIBILITY + 1;

            int xMin = chunkPos.x - visiblityCache;
            int xMax = chunkPos.x + visiblityCache;
            int zMin = chunkPos.y - visiblityCache;
            int zMax = chunkPos.y + visiblityCache;

            // собираем новые, и удаляем в старье если они есть
            for (int x = xMin; x <= xMax; x++)
            {
                for (int z = zMin; z <= zMax; z++)
                {
                    // Открываем нужные регион файлы
                    keyRegions.Remove(RegionAdd(x, z));
                   // Debag.GetInstance().CountTest++;
                    // определяем если чанк отрендереин и имеет меш, то мы его не пересаздаём
                    //bool isDel = false;
                    ChunkRender cr = GetChunk(x, z);
                    if (cr != null && cr.IsRender())
                    {
                        //ChunkMeshs cm = OpenGLF.GetInstance().WorldM.GetChunk(x, z);
                        //if (cm != null && cm.CountPoligon > 0)
                        //{
                        //    cm.MeshDense.Render(cr.ToBuffer());
                        //    cm.MeshAlpha.Render(cr.ToBufferAlpha());
                        //}

                        //}
                        //OpenGLF.GetInstance().WorldM.RenderChank(e.Chunk.X, e.Chunk.Z, e.Chunk.ToBuffer());
                        //if (cr != null && cr.IsRender()) isDel = true;
                        //if (cr != null) isDel = true;
                        //if (isDel)
                        //if (cr != null && cr.IsRender())
                        //{
                        // Добавляем чанк
                        //vs.Add(WorldMesh.KeyChunk(x, z));
                        //AddChunck(cr);
                        //chunksBuffer[_chunkIndex] = cr;
                        //_chunkIndex++;

                        if (x == xMin || x == xMax || z == zMin || z == zMax)
                        {
                            // Если чанк попадает за пределы видимости но ещё в кеше, очищаем буфер
                            cr.ClearBuffer();
                        }
                    }
                    else
                    //if (cr == null || !cr.IsRender())
                    {
                        //if (cr != null && cr.IsData)
                        //{
                        //    // Данный чанк надо перерисовать
                        //  //  vs.Add(WorldMesh.KeyChunk(x, z));
                        //    //AddChunck(cr);
                        //    //chunksBuffer[_chunkIndex] = cr;
                        //    //_chunkIndex++;
                        //} 

                        // определяем дистанцию от камеры до чанка
                        float d = Camera.DistanceChunkTo(chunkPos, new vec2i(x, z));

                        // добавляем
                        if (x != xMin && x != xMax && z != zMin && z != zMax)
                        {
                            // Индекс по чётности
                            int index = (((x & 1) == 0) ? 1 : 0) + (((z & 1) == 0) ? 2 : 0);
                            //index = 1;
                            // на 1 чанк по периметру меньше для прорисовки, для кеша больше
                            listLoading[index].Add(new ChunkLoading(x, z, d));
                        }

                        // кэш
                        if (cr == null)
                        {
                            // Индекс по чётности
                            int index = (((x & 1) == 0) ? 1 : 0) + (((z & 1) == 0) ? 2 : 0);

                            listLoading[index + 4].Add(new ChunkLoading(x, z, d));
                        }
                    }
                }
            }

            // Сортируем по дистанции
            for (int i = 0; i < listLoading.Length; i++)
            {
                if (listLoading[i].Count > 0)
                {
                    listLoading[i].Sort();
                }
            }
            Debag.GetInstance().CountTest = listLoading[4].Count;

            // Удаляем
            //if (vs.Count > 0)
            //{
            //    foreach (string key in vs)
            //    {
            //        _chunksRender.Remove(key);
            //    }
            //}

            //// Передаём в основной массив чанки с буфера
            //_chunksRender = chunksBuffer;
            //// буфер очищаем
            //chunksBuffer = new ChunkRender[0];

            // удаляем остатки регион старья по кэшу
            foreach (vec2i key in keyRegions)
            {
                RegionRemove(key);
            }

            // Debag.GetInstance().CacheChunk = _chunksRender.Length;

            Debag.GetInstance().CacheChunk = _chunksRender.Count;
            if (isAndAlpha) { RenderAlpha(chunkPos); }
            return true;
        }

        /// <summary>
        /// Запуск генерации меша одного или двух чанков, при изменении вокселя
        /// </summary>
        /// <param name="chunkPos">координаты камеры</param>
        /// <param name="beside">если есть соседний чанк его кооры</param>
        /// <returns>Если пауза выплёвываем с ошибкой, для последуещей попытки</returns>
        public bool RenderOne(vec2i chunkPos, vec2i[] beside)
        {
            // Пауза для проверки что потоки установились которые прорисовывают чанки
            if (AllPause()) return false;

            int index;
            for (int i = 0; i < beside.Length; i++)
            {
                index = (((beside[i].x & 1) == 0) ? 1 : 0) + (((beside[i].y & 1) == 0) ? 2 : 0);
                listLoading[index + 4].Insert(0, new ChunkLoading(beside[i].x, beside[i].y, 0));
                listLoading[index].Insert(0, new ChunkLoading(beside[i].x, beside[i].y, 0));
            }

            index = (((chunkPos.x & 1) == 0) ? 1 : 0) + (((chunkPos.y & 1) == 0) ? 2 : 0);

            listLoading[index + 4].Insert(0, new ChunkLoading(chunkPos.x, chunkPos.y, 0));
            listLoading[index].Insert(0, new ChunkLoading(chunkPos.x, chunkPos.y, 0));
            
            return true;
        }

        /// <summary>
        /// Запуск генерации альфа меша для одного чанка
        /// </summary>
        /// <param name="chunkPos">координаты камеры</param>
        /// <returns>Если пауза выплёвываем с ошибкой, для последуещей попытки</returns>
        public bool RenderOneAlpha(vec2i chunkPos)
        {
            // Пауза для проверки что потоки установились которые прорисовывают чанки
            if (AllPause()) return false;

            // Крестом обнавляем альфа блоки, 5 чанков.
            vec2i[] beside = new vec2i[] { new vec2i(0, 1), new vec2i(1, 0), new vec2i(0, -1), new vec2i(-1, 0), new vec2i(0, 0) };

            int index;
            for (int i = 0; i < beside.Length; i++)
            {
                int x = chunkPos.x + beside[i].x;
                int z = chunkPos.y + beside[i].y;
                index = (((x & 1) == 0) ? 1 : 0) + (((z & 1) == 0) ? 2 : 0);
                listLoading[index].Insert(0, new ChunkLoading(x, z, 0, true));
            }

            return true;
        }

        /// <summary>
        /// Запуск генерации альфа чанков меша
        /// </summary>
        /// <param name="chunkPos">координаты камеры</param>
        /// <returns>Если пауза выплёвываем с ошибкой, для последуещей попытки</returns>
        public bool RenderAlpha(vec2i chunkPos)
        {
            // Пауза для проверки что потоки установились которые прорисовывают чанки
            if (AllPause()) return false;

            // массив дальности
            ChunkLoading[] spiral = OpenGLF.GetInstance().DistSqrtAlpha;

            // Прорисовка алфы в зависимости куда смотрим. От до
            for (int i = spiral.Length - 1; i >= 0; i--)
            {
                int x = spiral[i].X + chunkPos.x;
                int z = spiral[i].Z + chunkPos.y;
                ChunkRender cr = GetChunk(x, z);
                if (cr != null && cr.CountBufferAlpha() > 0)
                {
                    // Индекс по чётности
                    int index = (((x & 1) == 0) ? 1 : 0) + (((z & 1) == 0) ? 2 : 0);
                    // на 1 чанк по периметру меньше для прорисовки, для кеша больше
                    listLoading[index].Insert(0, new ChunkLoading(x, z, spiral[i].Distance, true));
                }
            }

            return true;
        }

        /// <summary>
        /// Пробегаем по загрузке чанка
        /// задумка один в тике
        /// </summary>
        public void LoadRender(int index)
        {
            if (listLoading[index].Count > 0)
            {
                ChunkLoading cl = listLoading[index][0];
                if (index >= 4)
                {
                    // потоки получения данных
                    bool notSave = true;
                    ChunkRender cr = GetChunk(cl.X, cl.Z);
                    if (cr != null)
                    {
                        cr.Save();
                        notSave = false;
                    }
                    //for (int i = 0; i < _chunksRender.Length; i++)
                    //{
                    //    if (_chunksRender[i] != null && _chunksRender[i].X == cl.X && _chunksRender[i].Z == cl.Z)
                    //    {
                    //        _chunksRender[i].Save();
                    //        // _chunksRender[i].RenderSkyLight();
                    //        //GenerationSkyLight(_chunksRender[i]);
                    //        notSave = false;
                    //    }
                    //}
                    if (notSave)
                    {
                        //int xMin = cl.X - 1;
                        //int xMax = cl.X + 1;
                        //int zMin = cl.Z - 1;
                        //int zMax = cl.Z + 1;
                        //int count = 0;

                        ChunkRender chunk = new ChunkRender(cl.X, cl.Z, this);
                        //chunk.ChunkWest = GetChunk(cl.X - 1, cl.Z);
                        //if (chunk.ChunkWest != null) chunk.ChunkWest.ChunkEast = chunk;
                        //chunk.ChunkEast = GetChunk(cl.X + 1, cl.Z);
                        //if (chunk.ChunkEast != null) chunk.ChunkEast.ChunkWest = chunk;
                        //chunk.ChunkNorth = GetChunk(cl.X, cl.Z - 1);
                        //if (chunk.ChunkNorth != null) chunk.ChunkNorth.ChunkSouth = chunk;
                        //chunk.ChunkSouth = GetChunk(cl.X, cl.Z + 1);
                        //if (chunk.ChunkSouth != null) chunk.ChunkSouth.ChunkNorth = chunk;

                        //chunk = GetChunk(cl.X, cl.Z);
                        
                        //if (IsChunkLoaded(xMin, cl.Z)) count++;
                        //if (IsChunkLoaded(xMax, cl.Z)) count++;
                        //if (IsChunkLoaded(cl.X, zMin)) count++;
                        //if (IsChunkLoaded(cl.X, zMax)) count++;

                        //if (IsChunkLoaded(xMin, zMin)) count++;
                        //if (IsChunkLoaded(xMin, zMax)) count++;
                        //if (IsChunkLoaded(xMax, zMin)) count++;
                        //if (IsChunkLoaded(xMax, zMax)) count++;

                        //if (count < 8)
                        //{
                        //    return;
                        //}
                        chunk.LoadinData();
                        //GenerationSkyLight(chunk);
                        // добавляем новый чанк из-за загрузки потока
                        AddChunck(chunk);

                        //_chunksRender[_chunkIndex] = chunk;
                        //_chunkIndex++;

                        Debag.GetInstance().CacheChunk = _chunksRender.Count;// _chunkIndex;
                    }
                    listLoading[index].RemoveAt(0);
                }
                else
                {
                    // потоки генерации меша
                    ChunkRender chunk = null;// = _chunksRender[0];
                    int xMin = cl.X - 1;
                    int xMax = cl.X + 1;
                    int zMin = cl.Z - 1;
                    int zMax = cl.Z + 1;
                    int count = 0;
                    // Сколько чанков должно быть загружено
                    int countRes = 9;// VEC.GetInstance().AmbientOcclusion ? 9 : 5;

                    if (IsChunkLoaded(cl.X, cl.Z))
                    {
                        chunk = GetChunk(cl.X, cl.Z);
                        count++;
                        if (IsChunkLoaded(xMin, cl.Z))
                        {
                            count++;
                           // if (chunk.ChunkWest == null) chunk.ChunkWest = GetChunk(xMin, cl.Z);
                        }
                        if (IsChunkLoaded(xMax, cl.Z))
                        {
                            count++;
                           // if (chunk.ChunkWest == null) chunk.ChunkEast = GetChunk(xMax, cl.Z);
                        }
                        if (IsChunkLoaded(cl.X, zMin))
                        {
                            count++;
                           // if (chunk.ChunkWest == null) chunk.ChunkNorth = GetChunk(cl.X, zMin);
                        }
                        if (IsChunkLoaded(cl.X, zMax))
                        {
                            count++;
                            //if (chunk.ChunkWest == null) chunk.ChunkSouth = GetChunk(cl.X, zMax);
                        }

                        if (IsChunkLoaded(xMin, zMin)) count++;
                        if (IsChunkLoaded(xMin, zMax)) count++;
                        if (IsChunkLoaded(xMax, zMin)) count++;
                        if (IsChunkLoaded(xMax, zMax)) count++;
                    }

                    //for (int i = 0; i < _chunksRender.Length; i++)
                    //{
                    //    if (_chunksRender[i] != null)
                    //    {
                    //        if (_chunksRender[i].X == cl.X && _chunksRender[i].Z == cl.Z)
                    //        {
                    //            chunk = _chunksRender[i];
                    //            count++;
                    //        }
                    //        else if (_chunksRender[i].X == xMin && _chunksRender[i].Z == cl.Z) count++;
                    //        else if (_chunksRender[i].X == xMax && _chunksRender[i].Z == cl.Z) count++;
                    //        else if (_chunksRender[i].X == cl.X && _chunksRender[i].Z == zMin) count++;
                    //        else if (_chunksRender[i].X == cl.X && _chunksRender[i].Z == zMax) count++;

                    //       // if (VEC.GetInstance().AmbientOcclusion)
                    //        {
                    //            if (_chunksRender[i].X == xMin && _chunksRender[i].Z == zMin) count++;
                    //            else if (_chunksRender[i].X == xMin && _chunksRender[i].Z == zMax) count++;
                    //            else if (_chunksRender[i].X == xMax && _chunksRender[i].Z == zMin) count++;
                    //            else if (_chunksRender[i].X == xMax && _chunksRender[i].Z == zMax) count++;
                    //        }
                    //    }
                    //}

                    if (count == countRes && chunk != null)
                    {
                        if (cl.IsAlpha) chunk.RenderAlpha();
                        else
                        {
                            ///GenerationSkyLight(chunk);
                            chunk.Render();
                        }
                        listLoading[index].RemoveAt(0);
                        OnChunkDone(chunk, cl.IsAlpha);
                    }
                    else if (CountLoading4plus() == 0)
                    {
                        listLoading[index].RemoveAt(0);
                    }
                    //else if ((listLoading[4].Count == 0)
                    //{
                    //    // Очищаем с очереди, так как чанка в кеше нет и уже не будет
                    //    listLoading[index].RemoveAt(0);
                    //}
                }
                   
                string s = "";
                int c = 0;
                for (int i = 0; i < listLoading.Length; i++)
                {
                    c += listLoading[i].Count;
                    s += listLoading[i].Count.ToString() + " ";
                }

                if (listLoading[4].Count == 0) Debag.GetInstance().EndTimeLoad();
                if (c == 0) Debag.GetInstance().EndTime();
                Debag.GetInstance().LoadingChunk = s;
            }
        }

        protected int CountLoading4plus()
        {
            return listLoading[4].Count + listLoading[5].Count + listLoading[6].Count + listLoading[7].Count;
        }

        /// <summary>
        /// Получить кэш памяти регионов в байтах 
        /// </summary>
        protected int _Mem()
        {
            int mem = 0;
            foreach (RegionFile region in _regions.Values)
            {
                mem += region.Mem();
            }
            return mem;
        }

        /// <summary>
        /// Добавить регион файл
        /// </summary>
        public vec2i RegionAdd(int chunkX, int chunkZ)
        {
            int x = chunkX >> 5;
            int z = chunkZ >> 5;
            vec2i key = new vec2i(x, z);

            if (!_regions.ContainsKey(key))
            {
                RegionFile region = new RegionFile(chunkX, chunkZ);
                region.ReadFile();
                _regions.Add(key, region);
                Debag.GetInstance().CacheRegion = _regions.Count;
                Debag.GetInstance().CacheRegionMem = _Mem();
            }
           
            return key;
        }

        /// <summary>
        /// Удалить регион но перед этим записать
        /// </summary>
        public void RegionRemove(vec2i key)
        {
            if (_regions.ContainsKey(key))
            {
                RegionFile region = _regions[key];
                region.WriteFile();
                _regions.Remove(key);
                Debag.GetInstance().CacheRegion = _regions.Count;
                Debag.GetInstance().CacheRegionMem = _Mem();
            }
        }

        /// <summary>
        /// Получить файл региона
        /// </summary>
        public RegionFile GetRegion(int chunkX, int chunkZ)
        {
            int x = chunkX >> 5;
            int z = chunkZ >> 5;
            vec2i key = new vec2i(x, z);
            return _regions.ContainsKey(key) ? _regions[key] : null;
        }

        /// <summary>
        /// Записать все регионы
        /// </summary>
        public void RegionsWrite()
        {
            WorldFile.Save(); // TODO::Save

            foreach (RegionFile region in _regions.Values)
            {
                region.WriteFile();
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
            ChunkRender chunk = GetChunk(pos.x >> 4, pos.z >> 4);
            if (chunk == null) return new Voxel();
            return chunk.GetVoxel(pos.x & 15, pos.y, pos.z & 15);
        }

        /// <summary>
        /// Заменить воксель
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="id"></param>
        //public vec2i[] SetVoxelId(Block block)
        //{
        //    int vx = block.Position.X & 15;
        //    int vz = block.Position.Z & 15;

        //    ChunkRender chunk = GetChunk(block.Position.X >> 4, block.Position.Z >> 4);
        //    chunk.SetVoxelId(vx, block.Position.Y, vz, block.Id);
        //    //chunk.SetHeightMap(block.Position, block.Id);
        //    chunk.GenerateSkylightMap();
        //    CheckLight(block.Position);
        //    //chunk.GenerateSkylightMap();
        //    chunk._RelightBlock(vx, block.Position.Y, vz);
            
        //    // CheckLight(new BlockPos(vx, block.Position.Y, vz));
        //    List<vec2i> p = new List<vec2i>();
            
        //    if (block.Id == 12 || block.Id == 0)
        //    {
        //        p.AddRange(new vec2i[] {
        //            new vec2i(-1, -1), new vec2i(-1, 0), new vec2i(-1, 1),
        //            new vec2i(0, -1), new vec2i(0, 1),
        //            new vec2i(1, -1), new vec2i(1, 0), new vec2i(1, 1)
        //        });
        //    } else
        //    {
        //        if (vx == 0) p.Add(new vec2i(-1, 0));
        //        if (vz == 0) p.Add(new vec2i(0, -1));
        //        if (vx == 15) p.Add(new vec2i(1, 0));
        //        if (vz == 15) p.Add(new vec2i(0, 1));
        //    }
            
        //    return p.ToArray();
        //}

        //public void SetBlockState2(Block newBlock)
        //{
        //    OnVoxelChanged(newBlock.Position.ToVec3i(), SetBlockState(newBlock));
        //}

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

            ChunkRender chunk = GetChunk(newBlock.Position.X >> 4, newBlock.Position.Z >> 4);
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
                } else
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

        /// <summary>
        /// загружен ли чанк
        /// </summary>
        public bool IsChunkLoaded(int x, int z)
        {
            return _chunksRender.ContainsKey(WorldMesh.KeyChunk(x, z));
            //for (int i = 0; i < _chunksRender.Length; i++)
            //{
            //    if (_chunksRender[i] != null && _chunksRender[i].X == x && _chunksRender[i].Z == z) return true;
            //}
            //return false;
        }


        /// <summary>
        /// Получить чанк с кэша//, если его там нет, то сгенерировать его
        /// </summary>
        public ChunkRender GetChunk(int x, int z)
        {
            if (IsChunkLoaded(x, z)) return _chunksRender[WorldMesh.KeyChunk(x, z)] as ChunkRender;
            return null;
            //for (int i = 0; i < _chunksRender.Length; i++)
            //{
            //    if (_chunksRender[i] != null && _chunksRender[i].X == x && _chunksRender[i].Z == z) return _chunksRender[i];
            //}
            //return null;
        }

        /// <summary>
        /// Получить чанк с кэша//, если его там нет, то сгенерировать его
        /// </summary>
        public ChunkRender GetChunk(BlockPos pos)
        {
            return GetChunk(pos.X >> 4, pos.Z >> 4);
        }
        /// <summary>
        /// Получить чанк с кэша//, если его там нет, то сгенерировать его
        /// </summary>
        //public ChunkRender GetChunk(int x, int z)
        //{
        //    ChunkRender chunk = _chunksRender[0];
        //    while (true)
        //    {
        //        if (chunk != null)
        //        {
        //            if (chunk.X == x && chunk.Z == z)
        //            {
        //                return chunk;
        //            }
        //            else if (chunk.X > x) chunk = chunk.ChunkWest;
        //            else if (chunk.X < x) chunk = chunk.ChunkEast;
        //            else if (chunk.Z > z) chunk = chunk.ChunkSouth;
        //            else if (chunk.Z < z) chunk = chunk.ChunkNorth;
        //        }
        //        else return null;
        //    }
        //}


        /// <summary>
        /// Может ли видеть небо (CanSeeSky)
        /// </summary>
        public bool IsAgainstSky(BlockPos pos)
        {
            ChunkRender chunk = GetChunk(pos);
            if (chunk == null) return false;
            return chunk.CanSeeSky(pos);
        }

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

        protected bool _IsAreaLoaded(int x0, int y0, int z0, int x1, int y1, int z1)
        {
            if (y0 >= 0 && y1 < 256)
            {
                x0 >>= 4;
                z0 >>= 4;
                x1 >>= 4;
                z1 >>= 4;

                for (int x = x0; x <= x1; x++)
                {
                    for (int z = z0; z <= z1; z++)
                    {
                        if (!IsChunkLoaded(x, z)) return false;
                    }
                }
                return true;

            }
            return false;
        }

        public bool _IsAreaLoaded(BlockPos pos, int radius)
        {
            return _IsAreaLoaded(
                pos.X - radius, pos.Y - radius, pos.Z - radius,
                pos.X + radius, pos.Y + radius, pos.Z + radius);
        }

        /// <summary>
        /// Генерация освещения блока
        /// </summary>
        public bool CheckLightFor(EnumSkyBlock type,  BlockPos pos)
        {
            // Проверка загруженности чанков в области 
            if (!_IsAreaLoaded(pos, 17))
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
            ChunkRender chunk = GetChunk(pos.X >> 4, pos.Z >> 4);
            if (chunk == null) return (byte)type;
            return chunk.GetLightFor(pos.X & 15, pos.Y, pos.Z & 15, type);
        }

        /// <summary>
        /// Задать уровень яркости тикущего блока
        /// </summary>
        public void SetLightFor(EnumSkyBlock type, BlockPos pos, byte lightValue)
        {
            if (pos.Y < 0) pos = new BlockPos(pos.X, 0, pos.Z);
            ChunkRender chunk = GetChunk(pos.X >> 4, pos.Z >> 4);
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


        /*
        public void GenerationSkyLight(ChunkRender chunk)
        {
            int x, y, z;
            // Всё темно
            for (y = 0; y < 255; y++)
            {
                for (z = 0; z < 16; z++)
                {
                    for (x = 0; x < 16; x++)
                    {
                        chunk.SetLightFor(x, y, z, EnumSkyBlock.Sky, 0);
                    }
                }
            }
            // Пробегаем вниз
            for (z = 0; z < 16; z++)
            {
                for (x = 0; x < 16; x++)
                {
                    for (y = 255; y >= 0; y--)
                    {
                        if (chunk.GetVoxel(x, y, z).GetId() != 0) // нет прозрачности
                        {
                            chunk.SetUpBlock(x, y, z);
                            break;
                        }
                        chunk.SetLightFor(x, y, z, EnumSkyBlock.Sky, 15);
                    }
                }
            }
            
            //// пробуем проход сверху вних
            //byte[,] mapB = new byte[16, 16];
            //for (y = 254; y >= 0; y--)
            //{
            //    byte[,] map = new byte[16, 16];
            //    byte[,] map2 = new byte[16, 16];
            //    for (z = 0; z < 16; z++)
            //    {
            //        for (x = 0; x < 16; x++)
            //        {
            //            Voxel v = chunk.GetVoxel(x, y, z);
            //            map[x, z] = v.GetId() == 0 ? v.GetSkyLight() : (byte)0;
            //            map2[x, z] = map[x, z];
            //        }
            //    }
            //    for (z = 1; z < 15; z++)
            //    {
            //        for (x = 1; x < 15; x++)
            //        {
            //            bool isAir = chunk.GetVoxel(x, y, z).GetId() == 0;
            //            if (isAir && map[x, z] < 15)
            //            {
            //                byte bb = 0;
            //                byte bb2 = 0;
            //                bb = map[x - 1, z]; if (bb > bb2) bb2 = bb;
            //                bb = map[x + 1, z]; if (bb > bb2) bb2 = bb;
            //                bb = map[x, z - 1]; if (bb > bb2) bb2 = bb;
            //                bb = map[x, z + 1]; if (bb > bb2) bb2 = bb;
            //                bb = mapB[x - 1, z]; if (bb > bb2) bb2 = bb;
            //                bb = mapB[x + 1, z]; if (bb > bb2) bb2 = bb;
            //                bb = mapB[x, z - 1]; if (bb > bb2) bb2 = bb;
            //                bb = mapB[x, z + 1]; if (bb > bb2) bb2 = bb;
            //                if (bb2 > 1) bb2-=2;
            //                map2[x, z] = bb2;
            //            }
            //        }
            //    }
            //    mapB = map2;
            //    for (z = 1; z < 15; z++)
            //    {
            //        for (x = 1; x < 15; x++)
            //        {
            //            chunk.SetSkyLight(x, y, z, map2[x, z]);
            //        }
            //    }
            //}

            //// пробуем проход с низу вверх
            //for (y = 0; y < 255; y++)
            //{
            //    byte[,] map = new byte[16, 16];
            //    byte[,] map2 = new byte[16, 16];
            //    for (z = 0; z < 16; z++)
            //    {
            //        for (x = 0; x < 16; x++)
            //        {
            //            Voxel v = chunk.GetVoxel(x, y, z);
            //            map[x, z] = v.GetId() == 0 ? v.GetSkyLight() : (byte)0;
            //            map2[x, z] = map[x, z];
            //        }
            //    }
            //    for (z = 1; z < 15; z++)
            //    {
            //        for (x = 1; x < 15; x++)
            //        {
            //            bool isAir = chunk.GetVoxel(x, y, z).GetId() == 0;
            //            if (isAir && map[x, z] < 15)
            //            {
            //                byte bb = 0;
            //                byte bb2 = 0;
            //                bb = map[x - 1, z]; if (bb > bb2) bb2 = bb;
            //                bb = map[x + 1, z]; if (bb > bb2) bb2 = bb;
            //                bb = map[x, z - 1]; if (bb > bb2) bb2 = bb;
            //                bb = map[x, z + 1]; if (bb > bb2) bb2 = bb;
            //                bb = mapB[x - 1, z]; if (bb > bb2) bb2 = bb;
            //                bb = mapB[x + 1, z]; if (bb > bb2) bb2 = bb;
            //                bb = mapB[x, z - 1]; if (bb > bb2) bb2 = bb;
            //                bb = mapB[x, z + 1]; if (bb > bb2) bb2 = bb;
            //                if (bb2 > 0) bb2 -= 1;
            //                map2[x, z] = bb2;
            //            }
            //        }
            //    }
            //    mapB = map2;
            //    for (z = 1; z < 15; z++)
            //    {
            //        for (x = 1; x < 15; x++)
            //        {
            //            chunk.SetSkyLight(x, y, z, map2[x, z]);
            //        }
            //    }
            //}
            

        }*/
        

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
        /// Событие сделано
        /// </summary>
        public event ChunkEventHandler ChunkDone;
        /// <summary>
        /// Событие сделано
        /// </summary>
        protected void OnChunkDone(ChunkRender chunk, bool isAlpha)
        {
            ChunkDone?.Invoke(this, new ChunkEventArgs(chunk, isAlpha));
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

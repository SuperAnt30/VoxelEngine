using VoxelEngine.Glm;
using System.Collections.Generic;
using VoxelEngine.Util;
using System.Collections;
using System.Threading.Tasks;
using VoxelEngine.World;
using VoxelEngine.World.Chunk;

namespace VoxelEngine
{ 
    public class WorldRender : WorldD
    {
        /// <summary>
        /// Массив загрузки чанков на 4 потока
        /// </summary>
        // protected List<ChunkLoading>[] listLoading = new List<ChunkLoading>[8];
      //  protected List<ChunkLoading> listLoading2 = new List<ChunkLoading>();

        /// <summary>
        /// Такт 20 в секунду
        /// </summary>
        public new void Tick()
        {
            base.Tick();

            // дла чанка, где стоишь и рядом (крест)
            vec2i c = OpenGLF.GetInstance().Cam.ToPositionChunk();
            int d = 2; // дистанция


            ChunkLoading[] spiral = VES.GetInstance().DistSqrt;

            // тик по спирале
            for (int i = 0; i < VE.CHUNKS_TICK; i++) // spiral.Length
            {
                ChunkD cm = GetChunk(c.x + spiral[i].X, c.y + spiral[i].Z);
                if (cm != null) cm.Tick();
            }

           // Task.Factory.StartNew(() =>
            // {
            //Render(c);
            Render(c);
              //  Render(c);
            // LoadRender();
            // LoadRender();
            //LoadRender();
            //LoadRender();
            //LoadRender();
           // });


        }

        public ChunkRender GetChunkRender(int x, int z)
        {
            if (IsChunkLoaded(x, z))
            {
                ChunkD chunk = GetChunk(x, z);
                if (chunk != null)
                {
                    if (chunk.Tag != null && chunk.Tag.GetType() == typeof(ChunkRender))
                    {
                        return chunk.Tag as ChunkRender;
                    }
                    else
                    {
                        return new ChunkRender(chunk, this);
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Запуск генерации меша
        /// </summary>
        /// <param name="chunkPos">координаты камеры</param>
        /// <returns>Если пауза выплёвываем с ошибкой, для последуещей попытки</returns>
        public bool Render(vec2i chunkPos)
        {
            int count = 0;
            // Пауза для проверки что потоки установились которые прорисовывают чанки
            //if (AllPause()) return false;
            // Debag.Log("log", "WorldRender.Render XZ {0}/{1}", chunkPos.X, chunkPos.Y);

            // Собираем массив чанков которые уже не попадают в видимость
            //RemoveAway(chunkPos);
            //List<string> vs = new List<string>();

            // Готовим буфер для заполнения
            //ChunkRender[] chunksBuffer = new ChunkRender[VE.CHUNK_ALL];
            //_chunkIndex = 0;

            // Список используемых регионов
            List<vec2i> keyRegions = RegionPr.KeyRegions();

            // Очистить списки загрузки чанков, так как сейчас мы новые списки создадим
            //for (int i = 0; i < listLoading.Length; i++)
            {
           //     listLoading2.Clear();
            }
            // Debag.GetInstance().CountTest = 0;
            // дальность чанков с учётом кэша
            int visiblityCache = VE.CHUNK_VISIBILITY + 1;

            int xMin = chunkPos.x - visiblityCache;
            int xMax = chunkPos.x + visiblityCache;
            int zMin = chunkPos.y - visiblityCache;
            int zMax = chunkPos.y + visiblityCache;

            ChunkLoading[] chunkLoading = VES.GetInstance().DistSqrtYaw(OpenGLF.GetInstance().Cam.Yaw);
            // собираем новые, и удаляем в старье если они есть
            for (int i = 0; i < chunkLoading.Length; i++)
            //for (int x = xMin; x <= xMax; x++)
            {
              //  for (int z = zMin; z <= zMax; z++)
                {
                    int x = chunkLoading[i].X + chunkPos.x;
                    int z = chunkLoading[i].Z + chunkPos.y;
                    // Открываем нужные регион файлы
                    keyRegions.Remove(RegionPr.RegionSet(x, z));
                    // Debag.GetInstance().CountTest++;
                    // определяем если чанк отрендереин и имеет меш, то мы его не пересаздаём
                    //bool isDel = false;

                    ChunkRender cr = GetChunkRender(x, z);// new ChunkRender(GetChunk(x, z), this);

                    if (cr != null && !cr.IsRender())
                    {
                        ChunkRender(new vec2i(x, z));
                        //ChunkRender(new vec2i(x, z), true);
                        count++;
                        if (count >= VE.RENDER_CHUNK_TPS)
                            return true;
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
                    //else
                    ////if (cr == null || !cr.IsRender())
                    //{
                    //    //if (cr != null && cr.IsData)
                    //    //{
                    //    //    // Данный чанк надо перерисовать
                    //    //  //  vs.Add(WorldMesh.KeyChunk(x, z));
                    //    //    //AddChunck(cr);
                    //    //    //chunksBuffer[_chunkIndex] = cr;
                    //    //    //_chunkIndex++;
                    //    //} 

                    //    // определяем дистанцию от камеры до чанка
                    //    float d = Camera.DistanceChunkTo(chunkPos, new vec2i(x, z));

                    //    // добавляем
                    //    if (x != xMin && x != xMax && z != zMin && z != zMax)
                    //    {
                    //        // Индекс по чётности
                    //        int index = (((x & 1) == 0) ? 1 : 0) + (((z & 1) == 0) ? 2 : 0);
                    //        //index = 1;
                    //        // на 1 чанк по периметру меньше для прорисовки, для кеша больше
                    //        //listLoading2.Add(new ChunkLoading(x, z, d));
                    //    }

                    //    // кэш
                    //    //if (cr == null)
                    //    //{
                    //    //    // Индекс по чётности
                    //    //    int index = (((x & 1) == 0) ? 1 : 0) + (((z & 1) == 0) ? 2 : 0);

                    //    //    listLoading[index + 4].Add(new ChunkLoading(x, z, d));
                    //    //}
                    //}
                }
            }

            // Сортируем по дистанции
            //for (int i = 0; i < listLoading.Length; i++)
            {
                //if (listLoading2.Count > 0)
                //{
                //    listLoading2.Sort();
                //}
            }
           // Debag.GetInstance().CountTest = listLoading[4].Count;

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
                RegionPr.RegionRemove(key.x, key.y);
            }

            // Debag.GetInstance().CacheChunk = _chunksRender.Length;

            Debag.GetInstance().CacheChunk = ChunkPr.Count();
            //if (isAndAlpha) { RenderAlpha(chunkPos); }
            return true;
        }
/*
        /// <summary>
        /// Запуск генерации меша одного или двух чанков, при изменении вокселя
        /// </summary>
        /// <param name="chunkPos">координаты камеры</param>
        /// <param name="beside">если есть соседний чанк его кооры</param>
        /// <returns>Если пауза выплёвываем с ошибкой, для последуещей попытки</returns>
        public bool RenderOne(vec2i chunkPos, vec2i[] beside)
        {
            // Пауза для проверки что потоки установились которые прорисовывают чанки
          //  if (AllPause()) return false;

            int index;
            for (int i = 0; i < beside.Length; i++)
            {
                index = (((beside[i].x & 1) == 0) ? 1 : 0) + (((beside[i].y & 1) == 0) ? 2 : 0);
                //listLoading[index + 4].Insert(0, new ChunkLoading(beside[i].x, beside[i].y, 0));
                listLoading2.Insert(0, new ChunkLoading(beside[i].x, beside[i].y, 0));
            }

            index = (((chunkPos.x & 1) == 0) ? 1 : 0) + (((chunkPos.y & 1) == 0) ? 2 : 0);

           // listLoading[index + 4].Insert(0, new ChunkLoading(chunkPos.x, chunkPos.y, 0));
            listLoading2.Insert(0, new ChunkLoading(chunkPos.x, chunkPos.y, 0));
            ChunkD cr = GetChunk(chunkPos.x, chunkPos.y);
            if (cr != null)
            {
                cr.Save();
                //cr.SetChunkModified();
            }

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
          //  if (AllPause()) return false;

            // Крестом обнавляем альфа блоки, 5 чанков.
            vec2i[] beside = new vec2i[] { new vec2i(0, 1), new vec2i(1, 0), new vec2i(0, -1), new vec2i(-1, 0), new vec2i(0, 0) };

            int index;
            for (int i = 0; i < beside.Length; i++)
            {
                int x = chunkPos.x + beside[i].x;
                int z = chunkPos.y + beside[i].y;
                index = (((x & 1) == 0) ? 1 : 0) + (((z & 1) == 0) ? 2 : 0);
                listLoading2.Insert(0, new ChunkLoading(x, z, 0, true));
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
          //  if (AllPause()) return false;

            // массив дальности
            ChunkLoading[] spiral = VES.GetInstance().DistSqrtAlpha;

            // Прорисовка алфы в зависимости куда смотрим. От до
            for (int i = spiral.Length - 1; i >= 0; i--)
            {
                int x = spiral[i].X + chunkPos.x;
                int z = spiral[i].Z + chunkPos.y;
                ChunkRender cr = GetChunkRender(x, z);// new ChunkRender(GetChunk(x, z), this);
                if (cr != null && cr.CountBufferAlpha() > 0)
                {
                    // Индекс по чётности
                    int index = (((x & 1) == 0) ? 1 : 0) + (((z & 1) == 0) ? 2 : 0);
                    // на 1 чанк по периметру меньше для прорисовки, для кеша больше
                    listLoading2.Insert(0, new ChunkLoading(x, z, spiral[i].Distance, true));
                }
            }

            return true;
        }

        /// <summary>
        /// Пробегаем по загрузке чанка
        /// задумка один в тике
        /// </summary>
        public void LoadRender()//int index)
        {
            if (listLoading2.Count > 0)
            {
                ChunkLoading cl = listLoading2[0];
                // потоки генерации меша
                ChunkD chunk = null;// = _chunksRender[0];
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

                if (count == countRes && chunk != null)
                {
                    ChunkRender chunkR = GetChunkRender(cl.X, cl.Z);// new ChunkRender(chunk, this);
                    if (cl.IsAlpha) chunkR.RenderAlpha();
                    else
                    {
                        ///GenerationSkyLight(chunk);
                        chunkR.Render();
                    }
                    listLoading2.RemoveAt(0);
                    OnChunkDone(chunkR, cl.IsAlpha);
                }
                //else if (CountLoading4plus() == 0)
                //{
                //    listLoading2.RemoveAt(0);
                //}
                //else if ((listLoading[4].Count == 0)
                //{
                //    // Очищаем с очереди, так как чанка в кеше нет и уже не будет
                //    listLoading[index].RemoveAt(0);
                //}
                
                   
                string s = "";
                int c = 0;
                //for (int i = 0; i < listLoading.Length; i++)
                {
                    c += listLoading2.Count;
                    s += listLoading2.Count.ToString() + " ";
                }

                if (listLoading2.Count == 0) Debag.GetInstance().EndTimeLoad();
                if (c == 0) Debag.GetInstance().EndTime();
                Debag.GetInstance().LoadingChunk = s;
            }
        }*/

        protected void ChunkRender(vec2i pos)
        {
            //ChunkD chunk = null;// = _chunksRender[0];
            bool isLoad = IsAreaLoaded(pos, 1);

            if (isLoad)
            {
                ChunkRender chunkR = GetChunkRender(pos.x, pos.y);// new ChunkRender(GetChunk(pos.x, pos.y), this);
                if (chunkR != null)
                {
                    chunkR.RenderAlpha();
                    chunkR.Render();
                    //listLoading2.RemoveAt(0);
                    OnChunkDone(chunkR, false);
                }
            }

            //int xMin = pos.x - 1;
            //int xMax = pos.x + 1;
            //int zMin = pos.y - 1;
            //int zMax = pos.y + 1;
            //int count = 0;
            //// Сколько чанков должно быть загружено
            //int countRes = 9;// VEC.GetInstance().AmbientOcclusion ? 9 : 5;

            //if (IsChunkLoaded(pos.x, pos.y))
            //{
            //    chunk = GetChunk(pos.x, pos.y);
            //    count++;
            //    if (IsChunkLoaded(xMin, cl.Z))
            //    {
            //        count++;
            //        // if (chunk.ChunkWest == null) chunk.ChunkWest = GetChunk(xMin, cl.Z);
            //    }
            //    if (IsChunkLoaded(xMax, cl.Z))
            //    {
            //        count++;
            //        // if (chunk.ChunkWest == null) chunk.ChunkEast = GetChunk(xMax, cl.Z);
            //    }
            //    if (IsChunkLoaded(cl.X, zMin))
            //    {
            //        count++;
            //        // if (chunk.ChunkWest == null) chunk.ChunkNorth = GetChunk(cl.X, zMin);
            //    }
            //    if (IsChunkLoaded(cl.X, zMax))
            //    {
            //        count++;
            //        //if (chunk.ChunkWest == null) chunk.ChunkSouth = GetChunk(cl.X, zMax);
            //    }

            //    if (IsChunkLoaded(xMin, zMin)) count++;
            //    if (IsChunkLoaded(xMin, zMax)) count++;
            //    if (IsChunkLoaded(xMax, zMin)) count++;
            //    if (IsChunkLoaded(xMax, zMax)) count++;
            //}

            //if (count == countRes && chunk != null)
            //{
            //    ChunkRender chunkR = new ChunkRender(chunk, this);
            //    if (cl.IsAlpha) chunkR.RenderAlpha();
            //    else
            //    {
            //        ///GenerationSkyLight(chunk);
            //        chunkR.Render();
            //    }
            //    listLoading2.RemoveAt(0);
            //    OnChunkDone(chunkR, cl.IsAlpha);
            //}
            //else if (CountLoading4plus() == 0)
            //{
            //    listLoading2.RemoveAt(0);
            //}
            //else if ((listLoading[4].Count == 0)
            //{
            //    // Очищаем с очереди, так как чанка в кеше нет и уже не будет
            //    listLoading[index].RemoveAt(0);
            //}


            string s = "";
            int c = 0;
            //for (int i = 0; i < listLoading.Length; i++)
            //{
            //    c += listLoading2.Count;
            //    s += listLoading2.Count.ToString() + " ";
            //}

            //if (listLoading2.Count == 0) Debag.GetInstance().EndTimeLoad();
            if (c == 0) Debag.GetInstance().EndTime();
            Debag.GetInstance().LoadingChunk = s;
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
    }
}

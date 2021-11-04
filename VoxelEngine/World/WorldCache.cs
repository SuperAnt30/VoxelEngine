using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxelEngine.Gen;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;
using VoxelEngine.Util;
using VoxelEngine.World.Chk;

namespace VoxelEngine.World
{
    /// <summary>
    /// Объект мира кэша, для работы с загрузками чанков
    /// </summary>
    public class WorldCache : WorldBase
    {
        /// <summary>
        /// Переключится на этап очистки чанков
        /// </summary>
        protected bool isCleaningChunk = false;
        /// <summary>
        /// Переключится на этап очистки чанков
        /// </summary>
        protected bool isCleaningRegion = false;

        /// <summary>
        /// Загрузка области чанков 3*3
        /// true - вся загружена
        /// false - не вся
        /// </summary>
        //public bool AreaLoadingChunk(int cx, int cz)
        //{
        //    int x0 = cx - 1;
        //    int x1 = cx + 1;
        //    int z0 = cz - 1;
        //    int z1 = cz + 1;

        //    for (int x = x0; x <= x1; x++)
        //    {
        //        for (int z = z0; z <= z1; z++)
        //        {
        //            if (x == cx && z == cz) continue;
        //            if (!LoadingChunk(x, z)) return false;
        //        }
        //    }
        //    return true;
        //}

        protected void AreaLoadingChunk(int cx, int cz)
        {
            int x0 = cx - 3;
            int x1 = cx + 3;
            int z0 = cz - 3;
            int z1 = cz + 3;

            for (int x = x0; x <= x1; x++)
            {
                for (int z = z0; z <= z1; z++)
                {
                    if (!ChunkPr.IsChunk(x, z)) ChunkPr.LoadChunk(x, z);
                }
            }
        }

        /// <summary>
        /// Загрузка чанка
        /// true - загружен
        /// false - не вся
        /// </summary>
        protected bool LoadingChunk(int x, int z)//, EnumGeterationStatus status)
        {
            //AreaLoadingChunk(x, z);

            // false - будет прерван массив
            try
            {
                ChunkBase chunk = GetChunk(x, z);
                if (chunk != null)
                {
                    EnumGeterationStatus status = chunk.PreparationStatus;
                    if (status == EnumGeterationStatus.Chunk)
                    {
                        // Если у генерации чанка не было 
                        if (IsArea(x, z)) chunk.GenerationArea();
                    }
                    if (status == EnumGeterationStatus.Area)
                    {
                        // Если у генерации чанка не было 
                        if (IsGeterationArea(x, z, EnumGeterationStatus.Area)) chunk.GenerationDoubleArea();
                    }
                    if (status == EnumGeterationStatus.DoubleArea)
                    {
                        // Если у генерации чанка не было 
                        if (IsGeterationArea(x, z, EnumGeterationStatus.DoubleArea)) chunk.GenerationReady();
                    }
                    return true;
                }
                ChunkPr.LoadChunk(x, z);
                return false;
            }
            catch
            {
                return true;
            }

            //bool res = true;
            //ChunkBase chunk = GetChunk(chx, chz);
            //if (chunk == null)
            //{
            //    chunk = ChunkPr.LoadChunk(chx, chz);
            //    if (status == EnumGeterationStatus.Chunk) return false;
            //}
            // Чанк загружен

            //if (chunk.GeterationStatus == EnumGeterationStatus.DoubleArea)
            //{
            //    return true;
            //}
            //else if (chunk.GeterationStatus == EnumGeterationStatus.Chunk)
            //{
            //    int x0 = chx - 1;
            //    int x1 = chx + 1;
            //    int z0 = chz - 1;
            //    int z1 = chz + 1;
            //    for (int x = x0; x <= x1; x++)
            //    {
            //        for (int z = z0; z <= z1; z++)
            //        {
            //            LoadingChunk(x, z, EnumGeterationStatus.Area);
            //        }
            //    }
            //    chunk.GenerationArea();
            //    if (status == EnumGeterationStatus.Area) return false;
            //}
            //if (status == EnumGeterationStatus.Area) return true;
            //if (chunk.GeterationStatus == EnumGeterationStatus.Area)
            //{
            //    // Если двойной облости у генерации чанка не было 
            //    int x0 = chx - 1;
            //    int x1 = chx + 1;
            //    int z0 = chz - 1;
            //    int z1 = chz + 1;
            //    for (int x = x0; x <= x1; x++)
            //    {
            //        for (int z = z0; z <= z1; z++)
            //        {
            //            LoadingChunk(x, z, EnumGeterationStatus.Chunk);
            //        }
            //    }
            //    chunk.GenerationDoubleArea();
            //    return false;
            //}
            //return true;
        }

        /// <summary>
        /// Запуск рендер паета
        /// </summary>
        protected void PackageLoadChunkCache()
        {
            PackageLoadChunkCache(true, true);
            if (VE.IS_FAST)
            {
                PackageLoadChunkCache(true, false);
                PackageLoadChunkCache(false, true);
                PackageLoadChunkCache(false, false);
            }
        }

        /// <summary>
        /// Запуск рендер паета
        /// </summary>
        protected void PackageLoadChunkCache(bool isEvenX, bool isEvenZ)
        {
            if (isCleaningChunk)
            {
                Task.Factory.StartNew(() => { CleaningChunk(isEvenX, isEvenZ); });
            }
            else
            {
                Task.Factory.StartNew(() => { LoadingChunkCache(isEvenX, isEvenZ); });
            }
        }

        protected bool LoadingChunkCache(int x, int z, bool isEvenX, bool isEvenZ)
        {
            if (VE.IS_FAST)
            {
                if (Bit.IsEven(x) != isEvenX) return false;
                if (Bit.IsEven(z) != isEvenZ) return false;
            }
            return !LoadingChunk(x, z);
        }

        /// <summary>
        /// Запуск генерации меша
        /// </summary>
        protected void LoadingChunkCache(bool isEvenX, bool isEvenZ)
        {
            vec2i[] chunkFC = OpenGLF.GetInstance().Cam.ChunkLoadingFC;
            vec2i ch = OpenGLF.GetInstance().Cam.ChunkPos;

            for (int x = ch.x - 3; x <= ch.x + 3; x++)
            {
                for (int z = ch.y - 3; z <= ch.y + 3; z++)
                {
                    if (LoadingChunkCache(x, z, isEvenX, isEvenZ)) break;
                }
            }

            // собираем новые, и удаляем в старье если они есть
            int i;
            for (i = 0; i < chunkFC.Length; i++)
            {
                int x = chunkFC[i].x;
                int z = chunkFC[i].y;

                if (LoadingChunkCache(x, z, isEvenX, isEvenZ)) break;
                //if (VE.IS_FAST)
                //{
                //    if (Bit.IsEven(x) != isEvenX) continue;
                //    if (Bit.IsEven(z) != isEvenZ) continue;
                //}

                //if (!LoadingChunk(x, z)) break; // быстро но без крайних чанков

                //ChunkBase chunk = GetChunk(x, z);
                //EnumGeterationStatus status = chunk.GeterationStatus;
                //if (status == EnumGeterationStatus.Chunk)
                //{
                //    // Если у генерации чанка не было 
                //    if (IsArea(x, z)) chunk.GenerationArea();
                //}
                //if (status == EnumGeterationStatus.Area)
                //{
                //    // Если у генерации чанка не было 
                //    if (IsGeterationArea(x, z, EnumGeterationStatus.Area)) chunk.GenerationDoubleArea();
                //}
                //if (status == EnumGeterationStatus.DoubleArea)
                //{
                //    // Если у генерации чанка не было 
                //    if (IsGeterationArea(x, z, EnumGeterationStatus.DoubleArea)) chunk.GenerationReady();
                //}

                //if (!AreaLoadingChunk(x, z)) break; // медленно
            }
            // ЭТОТ СЛИП чтоб не подвисал проц. И для перехода других потоков.
            System.Threading.Thread.Sleep(1);
            if (i >= chunkFC.Length - 1) OnNotLoadCache();
            else OnLoadCache();
            PackageLoadChunkCache(isEvenX, isEvenZ);
        }

        #region Region

        /// <summary>
        /// Запуск пакета загрузки регионов
        /// </summary>
        protected void PackageLoadRegionCache()
        {
            if (isCleaningRegion)
            {
                Task.Factory.StartNew(() => { CleaningRegion(); });
            }
            else
            {
                Task.Factory.StartNew(() => { LoadingRegionCache(); });
            }
        }

        /// <summary>
        /// Загрузка регионов мира
        /// </summary>
        protected void LoadingRegionCache()
        {
            vec2i[] regionFC = OpenGLF.GetInstance().Cam.RegionLoadingFC;
            for (int i = 0; i < regionFC.Length; i++)
            {
                int x = regionFC[i].x;
                int z = regionFC[i].y;
                RegionPr.RegionSet(x, z);
            }
            // ЭТОТ СЛИП чтоб не подвисал проц. И для перехода других потоков.
            System.Threading.Thread.Sleep(1);
            PackageLoadRegionCache();
        }

        #endregion

        #region Clean

        /// <summary>
        /// Запуск этапа чистки
        /// </summary>
        public void CleaningTrue()
        {
            isCleaningChunk = true;
            isCleaningRegion = true;
        }

        /// <summary>
        /// Рендер чанка для альфа
        /// </summary>
        protected virtual void ChunkRenderAlpha() { }

        /// <summary>
        /// Удалить дальние чанки из массива кэша и регионы
        /// </summary>
        protected void CleaningChunk(bool isEvenX, bool isEvenZ)
        {
            vec2i positionCam = OpenGLF.GetInstance().Cam.ChunkPos;
            List<vec2i> chunks = new List<vec2i>();

            // дальность чанков с учётом кэша
            int visiblityCache = VEC.chunkVisibility + 4;
            int xMin = positionCam.x - visiblityCache;
            int xMax = positionCam.x + visiblityCache;
            int zMin = positionCam.y - visiblityCache;
            int zMax = positionCam.y + visiblityCache;
            // Собираем массив чанков которые уже не попадают в видимость
            Hashtable ht = ChunkPr.CloneHashtable();
            foreach (ChunkBase cr in ht.Values)
            {
                if (VE.IS_FAST)
                {
                    if (Bit.IsEven(cr.X) != isEvenX) continue;
                    if (Bit.IsEven(cr.Z) != isEvenZ) continue;
                }

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
            Debug.CacheChunk = ChunkPr.Count();
            
            // Закончена чистка
            isCleaningChunk = false;
            ChunkRenderAlpha();
            LoadingChunkCache(isEvenX, isEvenZ);
        }

        /// <summary>
        /// Удалить дальние регионы из массива кэша
        /// </summary>
        protected void CleaningRegion()
        {
            vec2i positionCam = OpenGLF.GetInstance().Cam.ChunkPos;
            // дальность чанков с учётом кэша
            int visiblityCache = VEC.chunkVisibility + 4;
            int xMin = (positionCam.x - visiblityCache) >> 5;
            int xMax = (positionCam.x + visiblityCache) >> 5;
            int zMin = (positionCam.y - visiblityCache) >> 5;
            int zMax = (positionCam.y + visiblityCache) >> 5;

            List<vec2i> regions = new List<vec2i>();
            foreach (RegionBinary rf in RegionPr.Values)
            {
                if (rf.X < xMin || rf.X > xMax || rf.Z < zMin || rf.Z > zMax)
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

            // Закончена чистка
            isCleaningRegion = false;
            LoadingRegionCache();
        }

        #endregion

        #region  Event

        /// <summary>
        /// Событие сделанного кэш пакета
        /// </summary>
        public event EventHandler LoadCache;
        /// <summary>
        /// Событие сделанного кэш пакета
        /// </summary>
        protected void OnLoadCache() => LoadCache?.Invoke(this, new EventArgs());
        /// <summary>
        /// Событие нет загрузки сделанного кэш пакета
        /// </summary>
        public event EventHandler NotLoadCache;
        /// <summary>
        /// Событие нет загрузки сделанного кэш пакета
        /// </summary>
        protected void OnNotLoadCache() => NotLoadCache?.Invoke(this, new EventArgs());

        #endregion
    }
}

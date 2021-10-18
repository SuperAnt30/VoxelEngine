using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;
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
        protected bool isCleaning = false;

        /// <summary>
        /// Загрузка области чанков 3*3
        /// true - вся загружена
        /// false - не вся
        /// </summary>
        public bool AreaLoadingChunk(vec2i pos)
        {
            int x0 = pos.x - 1;
            int x1 = pos.x + 1;
            int z0 = pos.y - 1;
            int z1 = pos.y + 1;

            for (int x = x0; x <= x1; x++)
            {
                for (int z = z0; z <= z1; z++)
                {
                    if (!LoadingChunk(x, z)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Загрузка чанка
        /// true - загружен
        /// false - не вся
        /// </summary>
        public bool LoadingChunk(int x, int z)
        {
            if (IsChunk(x, z)) return true;
            // Если не загружен загружаем и выходим
            RegionPr.RegionSet(x, z);
            ChunkPr.LoadChunk(x, z);
            return false;
        }

        /// <summary>
        /// Запуск рендер паета
        /// </summary>
        public void PackageLoadCache()
        {
            if (isCleaning)
            {
                Task.Factory.StartNew(() => { Cleaning(); });
            }
            else
            {
                Task.Factory.StartNew(() => { LoadingChunkCache(); });
            }
        }

        /// <summary>
        /// Запуск генерации меша
        /// </summary>
        protected void LoadingChunkCache()
        {
            ChunkLoading[] chunkLoading = OpenGLF.GetInstance().Cam.ChunkLoadingFC;

            // собираем новые, и удаляем в старье если они есть
            for (int i = 0; i < chunkLoading.Length; i++)
            {
                int x = chunkLoading[i].X;
                int z = chunkLoading[i].Z;
                if (!LoadingChunk(x, z)) break; // быстро но без крайних чанков
                //if (!AreaLoadingChunk(new vec2i(x, z))) break; // медленно
            }
            // ЭТОТ СЛИП чтоб не подвисал проц. И для перехода других потоков.
            System.Threading.Thread.Sleep(1);
            OnLoadCache();
            PackageLoadCache();
        }

        #region Clean

        /// <summary>
        /// Запуск этапа чистки
        /// </summary>
        public void CleaningTrue() => isCleaning = true;

        /// <summary>
        /// Рендер чанка для альфа
        /// </summary>
        protected virtual void ChunkRenderAlpha() { }

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

            // Закончена чистка
            isCleaning = false;
            ChunkRenderAlpha();
            LoadingChunkCache();
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

        #endregion
    }
}

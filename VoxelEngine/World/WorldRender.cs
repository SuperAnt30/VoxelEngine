using VoxelEngine.Glm;
using System.Threading.Tasks;
using VoxelEngine.World;
using VoxelEngine.World.Chunk;
using System;

namespace VoxelEngine
{ 
    public class WorldRender : WorldD
    {
        /// <summary>
        /// Такт 20 в секунду
        /// </summary>
        public new void Tick()
        {
            base.Tick();

            // дла чанка, где стоишь и рядом (крест)
            vec2i c = OpenGLF.GetInstance().Cam.ToPositionChunk();

            ChunkLoading[] spiral = VES.GetInstance().DistSqrt;

            // тик по спирале
            for (int i = 0; i < VE.CHUNKS_TICK; i++) // spiral.Length
            {
                ChunkD cm = GetChunk(c.x + spiral[i].X, c.y + spiral[i].Z);
                if (cm != null) cm.Tick();
            }

           // RenderPackage();
            // Запуск рендера чанков в другом потоке
            //Task.Factory.StartNew(() => { Render(c); });
            //Render(c);
        }

        /// <summary>
        /// Пометить что надо перерендерить сетку чанка
        /// координаты чанка
        /// </summary>
        public void ModifiedToRenderChunk(vec2i pos)
        {
            ChunkRender chunk = GetChunkRender(pos.x, pos.y);
            if (chunk != null) chunk.ModifiedToRender();
        }
        /// <summary>
        /// Пометить что надо перерендерить сетку чанков
        /// координаты чанков
        /// </summary>
        public void ModifiedToRenderChunks(vec2i[] beside)
        {
            for (int i = 0; i < beside.Length; i++)
            {
                ModifiedToRenderChunk(beside[i]);
            }
        }

        /// <summary>
        /// Получить чанк рендера по координатам чанка
        /// </summary>
        public ChunkRender GetChunkRender(int x, int z)
        {
            if (IsChunkLoaded(x, z))
            {
                ChunkD chunk = GetChunk(x, z);
                if (chunk != null)
                {
                    if (chunk.ChunkTag != null)
                    {
                        return chunk.ChunkTag;
                    }
                    return new ChunkRender(chunk, this);
                }
            }
            return null;
        }

        /// <summary>
        /// Запуск рендер паета
        /// </summary>
        public void RenderPackage()
        {
            vec2i c = OpenGLF.GetInstance().Cam.ToPositionChunk();
            Task.Factory.StartNew(() => { Render(c); });
        }


        /// <summary>
        /// Запуск генерации меша
        /// </summary>
        /// <param name="chunkPos">координаты камеры</param>
        /// <returns>Если пауза выплёвываем с ошибкой, для последуещей попытки</returns>
        protected void Render(vec2i chunkPos)
        {
            int count = 0;
            // Получить массив сектора
            ChunkLoading[] chunkLoading = VES.GetInstance().DistSqrtYaw(OpenGLF.GetInstance().Cam.Yaw);
            // собираем новые, и удаляем в старье если они есть
            for (int i = 0; i < chunkLoading.Length; i++)
            {
                int x = chunkLoading[i].X + chunkPos.x;
                int z = chunkLoading[i].Z + chunkPos.y;
                // Открываем нужные регион файлы
                RegionPr.RegionSet(x, z);

                ChunkRender cr = GetChunkRender(x, z);

                if (cr != null && !cr.IsRender())
                {
                    ChunkRender(new vec2i(x, z));
                    count++;
                    if (count >= VE.RENDER_CHUNK_TPS)
                    {
                        break;
                    }
                        
                }
            }
            System.Threading.Thread.Sleep(1); // ЭТОТ    СЛИП чтоб не подвисал проц. И для перехода других потоков.
            OnRendered();
        }

        /// <summary>
        /// Рендер конкретного чанка
        /// </summary>
        /// <param name="pos"></param>
        protected void ChunkRender(vec2i pos)
        {
            bool isLoad = IsAreaLoaded(pos, 1);

            if (isLoad)
            {
                ChunkRender chunkR = GetChunkRender(pos.x, pos.y);
                if (chunkR != null)
                {
                    chunkR.Render();
                    //chunkR.RenderAlpha();
                    OnChunkDone(chunkR, false);
                }
            }
        }

        /// <summary>
        /// изменен воксель
        /// </summary>
        protected override void OnVoxelChanged(vec3i position, vec2i[] beside)
        {
            base.OnVoxelChanged(position, beside);
            ModifiedToRenderChunks(beside);
        }

        #region Event

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
        /// Событие сделанного ренер пакета
        /// </summary>
        public event EventHandler Rendered;

        protected void OnRendered()
        {
            Rendered?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}

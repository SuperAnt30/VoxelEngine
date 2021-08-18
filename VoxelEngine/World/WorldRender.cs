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
        /// Изменён чанк, для перегенерации альфа цвета
        /// </summary>
        public int ChunckChanged { get; set; } = 0;

        /// <summary>
        /// Запуск рендер паета
        /// </summary>
        public void PackageRender()
        {
            Task.Factory.StartNew(() => { Render(); });
        }

        /// <summary>
        /// Такт 20 в секунду
        /// </summary>
        protected override void Tick()
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

            OnTicked();
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
        /// Запуск генерации меша
        /// </summary>
        protected void Render()
        {
            vec2i chunkPos = OpenGLF.GetInstance().Cam.ToPositionChunk();
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
                if (cr == null) 
                {
                    // если нет чанка, включаем счётчик
                    count++;
                }
                else
                {
                    // Если чанк есть и не отрендерин
                    if (!cr.Chunk.IsChunkLoaded)
                    {
                        // Если у чанка нет данных, загружаем (перегружаем)
                        cr.Chunk.OnChunkLoad();
                    }
                    else if (!cr.IsRender())
                    {
                        // Рендер чанка, если норм то выходим с массива
                        if (ChunkRender(cr, false)) break;
                    }
                    else if (i < ChunckChanged)
                    {
                        // Рендер алфа блоков, без выхода с цикла
                        ChunkRender(cr, true);
                    }
                }
                // Количество чанков без данных, выходим с цыкла
                if (count >= 2) break;
            }
            System.Threading.Thread.Sleep(1); // ЭТОТ    СЛИП чтоб не подвисал проц. И для перехода других потоков.
            OnRendered();
        }

        /// <summary>
        /// Рендер конкретного чанка
        /// </summary>
        protected bool ChunkRender(ChunkRender chunkR, bool isAlphe)
        {
            if (IsAreaLoaded(new vec2i(chunkR.Chunk.X, chunkR.Chunk.Z), 1))
            {
                if (chunkR.Chunk.IsChunkLoaded)
                {
                    OnChunkDone(isAlphe
                        ? new BufferEventArgs(chunkR.Chunk.X, chunkR.Chunk.Z, chunkR.RenderAlpha())
                        : new BufferEventArgs(chunkR.Chunk.X, chunkR.Chunk.Z, chunkR.Render(), chunkR.RenderAlpha()));
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// изменен воксель
        /// </summary>
        protected override void OnVoxelChanged(vec3i position, vec2i[] beside)
        {
            base.OnVoxelChanged(position, beside);
        }

        #region Event

        /// <summary>
        /// Событие сделано
        /// </summary>
        public event BufferEventHandler ChunkDone;
        /// <summary>
        /// Событие сделано
        /// </summary>
        protected void OnChunkDone(BufferEventArgs e)
        {
            ChunkDone?.Invoke(this, e);
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

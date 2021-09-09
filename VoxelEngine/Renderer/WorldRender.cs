using VoxelEngine.Glm;
using System.Threading.Tasks;
using VoxelEngine.World;
using System;
using VoxelEngine.Renderer.Chk;
using VoxelEngine.World.Chk;
using VoxelEngine.Graphics;

namespace VoxelEngine.Renderer
{ 
    public class WorldRender : WorldBase
    {
        /// <summary>
        /// Изменён чанк, для перегенерации альфа цвета
        /// </summary>
        public int ChunckChanged { get; set; } = -1;

        /// <summary>
        /// Запуск рендер паета
        /// </summary>
        public void PackageRender()
        {
            Task.Factory.StartNew(() => { Render(); });
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
            if (IsChunk(x, z))
            {
                ChunkBase chunk = GetChunk(x, z);
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
            // Получить массив сектора
            ChunkLoading[] chunkLoading = VES.GetInstance().DistSqrtYaw(OpenGLF.GetInstance().Cam.Yaw);

            // собираем новые, и удаляем в старье если они есть
            for (int i = 0; i < chunkLoading.Length; i++)
            {
                int x = chunkLoading[i].X + chunkPos.x;
                int z = chunkLoading[i].Z + chunkPos.y;

                // Загрузка облости в отдельных потоках
                AreaLoaded(new vec2i(x, z));

                // Проверка облости на один чанк в округе, для рендера чанка
                // кэш соседних чанков обязателен!
                if (IsArea(new vec2i(x, z), 1))
                {
                    ChunkRender cr = GetChunkRender(x, z);
                    if (cr.Chunk.GeterationStatus != Gen.EnumGeterationStatus.Area)
                    {
                        // Если у генерации чанка не было 
                        cr.Chunk.GenerationArea();
                    }
                    if (!cr.IsRender())
                    {
                        // Рендер чанка, если норм то выходим с массива
                        if (ChunkRender(cr, false)) break;
                    }
                    else if (i <= ChunckChanged)
                    {
                        // Рендер алфа блоков, без выхода с цикла
                        ChunkRender(cr, true);
                        if (ChunckChanged == i)
                        {
                            ChunckChanged = -1;
                        }
                    }
                }
                else
                {
                    // Если облости не готовы, нет смысла расматривать дальнейшие чанки
                    // скорее всего просто была подгрузка кэш чанков
                    break;
                }
            }
            // ЭТОТ СЛИП чтоб не подвисал проц. И для перехода других потоков.
            System.Threading.Thread.Sleep(1); 
            OnRendered();
        }

        /// <summary>
        /// Рендер конкретного чанка
        /// </summary>
        protected bool ChunkRender(ChunkRender chunkR, bool isAlphe)
        {
            // Нужна проверка облости в один соседний чанк, типа IsAreaLoaded(new vec2i(X, Z), 1)
            // Но этот метод вызывается всегда после этой проверки, по этому не повторяем бесмысленную проверку
            if (chunkR.Chunk.IsChunkLoaded)
            {
                if (isAlphe)
                {
                    float[] ba = chunkR.RenderAlpha();
                    if (ba.Length > 0)
                    {
                        OnChunkDone(new BufferEventArgs(chunkR.Chunk.X, chunkR.Chunk.Z, ba));
                    }
                } else
                {
                    OnChunkDone(new BufferEventArgs(chunkR.Chunk.X, chunkR.Chunk.Z, chunkR.Render(), chunkR.RenderAlpha()));
                }
                return true;
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

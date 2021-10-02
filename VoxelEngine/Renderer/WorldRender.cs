using VoxelEngine.Glm;
using System.Threading.Tasks;
using VoxelEngine.World;
using System;
using VoxelEngine.Renderer.Chk;
using VoxelEngine.World.Chk;
using VoxelEngine.Graphics;
using VoxelEngine.Entity;
using VoxelEngine.Models;
using System.Collections;
using VoxelEngine.Renderer.Entity;
using VoxelEngine.Util;

namespace VoxelEngine.Renderer
{ 
    public class WorldRender : WorldBase
    {
        /// <summary>
        /// Изменён чанк, для перегенерации альфа цвета
        /// </summary>
        public int ChunckChanged { get; set; } = -1;

        /// <summary>
        /// Позиция камеры
        /// </summary>
        public vec3 CameraPosition { get; protected set; }
        /// <summary>
        /// Направление
        /// </summary>
        public vec3 CameraDirection { get; protected set; }

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
            Camera cam = OpenGLF.GetInstance().Cam;
            vec2i chunkPos = Entity.HitBox.ChunkPos;

            // Получить массив сектора
            //ChunkLoading[] chunkLoading = VES.GetInstance().DistSqrtYaw(OpenGLF.GetInstance().Cam.Yaw);

            ChunkLoading[] chunkLoading = cam.ChunkLoadingFC;
            
            // собираем новые, и удаляем в старье если они есть
            for (int i = 0; i < chunkLoading.Length; i++)
            {
                int x = chunkLoading[i].X;// + chunkPos.x;
                int z = chunkLoading[i].Z;// + chunkPos.y;

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
                        OnDone(new BufferEventArgs(chunkR.Chunk.X, chunkR.Chunk.Z, ba));
                    }
                } else
                {
                    OnDone(new BufferEventArgs(chunkR.Chunk.X, chunkR.Chunk.Z, chunkR.Render(), chunkR.RenderAlpha()));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Такт конкретной сущьности
        /// </summary>
        protected override void TickEntity(EntityLiving entity, long tick)
        {
            Camera cam = OpenGLF.GetInstance().Cam;
            bool v = cam.IsBoxInFrustum(entity.HitBox);
            if (!v)
            {
                entity.Visible = entity.Visible == VisibleDraw.See 
                    ? VisibleDraw.Delete : VisibleDraw.Invisible;
            }
            else
            {
                entity.Visible = VisibleDraw.See;
            }
        }

        /// <summary>
        /// Пакет в отдельном потоке рендера сущьностей
        /// </summary>
        /// <param name="timeAll">время в секундах от запуска</param>
        public void PackageEntities(float timeFrame, float timeAll)
        {
            Task.Factory.StartNew(() => {
                // Такты мобов
                Hashtable hashtable = (Hashtable)Entities.Clone();
                foreach (EntityLiving entity in hashtable.Values)
                {
                    if (entity.Key != EnumEntity.Player)
                    {
                        EntityRender entityRender = new EntityRender(this, entity);
                        entityRender.UpdateDraw(timeFrame, timeAll);

                        if (entity.IsKill)
                        {
                            // дисспавн
                            RemoveEntity(entity);
                            continue;
                        }
                        if (entity.Visible == VisibleDraw.See)
                        {
                            if (entity.Key == EnumEntity.Chicken && entity.IsRender)
                            {
                                entity.RenderDone();
                                ModelChicken chicken = new ModelChicken();

                                chicken.Render(
                                    entity,
                                    timeAll * 8f, // TODO:: Надо как-то от скорости сущности менять
                                    entity.Moving.GetVerticalValue(),
                                    entity.OnGround ? 0 : timeAll * 32f, 0, 0, VE.UV_SIZE);
                                OnDone(new BufferEventArgs(entity.HitBox.Index, entity.Key, chicken.Buffer));
                            }
                        } else if (entity.Visible == VisibleDraw.Delete)
                        {
                            // Сущность не попадает в экран, по этому её сетку удаляем
                            OnDone(new BufferEventArgs(entity.HitBox.Index, entity.Key, new float[0]));
                        }
                    }
                }
            });
        }


        public override void RemoveEntity(EntityLiving entity)
        {
            base.RemoveEntity(entity);
            OnDone(new BufferEventArgs(entity.HitBox.Index, entity.Key, new float[0]));
        }

        public void Camera(vec3 pos, vec3 dir)
        {
            CameraPosition = pos;
            CameraDirection = dir;
        }

        /// <summary>
        /// Обработка луча сущности
        /// </summary>
        protected override EntityDistance RayCrossEntity(EntityLiving entity)
        {
            float dis = 100f;
            if (entity.Visible == VisibleDraw.See)
            {
                dis = Entity.HitBox.DistanceEyesTo(entity.HitBox.BlockPos);
                if (dis < 10f)
                {
                    RayCross ray = new RayCross(CameraPosition, CameraDirection, 10f);
                    HitBoxEntity.HitBoxSizeUD hitBox = entity.HitBox.SizeUD();
                    if (ray.CrossLineToRectangle(hitBox.Vd, hitBox.Vu))
                    {
                        return new EntityDistance(entity, dis);
                    }
                }
            }
            return new EntityDistance();
        }

        /// <summary>
        /// Изменена сущьность попавшая под луч
        /// </summary>
        protected override void RayCastEntityChange()
        {
            RayCast(CameraPosition, CameraDirection, VE.MAX_DIST);
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
        /// Событие чанк сделано
        /// </summary>
        public event BufferEventHandler Done;
        /// <summary>
        /// Событие чанк сделано
        /// </summary>
        protected void OnDone(BufferEventArgs e)
        {
            Done?.Invoke(this, e);
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

using VoxelEngine.Glm;

namespace VoxelEngine
{
    /// <summary>
    /// Объект потока который контралирует рендер мира от камеры
    /// </summary>
    public class ThreadWorld : ThreadObject
    {
        /// <summary>
        /// Объект мира
        /// </summary>
        public WorldRender World { get; protected set; } = new WorldRender();

        /// <summary>
        /// Тип запроса потоков
        /// </summary>
        public enum RenderType
        {
            /// <summary>
            /// Нет
            /// </summary>
            None = -1,
            /// <summary>
            /// Рендер всех чанков сплошных блоков
            /// </summary>
            Dense = 0,
            /// <summary>
            /// Рендер тикущего чанка алфа блоков
            /// </summary>
            AlphaOne = 1,
            /// <summary>
            /// Рендер всех чанков алфа блоков
            /// </summary>
          //  Alpha = 2,
            /// <summary>
            /// Рендер тикущего чанка сплошных блоков
            /// </summary>
            DenseOne = 3,
            /// <summary>
            /// Всё
            /// </summary>
            All = 4
        }

        /// <summary>
        /// Сохраняем ли мир
        /// </summary>
        public bool IsSave { get; set; } = false;

        //public ThreadWorld()
        //{
        //   // World.ChunkDone += _world_ChunkDone;
        //}

        //private void _world_ChunkDone(object sender, ChunkEventArgs e)
        //{
        //    OnChunkDone(e);
        //}

        /// <summary>
        /// Массив запросов
        /// </summary>
        protected ChunkLoading[] _dones = new ChunkLoading[8];

        /// <summary>
        /// Затать запрос для смены вокселя
        /// </summary>
        /// <param name="positionCenter">позиция центра</param>
        /// <param name="beside">соседний от центра</param>
        public void SetVoxel(vec2i positionCenter, vec2i[] beside)
        {
            _dones[(int)RenderType.DenseOne] = new ChunkLoading(positionCenter.x, positionCenter.y) { Beside = beside };
        }

        /// <summary>
        /// Затать запрос по типу
        /// </summary>
        /// <param name="positionCenter">позиция центра</param>
        /// <param name="render">тип запроса</param>
        /// <param name="beside">соседний от центра</param>
        public void SetCenterPosition(vec2i positionCenter, RenderType render)
        {
            _dones[(int)render] = new ChunkLoading(positionCenter.x, positionCenter.y);
        }

        /// <summary>
        /// Метод запуска для отдельного потока
        /// </summary>
        protected override void _Run()
        {
            for (int i = 0; i < _dones.Length; i++)
            {
                if (_dones[i] != null)
                {
                    bool isDone = true;
                    vec2i pos = new vec2i(_dones[i].X, _dones[i].Z);
                    switch ((RenderType)i)
                    {
                        case RenderType.AlphaOne: isDone = !World.RenderOneAlpha(pos); break;
                        case RenderType.Dense: isDone = !World.Render(pos, false); break;
                        case RenderType.DenseOne: isDone = !World.RenderOne(pos, _dones[i].Beside); break;
                     //   case RenderType.Alpha: isDone = !World.RenderAlpha(pos); break;
                        case RenderType.All: isDone = !World.Render(pos, true); break;
                    }

                    if (!isDone)
                    {
                        _dones[i] = null;
                        OnThreadDone();
                    }
                }
            }

            if (IsSave)
            {
                World.RegionsWrite();
                IsSave = false;
            }
        }

        /// <summary>
        /// Сохраняем регионы и закрываем поток
        /// </summary>
        public void StopSave()
        {
            World.RegionsWrite();
            Stop();
        }

        ///// <summary>
        ///// Событие сделано
        ///// </summary>
        //public event ChunkEventHandler ChunkDone;
        ///// <summary>
        ///// Событие сделано
        ///// </summary>
        //protected void OnChunkDone(ChunkEventArgs e)
        //{
        //    ChunkDone?.Invoke(this, e);
        //}
    }
}

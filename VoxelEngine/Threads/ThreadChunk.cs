namespace VoxelEngine
{/*
    /// <summary>
    /// Объект потока который занимается рендором чанка
    /// </summary>
    public class ThreadChunk : ThreadObject
    {
        //+---------+---------+
        //| index 0 | index 1 | 
        //| X нечёт | X чёт   |
        //| Y нечёт | Y нечёт |
        //+---------+---------+      
        //| index 2 | index 3 | 
        //| X нечёт | X чёт   |
        //| Y чёт   | Y чёт   |
        //+---------+---------+

        /// <summary>
        /// Объект мира которы берёт из объекта ThreadWorld
        /// </summary>
        public WorldRender World { get; protected set; }

        /// <summary>
        /// Индекс для определения с качими чанками работать.
        /// 0 x, y нечёт
        /// 1 x чёт, y нечёт
        /// 2 x нечёт, y чёт
        /// 3 x, y чёт
        /// </summary>
        public int Index { get; protected set; } = 0;

        public ThreadChunk(WorldRender world, int index)
        {
            Index = index;
            World = world;
            World.ChunkDone += _world_ChunkDone;
        }

        protected bool _isPause = false;

        public new void Done()
        {
            _isDone = true;
            //World.SetPause(Index, true);
            _isPause = true;
        }

        private void _world_ChunkDone(object sender, ChunkEventArgs e)
        {
            OnChunkDone(e);
        }

        /// <summary>
        /// Метод запуска для отдельного потока
        /// </summary>
        protected override void _Run()
        {
            //if (_isDone && World.CountLoading(Index) > 0)
            //{
            //    //System.Threading.Thread.Sleep(200);
            //    //Debag.Log("log" + Index.ToString(), "ThreadChunks index {0}", Index);
            //    World.LoadRender(Index);
                
            //}
            //if (!_isDone && _isPause)
            //{
            //    World.SetPause(Index, false);
            //    _isPause = false;
            //}
        }

        /// <summary>
        /// Событие сделано
        /// </summary>
        public event ChunkEventHandler ChunkDone;
        /// <summary>
        /// Событие сделано
        /// </summary>
        protected void OnChunkDone(ChunkEventArgs e)
        {
            ChunkDone?.Invoke(this, e);
        }
    }*/
}

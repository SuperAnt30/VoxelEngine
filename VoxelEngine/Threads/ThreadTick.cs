using System;
using System.Diagnostics;

namespace VoxelEngine
{
    /// <summary>
    /// объект отдельного потока отвечающий за FPS
    /// </summary>
    public class ThreadTick : ThreadObject
    {
        /// <summary>
        /// Интервал тика
        /// 490000 = TPS 20
        /// 156666 для TPS 60
        /// 73333 для TPS 120
        /// </summary>
        public long Interval { get; protected set; } = 50;// 490000;

        /// <summary>
        /// Задать тпс
        /// </summary>
        public void SetTps(int tps)
        {
            Interval = 1000 / tps;
        }

        protected long lastTime;// = DateTime.Now.Ticks;
        /// <summary>
        /// Объект для точного замера времени
        /// </summary>
        protected Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Подготовить запуск
        /// </summary>
        public void Start()
        {
            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;
        }

        protected override void _Run()
        {
            //long interval = 490000; // для TPS 20
            //long interval = 250000; // для TPS 40
            //long interval = 166667; // для TPS 60 (56-57)
            //long interval = 160000; // для TPS 62.5 (59-61)
            //long interval = 156666; // для TPS 60
            //long interval = 166666; // для TPS 60
            // long interval = 40000; // для TPS 200 (160-170)
            // 73333 для TPS 120
            // 156666 для TPS 60
            //double interval = 15.66f;

            //long currentTime = DateTime.Now.Ticks;
            long currentTime = stopwatch.ElapsedMilliseconds;
            if (currentTime - lastTime >= Interval)
            {
                lastTime = currentTime;
                OnThreadDone();
            }
        }
    }
}

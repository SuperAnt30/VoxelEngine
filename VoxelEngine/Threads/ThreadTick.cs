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
        /// </summary>
        public long Interval { get; protected set; } = 50;

        /// <summary>
        /// Задать тпс
        /// </summary>
        public void SetTps(int tps)
        {
            Interval = 1000 / tps;
        }

        protected long lastTime;
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
            long currentTime = stopwatch.ElapsedMilliseconds;
            if (currentTime - lastTime >= Interval)
            {
                lastTime = currentTime;
                OnThreadDone();
            }
        }
    }
}

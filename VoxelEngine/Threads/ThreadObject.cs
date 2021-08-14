using System;
using System.Threading;

namespace VoxelEngine
{
    /// <summary>
    /// Объект для потока
    /// </summary>
    public class ThreadObject
    {

        /// <summary>
        /// Событие сделано
        /// </summary>
        public event EventHandler ThreadDone;
        /// <summary>
        /// Событие сделано
        /// </summary>
        protected void OnThreadDone() => ThreadDone?.Invoke(this, new EventArgs());

        /// <summary>
        /// Переменая для обработчика понимать, что надо делать
        /// </summary>
        protected bool _isDone = true;
        /// <summary>
        /// Надо сделать
        /// </summary>
        public void Done() => _isDone = true;
        /// <summary>
        /// Надо сделать // TODO:: поток зачем пауза
        /// </summary>
        public void Pause() => _isDone = false;

        /// <summary>
        /// Запущен ли цикл потока
        /// </summary>
        public bool IsRun { get; protected set; } = true;
        /// <summary>
        /// Остановить цикл потока
        /// </summary>
        public void Stop() => IsRun = false;

        /// <summary>
        /// Событие, стопа
        /// </summary>
        public event EventHandler Stoped;
        /// <summary>
        /// Событие стопа
        /// </summary>
        protected void OnStoped() => Stoped?.Invoke(this, new EventArgs());


        /// <summary>
        /// Метод запуска для отдельного потока
        /// </summary>
        public void Run()
        {
            while (IsRun)
            {
                _Run();
                Thread.Sleep(1); // ЭТОТ    СЛИП чтоб не подвисал проц. И для перехода других потоков.
            }
            OnStoped();
        }

        protected virtual void _Run()
        {

        }
    }
}

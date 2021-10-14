using System;
using System.Windows.Forms;

namespace VoxelEngine.Gui
{
    public partial class BaseControl : UserControl
    {
        /// <summary>
        /// Основная форм
        /// </summary>
        public FormGame FGame { get; protected set; }

        protected BaseControl() { }
        public BaseControl(FormGame form) => FGame = form;

        /// <summary>
        /// Запуск
        /// </summary>
        public virtual void Open() { }

        #region Event

        /// <summary>
        /// Событие закрыт
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Событие закрыт
        /// </summary>
        protected void OnClosed()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}

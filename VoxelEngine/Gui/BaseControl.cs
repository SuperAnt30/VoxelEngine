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
        /// Нажатие клавиши отмена
        /// </summary>
        protected void KeyDownClose(KeyEventArgs e, params Keys[] args)
        {
            e.SuppressKeyPress = true;
            foreach (Keys keys in args)
            {
                if (e.KeyCode == keys)
                {
                    OnClosed();
                    return;
                }
            }
        }

        /// <summary>
        /// Проверка нажатия цифры
        /// </summary>
        protected bool CheckNumber(KeyEventArgs e)
        {
            Keys[] keys = new Keys[]
            {
                Keys.D0,Keys.D1,Keys.D2,Keys.D3,Keys.D4,Keys.D5,Keys.D6,Keys.D7,Keys.D8,Keys.D9
            };
            foreach (Keys key in keys)
            {
                if (e.KeyCode == key) return true;
            }
            return false;
        }

        /// <summary>
        /// Нажатие клавиши применить c закрытием
        /// </summary>
        protected bool KeyDownApply(KeyEventArgs e, params Keys[] args)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                return true;
            }

            foreach (Keys keys in args)
            {
                if (e.KeyCode == keys)
                {
                    e.SuppressKeyPress = true;
                    OnClosed();
                    break;
                }
            }
            return false;
        }

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

        /// <summary>
        /// Событие следующее окно
        /// </summary>
        public event EventHandler Next;

        /// <summary>
        /// Событие следующее окно
        /// </summary>
        protected void OnNext()
        {
            Next?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}

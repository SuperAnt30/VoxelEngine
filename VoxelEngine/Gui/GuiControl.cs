using System;
using System.Drawing;
using System.Windows.Forms;
using VoxelEngine.Actions;

namespace VoxelEngine.Gui
{
    public partial class GuiControl : UserControl
    {
        //if (e.KeyCode == Keys.I) WindowClosed();

        /// <summary>
        /// Основная форм
        /// </summary>
        public FormGame FGame
        {
            get { return Parent as FormGame; }
        }

        public GuiControl()
        {
            InitializeComponent();
            Controls.Clear();
        }

        #region Opens

        /// <summary>
        /// Открыть окно настроек
        /// </summary>
        public void OpenOptions() => WindowOpen(new OptionsControl(FGame));


        #endregion


        private void Control_Closed(object sender, EventArgs e)
        {
            Visible = false;
            Controls.Clear();
            Mouse.GetInstance().Move(true);
        }

        /// <summary>
        /// Открыть контрол
        /// </summary>
        protected void WindowOpen(BaseControl control)
        {
            Keyboard.GetInstance().KeyMove.CancelAll();
            Size = control.Size;
            control.Closed += Control_Closed;
            Controls.Add(control);
            control.Open();
            Location = new Point(
                (FGame.Width - Width) / 2,
                (FGame.Height - Height) / 2
            );
            Visible = true;
            Focus();
        }
    }
}

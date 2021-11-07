using System;
using System.Drawing;
using System.Windows.Forms;
using VoxelEngine.Actions;
using VoxelEngine.Graphics;

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
        /// <summary>
        /// Открыть инвентарь
        /// </summary>
        public void OpenInventory() => WindowOpen(new InventoryControl(FGame));
        /// <summary>
        /// Открыть меню
        /// </summary>
        public void OpenMenu() => WindowOpen(new MenuControl(FGame));

        #endregion


        private void Control_Closed(object sender, EventArgs e)
        {
            Visible = false;
            PlayerWidget.IsOpenForm = false;
            Controls.Clear();
            Mouse.GetInstance().Move(true);
            OpenGLF.GetInstance().Widget.RefreshDraw();
        }

        private void Control_Next(object sender, EventArgs e)
        {
            Controls.Clear();
        }

        /// <summary>
        /// Открыть контрол
        /// </summary>
        protected void WindowOpen(BaseControl control)
        {
            Keyboard.GetInstance().KeyMove.CancelAll();
            PlayerWidget.IsOpenForm = true;
            OpenGLF.GetInstance().Widget.RefreshDrawDarken();
            control.Open();
            Size = control.Size;
            control.Closed += Control_Closed;
            control.Next += Control_Next;
            Controls.Add(control);
            Location = new Point(
                (FGame.Width - Width) / 2,
                (FGame.Height - Height) / 2
            );
            Visible = true;
            Focus();
        }

        
    }
}

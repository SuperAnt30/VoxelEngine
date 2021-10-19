using System;
using System.Windows.Forms;

namespace VoxelEngine.Gui
{
    /// <summary>
    /// Контрол инвентарь
    /// </summary>
    public partial class InventoryControl : BaseControl
    {
        public InventoryControl(FormGame form) : base(form)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Запуск
        /// </summary>
        public override void Open()
        {

        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.E) OnClosed();
        }
    }
}

using System.Windows.Forms;

namespace VoxelEngine.Gui
{
    public partial class MenuControl : BaseControl
    {
        public MenuControl(FormGame form) : base(form)
        {
            InitializeComponent();
            label1.Text = "VE by SuperAnt\r\nversion " + Debug.Version;
        }

        private void buttonClose_Click(object sender, System.EventArgs e) => FGame.Close();

        private void button1_Click(object sender, System.EventArgs e) => WorldBegin("1", 1);
        private void button2_Click(object sender, System.EventArgs e) => WorldBegin("2", 2);
        private void button3_Click(object sender, System.EventArgs e) => WorldBegin("3", 3);
        private void button4_Click(object sender, System.EventArgs e) => WorldBegin("4", 4);
        private void button5_Click(object sender, System.EventArgs e) => WorldBegin("5", 5);

        private void button_KeyDown(object sender, KeyEventArgs e) => KeyDownClose(e, Keys.Escape);

        protected void WorldBegin(string name, int seed)
        {
            FGame.WorldBegin(name, seed);
            OnClosed();
        }

        
    }
}

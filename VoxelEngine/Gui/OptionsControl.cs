using System;

namespace VoxelEngine.Gui
{
    /// <summary>
    /// Контрол опций
    /// </summary>
    public partial class OptionsControl : BaseControl
    {
        public OptionsControl(FormGame form) : base(form)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Запуск
        /// </summary>
        public override void Open()
        {
            numericUpDownFPS.Value = VE.FPS;
            numericUpDownChunk.Value = VE.CHUNK_VISIBILITY;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            OnClosed();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            VE.CHUNK_VISIBILITY = (int)numericUpDownChunk.Value;
            VES.GetInstance().DistSqrtRefrash();
            VE.FPS = (int)numericUpDownFPS.Value;
            FGame.RefreshFps();
            FGame.RefreshFov();
            OnClosed();
        }
    }
}

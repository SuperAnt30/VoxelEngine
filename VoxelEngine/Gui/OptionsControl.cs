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
            numericUpDownFPS.Value = VEC.fps;
            numericUpDownChunk.Value = VEC.chunkVisibility;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            OnClosed();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            VEC.chunkVisibility = (int)numericUpDownChunk.Value;
            VES.DistSqrtRefrash();
            VEC.fps = (int)numericUpDownFPS.Value;
            FGame.RefreshFps();
            FGame.RefreshFov();
            OnClosed();
        }
    }
}

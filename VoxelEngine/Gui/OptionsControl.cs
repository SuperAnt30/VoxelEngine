using System;
using System.Windows.Forms;
using VoxelEngine.Graphics;

namespace VoxelEngine.Gui
{
    /// <summary>
    /// Контрол опций
    /// </summary>
    public partial class OptionsControl : BaseControl
    {
        public OptionsControl(FormGame form) : base(form) => InitializeComponent();

        /// <summary>
        /// Запуск
        /// </summary>
        public override void Open()
        {
            numericUpDownFPS.Value = VEC.fps;
            numericUpDownChunk.Value = VEC.chunkVisibility;
        }

        private void buttonCancel_Click(object sender, EventArgs e) => OnClosed();

        private void buttonOk_Click(object sender, EventArgs e) => Apply();

        private void numericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyDownApply(e, Keys.Escape, Keys.I))
            {
                Apply();
            }
        }

        protected void Apply()
        {
            VEC.chunkVisibility = (int)numericUpDownChunk.Value;
            VES.DistSqrtRefrash();
            VEC.fps = (int)numericUpDownFPS.Value;
            FGame.RefreshFps();
            FGame.RefreshFov();
            OpenGLF.GetInstance().ChunkVisibilityChanged();
            OnClosed();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            OnNext();
            FGame.WorldEnd(true);
        }
    }
}

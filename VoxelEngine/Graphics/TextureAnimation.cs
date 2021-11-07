using System.Drawing;

namespace VoxelEngine.Graphics
{
    public class TextureAnimation
    {
        /// <summary>
        /// Основной атлас текстур блоков
        /// </summary>
        public Bitmap AtlasBox { get; protected set; }
        /// <summary>
        /// Основной атлас текстур блоков
        /// </summary>
        public Bitmap AtlasBoxOriginal { get; protected set; }

        /// <summary>
        /// Волны воды
        /// </summary>
        public Bitmap WaterStill { get; protected set; }
        /// <summary>
        /// Максимальный шаг волны воды
        /// </summary>
        public int StepWaterStillMax { get; protected set; } = 0;
        /// <summary>
        /// Тикущий шаг волны воды
        /// </summary>
        public int StepWaterStill { get; protected set; } = 0;

        /// <summary>
        /// Течение воды
        /// </summary>
        public Bitmap WaterFlow { get; protected set; }
        /// <summary>
        /// Максимальный шаг волны воды
        /// </summary>
        public int StepWaterFlowMax { get; protected set; } = 0;
        /// <summary>
        /// Тикущий шаг волны воды
        /// </summary>
        public int StepWaterFlow { get; protected set; } = 0;

        public TextureAnimation()
        {
            Bitmap waterStill = new Bitmap(VE.TEXTURE_WATER_STILL);
            Bitmap waterFlow = new Bitmap(VE.TEXTURE_WATER_FLOW);
            AtlasBoxOriginal = VES.AtlasBoxOriginal;
            WaterStill = new Bitmap(waterStill);
            StepWaterStillMax = WaterStill.Height / WaterStill.Width;

            WaterFlow = new Bitmap(waterFlow);
            StepWaterFlowMax = WaterFlow.Height / WaterFlow.Width;
        }

        /// <summary>
        /// пауза для волны
        /// </summary>
        protected int pauseWaterStill = 0;
        protected int pauseWaterFlow = 0;

        public bool Render()
        {
            pauseWaterStill--;
            pauseWaterFlow--;
            if (pauseWaterStill <= 0 || pauseWaterFlow <= 0)
            {
                Bitmap bp = new Bitmap(AtlasBoxOriginal);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bp);
                int w, wa;

                w = WaterStill.Width;
                wa = AtlasBoxOriginal.Width / 16;
                g.DrawImage(WaterStill.Clone(new Rectangle(0, StepWaterStill * w, w, w), AtlasBoxOriginal.PixelFormat), wa * 15, 0);

                w = WaterFlow.Width / 2;
                wa = AtlasBoxOriginal.Width / 16;
                g.DrawImage(WaterFlow.Clone(new Rectangle(0, StepWaterFlow * w * 2, w, w), AtlasBoxOriginal.PixelFormat), wa * 15, wa);

                if (AtlasBox != null) AtlasBox.Dispose();
                AtlasBox = new Bitmap(bp);

                if (pauseWaterStill <= 0)
                {
                    pauseWaterStill = 3;
                    StepWaterStill++;
                    if (StepWaterStill >= StepWaterStillMax) StepWaterStill = 0;
                }
                if (pauseWaterFlow <= 0)
                {
                    pauseWaterFlow = 1;
                    StepWaterFlow++;
                    if (StepWaterFlow >= StepWaterFlowMax) StepWaterFlow = 0;
                }

                return true;
            }
            return false;
        }
    }
}

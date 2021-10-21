using System.Collections.Generic;
using System.Drawing;
using VoxelEngine.Actions;
using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Chk;

namespace VoxelEngine
{
    /// <summary>
    /// Voxel Engine Starting
    /// Генерация специальных массивов, для движка, чтоб потом быстрее по массиву обрабатывать
    /// </summary>
    public class VES
    {
        /// <summary>
        /// Массив по длинам используя квадратный корень для всей видимости
        /// </summary>
        public static vec2i[] DistSqrt { get; protected set; }
        /// <summary>
        /// Параметр [0]=0 .. [16]=0.0625f
        /// </summary>
        public static float[] Uv { get; protected set; } = new float[17];
        /// <summary>
        /// Параметр [1]=0.0625f .. [16]=1.0f
        /// </summary>
        public static float[] Xy { get; protected set; } = new float[17];
        /// <summary>
        /// Основной атлас текстур блоков
        /// </summary>
        public static Bitmap AtlasBoxOriginal { get; protected set; }
        /// <summary>
        /// Картинки блоков для GUI
        /// </summary>
        public static Bitmap[] ImageGui { get; protected set; } = new Bitmap[Blocks.BLOCKS_COUNT];

        /// <summary>
        /// Инициализация, запускаем при старте
        /// </summary>
        public static void Initialized()
        {
            glm.Initialized();
            DistSqrtRefrash();
            for (int i = 0; i <= 16; i++)
            {
                Uv[i] = (float)i * 0.00390625f;
                Xy[i] = (float)i * 0.0625f;
            }
            BlocksGui();
            PlayerWidget.Initialized();
        }

        /// <summary>
        /// Обновить массив длин
        /// </summary>
        public static void DistSqrtRefrash() => DistSqrt = GetSqrt();

        /// <summary>
        /// Сгенерировать массив по длинам используя квадратный корень
        /// </summary>
        /// <param name="vis">Видимость, в одну сторону от ноля</param>
        protected static vec2i[] GetSqrt()
        {
            int vis = VEC.chunkVisibility;
            List<ChunkLoading> r = new List<ChunkLoading>();
            for (int x = -vis; x <= vis; x++)
            {
                for (int y = -vis; y <= vis; y++)
                {
                    r.Add(new ChunkLoading(x, y, Mth.Sqrt(x * x + y * y)));
                }
            }
            r.Sort();
            vec2i[] list = new vec2i[r.Count];
            for (int i = 0; i < r.Count; i++)
            {
                list[i] = new vec2i(r[i].X, r[i].Z);
            }
            return list;
        }

        /// <summary>
        /// Генерация текстуры блоков для GUI
        /// </summary>
        protected static void BlocksGui()
        {
            AtlasBoxOriginal = new Bitmap(VE.TEXTURE_ATLAS);
            Bitmap atlas = new Bitmap(AtlasBoxOriginal);
            for (int i = 1; i <= Blocks.BLOCKS_COUNT; i++)
            {
                EnumBlock enumBlock = (EnumBlock)i;
                BlockBase block = Blocks.GetBlock(enumBlock, new BlockPos());
                bool isColor = block.Boxes[0].Faces[0].IsColor;
                int numTexture = block.Boxes[0].Faces[0].NumberTexture;
                int by = numTexture >> 4;
                int bx = numTexture & 15;
                Bitmap bp = new Bitmap(16, 16);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bp);
                Bitmap side = atlas.Clone(new Rectangle(bx << 4, by << 4, 16, 16), atlas.PixelFormat);
                if (isColor)
                {
                    Color color = block.IsWater ? Color.LightBlue : Color.Green;
                    for (int x = 0; x < 16; x++)
                    {
                        for (int y = 0; y < 16; y++)
                        {
                            Color c = side.GetPixel(x, y);
                            if (c.A > 0)
                            {
                                bp.SetPixel(x, y, Color.FromArgb((c.R + color.R) / 2, (c.G + color.G) / 2, (c.B + color.B) / 2));
                            }
                        }
                    }
                }
                else
                {
                    g.DrawImage(side, 0, 0);
                }
                ImageGui[i - 1] = bp;
            }
        }
    }
}

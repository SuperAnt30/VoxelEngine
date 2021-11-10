/*
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VoxelEngine.Gen;
using VoxelEngine.Util;

namespace VoxelEngine
{
    public partial class FormTest : Form
    {
        /// <summary>
        /// Таймер для фиксации времени
        /// </summary>

        protected Stopwatch stopwatch = new Stopwatch();

        public int octave = 8;
        /// <summary>
        /// octave = 1 d = 0.9f
        /// octave = 2 d = 2.4f
        /// octave = 3 d = 4.8f
        /// octave = 4 d = 9.6f
        /// octave = 5 d = 19.2f
        /// octave = 6 d = 38.4f
        /// octave = 7 d = 76.8f
        /// octave = 8 d = 160.0f
        /// octave = 12 d = 2300.0f
        /// </summary>
        public float dl = 160f; // 160
        public int oc = 8; //8
        public float sc = .6f; //0.8f (game 0.2f)
        protected float delitel = 14f;//14f;

        public float scale = 0.8f;//0.01f;


        public float plus = 5f;
        public float umn = 25f;
        public int seed = 2;
        public bool begin = true;
        public string name = "Begin";
        public float[,] ar1;
        public float[,] ar2;

        public FormTest()
        {
            InitializeComponent();
        }

        private void FormTest_Load(object sender, EventArgs e)
        {
            WinApi.TimeBeginPeriod(1);
            //Test();
            //return;


            if (begin)
            {
                FormTest form1 = new FormTest
                {
                    octave = oc, // 4
                    scale = sc,// .08
                    begin = false,
                    name = "Первый",
                    plus = 2f,
                    umn = 64f,
                    seed = 2
                };
                form1.Show();
                ar1 = form1.ar;
                FormTest form2 = new FormTest
                {
                    octave = oc,
                    scale = sc,
                    begin = false,
                    name = "Второй",
                    plus = 2f,
                    umn = 64f,
                    seed = 3
                };
                form2.Show();
                ar2 = form2.ar;

                TestBegin();
            } else
            {
                Test();
            }
        }

        private void FormTest_SizeChanged(object sender, EventArgs e)
        {
            //if (begin) TestBegin(); else Test();
        }

        private void FormTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            WinApi.TimeEndPeriod(1);
        }

        protected void TestBegin()
        {
            int w = pictureBox1.Width;
            int h = pictureBox1.Height;
            w2 = w;
            h2 = h;
            ar = new float[w2 + 15, h2 + 15];
            stopwatch.Restart();

            long ms = stopwatch.ElapsedTicks * 1000 / Stopwatch.Frequency;

            pictureBox1.Image = new Bitmap(w, h);
            g = System.Drawing.Graphics.FromImage(pictureBox1.Image);

            for (int x = 0; x < w2; x++)
            {
                for (int y = 0; y < h2; y++)
                {
                    //Color c = biome(ar2[x, y] + 10, ar1[x, y]);
                    Color c = biome(ar2[x, y], ar1[x, y]);
                    Pixel(x, y, c);
                }
            }

            pictureBox1.Refresh();
            long ms2 = stopwatch.ElapsedTicks * 1000 / Stopwatch.Frequency;
            // 280 ms работа с Graphics
            Text = string.Format(name + " {1} ms {0} ms [{2:0.00}; {3:0.00}]",
                ms, ms2, fmin, fmax
            );
        }

        protected Color biome(float e, float m)
        {
            e /= delitel;
            m /= delitel;
            // e высота
            // m влажность
            if (e < -3.8f)
            {
                // высоко
                if (m < -2f) return Color.Orange; // горы в пустыне
                return Color.LightGray; // горы каменные
            }
            if (e > -1.0f && e < 0.8f && m >= 3f)
            {
                return Color.Gray; // болото
            }
            if (e < 0f)
            {
                // средняя местность по высоте
                if (m < -2f) return Color.Yellow; // пустыня
                if (m < 1f) return Color.Green; // ровнина
                return Color.DarkGreen; // лес
            }
            if (e < 0.1f)
            {
                if (m < -2f) return Color.Yellow; // пустыня
                if (m < 1f) return Color.White; // пляж
                return Color.DarkGreen; // лес
            }
            // вода
            return Color.Blue;
        }


        protected void SetPerlin1()
        {
            NoiseGeneratorPerlin noiseO = new NoiseGeneratorPerlin(new Random(seed), octave); // 5
            //float no = 0.02f;

            fmin = 10000f;
            fmax = -10000f;
            float d = (float)Math.Pow(2, octave - 1);
            float[] stoneNoise = new float[256];
            for (int x = 0; x < w2; x += 16)
            {
                for (int y = 0; y < h2; y += 16)
                {
                    stoneNoise = noiseO.GenerateNoise2d(stoneNoise, x, y, 16, 16, scale, scale);

                    for (int x0 = 0; x0 < 16; x0++)
                    {
                        for (int y0 = 0; y0 < 16; y0++)
                        {
                            //float fn = (float)stoneNoise[y0 * 16 + x0];
                            //ar[x + x0, y + y0] = Result128(fn + 7, 18f);

                            //float fn = (float)stoneNoise[x0 * 16 + y0];
                            //ar[x + x0, y + y0] = Result128(fn + 8, 16f);
                            float fn = (float)stoneNoise[x0 * 16 + y0];
                            //float fn2 = fn / 3.5f;
                            //int r = Result128(fn2 + plus, umn);
                            // по r и r1 определяем биом, образно температура и влажность
                            //float fn2 = fn / d; 
                            ar[x + x0, y + y0] = fn;// r;

                            if (fn < fmin) fmin = fn;
                            if (fn > fmax) fmax = fn;
                        }
                    }
                }
            }

        }

        System.Drawing.Graphics g;

        int w2;
        int h2;
        public float[,] ar;
        float fmin = 10000f;
        float fmax = -10000f;

        protected void Test()
        {
            
            //Random random = new Random(2);// влажность
            //Random random = new Random(3);// высоты
            int w = pictureBox1.Width;
            int h = pictureBox1.Height;
            w2 = w;
            h2 = h;
            ar = new float[w2 + 15, h2 + 15];

            stopwatch.Restart();

            SetPerlin1();
            Text = string.Format(name + " [{0:0.00}; {1:0.00}]",
                fmin, fmax
            );
            //return;
            //Perlin noise = new Perlin();
            //noise.Perlin2D(2);
            //float size = 0.01f;

            // для высоты рельефа можно идти от 
            // NoiseGeneratorOctaves 4, (0.05f - 0.1f) (диапазон -8f..+8f)

            //NoiseGeneratorPerlin noiseO1 = new NoiseGeneratorPerlin(new Random(2), 5); // 5
            //float no1 = 0.03f;
            //NoiseGeneratorPerlin noiseO = new NoiseGeneratorPerlin(new Random(2), 3); // 5
            //float no = 0.02f;

            //float fmin = 10000f;
            //float fmax = -10000f;



            //float[] stoneNoise1 = new float[256];
            //float[] stoneNoise = new float[256];
            //float var5 = 0.03125f;
            //for (int x = 0; x < w2; x += 16)
            //{
            //    for (int y = 0; y < h2; y += 16)
            //    {
            //        //stoneNoise = noiseS.GenerationToArray(stoneNoise, x, y, 16, 16, var5 * 2f, var5 * 2f, 1f);
            //        //stoneNoise = noiseO.GenerateNoise2d(stoneNoise, x, y, 16, 16, no, no);
            //        stoneNoise1 = noiseO1.GenerateNoise2d(stoneNoise1, x, y, 16, 16, no, no);
            //        stoneNoise = noiseO.GenerateNoise2d(stoneNoise, x, y, 16, 16, no, no);

            //        for (int x0 = 0; x0 < 16; x0++)
            //        {
            //            for (int y0 = 0; y0 < 16; y0++)
            //            {
            //                //float fn = (float)stoneNoise[y0 * 16 + x0];
            //                //ar[x + x0, y + y0] = Result128(fn + 7, 18f);

            //                //float fn = (float)stoneNoise[x0 * 16 + y0];
            //                //ar[x + x0, y + y0] = Result128(fn + 8, 16f);
            //                float fn1 = (float)stoneNoise[x0 * 16 + y0];
            //                int r1 = Result128(fn1 + 8, 16f);
            //                float fn = (float)stoneNoise[x0 * 16 + y0];
            //                int r = Result128(fn + 8, 16f);
            //                // по r и r1 определяем биом, образно температура и влажность
            //                ar[x + x0, y + y0] = (r + r1) / 2;

            //                if (fn < fmin) fmin = fn;
            //                if (fn > fmax) fmax = fn;
            //            }
            //        }
            //    }
            //}


            //for (int x = 0; x < w2; x++)
            //{
            //    for (int y = 0; y < h2; y++)
            //    {
            //        //float fn = noise.Noise(size * x, size * y, 5, 0.9f) + 1f; // -1 .. 1
            //        float fn = noiseS.func_151601_a(size * x, size * y) + 2.5f;
            //        //ar[x, y] = Result128(fn, 51.2f);
            //        ar[x, y] = Result2(fn);

            //        if (fn < fmin) fmin = fn;
            //        if (fn > fmax) fmax = fn;

            //        //ar[x, y] = Result128(fn, 128f);
            //    }
            //}

            long ms = stopwatch.ElapsedTicks * 1000 / Stopwatch.Frequency;

           // float d = (float)Math.Pow(2, octave - 1);

            pictureBox1.Image = new Bitmap(w, h);
            g = System.Drawing.Graphics.FromImage(pictureBox1.Image);

            for (int x = 0; x < w2; x++)
            {
                for (int y = 0; y < h2; y++)
                {
                    Pixel(x, y, ar[x, y] / dl);
                    //PixelHeight(x, y, ar[x, y]);
                    //PixelWetness(x, y, ar[x, y]);
                }
            }

            pictureBox1.Refresh();
            long ms2 = stopwatch.ElapsedTicks * 1000 / Stopwatch.Frequency;
            // 280 ms работа с Graphics
            Text = string.Format(name + " {1} ms {0} ms [{2:0.00}; {3:0.00}]",
                ms, ms2, fmin, fmax
            );
        }

        /// <summary>
        /// Флоат от 0 и до Х превести к 0 - 255
        /// </summary>
        /// <param name="fn">значение</param>
        /// <param name="k">коэфициент умнажения, при 0-2 коэф 128 при 0-1 коэф 256</param>
        //protected int Result128(float fn, float k)
        //{
        //    if (fn < 0) fn = 0;
        //    fn = fn * k;
        //    if (fn > 255) fn = 255;
        //    return (int)fn;
        //}

        //protected int Result2(float fn)
        //{
        //    int i = Result128(fn, 51.2f);
        //    int j = i / 64;

        //    return j * 64;
        //}
        
        protected void PixelWetness(int x, int y, float col)
        {
            Color c = Color.Gray;
            if (col < -2f) c = Color.Orange;
            else if (col < 1f) c = Color.Green;
            else if (col < 4f) c = Color.DarkGreen;
            Pixel(x, y, c);
        }
        protected void PixelHeight(int x, int y, float col)
        {
            int c = 0;
            if (col < -3.8f) c = 255;
            else if (col < 0f) c = 128;
            else if (col < 0.2f) c = 160;
            Pixel(x, y, Color.FromArgb(255, c, c, c));
        }
        protected void Pixel(int x, int y, float col)
        {
            int c = (int)(col * 128f + 128);
            Pixel(x, y, Color.FromArgb(255, c, c, c));
        }

        protected void Pixel(int x, int y, Color color)
        {
            g.FillRectangle(new SolidBrush(color), x, y, 1, 1);

            //g.Dre.DrawLine(new Pen(color, 1), x, y, x, y);
            //g.DrawRectangle(new Pen(color, 0), new Rectangle(x, y, 1, 1));
        }

        
    }
}*/

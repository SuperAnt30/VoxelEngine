using VoxelEngine.Glm;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Форма движка
    /// </summary>
    public partial class FormGame : Form
    {
        protected bool isFullScreen = false;
        /// <summary>
        /// Счётчик FPS
        /// </summary>
        private CounterTick counterFps = new CounterTick();
        /// <summary>
        /// Счётчик TPS
        /// </summary>
        private CounterTick counterTps = new CounterTick();
        /// <summary>
        /// Отдельный поток FPS
        /// </summary>
        private ThreadTick threadFps = new ThreadTick();
        /// <summary>
        /// Отдельный поток TPS
        /// </summary>
        private ThreadTick threadTps = new ThreadTick();
        /// <summary>
        /// Отдельный поток мира
        /// </summary>
        private ThreadWorld threadWorld = new ThreadWorld();
        /// <summary>
        /// Отдельные потоки загрузки чанков
        /// </summary>
        private ThreadChunk[] threadChunks = new ThreadChunk[8];

        public FormGame()
        {

            int it1 = -146;
            int it2 = it1 & 15;
            int it3 = it1 >> 4;

            glm.Initialized();
            InitializeComponent();
            

            counterFps.Tick += CounterFps_Tick;
            threadTps.SetTps(20);
            threadTps.ThreadDone += ThreadTps_ThreadDone;
            threadTps.Stoped += Thread_Stoped;

            //threadFps.Interval = 240000; // TPS 40
            //threadFps.Interval = 156666; // TPS 60
            //threadFps.Interval = 73333; // TPS 120
            threadFps.SetTps(60);
            threadFps.ThreadDone += ThreadFps_ThreadDone;
            threadFps.Stoped += Thread_Stoped;

            threadWorld.ThreadDone += ThreadWorld_ThreadDone;
            threadWorld.Stoped += Thread_Stoped;
        }

        #region Form

        /// <summary>
        /// Загрузка формы
        /// </summary>
        private void FormGame_Load(object sender, EventArgs e)
        {
            WinApi.TimeBeginPeriod(1);
            threadFps.Start();
            threadTps.Start();
            new Thread(threadFps.Run).Start();
            new Thread(threadTps.Run).Start();
            new Thread(threadWorld.Run).Start();

            for (int i = 0; i < threadChunks.Length; i++)
            {
                new Thread(threadChunks[i].Run).Start();
            }
        }

        /// <summary>
        /// Подготовка к закрытию формы
        /// </summary>
        private void FormGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadTps.IsRun)
            {
                e.Cancel = true;
                threadTps.Stop();
                return;
            }
            if (threadFps.IsRun)
            {
                e.Cancel = true;
                threadFps.Stop();
                return;
            }
            if (threadWorld.IsRun)
            {
                e.Cancel = true;
                threadWorld.StopSave();
                return;
            }
            for (int i = 0; i < threadChunks.Length; i++)
            {
                if (threadChunks[i].IsRun)
                {
                    e.Cancel = true;
                    threadChunks[i].Stop();
                    break;
                }
            }

            WinApi.TimeEndPeriod(1);
        }

        #endregion

        #region Потоки

        /// <summary>
        /// Сгенерированного мира со списками чанков
        /// </summary>
        private void ThreadWorld_ThreadDone(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke(new EventHandler(ThreadWorld_ThreadDone), sender, e);
            else
            {
                for (int i = 0; i < threadChunks.Length; i++)
                {
                    threadChunks[i].Done();
                }
            }
        }

        /// <summary>
        /// Сгенерирован чанк
        /// </summary>
        private void ThreadChunk_ChunkDone(object sender, ChunkEventArgs e)
        {
            if (InvokeRequired) Invoke(new ChunkEventHandler(ThreadChunk_ChunkDone), sender, e);
            else
            {
                if (!e.IsAlpha)
                {
                    //Debag.GetInstance().CountTest++;
                    OpenGLF.GetInstance().WorldM.RenderChank(e.Chunk.X, e.Chunk.Z, e.Chunk.ToBuffer());
                    // можно очистить буфер сетки
                    //e.Chunk.ClearBuffer(); // нельзя, так как заного рендерим когда перемещаемся
                }
                OpenGLF.GetInstance().WorldM.RenderChankAlpha(e.Chunk.X, e.Chunk.Z, e.Chunk.ToBufferAlpha());
                //OpenGLF.GetInstance().WorldM.RenderChank(e.Chunk.X, e.Chunk.Z,
                //    e.IsAlpha ? e.Chunk.ToBufferAlpha() :
                //    e.Chunk.ToBuffer());
            }
        }

        ///// <summary>
        ///// Сгенерирован чанк
        ///// </summary>
        //private void ThreadChunk_ChunkAlphaDone(object sender, ChunkEventArgs e)
        //{
        //    if (InvokeRequired) Invoke(new ChunkEventHandler(ThreadChunk_ChunkAlphaDone), sender, e);
        //    else
        //    {
        //        OpenGLF.GetInstance().WorldM.RenderChankAlpha(e.Chunk.X, e.Chunk.Z, e.Chunk.ToBufferAlpha());
        //    }
        //}

        /// <summary>
        /// Тик FPS
        /// </summary>
        private void ThreadFps_ThreadDone(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke(new EventHandler(ThreadFps_ThreadDone), sender, e);
            else openGLControl1.DoRender();
        }

        /// <summary>
        /// Тик TPS
        /// </summary>
        private void ThreadTps_ThreadDone(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke(new EventHandler(ThreadTps_ThreadDone), sender, e);
            else
            {
                Debag.GetInstance().TickCount++;
                counterTps.CalculateFrameRate();

                // Тики в других объектах
                OpenGLF.GetInstance().Tick();
                threadWorld.World.Tick();

                //Text = "Game by SuperAnt " + string.Format("FPS {1} TPS {2} CountMesh {0}",
                //    Debag.GetInstance().CountMesh, counterFps.Fps, counterTps.Fps);
            }
        }

        /// <summary>
        /// Остановка потока для закрытия приложения
        /// </summary>
        private void Thread_Stoped(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke(new EventHandler(Thread_Stoped), sender, e);
            else Close();
        }

        #endregion

        #region OpenGL

        /// <summary>
        /// Инициализация OpenGL
        /// </summary>
        private void openGLControl1_OpenGLInitialized(object sender, EventArgs e)
        {
            OpenGLF.GetInstance().Initialized(openGLControl1.OpenGL);
            //OpenGLF.GetInstance().Cam = new Camera(new vec3(-24, 7, -24), glm.radians(70.0f));
            OpenGLF.GetInstance().Cam = new Camera(new vec3(0, 70, 0), glm.radians(70.0f));
            // TODO::Load
            WorldFile.Load();
            //OpenGLF.GetInstance().Cam.Rotate(0, glm.radians(20f), 0);
            //OpenGLF.GetInstance().Cam.Rotate(0, glm.radians(-70f), 0);
            //OpenGLF.GetInstance().Cam = new Camera(new vec3(100000, 7, 24), glm.radians(70.0f));
            //OpenGLF.GetInstance().Cam = new Camera(new vec3(0, 70, 0), glm.radians(70.0f));
            OpenGLF.GetInstance().Cam.PositionChunkChanged += Cam_PositionChunkChanged;
            OpenGLF.GetInstance().Cam.PositionBlockChanged += Cam_PositionBlockChanged;
            for (int i = 0; i < threadChunks.Length; i++)
            {
                threadChunks[i] = new ThreadChunk(threadWorld.World, i);
                //if (i < 8)
                //threadChunks[i].ChunkDone += ThreadChunk_ChunkDone;
                //else threadChunks[i].ChunkDone += ThreadChunk_ChunkAlphaDone;
                threadChunks[i].Stoped += Thread_Stoped;
            }
            threadWorld.World.ChunkDone += ThreadChunk_ChunkDone;
            threadWorld.World.VoxelChanged += FormGame_VoxelChanged;

           // OpenGLF.GetInstance().WorldM.World = threadWorld.World;

            Keyboard.GetInstance().VoxelChanged += FormGame_VoxelChanged;
            Keyboard.GetInstance().MoveChanged += FormGame_MoveChanged;
            Keyboard.GetInstance().World = threadWorld.World;
            //Mouse.GetInstance().VoxelChanged += FormGame_VoxelChanged;
            Mouse.GetInstance().MoveChanged += FormGame_MoveChanged;
            Mouse.GetInstance().World = threadWorld.World;
            Debag.GetInstance().StartTime();
            Debag.GetInstance().World = threadWorld.World;
            _RenderWorld(OpenGLF.GetInstance().Cam.ToPositionChunk(), ThreadWorld.RenderType.Dense);
        }

       

        /// <summary>
        /// Изменён размер окна OpenGL
        /// </summary>
        private void openGLControl1_Resized(object sender, EventArgs e)
        {
            OpenGLF.GetInstance().Resized(openGLControl1.Size);
        }

        /// <summary>
        /// Прорисовка OpenGL (1 тик)
        /// </summary>
        private void openGLControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            counterFps.CalculateFrameRate();
            OpenGLF.GetInstance().Draw();
        }

        #endregion

        #region Mouse

        /// <summary>
        /// Движение мышки
        /// </summary>
        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            Mouse.GetInstance().Move(e.Location, Bounds, MousePosition);
        }

        /// <summary>
        /// Клик мышки
        /// </summary>
        private void openGLControl1_Click(object sender, EventArgs e)
        {
            Mouse.GetInstance().RunMove();
            
        }

        /// <summary>
        /// Нажатие мышки
        /// </summary>
        private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            Mouse.GetInstance().Down(e);
        }

        #endregion

        #region Keyboard

        /// <summary>
        /// Нажата клавиша
        /// </summary>
        private void openGLControl1_KeyDown(object sender, KeyEventArgs e)
        {
            // Debag.GetInstance().BeginTick = System.DateTime.Now.Ticks;
            //Debag.Log("LogKey", e.KeyCode.ToString());

            Keyboard.GetInstance().KeyDown(e.KeyCode);
            if (e.KeyCode == Keys.F11)
            {
                isFullScreen = !isFullScreen;
                if (isFullScreen)
                {
                    TopMost = true;
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                } else
                {
                    TopMost = false;
                    FormBorderStyle = FormBorderStyle.Sizable;
                    WindowState = FormWindowState.Normal;
                }
            }
            else if (e.KeyCode == Keys.O)
            {
                // Перерендер
                Debag.GetInstance().StartTime();
                threadWorld.World.ChunkRendersClear();
                Cam_PositionChunkChanged(sender, new EventArgs());
            }
        }

        private void openGLControl1_KeyUp(object sender, KeyEventArgs e)
        {
            Keyboard.GetInstance().KeyUp(e.KeyCode);
        }

        /// <summary>
        /// Нажата специальная клавиша
        /// </summary>
        private void openGLControl1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Keyboard.GetInstance().PreviewKeyDown(e.KeyCode);
        }

        #endregion

        /// <summary>
        /// Тик счётчика
        /// </summary>
        private void CounterFps_Tick(object sender, EventArgs e)
        {
            Debag d = Debag.GetInstance();
            d.Fps = counterFps.CountTick;
            d.Tps = counterTps.CountTick;
            d.SpeedFrame = d.Fps == 0 ? 0 : d.CountFrame / ((float)d.Fps * System.Diagnostics.Stopwatch.Frequency / 1000f);
            d.CountFrame = 0;
        }

        

        /// <summary>
        /// Движение камеры или обзора камеры
        /// </summary>
        private void FormGame_MoveChanged(object sender, EventArgs e)
        {
            OpenGLF openGLF = OpenGLF.GetInstance();
            //Voxel vox = threadWorld.World.RayCast(openGLF.Cam.Position, openGLF.Cam.Front, 10.0f, out vec3 end, out vec3i norm, out vec3i iend);
            Block block = threadWorld.World.RayCast(openGLF.Cam.Position, openGLF.Cam.Front, 10.0f, out vec3 end, out vec3i norm, out vec3i iend);
            //Debag.GetInstance().BB = norm.ToString();
            if (block.Id > 0)
            {
                float size = 1.01f;
                openGLF.WorldLineM.Box("cursor", iend.x + .5f, iend.y + .5f, iend.z + .5f, size, size, size, .9f, .9f, .1f, .6f);
            }
            else
            {
                openGLF.WorldLineM.Remove("cursor");
            }
            Debag.GetInstance().RayCastBlock = block;
        }

        /// <summary>
        /// Изменён воксель
        /// </summary>
        private void FormGame_VoxelChanged(object sender, VoxelEventArgs e)
        {
            if (e.Beside.Length == 0)
            {
                _RenderWorld(Camera.ToPositionChunk(e.Position), ThreadWorld.RenderType.DenseOne);
            } else
            {
                // с соседним чанкум
                vec2i pos = Camera.ToPositionChunk(e.Position);
                
                for (int i = 0; i < threadChunks.Length; i++)
                {
                    threadChunks[i].Pause();
                }
                vec2i[] beside = new vec2i[e.Beside.Length];
                for (int i = 0; i < e.Beside.Length; i++)
                {
                    beside[i] = new vec2i(pos.x + e.Beside[i].x, pos.y + e.Beside[i].y);
                }
                threadWorld.SetVoxel(pos, beside);
            }
            FormGame_MoveChanged(sender, new EventArgs());
        }

        /// <summary>
        /// Событие изменение позиции камеры на другой чанк
        /// </summary>
        private void Cam_PositionChunkChanged(object sender, EventArgs e)
        {
            _RenderWorld(OpenGLF.GetInstance().Cam.ToPositionChunk(), ThreadWorld.RenderType.All); // был All
            if (Debag.GetInstance().IsDrawChunk) 
            {
                OpenGLF.GetInstance().WorldLineM.Chunk();
            }

            OpenGLF.GetInstance().WorldM.RemoveAway(OpenGLF.GetInstance().Cam.ToPositionChunk());
            
        }

        private void Cam_PositionBlockChanged(object sender, EventArgs e)
        {
            // TODO:: 2021-07-15 обнуляет очередь загрузки чанков, из-за этого глюки. 
            //_RenderWorld(OpenGLF.GetInstance().Cam.ToPositionChunk(), ThreadWorld.RenderType.AlphaOne);
        }

        /// <summary>
        /// Подготовка для генерации мира
        /// </summary>
        private void _RenderWorld(vec2i chunk, ThreadWorld.RenderType render)
        {
            for (int i = 0; i < threadChunks.Length; i++)
            {
                threadChunks[i].Pause();
            }
            threadWorld.SetCenterPosition(chunk, render);
        }

        
    }
}

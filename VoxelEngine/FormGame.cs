using VoxelEngine.Glm;
using System;
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
        //private ThreadTick threadFps = new ThreadTick();
        /// <summary>
        /// Отдельный поток TPS
        /// </summary>
        private ThreadTick threadTps = new ThreadTick();
        /// <summary>
        /// Объект мира
        /// </summary>
        public WorldRender World { get; protected set; } = new WorldRender();
        /// <summary>
        /// Пометка первого такта
        /// </summary>
        protected bool startTick = true;

        public FormGame()
        {
            glm.Initialized();
            VES.GetInstance();
            InitializeComponent();

            counterFps.Tick += CounterFps_Tick;
            //threadTps.SetTps(20);
            //threadTps.ThreadDone += ThreadTps_ThreadDone;
            //threadTps.Stoped += Thread_Stoped;

            //threadFps.Interval = 240000; // TPS 40
            //threadFps.Interval = 156666; // TPS 60
            //threadFps.Interval = 73333; // TPS 120
            //threadFps.SetTps(60);
            //threadFps.ThreadDone += ThreadFps_ThreadDone;
            //threadFps.Stoped += Thread_Stoped;
        }

        #region Form

        /// <summary>
        /// Загрузка формы
        /// </summary>
        private void FormGame_Load(object sender, EventArgs e)
        {
            WinApi.TimeBeginPeriod(1);

            //Test test = new Test();
            //test.ArrayChunk();
            timer1.Start();

            //threadFps.Start();
            //threadTps.Start();
           // new Thread(threadFps.Run).Start();
            //new Thread(threadTps.Run).Start();
        }

        /// <summary>
        /// Подготовка к закрытию формы
        /// </summary>
        private void FormGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (threadTps.IsRun)
            //{
            //    e.Cancel = true;
            //    threadTps.Stop();
            //    return;
            //}
            //if (threadFps.IsRun)
            //{
            //    e.Cancel = true;
            //    threadFps.Stop();
            //    return;
            //}
            isClosing = true;
            if (isClosed)
            {
                World.RegionPr.RegionsWrite();
                WinApi.TimeEndPeriod(1);
            }
            else
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region Потоки

        

        /// <summary>
        /// Тик FPS
        /// </summary>
        //private void ThreadFps_ThreadDone(object sender, EventArgs e)
        //{
        //    if (InvokeRequired) Invoke(new EventHandler(ThreadFps_ThreadDone), sender, e);
        //    else openGLControl1.DoRender();
        //}

        /// <summary>
        /// Тик TPS
        /// </summary>
        private void ThreadTps_ThreadDone(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke(new EventHandler(ThreadTps_ThreadDone), sender, e);
            else Tick(); 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Tick();
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
            OpenGLF.GetInstance().RemoveChunkMeshChanged += OpenGLFRemoveChunkMeshChanged;
            // TODO::Load
            WorldFile.Load();
            //OpenGLF.GetInstance().Cam.Rotate(0, glm.radians(20f), 0);
            //OpenGLF.GetInstance().Cam.Rotate(0, glm.radians(-70f), 0);
            //OpenGLF.GetInstance().Cam = new Camera(new vec3(100000, 7, 24), glm.radians(70.0f));
            //OpenGLF.GetInstance().Cam = new Camera(new vec3(0, 70, 0), glm.radians(70.0f));
            OpenGLF.GetInstance().Cam.PositionChunkChanged += Cam_PositionChunkChanged;
            OpenGLF.GetInstance().Cam.PositionBlockChanged += Cam_PositionBlockChanged;
            World.ChunkDone += WorldChunkDone;
            World.VoxelChanged += WorldVoxelChanged;
            World.Rendered += WorldRendered;
            World.Cleaned += WorldCleaned;
            World.Ticked += WorldTicked;

            Keyboard.GetInstance().MoveChanged += FormGame_MoveChanged;
            Keyboard.GetInstance().World = World;
            Mouse.GetInstance().MoveChanged += FormGame_MoveChanged;
            Mouse.GetInstance().World = World;
            Debag.GetInstance().StartTime();
            Debag.GetInstance().World = World;
        }

        

        private void OpenGLFRemoveChunkMeshChanged(object sender, CoordEventArgs e)
        {
            // Задача если чанк с сеткой удаляется, надо пометить в кэше чанков, для будущего рендера
            ChunkRender chunk = World.GetChunkRender(e.Position2d.x, e.Position2d.y);
            if (chunk != null) chunk.ModifiedToRender();
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
                World.ChunksClear();
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
            Block block = World.RayCast(openGLF.Cam.Position, openGLF.Cam.Front, 10.0f, out vec3 end, out vec3i norm, out vec3i iend);
            if (!block.IsAir)
            {
                float size = 1.01f;
                openGLF.WorldLineM.Box("cursor", iend.x + .5f, iend.y + .5f, iend.z + .5f, size, size, size, .9f, .9f, .1f, .6f);
            }
            else
            {
                openGLF.WorldLineM.Remove("cursor");
            }
            Debag.GetInstance().RayCastBlock = block;
            Debag.GetInstance().RayCastBlockUp = World.GetBlock(new BlockPos(block.Position.X, block.Position.Y + 1f, block.Position.Z));
        }

        /// <summary>
        /// Изменён воксель
        /// </summary>
        private void WorldVoxelChanged(object sender, VoxelEventArgs e)
        {

        }

        /// <summary>
        /// Сгенерирован чанк
        /// </summary>
        private void WorldChunkDone(object sender, BufferEventArgs e)
        {
            if (InvokeRequired) Invoke(new BufferEventHandler(WorldChunkDone), sender, e);
            else
            {
                if (!e.IsAlpha)
                {
                    // Если пометка не альфа, то сетка твёрдых блоков
                    OpenGLF.GetInstance().WorldM.RenderChank(e.ChunkPos.x, e.ChunkPos.y, e.Buffer);
                }
                // Генерация альфы всегда есть
                OpenGLF.GetInstance().WorldM.RenderChankAlpha(e.ChunkPos.x, e.ChunkPos.y, e.BufferAlpha);
            }
        }

        /// <summary>
        /// Подготовка к закрытию
        /// </summary>
        protected bool isClosing = false;
        /// <summary>
        /// Потоки приостановленны для закрытия
        /// </summary>
        protected bool isClosed = false;
        /// <summary>
        /// Этап очистки, определяется по смене чанка, в этот момент глушится загрузка чанков
        /// </summary>
        protected bool isCleaning = false;

        private void WorldRendered(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke(new EventHandler(WorldRendered), sender, e);
            else
            {
                if (isClosing)
                {
                    isClosed = true;
                    Close();
                }
                else
                {
                    if (isCleaning)
                    {
                        World.PackageCleaning();
                    }
                    else
                    {
                        World.PackageRender();
                    }
                }
            }
        }

        private void WorldCleaned(object sender, EventArgs e)
        {
            isCleaning = false;
            World.ChunckChanged = VE.CHUNK_RENDER_ALPHA;
            World.PackageRender();
        }

        private void WorldTicked(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Событие изменение позиции камеры на другой чанк
        /// </summary>
        private void Cam_PositionChunkChanged(object sender, EventArgs e)
        {
            if (Debag.GetInstance().IsDrawChunk) 
            {
                OpenGLF.GetInstance().WorldLineM.Chunk();
            }
            vec2i c = OpenGLF.GetInstance().Cam.ToPositionChunk();
            OpenGLF.GetInstance().WorldM.Cleaning(c);
            isCleaning = true;
            ChunkRender chunk = World.GetChunkRender(c.x, c.y);
            Debag.GetInstance().ChunkAlpheBlock = chunk == null
                ? 0 : chunk.CountBlockAlpha();
        }

        private void Cam_PositionBlockChanged(object sender, EventArgs e)
        {
            World.ChunckChanged = VE.CHUNK_RENDER_ALPHA_BLOCK;
        }

        protected void Tick()
        {
            Debag.GetInstance().TickCount++;
            counterTps.CalculateFrameRate();

            // Тики в других объектах
            OpenGLF.GetInstance().Tick();
            Keyboard.GetInstance().PlCamera.Tick();
            World.PackageTick();

            if (startTick)
            {
                World.PackageRender();
                startTick = false;
            }
        }
    }
}

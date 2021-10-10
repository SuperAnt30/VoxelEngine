using VoxelEngine.Glm;
using System;
using System.Windows.Forms;
using VoxelEngine.Util;
using VoxelEngine.Actions;
using VoxelEngine.Renderer.Chk;
using VoxelEngine.Vxl;
//using VoxelEngine.World.Blk;
using VoxelEngine.Renderer;
using VoxelEngine.Graphics;
using VoxelEngine.Entity;
using System.Diagnostics;
using System.Threading;

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
        /// Счётчик Рендер чанков
        /// </summary>
        private CounterTick counterRc = new CounterTick();
        /// <summary>
        /// Счётчик Рендер чанков альфа
        /// </summary>
        private CounterTick counterRca = new CounterTick();
        /// <summary>
        /// Отдельный поток FPS
        /// </summary>
        private ThreadTick threadFps = new ThreadTick();
        /// <summary>
        /// Объект мира
        /// </summary>
        public WorldRender World { get; protected set; } = new WorldRender();
        /// <summary>
        /// Пометка первого такта
        /// </summary>
        protected bool startTick = true;
        /// <summary>
        /// Таймер для фиксации времени c запуска приложения
        /// </summary>
        protected Stopwatch stopwatch = new Stopwatch();
        /// <summary>
        /// Таймер для фиксации времени одного кадра
        /// </summary>
        protected Stopwatch stopwatchFrame = new Stopwatch();

        public FormGame()
        {
            glm.Initialized();
            VES.GetInstance();
            InitializeComponent();

            counterFps.Tick += CounterFps_Tick;

            threadFps.SetTps(60); // FPS
            threadFps.ThreadDone += ThreadFps_ThreadDone;
            threadFps.Stoped += Thread_Stoped;
        }

        #region Form

        /// <summary>
        /// Загрузка формы
        /// </summary>
        private void FormGame_Load(object sender, EventArgs e)
        {
            WinApi.TimeBeginPeriod(1);

            threadFps.Start();
            new Thread(threadFps.Run).Start();
        }

        /// <summary>
        /// Подготовка к закрытию формы
        /// </summary>
        private void FormGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            World.TickStop();

            if (threadFps.IsRun)
            {
                e.Cancel = true;
                threadFps.Stop();
                return;
            }

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
        private void ThreadFps_ThreadDone(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke(new EventHandler(ThreadFps_ThreadDone), sender, e);
            else openGLControl1.DoRender();
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
            stopwatch.Start();
            stopwatchFrame.Start();
            Mouse.GetInstance().SetWorld(World);
            OpenGLF.GetInstance().Initialized(openGLControl1.OpenGL);
            OpenGLF.GetInstance().Config = VEC.GetInstance();
            OpenGLF.GetInstance().Cam = new Camera(new vec3(0, 70, 0), glm.radians(70.0f));
            //OpenGLF.GetInstance().Cam = new Camera(new vec3(100000, 7, 24), glm.radians(70.0f));
            OpenGLF.GetInstance().RemoveChunkMeshChanged += OpenGLFRemoveChunkMeshChanged;
            // TODO::Load
            WorldFile.Load(World);
            
            OpenGLF.GetInstance().Cam.PositionChunkChanged += Cam_PositionChunkChanged;
            OpenGLF.GetInstance().Cam.PositionBlockChanged += Cam_PositionBlockChanged;
            OpenGLF.GetInstance().Cam.SetPosRotation(
                World.Entity.HitBox.Position,
                World.Entity.RotationYaw,
                World.Entity.RotationPitch
            );
            //OpenGLF.GetInstance().Cam.HitBox.HitBoxChanged += HitBox_Changed;
            World.Done += WorldRenderDone;
            World.VoxelChanged += WorldVoxelChanged;
            World.Rendered += WorldRendered;
            World.Cleaned += WorldCleaned;
            World.Ticked += WorldTicked;
            World.HitBoxChanged += WorldHitBoxChanged;
            World.LookAtChanged += WorldLookAtChanged;

            Keyboard.GetInstance().MoveChanged += FormGame_MoveChanged;
            Mouse.GetInstance().MoveChanged += FormGame_MoveChanged;

            Keyboard.GetInstance().SetWorld(World);
            Debug.GetInstance().SetWorld(World);

            World.PackageRender();
            Tick();
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
        private void openGLControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs e)
        {
            if (InvokeRequired) Invoke(new SharpGL.RenderEventHandler(openGLControl1_OpenGLDraw), sender, e);
            else
            {
                Draw();
            }
        }

        protected void Draw()
        {
            float timeAll = stopwatch.ElapsedMilliseconds / 1000f;
            // Время за один прошедший кадр, секунд
            float timeFrame = stopwatchFrame.ElapsedMilliseconds / 1000f;
            stopwatchFrame.Restart();
            if (timeFrame > 1.5f) timeFrame = 1.5f;

            World.PackageEntities(timeFrame, timeAll);
            counterFps.CalculateFrameRate();
            OpenGLF.GetInstance().Draw(timeFrame, timeAll);

            Debug.GetInstance().CountFrame += stopwatchFrame.ElapsedTicks;
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
            Debug d = Debug.GetInstance();
            d.Fps = counterFps.CountTick;
            d.Tps = counterTps.CountTick;
            d.Rc = counterRc.CountTick;
            d.Rca = counterRca.CountTick;
            d.SpeedFrame = d.Fps == 0 ? 0 : d.CountFrame / ((float)d.Fps * System.Diagnostics.Stopwatch.Frequency / 1000f);
            d.CountFrame = 0;
        }

        /// <summary>
        /// Движение камеры или обзора камеры
        /// </summary>
        private void FormGame_MoveChanged(object sender, EventArgs e)
        {
            OpenGLF openGLF = OpenGLF.GetInstance();
            World.Camera(openGLF.Cam.PosPlus(), openGLF.Cam.Front);
            MovingObjectPosition moving = World.RayCast(World.CameraPosition, World.CameraDirection, VE.MAX_DIST);
            if (moving.IsBlock() && !moving.Block.IsAir)
            {
                vec3 from = moving.Block.HitBox.From;
                vec3 size = moving.Block.HitBox.Size;
                //vec3 size2 = size;
                vec3 bias = size / 2f;
                size += new vec3(.01f);
                //size2 -= new vec3(.01f);
                openGLF.WorldLineM.Box("cursor",
                    moving.IEnd.x + bias.x + from.x, moving.IEnd.y + bias.y + from.y, moving.IEnd.z + bias.z + from.z, 
                    size.x, size.y, size.z, .2f, .2f, .2f, 1.0f);
                //openGLF.WorldLineM.Box("cursor2",
                //    iend.x + bias.x + from.x, iend.y + bias.y + from.y, iend.z + bias.z + from.z,
                //    size2.x, size2.y, size2.z, .9f, .9f, .1f, .6f);
                Debug.GetInstance().RayCastBlockUp = World.GetBlock(new BlockPos(moving.Block.Position.X, moving.Block.Position.Y + 1f, moving.Block.Position.Z));
            }
            else
            {
                openGLF.WorldLineM.Remove("cursor");
                //openGLF.WorldLineM.Remove("cursor2");
                Debug.GetInstance().RayCastBlockUp = null;
            }
            Debug.GetInstance().RayCastObject = moving;
        }

        /// <summary>
        /// Изменён воксель
        /// </summary>
        private void WorldVoxelChanged(object sender, VoxelEventArgs e)
        {

        }

        /// <summary>
        /// Изменена камера обзора
        /// </summary>
        private void WorldLookAtChanged(object sender, EventArgs e)
        {
            OpenGLF.GetInstance().Cam.SetFov(glm.radians(70 + World.Entity.Moving.Sprinting.Moving * 10f));
            OpenGLF.GetInstance().Cam.SetEyesWater(World.Entity.HitBox.Size.Eyes, World.Entity.HitBox.IsEyesWater);
        }

        /// <summary>
        /// Сгенерировано
        /// </summary>
        private void WorldRenderDone(object sender, BufferEventArgs e)
        {
            try
            {
                if (InvokeRequired) Invoke(new BufferEventHandler(WorldRenderDone), sender, e);
                else
                {
                    WorldMesh world = OpenGLF.GetInstance().WorldM;
                    if (e.Answer == BufferEventArgs.EnumAnswer.ChunkAll)
                    {
                        // Если пометка не альфа, то сетка твёрдых блоков
                        world.RenderChank(e.ChunkPos.x, e.ChunkPos.y, e.Buffer);
                        counterRc.CalculateFrameRate();
                    }
                    if (e.Answer == BufferEventArgs.EnumAnswer.ChunkAll || e.Answer == BufferEventArgs.EnumAnswer.ChunkAlpha)
                    {
                        // Генерация альфы всегда есть
                        world.RenderChankAlpha(e.ChunkPos.x, e.ChunkPos.y, e.BufferAlpha);
                        counterRca.CalculateFrameRate();
                    }
                    else if (e.Answer == BufferEventArgs.EnumAnswer.Entity)
                    {
                        // Генерация моба
                        world.RenderEntity(e.Index, e.KeyEntity, e.Buffer);
                    }
                }
            }
            catch { }

        }

        private void WorldHitBoxChanged(object sender, EntityEventArgs e)
        {
            if (InvokeRequired) Invoke(new EntityEventHandler(WorldHitBoxChanged), sender, e);
            else
            {
                OpenGLF.GetInstance().WorldLineM.RenderChank(
                    "HitBox" + e.Entity.HitBox.Index,
                    e.Entity.HitBox.Buffer
                );
            }
        }

        //private void HitBox_Changed(object sender, EventArgs e)
        //{
        //    OpenGLF.GetInstance().WorldLineM.RenderChank("HitBoxPlayer", OpenGLF.GetInstance().Cam.HitBox.Buffer);
        //}

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
                    counterRc.CalculateFrameRate(false);
                    counterRca.CalculateFrameRate(false);
                }
            }
        }

        private void WorldCleaned(object sender, EventArgs e)
        {
            isCleaning = false;
            World.ChunckChanged = VE.CHUNK_RENDER_ALPHA;
            World.PackageRender();
        }

        

        /// <summary>
        /// Событие изменение позиции камеры на другой чанк
        /// </summary>
        private void Cam_PositionChunkChanged(object sender, EventArgs e)
        {
            if (Debug.GetInstance().IsDrawChunk) 
            {
                OpenGLF.GetInstance().WorldLineM.Chunk();
            }
            vec2i c = OpenGLF.GetInstance().Cam.ChunkPos;
            OpenGLF.GetInstance().WorldM.Cleaning(c);
            isCleaning = true;
            ChunkRender chunk = World.GetChunkRender(c.x, c.y);
            Debug.GetInstance().ChunkAlpheBlock = chunk == null
                ? 0 : chunk.CountBlockAlpha();
        }

        private void Cam_PositionBlockChanged(object sender, EventArgs e)
        {
            World.ChunckChanged = VE.CHUNK_RENDER_ALPHA_BLOCK;
        }

        /// <summary>
        /// Событие завершения такта, и после этого можем приступать к следующему такту
        /// </summary>
        private void WorldTicked(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke(new EventHandler(WorldTicked), sender, e);
            else Tick();
        }

        protected void Tick()
        {
            // Счётчик ТPS
            counterTps.CalculateFrameRate();

            // Перемещение игрока
            Keyboard.GetInstance().PlCamera.Tick();
            // Счётик и свойства яркости неба и угла солнца
            VEC.GetInstance().Tick();
            // Атлас и дебаг
            OpenGLF.GetInstance().Tick();

            // В потоке все игровые события контролирует 50 мили секунд, для 20 TPS
            World.PackageTick();
        }
    }
}

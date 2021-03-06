using VoxelEngine.Glm;
using System;
using System.Windows.Forms;
using VoxelEngine.Util;
using VoxelEngine.Actions;
using VoxelEngine.Renderer.Chk;
using VoxelEngine.Vxl;
using VoxelEngine.Renderer;
using VoxelEngine.Graphics;
using VoxelEngine.Entity;
using System.Diagnostics;
using System.Threading;
using VoxelEngine.World.Chk;
using VoxelEngine.Audio;

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
        /// Счётчик Load чанков
        /// </summary>
        private CounterTick counterLc = new CounterTick();
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
        /// Запущен ли мир
        /// </summary>
        protected bool isWorld = false;
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
        
        protected bool testStop = false;

        /// <summary>
        /// Объект звуков
        /// </summary>
        protected AudioBase audio = new AudioBase();

        public FormGame()
        {
            VES.Initialized();
            InitializeComponent();
            Debug.Instance();
            openGLControl1.MouseWheel += openGLControl1_MouseWheel;

            counterFps.Tick += CounterFps_Tick;

            RefreshFps();
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

            guiControl1.Visible = false;
            threadFps.Start();
            new Thread(threadFps.Run).Start();
            audio.Initialize();

            GuiMenu();
        }

        /// <summary>
        /// Подготовка к закрытию формы
        /// </summary>
        private void FormGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isWorld)
            {
                WorldEnd(false);
            }

            if (threadFps.IsRun)
            {
                e.Cancel = true;
                threadFps.Stop();
                return;
            }

            WinApi.TimeEndPeriod(1);
        }

        private void FormGame_Deactivate(object sender, EventArgs e)
        {
            // Если форма приложения стала не активной, делаем курсор не в движке 3д
            Mouse.GetInstance().Move(false);
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

        #region World

        /// <summary>
        /// Запустить мир
        /// </summary>
        public void WorldBegin(string name, int seed)
        {
            OpenGLF.GetInstance().WorldBegin();

            // Создаём мир
            World = new WorldRender();
            Mouse.GetInstance().SetWorld(World);
            // Загружаем мир
            bool isLoad = WorldFile.ReadFile(name, World);
            if (!isLoad)
            {
                // Задаём сид для нового мира
                World.SetSeed(seed);
                // Анализ на позицию игрока если мир создался впервые
                World.Entity.HitBox.SetPos(new vec3(0, 80, 0));
            }
            // Привязки мира
            OpenGLF.GetInstance().Cam.SetPosRotation(
                World.Entity.HitBox.Position,
                World.Entity.RotationYaw,
                World.Entity.RotationPitch
            );
            World.UpdateModePlayer();
            World.Done += WorldRenderDone;
            World.Rendered += WorldRendered;
            World.Ticked += WorldTicked;
            World.HitBoxChanged += WorldHitBoxChanged;
            World.LookAtChanged += WorldLookAtChanged;
            World.LoadCache += WorldLoadCache;
            World.NotLoadCache += WorldNotLoadCache;

            audio.SetWorld(World);
            Keyboard.GetInstance().SetWorld(World);
            Debug.InstanceWorld(World);
            Tick();
            World.RunPackings();
            isWorld = true;
        }

        /// <summary>
        /// Закрыть мир
        /// </summary>
        public void WorldEnd(bool isMenu)
        {
            World.TickStop();
            World.RegionPr.RegionsWrite();
            isWorld = false;
            if (isMenu) GuiMenu();
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
            
            OpenGLF.GetInstance().Initialized(openGLControl1.OpenGL);
            OpenGLF.GetInstance().Cam = new Camera(new vec3(0, 70, 0), glm.radians(70.0f));
            OpenGLF.GetInstance().RemoveChunkMeshChanged += OpenGLFRemoveChunkMeshChanged;
            OpenGLF.GetInstance().Cam.PositionChunkChanged += Cam_PositionChunkChanged;
            OpenGLF.GetInstance().Cam.PositionBlockChanged += Cam_PositionBlockChanged;
            OpenGLF.GetInstance().Cam.WidgetChanged += Cam_WidgetChanged;

            // TODO::Load!!!
            //WorldFile.Load(World);
            //Mouse.GetInstance().SetWorld(World);
            //WorldFile.ReadFile("1", World);


            //OpenGLF.GetInstance().Cam.SetPosRotation(
            //    World.Entity.HitBox.Position,
            //    World.Entity.RotationYaw,
            //    World.Entity.RotationPitch
            //);
            //World.UpdateModePlayer();
            //World.Done += WorldRenderDone;
            //World.Rendered += WorldRendered;
            //World.Ticked += WorldTicked;
            //World.HitBoxChanged += WorldHitBoxChanged;
            //World.LookAtChanged += WorldLookAtChanged;
            //World.LoadCache += WorldLoadCache;
            //World.NotLoadCache += WorldNotLoadCache;

            //Keyboard.GetInstance().MoveChanged += FormGame_MoveChanged;
            //Mouse.GetInstance().MoveChanged += FormGame_MoveChanged;

            //Keyboard.GetInstance().SetWorld(World);
            //Debug.Instance(World);

            //Tick();
            //World.RunPackings();

            Keyboard.GetInstance().MoveChanged += FormGame_MoveChanged;
            Mouse.GetInstance().MoveChanged += FormGame_MoveChanged;
        }

        private void WorldNotLoadCache(object sender, EventArgs e)
        {
            counterLc.CalculateFrameRate(false);
            //World.PackageRender();
        }

        private void WorldLoadCache(object sender, EventArgs e) => counterLc.CalculateFrameRate();

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

            Debug.CountFrame += stopwatchFrame.ElapsedTicks;
        }


        /// <summary>
        /// Появился фокуса в 3д
        /// </summary>
        private void openGLControl1_Enter(object sender, EventArgs e)
        {
            if (guiControl1.Visible)
            {
                // Если фокус появился, а GUI активен, значит был клик по 3д
                // Возращаем фркус на GUI
                guiControl1.Focus();
            }
        }

        #endregion

        #region Mouse

        /// <summary>
        /// Вращение колёсика
        /// </summary>
        private void openGLControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0 && !PlayerWidget.IsOpenForm) Mouse.GetInstance().Wheel(e.Delta);
        }

        /// <summary>
        /// Движение мышки
        /// </summary>
        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!guiControl1.Visible) Mouse.GetInstance().Move(e.Location, Bounds, MousePosition);
        }

        /// <summary>
        /// Клик мышки
        /// </summary>
        private void openGLControl1_Click(object sender, EventArgs e)
        {
            if (!guiControl1.Visible) Mouse.GetInstance().RunMove();
        }

        /// <summary>
        /// Нажатие мышки
        /// </summary>
        private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!guiControl1.Visible) Mouse.GetInstance().Down(e);
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
            else if (e.KeyCode == Keys.E)
            {
                // Инвентарь
                Mouse.GetInstance().Move(false);
                GuiInventory();
            }
            else if (e.KeyCode == Keys.I || e.KeyCode == Keys.Escape)
            {
                // Опции
                Mouse.GetInstance().Move(false);
                GuiOptions();
            }
            else if (e.KeyCode == Keys.Insert)
            {
                testStop = true;
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

        #region GUI

        private void guiControl1_VisibleChanged(object sender, EventArgs e)
        {
            if (!guiControl1.Visible)
            {
                openGLControl1.Focus();
            }
        }
        /// <summary>
        /// Активация GUI опций
        /// </summary>
        protected void GuiOptions() => guiControl1.OpenOptions();
        /// <summary>
        /// Активация GUI инвентарь
        /// </summary>
        protected void GuiInventory() => guiControl1.OpenInventory();
        /// <summary>
        /// Активация Меню
        /// </summary>
        public void GuiMenu() => guiControl1.OpenMenu();

        #endregion

        #region Разное

        /// <summary>
        /// Обновить фпс
        /// </summary>
        public void RefreshFps() => threadFps.SetTps();
        /// <summary>
        /// Обновить угол обзора камеры
        /// </summary>
        public void RefreshFov() => OpenGLF.GetInstance().Cam.SetFov(glm.radians(70 + World.Entity.Moving.Sprinting.Moving * 10f));
        /// <summary>
        /// Тик счётчика
        /// </summary>
        private void CounterFps_Tick(object sender, EventArgs e)
        {
            Debug.Fps = counterFps.CountTick;
            Debug.Tps = counterTps.CountTick;
            Debug.Lc = counterLc.CountTick;
            Debug.Rc = counterRc.CountTick;
            Debug.Rca = counterRca.CountTick;
            Debug.SpeedFrame = Debug.Fps == 0 ? 0 : Debug.CountFrame / ((float)Debug.Fps * Stopwatch.Frequency / 1000f);
            Debug.CountFrame = 0;
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
                vec3 sizeB = size + new vec3(.01f);
                vec3 sizeO = size + new vec3(.015f);
                vec3 sizeY = size + new vec3(.02f);
                //size2 -= new vec3(.01f);
                openGLF.WorldLineM.Box("cursor",
                    moving.IEnd.x + bias.x + from.x, moving.IEnd.y + bias.y + from.y, moving.IEnd.z + bias.z + from.z, 
                    sizeB.x, sizeB.y, sizeB.z, .8f, .8f, .2f, 1.0f);
                openGLF.WorldLineM.Box("cursoro",
                    moving.IEnd.x + bias.x + from.x, moving.IEnd.y + bias.y + from.y, moving.IEnd.z + bias.z + from.z,
                    sizeO.x, sizeO.y, sizeO.z, .9f, .9f, .2f, 1.0f);
                openGLF.WorldLineM.Box("cursory",
                    moving.IEnd.x + bias.x + from.x, moving.IEnd.y + bias.y + from.y, moving.IEnd.z + bias.z + from.z,
                    sizeY.x, sizeY.y, sizeY.z, 1.0f, 1.0f, .2f, 1.0f);
                //openGLF.WorldLineM.Box("cursor2",
                //    iend.x + bias.x + from.x, iend.y + bias.y + from.y, iend.z + bias.z + from.z,
                //    size2.x, size2.y, size2.z, .9f, .9f, .1f, .6f);
                Debug.RayCastBlockUp = World.GetBlock(new BlockPos(moving.Block.Position.X, moving.Block.Position.Y + 1f, moving.Block.Position.Z));
                ChunkBase chunk = World.GetChunk(moving.Block.Position);
                Debug.BB = string.Format(
                    "h:{0} maxh:{1}",
                    chunk.Light.GetHeight(moving.Block.Position.X & 15, moving.Block.Position.Z & 15),
                    chunk.Light.HeightMapMax);
            }
            else
            {
                openGLF.WorldLineM.Remove("cursor");
                openGLF.WorldLineM.Remove("cursoro");
                openGLF.WorldLineM.Remove("cursory");
                //openGLF.WorldLineM.Remove("cursor2");
                Debug.RayCastBlockUp = null;
                //Debug.GetInstance().BB = "";
            }
            Debug.RayCastObject = moving;
        }

        /// <summary>
        /// Изменена камера обзора
        /// </summary>
        private void WorldLookAtChanged(object sender, EventArgs e)
        {
            RefreshFov();
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

        private void WorldRendered(object sender, EventArgs e)
        {
            try
            {
                if (InvokeRequired) Invoke(new EventHandler(WorldRendered), sender, e);
                else
                {
                    counterRc.CalculateFrameRate(false);
                    counterRca.CalculateFrameRate(false);
                }
            }
            catch { }
        }

        /// <summary>
        /// Событие изменение позиции камеры на другой чанк
        /// </summary>
        private void Cam_PositionChunkChanged(object sender, EventArgs e)
        {
            if (Debug.IsDrawChunk) 
            {
                OpenGLF.GetInstance().WorldLineM.Chunk();
            }
            vec2i c = OpenGLF.GetInstance().Cam.ChunkPos;
            OpenGLF.GetInstance().WorldM.Cleaning(c);
            World.CleaningTrue();
            ChunkRender chunk = World.GetChunkRender(c.x, c.y);
            Debug.ChunkAlpheBlock = chunk == null
                ? 0 : chunk.CountBlockAlpha();
        }

        /// <summary>
        /// Изменена позиция блока
        /// </summary>
        private void Cam_PositionBlockChanged(object sender, EventArgs e)
        {
            World.ChunkRenderAlphaBlock();
        }

        /// <summary>
        /// Обновить прорисовку виджета
        /// </summary>
        private void Cam_WidgetChanged(object sender, EventArgs e)
        {
            if (InvokeRequired) Invoke(new EventHandler(Cam_WidgetChanged), sender, e);
            else OpenGLF.GetInstance().Widget.RefreshDraw();
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
            Keyboard.GetInstance().KeyMove.PlCamera.Tick();
            // Счётик и свойства яркости неба и угла солнца
            VEC.Tick();
            // Атлас и дебаг
            OpenGLF.GetInstance().Tick();

            // В потоке все игровые события контролирует 50 мили секунд, для 20 TPS
            World.PackageTick();
        }

        #endregion
    }
}

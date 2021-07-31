using System;
using System.IO;
using System.Windows.Forms;
using VoxelEngine.Glm;

namespace VoxelEngine
{
    /// <summary>
    /// Объект одиночка клавиатуры
    /// </summary>
    public class Keyboard
    {
        #region Instance

        private static Keyboard instance;
        private Keyboard() { }

        /// <summary>
        /// Передать по ссылке объект если он создан, иначе создать
        /// </summary>
        /// <returns>объект Keyboard</returns>
        public static Keyboard GetInstance()
        {
            if (instance == null) instance = new Keyboard();
            return instance;
        }

        #endregion

        /// <summary>
        /// Объект мира
        /// </summary>
        public WorldRender World { get; set; }

        public PlayerCamera PlCamera { get; protected set; } = new PlayerCamera();

        public void PreviewKeyDown(Keys keys)
        {
            switch (keys)
            {
                case Keys.F3:
                    Debag.GetInstance().IsDraw = !Debag.GetInstance().IsDraw;
                    break;
                case Keys.F4:
                    VEC.GetInstance().Moving = VEC.VEMoving.FreeFlight;
                    break;
                case Keys.F5:
                    VEC.GetInstance().Moving = VEC.VEMoving.ObstacleFlight;
                    // save
                    //File.WriteAllBytes("map.dat", OpenGLF.GetInstance().ChunkItems.Write());
                    break;
                case Keys.F6:
                    VEC.GetInstance().Moving = VEC.VEMoving.Survival;
                    // load
                    //OpenGLF.GetInstance().ChunkItems.Read(File.ReadAllBytes("map.dat"));
                    break;
                case Keys.F7:
                    Debag.GetInstance().IsDrawChunk = !Debag.GetInstance().IsDrawChunk;
                    if (!Debag.GetInstance().IsDrawChunk)
                        OpenGLF.GetInstance().WorldLineM.Remove("chunk");
                    else
                        OpenGLF.GetInstance().WorldLineM.Chunk();
                    break;
                case Keys.F8:
                    Debag.GetInstance().IsDrawCollisium = !Debag.GetInstance().IsDrawCollisium;
                    if (!Debag.GetInstance().IsDrawCollisium)
                    {
                        OpenGLF.GetInstance().WorldLineM.Remove("HitBoxPlayer");
                    }
                    break;
                case Keys.Tab:
                    Mouse.GetInstance().Move();
                    break;
                case Keys.Escape:
                    Mouse.GetInstance().Move(false);
                    break;
                case Keys.Delete:

                    break;
                case Keys.ControlKey:
                    PlCamera.Speed();
                    break;

            }
        }


       // protected Keys _keysOld = Keys.None;

        public void KeyUp(Keys keys)
        {
            if (keys == Keys.D || keys == Keys.A) PlCamera.KeyUpHorizontal();
            else if (keys == Keys.W || keys == Keys.S) PlCamera.KeyUpVertical();
            else if (keys == Keys.Space) PlCamera.KeyUpJamp();
            else if (keys == Keys.ShiftKey) PlCamera.KeyUpSneaking();
            //PlCamera.KeyUp();
            //  _keysOld = Keys.None;
        }

        public void KeyDown(Keys keys)
        {
            //if (_keysOld == keys)
            //{
            //    // NEXT
            //    switch (keys)
            //    {
            //        case Keys.D:
            //            PlCamera.NextRight();
            //            OnMoveChanged();
            //            break;
            //    }
            //    return;
            //}

            //if (_keysOld == Keys.None)
            {
                //Debag.GetInstance().BeginTick = DateTime.Now.Ticks - 1000000;
              //  _keysOld = keys;
            }
            vec2i ch;
            vec3i bl;
            //Block blk;
            ChunkRender chunk;
            //Debag.Log("LogKey", keys.ToString());
            switch (keys)
            {
                case Keys.E:
                    OpenGLF.GetInstance().DrawLine();
                    break;
                case Keys.B:
                    //isDone = !isDone;
                    break;
                case Keys.P:
                    // Перегенерация
                    ch = OpenGLF.GetInstance().Cam.ToPositionChunk();
                    bl = OpenGLF.GetInstance().Cam.ToPositionBlock();
                    chunk = World.GetChunk(ch.x, ch.y);
                    chunk.Regen();
                    OnVoxelChanged(bl, new vec2i[0]);

                    //MediaPlayer player = new MediaPlayer();
                    //player.Open(new Uri(@"D:\Work\3d\TestAnimation\say1.mp3"));
                    //player.Volume = 1f;
                    //player.Balance = 0f;
                    //player.Play();
                    break;
                case Keys.L:
                    //ch = OpenGLF.GetInstance().Cam.ToPositionChunk();
                    //bl = OpenGLF.GetInstance().Cam.ToPositionBlock();
                    //blk = Debag.GetInstance().RayCastBlock;
                    //chunk = World.GetChunk(ch.x, ch.y);

                    ///World.CheckLight(blk.Position);


                    //chunk.GenerateSkylightMap();
                    //World.MarkBlocksDirtyVertical(blk.Position.X, blk.Position.Z, 0, 120);
                    //chunk.SetLightFor(blk.Position.X & 15, blk.Position.Y, blk.Position.Z & 15, Util.EnumSkyBlock.Sky, 0);
                    //chunk.SetLightFor(blk.Position.X & 15, blk.Position.Y - 1, blk.Position.Z & 15, Util.EnumSkyBlock.Sky, 0);
                    //chunk._RelightBlock(blk.Position.X & 15, blk.Position.Y, blk.Position.Z & 15);

                    //World.CheckLightFor(Util.EnumSkyBlock.Sky, blk.Position);
                    //chunk = World.GetChunk(ch.x, ch.y);
                    //chunk.GenerateSkylightMap();

                    //OnVoxelChanged(blk.Position.ToVec3i(),
                    //    new vec2i[] {
                    //        new vec2i(1, 0), new vec2i(1, 1), new vec2i(0, 1),
                    //        new vec2i(-1, 1), new vec2i(-1, 0), new vec2i(-1, -1),
                    //        new vec2i(0, -1), new vec2i(1, -1)});
                    //OnVoxelChanged(bl, new vec2i[0]);
                    break;
                case Keys.M:
                    //ch = OpenGLF.GetInstance().Cam.ToPositionChunk();
                    //bl = OpenGLF.GetInstance().Cam.ToPositionBlock();
                    //blk = Debag.GetInstance().RayCastBlock;
                    //chunk = World.GetChunk(ch.x, ch.y);
                    ////chunk.G
                    //chunk.GenerateSkylightMap();
                    //// World.CheckLight(blk.Position);

                    ////World.MarkBlocksDirtyVertical(blk.Position.X, blk.Position.Z, 0, 120);
                    ////chunk._RelightBlock(blk.Position.X & 15, blk.Position.Y, blk.Position.Z & 15);

                    ////World.CheckLightFor(Util.EnumSkyBlock.Sky, blk.Position);
                    ////chunk = World.GetChunk(ch.x, ch.y);
                    ////chunk.GenerateSkylightMap();

                    //OnVoxelChanged(blk.Position.ToVec3i(),
                    //    new vec2i[] {
                    //        new vec2i(1, 0), new vec2i(1, 1), new vec2i(0, 1),
                    //        new vec2i(-1, 1), new vec2i(-1, 0), new vec2i(-1, -1),
                    //        new vec2i(0, -1), new vec2i(1, -1)});
                    ////OnVoxelChanged(bl, new vec2i[0]);
                    break;
                case Keys.Space:
                    PlCamera.StepJamp();
                    OnMoveChanged();
                    break;
                case Keys.ShiftKey:
                    PlCamera.StepDown();
                    OnMoveChanged();
                    break;
                case Keys.A:
                    PlCamera.StepLeft();
                    OnMoveChanged();
                    break;
                case Keys.D:
                    PlCamera.StepRight();
                    OnMoveChanged();
                    break;
                case Keys.W:
                    PlCamera.StepForward();
                    OnMoveChanged();
                    break;
                case Keys.S:
                    PlCamera.StepBack();
                    OnMoveChanged();
                    break;
                case Keys.D1: Debag.GetInstance().NumberBlock = 1; break;
                case Keys.D2: Debag.GetInstance().NumberBlock = 2; break;
                case Keys.D3: Debag.GetInstance().NumberBlock = 4; break;
                case Keys.D4: Debag.GetInstance().NumberBlock = 5; break;
                case Keys.D5: Debag.GetInstance().NumberBlock = 6; break;
                case Keys.D6: Debag.GetInstance().NumberBlock = 11; break;
                case Keys.D7: Debag.GetInstance().NumberBlock = 8; break;
                case Keys.D8: Debag.GetInstance().NumberBlock = 9; break;
                case Keys.D9: Debag.GetInstance().NumberBlock = 10; break;
                case Keys.D0: Debag.GetInstance().NumberBlock = 12; break;

            }
        }

        /// <summary>
        /// Событие движение WASD
        /// </summary>
        public event EventHandler MoveChanged;

        /// <summary>
        /// Изменена движение WASD
        /// </summary>
        protected void OnMoveChanged()
        {
            MoveChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Событие изменена позиция чанка
        /// </summary>
        public event VoxelEventHandler VoxelChanged;

        /// <summary>
        /// Изменена позиция чанка
        /// </summary>
        protected void OnVoxelChanged(vec3i position, vec2i[] beside)
        {
            VoxelChanged?.Invoke(this, new VoxelEventArgs(position, beside));
        }
    }
}

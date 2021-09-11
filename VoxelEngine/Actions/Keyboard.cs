using System;
using System.Windows.Forms;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;
using VoxelEngine.World;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Chk;

namespace VoxelEngine.Actions
{
    /// <summary>
    /// Объект одиночка клавиатуры
    /// </summary>
    public class Keyboard : WorldHeirSet
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

        public PlayerCamera PlCamera { get; protected set; } = new PlayerCamera();

        public void PreviewKeyDown(Keys keys)
        {
            switch (keys)
            {
                case Keys.F3:
                    Debug.GetInstance().IsDraw = !Debug.GetInstance().IsDraw;
                    break;
                case Keys.F4:
                    VEC.GetInstance().Moving = VEMoving.FreeFlight;
                    break;
                case Keys.F5:
                    VEC.GetInstance().Moving = VEMoving.ObstacleFlight;
                    // save
                    //File.WriteAllBytes("map.dat", OpenGLF.GetInstance().ChunkItems.Write());
                    break;
                case Keys.F6:
                    VEC.GetInstance().Moving = VEMoving.Survival;
                    // load
                    //OpenGLF.GetInstance().ChunkItems.Read(File.ReadAllBytes("map.dat"));
                    break;
                case Keys.F7:
                    Debug.GetInstance().IsDrawChunk = !Debug.GetInstance().IsDrawChunk;
                    if (!Debug.GetInstance().IsDrawChunk)
                        OpenGLF.GetInstance().WorldLineM.Remove("chunk");
                    else
                        OpenGLF.GetInstance().WorldLineM.Chunk();
                    break;
                case Keys.F8:
                    Debug.GetInstance().IsDrawCollisium = !Debug.GetInstance().IsDrawCollisium;
                    if (!Debug.GetInstance().IsDrawCollisium)
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
            ChunkBase chunk;
            //Debag.Log("LogKey", keys.ToString());
            switch (keys)
            {
                case Keys.E:
                    OpenGLF.GetInstance().LineOn();
                    break;
                case Keys.Z:
                    VEC.GetInstance().Zoom = VEC.GetInstance().Zoom == 1 ? 2 : 1;
                    Debug.GetInstance().CountTest2 = VEC.GetInstance().Zoom;
                    break;
                case Keys.B:
                    //for (int i = 0; i < 50; i++)
                        World.AddEntity();
                    break;
                case Keys.P:
                    // Перегенерация
                    ch = OpenGLF.GetInstance().Cam.ToPositionChunk();
                    bl = OpenGLF.GetInstance().Cam.ToPositionBlock();
                    chunk = World.GetChunk(ch.x, ch.y);
                    chunk.Regen();

                    //MediaPlayer player = new MediaPlayer();
                    //player.Open(new Uri(@"D:\Work\3d\TestAnimation\say1.mp3"));
                    //player.Volume = 1f;
                    //player.Balance = 0f;
                    //player.Play();
                    break;
                case Keys.L:
                    ch = OpenGLF.GetInstance().Cam.ToPositionChunk();
                    //bl = OpenGLF.GetInstance().Cam.ToPositionBlock();
                    //blk = Debag.GetInstance().RayCastBlock;
                    chunk = World.GetChunk(ch.x, ch.y);

                    ///World.CheckLight(blk.Position);
                    chunk.StartRecheckGaps(true);
                    //chunk.RecheckGaps();
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
                case Keys.T:
                    // Плюс четверть дня
                    VEC.GetInstance().AddQuarterTick();
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
                case Keys.D1: Debug.GetInstance().NumberBlock = EnumBlock.Stone; break;
                case Keys.D2: Debug.GetInstance().NumberBlock = EnumBlock.Dirt; break;
                case Keys.D3: Debug.GetInstance().NumberBlock = EnumBlock.Sand; break;
                case Keys.D4: Debug.GetInstance().NumberBlock = EnumBlock.Planks; break;
                case Keys.D5: Debug.GetInstance().NumberBlock = EnumBlock.Log; break; // 6
                case Keys.D6: Debug.GetInstance().NumberBlock = EnumBlock.Water; break; // 7
                case Keys.D7: Debug.GetInstance().NumberBlock = EnumBlock.Glass; break; // 8
                case Keys.D8: Debug.GetInstance().NumberBlock = EnumBlock.Sapling; break; // 9
                case Keys.D9: Debug.GetInstance().NumberBlock = EnumBlock.Cactus; break;
                case Keys.D0: Debug.GetInstance().NumberBlock = EnumBlock.Brol; break;

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
    }
}

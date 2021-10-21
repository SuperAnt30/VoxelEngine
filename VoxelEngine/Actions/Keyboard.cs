using System;
using System.Windows.Forms;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;
using VoxelEngine.Renderer;
using VoxelEngine.Renderer.Entity;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Chk;

namespace VoxelEngine.Actions
{
    /// <summary>
    /// Объект одиночка клавиатуры
    /// </summary>
    public class Keyboard
    {
        #region Instance

        private static Keyboard instance;
        private Keyboard()
        {
            KeyMove.MoveChanged += KeyMove_MoveChanged;
        }

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
        /// Объект кэш чанка
        /// </summary>
        public WorldRender World { get; protected set; }
        /// <summary>
        /// Объект хранящий нажатие клавиш перемещения
        /// </summary>
        public KeyboardMove KeyMove { get; protected set; } = new KeyboardMove();

        /// <summary>
        /// Задать объект мира
        /// </summary>
        public void SetWorld(WorldRender world)
        {
            World = world;
            KeyMove.PlCamera.EntityR = new EntityRender(world, World.Entity);
        }

        public void UpdateFPS(float timeFrame, float timeAll)
        {
            KeyMove.PlCamera.Update(timeFrame, timeAll);
        }

        public void PreviewKeyDown(Keys keys)
        {
            switch (keys)
            {
                case Keys.F2:
                    OpenGLF.GetInstance().LineOn();
                    break;
                case Keys.F3:
                    Debug.GetInstance().IsDraw = !Debug.GetInstance().IsDraw;
                    break;
                case Keys.F4:
                    VEC.moving = VEMoving.FreeFlight;
                    World.UpdateModePlayer();
                    break;
                case Keys.F5:
                    VEC.moving = VEMoving.ObstacleFlight;
                    World.UpdateModePlayer();
                    break;
                case Keys.F6:
                    VEC.moving = VEMoving.Survival;
                    World.UpdateModePlayer();
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
                    if (Debug.GetInstance().IsDrawCollisium)
                    {
                        World.Entity.HitBox.RefrashDrawHitBox();
                    }
                    else 
                    {
                        OpenGLF.GetInstance().WorldLineM.RemovePrefix("HitBox");
                    }
                    break;
                case Keys.F9:
                    VEC.isDebugTextureAtlas = !VEC.isDebugTextureAtlas;
                    OpenGLF.GetInstance().Widget.RefreshDraw();
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
                    KeyMove.Sprinting();
                    break;

            }
        }

        public void KeyUp(Keys keys)
        {
            if (keys == Keys.D || keys == Keys.A) KeyMove.CancelHorizontal();
            else if (keys == Keys.W || keys == Keys.S) KeyMove.CancelVertical();
            else if (keys == Keys.Space) KeyMove.CancelUp();
            else if (keys == Keys.ShiftKey) KeyMove.CancelDown();
            else if (keys == Keys.ControlKey) KeyMove.CancelSprinting();
        }

        public void KeyDown(Keys keys)
        {
            vec2i ch;
            vec3i bl;
            ChunkBase chunk;
            switch (keys)
            {
                //case Keys.E:
                //    OpenGLF.GetInstance().LineOn();
                //    break;
                case Keys.Z:
                    //VEC.GetInstance().Zoom = VEC.GetInstance().Zoom == 1 ? 2 : 1;
                    //Debug.GetInstance().CountTest2 = VEC.GetInstance().Zoom;
                    break;
                case Keys.B:
                    World.AddEntity();
                    break;
                case Keys.P:
                    // Перегенерация
                    ch = OpenGLF.GetInstance().Cam.ChunkPos;
                    bl = OpenGLF.GetInstance().Cam.BlockPos;
                    chunk = World.GetChunk(ch.x, ch.y);
                    chunk.Regen();

                    //MediaPlayer player = new MediaPlayer();
                    //player.Open(new Uri(@"D:\Work\3d\TestAnimation\say1.mp3"));
                    //player.Volume = 1f;
                    //player.Balance = 0f;
                    //player.Play();
                    break;
                case Keys.L:
                    ch = OpenGLF.GetInstance().Cam.ChunkPos;
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
                    VEC.AddQuarterTick();
                    break;
                case Keys.Space: KeyMove.Up(); break;
                case Keys.ShiftKey: KeyMove.Down(); break;
                case Keys.A: KeyMove.Left(); break;
                case Keys.D: KeyMove.Right(); break;
                case Keys.W: KeyMove.Forward(); break;
                case Keys.S: KeyMove.Back(); break;
                case Keys.D1: SetNumberBlock(0); break;
                case Keys.D2: SetNumberBlock(1); break;
                case Keys.D3: SetNumberBlock(2); break;
                case Keys.D4: SetNumberBlock(3); break;
                case Keys.D5: SetNumberBlock(4); break;
                case Keys.D6: SetNumberBlock(5); break;
                case Keys.D7: SetNumberBlock(6); break;
                case Keys.D8: SetNumberBlock(7); break;
                case Keys.D9: SetNumberBlock(8); break;
            }
        }

        /// <summary>
        /// Задать выбранную ячейку
        /// </summary>
        protected void SetNumberBlock(int index)
        {
            PlayerWidget.SetIndex(index);
            OpenGLF.GetInstance().Widget.RefreshDraw();
        }

        #region Event

        private void KeyMove_MoveChanged(object sender, EventArgs e)
        {
            OnMoveChanged();
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

        #endregion
    }
}

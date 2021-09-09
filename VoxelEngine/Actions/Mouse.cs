using VoxelEngine.Glm;
using System;
using System.Drawing;
using System.Windows.Forms;
using VoxelEngine.Util;
using VoxelEngine.World;
using VoxelEngine.World.Blk;
using VoxelEngine.Graphics;

namespace VoxelEngine.Actions
{
    /// <summary>
    /// Объект одиночка мышки
    /// </summary>
    public class Mouse : WorldHeirSet
    {
        #region Instance

        private static Mouse instance;
        private Mouse() { }

        /// <summary>
        /// Передать по ссылке объект если он создан, иначе создать
        /// </summary>
        /// <returns>объект Mouse</returns>
        public static Mouse GetInstance()
        {
            if (instance == null) instance = new Mouse();
            return instance;
        }

        #endregion

        /// <summary>
        /// Вращается ли камера от мышки
        /// </summary>
        public bool IsMove { get; protected set; } = false;

        /// <summary>
        /// Атрибут запуска управления мыши
        /// </summary>
        protected bool _firstMouse = true;

        /// <summary>
        /// Чувствительность мыши
        /// </summary>
        protected float _speedMouse = 1.5f;

        public void Down(MouseEventArgs e)
        {
            OpenGLF openGLF = OpenGLF.GetInstance();


            //Voxel vox = WorldCache.GetInstance().RayCast(openGLF.Cam.Position, openGLF.Cam.Front, 10.0f, out vec3 end, out vec3 norm, out vec3 iend);
            //Voxel vox = World.RayCast(openGLF.Cam.Position, openGLF.Cam.Front, 10.0f, out vec3 end, out vec3i norm, out vec3i iend);
            Block block = World.RayCast(openGLF.Cam.PosPlus(), openGLF.Cam.Front, 10.0f, out vec3 end, out vec3i norm, out vec3i iend);
            if (block != null && !block.IsAir)
            {
                if (e.Button == MouseButtons.Left)
                {
                    //int x = iend.x;
                    //int y = iend.y;
                    //int z = iend.z;
                    if (VEC.GetInstance().Zoom == 1)
                    {
                        BlockPos blockPos = new BlockPos(iend);
                        //OnVoxelChanged(iend, World.SetVoxelId(Blocks.GetAir(new BlockPos(iend))));
                        World.SetBlockState(Blocks.GetAir(blockPos), true);
                    }
                    else if (VEC.GetInstance().Zoom == 2)
                    {
                        for (int x = iend.x; x <= iend.x + 1; x++)
                        {
                            for (int y = iend.y; y <= iend.y + 1; y++)
                            {
                                for (int z = iend.z; z <= iend.z + 1; z++)
                                {
                                    World.SetBlockState(Blocks.GetAir(new BlockPos(x, y, z)), true);
                                }
                            }
                        }
                    }
                }
                if (e.Button == MouseButtons.Right)
                {
                    vec3i vec = iend + norm;

                    HitBoxPlayer hitBox = openGLF.Cam.HitBox;
                    if (!hitBox.IsVoxelBody(vec))
                    {
                        //OnVoxelChanged(vec, World.SetVoxelId(Blocks.GetBlock((byte)Debag.GetInstance().NumberBlock, new BlockPos(vec))));
                        World.SetBlockState(Blocks.GetBlock(Debug.GetInstance().NumberBlock, new BlockPos(vec)), true);
                        //(byte)Debag.GetInstance().NumberBlock));
                    }

                    //vec3i pos = openGLF.Cam.ToPositionBlock();
                    //vec3i posD = pos;
                    //posD.y--;

                    //if (pos != vec && posD != vec)
                    //OnVoxelChanged(vec, World.SetVoxel(vec, (byte)Debag.GetInstance().NumberBlock));
                    // openGLF.ChunkItems.SetVoxel(x, y, z, 2); // 244 стекло
                    //openGLF.lightSolver.Remove(x, y, z);
                    //for (int i = y - 1; i >= 0; i--)
                    //{
                    //    openGLF.lightSolver.Remove(x, i, z);
                    //    if (i == 0 || openGLF.ChunkItems.GetVoxel(x, i - 1, z).Id != 0) break;
                    //}
                    //openGLF.lightSolver.Solve();

                    //openGLF.lightSolver.Add(x, y, z, 7);
                    //openGLF.lightSolver.Solve();
                }
            }
            
        }

        public void Move()
        {
            Move(!IsMove);
        }

        public void Move(bool isMove)
        {
            IsMove = isMove;

            if (IsMove) Cursor.Hide();
            else
            {
                RunMove();
                Cursor.Show();
            }
        }

        /// <summary>
        /// Включить движение от первого лица
        /// </summary>
        public void RunMove()
        {
            _firstMouse = true;
        }

        

        /// <summary>
        /// Движение мыши
        /// </summary>
        /// <param name="mouse">координаты мыши</param>
        /// <param name="bounds">Размер рабочего окна</param>
        /// <param name="mousePosition">координаты экранной мыши</param>
        public void Move(Point mouse, Rectangle bounds, Point mousePosition)
        {
            if (!IsMove) return;

            

            Camera cam = OpenGLF.GetInstance().Cam;

            // координата центра курсора
            Point point = new Point(bounds.Width / 2 + bounds.X, bounds.Height / 2 + bounds.Y);
            if (_firstMouse)
            {
                _firstMouse = false;
                Cursor.Position = point;
                return;
            }

            float deltaX = mousePosition.X - point.X;
            float deltaY = mousePosition.Y - point.Y;
            if (deltaX == 0 && deltaY == 0) return;

            cam.Pitch += -deltaY / cam.Height * _speedMouse;
            cam.Yaw += -deltaX / cam.Height * _speedMouse;

            if (cam.Pitch < -glm.radians(89.0f))
            {
                cam.Pitch = -glm.radians(89.0f);
            }
            if (cam.Pitch > glm.radians(89.0f))
            {
                cam.Pitch = glm.radians(89.0f);
            }

            if (cam.Yaw > glm.radians(180f)) cam.Yaw -= glm.radians(360f);
            if (cam.Yaw < glm.radians(-180f)) cam.Yaw += glm.radians(360f);

            cam.Rotation = new mat4(1.0f);
            cam.Rotate(cam.Pitch, cam.Yaw, 0);

            OnMoveChanged();

            // Сбросить координаты в мыши
            Cursor.Position = point;
        }

        #region Event

        ///// <summary>
        ///// Событие изменена позиция чанка
        ///// </summary>
        //public event VoxelEventHandler VoxelChanged;

        ///// <summary>
        ///// Изменена позиция чанка
        ///// </summary>
        //protected void OnVoxelChanged(vec3i position, vec2i[] beside)
        //{
        //    VoxelChanged?.Invoke(this, new VoxelEventArgs(position, beside));
        //}

        /// <summary>
        /// Событие движение мыши
        /// </summary>
        public event EventHandler MoveChanged;

        /// <summary>
        /// Изменена движение мыши
        /// </summary>
        protected void OnMoveChanged()
        {
            MoveChanged?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}

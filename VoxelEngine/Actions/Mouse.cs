using VoxelEngine.Glm;
using System;
using System.Drawing;
using System.Windows.Forms;
using VoxelEngine.Util;
using VoxelEngine.World;
using VoxelEngine.World.Blk;
using VoxelEngine.Graphics;
using VoxelEngine.Gen.Group;

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

        /// <summary>
        /// Вращение колёсика
        /// </summary>
        public void Wheel(int delta)
        {
            if (delta < 0) PlayerWidget.IndexNext();
            else PlayerWidget.IndexBack();
            OpenGLF.GetInstance().Widget.RefreshDraw();
        }

        public void Down(MouseEventArgs e)
        {
            if (!IsMove)
            {
                Move(true);
                return;
            }

            Camera camera = OpenGLF.GetInstance().Cam;
            MovingObjectPosition moving = World.RayCast(camera.PosPlus(), camera.Front, VE.MAX_DIST);

            if (e.Button == MouseButtons.Left)
            {
                // Удалить
                if (moving.IsEntity())
                {
                    moving.Entity.Kill();
                }
                else if (moving.IsBlock() && !moving.Block.IsAir)
                {
                    BlockPos blockPos = new BlockPos(moving.IEnd);
                    World.SetBlockState(Blocks.GetAir(blockPos), true);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Поставить
                if (moving.IsBlock() && !moving.Block.IsAir)
                {
                    if (moving.Block.IsGroupModel)
                    {
                        // Модель блоков, возможность активации
                        if (moving.Block.Group != null)
                        {
                            moving.Block.Group.Action();
                        }
                    }
                    else
                    {
                        EnumBlock enumBlock = PlayerWidget.GetCell();
                        vec3i vec = moving.IEnd + moving.Norm;
                        if (enumBlock == EnumBlock.Door)
                        {
                            // Дверь
                            GroupDoor door = new GroupDoor(World, vec);
                            door.Put();
                        }
                        else if (enumBlock != EnumBlock.None)
                        {
                            if (!World.Entity.HitBox.IsVoxelBody(vec))
                            {
                                World.SetBlockState(
                                    Blocks.GetBlock(enumBlock, new BlockPos(vec)), true);
                            }
                        }
                    }
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
            if (IsMove && Cursor.Current.IsVisible())
            {
                Cursor.Hide();
            }
            else if (!IsMove && !Cursor.Current.IsVisible())
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

            float pitch = cam.Pitch - deltaY / cam.Height * _speedMouse;
            float yaw = cam.Yaw - deltaX / cam.Height * _speedMouse;

            if (pitch < -glm.radians(89.0f)) pitch = -glm.radians(89.0f);
            if (pitch > glm.radians(89.0f)) pitch = glm.radians(89.0f);
            if (yaw > glm.pi) yaw -= glm.pi360;
            if (yaw < -glm.pi) yaw += glm.pi360;

            cam.SetRotation(yaw, pitch);
            World.Entity.SetRotation(cam.Yaw, cam.Pitch);

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

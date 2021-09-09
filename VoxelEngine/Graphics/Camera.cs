using System;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine.Graphics
{
    /// <summary>
    /// Объект камеры
    /// </summary>
    public class Camera
    {
        protected vec3 _front;
        public vec3 Front
        {
            get { return _front; }
            protected set
            {
                if (!_front.Equals(value))
                {
                    _front = value;
                    ReplacePosition();
                }
                else _front = value;
            }
        }

        protected vec3 _up;
        public vec3 Up
        {
            get { return _up; }
            protected set
            {
                if (!_up.Equals(value))
                {
                    _up = value;
                    ReplacePosition();
                }
                else _up = value;
            }
        }


        public vec3 Right { get; protected set; }

        
        /// <summary>
        /// Корректировка по высоте масштаб блока 0,5 метра
        /// </summary>
        public void PositionVertical()
        {
            vec3 pos = Position;
            pos.y = Mth.Round(pos.y);
            Position = pos;
        }

        /// <summary>
        /// Корректировка по высоте масштаб блока 1 метр
        /// </summary>
        public void PositionVerticalMeter()
        {
            vec3 pos = Position;
            int y = Mth.Floor(pos.y);
            pos.y = (float)y + .5f;
            Position = pos;
        }

        protected vec3 _position;
        /// <summary>
        /// Позиция камеры
        /// </summary>
        public vec3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (!_position.Equals(value))
                {
                    if (!ToPositionBlock(_position).Equals(ToPositionBlock(value)))
                    {
                        if (!ToPositionChunk(new vec3i(_position)).Equals(ToPositionChunk(new vec3i(value))))
                        {
                            _position = value;
                            OnPositionChunkChanged();
                        } else
                        {
                            _position = value;
                            OnPositionBlockChanged();
                        }
                    }
                    else
                    {
                        _position = value;
                    }
                    HitBox.SetPos(_position);
                    ReplacePosition();
                } else
                {
                    _position = value;
                }
            }
        }

        /// <summary>
        /// Получить хит бокс игрока
        /// </summary>
        public HitBoxPlayer HitBox { get; protected set; } 

        /// <summary>
        /// Рост персонажа, камера + 0,5 блока
        /// </summary>
        public vec3 PosPlus()
        {
            // TODO:: Рост + 0,5
            return HitBox == null ? Position : Position + new vec3(0, HitBox.Size.Eyes, 0);
        }
             
        /// <summary>
        /// Угол обзора
        /// </summary>
        public float Fov { get; protected set; }
        public mat4 Rotation { get; set; }

        protected float _aspect;
        /// <summary>
        /// Ширина окна
        /// </summary>
        public float Width { get; protected set; }
        /// <summary>
        /// Высота окна
        /// </summary>
        public float Height { get; protected set; }

        public Camera(vec3 position, float fov)
        {
            HitBox = new HitBoxPlayer();
            HitBox.Size.LookAtChanged += SizeLookAtChanged;
            Worth();
            Position = position;
            Fov = fov;
            Rotation = new mat4(1.0f);
            _UpdateVectors();
            //Sitting();
            
        }

        private void SizeLookAtChanged(object sender, EventArgs e)
        {
            ReplacePosition();
            //OnLookAtChanged();
        }

        protected void _UpdateVectors()
        {
            Front = new vec3(Rotation * new vec4(0, 0, -1, 1));
            Right = new vec3(Rotation * new vec4(1, 0, 0, 1));
            Up = new vec3(Rotation * new vec4(0, 1, 0, 1));
        }

        public void Rotate(float x, float y, float z)
        {
            Rotation = glm.rotate(Rotation, z, new vec3(0, 0, 1));
            Rotation = glm.rotate(Rotation, y, new vec3(0, 1, 0));
            Rotation = glm.rotate(Rotation, x, new vec3(1, 0, 0));
            _UpdateVectors();
        }

        /// <summary>
        /// Изменить размер окна
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetResized(int width, int height)
        {
            Width = width;
            Height = height;
            _aspect = Width / Height;

            Projection = glm.perspective(Fov, _aspect, 0.001f, VE.CHUNK_VISIBILITY * 22.624f * 2f).to_array();
            Ortho2D = glm.ortho(0, Width, Height, 0).to_array();
        }

        /// <summary>
        /// Заменить LookAt и PositionView
        /// </summary>
        public void ReplacePosition()
        {
            vec3 pos = PosPlus();
            mat4 lookAt = glm.lookAt(pos, pos + Front, Up);
            LookAt = lookAt.to_array();

            // Для скайбокса позицию камеры не смещаем
            pos = Position;
            lookAt = glm.lookAt(pos, pos + Front, Up);
            PositionView = glm.translate(new mat4(1.0f), pos).to_array();
            ProjectionLookAt = (glm.perspective(Fov, _aspect, 0.001f, VE.CHUNK_VISIBILITY * 22.624f * 2f)
                * lookAt).to_array();
        }

        /// <summary>
        /// массив матрицы перспективу камеры 3D
        /// </summary>
        public float[] Projection { get; protected set; }
        /// <summary>
        /// массив матрицы перспективу камеры 3D c расположения камеры в пространстве
        /// </summary>
        public float[] ProjectionLookAt { get; protected set; }
        /// <summary>
        /// массив матрицы расположения камеры в пространстве
        /// </summary>
        public float[] LookAt { get; protected set; }
        /// <summary>
        /// массив матрицы  проекцию камеры 2D
        /// </summary>
        public float[] Ortho2D { get; protected set; }
        /// <summary>
        /// массив матрицы позиции камеры
        /// </summary>
        public float[] PositionView { get; protected set; }

        /// <summary>
        /// Угол по горизонтали в радианах
        /// </summary>
        public float Yaw { get; set; } = 0.0f;
        /// <summary>
        /// Угол по вертикали в радианах
        /// </summary>
        public float Pitch { get; set; } = 0.0f;

        /// <summary>
        /// Угол по горизонтали
        /// </summary>
        /// <returns></returns>
        public float AngleYaw()
        {
            return glm.degrees(Yaw);
            // https://qastack.ru/gamedev/50963/how-to-extract-euler-angles-from-transformation-matrix

            //if (Rotation[0, 0] == 1f || Rotation[0, 0] == -1f)
            //    return glm.atan(Rotation[2, 0], Rotation[3, 2]) * 57.32484f;
            //return glm.atan(-Rotation[0, 2], Rotation[0, 0]) * 57.32484f;
        }

        /// <summary>
        /// Угол по вертикали
        /// </summary>
        public float AnglePitch()
        {
            return glm.degrees(Pitch);
        }

        /// <summary>
        /// Определить направление обзора камеры по полюсам
        /// </summary>
        public Pole GetPole()
        {
            return EnumFacing.FromAngle(AngleYaw());
        }

        /// <summary>
        /// Растояние до объекта
        /// </summary>
        public float DistanceTo(vec3 vec)
        {
            float var2 = vec.x - Position.x;
            float var4 = vec.y - Position.y;
            float var6 = vec.z - Position.z;
            return Mth.Sqrt(var2 * var2 + var4 * var4 + var6 * var6);
        }

        /// <summary>
        /// Посчитать дистанцию 2д
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="chunk"></param>
        /// <returns></returns>
        public static float DistanceChunkTo(vec2i cam, vec2i chunk)
        {
            float var2 = cam.x - chunk.x;
            float var4 = cam.y - chunk.y;
            return Mth.Sqrt(var2 * var2 + var4 * var4);
        }

        /// <summary>
        /// Определить регион по позиции камеры 
        /// </summary>
        public vec2i ToPositionRegion()
        {
            vec2i chunk = ToPositionChunk();
            return new vec2i(chunk.x >> 5, chunk.y >> 5);
        }

        /// <summary>
        /// Определить чанк по позиции камеры
        /// </summary>
        public vec2i ToPositionChunk()
        {
            return ToPositionChunk(new vec3i(Position));
        }

        /// <summary>
        /// Определить чанк
        /// </summary>
        public static vec2i ToPositionChunk(vec3i pos)
        {
            return new vec2i((pos.x) >> 4, (pos.z) >> 4);
        }

        /// <summary>
        /// Определить блок по позиции камеры
        /// </summary>
        public vec3i ToPositionBlock()
        {
            return ToPositionBlock(Position);
        }

        /// <summary>
        /// Определить блок по позиции камеры
        /// </summary>
        public vec3i ToPositionBlock(vec3 pos)
        {
            return new vec3i(pos);
        }

        /// <summary>
        /// Стоит
        /// </summary>
        public void Worth()
        {
            HitBox.Size.SetSize(.6f, 3.7f);
            HitBox.Size.SetEyes(3.4f, 16);
        }
        /// <summary>
        /// Сидит
        /// </summary>
        public void Sneaking()
        {
            HitBox.Size.SetSize(.6f, 2.6f);
            HitBox.Size.SetEyes(2.4f, 16);
        }
        /// <summary>
        /// Плывём
        /// </summary>
        public void Sailing()
        {
            HitBox.Size.SetSize(.6f, 1.8f);
            HitBox.Size.SetEyes(1.5f, 10);
        }

        #region Event

        /// <summary>
        /// Событие изменена позиция блока
        /// </summary>
        public event EventHandler PositionBlockChanged;

        /// <summary>
        /// Изменена позиция блока
        /// </summary>
        protected void OnPositionBlockChanged()
        {
            PositionBlockChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Событие изменена позиция чанка
        /// </summary>
        public event EventHandler PositionChunkChanged;

        /// <summary>
        /// Изменена позиция чанка
        /// </summary>
        protected void OnPositionChunkChanged()
        {
            PositionChunkChanged?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}

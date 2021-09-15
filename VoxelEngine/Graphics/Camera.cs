using System;
using VoxelEngine.Entity;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine.Graphics
{
    /// <summary>
    /// Объект камеры
    /// </summary>
    public class Camera
    {
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
        public float Yaw { get; protected set; } = 0.0f;
        /// <summary>
        /// Угол по вертикали в радианах
        /// </summary>
        public float Pitch { get; protected set; } = 0.0f;

        /// <summary>
        /// Позиция камеры
        /// </summary>
        public vec3 Position { get; protected set; }

        public vec3 Front { get; protected set; }
        public vec3 Up { get; protected set; }
        public vec3 Right { get; protected set; }

        /// <summary>
        /// В каком чанке находится
        /// </summary>
        public vec2i ChunkPos { get; protected set; } = new vec2i();
        /// <summary>
        /// Позиция псевдо чанка
        /// </summary>
        public int ChunkY { get; protected set; }
        /// <summary>
        /// В каком блоке находится
        /// </summary>
        public vec3i BlockPos { get; protected set; } = new vec3i();
        /// <summary>
        /// Высота глаз
        /// </summary>
        public float Eyes { get; protected set; } = 0f;
        /// <summary>
        /// Под водой ли глаза
        /// </summary>
        public bool IsEyesWater { get; protected set; } = false;
        /// <summary>
        /// Угол обзора
        /// </summary>
        public float Fov { get; protected set; }

        protected mat4 rotation;

        protected float aspect;
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
            Position = position;
            Fov = fov;
            rotation = new mat4(1.0f);
            UpdateVectors();
        }

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

        public void SetPosRotation(vec3 pos, float yaw, float pitch)
        {
            SetRotation(yaw, pitch);
            SetPos(pos);
        }

        public void SetPos(vec3 pos)
        {
            if (!Position.Equals(pos))
            {
                if (!ToPositionBlock(Position).Equals(ToPositionBlock(pos)))
                {
                    if (!ToPositionChunk(new vec3i(Position)).Equals(ToPositionChunk(new vec3i(pos))))
                    {
                        Position = pos;
                        OnPositionChunkChanged();
                    }
                    else
                    {
                        Position = pos;
                        OnPositionBlockChanged();
                    }
                }
                else
                {
                    Position = pos;
                }
                BlockPos = new vec3i(Position);
                ChunkPos = new vec2i((BlockPos.x) >> 4, (BlockPos.z) >> 4);
                ChunkY = (BlockPos.y) >> 4;
                ReplacePosition();
            }
        }

        /// <summary>
        /// Задать вращение
        /// </summary>
        public void SetRotation(float yaw, float pitch)
        {
            Yaw = yaw;
            Pitch = pitch;
            rotation = new mat4(1.0f);
            Rotate(pitch, yaw, 0);
        }

        /// <summary>
        /// Задать высоту глаз и в воде ли они
        /// </summary>
        public void SetEyesWater(float eyes, bool isEyesWater)
        {
            Eyes = eyes;
            IsEyesWater = isEyesWater;
            ReplacePosition();
        }

        /// <summary>
        /// Рост персонажа, позиция + высота глаз
        /// </summary>
        public vec3 PosPlus()
        {
            return Position + new vec3(0, Eyes, 0);
        }
             
        public void SetFov(float fov)
        {
            Fov = fov;
            aspect = Width / Height;
            Projection = glm.perspective(Fov, aspect, 0.001f, VE.CHUNK_VISIBILITY * 22.624f * 2f).to_array();
        }

        protected void UpdateVectors()
        {
            Front = new vec3(rotation * new vec4(0, 0, -1, 1));
            Right = new vec3(rotation * new vec4(1, 0, 0, 1));
            Up = new vec3(rotation * new vec4(0, 1, 0, 1));
            ReplacePosition();
        }

        public void Rotate(float x, float y, float z)
        {
            rotation = glm.rotate(rotation, z, new vec3(0, 0, 1));
            rotation = glm.rotate(rotation, y, new vec3(0, 1, 0));
            rotation = glm.rotate(rotation, x, new vec3(1, 0, 0));
            UpdateVectors();
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
            aspect = Width / Height;

            Projection = glm.perspective(Fov, aspect, 0.001f, VE.CHUNK_VISIBILITY * 22.624f * 2f).to_array();
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
            ProjectionLookAt = (glm.perspective(Fov, aspect, 0.001f, VE.CHUNK_VISIBILITY * 22.624f * 2f)
                * lookAt).to_array();
        }

        

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
            return new vec2i(ChunkPos.x >> 5, ChunkPos.y >> 5);
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
        public vec3i ToPositionBlock(vec3 pos)
        {
            return new vec3i(pos);
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

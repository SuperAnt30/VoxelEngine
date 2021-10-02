using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine.World.Blk.Model
{
    /// <summary>
    /// Создать коробку
    /// </summary>
    public class Box
    {
        /// <summary>
        /// Начальная точка (0)
        /// </summary>
        public vec3 From { get; set; } = new vec3(0);
        /// <summary>
        /// Конечная точка (блок 1f)
        /// </summary>
        public vec3 To { get; set; } = new vec3(1f);
        /// <summary>
        /// Начальная точка текстуры (0)
        /// </summary>
        public vec2 UVFrom { get; set; } = new vec2(0);
        /// <summary>
        /// Конечная точка текстуры (блок 0.0625f)
        /// </summary>
        public vec2 UVTo { get; set; } = new vec2(VE.UV_SIZE);
        /// <summary>
        /// Размер
        /// </summary>
        public vec3 Size { get; protected set; } = new vec3(1f);
        /// <summary>
        /// Хитбок занимает полностью блок
        /// </summary>
        public bool IsHitBoxAll { get; protected set; } = true;
        /// <summary>
        /// Стороны
        /// </summary>
        public Face[] Faces { get; set; } = new Face[] { new Face(Pole.All, 0) };

        /// <summary>
        /// Указываем вращение блока по оси Y в радианах
        /// </summary>
        public float RotateYaw { get; set; } = 0;
        /// <summary>
        /// Указываем вращение блока по оси X в радианах
        /// </summary>
        public float RotatePitch { get; set; } = 0;

        public Box() { }

        public Box(vec3 from, vec3 to)
        {
            From = from;
            To = to;
            Size = To - From;
            IsHitBoxAll = false;
        }

        public Box(int numberTexture)
        {
            Faces = new Face[] { new Face(Pole.All, numberTexture) };
        }

        public Box(int numberTexture, bool isColor)
        {
            Faces = new Face[] { new Face(Pole.All, numberTexture, isColor) };
        }
    }
}

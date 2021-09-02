using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine.Model
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

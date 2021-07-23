using System.Collections.Generic;
using VoxelEngine.Glm;

namespace VoxelEngine
{
    public class LineRender
    {
        /// <summary>
        /// Массив буфера сетки
        /// </summary>
        protected List<float> buffer = new List<float>();

        public LineRender() { }

        /// <summary>
        /// Получить массив буфера сетки
        /// </summary>
        public float[] ToBuffer()
        {
            return buffer.ToArray();
        }

        public void Line(float x1, float y1, float z1, float x2, float y2, float z2,
        float r, float g, float b, float a)
        {
            buffer.AddRange(new float[]
            {
                x1, y1, z1, r, g, b, a,
                x2, y2, z2, r, g, b, a
            });
        }

        public void Line(float x1, float y1, float z1, float x2, float y2, float z2, vec4 rgba)
        {
            buffer.AddRange(new float[]
            {
                x1, y1, z1, rgba.x, rgba.y, rgba.z, rgba.w,
                x2, y2, z2, rgba.x, rgba.y, rgba.z, rgba.w
            });
        }
        public void Box(float x, float y, float z, float w, float h, float d, vec4 rgba)
        {
            Box(x, y, z, w, h, d, rgba.x, rgba.y, rgba.z, rgba.w);
        }

        public void Box(float x, float y, float z, float w, float h, float d,
        float r, float g, float b, float a)
        {
            w *= 0.5f;
            h *= 0.5f;
            d *= 0.5f;

            Line(x - w, y - h, z - d, x + w, y - h, z - d, r, g, b, a);
            Line(x - w, y + h, z - d, x + w, y + h, z - d, r, g, b, a);
            Line(x - w, y - h, z + d, x + w, y - h, z + d, r, g, b, a);
            Line(x - w, y + h, z + d, x + w, y + h, z + d, r, g, b, a);

            Line(x - w, y - h, z - d, x - w, y + h, z - d, r, g, b, a);
            Line(x + w, y - h, z - d, x + w, y + h, z - d, r, g, b, a);
            Line(x - w, y - h, z + d, x - w, y + h, z + d, r, g, b, a);
            Line(x + w, y - h, z + d, x + w, y + h, z + d, r, g, b, a);

            Line(x - w, y - h, z - d, x - w, y - h, z + d, r, g, b, a);
            Line(x + w, y - h, z - d, x + w, y - h, z + d, r, g, b, a);
            Line(x - w, y + h, z - d, x - w, y + h, z + d, r, g, b, a);
            Line(x + w, y + h, z - d, x + w, y + h, z + d, r, g, b, a);
        }

        public void Clear()
        {
            buffer.Clear();
        }



    }
}

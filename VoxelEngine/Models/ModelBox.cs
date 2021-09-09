using System.Collections.Generic;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine.Models
{
    /// <summary>
    /// Модель коробки
    /// </summary>
    public class ModelBox : BufferHeir
    {
        /// <summary>
        /// Позиции вершин (x, y, z) и координаты текстуры (u, v) для каждой из 8 точек на кубе
        /// </summary>
        //public TextureVertex[] Vertices { get; protected set; } = new TextureVertex[8];

        /// <summary>
        /// Массив из 6 текстурных квадратов, по одному на каждую грань куба.
        /// </summary>
        public TexturedQuad[] Quads { get; protected set; } = new TexturedQuad[6];

        /// <summary>
        /// Координата наименьшей вершины
        /// </summary>
        public vec3 PosMin { get; protected set; }
        /// <summary>
        /// Координата наибольшей вершины
        /// </summary>
        public vec3 PosMax { get; protected set; }

        public float RotateAngleX { get; set; }
        public float RotateAngleY { get; set; }
        public float RotateAngleZ { get; set; }
        public vec3 RotationPoint { get; protected set; }


        protected ModelBase modelBase;
        protected int u, v;
        protected int w, h, d;
        protected float scale;

        /// <summary>
        /// Создать коробку
        /// </summary>
        public ModelBox(ModelBase modelBase, int u, int v, float x, float y, float z, int w, int h, int d, float scale)//, bool mirrer)
        {
            this.modelBase = modelBase;
            this.u = u;
            this.v = v;
            this.w = w;
            this.h = h;
            this.d = d;
            this.scale = scale;

            PosMin = new vec3(x, y, z);
            PosMax = PosMin + new vec3(w, h, d);
        }

        public void SetRotationPoint(float rotationX, float rotationY, float rotationZ)
        {
            RotationPoint = new vec3(rotationX, rotationY, rotationZ);
        }

        /// <summary>
        /// До рендера обрабатываем вращение
        ///     +---+---+
        ///     | 2 | 3 |
        /// +---+---+---+---+
        /// | 1 | 4 | 0 | 5 |
        /// +---+---+---+---+
        ///             7 +-----+ 6
        /// y ^  _  z    /     /|
        ///   |  /|   3 +-----+2|
        ///   | /       | 4   | + 5
        ///   |/        |     |/
        ///   +----> x  +-----+     
        ///            0       1 
        /// </summary>
        protected void PreRender(float yaw)
        {
            float x = PosMin.x;
            float y = PosMin.y;
            float z = PosMin.z;
            float xm = PosMax.x;
            float ym = PosMax.y;
            float zm = PosMax.z;

            vec3[] p = new vec3[8];

            p[0] = new vec3(x, y, z);
            p[1] = new vec3(xm, y, z);
            p[2] = new vec3(xm, ym, z);
            p[3] = new vec3(x, ym, z);
            p[4] = new vec3(x, y, zm);
            p[5] = new vec3(xm, y, zm);
            p[6] = new vec3(xm, ym, zm);
            p[7] = new vec3(x, ym, zm);

            // Вращение
            for (int i = 0; i < 8; i++) 
            {
                p[i] = Rotates(p[i]);
                p[i] = glm.rotate(p[i], glm.pi, new vec3(0, 0, 1f)); // ????
                if (yaw != 0) p[i] = glm.rotate(p[i], yaw, new vec3(0, 1f, 0));
            }

            Quads[0] = new TexturedQuad(new vec3[]
            { p[5], p[1], p[2], p[6] }, u + d + w, v + d, u + d + w + d, v + d + h, modelBase.TextureSize);
            Quads[1] = new TexturedQuad(new vec3[]
            { p[0], p[4], p[7], p[3] }, u, v + d, u + d, v + d + h, modelBase.TextureSize);
            // up (по картинке, но по факту снизу)
            Quads[2] = new TexturedQuad(new vec3[]
            { p[5], p[4], p[0], p[1] }, u + d, v, u + d + w, v + d, modelBase.TextureSize);
            // down (по картинке, но по факту сверху)
            Quads[3] = new TexturedQuad(new vec3[]
            { p[2], p[3], p[7], p[6] }, u + d + w, v + d, u + d + w + w, v, modelBase.TextureSize);
            Quads[4] = new TexturedQuad(new vec3[]
            { p[1], p[0], p[3], p[2] }, u + d, v + d, u + d + w, v + d + h, modelBase.TextureSize);
            Quads[5] = new TexturedQuad(new vec3[]
            { p[4], p[5], p[6], p[7] }, u + d + w + d, v + d, u + d + w + d + w, v + d + h, modelBase.TextureSize);
        }

        public void Render(vec3 pos, float yaw, float scale)
        {
            PreRender(yaw);
            

            List<float> buffer = new List<float>();
            foreach (TexturedQuad quad in Quads)
            {
                quad.Render(this, pos, scale);
                buffer.AddRange(quad.Buffer);
            }
            // 0 2 
            // for (int i = 0; i < 6; i++)
            //{
            //TexturedQuad quad = Quads[5];
            //quad.Render(this, pos, scale);
            //buffer.AddRange(quad.Buffer);
            //////}
            Buffer = buffer.ToArray();
        }

        protected vec3 Rotates(vec3 pos)
        {
            //RotationPoint
            mat4 rotat = new mat4(1.0f);
            rotat = glm.translate(rotat, RotationPoint);
            if (RotateAngleZ != 0) rotat = glm.rotate(rotat, RotateAngleZ, new vec3(0, 0, 1f));
            if (RotateAngleY != 0) rotat = glm.rotate(rotat, RotateAngleY, new vec3(0, 1f, 0));
            if (RotateAngleX != 0) rotat = glm.rotate(rotat, RotateAngleX, new vec3(1f, 0, 0));
            return new vec3(glm.translate(rotat, pos));
            ////glm.rotate()
            //if (RotateAngleZ != 0) pos = glm.rotate(pos, RotateAngleZ, new vec3(0, 0, 1f));
            //if (RotateAngleY != 0) pos = glm.rotate(pos, RotateAngleY, new vec3(0, 1f, 0));
            //if (RotateAngleX != 0) pos = glm.rotate(pos, RotateAngleX, new vec3(1f, 0, 0));
            //return pos;
        }
    }
}

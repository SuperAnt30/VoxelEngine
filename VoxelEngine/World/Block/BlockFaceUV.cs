using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Построение блока с разных сторон
    /// </summary>
    public class BlockFaceUV
    {
        protected vec3 v1 = new vec3();
        protected vec3 v2 = new vec3();

        protected vec2 u1 = new vec2();
        protected vec2 u2 = new vec2();

        protected vec3 c = new vec3();
        /// <summary>
        /// Тень стороны
        /// </summary>
        protected vec4 l = new vec4(0f);

        protected vec2 leg = new vec2();

        public BlockFaceUV(vec3 vec1, vec3 vec2, vec2 uv1, vec2 uv2, vec3 color, vec4 lg, vec2 leght)
        {
            v1 = vec1;
            v2 = vec2;
            u1 = uv1;
            u2 = uv2;
            c = color;
            l = lg;
            leg = leght;
        }

        ///// <summary>
        ///// Соседняя сторона по индексу
        ///// </summary>
        ///// <param name="index"></param>
        ///// <returns></returns>
        //public static vec3i AdjacentSide(Pole pole)
        //{
        //    switch (pole)
        //    {
        //        case Pole.Down: return new vec3i(0, -1, 0);
        //        case Pole.East: return new vec3i(1, 0, 0);
        //        case Pole.West: return new vec3i(-1, 0, 0);
        //        case Pole.North: return new vec3i(0, 0, 1);
        //        case Pole.South: return new vec3i(0, 0, -1);
        //    }
        //    return new vec3i(0, 1, 0);
        //}

        /// <summary>
        /// Ввернуть сторону по индексу
        /// </summary>
        /// <param name="index">индекс стороны</param>
        /// <param name="l">освещение стороны</param>
        public float[] Side(Pole pole)//, vec4 l)
        {
            //vec4 l = new vec4(1f);
            switch (pole)
            {
                case Pole.Down: return Down();
                case Pole.East: return East();
                case Pole.West: return West();
                case Pole.North: return North();
                case Pole.South: return South();
            }
            return Up();
        }

        /// <summary>
        /// Вверх
        /// </summary>
        public float[] Up()
        {
            return new float[] {
                v1.x, v2.y, v1.z, u2.x, u1.y, c.x * l.x, c.y * l.x, c.z * l.x, leg.x, leg.y,
                v1.x, v2.y, v2.z, u2.x, u2.y, c.x * l.y, c.y * l.y, c.z * l.y, leg.x, leg.y,
                v2.x, v2.y, v2.z, u1.x, u2.y, c.x * l.z, c.y * l.z, c.z * l.z, leg.x, leg.y,

                v1.x, v2.y, v1.z, u2.x, u1.y, c.x * l.x, c.y * l.x, c.z * l.x, leg.x, leg.y,
                v2.x, v2.y, v2.z, u1.x, u2.y, c.x * l.z, c.y * l.z, c.z * l.z, leg.x, leg.y,
                v2.x, v2.y, v1.z, u1.x, u1.y, c.x * l.w, c.y * l.w, c.z * l.w, leg.x, leg.y
            };
        }

        /// <summary>
        /// Низ
        /// </summary>
        public float[] Down()
        {
            return new float[] {
                v1.x, v1.y, v1.z, u1.x, u1.y, c.x * l.x, c.y * l.x, c.z * l.x, leg.x, leg.y,
                v2.x, v1.y, v2.z, u2.x, u2.y, c.x * l.y, c.y * l.y, c.z * l.y, leg.x, leg.y,
                v1.x, v1.y, v2.z, u1.x, u2.y, c.x * l.z, c.y * l.z, c.z * l.z, leg.x, leg.y,

                v1.x, v1.y, v1.z, u1.x, u1.y, c.x * l.x, c.y * l.x, c.z * l.x, leg.x, leg.y,
                v2.x, v1.y, v1.z, u2.x, u1.y, c.x * l.w, c.y * l.w, c.z * l.w, leg.x, leg.y,
                v2.x, v1.y, v2.z, u2.x, u2.y, c.x * l.y, c.y * l.y, c.z * l.y, leg.x, leg.y
            };
        }

        /// <summary>
        /// Восточная сторона
        /// </summary>
        public float[] East()
        {
            return new float[] {
                v2.x, v1.y, v1.z, u2.x, u1.y, c.x * l.x, c.y * l.x, c.z * l.x, leg.x, leg.y,
                v2.x, v2.y, v1.z, u2.x, u2.y, c.x * l.y, c.y * l.y, c.z * l.y, leg.x, leg.y,
                v2.x, v2.y, v2.z, u1.x, u2.y, c.x * l.z, c.y * l.z, c.z * l.z, leg.x, leg.y,

                v2.x, v1.y, v1.z, u2.x, u1.y, c.x * l.x, c.y * l.x, c.z * l.x, leg.x, leg.y,
                v2.x, v2.y, v2.z, u1.x, u2.y, c.x * l.z, c.y * l.z, c.z * l.z, leg.x, leg.y,
                v2.x, v1.y, v2.z, u1.x, u1.y, c.x * l.w, c.y * l.w, c.z * l.w, leg.x, leg.y
            };
        }

        /// <summary>
        /// Западная сторона
        /// </summary>
        public float[] West()
        {
            return new float[] {
                v1.x, v1.y, v1.z, u1.x, u1.y, c.x * l.x, c.y * l.x, c.z * l.x, leg.x, leg.y,
                v1.x, v2.y, v2.z, u2.x, u2.y, c.x * l.y, c.y * l.y, c.z * l.y, leg.x, leg.y,
                v1.x, v2.y, v1.z, u1.x, u2.y, c.x * l.z, c.y * l.z, c.z * l.z, leg.x, leg.y,

                v1.x, v1.y, v1.z, u1.x, u1.y, c.x * l.x, c.y * l.x, c.z * l.x, leg.x, leg.y,
                v1.x, v1.y, v2.z, u2.x, u1.y, c.x * l.w, c.y * l.w, c.z * l.w, leg.x, leg.y,
                v1.x, v2.y, v2.z, u2.x, u2.y, c.x * l.y, c.y * l.y, c.z * l.y, leg.x, leg.y
            };
        }

        /// <summary>
        /// Южная сторона
        /// </summary>
        public float[] North()
        {
            return new float[] {
                v1.x, v1.y, v1.z, u2.x, u1.y, c.x * l.x, c.y * l.x, c.z * l.x, leg.x, leg.y,
                v1.x, v2.y, v1.z, u2.x, u2.y, c.x * l.y, c.y * l.y, c.z * l.y, leg.x, leg.y,
                v2.x, v2.y, v1.z, u1.x, u2.y, c.x * l.z, c.y * l.z, c.z * l.z, leg.x, leg.y,

                v1.x, v1.y, v1.z, u2.x, u1.y, c.x * l.x, c.y * l.x, c.z * l.x, leg.x, leg.y,
                v2.x, v2.y, v1.z, u1.x, u2.y, c.x * l.z, c.y * l.z, c.z * l.z, leg.x, leg.y,
                v2.x, v1.y, v1.z, u1.x, u1.y, c.x * l.w, c.y * l.w, c.z * l.w, leg.x, leg.y
            };
        }

        /// <summary>
        /// Северная сторона
        /// </summary>
        public float[] South()
        {
            return new float[] {
                v1.x, v1.y, v2.z, u1.x, u1.y, c.x * l.x, c.y * l.x, c.z * l.x, leg.x, leg.y,
                v2.x, v2.y, v2.z, u2.x, u2.y, c.x * l.y, c.y * l.y, c.z * l.y, leg.x, leg.y,
                v1.x, v2.y, v2.z, u1.x, u2.y, c.x * l.z, c.y * l.z, c.z * l.z, leg.x, leg.y,

                v1.x, v1.y, v2.z, u1.x, u1.y, c.x * l.x, c.y * l.x, c.z * l.x, leg.x, leg.y,
                v2.x, v1.y, v2.z, u2.x, u1.y, c.x * l.w, c.y * l.w, c.z * l.w, leg.x, leg.y,
                v2.x, v2.y, v2.z, u2.x, u2.y, c.x * l.y, c.y * l.y, c.z * l.y, leg.x, leg.y
            };
        }
    }
}

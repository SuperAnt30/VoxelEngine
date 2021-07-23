using System;

namespace VoxelEngine.Util
{
    public static partial class Mth
    {
        /// <summary>
        /// Округляем в меньшую сторону
        /// </summary>
        public static int Floor(float d)
        {
            int i = (int)d;
            return d < i ? i - 1 : i;
            //return (int)Math.Floor(d);
        }

        /// <summary>
        /// Округляем до ближайшего целого
        /// </summary>
        public static int Round(float d)
        {
            return (int)Math.Round(d);
        }

        /// <summary>
        /// Квадратный корень
        /// </summary>
        public static float Sqrt(float d)
        {
            return (float)Math.Sqrt(d);
        }

        /// <summary>
        /// Вернуть обсалютное значение
        /// </summary>
        public static float Abs(float a)
        {
            return a >= 0f ? a : -a;
        }

        /// <summary>
        /// Вернуть обсалютное значение
        /// </summary>
        public static int Abs(int a)
        {
            return a >= 0 ? a : -a;
        }

        /// <summary>
        /// Возращаем наибольшее
        /// </summary>
        public static byte Max(byte v1, byte v2)
        {
            return v1 > v2 ? v1 : v2;
        }

    }
}

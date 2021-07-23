using System;

namespace VoxelEngine.Glm
{
    // ReSharper disable InconsistentNaming
    public static partial class glm
    {
        private static float[] _sinTable = new float[65536];

        public static void Initialized()
        {
            for (int i = 0; i < 65536; i++)
            {
                _sinTable[i] = (float) Math.Sin((double)i * Math.PI * 2.0f / 65536.0f);
            }
        }

        public static float degrees(float radians)
        {
            return radians * (57.295779513082320876798154814105f);
        }

        public static float radians(float degrees)
        {
            return degrees * (0.01745329251994329576923690768489f);

        }
        public static float cos(float angle)
        {
            return _sinTable[(int)(angle * 10430.378F + 16384.0F) & 65535];
           //  return (float)Math.Cos(angle);
        }

        public static float sin(float angle)
        {
            return _sinTable[(int)(angle * 10430.378F) & 65535];
           // return (float)Math.Sin(angle);
        }

        public static float tan(float angle)
        {
            float c = cos(angle);
            return c == 0 ? float.PositiveInfinity : sin(angle) / c;
            //return (float)Math.Tan(angle);
        }

        //public static float sinh(float angle)
        //{
        //    return (float)Math.Sinh(angle);
        //}

        //public static float acos(float x)
        //{
        //    return (float)Math.Acos(x);
        //}

        //public static float acosh(float x)
        //{

        //    if (x < (1f))
        //        return (0f);
        //    return (float)Math.Log(x + Math.Sqrt(x * x - (1f)));
        //}

        //public static float asin(float x)
        //{
        //    return (float)Math.Asin(x);
        //}

        //public static float asinh(float x)
        //{
        //    return (float)(x < 0f ? -1f : (x > 0f ? 1f : 0f)) * (float)Math.Log(Math.Abs(x) + Math.Sqrt(1f + x * x));
        //}

        //public static float atan(float y, float x)
        //{
        //    return (float)Math.Atan2(y, x);
        //}

        //public static float atan(float y_over_x)
        //{
        //    return (float)Math.Atan(y_over_x);
        //}

        //public static float atanh(float x)
        //{
        //    if (Math.Abs(x) >= 1f)
        //        return 0;
        //    return (0.5f) * (float)Math.Log((1f + x) / (1f - x));
        //}



        //public static float cosh(float angle)
        //{
        //    return (float)Math.Cosh(angle);
        //}



        //public static float tanh(float angle)
        //{
        //    return (float)Math.Tanh(angle);
        //}
    }
}

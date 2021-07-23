using System;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Шум Перлина
    /// </summary>
    public class Perlin
    {
        /// https://habr.com/ru/post/265775/

        public float Noise(float fx, float fy, int octaves, float persistence = 0.5f)
        {
            float amplitude = 1f; // сила применения шума к общей картине, будет уменьшаться с "мельчанием" шума
                                 // как сильно уменьшаться - регулирует persistence
            float max = 0; // необходимо для нормализации результата
            float result = 0; // накопитель результата

            while (octaves-- > 0)
            {
                max += amplitude;
                result += Noise(fx, fy) * amplitude;
                amplitude *= persistence;
                fx *= 1.5f; // удваиваем частоту шума (делаем его более мелким) с каждой октавой
                fy *= 1.5f;
            }

            return result / max;
        }

        public float Noise(float fx, float fy)
        {
            // сразу находим координаты левой верхней вершины квадрата
            int left = Mth.Floor(fx);
            int top = Mth.Floor(fy);

            // а теперь локальные координаты точки внутри квадрата
            float pointInQuadX = fx - left;
            float pointInQuadY = fy - top;

            // извлекаем градиентные векторы для всех вершин квадрата:
            float[] topLeftGradient = GetPseudoRandomGradientVector(left, top);
            float[] topRightGradient = GetPseudoRandomGradientVector(left + 1, top);
            float[] bottomLeftGradient = GetPseudoRandomGradientVector(left, top + 1);
            float[] bottomRightGradient = GetPseudoRandomGradientVector(left + 1, top + 1);

            // вектора от вершин квадрата до точки внутри квадрата:
            float[] distanceToTopLeft = new float[] { pointInQuadX, pointInQuadY };
            float[] distanceToTopRight = new float[] { pointInQuadX - 1, pointInQuadY };
            float[] distanceToBottomLeft = new float[] { pointInQuadX, pointInQuadY - 1 };
            float[] distanceToBottomRight = new float[] { pointInQuadX - 1, pointInQuadY - 1 };

            // считаем скалярные произведения между которыми будем интерполировать
            /*
             tx1--tx2
              |    |
             bx1--bx2
            */
            float tx1 = Dot(distanceToTopLeft, topLeftGradient);
            float tx2 = Dot(distanceToTopRight, topRightGradient);
            float bx1 = Dot(distanceToBottomLeft, bottomLeftGradient);
            float bx2 = Dot(distanceToBottomRight, bottomRightGradient);

            // готовим параметры интерполяции, чтобы она не была линейной:
            pointInQuadX = QunticCurve(pointInQuadX);
            pointInQuadY = QunticCurve(pointInQuadY);

            // собственно, интерполяция:
            float tx = Lerp(tx1, tx2, pointInQuadX);
            float bx = Lerp(bx1, bx2, pointInQuadX);
            float tb = Lerp(tx, bx, pointInQuadY);

            // возвращаем результат:
            return tb;
        }

        protected float Lerp(float a, float b, float t)
        {
            // return a * (t - 1) + b * t; можно переписать с одним умножением (раскрыть скобки, взять в другие скобки):
            return a + (b - a) * t;
        }

        protected float QunticCurve(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        protected float[] GetPseudoRandomGradientVector(int x, int y)
        {
            // псевдо-случайное число от 0 до 3 которое всегда неизменно при данных x и y
            int v = (int)(((x * 1836311903) ^ (y * 2971215073) + 4807526976) & 1023);
            v = permutationTable[v] & 3;

            switch (v)
            {
                case 0: return new float[] { 1, 0 };
                case 1: return new float[] { -1, 0 };
                case 2: return new float[] { 0, 1 };
                default: return new float[] { 0, -1 };
            }
        }

        protected float Dot(float[] a, float[] b)
        {
            return a[0] * b[0] + a[1] * b[1];
        }

        byte[] permutationTable;

        public void Perlin2D(int seed = 0)
        {
            var rand = new Random(seed);
            permutationTable = new byte[1024];
            rand.NextBytes(permutationTable); // заполняем случайными байтами
        }
    }
}

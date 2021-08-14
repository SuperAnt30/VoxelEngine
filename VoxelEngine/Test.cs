
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using VoxelEngine.Glm;

namespace VoxelEngine
{
    /// <summary>
    /// Объект временный не связан с проектом, чисто для тестирования разных моментов
    /// </summary>
    public class Test
    {
        #region Timer
        /// <summary>
        /// Объект для точного замера времени
        /// </summary>
        protected Stopwatch stopwatch = new Stopwatch();

        public Test() => stopwatch.Start();

        /// <summary>
        /// Запустить таймер
        /// </summary>
        protected void TimeBegin() => stopwatch.Restart();

        /// <summary>
        /// Получить результат в мили секундах
        /// </summary>
        protected float TimeEnd()
        {
            return (float)stopwatch.ElapsedTicks * 1000f / (float)Stopwatch.Frequency;
        }
        #endregion

        public void ArrayChunk()
        {
            Hashtable ht = new Hashtable();

            int dis = 64;
            Random rnd = new Random();
            vec2i[] l = new vec2i[(dis + dis + 1) * (dis + dis + 1)];
            int i = 0;
            int all = 0;
            int lc = 0;
            //for (int x = -dis; x <= dis; x++)
            //{
            //    for (int z = -dis; z <= dis; z++)
            //    {
            //        vec2i ch = new vec2i(x, z);
            //        l[i] = ch;
            //        lc++;
            //        if (rnd.Next(2) == 0) ht.Add(ch.ToString(), ch);
            //    }
            //}

            for (int z = 0; z <= dis; z++)
            {
                for (int x = -z; x <= dis; x++)
                {
                    vec2i ch = new vec2i(x, z);
                    l[i] = ch;
                    lc++;
                    if (rnd.Next(2) == 0) ht.Add(ch.ToString(), ch);
                }
            }

            TimeBegin();
            int c = 0;
            for (int k = 0; k < 10; k++)
            {
                all = 0;
                for (i = 0; i < lc; i++)
                {
                    vec2i ch = l[i];
                    if (!ht.ContainsKey(ch.ToString())) c++;
                    all++;
                }
                //for (int x = -dis; x <= dis; x++)
                //{
                //    for (int z = -dis; z <= dis; z++)
                //    {
                //        vec2i ch = new vec2i(x, z);
                //        if (!ht.ContainsKey(ch.ToString())) c++;
                //    }
                //}
            }

            float t = TimeEnd();

            string s = t.ToString();
        }

        /// <summary>
        /// Тест склейки псевдо чанков
        /// </summary>
        public void Array()
        {
            float[][] fff = new float[16][];

            Random rnd = new Random();
            int c = 0;
            for (int i = 0; i < 16; i++)
            {
                fff[i] = new float[46080]; // 60 сторона * 16 * 16 * 3 слоя
                // заполним массив всякими данными
                for (int j = 0; j < fff[i].Length; j++)
                {
                    fff[i][j] = (float)rnd.Next(1000) / 10f;
                }
                c += fff[i].Length;
            }

            // Проверяем время склейки
            float[] fff2 = new float[c];

            //TimeBegin();
            //for (int k = 0; k < 10; k++)
            //{
            //    c = 0;
            //    for (int i = 0; i < 16; i++)
            //    {
            //        for (int j = 0; j < fff[i].Length; j++)
            //            fff2[c] = fff[i][j];
            //        c++;
            //    }
            //}

            //float t = TimeEnd();

            List<float> fff3 = new List<float>();
            TimeBegin();
            for (int k = 0; k < 10; k++)
            {
                fff3.Clear();
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < fff[i].Length; j++)
                        fff3.Add(fff[i][j]);
                }
            }
           // fff2 = fff3.ToArray();

            float t = TimeEnd();

            string s = t.ToString();




        }
    }
}

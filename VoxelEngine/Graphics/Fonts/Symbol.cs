using System;
using System.Drawing;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Объект символа
    /// </summary>
    public class Symbol
    {
        public Symbol(char c)
        {
            Symb = c;
        }

        public Symbol() { }

        public static string Key = " !\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~ АБВГДЕЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзиклмнопрстуфхцчшщъыьэюяЁёЙй";

        public void Render(Bitmap bm, int advance)
        {
            int index = Key.IndexOf(Symb) + 32;
            if (index == -1) return;
            int r = Mth.Floor(index / 16f);

            U1 = (index - r * 16f) * 0.0625f;
            U2 = U1 + 0.0625f;
            V1 = r * 0.0625f;
            V2 = V1 + 0.0625f;

            _GetWidth(bm, advance);
        }

        /// <summary>
        /// Получить ширину символа
        /// </summary>
        protected void _GetWidth(Bitmap bm, int advance)
        {
            int index = Key.IndexOf(Symb) + 32;
            if (index == -1) return;
            int r = Mth.Floor(index / 16f);
            
            int x0 = (index - r * 16) * advance;
            int y0 = r * advance;
            int x1 = x0 + advance - 1;

            for (int x = x1; x >= x0; x--)
            {
                for (int y = y0; y < y0 + advance; y++)
                {
                    Color col = bm.GetPixel(x, y);
                    if (col.A > 0)
                    {
                        Width = x - x0 + 2;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Символ
        /// </summary>
        public char Symb { get; protected set; }
        /// <summary>
        /// Ширина
        /// </summary>
        public float Width { get; protected set; } = 4f;

        public float U1 { get; protected set; } = 0f;
        public float U2 { get; protected set; } = 0f;
        public float V1 { get; protected set; } = 0f;
        public float V2 { get; protected set; } = 0f;

    }
}

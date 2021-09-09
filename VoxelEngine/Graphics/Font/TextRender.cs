using VoxelEngine.Glm;
using System;
using System.Collections.Generic;

namespace VoxelEngine.Graphics.Font
{
    public class TextRender : RenderMesh
    {
        protected string _text;
        protected vec2 _location;
        protected vec4 _color;

        /// <summary>
        /// Прорисовать текст конкретного цвета с фоном
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public TextRender(float x, float y, string text, vec4 color)
        {
            _location = new vec2(x, y);
            _color = color;
            _text = text;

            _Render();
        }

        protected void _Render()
        {
            List<float> buffer = new List<float>();
            string[] stringSeparators = new string[] { "\r\n" };
            string[] strs = _text.Split(stringSeparators, StringSplitOptions.None);
            float a = FontAdvance.GetInstance().Advance + 1f;

            for (int i = 0; i < strs.Length; i++)
            {
                buffer.AddRange(_DrawBackground(_location.x, _location.y + i * a, strs[i], _color));
            }
            Render(buffer);
        }

        public void Render(string text)
        {
            if (_text != text)
            {
                _text = text;
                _Render();
            }
        }

        /// <summary>
        /// Прорисовать текст конкретного цвета с фоном
        /// </summary>
        /// <param name="x">координата Х</param>
        /// <param name="y">координата Y</param>
        /// <param name="str">текст</param>
        protected List<float> _DrawBackground(float x, float y, string str, vec4 color)
        {
            List<float> buffer = new List<float>();
            if (str == "") return buffer;
            char[] vc = str.ToCharArray();
            float w = 0;
            float a = FontAdvance.GetInstance().Advance;

            // Background

            for (int i = 0; i < vc.Length; i++)
            {
                Symbol symbol = FontAdvance.GetInstance().Get(vc[i]);
                w += symbol.Width;
            }

            vec4 c = new vec4(0, 0, 0, 0.3f);
            float x0 = x - 1f;
            float x1 = x + w + 1f;
            float y0 = y - 1f;
            float y1 = y + a;
            
            buffer.AddRange(new float[] {
                x0, y0, 0, 0.9375f, 0.9375f, c.x, c.y, c.z, c.w,
                x0, y1, 0, 0.9375f, 1f, c.x, c.y, c.z, c.w,
                x1, y0, 0, 1f, 0.9375f, c.x, c.y, c.z, c.w,

                x0, y1, 0, 0.9375f, 1f, c.x, c.y, c.z, c.w,
                x1, y1, 0, 1f, 1f, c.x, c.y, c.z, c.w,
                x1, y0, 0, 1f, 0.9375f, c.x, c.y, c.z, c.w,
            });
            

            // TEXT
            w = 0;

            for (int i = 0; i < vc.Length; i++)
            {
                Symbol symbol = FontAdvance.GetInstance().Get(vc[i]);
                buffer.AddRange(new float[] {
                    x + w, y, 0, symbol.U1, symbol.V1, color.x, color.y, color.z, color.w,
                    x + w, y + a, 0, symbol.U1, symbol.V2, color.x, color.y, color.z, color.w,
                    x + w + a, y, 0, symbol.U2, symbol.V1, color.x, color.y, color.z, color.w,

                    x + w, y + a, 0, symbol.U1, symbol.V2, color.x, color.y, color.z, color.w,
                    x + w + a, y + a, 0, symbol.U2, symbol.V2, color.x, color.y, color.z, color.w,
                    x + w + a, y, 0, symbol.U2, symbol.V1, color.x, color.y, color.z, color.w
                });
                w += symbol.Width;
            }
            return buffer;
        }
    }
}

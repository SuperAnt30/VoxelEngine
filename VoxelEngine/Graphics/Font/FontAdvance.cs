using System.Collections;
using System.Drawing;

namespace VoxelEngine.Graphics.Font
{
    /// <summary>
    /// Объект одиночка горизонтального смещения шрифта
    /// </summary>
    public class FontAdvance
    {
        #region Instance

        private static FontAdvance instance;
        private FontAdvance() { }

        /// <summary>
        /// Передать по ссылке объект если он создан, иначе создать
        /// </summary>
        /// <returns>объект Font</returns>
        public static FontAdvance GetInstance()
        {
            if (instance == null)
            {
                instance = new FontAdvance();
                instance._Initialize();
            }
            return instance;
        }

        #endregion

        /// <summary>
        /// Инициализировать шрифты
        /// </summary>
        private void _Initialize()
        {
            Bitmap bm = new Bitmap(VE.TEXTURE_FONT_PATH);
            Advance = bm.Width / 16f;

            hashtable = new Hashtable();
            char[] vc = Symbol.Key.ToCharArray();
            for (int i = 0; i < vc.Length; i++)
            {
                Symbol symbol = new Symbol(vc[i]);
                symbol.Render(bm, AdvanceI);
                if (!hashtable.ContainsKey(vc[i])) hashtable.Add(symbol.Symb, symbol);
            }
        }

        /// <summary>
        /// Массив шрифтов
        /// </summary>
        protected Hashtable hashtable = new Hashtable();

        /// <summary>
        /// Горизонтальное смещение начала следующего глифа
        /// </summary>
        public float Advance { get; protected set; } = 8f;
        /// <summary>
        /// Горизонтальное смещение начала следующего глифа
        /// </summary>
        public int AdvanceI
        {
            get { return (int)Advance; }
        }

        public Symbol Get(char key)
        {
            if (hashtable.ContainsKey(key))
            {
                return hashtable[key] as Symbol;
            }
            return new Symbol();
        }
    }
}

using VoxelEngine.Util;

namespace VoxelEngine.Model
{
    /// <summary>
    /// Сторона блока
    /// </summary>
    public class Face
    {
        /// <summary>
        /// С какой стороны
        /// </summary>
        public Pole Side { get; protected set; } = Pole.All;
        /// <summary>
        /// Номер текстуры в карте
        /// </summary>
        public int NumberTexture { get; protected set; } = 0;
        /// <summary>
        /// Применение цвета
        /// </summary>
        public bool IsColor { get; protected set; } = true;

        public Face(Pole pole, int numberTexture)
        {
            Side = pole;
            NumberTexture = numberTexture;
            IsColor = false;
        }

        public Face(Pole pole, int numberTexture, bool isColor)
        {
            Side = pole;
            NumberTexture = numberTexture;
            IsColor = isColor;
        }

    }
}

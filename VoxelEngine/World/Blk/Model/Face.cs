using VoxelEngine.Util;

namespace VoxelEngine.World.Blk.Model
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
        /// <summary>
        /// С обеих сторон
        /// </summary>
        public bool IsTwoSides { get; protected set; } = false;

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
        public Face(Pole pole, int numberTexture, bool isColor, bool isTwoSides)
        {
            Side = pole;
            NumberTexture = numberTexture;
            IsColor = isColor;
            IsTwoSides = isTwoSides;
        }

    }
}

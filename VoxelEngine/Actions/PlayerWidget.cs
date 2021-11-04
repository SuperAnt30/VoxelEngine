using VoxelEngine.World.Blk;

namespace VoxelEngine.Actions
{
    /// <summary>
    /// Виджет игрока
    /// </summary>
    public class PlayerWidget
    {
        /// <summary>
        /// Что в ячейке
        /// </summary>
        protected static EnumBlock[] cells = new EnumBlock[9];
        /// <summary>
        /// Индекс выбранной ячейки (0 - 8)
        /// </summary>
        public static int Index { get; protected set; } = 0;

        /// <summary>
        /// Под водой ли глаза
        /// </summary>
        public static bool IsEyesWater { get; set; } = false;
        /// <summary>
        /// Открыта ли форма
        /// </summary>
        public static bool IsOpenForm { get; set; } = false;

        public static void Initialized()
        {
            cells = new EnumBlock[]
            {
                EnumBlock.Stone,
                EnumBlock.Brol,
                EnumBlock.Sand,
                EnumBlock.Planks,
                EnumBlock.Log,
                EnumBlock.Water,
                EnumBlock.Glass,
                EnumBlock.Sapling,
                EnumBlock.Torch
            };
        }

        /// <summary>
        /// Следующая ячейка
        /// </summary>
        public static void IndexNext()
        {
            if (Index == 8) SetIndex(0);
            else SetIndex(Index + 1);
        }

        /// <summary>
        /// Предыдущая ячейка
        /// </summary>
        public static void IndexBack()
        {
            if (Index == 0) SetIndex(8);
            else SetIndex(Index - 1);
        }

        /// <summary>
        /// Задать выбранную ячейку
        /// </summary>
        /// <param name="index"></param>
        public static void SetIndex(int index)
        {
            if (index >= 0 && index < 9)
            {
                Index = index;
            }
            Debug.NumberBlock = GetCell();
        }

        /// <summary>
        /// Получить тип блока в конкретной ячейке
        /// </summary>
        public static EnumBlock GetCell() => cells[Index];

        /// <summary>
        /// Получить тип блока в конкретной ячейке
        /// </summary>
        public static EnumBlock GetCell(int index) => (index >= 0 && index < 9) ? cells[index] : EnumBlock.None;

        /// <summary>
        /// Задать блок в ячейку
        /// </summary>
        /// <param name="index">номер ячейки 0 - 8</param>
        /// <param name="enumBlock">тип блока</param>
        public static void SetCell(int index, EnumBlock enumBlock)
        {
            if (index >= 0 && index < 9)
            {
                cells[index] = enumBlock;
            }
            if (index == Index)
            {
                Debug.NumberBlock = enumBlock;
            }
        }
    }
}

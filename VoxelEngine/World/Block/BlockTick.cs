using VoxelEngine.Util;

namespace VoxelEngine.World
{
    /// <summary>
    /// Объект воздействия на блока через определённое время тиков
    /// </summary>
    public class BlockTick
    {
        /// <summary>
        /// Позиция блока
        /// </summary>
        public BlockPos Position { get; protected set; }
        /// <summary>
        /// Какой блок запустил проверку
        /// </summary>
        public EnumBlock EBlock { get; protected set; }
        /// <summary>
        /// Количество тиков ещё
        /// </summary>
        protected int _countTick;

        public BlockTick(BlockPos pos, EnumBlock eBlock, int countTick)
        {
            Position = pos;
            EBlock = eBlock;
            _countTick = countTick;
        }

        public void Tick()
        {
            _countTick--;
        }

        public bool IsAction()
        {
            return _countTick <= 0;
        }
    }
}

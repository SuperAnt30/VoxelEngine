using VoxelEngine.Util;

namespace VoxelEngine.World
{
    /// <summary>
    /// Объект воздействия на блока через определённое время тиков
    /// </summary>
    public class BlockTick
    {
        public BlockPos Position { get; protected set; }

        /// <summary>
        /// Количество тиков ещё
        /// </summary>
        protected int _countTick;

        public BlockTick(BlockPos pos, int countTick)
        {
            Position = pos;
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

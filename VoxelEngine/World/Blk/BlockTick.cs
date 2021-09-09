using VoxelEngine.Binary;
using VoxelEngine.Util;

namespace VoxelEngine.World.Blk
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
        public int CountTick { get; protected set; }

        public BlockTick(BlockPos pos, EnumBlock eBlock, int countTick)
        {
            Position = pos;
            EBlock = eBlock;
            CountTick = countTick;
        }

        public BlockTickBin Get()
        {
            return new BlockTickBin()
            {
                EBlock = (ushort)this.EBlock,
                CountTick = this.CountTick,
                Position = this.Position.ToVec3()
            };
        }

        public void Tick()
        {
            CountTick--;
        }

        public bool IsAction()
        {
            return CountTick <= 0;
        }
    }
}

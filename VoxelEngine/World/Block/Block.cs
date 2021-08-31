using VoxelEngine.Glm;
using VoxelEngine.Model;
using VoxelEngine.Util;

namespace VoxelEngine.World
{
    /// <summary>
    /// Объект Блока
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Коробки
        /// </summary>
        public Box[] Boxes { get; protected set; } = new Box[] { new Box() };
        /// <summary>
        /// Вся ли прорисовка, аналог кактус, забор...
        /// </summary>
        public bool AllDrawing { get; protected set; } = false;
        /// <summary>
        /// Альфа блок, вода, стекло...
        /// </summary>
        public bool IsAlphe { get; protected set; } = false;
        /// <summary>
        /// Цвет блока
        /// </summary>
        public vec4 Color { get; protected set; } = new vec4(1f, 1f, 1f, 1f);
        
        /// <summary>
        /// Есть ли столкновение
        /// </summary>
        public bool IsCollision { get; protected set; } = true;
        /// <summary>
        /// Можно ли выбирать блок
        /// </summary>
        public bool IsAction { get; protected set; } = true;
        /// <summary>
        /// Вода ли это
        /// </summary>
        public bool IsWater { get; protected set; } = false;
        /// <summary>
        /// Листва ли это
        /// </summary>
        public bool IsLeaves { get; protected set; } = false;
        /// <summary>
        /// Явлыется ли блок небом
        /// </summary>
        public bool IsAir { get { return EBlock == EnumBlock.Air; } }

        /// <summary>
        /// Получить тип блока
        /// </summary>
        public EnumBlock EBlock { get; protected set; }

        /// <summary>
        /// Количество излучаемого света (плафон)
        /// </summary>
        public byte LightValue { get; protected set; } = 0;

        /// <summary>
        /// Дополнительный параметр блока 4 бита
        /// </summary>
        public byte Properties { get; set; } = 0;

        /// <summary>
        /// Строка
        /// </summary>
        public override string ToString()
        {
            return EBlock.ToString() + " " + Position.ToString();
        }

        public Block() { }

        public Voxel Voxel { get; protected set; }

        public void SetVoxel(Voxel voxel)
        {
            Voxel = voxel;
            EBlock = voxel.GetEBlock();
            Properties = voxel.GetParam4bit();
        }

        /// <summary>
        /// Позиция блока в мире
        /// </summary>
        public BlockPos Position { get; protected set; } = new BlockPos();
        /// <summary>
        /// Задать позицию блока
        /// </summary>
        public void SetPosition(BlockPos pos)
        {
            Position = pos;
        }

        /// <summary>
        /// Сколько света вычитается для прохождения этого блока
        /// </summary>
        public byte GetBlockLightOpacity()
        {
            return Blocks.GetBlockLightOpacity(EBlock);
        }

    }
}

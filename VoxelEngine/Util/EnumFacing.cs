using System;
using VoxelEngine.Glm;

namespace VoxelEngine.Util
{
    /// <summary>
    /// Объект перечня сторон
    /// </summary>
    public class EnumFacing
    {
        /// <summary>
        /// Нормализованный вектор, указывающий в направлении этой облицовки
        /// </summary>
        public static vec3i DirectionVec(Pole pole)
        {
            switch (pole)
            {
                case Pole.Up: return new vec3i(0, 1, 0);
                case Pole.Down: return new vec3i(0, -1, 0);
                case Pole.East: return new vec3i(1, 0, 0);
                case Pole.West: return new vec3i(-1, 0, 0);
                case Pole.North: return new vec3i(0, 0, -1);
                case Pole.South: return new vec3i(0, 0, 1);
                default: throw new ArgumentNullException("Не существует такой стороны");
            }
            
        }

        /// <summary>
        /// Получите облицовку, соответствующую заданному углу (0-360). Угол 0 - SOUTH, угол 90 - WEST.
        /// </summary>
        /// <param name="angle">угол в градусах</param>
        public static Pole FromAngle(float angle)
        {
            //return GetHorizontal(Mth.Floor(angle / 90.0f + 0.5f) & 3);
            if (angle >= -45f && angle <= 45f) return Pole.South;
            else if (angle > 45f && angle < 135f) return Pole.West;
            else if (angle < -45f && angle > -135f) return Pole.East;
            return Pole.North;
        }


        /// <summary>
        /// Получите сторону по его горизонтальному индексу (0-3). Заказ S-W-N-E.
        /// </summary>
        /// <param name="index">индекс (0-3)</param>
        public static Pole GetHorizontal(int index)
        {
            switch (index)
            {
                case 2: return Pole.North;
                case 0: return Pole.South;
                case 1: return Pole.West;
                case 3: return Pole.East;
                default: throw new ArgumentNullException("Не существует такой стороны");
            }
        }
        /// <summary>
        /// Нормализованный горизонтальный вектор, указывающий в направлении этой облицовки
        /// </summary>
        public static vec3i DirectionHorizontalVec(Pole pole)
        {
            switch (pole)
            {
                case Pole.East: return new vec3i(1, 0, 0);
                case Pole.West: return new vec3i(-1, 0, 0);
                case Pole.North: return new vec3i(0, 0, -1);
                case Pole.South: return new vec3i(0, 0, 1);
                default: throw new ArgumentNullException("Не существует такой стороны");
            }
        }

        ///// <summary>
        ///// Массив сторон для растекания жидкости с дном
        ///// </summary>
        ///// <returns></returns>
        //public static Pole[] LiquidDirection()
        //{
        //    return new Pole[] { Pole.  }

        //}



        //public static readonly EnumFacing DOWN = new EnumFacing(0, 1, -1, new vec3i(0, -1, 0));
        //public static readonly EnumFacing UP = new EnumFacing(1, 0, -1, new vec3i(0, 1, 0));
        //public static readonly EnumFacing NORTH = new EnumFacing(2, 3, 2, new vec3i(0, 0, -1));
        //public static readonly EnumFacing SOUTH = new EnumFacing(3, 2, 0, new vec3i(0, 0, 1));
        //public static readonly EnumFacing WEST = new EnumFacing(4, 5, 1, new vec3i(-1, 0, 0));
        //public static readonly EnumFacing EAST = new EnumFacing(5, 4, 3, new vec3i(1, 0, 0));

        //public static IEnumerable<EnumFacing> Values
        //{
        //    get
        //    {
        //        yield return NORTH;
        //        yield return SOUTH;
        //        yield return WEST;
        //        yield return EAST;
        //        yield return UP;
        //        yield return DOWN;
        //    }
        //}

        ///** Ordering index for D-U-N-S-W-E */
        //public int Index { get; protected set; }

        ///** Index of the opposite Facing in the VALUES array */
        //protected int _opposite;

        ///** Oredering index for the HORIZONTALS field (S-W-N-E) */
        //public int HorizontalIndex { get; protected set; }

        ///** Normalized Vector that points in the direction of this Facing */
        //public vec3i DirectionVec { get; protected set; }


        //private EnumFacing(int index, int opposite, int horizontalIndex, vec3i directionVec)
        //{
        //    Index = index;
        //    _opposite = opposite;
        //    HorizontalIndex = horizontalIndex;
        //    DirectionVec = directionVec;
        //}

        ///// <summary>
        ///// Get the opposite Facing (e.g. DOWN => UP)
        ///// </summary>
        //public EnumFacing GetOpposite()
        //{
        //    return GetFront(_opposite);
        //}

        ///// <summary>
        ///// Получите сторону по его индексу (0-5). Заказ D-U-N-S-W-E.
        ///// </summary>
        ///// <param name="index">индекс (0-5)</param>
        //public static EnumFacing GetFront(int index)
        //{
        //    switch(index)
        //    {
        //        case 1: return DOWN;
        //        case 2: return UP;
        //        case 3: return NORTH;
        //        case 4: return SOUTH;
        //        case 5: return WEST;
        //        case 6: return EAST;
        //        default: throw new ArgumentNullException("Не существует такой стороны");
        //    }
        //}

        ////




        ///// <summary>
        ///// Север
        ///// </summary>
        //North = 4,
        ///// <summary>
        ///// Юг
        ///// </summary>
        //South = 5,
        ///// <summary>
        ///// Запад
        ///// </summary>
        //West = 3,
        ///// <summary>
        ///// Восток
        ///// </summary>
        //East = 2,
        ///// <summary>
        ///// Вверх
        ///// </summary>
        //Up = 0,
        ///// <summary>
        ///// Низ
        ///// </summary>
        //Down = 1,
        ///// <summary>
        ///// Все стороны
        ///// </summary>
        //All = -1;

    }
}


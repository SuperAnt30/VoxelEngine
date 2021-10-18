namespace VoxelEngine.Util
{
    /// <summary>
    /// Побитовые методы
    /// </summary>
    public class Bit
    {
        /// <summary>
        /// Является ли число чётным
        /// </summary>
        public static bool IsEven(int value) => (value & 1) == 0;
        /// <summary>
        /// Является ли число нечётным
        /// </summary>
        public static bool IsOdd(int value) => (value & 1) == 1;
    }
}

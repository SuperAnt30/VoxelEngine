namespace VoxelEngine
{
    /// <summary>
    /// Voxel Engine Configuration
    /// </summary>
    public class VEC
    {
        #region Instance

        private static VEC instance;
        private VEC() { }

        /// <summary>
        /// Передать по ссылке объект если он создан, иначе создать
        /// </summary>
        /// <returns>объект Debag</returns>
        public static VEC GetInstance()
        {
            if (instance == null) instance = new VEC();
            return instance;
        }

        #endregion

        /// <summary>
        /// Режим перемещения
        /// </summary>
        public VEMoving Moving { get; set; } = VEMoving.FreeFlight;// ObstacleFlight;//.Survival;//.FreeFlight;

        /// <summary>
        /// Режим теней у блоках
        /// </summary>
        public bool AmbientOcclusion { get; set; } = true;

        /// <summary>
        /// Режим перемещения
        /// </summary>
        public enum VEMoving
        {
            /// <summary>
            /// Свободный полёт
            /// </summary>
            FreeFlight = 1,
            /// <summary>
            /// Полёт с препятствием
            /// </summary>
            ObstacleFlight = 2,
            /// <summary>
            /// Выживание
            /// </summary>
            Survival = 3
        }

    }
}

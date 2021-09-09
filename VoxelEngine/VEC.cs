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
        public VEMoving Moving { get; set; } = VEMoving.Survival;// ObstacleFlight;//.Survival;//.FreeFlight;

        /// <summary>
        /// Режим теней у блоках
        /// </summary>
        public bool AmbientOcclusion { get; set; } = true;

        /// <summary>
        /// Размер курсора
        /// </summary>
        public int Zoom { get; set; } = 1;

        #region Tick

        /// <summary>
        /// Счётчик времени игры
        /// </summary>
        public long TickCount { get; protected set; } = 0;
        /// <summary>
        /// Задать время тика
        /// </summary>
        /// <param name="tick"></param>
        public void SetTick(long tick)
        {
            TickCount = tick;
        }
        /// <summary>
        /// Добавить четверть суток
        /// </summary>
        public void AddQuarterTick()
        {
            TickCount += VE.COUNT_TICE_DAY / 4;
        }
        /// <summary>
        /// Такт времени
        /// </summary>
        public void Tick()
        {
            TickCount++;

            // Расчёт яркости неба
            float t = VE.COUNT_TICE_DAY;
            long it = TickCount / VE.COUNT_TICE_DAY;
            AngleSun = (float)(TickCount - it * t) / (t / 6.283185f);
            LeghtSky = (float)(TickCount - it * t) / t * 2f;
            if (LeghtSky > 1f) LeghtSky = 2f - LeghtSky;
            LeghtSky = 1f - LeghtSky;
        }

        #endregion

        /// <summary>
        /// Яркость освещения неба
        /// </summary>
        public float LeghtSky { get; protected set; }
        /// <summary>
        /// Угол для вращения солнца
        /// </summary>
        public float AngleSun { get; protected set; }


    }
}

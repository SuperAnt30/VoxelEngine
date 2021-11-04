namespace VoxelEngine
{
    /// <summary>
    /// Voxel Engine Configuration
    /// Объект статический параметров как конфигурации
    /// </summary>
    public class VEC
    {
        /// <summary>
        /// Количество FPS (кадров в секунду)
        /// </summary>
        public static int fps = 60;
        /// <summary>
        /// Сколько чанков видим
        /// </summary>
        public static int chunkVisibility = 8;

        /// <summary>
        /// Режим перемещения
        /// </summary>
        public static VEMoving moving = VEMoving.Survival;// ObstacleFlight;//.Survival;//.FreeFlight;

        /// <summary>
        /// Режим теней у блоках
        /// </summary>
        public static bool ambientOcclusion = true;

        /// <summary>
        /// Видем ли на экране дебаг текстурного атласа
        /// </summary>
        public static bool isDebugTextureAtlas = false;
        
        /// <summary>
        /// Размер курсора
        /// </summary>
        //public int Zoom { get; set; } = 1;

        #region Tick

        /// <summary>
        /// Счётчик времени игры
        /// </summary>
        public static long TickCount { get; protected set; } = 0;
        /// <summary>
        /// Задать время тика
        /// </summary>
        /// <param name="tick"></param>
        public static void SetTick(long tick) => TickCount = tick;
        /// <summary>
        /// Добавить четверть суток
        /// </summary>
        public static void AddQuarterTick() => TickCount += VE.COUNT_TICE_DAY / 4;
        /// <summary>
        /// Такт времени
        /// </summary>
        public static void Tick()
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

        #region Entity

        /// <summary>
        /// Счётчик порядкового номера сущьностей
        /// </summary>
        public static int EntityIndex { get; protected set; } = 0;
        /// <summary>
        /// Увеличить порядковый номер сущьности
        /// </summary>
        public static void EntityAdd() => EntityIndex++;

        #endregion

        #region SkyBox

        /// <summary>
        /// Яркость освещения неба
        /// </summary>
        public static float LeghtSky { get; protected set; }
        /// <summary>
        /// Угол для вращения солнца
        /// </summary>
        public static float AngleSun { get; protected set; }

        #endregion
    }
}

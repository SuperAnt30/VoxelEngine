namespace VoxelEngine
{
    /// <summary>
    /// Объект констант
    /// </summary>
    public class VE
    {
        /// <summary>
        /// Сколько чанков обрабатывается для альфы при смене чанка
        /// Количество общее число по спирале
        /// </summary>
        public const int CHUNK_RENDER_ALPHA = 16;
        /// <summary>
        /// Сколько чанков обрабатывается для альфы при смене блока
        /// Количество общее число по спирале
        /// </summary>
        public const int CHUNK_RENDER_ALPHA_BLOCK = 3;
        /// <summary>
        /// Скольк чанков вблизи камеры активны к тику 49 это 7*7 где чанк где камеры и 3 дистанция
        /// </summary>
        public const int CHUNKS_TICK = 49;

        /// <summary>
        /// Количество тиков в сутках
        /// </summary>
        public const int COUNT_TICE_DAY = 24000;

        /// <summary>
        /// Максимальная дистанция для луча
        /// </summary>
        public const float MAX_DIST = 10f;

        
        /// <summary>
        /// Ключ текстуры шрифта
        /// </summary>
        public const string TEXTURE_FONT_KEY = "font_ascii";
        /// <summary>
        /// Адрес текстуры шрифта
        /// </summary>
        public const string TEXTURE_FONT_PATH = @"textures\font\ascii4.png";
        /// <summary>
        /// Основной атлас текстуры
        /// </summary>
        public const string TEXTURE_ATLAS = @"textures\256.png";
        /// <summary>
        /// Текстура стоячей воды
        /// </summary>
        public const string TEXTURE_WATER_STILL = @"textures\water_still.png";
        /// <summary>
        /// Текстура течения воды
        /// </summary>
        public const string TEXTURE_WATER_FLOW = @"textures\water_flow.png";


        /// <summary>
        /// На сколько растекается вода (будет на 1 больше)
        /// </summary>
        public const byte WATER = 5;
        /// <summary>
        /// Уровень вады для высот полигона, = 1 / (WATER)
        /// </summary>
        public const float WATER_LEVEL = 0.19f;

        /// <summary>
        /// Скорость авто прыжка
        /// </summary>
        public const float SPEED_AUTOJAMP = 11f; //9
        /// <summary>
        /// Скорость авто прыжка в воде
        /// </summary>
        public const float SPEED_WATER_AUTOJAMP = 4f; //9
        /// <summary>
        /// Константа для авто прыжка чтоб встать
        /// </summary>
       // public const float SPEED_UPING = .5f; // 7f
        /// <summary>
        /// Скорость падения если нет блоков под нагами
        /// Чем больше тем быстрее падает
        /// </summary>
        public const float SPEED_DOWN = 4f; //48
        /// <summary>
        /// Скорость течения
        /// </summary>
        public const float SPEED_FLOW = 1f;

        /// <summary>
        /// Скорость растекания воды
        /// </summary>
        public const int TICK_WATER = 5;
        /// <summary>
        /// Скорость высыхания воды
        /// </summary>
        public const int TICK_WATER_DRY = 4;

        /// <summary>
        /// Время через сколько вырастет дерево
        /// </summary>
        public const int TICK_TREE_TIME = 60;
        /// <summary>
        /// Время через сколько вырастет дерево если сразу не получилось
        /// </summary>
        public const int TICK_TREE_REPEAT = 10;
        /// <summary>
        /// Через сколько будет исчезать листва
        /// </summary>
        public const int TICK_LEAVES = 10;
        /// <summary>
        /// Растояние высыхание листвы от бревна
        /// </summary>
        public const int SIZE_LEAVES = 4;

        /// <summary>
        /// На каком растоянии диспавн сущьностей
        /// </summary>
        public const int ENTITY_DISSPAWN = 64;

        #region Size
        /// <summary>
        /// 1/16 часть, для тестуры
        /// </summary>
        public const float UV_SIZE = 0.0625f;
        #endregion
    }
}

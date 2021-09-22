namespace VoxelEngine
{
    /// <summary>
    /// Объект констант
    /// </summary>
    public class VE
    {
        /// <summary>
        /// Сколько чанков видим
        /// </summary>
        public const int CHUNK_VISIBILITY = 12;
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
        /// 1/16 часть, для тестуры
        /// </summary>
        public const float UV_SIZE = 0.0625f;
        /// <summary>
        /// Ключ текстуры шрифта
        /// </summary>
        public static string TEXTURE_FONT_KEY = "font_ascii";
        /// <summary>
        /// Адрес текстуры шрифта
        /// </summary>
        public static string TEXTURE_FONT_PATH = @"textures\font\ascii4.png";


        /// <summary>
        /// На сколько растекается вода (будет на 1 больше)
        /// </summary>
        public static byte WATER = 5;
        /// <summary>
        /// Уровень вады для высот полигона, = 1 / (WATER)
        /// </summary>
        public static float WATER_LEVEL = 0.19f;

        /// <summary>
        /// Скорость шага
        /// </summary>
        //public const float SPEED_STEP = 8.6f;//4.3f;
        /// <summary>
        /// Скорость движения сидя
        /// </summary>
        //public const float SPEED_SNEAKING = 2.6f;//1.3f;
        /// <summary>
        /// Скорость бега
        /// </summary>
        //public const float SPEED_RUN = 11.2f;// 5.6f;
        /// <summary>
        /// Скорость прыжка за 0,2 секунды
        /// </summary>
        //public const float SPEED_JAMP = 20f; //16//10 лучше, это чуть больше 2 блоков, если 8 это 2 блока, но бага если нижнего блока нет, подвисает
        /// <summary>
        /// Скорость прыжка в воде на мели
        /// </summary>
        //public const float SPEED_WATER_JAMP = 12f;
        /// <summary>
        /// Скорость авто прыжка и когда встаём из положения сидя
        /// </summary>
        public const float SPEED_AUTOJAMP = 11f; //9
        /// <summary>
        /// Скорость авто прыжка и когда встаём из положения сидя в воде
        /// </summary>
        public const float SPEED_WATER_AUTOJAMP = 4f; //9
        /// <summary>
        /// Константа для авто прыжка чтоб встать
        /// </summary>
        public const float SPEED_UPING = .5f; // 7f
        /// <summary>
        /// Скорость падения если нет блоков под нагами
        /// Чем больше тем быстрее падает
        /// </summary>
        public const float SPEED_DOWN = 4f; //48
        /// <summary>
        /// Скорость полёта
        /// </summary>
        //public const float SPEED_FLY = 22f;
        /// <summary>
        /// Скорость быстрого полёта
        /// </summary>
        //public const float SPEED_FLY_FAST = 44f; // 22
        /// <summary>
        /// Скорость полёта вертикали
        /// </summary>
       // public const float SPEED_FLY_VERTICAL = 12f;
        /// <summary>
        /// Скорость течения
        /// </summary>
        public const float SPEED_FLOW = 1f;
        /// <summary>
        /// Скорость плавание под водой вверх/вниз
        /// </summary>
        //public const float SPEED_SWIM = 4f;

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
        public const int ENTITY_DISSPAWN = 48;

    }
}

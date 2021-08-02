namespace VoxelEngine
{
    /// <summary>
    /// Объект констант
    /// </summary>
    public class VE
    {

        public const int CHUNK_VISIBILITY = 6;
        //public const int CHUNK_ALL = 221;

        //public const int CHUNK_VISIBILITY = 10;
        //public const int CHUNK_ALL = 529;

        //public const int CHUNK_VISIBILITY = 16;
        //public const int CHUNK_ALL = 1225;

        //public const int CHUNK_VISIBILITY = 32;
        //public const int CHUNK_ALL = 4489;

        //public const int CHUNK_VISIBILITY = 51;
        //public const int CHUNK_ALL = 10609;

        /// <summary>
        /// Сколько чанков видим
        /// </summary>
        //public const int CHUNK_VISIBILITY = 11;
        /// <summary>
        /// Количество чанков в кэше и для сетки
        /// </summary>
        //public const int CHUNK_ALL = 529; //529 (11 + 11 + 1) * (11 + 11 + 1)

        /// <summary>
        /// Сколько чанков обрабатывается для альфы при смене чанка
        /// </summary>
        public const int CHUNK_VISIBILITY_ALPHA = 5;
        /// <summary>
        /// Скольк чанков вблизи камеры активны к тику 49 это 7*7 где чанк где камеры и 3 дистанция
        /// </summary>
        public const int CHUNKS_TICK = 49;
        /// <summary>
        /// Ширина и длинна чанка
        /// </summary>
       // public const int CHUNK_WIDTH = 16;
        /// <summary>
        /// Высота чанка
        /// </summary>
       // public const int CHUNK_HEIGHT = 256;
        /// <summary>
        /// Размер чанка в байтах 16 * 16 * 256
        /// </summary>
        public const int CHUNK_VOLUME = 65536;
        //public const int CHUNK_VOLUME = 67108864; // 512*512*256 (32*32 чанка)    

        /// <summary>
        /// 1/16 часть, для тестуры
        /// </summary>
        public const float UV_SIZE = 0.0625f;

        /// <summary>
        /// Размер региона в битах 5 bit = 32
        /// </summary>
        //public const byte REGION_SIZE_BIT = 5;

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
        public const float SPEED_STEP = 8.6f;//4.3f;
        /// <summary>
        /// Скорость движения сидя
        /// </summary>
        public const float SPEED_SNEAKING = 2.6f;//1.3f;
        /// <summary>
        /// Скорость бега
        /// </summary>
        public const float SPEED_RUN = 11.2f;// 5.6f;
        /// <summary>
        /// Скорость прыжка за 0,2 секунды
        /// </summary>
        public const float SPEED_JAMP = 20f; //16//10 лучше, это чуть больше 2 блоков, если 8 это 2 блока, но бага если нижнего блока нет, подвисает
        /// <summary>
        /// Скорость авто прыжка и когда встаём из положения сидя
        /// </summary>
        public const float SPEED_AUTOJAMP = 10f; //9
        /// <summary>
        /// Скорость падения если нет блоков под нагами
        /// Чем больше тем быстрее падает
        /// </summary>
        public const float SPEED_DOWN = 4f; //48
        /// <summary>
        /// Скорость полёта
        /// </summary>
        public const float SPEED_FLY = 11f;
        /// <summary>
        /// Скорость быстрого полёта
        /// </summary>
        public const float SPEED_FLY_FAST = 22f; // 22
        /// <summary>
        /// Скорость полёта вертикали
        /// </summary>
        public const float SPEED_FLY_VERTICAL = 12f;
        /// <summary>
        /// Скорость течения
        /// </summary>
        public const float SPEED_FLOW = 1f;
        /// <summary>
        /// Скорость плавание под водой вверх/вниз
        /// </summary>
        public const float SPEED_SWIM = 4f;

    }
}

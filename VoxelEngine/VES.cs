using System.Collections.Generic;
using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World.Chk;

namespace VoxelEngine
{
    /// <summary>
    /// Voxel Engine Starting
    /// Генерация специальных массивов, для движка, чтоб потом быстрее по массиву обрабатывать
    /// </summary>
    public class VES
    {
        #region Instance

        private static VES instance;
        private VES()
        {
            DistSqrt = _GetSqrt(VE.CHUNK_VISIBILITY);
            _GetSqrtPole3();

            for (int i = 0; i <=16; i++)
            {
                Uv[i] = (float)i * 0.00390625f;
                Xy[i] = (float)i * 0.0625f;
            }
        }

        /// <summary>
        /// Передать по ссылке объект если он создан, иначе создать
        /// </summary>
        /// <returns>объект OpenGLFunctions</returns>
        public static VES GetInstance()
        {
            if (instance == null) instance = new VES();
            return instance;
        }

        #endregion

        /// <summary>
        /// Массив по длинам используя квадратный корень для всей видимости
        /// </summary>
        public ChunkLoading[] DistSqrt { get; protected set; }
        /// <summary>
        /// Массив по длинам используя квадратный корень для всей видимости
        /// </summary>
        protected ChunkLoading[][] distSqrtPole = new ChunkLoading[8][];

        /// <summary>
        /// Параметр [0]=0 .. [16]=0.0625f
        /// </summary>
        public static float[] Uv { get; protected set; } = new float[17];
        /// <summary>
        /// Параметр [1]=0.0625f .. [16]=1.0f
        /// </summary>
        public static float[] Xy { get; protected set; } = new float[17];


        /// <summary>
        /// Сгенерировать массив по длинам используя квадратный корень
        /// </summary>
        /// <param name="vis">Видимость, в одну сторону от ноля</param>
        protected ChunkLoading[] _GetSqrt(int vis)
        {
            List<ChunkLoading> r = new List<ChunkLoading>();
            for (int x = -vis; x <= vis; x++)
                for (int y = -vis; y <= vis; y++)
                {
                    r.Add(new ChunkLoading(x, y, Mth.Sqrt(x * x + y * y)));
                }
            r.Sort();
            return r.ToArray();
        }

        


        /// <summary>
        /// Определить массив приоритетных чанков, по градусу Yaw
        /// </summary>
        /// <param name="yaw"></param>
        /// <returns></returns>
        public ChunkLoading[] DistSqrtYaw(float yaw)
        {
            int i;

            if (yaw >= 0)
            {
                if (yaw < glm.pi45) i = 0;
                else if (yaw < glm.pi90) i = 1;
                else if (yaw < glm.pi135) i = 2;
                else i = 3;
            } else
            {
                if (yaw > -glm.pi45) i = 7;
                else if (yaw > -glm.pi90) i = 6;
                else if (yaw > -glm.pi135) i = 5;
                else i = 4;
            }

            //Debag.GetInstance().BB = i.ToString();
            return distSqrtPole[i];
        }

        protected void _GetSqrtPoleOne(int i, List<ChunkLoading> r)
        {
            r.Sort();
            
            // TODO:: тест смещаем где я стаю на 5-ое место чтоб небыло фантомов от блоков возле разрыва чанков
            // {
            //ChunkLoading chunk = r[0];
            //for (int j = 0; j < 4; j++) r[j] = r[j + 1];
            //r[4] = chunk;
            // } сработало круто когда удалем блоки, а вот ставим, появляется фантом!!!

            distSqrtPole[i] = r.ToArray();
            r.Clear();
        }
        /// <summary>
        /// Заполнение сеток секторов
        /// </summary>
        protected void _GetSqrtPole3()
        {
            int dis = VE.CHUNK_VISIBILITY;
            List<ChunkLoading> r = new List<ChunkLoading>();

            // 0 .. 45
            for (int z = 0; z >= -dis; z--)
                for (int x = -z; x >= -dis; x--)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(0, r);

            // 45 .. 90
            for (int x = 0; x >= -dis; x--)
                for (int z = -x; z >= -dis; z--)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(1, r);

            // 90 .. 135
            for (int x = 0; x >= -dis; x--)
                for (int z = x; z <= dis; z++)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(2, r);

            // 135 .. 180
            for (int z = 0; z <= dis; z++)
                for (int x = z; x >= -dis; x--)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(3, r);

            // 0 .. -45
            for (int z = 0; z >= -dis; z--)
                for (int x = dis; x >= z; x--)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(7, r);

            // -45 .. -90
            for (int x = 0; x <= dis; x++)
                for (int z = -dis; z <= x; z++)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(6, r);

            // -90 .. -135
            for (int x = 0; x <= dis; x++)
                for (int z = -x; z <= dis; z++)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(5, r);

            // -135 .. -180
            for (int z = 0; z <= dis; z++)
                for (int x = -z; x <= dis; x++)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(4, r);
        }
        /// <summary>
        /// Заполнение сеток секторов
        /// </summary>
        protected void _GetSqrtPole0125()
        {
            int dis = VE.CHUNK_VISIBILITY;
            List<ChunkLoading> r = new List<ChunkLoading>();

            // 0 .. 45
            for (int z = 0; z >= -dis; z--)
                for (int x = 0; x >= z; x--)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(0, r);

            // 45 .. 90
            for (int x = 0; x >= -dis; x--)
                for (int z = 0; z >= x; z--)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(1, r);

            // 90 .. 135
            for (int x = 0; x >= -dis; x--)
                for (int z = 0; z <= -x; z++)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(2, r);

            // 135 .. 180
            for (int z = 0; z <= dis; z++)
                for (int x = 0; x >= -z; x--)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(3, r);

            // 0 .. -45
            for (int z = 0; z >= -dis; z--)
                for (int x = 0; x <= -z; x++)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(7, r);

            // -45 .. -90
            for (int x = 0; x <= dis; x++)
                for (int z = 0; z >= -x; z--)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(6, r);

            // -90 .. -135
            for (int x = 0; x <= dis; x++)
                for (int z = 0; z <= x; z++)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(5, r);

            // -135 .. -180
            for (int z = 0; z <= dis; z++)
                for (int x = 0; x <= z; x++)
                    r.Add(new ChunkLoading(x, z, Mth.Sqrt(x * x + z * z)));
            _GetSqrtPoleOne(4, r);
        }


    }
}

﻿using System.Collections.Generic;
using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World.Blk;

namespace VoxelEngine.World.Chk.Light
{
    /// <summary>
    /// Данные и обработки освещения
    /// </summary>
    public class ChunkLight : ChunkHeir
    {
        /// <summary>
        /// Высотная карта самый высокий блок в чанке от неба
        /// </summary>
        public int HeightMapMax { get; protected set; } = 0;
        /// <summary>
        /// Карта высот по чанку, XZ
        /// </summary>
        protected int[,] heightMap = new int[16, 16];

        public ChunkLight(ChunkBase chunk) : base(chunk) { }

        /// <summary>
        /// Сколько света вычитается для прохождения этого блока Air = 0
        /// </summary>
        public byte GetBlockLightOpacity(int x, int y, int z)
            => Blocks.GetBlockLightOpacity(Chunk.GetVoxel(x, y, z).GetEBlock());

        #region Modify Block

        /// <summary>
        /// Проверяем освещение блока и неба при изменении блока
        /// </summary>
        /// <param name="pos">позиция блока</param>
        /// <param name="opacity">Прозрачность тикущего блока</param>
        /// <param name="opacityOld">Прозрачность удалённого блока</param>
        /// <param name="isDifferentBright">Разность яркости блоков</param>
        public void CheckLightSetBlock(BlockPos pos, int opacity, int opacityOld, bool isDifferentBright)
        {
            WorkingLight light = new WorkingLight(Chunk, pos);
            bool isSky = RelightSky(pos, opacity, light);
            if (opacity != opacityOld || isDifferentBright)
            {
                light.CheckLight(pos, isSky);
            }
            light.ModifiedRender();
        }

        public void CheckLightSetBlock(BlockPos pos, int opacity, WorkingLight light)
        {
            bool isSky = RelightSky(pos, opacity, light);
            light.CheckLight(pos, isSky);
        }

        #endregion

        #region SkyColumn

        /// <summary>
        /// Небесный просвет, проверка открытия закрытия вверхнего блока
        /// </summary>
        /// <param name="pos">позиция блока</param>
        /// <param name="opacity">Сколько света вычитается для прохождения света</param>
        /// <returns>Надо ли проверять блок неба</returns>
        protected bool RelightSky(BlockPos pos, int opacity, WorkingLight light)
        {
            int yh = GetHeight(pos.X & 15, pos.Z & 15);
            if (opacity > 0) // Не небо
            {
                // закрываем небо
                if (pos.Y >= yh)
                {
                    RelightBlock(pos.X, pos.Y + 1, pos.Z, light, yh);
                    return false;
                }
            }
            else if (pos.Y == yh)
            {
                // Открываем небо
                RelightBlock(pos.X, pos.Y - 1, pos.Z, light, yh);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Обновить освещение в диапозоне столбца блоков
        /// </summary>
        /// <param name="x">глобальные X</param>
        /// <param name="y0">глобальные Y</param>
        /// <param name="z">глобальные Z</param>
        /// <param name="light">объект обработки освещения</param>
        /// <param name="yh">максимальная высота</param>
        protected void RelightBlock(int x, int y0, int z, WorkingLight light, int yh)
        {
            int bx = x & 15;
            int bz = z & 15;
            int yh0 = yh;
            // Определяем нижний блок
            if (y0 > yh) yh0 = y0;
            while (yh0 > 0 && GetBlockLightOpacity(bx, yh0 - 1, bz) == 0) { yh0--; }
            // Если блок равен высотной игнорируем
            if (yh == y0) return;

            if (yh < yh0)
            {
                // закрыли небо, надо затемнять
                int yDown = yh + 1;
                int yUp = yh0 - 1;
                UpdateHeight(bx, yUp, bz);
                light.CheckDarkenLightFor(x, yDown, yUp, z);
            }
            else
            {
                // открыли небо, надо осветлять
                int yDown = yh0;
                int yUp = yh;
                // пометка что убераем вверхний блок
                bool hMax = yh == HeightMapMax;
                UpdateHeight(bx, yDown - 1, bz);
                light.CheckLightFor(x, yDown, yUp + 1, z);
                // обновляем максимальные высоты
                if (hMax) GenerateHeightMap();
            }
        }

        #endregion

        #region Height

        /// <summary>
        /// Возвращает значение карты высот в этой координате x, z в чанке. 
        /// </summary>
        public int GetHeight(int x, int z) => heightMap[x, z];

        /// <summary>
        /// Обновить высоту если это надо
        /// </summary>
        public void UpdateHeight(int x, int y, int z)
        {
            heightMap[x, z] = y;
            if (HeightMapMax < y) HeightMapMax = y;
        }

        /// <summary>
        /// Создает карту высот для блока с нуля и тут же осветляет небесные блоки
        /// </summary>
        /// <param name="isSky">осветлять ли небо</param>
        protected void GenerateHeight(bool isSky)
        {
            HeightMapMax = 0;
            heightMap = new int[16, 16];
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int y = 255; y >= 0; y--)
                    {
                        int opacity = GetBlockLightOpacity(x, y, z);
                        if (opacity == 0)
                        {
                            // Небо, осветляем
                            if (isSky) Chunk.SetLightFor(x, y, z, EnumSkyBlock.Sky, 15);
                        }
                        else
                        {
                            // первый блок препятствия сверху
                            if (heightMap[x, z] < y) UpdateHeight(x, y, z);
                            if (isSky)
                            {
                                int light = 15;
                                int y2 = y;
                                BlockBase block = Chunk.GetBlock0(new vec3i(x, y, z));
                                bool isWater = block.IsWater;
                                if (isWater)
                                {
                                    opacity = GetBlockLightOpacity(x, y2, z);
                                }
                                // Запускаем цикл затемнения блоков
                                while (light > 0 && y2 > 0)
                                {
                                    if (!isWater)
                                    {
                                        opacity = GetBlockLightOpacity(x, y2, z);
                                    }
                                    light = light - opacity - 1;
                                    if (light < 0) light = 0;
                                    Chunk.SetLightFor(x, y2, z, EnumSkyBlock.Sky, (byte)light);
                                    opacity = 0;
                                    y2--;
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Создает карту высот для блока с нуля
        /// </summary>
        public void GenerateHeightMap() => GenerateHeight(false);

        /// <summary>
        /// Создает карту высот для блока с нуля и тут же осветляет небесные блоки
        /// </summary>
        public void GenerateHeightMapSky() => GenerateHeight(true);

        /// <summary>
        /// Может ли видеть небо
        /// </summary>
        /// <param name="pos">глобальная координата мира</param>
        public bool IsAgainstSky(BlockPos pos) => pos.Y >= heightMap[pos.X & 15, pos.Z & 15];

        #endregion

        #region Generation

        /// <summary>
        /// Запуск проверки бокового небесного освещения
        /// </summary>
        public void StartRecheckGaps()
        {
            WorkingLight light = new WorkingLight(Chunk);
            RecheckGaps(light);
            light.ModifiedRender();
        }

        /// <summary>
        /// Поиск по столбцам, где надо пробежаться с освещением
        /// </summary>
        protected void RecheckGaps(WorkingLight light)
        {
            int posX = Chunk.X << 4;
            int posZ = Chunk.Z << 4;
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    // Определяем наивысшый непрозрачный блок текущего ряда
                    int yh = GetHeight(x, z);
                    int xReal = posX + x;
                    int zReal = posZ + z;

                    // Минимальная высота для соседнего блока
                    int[] yMin = new int[] { 256, 256, 256, 256 };
                    // Максимальная высота для соседнего блока
                    int[] yMax = new int[] { 0, 0, 0, 0 };
                    for (int i = 0; i < 4; i++) // цикл сторон блока по горизонту
                    {
                        // Позиция соседнего блока, глобальные координаты
                        vec3i pos = EnumFacing.DirectionHorizontalVec(i) + new vec3i(xReal, 0, zReal);
                        // Позиция соседнего блока, координаты чанка
                        vec3i pos0 = new vec3i(pos.x & 15, 0, pos.z & 15);
                        ChunkBase chunk = World.GetChunk(pos.x >> 4, pos.z >> 4);
                        // Определяем наивысшый непрозрачный блок
                        int yh2 = chunk.Light.GetHeight(pos0.x, pos0.z);
                        // Если соседний блок выше, начинаем обработку
                        if (yh < yh2)
                        {
                            // Координата Y от которой анализируем, на блок выше вверхней, так-как блок нам не интересен
                            int yDown = yh + 1;
                            // Координата Y до которой анализируем, на блок ниже, так-как нам надо найти ущелие, а блок является перекрытием
                            int yUp = yh2 - 1;
                            // Если нижняя координата ниже вверхней или равны, начинаем анализ
                            if (yDown <= yUp)
                            {
                                for (int y = yDown; y <= yUp; y++) // цикл высоты
                                {
                                    pos0.y = y;
                                    BlockBase block = chunk.GetBlock0(pos0);
                                    if (!Blocks.IsNotTransparent(block.EBlock))
                                    {
                                        // Если блок прозрачный меняем высоты
                                        if (yMin[i] > y) yMin[i] = y;
                                        if (yMax[i] < y) yMax[i] = y;
                                    }
                                }
                            }
                        }
                    }
                    // Готовимся проверять высоты для изменения
                    int yMin2 = 256;
                    int yMax2 = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        if (yMin[i] < yMin2) yMin2 = yMin[i];
                        if (yMax[i] > yMax2) yMax2 = yMax[i];
                    }
                    // Если перепад высот имеется, запускаем правку небесного освещения ввиде столба
                    if (yMin2 != 256)
                    {
                        light.CheckLightFor(xReal, yMin2, yMax2, zReal);
                        // Тест столба для визуализации ввиде стекла
                        //for (int y = yMin2; y <= yMax2; y++) Chunk.SetBlockState(x, y, z, EnumBlock.Glass);
                    }
                }
            }
        }

        #endregion
    }
}

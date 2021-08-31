using VoxelEngine.Util;
using VoxelEngine.World;

namespace VoxelEngine
{
    /// <summary>
    /// Структура кубика, хранит тип блока, параметр его и данные освещения
    /// </summary>
    public struct Voxel
    {
        /// <summary>
        /// Данные блока
        /// 12 bit Id блока и 4 bit параметр блока
        /// </summary>
        private ushort data;
        /// <summary>
        /// Освещение блока
        /// 4 bit свет блока и 4 bit свет от неба
        /// </summary>
        private byte light;
        /// <summary>
        /// Пометка что объект не пустой
        /// </summary>
        private bool full;

        /// <summary>
        /// Пустой ли объект
        /// </summary>
        public bool IsEmpty => !full;

        /// <summary>
        /// Получить двух байтных блок данных
        /// Где 12 bit Id блока и 4 bit параметр блока
        /// </summary>
        public ushort GetVoxelData()
        {
            return data;
        }

        /// <summary>
        /// Задать двух байтный блок данных
        /// Где 12 bit Id блока и 4 bit параметр блока
        /// </summary>
        /// <param name="value"></param>
        public void SetVoxelData(ushort value)
        {
            data = value;
            full = true;
        }

        /// <summary>
        /// Получить тип блок
        /// </summary>
        public EnumBlock GetEBlock()
        {
            return (EnumBlock)(data & 0xFFF);
        }

        /// <summary>
        /// Задать тип блок
        /// </summary>
        public void SetEBlock(EnumBlock eBlock)
        {
            byte p = (byte)(data >> 12);
            ushort b = (ushort)eBlock;
            data = (ushort)(p << 12 | b);
            full = true;
        }

        /// <summary>
        /// Получить дополнительный параметр блока в 4 бита
        /// </summary>
        public byte GetParam4bit()
        {
            return (byte)(data >> 12);
        }

        /// <summary>
        /// Задать дополнительный параметр блока в 4 бита
        /// </summary>
        public void SetParam4bit(byte value)
        {
            ushort b = (ushort)(data & 0xFFF);
            byte p = (byte)(value & 0xF);
            data = (ushort)(p << 12 | b);
            full = true;
        }

        /// <summary>
        /// Получить яркость блока
        /// </summary>
        public byte GetLightFor(EnumSkyBlock type)
        {
            return type == EnumSkyBlock.Sky
                ? (byte)(light & 0xF)
                : (byte)((light & 0xF0) >> 4);
        }

        /// <summary>
        /// Задать яркость блока
        /// </summary>
        public void SetLightFor(EnumSkyBlock type, byte leght)
        {
            byte s = (byte)(light & 0xF);
            byte b = (byte)((light & 0xF0) >> 4);

            if (type == EnumSkyBlock.Sky) s = leght;
            else b = leght;

            light = (byte)(b << 4 | s);
        }


        /// <summary>
        /// Получить байт освещения неба и блока
        /// </summary>
        public byte GetLightsFor()
        {
            return light;
        }

        /// <summary>
        /// Задать байт  освещения неба и блока
        /// </summary>
        public void SetLightsFor(byte light)
        {
            this.light = light;
        }

        public override string ToString()
        {
            return GetEBlock().ToString() + "-" + GetParam4bit().ToString();
        }
    }
}

using VoxelEngine.Util;

namespace VoxelEngine
{
    public struct Voxel
    {
        public short Id; // TODO:: надо 2 byte, где 12 bit Id блока и 4 bit параметр блока
        //public bool LP;
        //public bool LP1;
        //public bool LP2;
        //public bool LP3;
        //public bool LP4;
        //public bool LP5;
        //public bool LP6;
        //public bool LP7;
        // public uint LP01;
        //public byte LP2;
        public short BB; // TODO:: надо byte, гдe 4 bit свет блока и 4 bit свет от неба


        //public bool Attend;

        //public short OpacityAnother;

        public bool IsEmpty()
        {
            //return !Attend;
            return (byte)(Id & 0xFF) == 0;
        }

        /// <summary>
        /// Id длока
        /// </summary>
        /// <returns></returns>
        public byte GetId()
        {
            return (byte)(Id >> 8);
        }

        /// <summary>
        /// Задать id блока
        /// </summary>
        public void SetIdByte(byte id)
        {
            //byte b = (byte)(Id & 0xFF);
            //Attend = true;
            //Id = (short)(id << 8 | b);
            Id = (short)(id << 8 | 1);
        }

        /// <summary>
        /// Получить яркость блока
        /// </summary>
        public byte GetLightFor(EnumSkyBlock type)
        {
            return type == EnumSkyBlock.Sky
                ? (byte)((BB & 0xF))
                : (byte)((BB & 0xF0) >> 4);
        }

        /// <summary>
        /// Получить байт освещения неба и блока
        /// </summary>
        public byte GetLightsFor()
        {
            return (byte)(BB & 0xFF);
        }

        /// <summary>
        /// Задать байт  освещения неба и блока
        /// </summary>
        public void SetLightsFor(byte light)
        {
            byte o = (byte)((BB & 0xF00) >> 8);
            byte n = (byte)(BB >> 12);
            BB = (short)(n << 12 | o << 8 | light);
        }

        /// <summary>
        /// Задать яркость блока
        /// </summary>
        public void SetLightFor(EnumSkyBlock type, byte leght)
        {
            byte s = (byte)(BB & 0xF);
            byte b = (byte)((BB & 0xF0) >> 4);
            byte o = (byte)((BB & 0xF00) >> 8);
            byte n = (byte)(BB >> 12);

            if (type == EnumSkyBlock.Sky) s = leght;
            else b = leght;

            BB = (short)(n << 12 | o << 8 | b << 4 | s);
        }

        /// <summary>
        /// Получить прозрачность блока, 0 прозрачный, 15 не прозрачный полностью
        /// </summary>
        //public byte GetBlockLightOpacity()
        //{
        //    return (byte)((BB & 0xF00) >> 8);
        //}

        ///// <summary>
        ///// Блок не прозрачный
        ///// </summary>
        ///// <returns></returns>
        //public bool IsNotTransparent()
        //{
        //    return GetBlockLightOpacity() > 13;
        //}

        ///// <summary>
        ///// Задать прозрачность блока, 0 не прозрачный, 15 прозрачный полностью
        ///// </summary>
        //public void SetBlockLightOpacity(byte leght)
        //{
        //    byte s = (byte)(BB & 0xF);
        //    byte b = (byte)((BB & 0xF0) >> 4);
        //    byte o = leght;// (byte)((BB & 0xF00) >> 8);
        //    byte n = (byte)(BB >> 12);

        //    BB = (short)(n << 12 | o << 8 | b << 4 | s);
        //}


        /// <summary>
        /// Задать дополнительный параметр блока в 4 бита
        /// </summary>
        public void SetParam4bit(byte value)
        {
            byte s = (byte)(BB & 0xF);
            byte b = (byte)((BB & 0xF0) >> 4);
            byte o = (byte)((BB & 0xF00) >> 8);
            byte n = value;// (byte)(BB >> 12);

            BB = (short)(n << 12 | o << 8 | b << 4 | s);
        }

        /// <summary>
        /// Получить дополнительный параметр блока в 4 бита
        /// </summary>
        public byte GetParam4bit()
        {
            return (byte)(BB >> 12);
        }


        public override string ToString()
        {
            return GetId().ToString() + "-" + GetParam4bit().ToString();
        }
    }
    /// <summary>
    /// Объект вокселя
    /// </summary>
    //public class Voxel
    //{
    //    /// <summary>
    //    /// FF00
    //    /// </summary>
    //    public byte Id { get; set; } = 0;
    //    /// <summary>
    //    /// 00F0
    //    /// </summary>
    //    public byte B1 { get; set; } = 0;
    //    /// <summary>
    //    /// 000F
    //    /// </summary>
    //    public byte B2 { get; set; } = 0;

    //    /// <summary>
    //    /// Сгенерировать данные вокселя из 2 байт (short)
    //    /// </summary>
    //    public Voxel(short key)
    //    {
    //        // FF00 8 bit
    //        Id = (byte)(key >> 8);
    //        // 00F0 4 bit
    //        B1 = (byte)((key & 0xF0) >> 4);
    //        // 000F 4 bit
    //        B2 = (byte)(key & 0xF); 
    //    }

    //    /// <summary>
    //    /// Получить данные вокснеля в 2 байта (short)
    //    /// </summary>
    //    public short GetKey()
    //    {
    //        return (short)(Id << 8 | B1 << 4 | B2);
    //    }
    //}
}

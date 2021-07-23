using VoxelEngine.Glm;
using SharpGL;
using System;
using System.IO;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Одиночный объект отладки
    /// </summary>
    public class Debag
    {
        #region Instance

        private static Debag instance;
        private Debag() { }

        /// <summary>
        /// Передать по ссылке объект если он создан, иначе создать
        /// </summary>
        /// <returns>объект Debag</returns>
        public static Debag GetInstance()
        {
            if (instance == null) instance = new Debag();
            return instance;
        }

        #endregion

        /// <summary>
        /// Объект мира которы берёт из объекта ThreadWorld
        /// </summary>
        public WorldRender World { get; set; }

        protected long _dtime = DateTime.Now.Ticks;
        protected TimeSpan _stime;// = new TimeSpan(0);
        protected TimeSpan _stimeLoad;// = new TimeSpan(0);
        protected bool _isbegin = true;
        protected bool _isbeginLoad = true;

        /// <summary>
        /// Запуск таймера
        /// </summary>
        public void StartTime()
        {
            _dtime = DateTime.Now.Ticks;
            _isbegin = true;
            _isbeginLoad = true;
        }

        /// <summary>
        /// Стоп таймер
        /// </summary>
        public void EndTime()
        {
            if (_isbegin)
            {
                long dtime = DateTime.Now.Ticks;
                long l = dtime - _dtime;
                _stime = new TimeSpan(l);
                _isbegin = false;
            }
        }
        /// <summary>
        /// Стоп таймер загрузки
        /// </summary>
        public void EndTimeLoad()
        {
            if (_isbeginLoad)
            {
                long dtime = DateTime.Now.Ticks;
                long l = dtime - _dtime;
                _stimeLoad = new TimeSpan(l);
                _isbeginLoad = false;
            }
        }

        public static void Log(string fileName, string logMessage, params object[] args)
        {

            using (StreamWriter w = File.AppendText(fileName + ".txt"))
            {
                w.Write($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToShortDateString()}");
                w.WriteLine(" :" + string.Format(logMessage, args));
            }

        }

        /// <summary>
        /// Кадры в секунду
        /// </summary>
        public int Fps { get; set; } = 0;
        /// <summary>
        /// Тики в секунду (20)
        /// </summary>
        public int Tps { get; set; } = 0;
        /// <summary>
        /// Скорость кадра 
        /// </summary>
        public float SpeedFrame { get; set; } = 0;
        /// <summary>
        /// Количество кадров за секунду
        /// </summary>
        public long CountFrame { get; set; } = 0;



        /// <summary>
        /// Вернуть строку TPS FPS
        /// </summary>
        public string ToStringTpsFps()
        {
            //SpeedFrame = Fps == 0 ? 0 : (float)CountFrame / ((float)Fps * 1000f);
            //CountFrame = 0;
            return string.Format("Speed: {1} tps {0} fps {2:0.00} mc", Fps, Tps, SpeedFrame);
        }

        public string ToStringPosCam()
        {
            Camera cam = OpenGLF.GetInstance().Cam;
            vec3i block = cam.ToPositionBlock();
            return string.Format("XYZ: {0} Block: {1}", cam.Position, block);
        }

        public string ToStringRegionChunk()
        {
            Camera cam = OpenGLF.GetInstance().Cam;
            vec2i chunk = cam.ToPositionChunk();
            vec2i region = cam.ToPositionRegion();
            return string.Format("Chunk: {0} {1} Region: {2} {3}",
                chunk.x, chunk.y, region.x, region.y);
        }


        public string ToStringFrontCam()
        {
            return string.Format("CamFront XYZ: {0} Angle: ({1:0.00} | {2:0.00}) {3}",
                OpenGLF.GetInstance().Cam.Front,
                OpenGLF.GetInstance().Cam.AngleYaw(),
                OpenGLF.GetInstance().Cam.AnglePitch(),
                EnumFacing.FromAngle(OpenGLF.GetInstance().Cam.AngleYaw())
                );
        }

        public string ToStringRightCam()
        {
            return string.Format("CamRight XYZ: {0}", OpenGLF.GetInstance().Cam.Right);
        }

        public string ToStringInfo()
        {
            OpenGL gl = OpenGLF.GetInstance().gl;
            return string.Format("OpenGL version {0}",//\r\nVendor: {1}\r\nRenderer: {2}",
                gl.GetString(OpenGL.GL_VERSION),
                gl.GetString(OpenGL.GL_VENDOR),
                gl.GetString(OpenGL.GL_RENDERER)
                );
        }

        /// <summary>
        /// Куда смотрит курсор
        /// </summary>
        //public vec3i RayCast { get; set; }// = new vec3i(0);
        public Block RayCastBlock { get; set; }
        /// <summary>
        /// Куда смотрит курсор
        /// </summary>
        public string ToStringRayCast()
        {
            if (RayCastBlock == null || RayCastBlock.Id < 1) return "Куда смотрит курсор XYZ: -";
            //Block b = World.GetBlock(RayCast);
            return string.Format("Куда смотрит курсор XYZ: {0} [{1:0.00}] b: {2}", 
                RayCastBlock.Position, RayCastBlock.Voxel.GetLightFor(EnumSkyBlock.Block),
                RayCastBlock.Voxel.ToString()
                );
        }

        public string ToStringCountsMeshs()
        {
            return string.Format("Количество мешей, All: {0}  M: {1}  L: {2}", CountMesh, CountMeshChunk, CountMeshLine);
        }

        public string LoadingChunk = "";

        public string ToStringCunckCache()
        {
            return "Количество чанков в потоках: " + LoadingChunk;
            //WorldCache wc = WorldCache.GetInstance();
            //return string.Format("Количество чанков в кэше: {0} / {1} ",
            //    wc.Count,
            //    wc.CountMax
            //);
        }

        public long TickCount { get; set; } = 0;


        public string ToStringTime()
        {
            int h = Mth.Floor(TickCount / 72000f);
            int m = Mth.Floor((TickCount - h * 72000f) / 1200f);
            int s = (int)(TickCount - h * 72000f - m * 1200f);
            return string.Format("Время {0}:{1}:{2:0}", h, m, s / 20f);
            //return string.Format("Время {0:0.00}", TickCount / 20f);
        }

        /// <summary>
        /// Выводить ли на экран
        /// </summary>
        public bool IsDraw { get; set; } = true;
        /// <summary>
        /// Выводить ли на экран колизию
        /// </summary>
        public bool IsDrawCollisium { get; set; } = false;
        /// <summary>
        /// Выводить ли на экран чанк
        /// </summary>
        public bool IsDrawChunk { get; set; } = false;

        /// <summary>
        /// Количество мешей
        /// </summary>
        public long CountMesh { get; set; } = 0;
        /// <summary>
        /// Количество мешей в чанках
        /// </summary>
        public long CountMeshChunk { get; set; } = 0;
        /// <summary>
        /// Количество мешей в чанках
        /// </summary>
        public int CountPoligonChunk { get; set; } = 0;
        /// <summary>
        /// Количество отрендереных линий
        /// </summary>
        public int CountMeshLine { get; set; } = 0;

        /// <summary>
        /// Количество отрендереных чанков
        /// </summary>
        public int RenderChunk { get; set; } = 0;
        /// <summary>
        /// Количество кэш чанков
        /// </summary>
        public int CacheChunk { get; set; } = 0;
        /// <summary>
        /// Количество кэш regionFile
        /// </summary>
        public int CacheRegion { get; set; } = 0;
        /// <summary>
        /// Количество памяти кэш regionFile
        /// </summary>
        public int CacheRegionMem { get; set; } = 0;


        /// <summary>
        /// Жидкие задачи в чанке
        /// </summary>
        public int ChunkLiquidTicks { get; set; } = 0;

        /// <summary>
        /// Объект 
        /// </summary>
        protected TextRender textRender;

        /// <summary>
        /// Номер блока в руке
        /// </summary>
        public int NumberBlock { get; set; } = 1;

        public string ToStringMem()
        {
            float memRegion = CacheRegionMem / 1048576f;
            float memChunk = CacheChunk * 0.2500762f;// ((16 * 16 * 256 + 20) * 4 / 1024) 20 хз может от того что массив [,,]
            float memMesh = CountPoligonChunk * 0.0001171875f;  // 0.00010299 (3 + 2 + 4) * 12 / 1024

            return string.Format("Память: R {0:0.00} + C {1:0.00} + M {2:0.00} = {3:0.00} Mb",
                memRegion, memChunk, memMesh,
                memRegion + memChunk + memMesh
                );
        }

        public void RenderDebug()
        {
            if (IsDraw)
            {
                // CountMesh++;
                string s = ToStringTpsFps() + "\r\n"
                    + ToStringPosCam() + "\r\n"
                    + ToStringFrontCam() + "\r\n"
                    + ToStringRayCast() + "\r\n"
                    //+ ToStringRightCam() + "\r\n"
                    + ToStringRegionChunk() + "\r\n"
                    //+ "Чанк " + OpenGLF.GetInstance().Cam.ToPositionChunk() + "\r\n"
                    + ToStringCountsMeshs() + "\r\n"
                    + "Количество отрендереных чанков: " + RenderChunk.ToString() + "\r\n"
                    + "Количество кэш чанков: " + CacheChunk.ToString() + "\r\n"
                    + "Количество кэш регионов: " + CacheRegion.ToString() + "\r\n"
                    + "Количество полигонов в чанках: " + CountPoligonChunk.ToString() + "\r\n"
                    + "Режим перемещения: " + VEC.GetInstance().Moving.ToString() + "" 
                        + (Keyboard.GetInstance().PlCamera.IsSpeed ? " - Speed" : "") 
                        + (OpenGLF.GetInstance().Cam.IsSneaking ? " - Sneaking" : "") + "\r\n"
                    + ToStringCunckCache() + "\r\n\r\n"
                    + ToStringTime() + "\r\n"
                    + "Жидкести tick чанк: " + ChunkLiquidTicks + "\r\n"
                    + "Время загрузки первых чанков: " + _stime.TotalSeconds.ToString() + "  загрузки: " + _stimeLoad.TotalSeconds.ToString() + "\r\n"
                    + ToStringMem() + "\r\n"
                    + Keyboard.GetInstance().PlCamera.jvh() + "\r\n"
                    + BB + " " + CountTest + " " + CountTest2 + "\r\n"
                    //+ Keyboard.GetInstance().PlCamera._time + "\r\n\r\n"
                    + "В руке: " + NumberBlock;
                    //+ ToStringInfo();

                if (textRender == null)
                {
                    textRender = new TextRender(2, 3, s, new vec4(1f, 1f, .8f, 0.8f));
                }
                else
                {
                    textRender.Render(s);
                }
            }
        }

        public string BB { get; set; } = "";
        public int CountTest { get; set; } = 0;
        public int CountTest2 { get; set; } = 0;

        public void DrawDebug()
        {
            if (IsDraw && textRender != null) textRender.Draw();
        }

    }
}

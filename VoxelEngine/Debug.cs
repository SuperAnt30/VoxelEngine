using VoxelEngine.Glm;
using SharpGL;
using System;
using System.IO;
using VoxelEngine.Util;
using VoxelEngine.World;
using VoxelEngine.Actions;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Chk;
using VoxelEngine.Graphics.Font;
using VoxelEngine.Graphics;
using System.Windows.Forms;

namespace VoxelEngine
{
    /// <summary>
    /// Одиночный объект отладки
    /// </summary>
    public class Debug : WorldHeirSet
    {
        #region Instance

        private static Debug instance;
        private Debug()
        {
            Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Version = string.Format("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
            Revision = ver.Revision.ToString();
        }

        /// <summary>
        /// Передать по ссылке объект если он создан, иначе создать
        /// </summary>
        /// <returns>объект Debag</returns>
        public static Debug GetInstance()
        {
            if (instance == null) instance = new Debug();
            return instance;
        }

        #endregion

        public static void Log(string fileName, string logMessage, params object[] args)
        {

            using (StreamWriter w = File.AppendText(fileName + ".txt"))
            {
                w.Write($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToShortDateString()}");
                w.WriteLine(" :" + string.Format(logMessage, args));
            }

        }

        /// <summary>
        /// Версия продукта
        /// </summary>
        public string Version { get; protected set; }
        /// <summary>
        /// Версия ребилдинга
        /// </summary>
        public string Revision { get; protected set; }
        /// <summary>
        /// Кадры в секунду
        /// </summary>
        public int Fps { get; set; } = 0;
        /// <summary>
        /// Тики в секунду (20)
        /// </summary>
        public int Tps { get; set; } = 0;
        /// <summary>
        /// Рендер чанков в секунду
        /// </summary>
        public int Rc { get; set; } = 0;
        /// <summary>
        /// Рендер чанков альфа в секунду
        /// </summary>
        public int Rca { get; set; } = 0;
        /// <summary>
        /// Скорость кадра 
        /// </summary>
        public float SpeedFrame { get; set; } = 0;
        /// <summary>
        /// Количество кадров за секунду
        /// </summary>
        public long CountFrame { get; set; } = 0;
        /// <summary>
        /// Куда смотрит курсор
        /// </summary>
        public Block RayCastBlock { get; set; }
        /// <summary>
        /// Куда смотрит курсор блок сверху
        /// </summary>
        public Block RayCastBlockUp { get; set; }

        //public long TickCount { get; set; } = 0;

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
        /// Количество альфа блоков в текущем чанке
        /// </summary>
        public int ChunkAlpheBlock { get; set; } = 0;
        /// <summary>
        /// Количество мобов
        /// </summary>
        public int Entities { get; set; } = 0;
        /// <summary>
        /// Количество мешей мобов
        /// </summary>
        public long CountMeshEntities { get; set; } = 0;

        /// <summary>
        /// Объект 
        /// </summary>
        protected TextRender textRender;

        /// <summary>
        /// Тип блока в руке
        /// </summary>
        public EnumBlock NumberBlock { get; set; } = EnumBlock.Stone;

        protected string ToStringInfo()
        {
            OpenGL gl = OpenGLF.GetInstance().gl;
            return string.Format("OpenGL version {0}",//\r\nVendor: {1}\r\nRenderer: {2}",
                gl.GetString(OpenGL.GL_VERSION),
                gl.GetString(OpenGL.GL_VENDOR),
                gl.GetString(OpenGL.GL_RENDERER)
                );
        }

        protected string ToStringTime()
        {
            long tickCount = VEC.GetInstance().TickCount;
            int h = Mth.Floor(tickCount / 72000f);
            int m = Mth.Floor((tickCount - h * 72000f) / 1200f);
            int s = (int)(tickCount - h * 72000f - m * 1200f);
            return string.Format("Time {0}:{1}:{2:0}", h, m, s / 20f);
            //return string.Format("Время {0:0.00}", TickCount / 20f);
        }

        protected string ToStringMem()
        {
            float memRegion = CacheRegionMem / 1048576f;
            float memChunk = CacheChunk * 0.2500762f;// ((16 * 16 * 256 + 20) * 4 / 1024) 20 хз может от того что массив [,,]
            float memMesh = CountPoligonChunk * 0.0001171875f;  // 0.00010299 (3 + 2 + 4) * 12 / 1024

            return string.Format("Mem: R {0:0.00} + C {1:0.00} + M {2:0.00} = {3:0.00} Mb",
                memRegion, memChunk, memMesh,
                memRegion + memChunk + memMesh
                );
        }

        protected string ToRayCast(Block block)
        {
            return block == null ? "" : string.Format("{0} light SB: {1}-{2} b: {3}",
                block.Position,
                block.Voxel.GetLightFor(EnumSkyBlock.Sky),
                block.Voxel.GetLightFor(EnumSkyBlock.Block),
                block.Voxel.ToString()
                );
        }

        protected string ToStringAll()
        {
            Camera cam = OpenGLF.GetInstance().Cam;
            
            string strHeader = string.Format(
                "Voxel Engine v{3}\r\nRevision {5}\r\n{4}\r\n{0} tps {1} fps {2:0.00} mc",
                Tps, Fps, SpeedFrame, Version, ToStringTime(), Revision
            );

            vec2i posChunk = cam.ChunkPos;
            vec3i posBlock = cam.BlockPos;
            ChunkBase chunk = World.GetChunk(posChunk.x, posChunk.y);
            
            string strPosition = string.Format(
                "XYZ: {0}\r\n" +
                "Block: {1}\r\n" +
                "Chunk: {2} Region: {3}\r\n" +
                "Angle YP: ({4:0.0} | {5:0.0}) {6}\r\n" +
                "Biome: {7}\r\n" +
                "Move {8}",
                    cam.Position,
                    posBlock,
                    posChunk, cam.ToPositionRegion(),
                    cam.AngleYaw(), cam.AnglePitch(), EnumFacing.FromAngle(cam.AngleYaw()),
                    chunk == null ? "-" : chunk.GetBiome(posBlock.x & 15, posBlock.z & 15).ToString(),
                    Keyboard.GetInstance().PlCamera.StrDebug);

            string strCursor = string.Format(
                "Cursor: {0}\r\n" +
                "CursUp: {1}",
                ToRayCast(RayCastBlock),
                ToRayCast(RayCastBlockUp)
            );

            string strChunck = string.Format(
                "Chunk LT: {0} Ah: {1} Rc: {2} Rca: {3}",
                ChunkLiquidTicks,
                ChunkAlpheBlock,
                Rc, Rca
            );

            string strMeshMem = string.Format(
                "Mesh All: {0} M: {1} L: {2}\r\n" +
                "Cache Ch: {3} Rg: {4}\r\n" +
                "PoligonsCh: {5}",
                CountMesh, CountMeshChunk, CountMeshLine,
                CacheChunk, CacheRegion,
                CountPoligonChunk,
                ToStringMem());

            string strEntity = string.Format(
                "Entities: {0} M: {1}\r\nSound: {2}",
                Entities, CountMeshEntities, World.Audio.StrDebug);

            string strAnother = string.Format(
                "В руке: {0}\r\nBB: {1} CT: {2:0.00} CT2: {3}",
                NumberBlock, BB, CountTest, CountTest2
            );

            return strHeader + "\r\n\r\n" 
                + strMeshMem + "\r\n"
                + strChunck + "\r\n"
                + strEntity + "\r\n\r\n" 
                + strPosition + "\r\n" 
                + strCursor + "\r\n"
                + strAnother;
        }

        public void RenderDebug()
        {
            if (IsDraw)
            {
                if (textRender == null)
                {
                    textRender = new TextRender(2, 3, ToStringAll(), new vec4(1f, 1f, .8f, 0.8f));
                }
                else
                {
                    textRender.Render(ToStringAll());
                }
            }
        }

        public string BB { get; set; } = "";
        public float CountTest { get; set; } = 0;
        public int CountTest2 { get; set; } = 0;

        public void DrawDebug()
        {
            if (IsDraw && textRender != null) textRender.Draw();
        }

    }
}

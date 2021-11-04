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

namespace VoxelEngine
{
    /// <summary>
    /// Одиночный объект отладки
    /// </summary>
    public class Debug
    {
        #region Свойства

        /// <summary>
        /// Объект кэш чанка
        /// </summary>
        public static WorldBase World { get; protected set; }
        /// <summary>
        /// Версия продукта
        /// </summary>
        public static string Version { get; protected set; }
        /// <summary>
        /// Версия ребилдинга
        /// </summary>
        public static string Revision { get; protected set; }
        /// <summary>
        /// Кадры в секунду
        /// </summary>
        public static int Fps { get; set; } = 0;
        /// <summary>
        /// Тики в секунду (20)
        /// </summary>
        public static int Tps { get; set; } = 0;
        /// <summary>
        /// Загрузка чанков в секунду
        /// </summary>
        public static int Lc { get; set; } = 0;
        /// <summary>
        /// Рендер чанков в секунду
        /// </summary>
        public static int Rc { get; set; } = 0;
        /// <summary>
        /// Рендер чанков альфа в секунду
        /// </summary>
        public static int Rca { get; set; } = 0;
        /// <summary>
        /// Скорость кадра 
        /// </summary>
        public static float SpeedFrame { get; set; } = 0;
        /// <summary>
        /// Количество кадров за секунду
        /// </summary>
        public static long CountFrame { get; set; } = 0;
        /// <summary>
        /// Куда смотрит курсор
        /// </summary>
        public static MovingObjectPosition RayCastObject { get; set; }
        /// <summary>
        /// Куда смотрит курсор блок сверху
        /// </summary>
        public static BlockBase RayCastBlockUp { get; set; }
        /// <summary>
        /// Выводить ли на экран
        /// </summary>
        public static bool IsDraw { get; set; } = true;
        /// <summary>
        /// Выводить ли на экран колизию
        /// </summary>
        public static bool IsDrawCollisium { get; set; } = false;
        /// <summary>
        /// Выводить ли на экран чанк
        /// </summary>
        public static bool IsDrawChunk { get; set; } = false;
        /// <summary>
        /// Количество мешей
        /// </summary>
        public static long CountMesh { get; set; } = 0;
        /// <summary>
        /// Количество мешей в чанках
        /// </summary>
        public static long CountMeshChunk { get; set; } = 0;
        /// <summary>
        /// Количество мешей в чанках
        /// </summary>
        public static int CountPoligonChunk { get; set; } = 0;
        /// <summary>
        /// Количество отрендереных линий
        /// </summary>
        public static int CountMeshLine { get; set; } = 0;
        /// <summary>
        /// Количество отрендереных чанков
        /// </summary>
        public static int RenderChunk { get; set; } = 0;
        /// <summary>
        /// Количество кэш чанков
        /// </summary>
        public static int CacheChunk { get; set; } = 0;
        /// <summary>
        /// Количество кэш regionFile
        /// </summary>
        public static int CacheRegion { get; set; } = 0;
        /// <summary>
        /// Количество памяти кэш regionFile
        /// </summary>
        public static int CacheRegionMem { get; set; } = 0;
        /// <summary>
        /// Жидкие задачи в чанке
        /// </summary>
        public static int ChunkLiquidTicks { get; set; } = 0;
        /// <summary>
        /// Количество альфа блоков в текущем чанке
        /// </summary>
        public static int ChunkAlpheBlock { get; set; } = 0;
        /// <summary>
        /// Количество мобов
        /// </summary>
        public static int Entities { get; set; } = 0;
        /// <summary>
        /// Количество мешей мобов
        /// </summary>
        public static long CountMeshEntities { get; set; } = 0;
        /// <summary>
        /// Количество блоков изменённых освещения при операции
        /// </summary>
        public static int CountSetBlockLight { get; set; } = 0;
        /// <summary>
        /// Милисекунд затраченых на изменённых освещения при операции
        /// </summary>
        public static long TimeSetBlockLight { get; set; } = 0;
        /// <summary>
        /// Тип блока в руке
        /// </summary>
        public static EnumBlock NumberBlock { get; set; } = EnumBlock.Stone;

        public static string BB { get; set; } = "";
        public static float CountTest { get; set; } = 0;
        public static int CountTest2 { get; set; } = 0;

        #endregion

        /// <summary>
        /// Объект 
        /// </summary>
        protected static TextRender textRender;

        public static void Log(string fileName, string logMessage, params object[] args)
        {
            using (StreamWriter w = File.AppendText(fileName + ".txt"))
            {
                w.Write($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToShortDateString()}");
                w.WriteLine(" :" + string.Format(logMessage, args));
            }
        }

        /// <summary>
        /// Задать объект мира
        /// </summary>
        public static void Instance(WorldBase world)
        {
            World = world;
            Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Version = string.Format("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
            Revision = ver.Revision.ToString();
        }

        public static void RenderDebug()
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

        public static void DrawDebug()
        {
            if (IsDraw && textRender != null) textRender.Draw();
        }

        #region Methods protected

        protected static string ToStringInfo()
        {
            OpenGL gl = OpenGLF.GetInstance().gl;
            return string.Format("OpenGL version {0}",//\r\nVendor: {1}\r\nRenderer: {2}",
                gl.GetString(OpenGL.GL_VERSION),
                gl.GetString(OpenGL.GL_VENDOR),
                gl.GetString(OpenGL.GL_RENDERER)
            );
        }

        protected static string ToStringTime()
        {
            long tickCount = VEC.TickCount;
            int h = Mth.Floor(tickCount / 72000f);
            int m = Mth.Floor((tickCount - h * 72000f) / 1200f);
            int s = (int)(tickCount - h * 72000f - m * 1200f);
            return string.Format("Time {0}:{1}:{2:0}", h, m, s / 20f);
        }

        protected static string ToStringMem()
        {
            float memRegion = CacheRegionMem / 1048576f;
            float memChunk = CacheChunk * 0.2500762f;// ((16 * 16 * 256 + 20) * 4 / 1024) 20 хз может от того что массив [,,]
            float memMesh = CountPoligonChunk * 0.0001171875f;  // 0.00010299 (3 + 2 + 4) * 12 / 1024

            return string.Format("Mem: R {0:0.00} + C {1:0.00} + M {2:0.00} = {3:0.00} Mb",
                memRegion, memChunk, memMesh,
                memRegion + memChunk + memMesh
                );
        }

        protected static string ToRayCast(MovingObjectPosition moving)
        {
            if (moving != null)
            {
                if (moving.IsBlock() && moving.Block != null)
                {
                    return string.Format("{0} light SB: {1}-{2} b: {3}",
                        moving.Block.Position,
                        moving.Block.Voxel.GetLightFor(EnumSkyBlock.Sky),
                        moving.Block.Voxel.GetLightFor(EnumSkyBlock.Block),
                        moving.Block.Voxel.ToString()
                    );
                }
                if (moving.IsEntity())
                {
                    return string.Format("{0} {1}", moving.Entity.Key, moving.Entity.HitBox.Position);
                }
            }
            return "";
        }

        protected static string ToStringAll()
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
                    Keyboard.GetInstance().KeyMove.PlCamera.StrDebug);

            string strCursor = string.Format(
                "Cursor: {0}\r\n" +
                "CursUp: {1}",
                ToRayCast(RayCastObject),
                ToRayCast(new MovingObjectPosition(RayCastBlockUp))
            );

            string strChunck = string.Format(
                "Chunk LT: {0} Ah: {1} Lc: {4} Rc: {2} Rca: {3}",
                ChunkLiquidTicks,
                ChunkAlpheBlock,
                Rc, Rca, Lc
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

            string strLight = string.Format(
                "Освещение Time: {0}: Count: {1}", TimeSetBlockLight, CountSetBlockLight 
            );

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
                + strLight + "\r\n"
                + strAnother;
        }

        #endregion
    }
}

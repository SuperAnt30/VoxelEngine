using SharpGL;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Объект одиночка работы с OpneGl
    /// </summary>
    public class OpenGLF
    {
        #region Instance

        private static OpenGLF instance;
        private OpenGLF()
        {
            DistSqrt = _GetSqrt(VE.CHUNK_VISIBILITY);
            DistSqrtAlpha = _GetSqrt(VE.CHUNK_VISIBILITY_ALPHA);
        }

        /// <summary>
        /// Передать по ссылке объект если он создан, иначе создать
        /// </summary>
        /// <returns>объект OpenGLFunctions</returns>
        public static OpenGLF GetInstance()
        {
            if (instance == null) instance = new OpenGLF();
            return instance;
        }

        #endregion

        /// <summary>
        /// Объект OpneGl
        /// </summary>
        public OpenGL gl { get; protected set; }
        /// <summary>
        /// Размер окна
        /// </summary>
        public Camera Cam { get; set; }
        /// <summary>
        /// Прорисовка текстурами или рёбрами
        /// </summary>
        public bool IsLine { get; protected set; } = false;

        /// <summary>
        /// Объект текстур
        /// </summary>
        protected Texture texture = new Texture();
        /// <summary>
        /// Объект шейдоров
        /// </summary>
        public Shaders Sh { get; protected set; } = new Shaders();

        /// <summary>
        /// Объект рендера мира
        /// </summary>
        public WorldMesh WorldM { get; protected set; }

        /// <summary>
        /// Объект рендера skybox
        /// </summary>
        public SkyBoxMesh SkyBoxM { get; protected set; }
        public SkyBoxMesh SkyBoxSun { get; protected set; }


        /// <summary>
        /// Объект рендера линий мира
        /// </summary>
        public WorldLineMesh WorldLineM { get; protected set; } = new WorldLineMesh();
        
        /// <summary>
        /// Объект курсора
        /// </summary>
        public GuiCursor guiCursor = new GuiCursor();
        /// <summary>
        /// Объект под водой
        /// </summary>
        public GuiWater guiWater = new GuiWater();


        /// <summary>
        /// Таймер для фиксации времени
        /// </summary>
        protected Stopwatch stopwatch = new Stopwatch();

        public TextureAnimation textureAnimation;

        public void Initialized(OpenGL openGL)
        {
            stopwatch.Start();

            gl = openGL;
            gl.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);


            //gl.Enable(OpenGL.GL_ALPHA_TEST);
            //gl.AlphaFunc(OpenGL.GL_GREATER, 0.8f);
            //gl.Enable(OpenGL.GL_MULTISAMPLE);

            // Устанавливаем камеру
            //Cam = new Camera(new vec3(24, 7, 24), glm.radians(70.0f));
            //Cam.PositionChunkChanged += Cam_PositionChunkChanged;

            textureAnimation = new TextureAnimation(
                new Bitmap(@"textures\256.png"),
                new Bitmap(@"textures\water_still.png"),
                new Bitmap(@"textures\water_flow.png")
            );
                
            // генерация индексов для текстуры
            texture.Initialize();
            textureAnimation.Render();
            texture.SetTexture("atlas", textureAnimation.AtlasBox);
            // горизонтального смещения шрифта
            FontAdvance.GetInstance();

            Sh.Create(gl);

            WorldM = new WorldMesh();

            SkyBoxM = new SkyBoxMesh(1f);
            SkyBoxSun = new SkyBoxMesh(0.9f);
            // worldM.Render();

            //Thread myThread = new Thread(new ThreadStart(worldR.BeginLoading));
            //myThread.Start();
            //worldR.BeginLoading();
        }

        ///// <summary>
        ///// Событие изменение позиции камеры на другой чанк
        ///// </summary>
        //private void Cam_PositionChunkChanged(object sender, System.EventArgs e)
        //{
        //  //  worldM.Render();
        //    //WorldRender();
        //}

        public void Resized(Size size)
        {
            Cam.SetResized(size.Width, size.Height);
            guiCursor.Render(size);
            guiWater.Render(size);
        }

        

        public void Draw()
        {
            stopwatch.Restart();

            Debag.GetInstance().CountMesh = 0;
            Keyboard.GetInstance().PlCamera.Draw();

            if (IsLine)
            {
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
                gl.Disable(OpenGL.GL_CULL_FACE);
            }
            else
            {
                gl.Enable(OpenGL.GL_CULL_FACE);
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
            }
            
            // Включает Буфер глубины 
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            int t = 24000;
            long tc = Debag.GetInstance().TickCount;
            long it = tc / t;
            float ir = (float)(tc - it * t) / ((float)t / 6.283185f);
            float light = (float)(tc - it * t) / (float)t * 2f;
            if (light > 1f) light = 2f - light;
            light = 1f - light;

            //float ir = 0.5f;
            //float light = 0.8f;

            

            //Debag.GetInstance().BB = string.Format("{0:0.00} - {1:0.00} - {2:0.00}", light, light * 1.0f, light * 0f);

            //SKYBOX
            gl.DepthMask(0);
            Sh.ShSkyBox.Bind(gl);
            Sh.ShSkyBox.SetUniformMatrix4(gl, "projection", Cam.ProjectionLookAt);
            Sh.ShSkyBox.SetUniformMatrix4(gl, "view", Cam.PositionView);
            Sh.ShSkyBox.SetUniform1(gl, "light", light);
            //Sh.ShSkyBox.SetUniform1(gl, "light", 1f);

            texture.BindTextureSkyBox();
            SkyBoxM.Draw();

            Sh.ShSkyBox.SetUniformMatrix4(gl, "view",
                (Glm.glm.translate(new Glm.mat4(1.0f), Cam.Position)
                * Glm.glm.rotate(ir, new Glm.vec3(0, 0, 1))).to_array()
            );
            Sh.ShSkyBox.SetUniform1(gl, "light", 1f);

            texture.BindTextureSkyBox2();
            SkyBoxSun.Draw();
            gl.DepthMask(1);

            Sh.ShSkyBox.Unbind(gl);


            // VOXEL
            Sh.ShVoxel.Bind(gl);
            Sh.ShVoxel.SetUniformMatrix4(gl, "projection", Cam.Projection);
            Sh.ShVoxel.SetUniformMatrix4(gl, "lookat", Cam.LookAt);
            Sh.ShVoxel.SetUniform1(gl, "light", light);
            // Рендер мира


            //texture.BindTexture("test128");
            //WorldM.DrawDense(37, -1);
            //texture.BindTexture("test256");
            //WorldM.DrawDense(9, 36);
            texture.BindTexture("atlas");
           // texture.MultiTexture();
            //WorldM.DrawDense(0, -1); //(0, 36);
            WorldM.DrawDenseOld();
            //texture.BindTexture("test256");
            WorldM.DrawAlpha();
            Sh.ShVoxel.Unbind(gl);

            // LINE
            Sh.ShLine.Bind(gl);
            Sh.ShLine.SetUniformMatrix4(gl, "projection", Cam.Projection);
            Sh.ShLine.SetUniformMatrix4(gl, "lookat", Cam.LookAt);
            WorldLineM.Draw();
            Sh.ShLine.Unbind(gl);


            if (IsLine)
            {
                gl.Enable(OpenGL.GL_CULL_FACE);
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
            }


            // DEBUG
            Sh.ShFont.Bind(gl);
            Sh.ShFont.SetUniformMatrix4(gl, "projview", Cam.Ortho2D);


            texture.BindTexture("gui");
            if (Keyboard.GetInstance().PlCamera.IsWater) guiWater.Draw();
            guiCursor.Draw();

            texture.BindTexture(VE.TEXTURE_FONT_KEY);
            Debag.GetInstance().DrawDebug();

            Sh.ShFont.Unbind(gl);

            Debag.GetInstance().CountFrame += stopwatch.ElapsedTicks;
        }


        /// <summary>
        /// Такт 20 в секунду
        /// </summary>
        public void Tick()
        {
            Keyboard.GetInstance().PlCamera.Tick();
            textureAnimation.Render();
            texture.SetTexture("atlas", textureAnimation.AtlasBox);
            Debag.GetInstance().RenderDebug();
        }

        /// <summary>
        /// Включить или выключить прорисовку линией
        /// </summary>
        public void DrawLine()
        {
            IsLine = !IsLine;
        }
        /// <summary>
        /// Массив по длинам используя квадратный корень для всей видимости
        /// </summary>
        public ChunkLoading[] DistSqrt { get; protected set; }
        /// <summary>
        /// Массив по длинам используя квадратный корень для альфа видимости
        /// </summary>
        public ChunkLoading[] DistSqrtAlpha { get; protected set; }

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
    }
}

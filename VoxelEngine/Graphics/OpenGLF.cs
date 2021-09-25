using SharpGL;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using VoxelEngine.Actions;
using VoxelEngine.Glm;
using VoxelEngine.Graphics.Font;
using VoxelEngine.Graphics.Shader;
using VoxelEngine.Renderer;
using VoxelEngine.Util;

namespace VoxelEngine.Graphics
{
    /// <summary>
    /// Объект одиночка работы с OpneGl
    /// </summary>
    public class OpenGLF
    {
        #region Instance

        private static OpenGLF instance;
        private OpenGLF() { }

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
        /// Камера
        /// </summary>
        public Camera Cam { get; set; }
        /// <summary>
        /// Конфиг
        /// </summary>
        public VEC Config { get; set; }
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
        /// Объект под дебага
        /// </summary>
        public GuiDebug guiDebug = new GuiDebug();




        public TextureAnimation textureAnimation;

        /// <summary>
        /// Обзор тумана
        /// </summary>
        protected float lengthFog;

        public void Initialized(OpenGL openGL)
        {
            gl = openGL;
            gl.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_BLEND);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            
            //gl.Enable(OpenGL.GL_ALPHA_TEST);
            //gl.AlphaFunc(OpenGL.GL_GREATER, 0.8f);
            //gl.Enable(OpenGL.GL_MULTISAMPLE);

            textureAnimation = new TextureAnimation(
                new Bitmap(@"textures\256.png"),
                new Bitmap(@"textures\water_still.png"),
                new Bitmap(@"textures\water_flow.png")
            );
                
            // генерация индексов для текстуры
            texture.Initialize();
            // горизонтального смещения шрифта
            FontAdvance.GetInstance();

            Sh.Create(gl);

            WorldM = new WorldMesh();
            WorldM.RemoveChanged += WorldMRemoveChanged;

            SkyBoxM = new SkyBoxMesh(1f);
            SkyBoxSun = new SkyBoxMesh(0.9f);

            lengthFog = VE.CHUNK_VISIBILITY * 16f - 16f;
        }

        public void Resized(Size size)
        {
            Cam.SetResized(size.Width, size.Height);
            guiCursor.Render(size.Width, size.Height);
            guiWater.Render(size);
            guiDebug.Render(size);
        }

        public void Draw(float timeFrame, float timeAll)
        {
            DrawBegin(timeFrame, timeAll);
            DrawSkyBox();
            IsLineOn(); // Для прорисовки сетки
            DrawLine();
            DrawEntity();
            DrawVoxel();
            IsLineOff(); // Для прорисовки сетки
            DrawGui();
            DrawDebug();
        }

        /// <summary>
        /// Такт 20 в секунду
        /// </summary>
        public void Tick()
        {
            // В потоке генерируем атлас
            TickAtlas();

            // Генерация дебага
            Debug.GetInstance().RenderDebug();
        }

        /// <summary>
        /// Атлас в такте
        /// </summary>
        protected async void TickAtlas()
        {
            if (await Task.Run(() => textureAnimation.Render()))
            {
                texture.SetTexture("atlas", textureAnimation.AtlasBox);
            }
        }

        /// <summary>
        /// Включить или выключить прорисовку линией
        /// </summary>
        public void LineOn()
        {
            IsLine = !IsLine;
        }
        /// <summary>
        /// Включаем прорисовку сеткой без заполнения полигона
        /// </summary>
        protected void IsLineOn()
        {
            if (IsLine)
            {
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
                gl.Disable(OpenGL.GL_CULL_FACE);
            }
        }

        /// <summary>
        /// Выключаем прорисовку сеткой без заполнения полигона
        /// </summary>
        protected void IsLineOff()
        {
            if (IsLine)
            {
                gl.Enable(OpenGL.GL_CULL_FACE);
                gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
            }
        }

        /// <summary>
        /// Событие удалена сетка
        /// </summary>
        public event CoordEventHandler RemoveChunkMeshChanged;

        /// <summary>
        /// удалена сетка
        /// </summary>
        protected virtual void OnRemoveChunkMeshChanged(CoordEventArgs e)
        {
            RemoveChunkMeshChanged?.Invoke(this, e);
        }

        private void WorldMRemoveChanged(object sender, CoordEventArgs e)
        {
            OnRemoveChunkMeshChanged(e);
        }

        #region Draw

        /// <summary>
        /// Стартовый покет прорисовки
        /// </summary>
        protected void DrawBegin(float timeFrame, float timeAll)
        {
            Debug.GetInstance().CountMesh = 0;
            
            Keyboard.GetInstance().UpdateFPS(timeFrame, timeAll);

            // Включает Буфер глубины 
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
        }

        /// <summary>
        /// Прорисовка интерфейса 2д
        /// </summary>
        protected void DrawGui()
        {
            Sh.ShFont.Bind(gl);
            Sh.ShFont.SetUniformMatrix4(gl, "projview", Cam.Ortho2D);
            texture.BindTexture("gui");

            // Эффект под водой
            if (Cam.IsEyesWater) guiWater.Draw();
            // Курсор
            guiCursor.Draw();

            //texture.BindTexture("atlas");
            //guiDebug.Draw();

            Sh.ShFont.Unbind(gl);
        }

        /// <summary>
        /// Прорисовка текст дебага 2д
        /// </summary>
        protected void DrawDebug()
        {
            Sh.ShFont.Bind(gl);
            Sh.ShFont.SetUniformMatrix4(gl, "projview", Cam.Ortho2D);
            texture.BindTexture(VE.TEXTURE_FONT_KEY);
            Debug.GetInstance().DrawDebug();
            Sh.ShFont.Unbind(gl);
        }

        /// <summary>
        /// Прорисовка блоков 3д
        /// </summary>
        protected void DrawVoxel()
        {
            Sh.ShVoxel.Bind(gl);
            Sh.ShVoxel.SetUniformMatrix4(gl, "projection", Cam.Projection);
            Sh.ShVoxel.SetUniformMatrix4(gl, "lookat", Cam.LookAt);
            Sh.ShVoxel.SetUniform1(gl, "light", Config.LeghtSky);
            Sh.ShVoxel.SetUniform1(gl, "length", lengthFog);
            texture.BindTexture("atlas");
            WorldM.Draw();
            Sh.ShVoxel.Unbind(gl);
        }

        /// <summary>
        /// Прорисовка сущностей 3д
        /// </summary>
        protected void DrawEntity()
        {
            float leght = Config.LeghtSky + .3f;
            if (leght > 1f) leght = 1f;
            Sh.ShEntity.Bind(gl);
            Sh.ShEntity.SetUniformMatrix4(gl, "projection", Cam.Projection);
            Sh.ShEntity.SetUniformMatrix4(gl, "lookat", Cam.LookAt);
            Sh.ShEntity.SetUniform1(gl, "light", leght);
            //Sh.ShFont.SetUniformMatrix4(gl, "projview", Cam.PositionView);
            //Sh.ShEntity.SetUniform1(gl, "light", Config.LeghtSky);

            texture.BindTexture("chicken");
            //texture.BindTexture("gui");

            WorldM.DrawEntities();
            Sh.ShEntity.Unbind(gl);
        }

        /// <summary>
        /// Прорисовка линий 3д
        /// </summary>
        protected void DrawLine()
        {
            Sh.ShLine.Bind(gl);
            Sh.ShLine.SetUniformMatrix4(gl, "projection", Cam.Projection);
            Sh.ShLine.SetUniformMatrix4(gl, "lookat", Cam.LookAt);
            WorldLineM.Draw();
            Sh.ShLine.Unbind(gl);
        }

        

        /// <summary>
        /// Прорисовка неба
        /// </summary>
        protected void DrawSkyBox()
        {
            gl.DepthMask(0);
            Sh.ShSkyBox.Bind(gl);
            Sh.ShSkyBox.SetUniformMatrix4(gl, "projection", Cam.ProjectionLookAt);
            Sh.ShSkyBox.SetUniformMatrix4(gl, "view", Cam.PositionView);
            Sh.ShSkyBox.SetUniform1(gl, "light", Config.LeghtSky);

            texture.BindTextureSkyBox();
            SkyBoxM.Draw();
            Sh.ShSkyBox.SetUniformMatrix4(gl, "view",
                (glm.translate(new mat4(1.0f), Cam.Position)
                * glm.rotate(Config.AngleSun, new vec3(0, 0, 1))).to_array()
            );
            Sh.ShSkyBox.SetUniform1(gl, "light", 1f);

            texture.BindTextureSkyBox2();
            SkyBoxSun.Draw();
            gl.DepthMask(1);

            Sh.ShSkyBox.Unbind(gl);
        }

        #endregion
    }
}

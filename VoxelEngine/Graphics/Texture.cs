using SharpGL;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace VoxelEngine.Graphics
{
    /// <summary>
    /// Объект одиночка текстуры
    /// </summary>
    public class Texture
    {
        /// <summary>
        /// Массив текстур uint
        /// </summary>
        protected Hashtable hashtable = new Hashtable();

        /// <summary>
        /// Индекст для текстуры 
        /// </summary>
        public void Initialize()
        {
            SetTexture("gui", @"textures\guiicons.png");
            SetTexture("chicken", @"textures\chicken.png");
            // Текстурный атлас 
            // SetTexture("test128", @"textures\128.png");
            //Texture("test512", @"textures\512.png");
            //SetTexture("test256", @"textures\256.png");

            SetTextureSkyBox();
            SetTextureSkyBox2();

            // Текстура шрифта
            SetTexture(VE.TEXTURE_FONT_KEY, VE.TEXTURE_FONT_PATH);
        }

        /// <summary>
        /// Получить индекс текстуры по ключу
        /// </summary>
        /// <param name="key">ключ текстуры</param>
        public uint GetData(string key)
        {
            if (hashtable.ContainsKey(key))
            {
                return (uint)hashtable[key];
            }
            return 0;
        }

        /// <summary>
        /// Запустить текстуру
        /// </summary>
        /// <param name="key">ключ текстуры</param>
        public void BindTexture(string key)
        {
            BindTexture(key, 0);
        }

        /// <summary>
        /// Запустить текстуру
        /// </summary>
        /// <param name="key">ключ текстуры</param>
        /// <param name="texture">OpenGL.GL_TEXTURE0 + texture</param>
        public void BindTexture(string key, uint texture)
        {
            if (hashtable.ContainsKey(key))
            {
                OpenGLF.GetInstance().gl.ActiveTexture(OpenGL.GL_TEXTURE0 + texture);
                OpenGLF.GetInstance().gl.BindTexture(OpenGL.GL_TEXTURE_2D, GetData(key));
            }
        }

        /// <summary>
        /// Запустить текстуру SkyBox
        /// </summary>
        /// <param name="key">ключ текстуры</param>
        public void BindTextureSkyBox()
        {
            if (hashtable.ContainsKey("skybox"))
            {
                OpenGLF.GetInstance().gl.BindTexture(OpenGL.GL_TEXTURE_CUBE_MAP, GetData("skybox"));
            }
        }
        /// <summary>
        /// Запустить текстуру SkyBox
        /// </summary>
        /// <param name="key">ключ текстуры</param>
        public void BindTextureSkyBox2()
        {
            if (hashtable.ContainsKey("skybox2"))
            {
                OpenGLF.GetInstance().gl.BindTexture(OpenGL.GL_TEXTURE_CUBE_MAP, GetData("skybox2"));
            }
        }

        /// <summary>
        /// Внесение в кеш текстур
        /// </summary>
        /// <param name="key">ключ текстуры</param>
        /// <param name="fileName">адрес файла текстуры</param>
        public uint SetTexture(string key, string fileName)
        {
            return SetTexture(key, new Bitmap(fileName));
        }

        /// <summary>
        /// Внесение в кеш текстур
        /// </summary>
        /// <param name="key">ключ текстуры</param>
        /// <param name="bitmap">рисунок</param>
        public uint SetTexture(string key, Bitmap bitmap)
        {
            Bitmap iw = new Bitmap(bitmap);
            
            OpenGL gl = OpenGLF.GetInstance().gl;

            uint[] texture = new uint[1];

            if (hashtable.ContainsKey(key))
            {
                texture[0] = (uint)hashtable[key];
            }
            else
            {
                gl.GenTextures(1, texture);
                hashtable.Add(key, texture[0]);
            }

            

            BitmapData data = iw.LockBits(
                new Rectangle(0, 0, iw.Width, iw.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture[0]);
            gl.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0,
                            OpenGL.GL_RGBA, // 3
                            iw.Width, iw.Height,
                            0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE,
                            data.Scan0);

            iw.UnlockBits(data);

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST_MIPMAP_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            // фильтр размытости текстуры
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAX_LEVEL, 0);
            gl.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            
            return texture[0];
        }

        /// <summary>
        /// Внесение в кеш текстур SkyBox
        /// </summary>
        /// <param name="key">ключ текстуры</param>
        /// <param name="bitmap">рисунок</param>
        public uint SetTextureSkyBox()//Bitmap[] bitmaps)//Bitmap bitmap)
        {
            Bitmap[] bitmaps = new Bitmap[]
            {
                new Bitmap(@"textures\skybox\side.png"),
                new Bitmap(@"textures\skybox\side.png"),
                new Bitmap(@"textures\skybox\top.png"),
                new Bitmap(@"textures\skybox\bottom.png"),
                new Bitmap(@"textures\skybox\side.png"),
                new Bitmap(@"textures\skybox\side.png")
            };
            
            string key = "skybox";
            //Bitmap iw = bitmap;

            OpenGL gl = OpenGLF.GetInstance().gl;

            uint[] texture = new uint[1];

            if (hashtable.ContainsKey(key))
            {
                texture[0] = (uint)hashtable[key];
            }
            else
            {
                gl.GenTextures(1, texture);
                hashtable.Add(key, texture[0]);
            }

            for (uint i = 0; i < bitmaps.Length; i++)
            {
                Bitmap iw = bitmaps[i];
                BitmapData data = iw.LockBits(
                    new Rectangle(0, 0, iw.Width, iw.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb
                );
                gl.BindTexture(OpenGL.GL_TEXTURE_CUBE_MAP, texture[0]);
                gl.TexImage2D(OpenGL.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0,
                                OpenGL.GL_RGBA, // 3
                                iw.Width, iw.Height,
                                0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE,
                                data.Scan0);
                iw.UnlockBits(data);
            }
            //gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST_MIPMAP_LINEAR);
            //gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            // фильтр размытости текстуры
            //gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MAX_LEVEL, 0);

            gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
            gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
            gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_WRAP_R, OpenGL.GL_CLAMP_TO_EDGE);
            gl.BindTexture(OpenGL.GL_TEXTURE_CUBE_MAP, 0);


            //BitmapData data = iw.LockBits(
            //    new Rectangle(0, 0, iw.Width, iw.Height),
            //    ImageLockMode.ReadOnly,
            //    PixelFormat.Format32bppArgb
            //);

            //gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture[0]);
            //gl.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);
            //gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0,
            //                OpenGL.GL_RGBA, // 3
            //                iw.Width, iw.Height,
            //                0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE,
            //                data.Scan0);

            //iw.UnlockBits(data);

            //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST_MIPMAP_LINEAR);
            //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            //// фильтр размытости текстуры
            //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAX_LEVEL, 1);
            //gl.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);
            //gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

            return texture[0];
        }

        /// <summary>
        /// Внесение в кеш текстур SkyBox
        /// </summary>
        /// <param name="key">ключ текстуры</param>
        /// <param name="bitmap">рисунок</param>
        public uint SetTextureSkyBox2()//Bitmap[] bitmaps)//Bitmap bitmap)
        {
            Bitmap[] bitmaps = new Bitmap[]
            {
                new Bitmap(@"textures\skybox\alpha.png"),
                new Bitmap(@"textures\skybox\alpha.png"),
                new Bitmap(@"textures\skybox\sun.png"),
                new Bitmap(@"textures\skybox\moon.png"),
                new Bitmap(@"textures\skybox\alpha.png"),
                new Bitmap(@"textures\skybox\alpha.png")
            };

            string key = "skybox2";
            //Bitmap iw = bitmap;

            OpenGL gl = OpenGLF.GetInstance().gl;

            uint[] texture = new uint[1];

            if (hashtable.ContainsKey(key))
            {
                texture[0] = (uint)hashtable[key];
            }
            else
            {
                gl.GenTextures(1, texture);
                hashtable.Add(key, texture[0]);
            }

            for (uint i = 0; i < bitmaps.Length; i++)
            {
                Bitmap iw = bitmaps[i];
                BitmapData data = iw.LockBits(
                    new Rectangle(0, 0, iw.Width, iw.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb
                );
                gl.BindTexture(OpenGL.GL_TEXTURE_CUBE_MAP, texture[0]);
                gl.TexImage2D(OpenGL.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0,
                                OpenGL.GL_RGBA, // 3
                                iw.Width, iw.Height,
                                0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE,
                                data.Scan0);
                iw.UnlockBits(data);
            }
            //gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST_MIPMAP_LINEAR);
            //gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            // фильтр размытости текстуры
            //gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MAX_LEVEL, 0);

            gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
            gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
            gl.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_WRAP_R, OpenGL.GL_CLAMP_TO_EDGE);
            gl.BindTexture(OpenGL.GL_TEXTURE_CUBE_MAP, 0);


            //BitmapData data = iw.LockBits(
            //    new Rectangle(0, 0, iw.Width, iw.Height),
            //    ImageLockMode.ReadOnly,
            //    PixelFormat.Format32bppArgb
            //);

            //gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture[0]);
            //gl.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);
            //gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0,
            //                OpenGL.GL_RGBA, // 3
            //                iw.Width, iw.Height,
            //                0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE,
            //                data.Scan0);

            //iw.UnlockBits(data);

            //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST_MIPMAP_LINEAR);
            //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            //// фильтр размытости текстуры
            //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAX_LEVEL, 1);
            //gl.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);
            //gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

            return texture[0];
        }
    }
}

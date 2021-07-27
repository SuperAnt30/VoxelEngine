using VoxelEngine.Glm;
using System.Drawing;

namespace VoxelEngine
{
    /// <summary>
    /// Объект вы под водой
    /// </summary>
    public class GuiWater : Render
    {
        protected override int[] _Attrs { get; } = new int[] { 3, 2, 4 };

        public void Render(Size size)
        {
            float x0 = 0; //size.Width / 2f - 16f;
            float x1 = size.Width;// x0 + 32f;
            float y0 = 0;// size.Height / 2f - 16f;
            float y1 = size.Height;// y0 + 32f;
            vec4 c = new vec4(.0f, .6f, .9f, 0.5f);
            float[] buffer = new float[]
            {
                x0, y0, 0, 0.0624f, 0f, c.x, c.y, c.z, c.w,
                x0, y1, 0, 0.0624f, 0.0624f, c.x, c.y, c.z, c.w,
                x1, y0, 0, 0.1248f, 0f, c.x, c.y, c.z, c.w,

                x0, y1, 0, 0.0624f, 0.0624f, c.x, c.y, c.z, c.w,
                x1, y1, 0, 0.1248f, 0.0624f, c.x, c.y, c.z, c.w,
                x1, y0, 0, 0.1248f, 0f, c.x, c.y, c.z, c.w
            };
            _Render(buffer);
        }
    }
}

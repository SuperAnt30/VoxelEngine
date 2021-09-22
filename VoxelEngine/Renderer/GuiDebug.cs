using VoxelEngine.Glm;
using System.Drawing;
using VoxelEngine.Graphics;

namespace VoxelEngine.Renderer
{
    /// <summary>
    /// Объект для дебага
    /// </summary>
    public class GuiDebug : RenderMesh
    {
        protected override int[] _Attrs { get; } = new int[] { 3, 2, 4 };

        public void Render(Size size)
        {
            float x0 = size.Width - 512;
            float x1 = size.Width;
            float y0 = 0;
            float y1 = 512;
            vec4 c = new vec4(1f);
            float[] buffer = new float[]
            {
                x0, y0, 0, 0, 0f, c.x, c.y, c.z, c.w,
                x0, y1, 0, 0, 1f, c.x, c.y, c.z, c.w,
                x1, y0, 0, 1, 0f, c.x, c.y, c.z, c.w,

                x0, y1, 0, 0, 1f, c.x, c.y, c.z, c.w,
                x1, y1, 0, 1, 1f, c.x, c.y, c.z, c.w,
                x1, y0, 0, 1, 0f, c.x, c.y, c.z, c.w
            };
            Render(buffer);
        }
    }
}

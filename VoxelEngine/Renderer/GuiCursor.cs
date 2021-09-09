using VoxelEngine.Glm;
using VoxelEngine.Graphics;

namespace VoxelEngine.Renderer
{
    /// <summary>
    /// Объект курсора
    /// </summary>
    public class GuiCursor : RenderMesh
    {
        protected override int[] _Attrs { get; } = new int[] { 3, 2, 4 };

        public void Render(int width, int height)
        {
            float x0 = width / 2f - 16f;
            float x1 = x0 + 32f;
            float y0 = height / 2f - 16f;
            float y1 = y0 + 32f;
            vec4 c = new vec4(.9f, .9f, .9f, 0.9f);
            float[] buffer = new float[]
            {
                x0, y0, 0, 0f, 0f, c.x, c.y, c.z, c.w,
                x0, y1, 0, 0f, 0.0624f, c.x, c.y, c.z, c.w,
                x1, y0, 0, 0.0624f, 0f, c.x, c.y, c.z, c.w,

                x0, y1, 0, 0f, 0.0624f, c.x, c.y, c.z, c.w,
                x1, y1, 0, 0.0624f, 0.0624f, c.x, c.y, c.z, c.w,
                x1, y0, 0, 0.0624f, 0f, c.x, c.y, c.z, c.w
            };
            Render(buffer);
        }
    }
}

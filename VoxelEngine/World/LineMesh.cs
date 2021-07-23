namespace VoxelEngine
{
    /// <summary>
    /// Прорисовка линий
    /// </summary>
    public class LineMesh : Render
    {
        /// <summary>
        /// Буфер точки, точка xyz и цвет точки rgba
        /// </summary>
        protected override int[] _Attrs { get; } = new int[] { 3, 4 };

        /// <summary>
        /// Ключь сетки линии
        /// </summary>
        public string Key { get; protected set; }

        public LineMesh(string key)
        {
            Key = key;
        }

        public void Render(float[] buffer)
        {
            _Render(buffer);
        }
    }
}

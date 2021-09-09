using VoxelEngine.Glm;

namespace VoxelEngine.Models
{
    /// <summary>
    /// Объект вершины с текстурой
    /// </summary>
    public class TextureVertex
    {
        /// <summary>
        /// Координаты точки
        /// </summary>
        public vec3 Position { get; protected set; }
        /// <summary>
        /// Координаты текстуры
        /// </summary>
        public vec2 Texture { get; protected set; }

        public TextureVertex(vec3 pos)
        {
            Position = pos;
        }
        public TextureVertex(vec3 pos, vec2 texture)
        {
            Position = pos;
            Texture = texture;
        }
        public TextureVertex(float x, float y, float z, float u, float v)
        {
            Position = new vec3(x, y, z);
            Texture = new vec2(u, v);
        }

        public TextureVertex(TextureVertex textureVertex, float u, float v)
        {
            Position = textureVertex.Position;
            Texture = new vec2(u, v);
        }

        public TextureVertex SetTexturePosition(float u, float v)
        {
            //Texture = new vec2(u, v);
            //return this;
            return new TextureVertex(this, u, v);
        }

        public void SetPosition(vec3 pos)
        {
            Position = pos;
        }
    }
}

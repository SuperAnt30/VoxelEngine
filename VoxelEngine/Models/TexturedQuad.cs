
using System.Collections.Generic;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine.Models
{
    /// <summary>
    /// Объект стороны
    /// </summary>
    public class TexturedQuad : BufferHeir
    {
        /// <summary>
        /// Позиции вершин (x, y, z) и координаты текстуры (u, v) для каждой из 4 точек на стороне
        /// </summary>
        public TextureVertex[] Vertices { get; protected set; } = new TextureVertex[4];

        public TexturedQuad(vec3[] pos)
        {
            Ver(pos);
        }

        public TexturedQuad(vec3[] pos, int u1, int v1, int u2, int v2, vec2 textureSize)
        {
            Ver(pos);
            float w = 0.0f;// / textureWidth;
            float h = 0.0f;// / textureHeight;
            Vertices[0] = Vertices[0].SetTexturePosition((float)u2 / textureSize.x - w, (float)v1 / textureSize.y + h);
            Vertices[1] = Vertices[1].SetTexturePosition((float)u1 / textureSize.x + w, (float)v1 / textureSize.y + h);
            Vertices[2] = Vertices[2].SetTexturePosition((float)u1 / textureSize.x + w, (float)v2 / textureSize.y - h);
            Vertices[3] = Vertices[3].SetTexturePosition((float)u2 / textureSize.x - w, (float)v2 / textureSize.y - h);
        }

        protected void Ver(vec3[] pos)
        {
            for (int i = 0; i < 4; i++)
            {
                Vertices[i] = new TextureVertex(pos[i]);
            }
        }

        protected float[] Tr(int index)
        {
            TextureVertex vertex = Vertices[index];
            return new float[]{
                pos.x + vertex.Position.x * scale, pos.y + (1.51f - vertex.Position.y * scale), pos.z + vertex.Position.z * scale,
                vertex.Texture.x, vertex.Texture.y
            };
        }

        protected vec3 pos;
        protected float scale;

        public void Render(ModelBox modelBox, vec3 pos, float scale)
        {
            this.pos = pos;
            this.scale = scale;

            List<float> buffer = new List<float>();

            int[] indexs = new int[] {
                0, 1, 2, 0, 2, 3, // видим
                2, 1, 0, 3, 2, 0 // изнутри
            };

            foreach (int index in indexs)
            {
                buffer.AddRange(Tr(index));
            }

            Buffer = buffer.ToArray();
            
        }
    }
}

using SharpGL;
using System;

namespace VoxelEngine
{
    /// <summary>
    /// Объект буфера сетки через VAO
    /// </summary>
    public class Mesh : IDisposable
    {
        protected uint[] vao = new uint[1];
        protected uint[] vbo = new uint[1];
        protected int[] _attrs;
        protected OpenGL gl;
        protected int countVertices = 0;
        protected int vertexSize = 0;
        /// <summary>
        /// Количество float в буфере на один полигон
        /// </summary>
        public int PoligonFloat { get; protected set; } = 0;

        public Mesh(float[] vertices, int[] attrs)
        {
            for (int i = 0; i < attrs.Length; i++)
            {
                vertexSize += attrs[i];
            }
            countVertices = vertices.Length / vertexSize;
            PoligonFloat = vertexSize * 3;
            _attrs = attrs;

            gl = OpenGLF.GetInstance().gl;

            _BufferData(vertices);
        }

        protected void _BufferData(float[] vertices)
        {
            gl.GenVertexArrays(1, vao);
            gl.BindVertexArray(vao[0]);
            gl.GenBuffers(1, vbo);
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vbo[0]);
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, vertices, OpenGL.GL_STATIC_DRAW);

            // attributes
            int stride = vertexSize * sizeof(float);
            int offset = 0;
            for (uint i = 0; i < _attrs.Length; i++)
            {
                if (_attrs[i] == 0) break;
                int size = _attrs[i];
                gl.VertexAttribPointer(i, size, OpenGL.GL_FLOAT, false, stride, new IntPtr(offset * sizeof(float)));
                gl.EnableVertexAttribArray(i);
                offset += size;
            }

            gl.BindVertexArray(0);
        }

        /// <summary>
        /// Удалить меш
        /// </summary>
        public void Delete()
        {
            gl.DeleteVertexArrays(1, vao);
            gl.DeleteBuffers(1, vbo);
        }

        /// <summary>
        /// Прорисовать меш
        /// </summary>
        /// <param name="primitive">OpenGL.GL_TRIANGLES || OpenGL.GL_QUADS</param>
        public void Draw(uint primitive)
        {
            gl.BindVertexArray(vao[0]);
            gl.DrawArrays(primitive, 0, countVertices);
            gl.BindVertexArray(0);
        }

        /// <summary>
        /// Прорисовать меш с треугольными полигоvoa нами
        /// </summary>
        public void Draw()
        {
            gl.BindVertexArray(vao[0]);
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, countVertices);
            gl.BindVertexArray(0);
        }

        /// <summary>
        /// Прорисовать меш с линиями
        /// </summary>
        public void DrawLine()
        {
            gl.BindVertexArray(vao[0]);
            gl.DrawArrays(OpenGL.GL_LINES, 0, countVertices);
            gl.BindVertexArray(0);
        }

        /// <summary>
        /// Перезаписать полигоны, не создавая и не меняя длинну одной точки
        /// </summary>
        /// <param name="vertices"></param>
        public void Reload(float[] vertices)
        {
            countVertices = vertices.Length / vertexSize;

            gl.BindVertexArray(vao[0]);
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vbo[0]);
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, vertices, OpenGL.GL_STATIC_DRAW);
        }

        public void Dispose()
        {
            Delete();
        }
    }
}

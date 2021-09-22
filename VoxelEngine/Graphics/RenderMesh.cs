﻿using System;
using System.Collections.Generic;

namespace VoxelEngine.Graphics
{
    /// <summary>
    /// Объект создания сетки буфера, прорисовки и удаления
    /// </summary>
    public class RenderMesh : IDisposable
    {
        protected Mesh _mesh;
        /// <summary>
        /// Буфер точки, точка xyz, текстура точки uv, цвет точки rgba
        /// </summary>
        protected virtual int[] _Attrs { get; } = new int[] { 3, 2, 4 };

        public int CountPoligon { get; protected set; } = 0;

        /// <summary>
        /// Сгенерировать
        /// </summary>
        public virtual void Render(List<float> buffer)
        {
            Render(buffer.ToArray());
        }

        /// <summary>
        /// Сгенерировать
        /// </summary>
        public virtual void Render(float[] buffer)
        {
            
            if (_mesh != null)
            {
                _mesh.Reload(buffer);
            }
            else
            {
                _mesh = new Mesh(buffer, _Attrs);
            }
            CountPoligon = buffer.Length / _mesh.PoligonFloat;
        }

        /// <summary>
        /// Прорисовать
        /// </summary>
        public void Draw()
        {
            if (_mesh != null)
            {
                Debug.GetInstance().CountMesh++;
                _mesh.Draw();
            }
        }

        /// <summary>
        /// Прорисовать
        /// </summary>
        public void DrawLine()
        {
            Debug.GetInstance().CountMesh++;
            _mesh.DrawLine();
        }

        /// <summary>
        /// Удалить
        /// </summary>
        public void Delete()
        {
            CountPoligon = 0;
            if (_mesh != null) _mesh.Delete();
        }

        /// <summary>
        /// Удаление если объект удалиться сам
        /// </summary>
        public void Dispose()
        {
            Delete();
        }
    }
}
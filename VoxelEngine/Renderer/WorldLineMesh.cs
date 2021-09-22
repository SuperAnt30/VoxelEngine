using System.Collections;
using VoxelEngine.Glm;
using VoxelEngine.Graphics;

namespace VoxelEngine.Renderer
{
    /// <summary>
    /// Объект для 3д хранение диний Mesh
    /// </summary>
    public class WorldLineMesh
    {
        /// <summary>
        /// Массив кэша линий
        /// </summary>
        protected Hashtable items = new Hashtable();

        /// <summary>
        /// Прорисовка чанков
        /// </summary>
        public void Draw()
        {
            Debug.GetInstance().CountMeshLine = items.Count;
            foreach (DictionaryEntry s in items)
            {
                LineMesh cm = s.Value as LineMesh;
                cm.DrawLine();
            }
        }

        /// <summary>
        /// Получить кэш линии
        /// </summary>
        public LineMesh GetLine(string key)
        {
            if (items.ContainsKey(key))
            {
                return items[key] as LineMesh;
            }
            return null;
        }

        /// <summary>
        /// Внести линию буфера
        /// </summary>
        /// <param name="key"></param>
        /// <param name="buffer"></param>
        public void RenderChank(string key, float[] buffer)
        {
            LineMesh lineMesh = GetLine(key);
            if (lineMesh == null)
            {
                lineMesh = new LineMesh(key);
                items.Add(key, lineMesh);
            }
            lineMesh.Render(buffer);
        }

        /// <summary>
        /// Удалить линию буфера
        /// </summary>
        public void Remove(string key)
        {
            LineMesh lineMesh = GetLine(key);
            if (lineMesh != null)
            {
                items.Remove(key);
            }
        }

        /// <summary>
        /// Удалить линии буфера по префиксу
        /// </summary>
        public void RemovePrefix(string prefixKey)
        {
            Hashtable itemsClone = (Hashtable)items.Clone();
            foreach (DictionaryEntry s in itemsClone)
            {
                if (s.Key.ToString().Substring(0, prefixKey.Length) == prefixKey)
                {
                    items.Remove(s.Key);
                }
            }
        }

        /// <summary>
        /// Добавить блок по рёбрам
        /// </summary>
        /// <param name="key">ключ</param>
        /// <param name="x">XYZ</param>
        /// <param name="y">XYZ</param>
        /// <param name="z">XYZ</param>
        /// <param name="w">ширина</param>
        /// <param name="h">высота</param>
        /// <param name="d">длина</param>
        /// <param name="r">RGBA</param>
        /// <param name="g">RGBA</param>
        /// <param name="b">RGBA</param>
        /// <param name="a">RGBA</param>
        public void Box(string key, float x, float y, float z, float w, float h, float d,
        float r, float g, float b, float a)
        {
            LineRender line = new LineRender();
            line.Box(x, y, z, w, h, d, r, g, b, a);
            RenderChank(key, line.ToBuffer());
        }

        /// <summary>
        /// Прорисовка сетки текущего чанка
        /// </summary>
        public void Chunk()
        {
            vec2i vc = OpenGLF.GetInstance().Cam.ChunkPos;
            vc = new vec2i(vc.x << 4, vc.y << 4);
            vec2i vc16 = vc + 16;

            LineRender line = new LineRender();
            vec4 c1 = new vec4(0f, 0f, 1f, 1f);
            vec4 c2 = new vec4(1f, 1f, 0f, 1f);
            vec4 c3 = new vec4(1f, 0f, 0f, 1f);
            
            // Вертикальные
            for (int i = 0; i < 16; i += 2) {
                vec4 c = i == 0 ? c1 : c2;
                if (i != 0) line.Line(vc.x + i, 0, vc.y, vc.x + i, 256, vc.y, c);
                line.Line(vc.x + i, 0, vc16.y, vc.x + i, 256, vc16.y, c);
                line.Line(vc.x, 0, vc.y + i, vc.x, 256, vc.y + i, c);
                line.Line(vc16.x, 0, vc.y + i, vc16.x, 256, vc.y + i, c);
            }
            line.Line(vc16.x, 0, vc16.y, vc16.x, 256, vc16.y, c1);
            // Горизонтальные
            for (int i = 0; i <= 256; i += 2)
            {
                vec4 c = ((byte)i & 0xF) == 0 ? c1 : c2;
                line.Line(vc.x, i, vc.y, vc.x, i, vc16.y, c);
                line.Line(vc.x, i, vc.y, vc16.x, i, vc.y, c);
                line.Line(vc.x, i, vc16.y, vc16.x, i, vc16.y, c);
                line.Line(vc16.x, i, vc.y, vc16.x, i, vc16.y, c);
            }

            // Соседние углы чанков
            int ch2 = 32;
            for (int i = -16; i < ch2; i += 16)
            {
                line.Line(vc.x - 16, 0, vc.y + i, vc.x - 16, 256, vc.y + i, c3);
                line.Line(vc.x + ch2, 0, vc.y + i, vc.x + ch2, 256, vc.y + i, c3);
                if (i != -16) line.Line(vc.x + i, 0, vc.y - 16, vc.x + i, 256, vc.y - 16, c3);
                line.Line(vc.x + i, 0, vc.y + ch2, vc.x + i, 256, vc.y + ch2, c3);
            }
            line.Line(vc.x + ch2, 0, vc.y + ch2, vc.x + ch2, 256, vc.y + ch2, c3);

            RenderChank("chunk", line.ToBuffer());
        }

    }
}

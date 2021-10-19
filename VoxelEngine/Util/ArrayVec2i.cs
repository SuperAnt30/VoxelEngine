using System.Collections;
using System.Collections.Generic;
using VoxelEngine.Glm;

namespace VoxelEngine.Util
{
    /// <summary>
    /// Массив vec2i с доб проверкой
    /// </summary>
    public class ArrayVec2i
    {
        List<vec2i> list = new List<vec2i>();
        Hashtable ht = new Hashtable();

        /// <summary>
        /// Добавить с проверкой
        /// </summary>
        public void AddCheck(vec2i value)
        {
            string key = value.ToString();
            if (!ht.ContainsKey(key))
            {
                list.Add(value);
                ht.Add(key, value);
            }
        }
        /// <summary>
        /// Добавить с проверкой
        /// </summary>
        public void AddCheck(int x, int z)
        {
            AddCheck(new vec2i(x, z));
        }

        /// <summary>
        /// Добавить
        /// </summary>
        public void Add(vec2i value)
        {
            list.Add(value);
            ht.Add(value.ToString(), value);
        }

        /// <summary>
        /// Добавить
        /// </summary>
        public void Add(int x, int z)
        {
            Add(new vec2i(x, z));
        }

        /// <summary>
        /// Вернуть массив
        /// </summary>
        public vec2i[] ToArray()
        {
            return list.ToArray();
        }
    }
}

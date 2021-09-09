using System.Collections;
using System.Collections.Generic;

namespace VoxelEngine.World.Chunk
{
    public class ChunkLoadMap
    {
        /// <summary>
        /// Массив кэша чанков vec2i, ChunkMeshs
        /// </summary>
        protected Hashtable _ht = new Hashtable();
        /// <summary>
        /// дубликат
        /// </summary>
        //protected List<ChunkLoading> _list = new List<ChunkLoading>();

        /// <summary>
        /// Добавить или изменить
        /// </summary>
        /// <param name="chunk"></param>
        public void Set(ChunkLoading chunk)
        {
            //_list.Add(chunk);
            string key = chunk.Key();
            if (_ht.ContainsKey(key))
            {
                //ChunkLoading old = _ht[key] as ChunkLoading;
                //if (_list.Contains(old))
                //{
                //    _list.Remove(old);
                //}
                _ht[key] = chunk;
                //_list.Add(chunk);
            }
            else
            {
                _ht.Add(key, chunk);
                //_list.Add(chunk);
            }
        }

        /// <summary>
        /// Очистить
        /// </summary>
        //public void Clear()
        //{
        //    _ht.Clear();
        //    _list.Clear();
        //}

        //public void Remove(ChunkLoading cl)
        //{

        //    if (_list.Contains(cl))
        //    {
        //        _list.Remove(cl);
        //    }
        //}

        /// <summary>
        /// Удалить
        /// </summary>
        public void Remove(string key)
        {
            if (_ht.ContainsKey(key))
            {
            //    _list.Remove((ChunkLoading)_ht[key]);
                _ht.Remove(key);
            }
        }


        /// <summary>
        /// Получить количество
        /// </summary>
        public int Count { get { return _ht.Count; } }

        /// <summary>
        /// Получить первое значение по дистанции
        /// </summary>
        public ChunkLoading One()
        {
            if (_ht.Count == 0) return null;
            ChunkLoading cl = new ChunkLoading();
            float d = 1000f;
            foreach(ChunkLoading de in _ht.Values)
            {
                if (d > de.Distance)
                {
                    d = de.Distance;
                    cl = de;
                }
            }
            return cl;
            //if (_list.Count > 0)
            //{
            //    if (_list.Count > 1) _list.Sort();
            //    return _list[0];
            //}
            return null;
        }

    }
}

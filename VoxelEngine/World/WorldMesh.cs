using System.Collections;
using System.Collections.Generic;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Объект для 3д хранение Mesh
    /// </summary>
    public class WorldMesh
    {
        /// <summary>
        /// Массив кэша чанков vec2i, ChunkMeshs
        /// </summary>
        protected Hashtable _chunks = new Hashtable();

        /// <summary>
        /// Прорисовка чанков
        /// </summary>
        public void DrawDenseOld()
        {
            Debag.GetInstance().CountMeshChunk = _chunks.Count;

            foreach (ChunkMeshs cm in _chunks.Values)
            {
                cm.MeshDense.Draw();
            }
        }

        /// <summary>
        /// Ключ для массива
        /// </summary>
        public static string KeyChunk(int x, int z)
        {
            return x.ToString() + ";" + z.ToString();
        }

        /// <summary>
        /// Прорисовка чанков
        /// </summary>
        public void DrawDense(int min, int max)
        {
            Debag.GetInstance().CountMeshChunk = _chunks.Count;

            Camera camera = OpenGLF.GetInstance().Cam;
            Pole pole = camera.GetPole();
            vec2i pos = camera.ToPositionChunk();
            ChunkLoading[] spiral = VES.GetInstance().DistSqrt;

            if (max == -1) max = spiral.Length - 1;

            // Прорисовка алфы в зависимости куда смотрим. От до
            for (int i = max; i >= min; i--)
            {
                string key = KeyChunk(pos.x + spiral[i].X, pos.y + spiral[i].Z);
                if (_chunks.ContainsKey(key))
                {
                    ChunkMeshs cm = _chunks[key] as ChunkMeshs;
                    cm.MeshDense.Draw();
                }
            }
        }

        /// <summary>
        /// Прорисовка чанков
        /// </summary>
        public void DrawDenseNew()
        {
            Debag.GetInstance().CountMeshChunk = _chunks.Count;

            Camera camera = OpenGLF.GetInstance().Cam;
            Pole pole = camera.GetPole();
            vec3i vec = EnumFacing.DirectionVec(pole);
            vec2i pos = camera.ToPositionChunk();
            ChunkLoading[] spiral = VES.GetInstance().DistSqrt;

            // Прорисовка алфы в зависимости куда смотрим. От до
            for (int i = spiral.Length - 1; i >= 0; i--)
            {
                vec2i vec2 = new vec2i(pos.x + spiral[i].X, pos.y + spiral[i].Z);

                if ((vec.x > 0 && vec2.x >= pos.x)
                    || (vec.x < 0 && vec2.x <= pos.x)
                    || (vec.z > 0 && vec2.y <= pos.y)
                    || (vec.z < 0 && vec2.y >= pos.y)
                    )
                {
                    string key = KeyChunk(vec2.x, vec2.y);
                    if (_chunks.ContainsKey(key))
                    {
                        ChunkMeshs cm = _chunks[key] as ChunkMeshs;
                        cm.MeshDense.Draw();
                    }
                }
            }
        }

        /// <summary>
        /// Прорисовка чанков
        /// </summary>
        public void DrawAlpha()
        {
            Camera camera = OpenGLF.GetInstance().Cam;
            Pole pole = camera.GetPole();
            vec2i pos = camera.ToPositionChunk();
            ChunkLoading[] spiral = VES.GetInstance().DistSqrt;

            // Прорисовка алфы в зависимости куда смотрим. От до
            for (int i = spiral.Length - 1; i >= 0; i--)
            {
                string key = KeyChunk(pos.x + spiral[i].X, pos.y + spiral[i].Z);
                if (_chunks.ContainsKey(key))
                {
                    ChunkMeshs cm = _chunks[key] as ChunkMeshs;
                    if (cm.MeshAlpha.CountPoligon > 0) cm.MeshAlpha.Draw();
                }
            }
        }

        /// <summary>
        /// Получить чанк с кэша, если его там нет, то сгенерировать его
        /// </summary>
        public ChunkMeshs GetChunk(int x, int z)
        {
            string key = KeyChunk(x, z);
            if (!_chunks.ContainsKey(key))
            {
                _chunks.Add(key, new ChunkMeshs(x, z));
                Debag.GetInstance().RenderChunk = _chunks.Count;
            }

            return _chunks[key] as ChunkMeshs;
        }

        /// <summary>
        /// Внести сетку буфера в чанк
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="buffer"></param>
        public void RenderChank(int x, int z, float[] buffer)
        {
            GetChunk(x, z).MeshDense.Render(buffer);
        }

        /// <summary>
        /// Внести сетку буфера в чанк
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="buffer"></param>
        public void RenderChankAlpha(int x, int z, float[] buffer)
        {
            GetChunk(x, z).MeshAlpha.Render(buffer);
        }

        /// <summary>
        /// Удалить дальние чанки из массива кэша сеток
        /// </summary>
        public void RemoveAway(vec2i positionCam)
        {
            List<string> vs = new List<string>();
            // дальность чанков с учётом кэша
            int visiblityCache = VE.CHUNK_VISIBILITY + 2;

            int xMin = positionCam.x - visiblityCache;
            int xMax = positionCam.x + visiblityCache;
            int zMin = positionCam.y - visiblityCache;
            int zMax = positionCam.y + visiblityCache;
            // Собираем массив чанков которые уже не попадают в видимость
            foreach (DictionaryEntry s in _chunks)
            {
                ChunkMeshs cm = s.Value as ChunkMeshs;
                if (cm.X <= xMin || cm.X >= xMax || cm.Z <= zMin || cm.Z >= zMax)
                {
                    Debag.GetInstance().CountPoligonChunk -= cm.CountPoligon;
                    OnRemoveChanged(new vec2i(cm.X, cm.Z));
                    cm.Delete();
                    vs.Add(s.Key.ToString());
                }
            }

            // Удаляем
            if (vs.Count > 0)
            {
                foreach (string key in vs)
                {
                    _chunks.Remove(key);
                }
            }

            Debag.GetInstance().RenderChunk = _chunks.Count;
        }

        
        /// <summary>
        /// Не используется Спираль массива
        /// </summary>
        public static vec2i[] GetSpiralOld(bool one)
        {
            int size = VE.CHUNK_VISIBILITY * 2 + 1;
            vec2i[,] a = new vec2i[size, size];
            if (one)
            {
                for (int x = 0; x < size; x++)
                    for (int y = 0; y < size; y++)
                        a[x, y] = new vec2i(x, y);
            } else
            {
                for (int x = 0; x < size; x++)
                    for (int y = 0; y < size; y++)
                        a[x, y] = new vec2i(size - x - 1, y);
            }

            var r = new List<vec2i>();
            var n = a.GetLength(0);
            int j = -1, i = 0;
            bool h = true;
            bool d = false;
            int c = 0;
            int p = n;
            int max = n;
            for (var cnt = 1; cnt <= a.Length; cnt++)
            {
                i = h ? i : !d ? ++i : --i;
                j = !h ? j : !d ? ++j : --j;
                p--;
                r.Add(a[i, j]);
                if (p <= 0)
                {
                    h = !h;
                    if ((c + 1) % 2 == 0) { d = !d; }
                    if (cnt == n || c > 1 && (c + 1) % 2 != 0) { --max; }
                    p = max;
                    c++;
                }
            }
            return r.ToArray();
        }

        /// <summary>
        /// Событие удалена сетка
        /// </summary>
        public event CoordEventHandler RemoveChanged;

        /// <summary>
        /// удалена сетка
        /// </summary>
        protected virtual void OnRemoveChanged(vec2i position)
        {
            RemoveChanged?.Invoke(this, new CoordEventArgs(position));
        }
    }
}

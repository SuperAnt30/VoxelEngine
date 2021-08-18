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
        public void DrawDense()
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
        public void Cleaning(vec2i positionCam)
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

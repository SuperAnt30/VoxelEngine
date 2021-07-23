using System.Collections.Generic;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine
{
    /// <summary>
    /// Объект для 3д хранение Mesh
    /// НЕ используется аналог объекта WorldMesh
    /// </summary>
    public class WorldMeshDictionary
    {
        /// <summary>
        /// Массив видимых чанков ChunkRender
        /// </summary>
        protected Dictionary<vec2i, ChunkMeshs> chunksMeshs = new Dictionary<vec2i, ChunkMeshs>(); 

        /// <summary>
        /// Прорисовка чанков
        /// </summary>
        public void DrawDense(int min, int max)
        {
            Debag.GetInstance().CountMeshChunk = chunksMeshs.Count;

            Camera camera = OpenGLF.GetInstance().Cam;
            Pole pole = camera.GetPole();
            vec2i pos = camera.ToPositionChunk();
            ChunkLoading[] spiral = OpenGLF.GetInstance().DistSqrt;

            if (max == -1) max = spiral.Length - 1;

            // Прорисовка алфы в зависимости куда смотрим. От до
            for (int i = max; i >= min; i--)
            {
                vec2i key = new vec2i(pos.x + spiral[i].X, pos.y + spiral[i].Z);
                if (chunksMeshs.ContainsKey(key))
                {
                    chunksMeshs[key].MeshDense.Draw();
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
            ChunkLoading[] spiral = OpenGLF.GetInstance().DistSqrt;

            // Прорисовка алфы в зависимости куда смотрим. От до
            for (int i = spiral.Length - 1; i >= 0; i--)
            {
                vec2i key = new vec2i(pos.x + spiral[i].X, pos.y + spiral[i].Z);
                if (chunksMeshs.ContainsKey(key) && chunksMeshs[key].MeshAlpha.CountPoligon > 0)
                {
                    chunksMeshs[key].MeshAlpha.Draw();
                }
            }
        }

        /// <summary>
        /// Получить чанк с кэша, если его там нет, то сгенерировать его
        /// </summary>
        public ChunkMeshs GetChunk(int x, int z)
        {
            vec2i key = new vec2i(x, z);
            if (!chunksMeshs.ContainsKey(key))
            {
                chunksMeshs[key] = new ChunkMeshs(x, z);
                Debag.GetInstance().RenderChunk = chunksMeshs.Count;
            }
            return chunksMeshs[key];
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
            int xMin = positionCam.x - VE.CHUNK_VISIBILITY;
            int xMax = positionCam.x + VE.CHUNK_VISIBILITY;
            int zMin = positionCam.y - VE.CHUNK_VISIBILITY;
            int zMax = positionCam.y + VE.CHUNK_VISIBILITY;
            List<vec2i> vs = new List<vec2i>();

            foreach (ChunkMeshs cm in chunksMeshs.Values)
            {
                if (cm.X <= xMin || cm.X >= xMax || cm.Z <= zMin || cm.Z >= zMax)
                {
                    Debag.GetInstance().CountPoligonChunk -= cm.CountPoligon;
                    cm.Delete();
                    vs.Add(new vec2i(cm.X, cm.Z));
                }
            }
            // Удаляем
            if (vs.Count > 0)
            {
                foreach (vec2i key in vs)
                {
                    chunksMeshs.Remove(key);
                }
            }

            Debag.GetInstance().RenderChunk = chunksMeshs.Count;
        }
    }
}

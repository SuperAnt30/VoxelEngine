using System.Collections.Generic;
using VoxelEngine.Glm;
using VoxelEngine.Util;

namespace VoxelEngine
{/*
    /// <summary>
    /// Объект для 3д хранение Mesh
    /// НЕ используется аналог объекта WorldMesh
    /// </summary>
    public class WorldMeshArray
    {
        /// <summary>
        /// Массив видимых чанков ChunkRender
        /// </summary>
        protected ChunkMeshs[] _chunksMeshs = new ChunkMeshs[VE.CHUNK_ALL];
        //protected vec2i[] _chunksVec2i = new vec2i[VE.CHUNK_ALL];
        //protected Dictionary<vec2i, ChunkMeshs> chunksMeshs = new Dictionary<vec2i, ChunkMeshs>(); 
       // protected Dictionary<vec2i, int> _cMI = new Dictionary<vec2i, int>();
        /// <summary>
        /// Индекс количества загруженых чанков
        /// </summary>
        protected int _chunkIndex = 0;

        /// <summary>
        /// Прорисовка чанков
        /// </summary>
        public void DrawDenseOld()
        {
            Debag.GetInstance().CountMeshChunk = _chunkIndex;

            foreach (ChunkMeshs cm in _chunksMeshs)
            {
                cm.MeshDense.Draw();
            }
        }

        /// <summary>
        /// Прорисовка чанков
        /// </summary>
        public void DrawDense(int min, int max)
        {
            Debag.GetInstance().CountMeshChunk = _chunkIndex;

            //Camera camera = OpenGLF.GetInstance().Cam;
            //Pole pole = camera.GetPole();
            //vec2i pos = camera.ToPositionChunk();
            //ChunkLoading[] spiral = OpenGLF.GetInstance().DistSqrt;

            //if (max == -1) max = spiral.Length - 1;

            // Прорисовка алфы в зависимости куда смотрим. От до
            //for (int i = max; i >= min; i--)
            //{
            //    for (int j = 0; j < _chunksMeshs.Length; j++)
            //    {
            //        if (_chunksMeshs[j] != null && _chunksMeshs[j].X == pos.x + spiral[i].X && _chunksMeshs[j].Z == pos.y + spiral[i].Z)
            //        {
            //            _chunksMeshs[j].MeshDense.Draw();
            //        }
            //    }
            //}
            
            //for (int i = max; i >= min; i--)
            //{

            for (int j = 0; j < _chunksMeshs.Length; j++)
            {
                if (_chunksMeshs[j] != null)// && _chunksMeshs[j].X == pos.x + spiral[i].X && _chunksMeshs[j].Z == pos.y + spiral[i].Z)
                {
                    _chunksMeshs[j].MeshDense.Draw();
                }
            }

            //}
        }

        /// <summary>
        /// Прорисовка чанков
        /// </summary>
        public void DrawAlpha()
        {
            return;
            Camera camera = OpenGLF.GetInstance().Cam;
            Pole pole = camera.GetPole();
            vec2i pos = camera.ToPositionChunk();
            ChunkLoading[] spiral = OpenGLF.GetInstance().DistSqrt;

            // Прорисовка алфы в зависимости куда смотрим. От до
            for (int i = spiral.Length - 1; i >= 0; i--)
            {

                for (int j = 0; j < _chunksMeshs.Length; j++)
                {
                    if (_chunksMeshs[j] != null && _chunksMeshs[j].X == pos.x + spiral[i].X && _chunksMeshs[j].Z == pos.y + spiral[i].Z
                        && _chunksMeshs[j].MeshAlpha.CountPoligon > 0)
                    {
                        _chunksMeshs[j].MeshAlpha.Draw();
                    }
                }
            }
        }

        //public ChunkMeshs GetChunk(int x, int z)
        //{
        //    vec2i v = new vec2i(x, z);
        //    if (_cMI.ContainsKey(v)) return _chunksMeshs[_cMI[v]];
        //    _chunksMeshs[_chunkIndex] = new ChunkMeshs(x, z);
        //    _cMI.Add(new vec2i(x, z), _chunkIndex);
        //    _chunkIndex++;

        //    Debag.GetInstance().RenderChunk = _chunkIndex;
        //    return _chunksMeshs[_chunkIndex - 1];
        //}


        /// <summary>
        /// Получить чанк с кэша, если его там нет, то сгенерировать его
        /// </summary>
        public ChunkMeshs GetChunk(int x, int z)
        {
            for (int i = 0; i < _chunksMeshs.Length; i++)
            {
                if (_chunksMeshs[i] != null && _chunksMeshs[i].X == x && _chunksMeshs[i].Z == z)
                {
                    return _chunksMeshs[i];
                }
            }
            _chunksMeshs[_chunkIndex] = new ChunkMeshs(x, z);
            _chunkIndex++;
            
            Debag.GetInstance().RenderChunk = _chunkIndex;
            return _chunksMeshs[_chunkIndex - 1];
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

            ChunkMeshs[] chunksBuffer = new ChunkMeshs[VE.CHUNK_ALL];
            Dictionary<vec2i, int> cMI = new Dictionary<vec2i, int>();
            _chunkIndex = 0;
            foreach(ChunkMeshs cm in _chunksMeshs)
            {
                if (cm != null)
                {
                    if (cm.X > xMin && cm.X < xMax && cm.Z > zMin && cm.Z < zMax)
                    {
                        chunksBuffer[_chunkIndex] = cm;
                        cMI.Add(new vec2i(cm.X, cm.Z), _chunkIndex);
                        _chunkIndex++;
                    }
                    else
                    {
                        Debag.GetInstance().CountPoligonChunk -= cm.CountPoligon;
                    }
                }
            }
            _chunksMeshs = chunksBuffer;
            //_cMI = cMI;
            chunksBuffer = new ChunkMeshs[0];
            Debag.GetInstance().RenderChunk = _chunkIndex;
        }
    }*/
}

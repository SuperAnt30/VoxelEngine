using VoxelEngine.Glm;
using System.Collections.Generic;
using VoxelEngine.Util;
using VoxelEngine.World.Chunk;
using System;

namespace VoxelEngine
{
    /// <summary>
    /// Объект рендера чанков
    /// </summary>
    public class ChunkRender
    {
        // Формула получения вокселя в чанке
        // (y * VE.CHUNK_WIDTH + z) * VE.CHUNK_WIDTH + x

        /// <summary>
        /// Объект мира которы берёт из объекта ThreadWorld
        /// </summary>
        public WorldRender World { get; protected set; }

        public ChunkD Chunk { get; protected set; }

        /// <summary>
        /// Массив альфа блоков Voxels
        /// </summary>
        protected List<VoxelData> _alphas = new List<VoxelData>();

        /// <summary>
        /// Массив буфера сетки
        /// </summary>
        protected float[] buffer = new float[0];

        /// <summary>
        /// Массив альфа буфера сетки 
        /// </summary>
        protected float[] bufferAlpha = new float[0];

        /// <summary>
        /// Изменения для рендера
        /// </summary>
        protected bool modifiedToRender = true;

        public ChunkRender(ChunkD chunk, WorldRender world)
        {
            Chunk = chunk;
            if (Chunk != null)
            {
                Chunk.Modified += ChunkModified;
                Chunk.Tag = this;
            }
            World = world;
        }

        private void ChunkModified(object sender, EventArgs e)
        {
            modifiedToRender = true;
        }

        public bool IsRender()
        {
            return !modifiedToRender;
            //return buffer.Length > 0;
            
        }

        

        /// <summary>
        /// Получить массив буфера сетки
        /// </summary>
        public float[] ToBuffer()
        {
            return buffer;
        }

        /// <summary>
        /// Очистить массив сетки
        /// </summary>
        public void ClearBuffer()
        {
            buffer = new float[0];
        }

        /// <summary>
        /// Получить массив буфера сетки альфа
        /// </summary>
        public float[] ToBufferAlpha()
        {
            return bufferAlpha;
        }

        public int CountBufferAlpha()
        {
            return bufferAlpha.Length;
        }


        /// <summary>
        /// Сгенерировать сетку меша
        /// </summary>
        public void Render()
        {
            List<float> bufferCache = new List<float>();
            _alphas = new List<VoxelData>();
            Camera camera = OpenGLF.GetInstance().Cam;

            for (int y = 0; y < 256; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        if (Chunk.GetVoxel(x, y, z).GetId() == 0) continue;
                        Block block = Chunk.GetBlock0(new vec3i(x, y, z));
                        if (block.IsAlphe)
                        {
                            _alphas.Add(new VoxelData()
                            {
                                Block = block,
                                //Vox = block.Voxel,
                                Distance = camera.DistanceTo(
                                    new vec3(Chunk.X << 4 | x, y, Chunk.Z << 4 | z)
                                    )
                            });
                        }
                        else
                        {
                            bufferCache.AddRange(_RenderVoxel(block));
                        }
                    }
                }
            }
            if (_alphas.Count > 0)
            {
                _alphas.Sort();
            }

            List<float> bufferAlpharCache = new List<float>();
            for (int i = _alphas.Count - 1; i >= 0; i--)
            {
                bufferAlpharCache.AddRange(_RenderVoxel(_alphas[i].Block));
            }
            modifiedToRender = false;
            bufferAlpha = bufferAlpharCache.ToArray();
            bufferAlpharCache.Clear();
            buffer = bufferCache.ToArray();
            bufferCache.Clear();
        }

        /// <summary>
        /// Сгенерировать сетку меша альфа
        /// </summary>
        public void RenderAlpha()
        {
            List<float> bufferAlpharCache = new List<float>();

            Camera camera = OpenGLF.GetInstance().Cam;
            Pole pole = camera.GetPole();

            foreach (VoxelData vd in _alphas)
            {
                vd.Distance = camera.DistanceTo(new vec3(vd.Block.Position.ToVec3i()));
            }

            if (_alphas.Count > 0)
            {
                _alphas.Sort();
            }

            for (int i = _alphas.Count - 1; i >= 0; i--)
            {
                bufferAlpharCache.AddRange(_RenderVoxel(_alphas[i].Block));
            }
            bufferAlpha = bufferAlpharCache.ToArray();
            bufferAlpharCache.Clear();
        }

        /// <summary>
        /// Рендер вокселя
        /// </summary>
        protected float[] _RenderVoxel(Block block)
        {
            BlockRender blockRender = new BlockRender(this, block);
            return blockRender.RenderMesh();
        }

        



    }
}

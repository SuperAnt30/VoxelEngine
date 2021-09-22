using VoxelEngine.Glm;
using System.Collections.Generic;
using VoxelEngine.Util;
using System;
using VoxelEngine.World.Chk;
using VoxelEngine.World.Blk;
using VoxelEngine.Renderer.Blk;
using VoxelEngine.Vxl;
using VoxelEngine.Graphics;

namespace VoxelEngine.Renderer.Chk
{
    /// <summary>
    /// Объект рендера чанков
    /// </summary>
    public class ChunkRender: ChunkHeir
    {
        /// <summary>
        /// Объект мира
        /// </summary>
        public WorldRender World { get; protected set; }
        /// <summary>
        /// Изменения для рендера
        /// true - нужен рендер
        /// </summary>
        protected bool modifiedToRender = true;

        public ChunkRender(ChunkBase chunk, WorldRender world) : base(chunk)
        {
            if (Chunk != null)
            {
                Chunk.Modified += ChunkModified;
                Chunk.ChunkTag = this;
            }
            World = world;
        }

        private void ChunkModified(object sender, EventArgs e)
        {
            ModifiedToRender();
        }

        /// <summary>
        /// Пометить что надо перерендерить сетку чанка
        /// </summary>
        public void ModifiedToRender()
        {
            modifiedToRender = true;
        }

        public bool IsRender()
        {
            return !modifiedToRender;
        }

        /// <summary>
        /// Количество блоков с альфо цветом в чанке
        /// </summary>
        public int CountBlockAlpha()
        {
            int count = 0;
            for (int i = 0; i < Chunk.StorageArrays.Length; i++)
            {
                count += Chunk.StorageArrays[i].Buffer.Alphas.Count;
            }
            return count;
        }

        /// <summary>
        /// Сгенерировать сетку меша
        /// </summary>
        public float[] Render()
        {
            Camera camera = OpenGLF.GetInstance().Cam;

            int yMax = Chunk.GetHighestHeight() >> 4;
            for (int i = 0; i <= yMax; i++)// Chunk.StorageArrays.Length; i++)
            {
                if (Chunk.StorageArrays[i].Buffer.IsModifiedRender)
                {
                    Chunk.StorageArrays[i].Buffer.Alphas.Clear();
                    int y0 = i * 16;
                    List<float> bufferCache = new List<float>();
                    for (int y = y0; y < y0 + 16; y++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            for (int x = 0; x < 16; x++)
                            {
                                if (Chunk.GetVoxel(x, y, z).GetEBlock() == EnumBlock.Air) continue;
                                Block block = Chunk.GetBlock0(new vec3i(x, y, z));
                                float[] buffer = _RenderVoxel(block);
                                if (block.IsAlphe)
                                {
                                    if (buffer.Length > 0)
                                    {
                                        Chunk.StorageArrays[i].Buffer.Alphas.Add(new VoxelData()
                                        {
                                            Block = block,
                                            Buffer = buffer,
                                            Distance = camera.DistanceTo(
                                                new vec3(Chunk.X << 4 | x, y, Chunk.Z << 4 | z)
                                                )
                                        });
                                    }
                                }
                                else
                                {
                                    bufferCache.AddRange(buffer);
                                }
                            }
                        }
                    }
                    Chunk.StorageArrays[i].Buffer.RenderDone(bufferCache.ToArray());
                    bufferCache.Clear();
                }
            }
            modifiedToRender = false;
            return GlueBuffer();
        }

        /// <summary>
        /// Склейка сетки
        /// </summary>
        protected float[] GlueBuffer()
        {
            int count = Chunk.StorageArrays.Length;
            int countAll = 0;
            for (int i = 0; i < count; i++)
            {
                countAll += Chunk.StorageArrays[i].Buffer.Buffer.Length;
            }

            float[] buffer = new float[countAll];
            countAll = 0;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < Chunk.StorageArrays[i].Buffer.Buffer.Length; j++)
                {
                    buffer[countAll] = Chunk.StorageArrays[i].Buffer.Buffer[j];
                    countAll++;
                }
            }
            return buffer;
        }

        /// <summary>
        /// Генерировать сетку альфы
        /// </summary>
        public float[] RenderAlpha()
        {
            List<VoxelData> alphas = new List<VoxelData>();

            // TODO:: можно оптемизировать в два массива, по удалению от персонажа, облегчим сортировку
            for (int i = 0; i < Chunk.StorageArrays.Length; i++)
            {
                if (Chunk.StorageArrays[i].Buffer.Alphas.Count > 0)
                {
                    foreach (VoxelData vd in Chunk.StorageArrays[i].Buffer.Alphas)
                    {
                        vd.Distance = World.Entity.HitBox.DistanceEyesTo(vd.Block.Position.ToVec3i());
                    }
                    alphas.AddRange(Chunk.StorageArrays[i].Buffer.Alphas);
                }
            }
            if (alphas.Count > 0)
            {
                alphas.Sort();
            }

            List<float> buffer = new List<float>();
            for (int i = alphas.Count - 1; i >= 0; i--)
            {
                buffer.AddRange(alphas[i].Buffer);
            }
            return buffer.ToArray();
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

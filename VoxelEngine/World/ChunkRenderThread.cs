using GlmNet;
using System.Collections.Generic;

namespace VoxelEngine
{
    /// <summary>
    /// Объект рендера чанков
    /// </summary>
    public class ChunkRenderThread
    {
        protected ChunkRender _chunk;

        public ChunkRenderThread(ChunkRender chunk)
        {
            _chunk = chunk;
        }

        public event ChunkEventHandler ThreadDone;

        /// <summary>
        /// Событие строка лога
        /// </summary>
        protected void OnDone(float[] buffer)
        {
            ThreadDone?.Invoke(this, new ChunkEventArgs(buffer));
        }

       // public List<float> buffer = new List<float>();

        public void Render()
        {
            List<float> buffer = new List<float>();
            Camera camera = OpenGLF.GetInstance().Cam;
            //Camera.Pole pole = camera.GetPole();
            //System.Threading.Thread.Sleep(100);
            List<VoxelData> items = new List<VoxelData>();
            for (int x = 0; x < VE.CHUNK_WIDTH; x++)
            {
                for (int z = 0; z < VE.CHUNK_WIDTH; z++)
                {
                    for (int y = 0; y < VE.CHUNK_HEIGHT; y++)
                    {
                        Voxel vox = WorldCache.GetInstance().GetChunk(_chunk.X, _chunk.Z).GetVoxel(x, y, z);
                        if (vox.Id == 0) continue;
                        items.Add(
                            new VoxelData()
                            {
                                X = x,
                                Y = y,
                                Z = z,
                                Distance = camera.DistanceTo(new vec3(
                                    _chunk.X * VE.CHUNK_WIDTH + x,
                                    y,
                                    _chunk.Z * VE.CHUNK_WIDTH + z)),
                                Vox = vox
                            }
                        );
                    }
                }
            }

            if (items.Count > 0)
            {
                items.Sort();
            }

            for (int i = items.Count - 1; i >= 0; i--)
            {
                buffer.AddRange(_RenderVoxel(items[i].X, items[i].Y, items[i].Z, items[i].Vox.Id));
            }

            OnDone(buffer.ToArray());
        }
             

        /// <summary>
        /// Рендер вокселя
        /// </summary>
        protected List<float> _RenderVoxel(int x, int y, int z, byte id)
        {
            List<float> buffer = new List<float>();
            float x0 = _chunk.X * VE.CHUNK_WIDTH + x;
            float z0 = _chunk.Z * VE.CHUNK_WIDTH + z;
            float u1 = (id % 16) * VE.UV_SIZE;
            float v2 = id / 16 * VE.UV_SIZE;
            //vec4 color = new vec4(1, 1, 1, 1);
            vec4 color = id == 143 ? new vec4(0.24f, 0.46f, 0.88f, 1f) : new vec4(0.45f, 0.69f, 0.38f, 1f);
            if (id == 3) color = new vec4(1, 1, 1, 1);
            BlockFaceUV block = new BlockFaceUV(
                new vec3(x0, y, z0),
                new vec3(x0 + 1f, y + 1f, z0 + 1f),
                new vec2(u1, v2 + VE.UV_SIZE),
                new vec2(u1 + VE.UV_SIZE, v2),
                color
            );

            if (!_IsBlocked(x, y + 1, z)) buffer.AddRange(block.Up(new vec4(1)));
            if (!_IsBlocked(x, y - 1, z)) buffer.AddRange(block.Down(new vec4(0.6f)));
            if (!_IsBlocked(x + 1, y, z)) buffer.AddRange(block.East(new vec4(0.9f)));
            if (!_IsBlocked(x - 1, y, z)) buffer.AddRange(block.West(new vec4(0.85f)));
            if (!_IsBlocked(x, y, z + 1)) buffer.AddRange(block.North(new vec4(0.75f)));
            if (!_IsBlocked(x, y, z - 1)) buffer.AddRange(block.South(new vec4(0.95f)));

            return buffer;
        }

        protected bool _IsBlocked(int x, int y, int z)
        {
            if (y < 0 || y >= VE.CHUNK_HEIGHT) return true; // true чтоб не видно было
            int xc = _chunk.X;
            int zc = _chunk.Z;
            int xv = x;
            int zv = z;
            if (x == -1)
            {
                xv = VE.CHUNK_WIDTH - 1;
                xc--;
            }
            else if (x == VE.CHUNK_WIDTH)
            {
                xv = 0;
                xc++;
            }
            if (z == -1)
            {
                zv = VE.CHUNK_WIDTH - 1;
                zc--;
            }
            else if (z == VE.CHUNK_WIDTH)
            {
                zv = 0;
                zc++;
            }
            return WorldCache.GetInstance().GetChunk(xc, zc).GetVoxel(xv, y, zv).Id != 0;
        }

    }
}

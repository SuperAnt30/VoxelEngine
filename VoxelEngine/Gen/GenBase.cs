using System;
using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Chk;

namespace VoxelEngine.Gen
{
    /// <summary>
    /// Объект для генерации объектов более одного блока
    /// </summary>
    public class GenBase : WorldHeir
    {
        /// <summary>
        /// Позиция в глобальном мире
        /// </summary>
        public vec3i Position { get; protected set; }

        protected GenBase() { }
        public GenBase(WorldBase world, vec3i pos) : base(world) => Position = pos;

        /// <summary>
        /// Поставить
        /// </summary>
        public virtual bool Put() => Put(new Random());

        /// <summary>
        /// Поставить
        /// </summary>
        public virtual bool Put(Random random) => false;

        /// <summary>
        /// Массив чанков используемых в генерации
        /// </summary>
        protected ChunkMap chunks = new ChunkMap();

        /// <summary>
        /// Задать блок на прямую к вокселю
        /// </summary>
        protected void SetBlockState(BlockPos pos, EnumBlock eBlock)
        {
            SetBlockState(pos, eBlock, 0);
        }

        /// <summary>
        /// Задать блок на прямую к вокселю
        /// </summary>
        protected void SetBlockState(BlockPos pos, EnumBlock eBlock, int properties)
        {
            int vx = pos.X & 15;
            int vz = pos.Z & 15;
            int cx = pos.X >> 4;
            int cz = pos.Z >> 4;

            if (World.IsChunk(cx, cz))
            {
                ChunkBase chunk = World.GetChunk(cx, cz);
                chunk.SetBlockState(vx, pos.Y, vz, eBlock);
                chunk.SetParam4bit(vx, pos.Y, vz, (byte)properties);
                if (eBlock != EnumBlock.Air && pos.Y > chunk.GetHeight(vx, vz))
                {
                    chunk.SetUpBlock(vx, pos.Y, vz);
                    chunk.PropagateSkylightOcclusion(vx, vz);
                }
                chunks.Set(chunk);
            }
        }

        /// <summary>
        /// Проверка освещение и прочего, по итогу как поставим
        /// </summary>
        protected void RecheckGaps()
        {
            foreach (ChunkBase chunk in chunks.Values)
            {
                chunk.RecheckGaps();
                chunk.SetChunkModified();
            }
        }

        /// <summary>
        /// Создает карту высот для блока с нуля
        /// </summary>
        protected void GenerateHeightMap()
        {
            foreach (ChunkBase chunk in chunks.Values)
            {
                chunk.GenerateHeightMap();
            }
        }
    }
}

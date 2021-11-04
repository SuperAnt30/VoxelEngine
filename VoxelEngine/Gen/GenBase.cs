using System;
using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Chk;
using VoxelEngine.World.Chk.Light;

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
        /// <summary>
        /// Объект обработки освещения
        /// </summary>
        protected WorkingLight light;
        /// <summary>
        /// Объект облости изменения
        /// </summary>
        protected RangeModified modified;
        /// <summary>
        /// Надо ли обрабатывать освещение
        /// </summary>
        public bool IsLight { get; protected set; } = false;

        protected GenBase() { }
        public GenBase(WorldBase world, vec3i pos) : base(world)
        {
            Position = pos;
            modified = new RangeModified(world, new BlockPos(pos));
        }

        public void Light()
        {
            IsLight = true;
            light = new WorkingLight(World.GetChunk(Position.x >> 4, Position.z >> 4));
        }

        /// <summary>
        /// Поставить
        /// </summary>
        public virtual bool Put() => Put(new Random());
        
        /// <summary>
        /// Поставить
        /// </summary>
        public virtual bool Put(Random random) => false;
        
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
                // Проверка освещения
                if (IsLight)
                {
                    chunk.Light.CheckLightSetBlock(pos, Blocks.GetBlockLightOpacity(eBlock), light);
                }
                else
                {
                    modified.BlockModify(pos);
                }
            }
        }

        /// <summary>
        /// Проверка освещение и прочего, по итогу как поставим
        /// </summary>
        protected void ModifiedRender()
        {
            if (IsLight)
            {
                light.ModifiedRender();
            } else
            {
                modified.ModifiedRender();
            }
        }
    }
}

using VoxelEngine.Glm;
using VoxelEngine.Util;
using VoxelEngine.World;
using VoxelEngine.World.Blk;
using VoxelEngine.World.Blk.Model;
using VoxelEngine.World.Chk;

namespace VoxelEngine.Gen.Group
{
    /// <summary>
    /// Объект базовой группы для создание модели из блоков
    /// </summary>
    public class GroupBase : GenBase
    {
        /// <summary>
        /// Размер группы
        /// </summary>
        public vec3i Size { get; protected set; } = new vec3i(1);
        /// <summary>
        /// Тип группы
        /// </summary>
        public EnumGroup Type { get; protected set; } = EnumGroup.None;
        /// <summary>
        /// Параметры группы 16 bit
        /// </summary>
        protected ushort properties;

        protected GroupBase() { }
        public GroupBase(WorldBase world, vec3i pos) : base(world, pos) { }

        /// <summary>
        /// Задать параметры
        /// </summary>
        public virtual void SetProperties(ushort prop) => properties = prop;
        /// <summary>
        /// Получить параметры
        /// </summary>
        public virtual ushort GetProperties() => properties;

        /// <summary>
        /// Проверка можно ли поставить
        /// </summary>
        protected bool Check(vec3i pos)
        {
            vec3i max = pos + Size;
            for (int x = pos.x; x < max.x; x++)
            {
                for (int z = pos.z; z < max.z; z++)
                {
                    for (int y = pos.y; y < max.y; y++)
                    {
                        BlockBase block = World.GetBlock(new BlockPos(x, y, z));
                        if (block == null || !block.IsAir)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Добавить группу
        /// </summary>
        public void SetGroupChunk(BlockPos pos)
        {
            int cx = pos.X >> 4;
            int cz = pos.Z >> 4;
            if (World.IsChunk(cx, cz))
            {
                ChunkBase chunk = World.GetChunk(cx, cz);
                SetGroupChunk(chunk, pos);
            }
        }

        /// <summary>
        /// Добавить группу в текущий чанк без проверки
        /// </summary>
        public void SetGroupChunk(ChunkBase chunk, BlockPos pos)
        {
            chunk.GroupModel.Set(new vec3i(pos.X & 15, pos.Y, pos.Z & 15), this);
        }

        /// <summary>
        /// Загрузить с файла
        /// </summary>
        public virtual void LoadBin(ChunkBase chunk, ushort prop)
        {
            SetProperties(prop);
        }

        /// <summary>
        /// Запрос загрузки
        /// </summary>
        public virtual void RequestLoad(ChunkBase chunk) { }

        /// <summary>
        /// Удалить группу
        /// </summary>
        protected void RemoveGroupChunk(BlockPos pos)
        {
            int cx = pos.X >> 4;
            int cz = pos.Z >> 4;
            if (World.IsChunk(cx, cz))
            {
                ChunkBase chunk = World.GetChunk(cx, cz);
                chunk.GroupModel.Remove(new vec3i(pos.X & 15, pos.Y, pos.Z & 15));
            }
        }

        /// <summary>
        /// Получить массив боксов от вокселя
        /// </summary>
        /// <param name="prop">параметры вокселя который прорисовываем</param>
        public virtual Box[] Box(int prop) => new Box[0];

        /// <summary>
        /// Обновить колизию блока
        /// </summary>
        public virtual void CollisionRefrash(BlockBase block) { }

        /// <summary>
        /// Действие блоков, клик по модельному блоку
        /// </summary>
        public virtual void Action() { }
    }
}

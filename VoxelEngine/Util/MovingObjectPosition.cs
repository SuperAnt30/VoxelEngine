using VoxelEngine.Entity;
using VoxelEngine.Glm;
using VoxelEngine.World.Blk;

namespace VoxelEngine.Util
{
    /// <summary>
    /// Объект отвечающий какой объект попадает под лучь курсора
    /// </summary>
    public class MovingObjectPosition
    {
        /// <summary>
        /// Объект сущьности
        /// </summary>
        public EntityLiving Entity { get; protected set; }
        /// <summary>
        /// Объект блока
        /// </summary>
        public BlockBase Block { get; protected set; }

        public vec3 End { get; protected set; }
        public vec3i Norm { get; protected set; }
        public vec3i IEnd { get; protected set; }

        protected MovingObjectType type = MovingObjectType.None;

        public MovingObjectPosition() { }

        public MovingObjectPosition(BlockBase block, vec3 end, vec3i iend, vec3i norm)
        {
            Block = block;
            End = end;
            IEnd = iend;
            Norm = norm;
            type = MovingObjectType.Block;
        }
        public MovingObjectPosition(BlockBase block)
        {
            Block = block;
            type = MovingObjectType.Block;
        }
        public MovingObjectPosition(EntityLiving entity)
        {
            Entity = entity;
            type = MovingObjectType.Entity;
        }

        public bool IsBlock()
        {
            return type == MovingObjectType.Block;
        }

        public bool IsEntity()
        {
            return type == MovingObjectType.Entity;
        }

        /// <summary>
        /// Тип объекта
        /// </summary>
        protected enum MovingObjectType
        {
            None = 0,
            Block = 1,
            Entity = 2
        }
    }
}

using VoxelEngine.Entity;
using VoxelEngine.Graphics;

namespace VoxelEngine.Renderer.Entity
{
    /// <summary>
    /// Сетка буфера сущностей
    /// </summary>
    public class EntityMesh : RenderMesh
    {
        protected override int[] _Attrs { get; } = new int[] { 3, 2 };

        /// <summary>
        /// Порядковый номер сущьности
        /// </summary>
        public int Index { get; protected set; }

        /// <summary>
        /// Тип конкретного моба
        /// </summary>
        public EnumEntity Key { get; protected set; }

        public EntityMesh(int index, EnumEntity key)
        {
            Index = index;
            Key = key;
        }
    }
}

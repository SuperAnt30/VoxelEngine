using VoxelEngine.Graphics;

namespace VoxelEngine.Renderer.Entity
{
    /// <summary>
    /// Сетка буфера сущностей
    /// </summary>
    public class EntityMesh : RenderMesh
    {
        protected override int[] _Attrs { get; } = new int[] { 3, 2 };
    }
}

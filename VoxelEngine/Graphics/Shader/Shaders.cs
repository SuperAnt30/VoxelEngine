using SharpGL;
using System.Collections.Generic;

namespace VoxelEngine.Graphics.Shader
{
    /// <summary>
    /// Колекция шейдеров
    /// </summary>
    public class Shaders
    {
        /// <summary>
        /// Шейдер шрифтов
        /// </summary>
        public ShaderFont ShFont { get; protected set; } = new ShaderFont();

        /// <summary>
        /// Шейдер вокселей
        /// </summary>
        public ShaderVoxel ShVoxel { get; protected set; } = new ShaderVoxel();

        /// <summary>
        /// Шейдер сущностей
        /// </summary>
        public ShaderEntity ShEntity { get; protected set; } = new ShaderEntity();

        /// <summary>
        /// Шейдер линий
        /// </summary>
        public ShaderLine ShLine { get; protected set; } = new ShaderLine();

        /// <summary>
        /// Шейдер скайблока
        /// </summary>
        public ShaderSkyBox ShSkyBox { get; protected set; } = new ShaderSkyBox();

        public void Create(OpenGL gl)
        {
            ShFont.Create(gl, new Dictionary<uint, string> { { 0, "v_position" }, { 1, "v_texCoord" }, { 2, "v_color" } });
            ShVoxel.Create(gl, new Dictionary<uint, string> { { 0, "v_position" }, { 1, "v_texCoord" }, { 2, "v_color" } });
            ShEntity.Create(gl, new Dictionary<uint, string> { { 0, "v_position" }, { 1, "v_texCoord" } });
            //ShEntity.Create(gl, new Dictionary<uint, string> { { 0, "v_position" }, { 1, "v_texCoord" }, { 2, "v_color" } });
            ShLine.Create(gl, new Dictionary<uint, string> { { 0, "v_position" }, { 1, "v_color" } });
            ShSkyBox.Create(gl, new Dictionary<uint, string> { { 0, "v_position" } });
        }
    }
}

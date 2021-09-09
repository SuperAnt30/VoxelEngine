namespace VoxelEngine.Graphics.Shader
{
    /// <summary>
    /// Шейдер сущностей
    /// </summary>
    public class ShaderEntity : ShaderVE
    {
        protected override string _VertexShaderSource { get; } = @"#version 330 core
layout(location = 0) in vec3 v_position;
layout(location = 1) in vec2 v_texCoord;

out vec4 a_color;
out vec2 a_texCoord;

uniform mat4 projection;
uniform mat4 lookat;
uniform float light;

void main()
{
    gl_Position = projection * lookat * vec4(v_position, 1.0);
    a_color = vec4(light, light, light, 1);
    a_texCoord = v_texCoord;
}";
        //0.3f
        protected override string _FragmentShaderSource { get; } = @"#version 330 core
 
in vec4 a_color;
in vec2 a_texCoord;
out vec4 f_color;

uniform sampler2D u_texture0;

void main(){
    vec4 tex_color = texture(u_texture0, a_texCoord);
	if (tex_color.a < 0.1) 
	discard;
    f_color = a_color * tex_color;
}";
    }
}

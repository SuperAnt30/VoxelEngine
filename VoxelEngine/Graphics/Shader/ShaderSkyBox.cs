namespace VoxelEngine.Graphics.Shader
{
    /// <summary>
    /// Шейдер Скайбокс 
    /// </summary>
    public class ShaderSkyBox : ShaderVE
    {
        protected override string _VertexShaderSource { get; } = @"#version 330 core
layout (location = 0) in vec3 v_position;

out vec3 a_position;

uniform mat4 projection;
uniform mat4 view;


void main()
{
    a_position = v_position;
    
    gl_Position = projection * view * vec4(v_position, 1.0);
}";
        //0.3f
        protected override string _FragmentShaderSource { get; } = @"#version 330 core
out vec4 FragColor;

in vec3 a_position;

uniform samplerCube skybox;
uniform float light;

void main()
{    
    FragColor = texture(skybox, a_position);
    FragColor.r = FragColor.r * light;
    FragColor.g = FragColor.g * light;
    FragColor.b = FragColor.b * light;
    FragColor.a = FragColor.a;
}";
    }
}

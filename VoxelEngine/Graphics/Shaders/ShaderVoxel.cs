namespace VoxelEngine
{
    /// <summary>
    /// Шейдер кубиков
    /// </summary>
    public class ShaderVoxel : ShaderVE
    {
        protected override string _VertexShaderSource { get; } = @"#version 330 core
#extension GL_EXT_gpu_shader4 : enable

layout(location = 0) in vec3 v_position;
layout(location = 1) in vec2 v_texCoord;
layout(location = 2) in vec3 v_color;
layout(location = 3) in vec2 v_light;

out vec4 a_color;
out vec2 a_texCoord;
out vec4 a_position;

uniform mat4 projection;
uniform mat4 lookat;
uniform float light;

float light2;
float lightS;

void main()
{
    //b = (a_color.a & 0xF);
    //ls = (a_color.a & 0xF) / 15.0;
    //lb = ((a_color.a & 0xF0) >> 4) / 15.0;
    //lb = a_color.a / 15.0;
    a_position = projection * lookat * vec4(v_position, 1.0);
    
    lightS = v_light.y * light;
    //light2 = light * v_color.a;
    //light2 = (light > v_color.a) ? light + 0.2 : v_color.a + 0.2;
    light2 = (lightS > v_light.x) ? lightS + 0.2 : v_light.x + 0.2;
    //light2 = light * v_color.a + 0.2;
    if (light2 > 1.0) light2 = 1.0;
    a_color.r = v_color.r * light2;
    a_color.g = v_color.g * light2;
    a_color.b = v_color.b * light2;
    a_color.a = 1.0;
    a_texCoord = v_texCoord;
    gl_Position = a_position;
}";
        //0.3f
        protected override string _FragmentShaderSource { get; } = @"#version 330 core
 
in vec4 a_color;
in vec4 a_position;
in vec2 a_texCoord;
out vec4 f_color;

uniform sampler2D u_texture0;

void main(){
	vec4 tex_color = texture(u_texture0, a_texCoord);
	if (tex_color.a < 0.1) 
	discard;
    vec4 color = a_color * tex_color;
    if (a_position.z > 0) {
      //  color.rgb *= 1.0 - (a_position.z - 64) * 0.01;
        //if (f_color.r >= 0.3 && color.r < 0.3) color.r = 0.3;
        //if (f_color.g >= 0.3 && color.g < 0.3) color.g = 0.3;
        //if (f_color.b >= 0.3 && color.b < 0.3) color.b = 0.3;
        //color.rgb += 0.5;
        //f_color.rgb += 0.3;
        //f_color = vec4(0.3, 0.3, 0.3, 1.0);
        //f_color.r = 0.3;
        //f_color.g = 0.3;
        //f_color.b = 0.3;
    }
    f_color = color;
}";
    }
}

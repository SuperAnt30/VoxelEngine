namespace VoxelEngine.Graphics.Shader
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
out float a_light;
out float fog_factor;

uniform mat4 projection;
uniform mat4 lookat;
uniform float light;
uniform float length;
uniform vec3 camera;

void main()
{
    a_position = projection * lookat * vec4(v_position, 1.0);
    float camera_distance = distance(camera, vec3(v_position));
    fog_factor = pow(clamp(camera_distance / length, 0.0, 1.0), 4.0);
    float lightS = v_light.y * light;
    float light2 = (lightS > v_light.x) ? lightS + 0.2 : v_light.x + 0.2;
    if (light2 > 1.0) light2 = 1.0;
    a_color.r = v_color.r * light2;
    a_color.g = v_color.g * light2;
    a_color.b = v_color.b * light2;
    a_color.a = 1.0;
    a_texCoord = v_texCoord;
    gl_Position = a_position;
    a_light = light;
}";
        //0.3f
        protected override string _FragmentShaderSource { get; } = @"#version 330 core
 
in vec4 a_color;
in vec4 a_position;
in vec2 a_texCoord;
in float a_light;
in float fog_factor;

out vec4 f_color;

uniform sampler2D sky_sampler;
uniform sampler2D atlas;

void main(){
	vec4 tex_color = texture(atlas, a_texCoord);
	if (tex_color.a < 0.1) discard;
    vec4 color = a_color * tex_color;
    vec3 col3 = vec3(color);
    //vec4 sky_color = texture(sky_sampler, vec2(0.07, 0.07));
    vec3 cols = vec3(0.73, 0.83, 1.0);
    cols.r = cols.r * a_light;
    cols.g = cols.g * a_light;
    cols.b = cols.b * a_light;
    col3 = mix(col3, cols, fog_factor);
    f_color = vec4(col3, color.a);
}";
    }
}

#version 330

layout(location = 0) in vec4 position;
layout(location = 1) in vec3 vNormal;
layout(location = 2) in vec2 uv0;
layout(location = 3) in vec2 uv1;
layout(location = 4) in vec4 color;

in vec3 vProbePosition;

uniform mat4 mtxMdl;
uniform mat4 mtxCam;

uniform vec4 gsys_bake_st0;
uniform vec4 gsys_bake_st1;

out vec2 f_texcoord0;
out vec2 f_texcoord1;
out vec2 f_texcoord2;

out vec4 vertexColor;
out vec3 normal;
out vec3 probePosition;
out vec3 fragPosition;

void main(){
    f_texcoord0 = uv0;
    f_texcoord1 = uv1;
    vertexColor = vec4(1,1,1,1);
    normal = vNormal;
    probePosition = vProbePosition;
    fragPosition = vec3(position.xyz);

    vec4 sampler2 = gsys_bake_st0;
    vec4 sampler3 = gsys_bake_st1;

	if (sampler2.x != 0 && sampler2.y != 0)
        f_texcoord1 = vec2((uv1 * sampler2.xy) + sampler2.zw);
     else
        f_texcoord1 = vec2((uv1 * vec2(1)) + sampler2.zw);

	if (sampler3.x != 0 && sampler3.y != 0)
        f_texcoord2 = vec2((uv1 * sampler3.xy) + sampler3.zw);
     else
        f_texcoord2 = vec2((uv1 * vec2(1)) + sampler3.zw);

    gl_Position = mtxCam*mtxMdl*position;
}
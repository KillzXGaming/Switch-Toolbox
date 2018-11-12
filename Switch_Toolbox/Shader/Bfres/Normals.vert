#version 330 core

in vec3 vPosition;
in vec3 vNormal;
in vec3 vTangent;
in vec3 vBitangent;
in vec2 vUV0;
in vec4 vColor;
in vec4 vBone;
in vec4 vWeight;
in vec2 vUV1;
in vec2 vUV2;
in vec3 vPosition2;
in vec3 vPosition3;

out VS_OUT {
    vec3 normal;
} vs_out;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

void main()
{
    gl_Position = projection * view * model * vec4(vPosition, 1.0); 
    mat3 normalMatrix = mat3(transpose(inverse(view * model)));
    vs_out.normal = normalize(vec3(projection * vec4(normalMatrix * vNormal, 0.0)));
}
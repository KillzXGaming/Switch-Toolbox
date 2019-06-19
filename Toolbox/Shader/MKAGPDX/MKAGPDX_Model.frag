#version 330
in vec3 normal;
in vec3 position;

in vec2 f_texcoord0;
in vec2 f_texcoord1;
in vec2 f_texcoord2;
in vec2 f_texcoord3;
in vec4 vertexColor;
in vec3 tangent;

in vec3 boneWeightsColored;

uniform vec3 difLightDirection;
uniform vec3 difLightColor;
uniform vec3 ambLightColor;


uniform int colorOverride;
uniform int renderType;
uniform int renderVertColor;
uniform mat4 modelview;

uniform int HasDiffuse;

uniform sampler2D DiffuseMap;
uniform sampler2D UVTestPattern;

out vec4 FragColor;

void main()
{

    vec3 displayNormal = (normal.xyz * 0.5) + 0.5;
    FragColor = vec4(displayNormal.rgb,1);
}

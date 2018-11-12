#version 110

varying vec2 f_texcoord0;
varying vec2 f_texcoord1;
varying vec2 f_texcoord2;
varying vec2 f_texcoord3;

varying vec3 objectPosition;

varying vec3 normal;
varying vec3 viewNormal;
varying vec4 vertexColor;
varying vec3 tangent;
varying vec3 bitangent;

varying vec3 boneWeightsColored;

uniform sampler2D DiffuseMap;


uniform int isTransparent;

#define gamma 2.2

void main()
{
   vec4 fragColor = vec4(vec3(0), 1);
	vec4 diffuseMapColor = vec4(texture2D(DiffuseMap, f_texcoord0).rgb, 1);
    fragColor.rgb += diffuseMapColor.rgb;
    fragColor.a *= texture2D(DiffuseMap, f_texcoord0).a;

	if (isTransparent != 1)
        fragColor.a = 1.0;

    gl_FragColor = fragColor;
}

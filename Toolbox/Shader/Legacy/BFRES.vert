#version 110

uniform mat4 mvpMatrix;

attribute vec3 vPosition;
attribute vec3 vNormal;
attribute vec3 vTangent;
attribute vec3 vBitangent;
attribute vec2 vUV0;
attribute vec4 vColor;
attribute vec4 vBone;
attribute vec4 vWeight;
attribute vec2 vUV1;
attribute vec2 vUV2;
attribute vec3 vPosition2;
attribute vec3 vPosition3;

varying vec2 f_texcoord0;
varying vec2 f_texcoord1;
varying vec2 f_texcoord2;
varying vec2 f_texcoord3;

varying vec3 normal;
varying vec4 vertexColor;
varying vec3 tangent;
varying vec3 bitangent;
  
// Shader Options
uniform vec4 gsys_bake_st0;
uniform vec4 gsys_bake_st1;

// Skinning uniforms
uniform mat4 bones[200];
 //Meshes have a bone index and will use their transform depending on skin influence amount
uniform mat4 singleBoneBindTransform;
uniform int NoSkinning;
uniform int RigidSkinning;

void main()
{
    gl_Position = mvpMatrix * vec4(vPosition.xyz, 1.0);
    normal = vNormal;
    f_texcoord0 = vUV0;
    f_texcoord1 = vUV1;
    f_texcoord2 = vUV1;
    f_texcoord3 = vUV2;
	tangent = vTangent;
	bitangent = vBitangent;
    vertexColor = vColor;
}
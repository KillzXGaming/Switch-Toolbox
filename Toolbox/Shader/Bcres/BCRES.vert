#version 330
 
const int MY_ARRAY_SIZE = 200;

in vec3 vPosition;
in vec3 vNormal;
in vec3 vTangent;
in vec2 vUV0;
in vec4 vColor;
in vec4 vBone;
in vec4 vWeight;
in vec2 vUV1;
in vec2 vUV2;

out vec2 f_texcoord0;
out vec2 f_texcoord1;
out vec2 f_texcoord2;
out vec2 f_texcoord3;

out vec3 normal;
out vec4 color;
out vec3 position;

// Skinning uniforms
uniform mat4 bones[190];

uniform mat4 mtxCam;
uniform mat4 mtxMdl;
uniform mat4 previewScale;

vec4 skin(vec3 pos, ivec4 index)
{
    vec4 newPosition = vec4(pos.xyz, 1.0);

    newPosition = bones[index.x] * vec4(pos, 1.0) * vWeight.x;
    newPosition += bones[index.y] * vec4(pos, 1.0) * vWeight.y;
    newPosition += bones[index.z] * vec4(pos, 1.0) * vWeight.z;
    if (vWeight.w < 1) //Necessary. Bones may scale weirdly without
		newPosition += bones[index.w] * vec4(pos, 1.0) * vWeight.w;

    return newPosition;
}

vec3 skinNRM(vec3 nr, ivec4 index)
{
    vec3 newNormal = vec3(0);

	newNormal = mat3(bones[index.x]) * nr * vWeight.x;
	newNormal += mat3(bones[index.y]) * nr * vWeight.y;
	newNormal += mat3(bones[index.z]) * nr * vWeight.z;
	newNormal += mat3(bones[index.w]) * nr * vWeight.w;

    return newNormal;
}

void main()
{
    ivec4 index = ivec4(vBone);

    vec4 objPos = vec4(vPosition.xyz, 1.0);

	vec4 position = mtxCam  * mtxMdl *  vec4(objPos.xyz, 1.0);

    normal = vNormal;
    color = vColor;
	position = objPos;
	f_texcoord0 = vUV0;
	f_texcoord1 = vUV1;
	f_texcoord2 = vUV2;

    gl_Position = mtxCam * mtxMdl * vec4(vPosition.xyz, 1.0);

    vec3 distance = (vPosition.xyz + vec3(5, 5, 5))/2;

}
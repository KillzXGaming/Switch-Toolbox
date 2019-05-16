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

uniform mat4 mtxProj;
uniform mat4 mtxCam;
uniform mat4 mtxMdl;
uniform mat4 camMtx;

// Skinning uniforms
uniform mat4 bones[200];

uniform int NoSkinning;
uniform int RigidSkinning;

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

	vec3 normal = vNormal;

    vec4 objPos = vec4(vPosition.xyz, 1.0);
	if (vBone.x != -1.0)
		objPos = skin(vPosition, index);

	if(vBone.x != -1.0)
		normal = normalize((skinNRM(vNormal.xyz, index)).xyz);


    if (RigidSkinning == 1)
    {
	     objPos = (bones[index.x] * vec4(vPosition, 1.0));
         normal = vNormal;
		 normal = mat3(bones[index.x]) * vNormal.xyz * 1;
	}

	normal = normalize(mat3(mtxMdl) * normal.xyz);

    gl_Position = mtxCam * mtxMdl * vec4(objPos.xyz, 1.0); 
    mat3 normalMatrix = mat3(transpose(inverse(camMtx * mtxMdl)));
    vs_out.normal = normalize(vec3(mtxProj * vec4(normalMatrix * normal, 0.0)));
}
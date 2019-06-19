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
out vec4 vertexColor;
out vec3 position;
out vec3 tangent;

out vec3 boneWeightsColored;

uniform int boneIds[190];

// Skinning uniforms
uniform mat4 bones[190];

uniform mat4 mtxCam;
uniform mat4 mtxMdl;
uniform mat4 previewScale;

// Bone Weight Display
uniform sampler2D weightRamp1;
uniform sampler2D weightRamp2;
uniform int selectedBoneIndex;
uniform int debugOption;

uniform int NoSkinning;
uniform int RigidSkinning;
uniform int SingleBoneIndex;

vec2 ST0_Translate;
float ST0_Rotate;
vec2 ST0_Scale;

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

vec3 BoneWeightColor(float weights)
{
	float rampInputLuminance = weights;
	rampInputLuminance = clamp((rampInputLuminance), 0.001, 0.999);
    if (debugOption == 1) // Greyscale
        return vec3(weights);
    else if (debugOption == 2) // Color 1
	   return texture(weightRamp1, vec2(1 - rampInputLuminance, 0.50)).rgb;
    else // Color 2
        return texture(weightRamp2, vec2(1 - rampInputLuminance, 0.50)).rgb;
}

float BoneWeightDisplay(ivec4 index)
{
    float weight = 0;
    if (selectedBoneIndex == boneIds[index.x])
        weight += vWeight.x;
    if (selectedBoneIndex == boneIds[index.y])
        weight += vWeight.y;
    if (selectedBoneIndex == boneIds[index.z])
        weight += vWeight.z;
    if (selectedBoneIndex == boneIds[index.w])
        weight += vWeight.w;

    if (selectedBoneIndex == boneIds[index.x] && RigidSkinning == 1)
        weight = 1;
   if (selectedBoneIndex == SingleBoneIndex && NoSkinning == 1)
        weight = 1;

    return weight;
}

void main()
{
    ivec4 index = ivec4(vBone);

    vec4 objPos = vec4(vPosition.xyz, 1.0);
	if (vBone.x != -1.0)
		objPos = skin(vPosition, index);
	if(vBone.x != -1.0)
		normal = normalize((skinNRM(vNormal.xyz, index)).xyz);

	vec4 position = mtxCam  * mtxMdl *  vec4(objPos.xyz, 1.0);

    normal = vNormal;
    vertexColor = vColor;
	position = objPos;
	f_texcoord0 = vUV0;
	f_texcoord1 = vUV1;
	f_texcoord2 = vUV2;
	tangent = vTangent;

    gl_Position = mtxCam * mtxMdl * vec4(vPosition.xyz, 1.0);

	float totalWeight = BoneWeightDisplay(index);
    boneWeightsColored = BoneWeightColor(totalWeight).rgb;

}
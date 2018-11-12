#version 330

uniform mat4 mvpMatrix;

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

out vec2 f_texcoord0;
out vec2 f_texcoord1;
out vec2 f_texcoord2;
out vec2 f_texcoord3;

out vec3 objectPosition;

out vec3 normal;
out vec3 viewNormal;
out vec4 vertexColor;
out vec3 tangent;
out vec3 bitangent;

out vec3 boneWeightsColored;

// Shader Options
uniform vec4 gsys_bake_st0;
uniform vec4 gsys_bake_st1;

// Skinning uniforms
uniform mat4 bones[200];
 //Meshes have a bone index and will use their transform depending on skin influence amount
uniform mat4 singleBoneBindTransform;
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

vec2 rotateUV(vec2 uv, float rotation)
{
    float mid = 0.5;
    return vec2(
        cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
        cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
    );
}

void main()
{
    ivec4 index = ivec4(vBone);

    vec4 objPos = vec4(vPosition.xyz, 1.0);
    objectPosition = vPosition.xyz;
	if (vBone.x != -1.0)
		objPos = skin(vPosition, index);

    gl_Position = mvpMatrix * vec4(objPos.xyz, 1.0);

    normal = vNormal;

	if(vBone.x != -1.0)
		normal = normalize((skinNRM(vNormal.xyz, index)).xyz);



    if (RigidSkinning == 1)
    {
	     gl_Position = mvpMatrix * bones[index.x] * vec4(vPosition, 1.0);
         normal = vNormal;
		 normal = mat3(bones[index.x]) * vNormal.xyz * 1;
	}
	if (NoSkinning == 1)
    {
	    gl_Position = mvpMatrix * singleBoneBindTransform * vec4(vPosition, 1.0);
	    normal = mat3(singleBoneBindTransform) * vNormal.xyz * 1;
		normal = normalize(normal);
	}

    f_texcoord0 = vUV0;

    vec4 sampler2 = gsys_bake_st0;
    vec4 sampler3 = gsys_bake_st1;

	if (sampler2.x != 0 && sampler2.y != 0)
        f_texcoord1 = vec2((vUV1 * sampler2.xy) + sampler2.zw);
     else
        f_texcoord1 = vec2((vUV1 * vec2(1)) + sampler2.zw);

	if (sampler3.x != 0 && sampler3.y != 0)
        f_texcoord2 = vec2((vUV1 * sampler3.xy) + sampler3.zw);
     else
        f_texcoord2 = vec2((vUV1 * vec2(1)) + sampler3.zw);

    f_texcoord3 = vUV2;

	tangent = vTangent;
	bitangent = vBitangent;
    vertexColor = vColor;
}

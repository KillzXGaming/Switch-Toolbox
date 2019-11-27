#version 330
in vec3 objectPosition;
in vec2 f_texcoord0;
in vec2 f_texcoord1;
in vec2 f_texcoord2;
in vec2 f_texcoord3;


in vec3 normal;
in vec4 vertexColor;
in vec3 tangent;
in vec3 binormal;

in vec3 boneWeightsColored;

// Viewport Camera/Lighting
uniform mat4 mtxCam;
uniform mat4 mtxMdl;

uniform vec3 specLightDirection;
uniform vec3 difLightDirection;
uniform mat4 projMatrix;
uniform mat4 normalMatrix;
uniform mat4 modelViewMatrix;
uniform mat4 rotationMatrix;

uniform int colorOverride;
uniform int renderType;
uniform int renderVertColor;
uniform mat4 modelview;
uniform int uvChannel;

// Texture Samplers
uniform sampler2D DiffuseMap;
uniform sampler2D BakeShadowMap;
uniform sampler2D NormalMap;
uniform sampler2D BakeLightMap;
uniform sampler2D UVTestPattern;
uniform sampler2D EmissionMap;
uniform sampler2D SpecularMap;
uniform sampler2D DiffuseLayer;
uniform sampler2D MetalnessMap;
uniform sampler2D RoughnessMap;
uniform sampler2D ProjectionMap;
uniform sampler2D SphereMap;

// Texture Map Toggles
uniform int HasDiffuse;
uniform int HasNormalMap;
uniform int HasSpecularMap;
uniform int HasShadowMap;
uniform int HasAmbientOcclusionMap;
uniform int HasLightMap;
uniform int HasEmissionMap;
uniform int HasDiffuseLayer;
uniform int HasMetalnessMap;
uniform int HasRoughnessMap;
uniform int HasProjectionMap;

// Diffuse Channel Toggles
uniform int RedChannel;
uniform int GreenChannel;
uniform int BlueChannel;
uniform int AlphaChannel;

uniform samplerCube irradianceMap;
uniform samplerCube specularIbl;
uniform sampler2D brdfLUT;

uniform int isTransparent;

uniform int HasNoNormals;

out vec4 fragColor;

struct VertexAttributes {
    vec3 objectPosition;
    vec2 texCoord;
    vec2 texCoord2;
    vec2 texCoord3;
    vec4 vertexColor;
    vec3 normal;
    vec3 viewNormal;
    vec3 tangent;
    vec3 binormal;
	};

#define gamma 2.2
const float PI = 3.14159265359;

// Shader code adapted from learnopengl.com's PBR tutorial:
// https://learnopengl.com/PBR/Theory

//In Utility.frag
float GetComponent(int Type, vec4 Texture);

void main()
{
    fragColor = vec4(0);

    // Create a struct for passing all the vertex attributes to other functions.
    VertexAttributes vert;
    vert.objectPosition = objectPosition;
    vert.texCoord = f_texcoord0;
    vert.texCoord2 = f_texcoord1;
    vert.texCoord3 = f_texcoord2;
    vert.vertexColor = vertexColor;
    vert.normal = normal;
    vert.tangent = tangent;
    vert.binormal = binormal;

	float specIntensity = 1;

    // Wireframe color.
    if (colorOverride == 1)
    {
        fragColor = vec4(1);
        return;
    }

	vec3 albedo = vec3(0.5);
	float alpha = 1.0f;
    if (HasDiffuse == 1)
	{
		vec4 DiffuseTex =  pow(texture(DiffuseMap, f_texcoord0).rgba, vec4(gamma));

		//Comp Selectors
		albedo.r = GetComponent(RedChannel, DiffuseTex);
		albedo.g = GetComponent(GreenChannel, DiffuseTex);
		albedo.b = GetComponent(BlueChannel, DiffuseTex);
	    alpha = GetComponent(AlphaChannel, DiffuseTex);
	}

	   // Diffuse lighting.
	   if (HasNoNormals == 0)
	   {
	   	    float halfLambert = dot(difLightDirection, normal) * 0.5 + 0.5;
 	        albedo *= halfLambert;
	   }

    fragColor.rgb += albedo.rgb;
    fragColor.rgb = pow(fragColor.rgb, vec3(1 / gamma));

    if (renderVertColor == 1)
        fragColor *= min(vert.vertexColor, vec4(1));

     // Global brightness adjustment.
	// fragColor.rgb *= 1.5;

    fragColor.a = 1;
	if (isTransparent == 1)
	{
	    fragColor.a = alpha;
	}

	 //Debug Shading
	vec2 displayTexCoord =  f_texcoord0;

	if (uvChannel == 1)
		displayTexCoord =  f_texcoord0;
	if (uvChannel == 2)
		displayTexCoord =  f_texcoord1;
	if (uvChannel == 3)
		displayTexCoord =  f_texcoord2;

    vec3 displayNormal = (normal.xyz * 0.5) + 0.5;

	// Diffuse lighting.
	if (HasNoNormals == 1)
	{
	   	displayNormal = vec3(1);
	}

    if (renderType == 1) // normals color
        fragColor = vec4(displayNormal.rgb,1);
	else if (renderType == 2)
	{
	    float halfLambert = dot(difLightDirection, normal) * 0.5 + 0.5;
        fragColor = vec4(vec3(halfLambert), 1);
	}
	else if (renderType == 3) //DiffuseColor
	{
		//Comp Selectors
		vec4 diffuseMapColor = vec4(texture(DiffuseMap, displayTexCoord).rgb, 1);
		diffuseMapColor.r = GetComponent(RedChannel, diffuseMapColor);
		diffuseMapColor.g = GetComponent(GreenChannel, diffuseMapColor);
		diffuseMapColor.b = GetComponent(BlueChannel, diffuseMapColor);

	    fragColor = vec4(diffuseMapColor.rgb, 1);
	}
	else if (renderType == 4) //Display Normal
         fragColor.rgb = texture(NormalMap, displayTexCoord).rgb;
    else if (renderType == 5) // vertexColor
        fragColor = vertexColor;
	else if (renderType == 6) //Display Ambient Occlusion
        fragColor = vec4(1);
   else if (renderType == 7) // uv coords
        fragColor = vec4(displayTexCoord.x, displayTexCoord.y, 1, 1);
    else if (renderType == 8) // uv test pattern
	{
        fragColor = vec4(texture(UVTestPattern, displayTexCoord).rgb, 1);
	}
    else if (renderType == 9) //Display tangents
    {
        vec3 displayTangent = (tangent * 0.5) + 0.5;
        if (dot(tangent, vec3(1)) == 0)
            displayTangent = vec3(0);

        fragColor = vec4(displayTangent,1);
    }
    else if (renderType == 10) //Display binormals
    {
        vec3 displayBinormal = (binormal * 0.5) + 0.5;
        if (dot(binormal, vec3(1)) == 0)
            displayBinormal = vec3(0);

        fragColor = vec4(displayBinormal,1);
    }
    else if (renderType == 12)
    {
        fragColor.rgb = boneWeightsColored;
    }
}

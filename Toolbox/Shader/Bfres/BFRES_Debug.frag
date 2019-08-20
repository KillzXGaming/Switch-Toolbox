#version 330

in vec2 f_texcoord0;
in vec2 f_texcoord1;
in vec2 f_texcoord2;
in vec2 f_texcoord3;

in vec3 objectPosition;

in vec3 normal;
in vec3 viewNormal;
in vec4 vertexColor;
in vec3 tangent;
in vec3 bitangent;

in vec3 boneWeightsColored;

// Viewport Camera/Lighting
uniform mat4 mtxMdl;
uniform mat4 mtxCam;

uniform mat4 projMatrix;
uniform mat4 normalMatrix;
uniform mat4 modelViewMatrix;
uniform mat4 rotationMatrix;

uniform int useImageBasedLighting;
uniform int enableCellShading;

uniform vec3 camPos;

uniform vec3 light1Pos;

const float levels = 3.0;

// Viewport Settings
uniform int uvChannel;
uniform int renderType;
uniform int useNormalMap;
uniform vec4 colorSamplerUV;
uniform int renderVertColor;
uniform vec3 difLightColor;
uniform vec3 ambLightColor;
uniform int colorOverride;
uniform float DefaultMetalness;
uniform float DefaultRoughness;

// Channel Toggles
uniform int renderR;
uniform int renderG;
uniform int renderB;
uniform int renderAlpha;

// Texture Samplers
uniform sampler2D DiffuseMap;
uniform sampler2D BakeShadowMap;
uniform sampler2D NormalMap;
uniform sampler2D BakeLightMap;
uniform sampler2D UVTestPattern;
uniform sampler2D TransparencyMap;
uniform sampler2D EmissionMap;
uniform sampler2D SpecularMap;
uniform sampler2D DiffuseLayer;
uniform sampler2D MetalnessMap;
uniform sampler2D RoughnessMap;
uniform sampler2D MRA;
uniform sampler2D TeamColorMap;
uniform sampler2D SphereMap;
uniform sampler2D SubSurfaceScatteringMap;

uniform samplerCube irradianceMap;
uniform samplerCube specularIbl;
uniform sampler2D brdfLUT;

// Shader Params
uniform float normal_map_weight;
uniform float ao_density;
uniform float emission_intensity;
uniform vec4 fresnelParams;
uniform vec4 base_color_mul_color;
uniform vec3 emission_color;
uniform vec3 specular_color;

// Shader Options
uniform float uking_texture2_texcoord;
uniform float bake_shadow_type;
uniform float enable_fresnel;
uniform float enable_emission;
uniform float cSpecularType;

uniform int UseSpecularColor;

// Texture Map Toggles
uniform int HasDiffuse;
uniform int HasNormalMap;
uniform int HasSpecularMap;
uniform int HasShadowMap;
uniform int HasAmbientOcclusionMap;
uniform int HasLightMap;
uniform int HasTransparencyMap;
uniform int HasEmissionMap;
uniform int HasDiffuseLayer;
uniform int HasMetalnessMap;
uniform int HasRoughnessMap;
uniform int HasMRA;
uniform int HasSubSurfaceScatteringMap;

uniform int roughnessAmount;

uniform int UseAOMap;
uniform int UseCavityMap;
uniform int UseMetalnessMap;
uniform int UseRoughnessMap;

// Diffuse Channel Toggles
uniform int RedChannel;
uniform int GreenChannel;
uniform int BlueChannel;
uniform int AlphaChannel;

int isTransparent;

struct VertexAttributes {
    vec3 objectPosition;
    vec2 texCoord;
    vec2 texCoord2;
    vec2 texCoord3;
    vec4 vertexColor;
    vec3 normal;
    vec3 viewNormal;
    vec3 tangent;
    vec3 bitangent;
	};

out vec4 fragColor;

#define gamma 2.2

// Defined in Utility.frag.
//float Luminance(vec3 rgb);

// Defined in BFRES_Utility.frag.
vec3 CalcBumpedNormal(vec3 normal, sampler2D normalMap, VertexAttributes vert, float texCoordIndex);
//float AmbientOcclusionBlend(sampler2D BakeShadowMap, VertexAttributes vert, float ao_density);
//vec3 EmissionPass(sampler2D EmissionMap, float emission_intensity, VertexAttributes vert, float texCoordIndex, vec3 emission_color);

vec2 displayTexCoord =  f_texcoord0;

float GetComponent(int Type, vec4 Texture);

void main()
{
	if (uvChannel == 1)
		displayTexCoord =  f_texcoord0;
	if (uvChannel == 2)
		displayTexCoord =  f_texcoord2;
	if (uvChannel == 3)
		displayTexCoord =  f_texcoord3;

    fragColor = vec4(vec3(0), 1);

    // Create a struct for passing all the vertex attributes to other functions.
    VertexAttributes vert;
    vert.objectPosition = objectPosition;
    vert.texCoord = f_texcoord0;
    vert.texCoord2 = f_texcoord1;
    vert.texCoord3 = f_texcoord2;
    vert.vertexColor = vertexColor;
    vert.normal = normal;
    vert.viewNormal = viewNormal;
    vert.tangent = tangent;
    vert.bitangent = bitangent;

    // Wireframe color.
    if (colorOverride == 1)
    {
		if (renderVertColor == 1)
            fragColor = vec4(vertexColor);
		else
            fragColor = vec4(1);

        return;
    }

    vec3 N = normal;
	if (HasNormalMap == 1 && useNormalMap == 1)
		N = CalcBumpedNormal(normal, NormalMap, vert, uking_texture2_texcoord);

	float metallic = 0;
	float roughness = 1;
	vec3 specIntensity = vec3(1);
	float ao = 1;

	if (HasMRA == 1) //Kirby Star Allies PBR map
	{
	    //Note KSA has no way to tell if one gets unused or not because shaders :(
		//Usually it's just metalness with roughness and works fine
		metallic = texture(MRA, f_texcoord0).r;
		roughness = texture(MRA, f_texcoord0).g;
		ao = texture(MRA, f_texcoord0).b;
		specIntensity = vec3(texture(MRA, f_texcoord0).a);
	}
	else if (HasShadowMap == 1)
    {
		ao = texture(BakeShadowMap, f_texcoord1).r;
    }

	if (HasMetalnessMap == 1)
    {
		 metallic = texture(MetalnessMap, displayTexCoord).r;
    }
	if (HasRoughnessMap == 1)
    {
	    roughness = texture(RoughnessMap, displayTexCoord).r;
    }
	if (HasSpecularMap == 1)
    {
	if (uking_texture2_texcoord == 1)
            specIntensity = texture(SpecularMap, f_texcoord1).rgb;
		else
            specIntensity = texture(SpecularMap, f_texcoord0).rgb;
    }

    if (renderType == 1) // normals vertexColor
    {
        vec3 displayNormal = (N * 0.5) + 0.5;
        fragColor = vec4(displayNormal,1);
    }
    else if (renderType == 2) // Lighting
    {
        vec3 I = vec3(0,0,-1) * mat3(mtxCam);
        vec3 V = normalize(I); // view
    	float light = max(dot(N, V), 0.0);
        fragColor = vec4(vec3(light), 1);
    }
	else if (renderType == 4) //Display Normal
	{
		if (uking_texture2_texcoord == 1)
            fragColor.rgb = texture(NormalMap, f_texcoord1).rgb;
		else
            fragColor.rgb = texture(NormalMap, displayTexCoord).rgb;
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
    else if (renderType == 5) // vertexColor
        fragColor = vertexColor;
	else if (renderType == 6) //Display Ambient Occlusion
	{
        fragColor = vec4(vec3(ao), 1);
	}
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
    else if (renderType == 10) //Display bitangents
    {
        vec3 displayBitangent = (bitangent * 0.5) + 0.5;
        if (dot(bitangent, vec3(1)) == 0)
            displayBitangent = vec3(0);

        fragColor = vec4(displayBitangent,1);
    }

    else if (renderType == 12)
    {
        fragColor.rgb = boneWeightsColored;
    }
	else if (renderType == 11) //Light map
    {
	    if (HasLightMap == 1)
        {
		    vec4 lightMap = texture(BakeLightMap, f_texcoord2);
            fragColor = vec4(lightMap.rgb, 1);
        }
		else
        {
            fragColor = vec4(1);
        }
    }
	else if (renderType == 13) //Specular
	{
	    if (UseSpecularColor == 1)
		    fragColor = vec4(specIntensity.rgb, 1);
		else
		  fragColor = vec4(vec3(specIntensity.r), 1);
	}
	else if (renderType == 14) //Shadow
	{
	    if (HasShadowMap == 1)
        {
		    float Shadow = texture(BakeShadowMap, f_texcoord1).g;
            fragColor = vec4(vec3(Shadow), 1);
        }
		else
        {
            fragColor = vec4(1);
        }
	}
	else if (renderType == 15) //MetalnessMap
    {
        fragColor = vec4(vec3(metallic), 1);
    }
	else if (renderType == 16) //RoughnessMap
    {
        fragColor = vec4(vec3(roughness), 1);
    }
	else if (renderType == 17) //SubSurfaceScatteringMap
    {
	    if (HasSubSurfaceScatteringMap == 1)
        {
		    vec3 sss = texture(SubSurfaceScatteringMap, displayTexCoord).rgb;
            fragColor = vec4(sss, 1);
        }
		else
        {
            fragColor = vec4(1);
        }
    }
	else if (renderType == 18) //EmmissionMap
    {
	    if (HasEmissionMap == 1)
        {
		    vec3 emm = texture(EmissionMap, displayTexCoord).rgb;
            fragColor = vec4(emm, 1);
        }
		else
        {
            fragColor = vec4(1);
        }
    }

    // Toggles rendering of individual color channels for all render modes.
    fragColor.rgb *= vec3(renderR, renderG, renderB);
    if (renderR == 1 && renderG == 0 && renderB == 0)
        fragColor.rgb = fragColor.rrr;
    else if (renderG == 1 && renderR == 0 && renderB == 0)
        fragColor.rgb = fragColor.ggg;
    else if (renderB == 1 && renderR == 0 && renderG == 0)
        fragColor.rgb = fragColor.bbb;

}

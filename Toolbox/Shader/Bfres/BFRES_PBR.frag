#version 330

in vec2 f_texcoord0;
in vec2 f_texcoord1;
in vec2 f_texcoord2;
in vec2 f_texcoord3;

in vec3 objectPosition;

in vec3 normal;
in vec4 vertexColor;
in vec3 tangent;
in vec3 bitangent;

in vec3 boneWeightsColored;

// Viewport Camera/Lighting
uniform mat4 mtxCam;
uniform mat4 mtxMdl;

uniform mat4 projMatrix;
uniform mat4 normalMatrix;
uniform mat4 modelViewMatrix;
uniform mat4 rotationMatrix;

uniform vec4 pickingColor;

uniform int useImageBasedLighting;
uniform int enableCellShading;

uniform vec3 camPos;

uniform vec3 light1Pos;

const float levels = 3.0;

// Viewport Settings
uniform vec3 specLightDirection;

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
uniform sampler2D BOTWSpecularMap;
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

// Shader Options
uniform float bake_light_type;
uniform float bake_calc_type;

// Texture Map Toggles
uniform int HasDiffuse;
uniform int HasNormalMap;
uniform int HasSpecularMap;
uniform int HasShadowMap;
uniform int HasAmbientOcclusionMap;
uniform int HasLightMap;
uniform int HasTransparencyMap;
uniform int HasTransmissionMap;
uniform int HasEmissionMap;
uniform int HasDiffuseLayer;
uniform int HasMetalnessMap;
uniform int HasRoughnessMap;
uniform int HasMRA;
uniform int HasBOTWSpecularMap;
uniform int HasSubSurfaceScatteringMap;

uniform int roughnessAmount;

uniform int UseAOMap;
uniform int UseCavityMap;
uniform int UseMetalnessMap;
uniform int UseRoughnessMap;

uniform int isTransparent;

// Diffuse Channel Toggles
uniform int RedChannel;
uniform int GreenChannel;
uniform int BlueChannel;
uniform int AlphaChannel;

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
const float PI = 3.14159265359;

struct BakedData
{
	float shadowIntensity;
	float aoIntensity;
	vec3 indirectLighting;
};

// Defined in BFRES_Utility.frag.

BakedData ShadowMapBaked(sampler2D ShadowMap, sampler2D LightMap, vec2 texCoordBake0, vec2 texCoordBake1, int ShadowType, int LightType, int CalcType, vec3 NormalMap);

vec3 CalcBumpedNormal(vec3 normal, sampler2D normalMap, VertexAttributes vert, float texCoordIndex);
//float AmbientOcclusionBlend(sampler2D BakeShadowMap, VertexAttributes vert, float ao_density);
vec3 EmissionPass(sampler2D EmissionMap, float emission_intensity, VertexAttributes vert, float texCoordIndex, vec3 emission_color);

// Shader code adapted from learnopengl.com's PBR tutorial:
// https://learnopengl.com/PBR/Theory

vec3 FresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

vec3 FresnelSchlickRoughness(float cosTheta, vec3 F0, float roughness)
{
    return F0 + (max(vec3(1.0 - roughness), F0) - F0) * pow(1.0 - cosTheta, 5.0);
}

float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a      = roughness*roughness;
    float a2     = a*a;
    float NdotH  = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;

    float num   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float num   = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return num / denom;
}

float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2  = GeometrySchlickGGX(NdotV, roughness);
    float ggx1  = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

vec3 saturation(vec3 rgb, float adjustment)
{
    const vec3 W = vec3(0.2125, 0.7154, 0.0721);
    vec3 intensity = vec3(dot(rgb, W));
    return mix(intensity, rgb, adjustment);
}

float GetComponent(int Type, vec4 Texture);

void main()
{
    bool RenderAsLighting = renderType == 2;

    fragColor = vec4(1);

    // Create a struct for passing all the vertex attributes to other functions.
    VertexAttributes vert;
    vert.objectPosition = objectPosition;
    vert.texCoord = f_texcoord0;
    vert.texCoord2 = f_texcoord1;
    vert.texCoord3 = f_texcoord2;
    vert.vertexColor = vertexColor;
    vert.normal = normal;
    vert.tangent = tangent;
    vert.bitangent = bitangent;

    vec3 lightColor = vec3(10);

    // Wireframe color.
    if (colorOverride == 1)
    {
        fragColor = vec4(1);
        return;
    }

	vec3 albedo = vec3(1);
    if (HasDiffuse == 1)
	{
		vec4 DiffuseTex =  pow(texture(DiffuseMap, f_texcoord0).rgba, vec4(gamma));

		//Comp Selectors
		albedo.r = GetComponent(RedChannel, DiffuseTex);
		albedo.g = GetComponent(GreenChannel, DiffuseTex);
		albedo.b = GetComponent(BlueChannel, DiffuseTex);
	}

	float metallic = 0;
    if (HasMetalnessMap == 1)
        metallic = texture(MetalnessMap, f_texcoord0).r;

	float roughness = 0.5;
    if (HasRoughnessMap == 1)
        roughness = texture(RoughnessMap, f_texcoord0).r;

	float ao = 1;
    if (HasShadowMap == 1 && bake_shadow_type == 0 || UseAOMap == 1)
        ao = texture(BakeShadowMap, f_texcoord1).r;

	float shadow = 1;
    if (HasShadowMap == 1 && bake_shadow_type == 1)
        shadow = texture(BakeShadowMap, f_texcoord1).g;

	float cavity = 1;

	vec3 emissionTerm = vec3(0);
    if (HasEmissionMap == 1 || enable_emission == 1) //Can be without texture map
		emissionTerm.rgb += EmissionPass(EmissionMap, emission_intensity, vert, 0, emission_color);

	vec3 lightMapColor = vec3(1);
	float lightMapIntensity = 0;
    if (HasLightMap == 1)
    {
        lightMapColor = texture(BakeLightMap, f_texcoord1).rgb;
        lightMapIntensity = texture(BakeLightMap, f_texcoord1).a;
    }

	if (RenderAsLighting)
	{
	     metallic = 0;
		 roughness = 1;
	}

	float specIntensity = 1;

	if (HasMRA == 1) //Kirby Star Allies PBR map
	{
	    //Note KSA has no way to tell if one gets unused or not because shaders :(
		//Usually it's just metalness with roughness and works fine
		metallic = texture(MRA, f_texcoord0).r;
		roughness = texture(MRA, f_texcoord0).g;
		ao = texture(MRA, f_texcoord0).b;
		specIntensity = texture(MRA, f_texcoord0).a;
	}

    // Calculate shading vectors.
    vec3 I = vec3(0,0,-1) * mat3(mtxCam);
    vec3 N = normal;
	if (HasNormalMap == 1 && useNormalMap == 1)
		N = CalcBumpedNormal(normal, NormalMap, vert, 0);

    vec3 V = normalize(I); // view
	vec3 L = normalize(specLightDirection ); // Light
	vec3 H = normalize(specLightDirection + I); // half angle
    vec3 R = reflect(I, N); // reflection

    vec3 f0 = mix(vec3(0.04), albedo, metallic); // dialectric
    vec3 kS = FresnelSchlickRoughness(max(dot(N, H), 0.0), f0, roughness);
    

    BakedData ShadowBake = ShadowMapBaked(BakeShadowMap,BakeLightMap, f_texcoord1, f_texcoord2, int(bake_shadow_type),int(bake_light_type), int(bake_calc_type), N );

	vec3 LightingDiffuse = vec3(0);
	if (HasLightMap == 1)
	{
	    vec3 LightIntensity = vec3(0.1);
		LightingDiffuse += ShadowBake.indirectLighting.rgb * LightIntensity;
	}

//	shadow *= ShadowBake.shadowIntensity;
//	ao *= ShadowBake.aoIntensity;

    // Diffuse pass
    vec3 diffuseIblColor = texture(irradianceMap, N).rgb;
    vec3 diffuseTerm = diffuseIblColor * albedo;
    diffuseTerm *= cavity;
    diffuseTerm *= ao;
    diffuseTerm *= shadow;
    diffuseTerm += LightingDiffuse;

    // Adjust for metalness.
    diffuseTerm *= clamp(1 - metallic, 0, 1);
    diffuseTerm *= vec3(1) - kS.xxx;
//
    // Specular pass.
    int maxSpecularLod = 8;
    vec3 specularIblColor = textureLod(specularIbl, R, roughness * maxSpecularLod).rgb;


    vec2 envBRDF  = texture(brdfLUT, vec2(max(dot(N, V), 0.0), roughness)).rg;
    vec3 brdfTerm = (kS * envBRDF.x + envBRDF.y);
   // vec3 specularTerm = specularIblColor * (kS * brdfTerm.x + brdfTerm.y) * specIntensity;
 //   vec3 specularTerm = specularIblColor * brdfTerm * specIntensity;
    vec3 specularTerm = specularIblColor * kS;


    // Add render passes.
    fragColor.rgb = vec3(0);
    fragColor.rgb += diffuseTerm;
    fragColor.rgb += specularTerm;
    fragColor.rgb += emissionTerm;

    // Global brightness adjustment.
    fragColor.rgb *= 2.5;

    fragColor *= min(pickingColor, vec4(1));

    fragColor.rgb *= min(boneWeightsColored, vec3(1));

    // HDR tonemapping
    fragColor.rgb = fragColor.rgb / (fragColor.rgb + vec3(1.0));

    // Convert back to sRGB.
    fragColor.rgb = pow(fragColor.rgb, vec3(1 / gamma));

    // Alpha calculations.
	if (isTransparent == 1)
	{
		 float alpha = GetComponent(AlphaChannel, texture(DiffuseMap, f_texcoord0));
		 fragColor.a = alpha;
	 }

    if (renderVertColor == 1)
        fragColor *= min(vert.vertexColor, vec4(1));

	if (RenderAsLighting)
	{
	    diffuseIblColor = texture(irradianceMap, N).rgb;
	    diffuseTerm = diffuseIblColor * vec3(0.5);
		diffuseTerm *= cavity;
		diffuseTerm *= ao;
		diffuseTerm *= shadow;
		diffuseTerm += LightingDiffuse;

		// Adjust for metalness.
		diffuseTerm *= clamp(1 - metallic, 0, 1);
		diffuseTerm *= vec3(1) - kS.xxx;

	    fragColor.rgb = vec3(0);
        fragColor.rgb += diffuseTerm;
	    fragColor.rgb += specularTerm;
	    fragColor.rgb += emissionTerm;
		 // Global brightness adjustment.
		 fragColor.rgb *= 1.5;
	    // HDR tonemapping
		fragColor.rgb = fragColor.rgb / (fragColor.rgb + vec3(1.0));
        // Convert back to sRGB.
		fragColor.rgb = pow(fragColor.rgb, vec3(1 / gamma));
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

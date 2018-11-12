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
uniform mat4 mvpMatrix;
uniform vec3 specLightDirection;
uniform vec3 difLightDirection;
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
uniform sampler2D tex0;
uniform sampler2D BakeShadowMap;
uniform sampler2D normalMap;
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

uniform int roughnessAmount;

uniform int UseAOMap;
uniform int UseCavityMap;
uniform int UseMetalnessMap;
uniform int UseRoughnessMap;

int isTransparent;

uniform int renderDiffuse;
uniform int renderSpecular;
uniform int renderFresnel;

struct VertexAttributes
{
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

// Defined in BFRES_Utility.frag.
vec3 CalcBumpedNormal(vec3 normal, sampler2D normalMap, VertexAttributes vert, float uking_texture2_texcoord);
float AmbientOcclusionBlend(sampler2D BakeShadowMap, VertexAttributes vert, float ao_density);
vec3 EmissionPass(sampler2D EmissionMap, float emission_intensity, VertexAttributes vert, float uking_texture2_texcoord, vec3 emission_color);

// Shader code adapted from learnopengl.com's PBR tutorial:
// https://learnopengl.com/PBR/Theory

vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

vec3 fresnelSchlickRoughness(float cosTheta, vec3 F0, float roughness)
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

vec3 DiffusePass(vec3 albedo, vec3 N, vec3 L, vec3 R)
{
    float lambert = max(dot(N, L), 0.0);

    // Higher blend values make the dark region smoother and larger.
    float smoothness = 0.1;
    float center = 0.5;
    float edgeL = center;
    float edgeR = center + (smoothness * 0.5);
    float smoothLambert = smoothstep(edgeL, edgeR, lambert);

    float ambient = 0.6;
    smoothLambert = clamp(smoothLambert + ambient, 0, 1);

    vec3 diffuseTerm = albedo;
    diffuseTerm *= smoothLambert;

    return diffuseTerm * 1.5;
}

vec3 SpecularPass(vec3 albedo, vec3 N, vec3 H, vec3 R, float metallic, float specularMapIntensity)
{
    // Specular pass
    vec3 specularTerm = vec3(0);

    // TODO: Metalness
    vec3 specularColor = albedo;

    specularTerm = specularColor;

    // Hack something together for now.
    vec3 specularLighting = texture(irradianceMap, R).rrr;
    float center = 0.375;
    float smoothness = 0.035;
    specularLighting = smoothstep(vec3(center - smoothness), vec3(center + smoothness), specularLighting);

    specularTerm *= specularLighting;
    specularTerm *= specularMapIntensity;

    return specularTerm;
}

vec3 FresnelPass(vec3 N, vec3 I)
{
    // Fake edge lighting
    float nDotI = clamp(dot(N, I), 0, 1);
    float fresnel = 1 - nDotI;

    // TODO: Extract cel shade function.
    float center = 0.75;
    float smoothness = 0.015;
    fresnel = smoothstep(center - smoothness, center + smoothness, fresnel);

    vec3 fresnelTerm = vec3(1, 1, 0.75) * fresnel * 0.2;
    return fresnelTerm;
}

void main()
{
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

	vec3 albedo = pow(texture(tex0, f_texcoord0).rgb, vec3(gamma));

	float metallic = DefaultMetalness;
    if (HasMetalnessMap == 1)
        metallic = texture(MetalnessMap, f_texcoord0).r;

	float roughness = DefaultRoughness;
    if (HasRoughnessMap == 1)
        roughness = texture(RoughnessMap, f_texcoord0).r;

	float ao = 1;
    if (HasShadowMap == 1 && UseAOMap == 1)
        ao = texture(BakeShadowMap, f_texcoord1).r;

	float shadow = 1;
    if (HasShadowMap == 1)
        shadow = texture(BakeShadowMap, f_texcoord1).g;

	float cavity = 1;

	vec3 lightMapColor = vec3(1);
	float lightMapIntensity = 0;
    if (HasLightMap == 1)
    {
        lightMapColor = texture(BakeLightMap, f_texcoord1).rgb;
        lightMapIntensity = texture(BakeLightMap, f_texcoord1).a;
    }

    // TODO: Extract function.
	float specularMapIntensity = 1;
	if (HasSpecularMap == 1)
	{
	   if (uking_texture2_texcoord == 1)
	   {
	       metallic = texture(SpecularMap, f_texcoord1).g;
	       specularMapIntensity = texture(SpecularMap, f_texcoord1).r;
	   }
	   else
	   {
	       metallic = texture(SpecularMap, f_texcoord0).g;
	       specularMapIntensity = texture(SpecularMap, f_texcoord0).r;
	   }
	}

    vec3 I = vec3(0,0,-1) * mat3(mvpMatrix);

    vec3 N = normal;
	if (HasNormalMap == 1 && useNormalMap == 1)
		N = CalcBumpedNormal(normal, normalMap, vert, uking_texture2_texcoord);

    vec3 V = normalize(I); //Eye View
	vec3 L = normalize(specLightDirection); //Light
	vec3 H = normalize(specLightDirection + I); //Half Angle
    vec3 R = reflect(I, N); // reflection

    // Render passes
	vec3 outputColor = vec3(0);
    float kDiffuse = clamp(1.0 - metallic, 0, 1);
    outputColor += DiffusePass(albedo, N, L, R) * renderDiffuse;
    outputColor += SpecularPass(albedo, N, H, R, metallic, specularMapIntensity) * renderSpecular;
    outputColor += FresnelPass(N, I) * renderFresnel;
    if (HasEmissionMap == 1 || enable_emission == 1) //Can be without texture map
        outputColor.rgb += EmissionPass(EmissionMap, emission_intensity, vert, uking_texture2_texcoord, emission_color);

    outputColor *= ao;
    outputColor *= (0.6 + shadow);

    float cavityStrength = 1.0;
    outputColor *= cavity * cavityStrength + (1.0 - cavityStrength);

    // TODO: Renders as black?
    // if (renderVertColor == 1)
    //     fragColor *= min(vertexColor, vec4(1));

    outputColor = pow(outputColor, vec3(1.0 / gamma));
    float alpha = texture(tex0, f_texcoord0).a;
	fragColor = vec4(outputColor, alpha);
}

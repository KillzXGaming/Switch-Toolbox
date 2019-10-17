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

uniform sampler2D DiffuseMap;
uniform sampler2D BakeShadowMap;
uniform sampler2D SpecularMap;
uniform sampler2D NormalMap;
uniform sampler2D BakeLightMap;
uniform sampler2D UVTestPattern;
uniform sampler2D TransparencyMap;
uniform sampler2D EmissionMap;
uniform sampler2D DiffuseLayer;
uniform sampler2D MetalnessMap;
uniform sampler2D RoughnessMap;
uniform sampler2D MRA;
uniform sampler2D BOTWSpecularMap;
uniform sampler2D SphereMap;
uniform sampler2D SubSurfaceScatteringMap;

// Viewport Camera/Lighting
uniform mat4 mtxCam;
uniform mat4 mtxMdl;

uniform vec3 cameraPosition;

// Viewport Settings
uniform vec3 difLightDirection;
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

uniform vec4 pickingColor;

uniform int isTransparent;

uniform vec3 materialSelectColor;

// Shader Params
uniform float normal_map_weight;
uniform float ao_density;
uniform float shadow_density;
uniform float emission_intensity;
uniform vec4 fresnelParams;
uniform vec4 base_color_mul_color;
uniform vec3 emission_color;
uniform vec3 specular_color;

uniform vec4 const_color0;
uniform vec3 albedo_tex_color;

// Shader Options
uniform float uking_texture2_texcoord;
uniform float bake_shadow_type;
uniform float bake_light_type;

uniform float bake_calc_type;

uniform float enable_fresnel;
uniform float enable_emission;
uniform float cSpecularType;
uniform float cIsEnableNormalMap;

uniform int UseSpecularColor;
uniform int UseMultiTexture;

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

// Diffuse Channel Toggles
uniform int RedChannel;
uniform int GreenChannel;
uniform int BlueChannel;
uniform int AlphaChannel;

// Channel Toggles
uniform int renderR;
uniform int renderG;
uniform int renderB;
uniform int renderAlpha;

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

struct BakedData
{
	float shadowIntensity;
	float aoIntensity;
	vec3 indirectLighting;
};

// Defined in Utility.frag.
float Luminance(vec3 rgb);

// Defined in BFRESTurboShadow.frag.
BakedData ShadowMapBaked(sampler2D ShadowMap, sampler2D LightMap, vec2 texCoordBake0, vec2 texCoordBake1, int ShadowType, int LightType, int CalcType, vec3 NormalMap);

// Defined in BFRES_Utility.frag.
vec3 CalcBumpedNormal(vec3 normal, sampler2D normalMap, VertexAttributes vert, float texCoordIndex);
float AmbientOcclusionBlend(sampler2D BakeShadowMap, VertexAttributes vert, float ao_density);
vec3 EmissionPass(sampler2D EmissionMap, float emission_intensity, VertexAttributes vert, float texCoordIndex, vec3 emission_color);
vec3 SpecularPass(vec3 I, vec3 normal, int HasSpecularMap, sampler2D SpecularMap, vec3 specular_color, VertexAttributes vert, float texCoordIndex, int UseSpecularColor);
vec3 ReflectionPass(vec3 N, vec3 I, vec4 diffuseMap, vec3 Specular, float aoBlend, vec3 tintColor, VertexAttributes vert);

float GetComponent(int Type, vec4 Texture);

void main()
{
    fragColor = vec4(vec3(0), 1);

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

	int NormalMapUVIndex = 0;

	if (uking_texture2_texcoord == 1 || cIsEnableNormalMap == 1)
		NormalMapUVIndex = 1;

    vec3 N = normal;
	if (HasNormalMap == 1 && useNormalMap == 1)
		N = CalcBumpedNormal(normal, NormalMap, vert, NormalMapUVIndex);

    // Calculate shading vectors.
    vec3 I = vec3(0,0,-1) * mat3(mtxCam);
    vec3 V = normalize(I); // view
    vec3 R = reflect(-I, N); // reflection
    float halfLambert = dot(difLightDirection, N) * 0.5 + 0.5;

    // Light Map
    vec4 LightMapColor = texture(BakeLightMap, f_texcoord2);

    BakedData ShadowBake = ShadowMapBaked(BakeShadowMap,BakeLightMap, f_texcoord1, f_texcoord2, int(bake_shadow_type),int(bake_light_type), int(bake_calc_type), N );

	vec4 diffuseMapColor = vec4(texture(DiffuseMap, f_texcoord0).rgba);
	vec4 albedo = vec4(0);
	//Comp Selectors
	albedo.r = GetComponent(RedChannel, diffuseMapColor);
	albedo.g = GetComponent(GreenChannel, diffuseMapColor);
	albedo.b = GetComponent(BlueChannel, diffuseMapColor);
	albedo.a = GetComponent(AlphaChannel, diffuseMapColor);

   //Texture Overlay (Like an emblem in mk8)
	if (UseMultiTexture == 1 && HasDiffuseLayer == 1)
	{
		vec4 AlbLayer = vec4(texture(DiffuseLayer, f_texcoord3).rgba);
		albedo.rgb = mix(albedo.rgb, AlbLayer.rgb, AlbLayer.a);
	}

    // Default Shader
    vec4 alpha = texture2D(DiffuseMap, f_texcoord0).aaaa;

    if (HasTransparencyMap == 1)
    {
        // TODO: Finish this
        alpha = texture2D(TransparencyMap, f_texcoord0).rgba;
        alpha *= 0.5;
    }

	albedo *= halfLambert;

	vec3 LightingDiffuse = vec3(0);
	if (HasLightMap == 1)
	{
	    vec3 LightIntensity = vec3(0.1);
		LightingDiffuse += ShadowBake.indirectLighting.rgb * LightIntensity;
	}

    float ShadowPass = 1;
    float AoPass = 1;

	if (HasShadowMap == 1)
	{
		float aoBlend = 0;
		aoBlend += 1.0 - ShadowBake.aoIntensity;
		float shadowBlend = 0;
		shadowBlend += 1.0 - ShadowBake.shadowIntensity;

		 ShadowPass *= 1.0 - shadowBlend * shadow_density * 0.5;
		 AoPass *= 1.0 - aoBlend * ao_density * 0.6;
	}

	albedo.rgb += LightingDiffuse;

    vec3 LightDiffuse = vec3(0.03);

    fragColor.rgb += albedo.rgb;
    fragColor.rgb *= ShadowPass;
    fragColor.rgb *= AoPass;

    vec3 color = vec3(1);
    vec3 normal = texture(NormalMap, f_texcoord0).rgb;
    vec3 specular = vec3(0);
	if (HasSpecularMap == 1){
		specular = texture(SpecularMap, f_texcoord0).rgb;
	}

	vec3 tintColor = vec3(1);

	   // Render Passes
    if (HasEmissionMap == 1 || enable_emission == 1) //Can be without texture map
		fragColor.rgb += EmissionPass(EmissionMap, emission_intensity, vert, 0, emission_color);
	fragColor.rgb += SpecularPass(I, N, HasSpecularMap, SpecularMap, specular_color, vert, 0, UseSpecularColor);
    fragColor.rgb += ReflectionPass(N, I, albedo, specular, AoPass, tintColor, vert);

	fragColor.rgb *= pickingColor.rgb;

	if (isTransparent == 1)
		fragColor.a *= albedo.a;

    fragColor.rgb *= min(boneWeightsColored, vec3(1));

    if (renderVertColor == 1)
        fragColor *= min(vert.vertexColor, vec4(1));

    fragColor *= min(vec4(albedo_tex_color, 1), vec4(1));

    fragColor.rgb += materialSelectColor * 0.5f;

    // Toggles rendering of individual color channels for all render modes.
    fragColor.rgb *= vec3(renderR, renderG, renderB);
    if (renderR == 1 && renderG == 0 && renderB == 0)
        fragColor.rgb = fragColor.rrr;
    else if (renderG == 1 && renderR == 0 && renderB == 0)
        fragColor.rgb = fragColor.ggg;
    else if (renderB == 1 && renderR == 0 && renderG == 0)
        fragColor.rgb = fragColor.bbb;
}

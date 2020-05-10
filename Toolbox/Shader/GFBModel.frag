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
uniform sampler2D AmbientMap;
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
uniform int HasAmbientMap;
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

//Parameters
uniform float ColorUVScaleU;
uniform float ColorUVScaleV;
uniform float ColorUVTranslateU;
uniform float ColorUVTranslateV;
uniform float ColorBaseU;
uniform float ColorBaseV;

uniform float NormalMapUVScaleU;
uniform float NormalMapUVScaleV;
uniform float NormalMapUVTranslateU;
uniform float NormalMapUVTranslateV;

uniform samplerCube irradianceMap;
uniform samplerCube specularIbl;
uniform sampler2D brdfLUT;

int isTransparent;

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

//Defined in PBR Utilty
vec3 FresnelSchlick(float cosTheta, vec3 F0);
vec3 FresnelSchlickRoughness(float cosTheta, vec3 F0, float roughness);
float DistributionGGX(vec3 N, vec3 H, float roughness);
float GeometrySchlickGGX(float NdotV, float roughness);
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness);
vec3 saturation(vec3 rgb, float adjustment);

//In Utility.frag
float GetComponent(int Type, vec4 Texture);

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
    vert.binormal = binormal;

	float specIntensity = 0.1f;

    // Wireframe color.
    if (colorOverride == 1)
    {
        fragColor = vec4(1);
        return;
    }

	vec3 albedo = vec3(1);
    if (HasDiffuse == 1)
	{
       vec2 uvScale = vec2(ColorUVScaleU, ColorUVScaleV);
        vec2 uvTranslate = vec2(ColorUVTranslateU, ColorUVTranslateV);
        vec2 shift = vec2(ColorBaseU, ColorBaseV);
	    vec2 texCoord0 = shift + uvScale * (f_texcoord0.xy + (uvTranslate));

		vec4 DiffuseTex =  pow(texture(DiffuseMap, texCoord0).rgba, vec4(gamma));

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
    if (HasAmbientMap == 1)
	{
	    vec2 ambientUV = f_texcoord0;
		ambientUV.x *= ColorUVScaleU + ColorUVTranslateU;
	    ambientUV.y *= ColorUVScaleV + ColorUVTranslateV;
	    float factor =  texture(AmbientMap, ambientUV).b; //Usually 128,128,128. Intensity??
        float ambient = 1 - (texture(AmbientMap, ambientUV).r * factor); //Inverted ao
        float shadows = 1 - (texture(AmbientMap, ambientUV).g * factor); //Inverted shadows

		ao = (ambient * shadows);
	}

	vec3 emissionTerm = vec3(0);

    // Calculate shading vectors.
    vec3 I = vec3(0,0,-1) * mat3(mtxCam);
    vec3 N = normal;


    vec3 V = normalize(I); // view
	vec3 L = normalize(specLightDirection); // Light
	vec3 H = normalize(specLightDirection + I); // half angle
    vec3 R = reflect(-I, N); // reflection

    vec3 f0 = mix(vec3(0.04), albedo, metallic); // dialectric
    vec3 kS = FresnelSchlickRoughness(max(dot(N, V), 0.0), f0, roughness);
   
     vec3 kD = 1.0 - kS;
    kD *= 1.0 - metallic;

    // Diffuse pass
    vec3 diffuseIblColor = texture(irradianceMap, N).rgb;
    vec3 diffuseTerm = diffuseIblColor * albedo;
    diffuseTerm *= kD;
    diffuseTerm *= ao;
    diffuseTerm *= 1.1f;


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
    vec3 specularTerm = specularIblColor * brdfTerm * specIntensity;

  // Add render passes.
    fragColor.rgb = vec3(0);
    fragColor.rgb += diffuseTerm;
    fragColor.rgb += specularTerm;
    fragColor.rgb += emissionTerm;


    fragColor.rgb *= min(boneWeightsColored, vec3(1));

    // HDR tonemapping
//    fragColor.rgb = fragColor.rgb / (fragColor.rgb + vec3(1.0));

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


	 //Debug Shading
	vec2 displayTexCoord =  f_texcoord0;

	if (uvChannel == 1)
		displayTexCoord =  f_texcoord0;
	if (uvChannel == 2)
		displayTexCoord =  f_texcoord2;
	if (uvChannel == 3)
		displayTexCoord =  f_texcoord3;

    vec3 displayNormal = (normal.xyz * 0.5) + 0.5;
    if (renderType == 1) // normals color
        fragColor = vec4(displayNormal.rgb,1);
	else if (renderType == 2)
	{
	    diffuseIblColor = texture(irradianceMap, N).rgb;
	    diffuseTerm = diffuseIblColor * vec3(0.5);
		diffuseTerm *= kD;
		diffuseTerm *= ao;

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
	else if (renderType == 3) //DiffuseColor
	{
        vec2 uvScale = vec2(ColorUVScaleU, ColorUVScaleV);
        vec2 uvTranslate = vec2(ColorUVTranslateU, ColorUVTranslateV);
        vec2 shift = vec2(ColorBaseU, ColorBaseV);
	    vec2 texCoord0 = shift + uvScale * (displayTexCoord.xy + (uvTranslate));

		//Comp Selectors
		vec4 diffuseMapColor = vec4(texture(DiffuseMap, texCoord0).rgb, 1);
		diffuseMapColor.r = GetComponent(RedChannel, diffuseMapColor);
		diffuseMapColor.g = GetComponent(GreenChannel, diffuseMapColor);
		diffuseMapColor.b = GetComponent(BlueChannel, diffuseMapColor);

	    fragColor = vec4(diffuseMapColor.rgb, 1);
	}
	else if (renderType == 4) //Display Normal
	{
	   if (HasNormalMap == 1)
         fragColor.rgb = texture(NormalMap, displayTexCoord).rgb;
	   else
		 fragColor.rgb  = vec3(0);
    }
    else if (renderType == 5) // vertexColor
        fragColor = vertexColor;
	else if (renderType == 6) //Display Ambient Occlusion
        fragColor = vec4(vec3(ao), 1);
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

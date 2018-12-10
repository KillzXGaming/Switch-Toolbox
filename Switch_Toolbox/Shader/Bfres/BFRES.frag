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
uniform mat4 mvpMatrix;
uniform vec3 difLightDirection;

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

uniform int isTransparent;

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

uniform int renderTevColors;
uniform int renderMatColors;

uniform vec4 const_color0;
uniform vec4 base_color_mul_color;
uniform vec4 tev_color0;
uniform vec4 tev_color1;
uniform vec4 mat_color0;
uniform vec4 mat_color1;

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
float Luminance(vec3 rgb);

vec3 CalcBumpedNormal(vec3 normal, sampler2D normalMap, VertexAttributes vert, float texCoordIndex);

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

	    // Calculate shading vectors.
    vec3 I = vec3(0,0,-1) * mat3(mvpMatrix);

    vec3 N = normal;
	if (HasNormalMap == 1 && useNormalMap == 1)
		N = CalcBumpedNormal(normal, NormalMap, vert, 0);

   // Diffuse lighting.
    float halfLambert = dot(difLightDirection, N) * 0.5 + 0.5;

	vec4 diffuseMapColor = vec4(texture(DiffuseMap, f_texcoord0).rgb, 1);
    diffuseMapColor *= halfLambert;

 //   vec3 displayNormal = (N * 0.5) + 0.5;


   // fragColor = vec4(displayNormal,1);

    fragColor.rgb += diffuseMapColor.rgb;

    vec3 color = vec3(1);
    vec3 normal = texture(NormalMap, f_texcoord0).rgb;
    vec3 specular = texture(SpecularMap, f_texcoord0).rgb;

    fragColor.a *= texture(DiffuseMap, f_texcoord0).a;

    if (renderVertColor == 1)
        fragColor *= min(vert.vertexColor, vec4(1));

	fragColor *= min(const_color0, vec4(1));
	fragColor *= min(base_color_mul_color, vec4(1));

   float colorScale = texture(DiffuseMap, f_texcoord0).r;
    if (renderMatColors == 1)
	{
      //  fragColor *= vec4(min(mat_color0, colorScale).rgb, 1.0);
      //  fragColor *= vec4(min(mat_color1, (1 - colorScale) + colorScale * 0).rgb, 1.0);
	}
    if (renderTevColors == 1)
	{
      //  fragColor *= vec4(min(tev_color0, colorScale).rgb, 1.0);
	//	fragColor *= vec4(min(tev_color1, (1 - colorScale) + colorScale * 0).rgb, 1.0);
	}

//	if (isTransparent != 1)
 //       fragColor.a = 1;
}

#version 330

// A struct is used for what would normally be attributes from the vert/geom shader.
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

#define BakeShadowAoc = 0;
#define BakeShadowShadow = 1;
#define BakeShadowAocShadow = 2;

uniform vec3 gsys_bake_light_scale;

struct BakedData
{
	float shadowIntensity;
	float aoIntensity;
	vec3 indirectLighting;
};

BakedData ShadowMapBaked(sampler2D ShadowMap, sampler2D LightMap, vec2 texCoordBake0, vec2 texCoordBake1, int ShadowType, int LightType, int CalcType, vec3 NormalMap) {
    BakedData bake0;

    bake0.indirectLighting = vec3(0);
    bake0.aoIntensity = 1;
    bake0.shadowIntensity = 1;

	if (CalcType == 222)
	{
         vec4 bakeTex0 = texture(ShadowMap, texCoordBake0);

	    if (CalcType == 0 || CalcType == 1)
		{
		}
	}
	else
	{
	     switch (ShadowType)
		 {
		     case 0:
			 {
				 bake0.aoIntensity = texture(ShadowMap, texCoordBake0).r;
				 bake0.shadowIntensity = 1.0;
				 break;
		     }
		     case 1:
			 {
				 bake0.aoIntensity = 1.0;
				 bake0.shadowIntensity = texture(ShadowMap, texCoordBake0).r;
				 break;
		     }
		     case 2:
			 {
				 bake0.aoIntensity = texture(ShadowMap, texCoordBake0).r;
				 bake0.shadowIntensity = texture(ShadowMap, texCoordBake0).g;
				 break;
		     }
			 default:
			 {
				 bake0.aoIntensity = 1.0;
		    	 bake0.shadowIntensity = 1.0;
			 }
		 }
		 if (LightType == 0)
		 {
				vec4 bakeMap1 = texture(LightMap, texCoordBake1);
		    	bake0.indirectLighting = gsys_bake_light_scale * bakeMap1.rgb * bakeMap1.a;
		 }
		 else
		 {
		 		bake0.indirectLighting = vec3(0);
		 }
	}
	return bake0;
}
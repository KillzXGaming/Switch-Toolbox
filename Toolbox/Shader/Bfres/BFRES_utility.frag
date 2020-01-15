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

uniform mat4 sphereMatrix;
uniform sampler2D SphereMap;
uniform samplerCube specularIbl;
uniform int HasSphereMap;
uniform int hasTangents;

// Defined in Utility.frag.
float Luminance(vec3 rgb);

vec3 SpecularPass(vec3 I, vec3 normal, int HasSpecularMap, sampler2D SpecularMap, vec3 SpecColor, VertexAttributes vert, float texcoord2, int UseSpecularColor)
{
    float specBrdf = max(dot(I, normal), 0);
    float exponent = 8;

	vec3 specularColor = SpecColor;
	if (specularColor == vec3(0)) //Color shouldn't be black unless it's not set
	    specularColor = vec3(1);

    if (HasSpecularMap == 0)
	{
        return 0.1 * SpecColor * pow(specBrdf, exponent);
	}

	vec2 SpecTexCoord = vert.texCoord;
	if (texcoord2 == 1)
	    SpecTexCoord = vert.texCoord2;


	vec3 Specular = texture(SpecularMap, SpecTexCoord).rgb;
	 if (UseSpecularColor == 0)
	    Specular =  texture(SpecularMap, SpecTexCoord).rrr;

    // TODO: Different games use the channels for separate textures.

    vec3 result = Specular * SpecColor * pow(specBrdf, exponent);
	result *= SpecColor.rgb;

    float intensity = 0.3;
    return result * vec3(intensity);
}

vec3 EmissionPass(sampler2D EmissionMap, float emission_intensity, VertexAttributes vert, float texCoordIndex, vec3 emission_color)
{
    vec3 result = vec3(0);

    // BOTW somtimes uses second uv channel for emission map
    vec3 emission = vec3(1);
    if (texCoordIndex == 1)
        emission = texture2D(EmissionMap, vert.texCoord2).rgb;
    else
        emission = texture2D(EmissionMap, vert.texCoord).rgb;

    // If tex is empty then use full brightness.
    //Some emissive mats have emission but no texture
    // if (Luminance(emission.rgb) < 0.01)
    //     result += vec3(emission_intensity) * emission_color;

    result += emission.rgb;

    return result;
}

vec3 SphereMapColor(vec3 viewNormal, sampler2D spheremap) {
    // Calculate UVs based on view space normals.
    vec2 sphereTexcoord = vec2(viewNormal.x, (1 - viewNormal.y));
    return texture(spheremap, sphereTexcoord * 0.5 + 0.5).rgb;
}

vec3 ReflectionPass(vec3 N, vec3 I, vec4 diffuseMap, vec3 Specular, float aoBlend, vec3 tintColor, VertexAttributes vert) {
    vec3 reflectionPass = vec3(0);
	// cubemap reflection
	vec3 R = reflect(-I, N);

   int maxSpecularLod = 8;
    vec3 cubeColor = textureLod(specularIbl, R, maxSpecularLod).rgb;

    reflectionPass += cubeColor * Specular;

    vec3 viewNormal = mat3(sphereMatrix) * normalize(N.xyz);
    vec3 sphereMapColor = SphereMapColor(vert.viewNormal, SphereMap);

	if (HasSphereMap == 1)
		reflectionPass += sphereMapColor;

    reflectionPass = max(reflectionPass, vec3(0));

    return reflectionPass;
}

float AmbientOcclusionBlend(sampler2D BakeShadowMap, VertexAttributes vert, float ao_density)
{
    float aoMap = texture(BakeShadowMap, vert.texCoord2).r;
    return mix(aoMap, 1, ao_density);
}

vec3 CalculateTangent()
{
    vec3 tangent = vec3(0);

	return tangent;
}

vec3 CalcBumpedNormal(vec3 inputNormal, sampler2D normalMap, VertexAttributes vert, float texCoordIndex)
{
    float normalIntensity = 1;

	//if (normal_map_weight != 0) //MK8 and splatoon 1/2 uses this param
	//      normalIntensity = normal_map_weight;

    // Calculate the resulting normal map and intensity.
	vec3 normalMapColor = vec3(1);
	if (texCoordIndex == 1)
        normalMapColor = vec3(texture(normalMap, vert.texCoord2).rg, 1);
    else
        normalMapColor = vec3(texture(normalMap, vert.texCoord).rg, 1);
    normalMapColor = mix(vec3(0.5, 0.5, 1), normalMapColor, normalIntensity);

    // Remap the normal map to the correct range.
    vec3 normalMapNormal = 2.0 * normalMapColor - vec3(1);

    // TBN Matrix.
    vec3 T = vert.tangent;
    vec3 B = vert.bitangent;

    if (Luminance(B) < 0.01)
        B = normalize(cross(T,  vert.normal));
    mat3 tbnMatrix = mat3(T, B,  vert.normal);

    vec3 newNormal = tbnMatrix * normalMapNormal;
    return normalize(newNormal);
}
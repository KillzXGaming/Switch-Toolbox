#version 330

uniform sampler2D tex;
uniform sampler2D normalMap;
uniform sampler2D bakeShadowMap;
uniform sampler2D bakeLightMap;
uniform sampler2D UVTestPattern;

uniform vec4 highlight_color;

uniform float Saturation;
uniform float Hue;
uniform float Brightness;

uniform int debugShading;

// Viewport Settings
uniform int uvChannel;
uniform int renderType;
uniform int useNormalMap;
uniform int renderVertColor;
uniform int useShadowMap;

in vec2 f_texcoord0;
in vec2 f_texcoord1;
in vec2 f_texcoord2;

in vec4 vertexColor;
in vec3 normal;

out vec4 fragColor;

#define gamma 2.2

vec2 displayTexCoord =  f_texcoord0;

void main()
{
	if (uvChannel == 1)
		displayTexCoord =  f_texcoord0;
	if (uvChannel == 2)
		displayTexCoord =  f_texcoord2;

    fragColor = vec4(vec3(0), 1);

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
	else if (useShadowMap == 1)
    {
		ao = texture(bakeShadowMap, f_texcoord1).r;
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
            fragColor.rgb = texture(normalMap, f_texcoord1).rgb;
		else
            fragColor.rgb = texture(normalMap, displayTexCoord).rgb;
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

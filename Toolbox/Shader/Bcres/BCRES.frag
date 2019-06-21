#version 330
in vec3 normal;
in vec3 position;
in vec2 f_texcoord0;
in vec2 f_texcoord1;
in vec2 f_texcoord2;
in vec2 f_texcoord3;
in vec4 vertexColor;
in vec3 tangent;

in vec3 boneWeightsColored;

uniform vec3 difLightDirection;
uniform vec3 difLightColor;
uniform vec3 ambLightColor;


uniform int colorOverride;
uniform int renderType;
uniform int renderVertColor;
uniform mat4 modelview;

uniform int HasDiffuse;

uniform sampler2D DiffuseMap;
uniform sampler2D UVTestPattern;

out vec4 FragColor;

void main()
{

    if (colorOverride == 1)
    {
        // Wireframe color.

		if (renderVertColor == 1)
		{
            FragColor = vertexColor;
		}
		else
		{
            FragColor = vec4(1);
		}
        return;
    }

	vec3 N = normal;

   // Diffuse lighting.
    float halfLambert = dot(difLightDirection, normal) * 0.5 + 0.5;

	vec4 diffuseMapColor = vec4(texture(DiffuseMap, f_texcoord0).rgb, 1);
    diffuseMapColor *= halfLambert;

     FragColor = vec4(0);
     FragColor.rgb += diffuseMapColor.rgb;

    if (renderVertColor == 1)
        FragColor *= min(vertexColor, vec4(1));

    FragColor.rgb *= min(boneWeightsColored, vec3(1));

    vec2 displayTexCoord =  f_texcoord0;

    vec3 displayNormal = (normal.xyz * 0.5) + 0.5;
    if (renderType == 1) // normals color
        FragColor = vec4(displayNormal.rgb,1);
    else if (renderType == 2) // Lighting
    {
        float halfLambert = dot(difLightDirection, N) * 0.5 + 0.5;
        FragColor = vec4(vec3(halfLambert), 1);
    }
	else if (renderType == 3) //DiffuseColor
	    FragColor = vec4(texture(DiffuseMap, displayTexCoord).rgb, 1);
    else if (renderType == 5) // vertexColor
        FragColor = vertexColor;
    else if (renderType == 7) // uv coords
        FragColor = vec4(displayTexCoord.x, displayTexCoord.y, 1, 1);
    else if (renderType == 8) // uv test pattern
	{
        FragColor = vec4(texture(UVTestPattern, displayTexCoord).rgb, 1);
	}
    else if (renderType == 9) //Display tangents
    {
        vec3 displayTangent = (tangent * 0.5) + 0.5;
        if (dot(tangent, vec3(1)) == 0)
            displayTangent = vec3(0);

        FragColor = vec4(displayTangent,1);
    }
    else if (renderType == 12)
    {
        FragColor.rgb = boneWeightsColored;
    }
}

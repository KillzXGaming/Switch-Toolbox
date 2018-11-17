#version 330
in vec3 normal;
in vec3 color;

uniform vec3 difLightDirection;
uniform vec3 difLightColor;
uniform vec3 ambLightColor;


uniform int colorOverride;
uniform int renderType;
uniform int renderVertColor;
uniform mat4 modelview;

out vec4 FragColor;

void main()
{

    if (colorOverride == 1)
    {
        // Wireframe color.

		if (renderVertColor == 1)
		{
            FragColor = vec4(color, 1);
		}
		else
		{
            FragColor = vec4(1);
		}
        return;
    }

    vec3 displayNormal = (normal.xyz * 0.5) + 0.5;

    vec4 diffuseColor = vec4(1);

    float halfLambert = dot(difLightDirection, normal.xyz);
    halfLambert = (halfLambert + 1) / 2;
    vec3 lighting = mix(ambLightColor, difLightColor, halfLambert); // gradient based lighting

    float normalBnW = dot(vec4(normal * mat3(modelview), 1.0), vec4(0.15,0.15,0.15,1.0));

	vec3 outputColor =  color.rgb * normalBnW;

    FragColor = vec4(outputColor.rgb,1);

    if (renderType == 1) // normals color
        FragColor = vec4(displayNormal.rgb,1);
}

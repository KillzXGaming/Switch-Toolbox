#version 330
in vec3 normal;
in vec4 color;
in vec3 position;

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
            FragColor = color;
		}
		else
		{
            FragColor = vec4(1);
		}
        return;
    }

     FragColor = vec4(1);

    vec3 displayNormal = (normal.xyz * 0.5) + 0.5;
    if (renderType == 1) // normals color
        FragColor = vec4(displayNormal.rgb,1);
}

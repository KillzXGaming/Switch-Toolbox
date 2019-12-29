#version 330
in vec3 normal;
in vec3 color;
in vec3 position;

uniform vec3 difLightDirection;
uniform vec3 difLightColor;
uniform vec3 ambLightColor;


uniform int colorOverride;
uniform int renderType;
uniform int renderVertColor;
uniform mat4 modelview;

out vec4 FragColor;

//inspired by blender checker texture node
float checker(vec3 p)
{
	p.x = (p.x + 0.000001) * 0.999999;
	p.y = (p.y + 0.000001) * 0.999999;
	p.z = (p.z + 0.000001) * 0.999999;
	
	int xi = int(round(abs(p.x)));
	int yi = int(round(abs(p.y)));
	int zi = int(round(abs(p.z)));

	if (mod(yi,2)==0) {
		if(mod(xi,2) != mod(zi,2))
			return 1.0;
		else
			return 0.5;
	}
	else {
		if (mod(xi,2) == mod(zi,2))
			return 1.0;
		else
			return 0.5;
	}
}

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
	
	if(renderType == 0){ //default
		float shading = max(displayNormal.y,0.5);
		FragColor = vec4(vec3(1,1,1)*shading*checker(position*0.015625), 1);
	}
	else if (renderType == 1) // normals color
        FragColor = vec4(displayNormal.rgb,1);
	else if (renderType == 2) // shading
        FragColor = vec4(vec3(1,1,1) * max(displayNormal.y,0.5), 1);
	else if (renderType == 3) // diffuse
		FragColor = vec4(vec3(1,1,1)*checker(position*0.015625), 1);
	else
		FragColor = vec4 (0,0,0,1);
}

#version 330

in vec3 normal;
in vec3 color;
in vec3 position;

out vec4 FragColor;

void main()
{
    vec3 displayNormal = (normal.xyz * 0.5) + 0.5;
	float shading = max(displayNormal.y,0.5);
    FragColor = vec4(vec3(shading),1);
}

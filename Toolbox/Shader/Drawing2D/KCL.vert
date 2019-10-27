#version 330

in vec3 vPosition;
in vec3 vNormal;
in vec3 vColor;

out vec3 normal;
out vec3 color;
out vec3 position;

uniform mat4 modelViewMatrix;
uniform mat4 modelMatrix;

void main()
{
    normal = vNormal;
    color = vColor;
	position = vPosition;

    gl_Position =  modelViewMatrix * vec4(vPosition.xzy, 1.0);
}
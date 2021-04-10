#version 330 core
layout (location = 0) in vec3 aPos;

uniform mat4 projection;
uniform mat4 rotView;
uniform mat4 mtxCam;

out vec3 TexCoords;

void main()
{
    TexCoords = vec3(-aPos.x, aPos.y, aPos.z);
    vec4 clipPos = projection * rotView * vec4(aPos, 1.0);

    gl_Position = clipPos.xyww;
}
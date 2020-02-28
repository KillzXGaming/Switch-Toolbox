#version 330 core
in vec3 vProbePosition;

in vec4 vCoefficent0;
in vec4 vCoefficent1;
in vec4 vCoefficent2;
in vec4 vCoefficent3;
in vec4 vCoefficent4;
in vec4 vCoefficent5;
in vec4 vCoefficent6;

uniform mat4 mtxMdl;
uniform mat4 mtxCam;

out vec3 color;

out vec4 coefficent0;
out vec4 coefficent1;
out vec4 coefficent2;
out vec4 coefficent3;
out vec4 coefficent4;
out vec4 coefficent5;
out vec4 coefficent6;

void main()
{
    gl_Position = mtxCam * mtxMdl * vec4(vProbePosition.xyz, 1);
    color = vec3(1,1,1);
    coefficent0 = vCoefficent0;
    coefficent1 = vCoefficent1;
    coefficent2 = vCoefficent2;
    coefficent3 = vCoefficent3;
    coefficent4 = vCoefficent4;
    coefficent5 = vCoefficent5;
    coefficent6 = vCoefficent6;
}  
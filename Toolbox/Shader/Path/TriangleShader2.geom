#version 330
layout(points) in;
layout(triangle_strip, max_vertices = 72) out;
                
in vec4 color[];

out vec4 fragColor;

uniform mat4 mtxMdl;
uniform mat4 mtxCam;
uniform vec4 pathColor;
uniform bool isPickingMode;
               
uniform float cubeScale;
uniform float controlCubeScale;

float cubeInstanceScale = cubeScale;

vec4 pos;

mat4 mtx = mtxCam*mtxMdl;
                
vec4 points[8] = vec4[](
    vec4(-1.0,-1.0,-1.0, 0.0),
    vec4( 1.0,-1.0,-1.0, 0.0),
    vec4(-1.0, 1.0,-1.0, 0.0),
    vec4( 1.0, 1.0,-1.0, 0.0),
    vec4(-1.0,-1.0, 1.0, 0.0),
    vec4( 1.0,-1.0, 1.0, 0.0),
    vec4(-1.0, 1.0, 1.0, 0.0),
    vec4( 1.0, 1.0, 1.0, 0.0)
);

void face(int p1, int p2, int p3, int p4){
    gl_Position = mtx * (pos + points[p1]*cubeInstanceScale); EmitVertex();
    gl_Position = mtx * (pos + points[p2]*cubeInstanceScale); EmitVertex();
    gl_Position = mtx * (pos + points[p3]*cubeInstanceScale); EmitVertex();
    gl_Position = mtx * (pos + points[p4]*cubeInstanceScale); EmitVertex();
    EndPrimitive();
}

void faceInv(int p3, int p4, int p1, int p2){
    gl_Position = mtx * (pos + points[p1]*cubeInstanceScale); EmitVertex();
    gl_Position = mtx * (pos + points[p2]*cubeInstanceScale); EmitVertex();
    gl_Position = mtx * (pos + points[p3]*cubeInstanceScale); EmitVertex();
    gl_Position = mtx * (pos + points[p4]*cubeInstanceScale); EmitVertex();
    EndPrimitive();
}

void main(){
    //draw point
    if(isPickingMode)
        fragColor = color[0];
    else
        fragColor = pathColor*.125+color[0]*.125;

    pos = gl_in[0].gl_Position;
    faceInv(0,1,2,3);
    face(4,5,6,7);
    face(0,1,4,5);
    faceInv(2,3,6,7);
    faceInv(0,2,4,6);
    face(1,3,5,7);
    cubeInstanceScale = controlCubeScale;
}
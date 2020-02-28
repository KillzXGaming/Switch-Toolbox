#version 330
layout(lines) in;
layout(line_strip, max_vertices = 119) out;
                
in vec4 color[];

in int gl_PrimitiveIDIn[];
out vec4 fragColor;

uniform mat4 mtxMdl;
uniform mat4 mtxCam;
uniform vec4 pathColor;
uniform int gapIndex;
uniform bool isPickingMode;
                
uniform float cubeScale;
uniform float controlCubeScale;

float cubeInstanceScale = cubeScale;
vec4 pos;

mat4 mtx = mtxCam*mtxMdl;
          
vec3 point = gl_in[0].gl_Position.xyz;

vec3 p0 = point;
vec3 p1 = gl_in[1].gl_Position.xyz;
                
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

void face(int p1, int p2, int p4, int p3){
    gl_Position = mtx * (pos + points[p1]*cubeInstanceScale); EmitVertex();
    gl_Position = mtx * (pos + points[p2]*cubeInstanceScale); EmitVertex();
    gl_Position = mtx * (pos + points[p3]*cubeInstanceScale); EmitVertex();
    gl_Position = mtx * (pos + points[p4]*cubeInstanceScale); EmitVertex();
    gl_Position = mtx * (pos + points[p1]*cubeInstanceScale); EmitVertex();
    EndPrimitive();
}

void line(int p1, int p2){
    gl_Position = mtx * (pos + points[p1]*cubeInstanceScale); EmitVertex();
    gl_Position = mtx * (pos + points[p2]*cubeInstanceScale); EmitVertex();
    EndPrimitive();
}

void main(){
    if(!isPickingMode){
        //draw Point 
        //outline
        fragColor = color[0];
        pos = vec4(point,1);
        face(0,1,2,3);
        face(4,5,6,7);
        line(0,4);
        line(1,5);
        line(2,6);
        line(3,7);

        cubeInstanceScale = controlCubeScale;
    }
                    
    //connection line (Point to Point)
    if(gl_PrimitiveIDIn[0]!=gapIndex){
        if(isPickingMode){
            fragColor = pathColor;
            gl_Position = mtx * vec4(p0, 1);
            EmitVertex();
            gl_Position = mtx * vec4(p1, 1);
            EmitVertex(); 
        }else{
            fragColor = color[0];
            gl_Position = mtx * vec4(p0, 1);
            EmitVertex();
            fragColor = color[1];
            gl_Position = mtx * vec4(p1, 1);
            EmitVertex(); 
        }
        EndPrimitive();
    }
}
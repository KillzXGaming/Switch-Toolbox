#version 110

uniform int flipTexture;
uniform mat4 rotationMatrix;
uniform int texCoords0GenType;
uniform int texCoords0Source;
uniform mat4 textureTransforms[3];

vec2 rotateUV(vec2 uv, float rotation)
{
    float mid = 0.5;
    return vec2(
        cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
        cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
    );
}

vec2 SetFlip(vec2 tex)
{
     vec2 outTexCoord = tex;

	if (flipTexture == 1) //FlipH
	      return vec2(-1, 1) * tex + vec2(1, 0);
	else if (flipTexture == 2) //FlipV
	      return vec2(1, -1) * tex + vec2(0, 1);
	else if (flipTexture == 3) //Rotate90
	{
	     float degreesR = 90.0;
	     return rotateUV(tex, radians(degreesR));
    }
	else if (flipTexture == 4) //Rotate180
	{
		  float degreesR = 180.0;
	      return rotateUV(tex, radians(degreesR));
	}
	else if (flipTexture == 5) //Rotate270
	{
		  float degreesR = 270.0;
	      return rotateUV(tex, radians(degreesR));
	}
	return outTexCoord;
}

vec2 SetTexCoordType(int type, vec2 tex)
{
     vec2 outTexCoord = tex;
	 if (type == 0) return tex; //Tex0
	 if (type == 1) return tex; //Tex1
	 if (type == 2) return tex; //Tex3
	 if (type == 3) return tex; //Ortho
	 if (type == 4) return tex; //Pane based
	 if (type == 5) return tex; //Proj
	return outTexCoord;
}

void main()
{
	gl_FrontColor = gl_Color;
	gl_TexCoord[0] =  textureTransforms[0] * gl_MultiTexCoord0;
	gl_TexCoord[0].st = SetFlip(vec2(0.5, 0.5) + gl_TexCoord[0].st);
	
	gl_Position = gl_ModelViewProjectionMatrix * rotationMatrix * gl_Vertex;
}
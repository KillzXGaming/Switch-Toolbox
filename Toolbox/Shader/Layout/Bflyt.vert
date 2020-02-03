#version 330

in vec2 vPosition;
in vec4 vColor;
in vec2 vTexCoord0;
in vec2 vTexCoord1;
in vec2 vTexCoord2;

uniform mat4 rotationMatrix;

uniform mat4 modelViewMatrix;
uniform vec2 uvScale0;
uniform vec2 uvRotate0;
uniform vec2 uvTranslate0;
uniform int flipTexture;

out vec4 VertexColor;
out vec2 TexCoord0;
out vec2 TexCoord1;
out vec2 TexCoord2;

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
	      return rotateUV(tex, 90.0);
	else if (flipTexture == 4) //Rotate180
	      return rotateUV(tex, 180.0);
	else if (flipTexture == 5) //Rotate270
	      return rotateUV(tex, 270.0);

	return outTexCoord;
}

void main()
{
	vec2 texCoord0Transformed = uvScale0 * vTexCoord0.xy + uvTranslate0;
	TexCoord0 = SetFlip(texCoord0Transformed);
	TexCoord1 = SetFlip(texCoord0Transformed);
	TexCoord2 = SetFlip(texCoord0Transformed);

	VertexColor = vColor;
	gl_Position = modelViewMatrix * rotationMatrix * vec4(vPosition, 0, 1);
}
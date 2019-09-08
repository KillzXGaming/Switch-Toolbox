uniform vec2 uvScale0;
uniform vec2 uvRotate0;
uniform vec2 uvTranslate0;

void main()
{
	gl_FrontColor = gl_Color;
	vec2 texCoord0 = uvScale0 * gl_MultiTexCoord0.xy + uvTranslate0;
	gl_TexCoord[0].st = texCoord0;
	gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
}
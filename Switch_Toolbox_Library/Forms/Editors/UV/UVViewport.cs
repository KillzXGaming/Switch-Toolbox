using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Toolbox.Library.Forms
{
    public class UVViewport : Viewport2D
    {
        public List<STGenericObject> ActiveObjects = new List<STGenericObject>();

        public int UvChannelIndex = 0;

        public float Brightness = 1.0f;

        public override void RenderSceme()
        {
            DrawTexturedPlane(1);
            DrawUVs(ActiveObjects);
        }

        private void DrawTexturedPlane(float scale)
        {
            Vector2 scaleCenter = new Vector2(0.5f, 0.5f);

            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0),
            };
            Vector2[] Positions = new Vector2[] {
                new Vector2(1,-1),
                new Vector2(-1,-1),
                new Vector2(-1,1),
                new Vector2(1,1),
            };

            TexCoords[0] = (TexCoords[0] - scaleCenter) * scale + scaleCenter;
            TexCoords[1] = (TexCoords[1] - scaleCenter) * scale + scaleCenter;
            TexCoords[2] = (TexCoords[2] - scaleCenter) * scale + scaleCenter;
            TexCoords[3] = (TexCoords[3] - scaleCenter) * scale + scaleCenter;
            Positions[0] = Positions[0] * scale;
            Positions[1] = Positions[1] * scale;
            Positions[2] = Positions[2] * scale;
            Positions[3] = Positions[3] * scale;

            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Brightness, Brightness, Brightness);
            GL.TexCoord2(TexCoords[0]);
            GL.Vertex2(Positions[0]);
            GL.TexCoord2(TexCoords[1]);
            GL.Vertex2(Positions[1]);
            GL.TexCoord2(TexCoords[2]);
            GL.Vertex2(Positions[2]);
            GL.TexCoord2(TexCoords[3]);
            GL.Vertex2(Positions[3]);
            GL.End();
        }

        public void DrawUVs(List<STGenericObject> genericObjects)
        {
            if (genericObjects.Count == 0) return;

            foreach (var genericObject in genericObjects)
            {
                int divisions = 4;
                int lineWidth = 1;

                Color uvColor = Color.LightGreen;
                Color gridColor = Color.Black;

                List<int> f = new List<int>();
                int displayFaceSize = 0;
                if (genericObject.lodMeshes.Count > 0)
                {
                    f = genericObject.lodMeshes[0].getDisplayFace();
                    displayFaceSize = genericObject.lodMeshes[0].displayFaceSize;
                }
                if (genericObject.PolygonGroups.Count > 0)
                {
                    foreach (var group in genericObject.PolygonGroups)
                    {
                        f.AddRange(genericObject.PolygonGroups[0].GetDisplayFace());
                        displayFaceSize += genericObject.PolygonGroups[0].displayFaceSize;
                    }
                }

                for (int v = 0; v < displayFaceSize; v += 3)
                {
                    if (displayFaceSize < 3 || genericObject.vertices.Count < 3)
                        return;

                    Vector2 v1 = new Vector2(0);
                    Vector2 v2 = new Vector2(0);
                    Vector2 v3 = new Vector2(0);

                    if (f.Count < v + 2)
                        continue;

                    if (UvChannelIndex == 0)
                    {
                        v1 = genericObject.vertices[f[v]].uv0;
                        v2 = genericObject.vertices[f[v + 1]].uv0;
                        v3 = genericObject.vertices[f[v + 2]].uv0;
                    }
                    if (UvChannelIndex == 1)
                    {
                        v1 = genericObject.vertices[f[v]].uv1;
                        v2 = genericObject.vertices[f[v + 1]].uv1;
                        v3 = genericObject.vertices[f[v + 2]].uv1;
                    }
                    if (UvChannelIndex == 2)
                    {
                        v1 = genericObject.vertices[f[v]].uv2;
                        v2 = genericObject.vertices[f[v + 1]].uv2;
                        v3 = genericObject.vertices[f[v + 2]].uv2;
                    }

                    v1 = new Vector2(v1.X, 1 - v1.Y);
                    v2 = new Vector2(v2.X, 1 - v2.Y);
                    v3 = new Vector2(v3.X, 1 - v3.Y);

                    DrawUVTriangleAndGrid(v1, v2, v3, divisions, uvColor, lineWidth, gridColor);
                }
            }
        }
        private void DrawUVTriangleAndGrid(Vector2 v1, Vector2 v2, Vector2 v3, int divisions,
            Color uvColor, int lineWidth, Color gridColor)
        {
            GL.UseProgram(0);

            float bounds = 1;
            Vector2 scaleUv = new Vector2(2);
            Vector2 transUv = new Vector2(1f);

            //Disable textures so they don't affect color
            GL.Disable(EnableCap.Texture2D);
            DrawUvTriangle(v1, v2, v3, uvColor, scaleUv, transUv);

            // Draw Grid
            GL.Color3(gridColor);
            //  DrawHorizontalGrid(divisions, bounds, scaleUv);
            // DrawVerticalGrid(divisions, bounds, scaleUv);
        }

        private static void DrawUvTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Color uvColor, Vector2 scaleUv, Vector2 transUv)
        {
            GL.Color3(uvColor);
            GL.Begin(PrimitiveType.Lines);
            GL.LineWidth(3);
            GL.Vertex2(v1 * scaleUv + transUv);
            GL.Vertex2(v2 * scaleUv + transUv);
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            GL.LineWidth(3);
            GL.Vertex2(v2 * scaleUv + transUv);
            GL.Vertex2(v3 * scaleUv + transUv);
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            GL.LineWidth(3);
            GL.Vertex2(v3 * scaleUv + transUv);
            GL.Vertex2(v1 * scaleUv + transUv);
            GL.End();
        }

        private static void DrawVerticalGrid(int divisions, float bounds, Vector2 scaleUv)
        {
            int verticalCount = divisions;
            for (int i = 0; i < verticalCount * bounds; i++)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(new Vector2((1.0f / verticalCount) * i, -bounds) * scaleUv);
                GL.Vertex2(new Vector2((1.0f / verticalCount) * i, bounds) * scaleUv);
                GL.End();
            }
        }

        private static void DrawHorizontalGrid(int divisions, float bounds, Vector2 scaleUv)
        {
            int horizontalCount = divisions;
            for (int i = 0; i < horizontalCount * bounds; i++)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(new Vector2(-bounds, (1.0f / horizontalCount) * i) * scaleUv);
                GL.Vertex2(new Vector2(bounds, (1.0f / horizontalCount) * i) * scaleUv);
                GL.End();
            }
        }
    }
}

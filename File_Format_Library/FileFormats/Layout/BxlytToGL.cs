using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;
using OpenTK;

namespace LayoutBXLYT
{
    public class BxlytToGL
    {
        public static int ConvertTextureWrap(WrapMode wrapMode)
        {
            switch (wrapMode)
            {
                case WrapMode.Clamp: return (int)TextureWrapMode.ClampToEdge;
                case WrapMode.Mirror: return (int)TextureWrapMode.MirroredRepeat;
                case WrapMode.Repeat: return (int)TextureWrapMode.Repeat;
                default: return (int)TextureWrapMode.Clamp;
            }
        }

        public static int ConvertMagFilterMode(FilterMode filterMode)
        {
            switch (filterMode)
            {
                case FilterMode.Linear: return (int)TextureMagFilter.Linear;
                case FilterMode.Near: return (int)TextureMagFilter.Nearest;
                default: return (int)TextureMagFilter.Linear;
            }
        }

        public static int ConvertMinFilterMode(FilterMode filterMode)
        {
            switch (filterMode)
            {
                case FilterMode.Linear: return (int)TextureMinFilter.Linear;
                case FilterMode.Near: return (int)TextureMinFilter.Nearest;
                default: return (int)TextureMinFilter.Linear;
            }
        }

        public static void DrawPictureBox(BasePane pane, LayoutViewer.Camera2D camera, bool gameWindow, byte effectiveAlpha, Dictionary<string, STGenericTexture> Textures, bool isSelected)
        {
            if (!Runtime.LayoutEditor.DisplayPicturePane)
                return;

            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0)
                };

            if (pane is Cafe.PIC1)
            {
                var pic1Pane = pane as Cafe.PIC1;

                STColor8[] stColors = SetupVertexColors(pane,
                     pic1Pane.ColorBottomRight, pic1Pane.ColorBottomLeft,
                     pic1Pane.ColorTopLeft, pic1Pane.ColorTopRight);

               Color[] Colors = new Color[] {
                    stColors[0].Color,
                    stColors[1].Color,
                    stColors[2].Color,
                    stColors[3].Color,
                };

                var mat = pic1Pane.Material as Cafe.Material;

                var mvp = camera.ModelViewMatrix;

                ShaderLoader.CafeShader.Enable();
                ShaderLoader.CafeShader.SetMatrix("modelViewMatrix", ref mvp);
                BflytShader.SetMaterials(ShaderLoader.CafeShader, mat, pane, Textures);

                if (pic1Pane.TexCoords.Length > 0)
                {
                    TexCoords = new Vector2[] {
                        pic1Pane.TexCoords[0].BottomLeft.ToTKVector2(),
                        pic1Pane.TexCoords[0].BottomRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopLeft.ToTKVector2(),
                   };
                }

                DrawRectangle(pane, gameWindow, pane.Rectangle, TexCoords, Colors, false, effectiveAlpha, isSelected);

                ShaderLoader.CafeShader.Disable();
            }
            else if (pane is CTR.PIC1)
            {
                var pic1Pane = pane as CTR.PIC1;

                Color[] Colors = new Color[] {
                    pic1Pane.ColorBottomLeft.Color,
                    pic1Pane.ColorBottomRight.Color,
                    pic1Pane.ColorTopRight.Color,
                    pic1Pane.ColorTopLeft.Color,
                };

                var mat = pic1Pane.Material as CTR.Material;

                ShaderLoader.CtrShader.Enable();
                BclytShader.SetMaterials(ShaderLoader.CtrShader, (CTR.Material)mat, pane, Textures);

                if (pic1Pane.TexCoords.Length > 0)
                {
                    TexCoords = new Vector2[] {
                        pic1Pane.TexCoords[0].BottomLeft.ToTKVector2(),
                        pic1Pane.TexCoords[0].BottomRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopLeft.ToTKVector2(),
                   };
                }

                DrawRectangle(pane, gameWindow, pane.Rectangle, TexCoords, Colors, false, effectiveAlpha, isSelected);

                ShaderLoader.CtrShader.Disable();
            }
            else if (pane is Revolution.PIC1)
            {
                var pic1Pane = pane as Revolution.PIC1;

                Color[] Colors = new Color[] {
                    pic1Pane.ColorBottomLeft.Color,
                    pic1Pane.ColorBottomRight.Color,
                    pic1Pane.ColorTopRight.Color,
                    pic1Pane.ColorTopLeft.Color,
                };

                var mat = pic1Pane.Material as Revolution.Material;
                RenderBRLYTMaterial(pic1Pane, mat, Textures);

                if (pic1Pane.TexCoords.Length > 0)
                {
                    List<Vector2> texCoords = new List<Vector2>();
                    if (mat.TexCoordGens.Count > 0) {
                        for (int i = 0; i < mat.TexCoordGens.Count; i++)
                        {
                            var texCoord = pic1Pane.TexCoords[(int)mat.TexCoordGens[i].Source - 4];
                            texCoords.Add(texCoord.BottomLeft.ToTKVector2());
                            texCoords.Add(texCoord.BottomRight.ToTKVector2());
                            texCoords.Add(texCoord.TopRight.ToTKVector2());
                            texCoords.Add(texCoord.TopLeft.ToTKVector2());
                        }
                        TexCoords = texCoords.ToArray();
                    }
                    else if (pic1Pane.TexCoords.Length > 0)
                    {
                        var texCoord = pic1Pane.TexCoords[0];
                        texCoords.Add(texCoord.BottomLeft.ToTKVector2());
                        texCoords.Add(texCoord.BottomRight.ToTKVector2());
                        texCoords.Add(texCoord.TopRight.ToTKVector2());
                        texCoords.Add(texCoord.TopLeft.ToTKVector2());
                        TexCoords = texCoords.ToArray();
                    }
                }

                DrawRectangle(pane, gameWindow, pane.Rectangle, TexCoords, Colors, false, effectiveAlpha, isSelected);

                mat.Shader.Disable();
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.Disable(EnableCap.Texture2D);
                //  ShaderLoader.RevShader.Disable();
            }
            else if (pane is GCBLO.PIC1)
            {
                var pic1Pane = pane as GCBLO.PIC1;

                ShaderLoader.BLOShader.Enable();
                BloShader.SetMaterials(ShaderLoader.BLOShader, (GCBLO.Material)pic1Pane.Material, pane, Textures);

                Color[] Colors = new Color[] {
                    pic1Pane.ColorBottomLeft.Color,
                    pic1Pane.ColorBottomRight.Color,
                    pic1Pane.ColorTopRight.Color,
                    pic1Pane.ColorTopLeft.Color,
                };

                if (pic1Pane.TexCoords.Length > 0)
                {
                    TexCoords = new Vector2[] {
                        pic1Pane.TexCoords[0].BottomLeft.ToTKVector2(),
                        pic1Pane.TexCoords[0].BottomRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopLeft.ToTKVector2(),
                   };
                }

                DrawRectangle(pane, gameWindow, pane.Rectangle, TexCoords, Colors, false, effectiveAlpha, isSelected);
                ShaderLoader.BLOShader.Disable();
            }
            else if (pane is GCBLO.PIC2)
            {
                var pic1Pane = pane as GCBLO.PIC2;

                ShaderLoader.BLOShader.Enable();
                BloShader.SetMaterials(ShaderLoader.BLOShader, (GCBLO.Material)pic1Pane.Material, pane, Textures);

                Color[] Colors = new Color[] {
                    pic1Pane.ColorBottomLeft.Color,
                    pic1Pane.ColorBottomRight.Color,
                    pic1Pane.ColorTopRight.Color,
                    pic1Pane.ColorTopLeft.Color,
                };

                if (pic1Pane.TexCoords.Length > 0)
                {
                    TexCoords = new Vector2[] {
                        pic1Pane.TexCoords[0].BottomLeft.ToTKVector2(),
                        pic1Pane.TexCoords[0].BottomRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopLeft.ToTKVector2(),
                   };
                }

                DrawRectangle(pane, gameWindow, pane.Rectangle, TexCoords, Colors, false, effectiveAlpha, isSelected);
                ShaderLoader.BLOShader.Disable();
            }
            else
                    {
                var pic1Pane = pane as IPicturePane;

                Color[] Colors = new Color[] {
                    pic1Pane.ColorBottomLeft.Color,
                    pic1Pane.ColorBottomRight.Color,
                    pic1Pane.ColorTopRight.Color,
                    pic1Pane.ColorTopLeft.Color,
                };

                if (pic1Pane.TexCoords.Length > 0)
                {
                    TexCoords = new Vector2[] {
                        pic1Pane.TexCoords[0].BottomLeft.ToTKVector2(),
                        pic1Pane.TexCoords[0].BottomRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopLeft.ToTKVector2(),
                   };
                }

                DrawRectangle(pane, gameWindow, pane.Rectangle, TexCoords, Colors, false, effectiveAlpha, isSelected);

            }

            //    GL.BindTexture(TextureTarget.Texture2D, 0);
            //   GL.Disable(EnableCap.Texture2D);
            //  GL.UseProgram(0);
        }

        public static void RenderBRLYTMaterial(BasePane pane, Revolution.Material mat, Dictionary<string, STGenericTexture> textures)
        {
            // ShaderLoader.RevShader.Enable();
            if (mat.Shader == null)
            {
                List<int> texIds = new List<int>();
                if (mat.TextureMaps.Length > 0)
                {
                    for (int i = 0; i < mat.TextureMaps.Length; i++)
                    {
                        string TexName = mat.TextureMaps[i].Name;
                        if (mat.animController.TexturePatterns.ContainsKey((LTPTarget)i))
                            TexName = mat.animController.TexturePatterns[(LTPTarget)i];

                        if (textures.ContainsKey(TexName))
                        {
                            bool binded = BindGLTexture(mat.TextureMaps[i], textures[TexName]);
                            if (binded)
                                texIds.Add(textures[TexName].RenderableTex.TexID);
                        }
                    }
                }

                mat.Shader = new Revolution.Shader(mat, (uint)mat.TextureMaps.Length);
                mat.Shader.Compile();
            }

            var alphaCompare = (Revolution.AlphaCompare)mat.AlphaCompare;

            var alphaFunc = BxlytToGL.ConvertAlphaFunc(alphaCompare.Comp0);
            var alphaFunc2 = BxlytToGL.ConvertAlphaFunc(alphaCompare.Comp1);
            var srcFactor = BxlytToGL.ConvertBlendFactor(mat.BlendMode.SourceFactor);
            var destFactor = BxlytToGL.ConvertBlendFactor(mat.BlendMode.DestFactor);
            var blendOp = BxlytToGL.ConvertBlendOperation(mat.BlendMode.BlendOp);
            var logicOp = BxlytToGL.ConvertLogicOperation(mat.BlendMode.LogicOp);

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(alphaFunc, alphaCompare.Ref0 / 255f);
            GL.AlphaFunc(alphaFunc2, alphaCompare.Ref1 / 255f);

            if (mat.BlendMode.BlendOp != 0)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendEquation(blendOp);
            }
            else
            {
                GL.Disable(EnableCap.Blend);
            }
            GL.BlendFunc(srcFactor, destFactor);
            GL.LogicOp(logicOp);

            for (int i = 0; i < 3; i++) {
                var matTranslate = Matrix4.CreateTranslation(0 / 1 - 0.5f, 0 / 1 - 0.5f, 0);
                mat.Shader.SetMatrix4(String.Format("textureTransforms[{0}]", i), ref matTranslate);
            }

            mat.Shader.Enable();
            mat.Shader.SetInt("debugShading", (int)Runtime.LayoutEditor.Shading);

            var paneRotate = pane.Rotate;
            if (pane.animController.PaneSRT?.Count > 0)
            {
                foreach (var animItem in pane.animController.PaneSRT)
                {
                    switch (animItem.Key)
                    {
                        case LPATarget.RotateX:
                            paneRotate.X = animItem.Value; break;
                        case LPATarget.RotateY:
                            paneRotate.Y = animItem.Value; break;
                        case LPATarget.RotateZ:
                            paneRotate.Z = animItem.Value; break;
                    }
                }
            }

            Matrix4 rotationX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(paneRotate.X));
            Matrix4 rotationY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(paneRotate.Y));
            Matrix4 rotationZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(paneRotate.Z));
            var rotationMatrix = rotationX * rotationY * rotationZ;

            mat.Shader.SetMatrix4("rotationMatrix", ref rotationMatrix);

            //Do uv test pattern
            GL.ActiveTexture(TextureUnit.Texture10);
            mat.Shader.SetInt("uvTestPattern", 10);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.uvTestPattern.RenderableTex.TexID);

            if (mat.TextureMaps.Length > 0 || Runtime.LayoutEditor.Shading == Runtime.LayoutEditor.DebugShading.UVTestPattern)
                GL.Enable(EnableCap.Texture2D);

            int id = 1;
            for (int i = 0; i < mat.TextureMaps.Length; i++)
            {
                string TexName = mat.TextureMaps[i].Name;
                if (mat.animController.TexturePatterns.ContainsKey((LTPTarget)i))
                    TexName = mat.animController.TexturePatterns[(LTPTarget)i];

                mat.Shader.SetInt($"hasTexture{i}", 0);
                if (textures.ContainsKey(TexName))
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + id);
                    mat.Shader.SetInt($"textures{i}", id);
                    bool binded = BindGLTexture(mat.TextureMaps[i], textures[TexName]);
                    mat.Shader.SetInt($"hasTexture{i}", binded ? 1 : 0);

                    var scale = new Syroot.Maths.Vector2F(1, 1);
                    float rotate = 0;
                    var translate = new Syroot.Maths.Vector2F(0, 0);

                    int index = (int)mat.TexCoordGens[i].MatrixSource / 3 - 10;
                    if (mat.TextureTransforms.Length > index)
                    {
                        var transform = mat.TextureTransforms[index];
                        scale = transform.Scale;
                        rotate = transform.Rotate;
                        translate = transform.Translate;

                        foreach (var animItem in mat.animController.TextureSRTS)
                        {
                            switch (animItem.Key)
                            {
                                case LTSTarget.ScaleS: scale.X = animItem.Value; break;
                                case LTSTarget.ScaleT: scale.Y = animItem.Value; break;
                                case LTSTarget.Rotate: rotate = animItem.Value; break;
                                case LTSTarget.TranslateS: translate.X = animItem.Value; break;
                                case LTSTarget.TranslateT: translate.Y = animItem.Value; break;
                            }
                        }
                    }

                    Matrix4 matTransform = Matrix4.Identity;
                    var matScale = Matrix4.CreateScale(scale.X, scale.Y, 1.0f);
                    var matRotate = Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), MathHelper.DegreesToRadians(rotate));
                    var matTranslate = Matrix4.CreateTranslation(
                        translate.X / scale.X - 0.5f,
                        translate.Y / scale.Y - 0.5f,0);

                    matTransform = matRotate * matTranslate * matScale;
                    mat.Shader.SetMatrix4(String.Format("textureTransforms[{0}]", i), ref matTransform);
                    id++;
                }
            }

            mat.Shader.RefreshColors(mat);
        }

        public static void DrawBoundryPane(BasePane pane, bool gameWindow, byte effectiveAlpha, bool isSelected)
        {
            if (!Runtime.LayoutEditor.DisplayBoundryPane || gameWindow || Runtime.LayoutEditor.IsGamePreview)
                return;

            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0)
                };

            Color color = Color.DarkGreen;
            if (isSelected)
                color = Color.Red;

            color = Color.FromArgb(70, color);

            Color[] Colors = new Color[] {
                color,
                color,
                color,
                color,
                };

            BxlytToGL.DrawRectangle(pane, gameWindow, pane.Rectangle, TexCoords, Colors, false, effectiveAlpha, isSelected);
        }


        public static void DrawAlignmentPane(BasePane pane, bool gameWindow, byte effectiveAlpha, bool isSelected)
        {
            if (!Runtime.LayoutEditor.DisplayAlignmentPane || gameWindow || Runtime.LayoutEditor.IsGamePreview)
                return;


            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0)
                };

            Color color = Color.Orange;
            if (isSelected)
                color = Color.Red;

            color = Color.FromArgb(70, color);

            Color[] Colors = new Color[] {
                color,
                color,
                color,
                color,
                };

            BxlytToGL.DrawRectangle(pane, gameWindow, pane.Rectangle, TexCoords, Colors, false, effectiveAlpha);
        }

        public static void DrawScissorPane(BasePane pane, bool gameWindow, byte effectiveAlpha, bool isSelected)
        {
            if (!Runtime.LayoutEditor.DisplayScissorPane || gameWindow || Runtime.LayoutEditor.IsGamePreview)
                return;

            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0)
                };

            Color color = Color.Yellow;
            if (isSelected)
                color = Color.Red;

            color = Color.FromArgb(70, color);

            Color[] Colors = new Color[] {
                color,
                color,
                color,
                color,
                };

            BxlytToGL.DrawRectangle(pane, gameWindow, pane.Rectangle, TexCoords, Colors, false, effectiveAlpha);
        }

        public static void DrawTextbox(BasePane pane, LayoutViewer.Camera2D camera, bool gameWindow, Bitmap fontBitmap, byte effectiveAlpha,
            Dictionary<string, STGenericTexture> Textures, List<BasePane> SelectedPanes, bool updateBitmap, bool isSelected)
        {
            GL.Enable(EnableCap.Texture2D);

            var textBox = (ITextPane)pane;

            if (updateBitmap)
                BindFontBitmap(pane, fontBitmap);

            SetupShaders(pane, textBox.Material, Textures);

            GL.ActiveTexture(TextureUnit.Texture0 + 1);
            SetupTextboxShader(pane, textBox.Material);

            GL.BindTexture(TextureTarget.Texture2D, textBox.RenderableFont.TexID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleR, ConvertChannel(STChannelType.Red));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleG, ConvertChannel(STChannelType.Green));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleB, ConvertChannel(STChannelType.Blue));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleA, ConvertChannel(STChannelType.Alpha));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,(int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            Vector2[] texCoords = new Vector2[] {
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 0),
            };

            Color[] Colors = new Color[] {
                Color.White,
                Color.White,
                Color.White,
                Color.White,
                };

            DrawRectangle(pane, gameWindow, pane.Rectangle, texCoords, Colors, false, effectiveAlpha, isSelected);

            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.PopAttrib();
            GL.UseProgram(0);
        }

        private static void BindFontBitmap(BasePane pane, Bitmap fontBitmap)
        {
            var textBox = (ITextPane)pane;

            switch ($"{textBox.HorizontalAlignment}_{textBox.VerticalAlignment}")
            {
                case "Center_Center":
                    {
                        Bitmap b = new Bitmap((int)pane.Width, (int)pane.Height);
                        using (Graphics g = Graphics.FromImage(b))
                            g.DrawImage(fontBitmap, b.Width / 2f - fontBitmap.Width / 2f, b.Height / 2f - fontBitmap.Height / 2f);

                        fontBitmap = b;
                    }
                    break;
                case "Left_Center":
                    {
                        Bitmap b = new Bitmap((int)pane.Width, (int)pane.Height);
                        using (Graphics g = Graphics.FromImage(b))
                            g.DrawImage(fontBitmap, 0, b.Height / 2f - fontBitmap.Height / 2f);

                        fontBitmap = b;
                    }
                    break;
                case "Right_Center":
                    {
                        Bitmap b = new Bitmap((int)pane.Width, (int)pane.Height);
                        using (Graphics g = Graphics.FromImage(b))
                            g.DrawImage(fontBitmap, 0, b.Height / 2f - fontBitmap.Height / 2f);

                        fontBitmap = b;
                    }
                    break;
                case "Center_Top":
                    {
                        Bitmap b = new Bitmap((int)pane.Width, (int)pane.Height);
                        using (Graphics g = Graphics.FromImage(b))
                            g.DrawImage(fontBitmap, b.Width - fontBitmap.Width, b.Height / 2f - fontBitmap.Height / 2f);

                        fontBitmap = b;
                    }
                    break;
                case "Center_Bottom":
                    {
                        Bitmap b = new Bitmap((int)pane.Width, (int)pane.Height);
                        using (Graphics g = Graphics.FromImage(b))
                            g.DrawImage(fontBitmap, b.Width / 2f - fontBitmap.Width / 2f, b.Height - fontBitmap.Height);

                        fontBitmap = b;
                    }
                    break;
                case "Left_Top":
                    {
                        Bitmap b = new Bitmap((int)pane.Width, (int)pane.Height);
                        using (Graphics g = Graphics.FromImage(b))
                            g.DrawImage(fontBitmap, 0, 0);

                        fontBitmap = b;
                    }
                    break;
                case "Right_Top":
                    {
                        Bitmap b = new Bitmap((int)pane.Width, (int)pane.Height);
                        using (Graphics g = Graphics.FromImage(b))
                            g.DrawImage(fontBitmap, b.Width - fontBitmap.Width, 0);

                        fontBitmap = b;
                    }
                    break;
                case "Left_Bottom":
                    {
                        Bitmap b = new Bitmap((int)pane.Width, (int)pane.Height);
                        using (Graphics g = Graphics.FromImage(b))
                            g.DrawImage(fontBitmap, 0, b.Height - fontBitmap.Height);

                        fontBitmap = b;
                    }
                    break;
                case "Right_Bottom":
                    {
                        Bitmap b = new Bitmap((int)pane.Width, (int)pane.Height);
                        using (Graphics g = Graphics.FromImage(b))
                            g.DrawImage(fontBitmap, b.Width - fontBitmap.Width, b.Height - fontBitmap.Height);

                        fontBitmap = b;
                    }
                    break;
            }
            if (textBox.RenderableFont == null)
                textBox.RenderableFont = RenderableTex.FromBitmap(fontBitmap);
            else
                textBox.RenderableFont.UpdateFromBitmap(fontBitmap);

            textBox.RenderableFont.TextureWrapS = TextureWrapMode.Clamp;
            textBox.RenderableFont.TextureWrapT = TextureWrapMode.Clamp;
        }

        //Huge thanks to layout studio for the window pane rendering code
        //https://github.com/Treeki/LayoutStudio/blob/master/layoutgl/widget.cpp
        public static void DrawWindowPane(BasePane pane, LayoutViewer.Camera2D camera, bool gameWindow, byte effectiveAlpha, Dictionary<string, STGenericTexture> Textures, bool isSelected)
        {
            if (!Runtime.LayoutEditor.DisplayWindowPane)
                return;

            BxlytShader shader = null;
            if (pane is Cafe.PAN1)
                shader = ShaderLoader.CafeShader;
            if (pane is CTR.PAN1)
                shader = ShaderLoader.CtrShader;
            if (pane is Revolution.PAN1)
                shader = ShaderLoader.RevShader;
            if (pane is GCBLO.PAN1)
                shader = ShaderLoader.BLOShader;

            var window = (IWindowPane)pane;

            float dX = DrawnVertexX(pane.Width, pane.originX);
            float dY = DrawnVertexY(pane.Height, pane.originY);

            float frameLeft = 0;
            float frameRight = 0;
            float frameTop = 0;
            float frameBottom = 0;
            switch (window.FrameCount)
            {
                case 1:
                    float oneW, oneH;
                    GetTextureSize(window.WindowFrames[0].Material, Textures, out oneW, out oneH);
                    frameLeft = frameRight = oneW;
                    frameTop = frameBottom = oneH;
                    break;
                case 4:
                case 8:
                    GetTextureSize(window.WindowFrames[0].Material, Textures, out frameLeft, out frameTop);
                    GetTextureSize(window.WindowFrames[3].Material, Textures, out frameRight, out frameBottom);
                    break;
            }

            if (frameLeft == 0) frameLeft = window.FrameElementLeft;
            if (frameRight == 0) frameRight = window.FrameElementRight;
            if (frameTop == 0) frameTop = window.FrameElementTop;
            if (frameBottom == 0) frameBottom = window.FrameElementBottm;

            Vector2[] texCoords = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                };

            if (window.Content.TexCoords.Count > 0)
            {
                texCoords = new Vector2[] {
                        window.Content.TexCoords[0].TopLeft.ToTKVector2(),
                        window.Content.TexCoords[0].TopRight.ToTKVector2(),
                        window.Content.TexCoords[0].BottomRight.ToTKVector2(),
                        window.Content.TexCoords[0].BottomLeft.ToTKVector2(),
                   };
            }

            //Setup vertex colors
            Color[] colors = new Color[] {
                 window.Content.ColorBottomLeft.Color,
                 window.Content.ColorBottomRight.Color,
                 window.Content.ColorTopRight.Color,
                 window.Content.ColorTopLeft.Color,
                };

            float contentWidth = ((window.StretchLeft + (pane.Width - frameLeft)) - frameRight) + window.StretchRight;
            float contentHeight = ((window.StretchTop + (pane.Height - frameTop)) - frameBottom) + window.StretchBottm;
            if (window.WindowKind == WindowKind.Horizontal)
                contentHeight = pane.Height;

            //Apply pane alpha
            for (int i = 0; i < colors.Length; i++)
            {
                uint setalpha = (uint)((colors[i].A * effectiveAlpha) / 255);
                colors[i] = Color.FromArgb((int)setalpha, colors[i]);
            }

            if (!window.NotDrawnContent && window.WindowKind != WindowKind.HorizontalNoContent)
            {
                SetupShaders(pane, window.Content.Material, Textures);
                if (window.WindowKind == WindowKind.Horizontal)
                {
                    DrawQuad(gameWindow, dX + frameLeft - window.StretchLeft,
                              dY,
                              contentWidth,
                              contentHeight,
                              texCoords, colors, isSelected);
                }
                else
                {
                    DrawQuad(gameWindow, dX + frameLeft - window.StretchLeft,
                              dY - frameTop + window.StretchTop,
                              contentWidth,
                              contentHeight,
                              texCoords, colors, isSelected);
                }
            }

            //After the content is draw, check this
            //If it's disabled, frames do not use vertex color
            if (!window.UseVertexColorForAll)
            {
                colors = new Color[] {
                    Color.FromArgb(effectiveAlpha, 255,255,255),
                    Color.FromArgb(effectiveAlpha, 255,255,255),
                    Color.FromArgb(effectiveAlpha, 255,255,255),
                    Color.FromArgb(effectiveAlpha, 255,255,255),
                };
            }

            bool hasTextures = true;

            switch (window.FrameCount)
            {
                case 2: //2 frame. 2 textures for corners sides (horizontal)
                case 1: //1 frame. 1 texture for corners (around) or sides (horizontal)
                    {
                        var windowFrame = window.WindowFrames[0];
                        SetupShaders(pane, windowFrame.Material, Textures);
                        SetWindowTextureFlip(shader, windowFrame.Material, window.WindowFrames[0].TextureFlip);

                        hasTextures = windowFrame.Material.TextureMaps?.Length > 0;

                        //2 sides, no corners
                        if (window.WindowKind == WindowKind.Horizontal)
                        {
                            //Left
                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            colors = new Color[] {
                                 window.Content.ColorTopRight.Color,
                                 window.Content.ColorTopRight.Color,
                                 window.Content.ColorBottomRight.Color,
                                 window.Content.ColorBottomRight.Color,
                                };

                            DrawQuad(gameWindow, dX, dY, frameLeft, pane.Height, texCoords, colors, isSelected, hasTextures);

                            //Right

                            if (window.FrameCount == 2)
                            {
                                SetupShaders(pane, window.WindowFrames[1].Material, Textures);
                                hasTextures = windowFrame.Material.TextureMaps?.Length > 0;
                            }

                            texCoords = new Vector2[]
                            {
                                new Vector2(1, 0),
                                new Vector2(0, 0),
                                new Vector2(0, 1),
                                new Vector2(1, 1),
                            };

                            colors = new Color[] {
                                 window.Content.ColorTopLeft.Color,
                                 window.Content.ColorTopLeft.Color,
                                 window.Content.ColorBottomLeft.Color,
                                 window.Content.ColorBottomLeft.Color,
                                };

                            DrawQuad(gameWindow, dX + frameRight + contentWidth, dY, frameRight, pane.Height, texCoords, colors, isSelected, hasTextures);
                        }
                        else if (window.WindowKind == WindowKind.HorizontalNoContent)
                        {
                            //Right

                            hasTextures = windowFrame.Material.TextureMaps?.Length > 0;

                            texCoords = new Vector2[]
                            {
                                new Vector2((pane.Width - frameRight) / frameRight, 0),
                                new Vector2(0, 0),
                                new Vector2(0, 1),
                                new Vector2((pane.Width - frameRight) / frameRight, 1),
                            };

                            colors = new Color[] {
                               //Top Right
                               window.Content.ColorTopRight.Color,

                               //Top Left
                               VertexColorHelper.Mix(window.Content.ColorTopLeft.Color,
                                                window.Content.ColorTopRight.Color,
                                                (frameLeft / frameLeft)),

                                //Bottom Left
                              VertexColorHelper.Mix(window.Content.ColorBottomLeft.Color,
                                window.Content.ColorBottomRight.Color,
                                (frameLeft / frameLeft)),

                              //Bottom Right
                               window.Content.ColorBottomRight.Color,
                            };

                            DrawQuad(gameWindow, dX + frameLeft, dY, pane.Width - frameLeft, pane.Height, texCoords, colors, isSelected, hasTextures);

                            //Left

                            if (window.FrameCount == 2)
                            {
                                SetupShaders(pane, window.WindowFrames[1].Material, Textures);
                                hasTextures = window.WindowFrames[1].Material.TextureMaps?.Length > 0;
                            }

                            colors = new Color[] {
                                 //Top Left
                                 window.Content.ColorTopLeft.Color,

                                 //Top Right
                                       VertexColorHelper.Mix(window.Content.ColorTopLeft.Color,
                                                       window.Content.ColorTopRight.Color,
                                                       1),
                                 //Bottom Right
                                 VertexColorHelper.Mix(window.Content.ColorBottomLeft.Color,
                                                       window.Content.ColorBottomRight.Color,
                                                        1),

                                //Bottom Left
                                 window.Content.ColorBottomLeft.Color,
                            };

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX, dY, pane.Width - frameLeft, pane.Height, texCoords, colors, isSelected, hasTextures);
                        }
                        else if (window.WindowKind == WindowKind.Around)
                        {
                            hasTextures = windowFrame.Material.TextureMaps?.Length > 0;

                            // top left
                            float pieceWidth = pane.Width - frameRight;
                            float pieceHeight = frameTop;

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX, dY, pieceWidth, pieceHeight, texCoords, colors, isSelected, hasTextures);

                            // top right
                            pieceWidth = frameRight;
                            pieceHeight = pane.Height - frameBottom;

                            texCoords = new Vector2[]
                            {
                                new Vector2(1, 0),
                                new Vector2(0, 0),
                                new Vector2(0,(pane.Height - frameTop) / frameTop),
                                new Vector2(1,(pane.Height - frameTop) / frameTop),
                            };

                            DrawQuad(gameWindow, dX + pane.Width - frameRight, dY, pieceWidth, pieceHeight, texCoords, colors, isSelected, hasTextures);

                            // bottom left
                            pieceWidth = frameLeft;
                            pieceHeight = pane.Height - frameTop;

                            texCoords = new Vector2[]
                           {
                                new Vector2(0,(pane.Height - frameBottom) / frameBottom),
                                new Vector2(1,(pane.Height - frameBottom) / frameBottom),
                                new Vector2(1, 0),
                                new Vector2(0, 0),
                           };

                            DrawQuad(gameWindow, dX, dY - frameTop, pieceWidth, pieceHeight, texCoords, colors, isSelected, hasTextures);

                            // bottom right
                            pieceWidth = pane.Width - frameLeft;
                            pieceHeight = frameBottom;

                            texCoords = new Vector2[]
                            {
                                new Vector2((pane.Width - frameLeft) / frameLeft, 1),
                                new Vector2(0, 1),
                                new Vector2(0, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 0),
                            };

                            DrawQuad(gameWindow, dX + frameLeft, dY - pane.Height + frameBottom, pieceWidth, pieceHeight, texCoords, colors, isSelected, hasTextures);
                        }
                    }
                    break;
                    //4 or more will always be around types
                case 4: //4 each corner
                    {
                        var matTL = window.WindowFrames[0].Material;
                        var matTR = window.WindowFrames[1].Material;
                        var matBL = window.WindowFrames[2].Material;
                        var matBR = window.WindowFrames[3].Material;

                        //Todo check this
                        if (window.UseOneMaterialForAll)
                        {
                          /*  matTR = matTL;
                            matBL = matTL;
                            matBR = matTL;*/
                        }

                        if (matTL.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matTL, Textures);
                            SetWindowTextureFlip(shader, matTL, window.WindowFrames[0].TextureFlip);

                            float pieceWidth = pane.Width - frameRight;
                            float pieceHeight = frameTop;

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX, dY, pieceWidth, pieceHeight, texCoords, colors, isSelected);
                        }
                        if (matTR.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matTR, Textures);
                            SetWindowTextureFlip(shader, matTR, window.WindowFrames[1].TextureFlip);

                            float pieceWidth = frameRight;
                            float pieceHeight = pane.Height - frameBottom;

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1,(pane.Height - frameTop) / frameTop),
                                new Vector2(0,(pane.Height - frameTop) / frameTop),
                            };

                            DrawQuad(gameWindow, dX + pane.Width - frameRight, dY, pieceWidth, pieceHeight, texCoords, colors, isSelected);
                        }
                        if (matBL.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matBL, Textures);
                            SetWindowTextureFlip(shader, matBL, window.WindowFrames[2].TextureFlip);

                            float pieceWidth = frameLeft;
                            float pieceHeight = pane.Height - frameTop;

                            texCoords = new Vector2[]
                            {
                                new Vector2(0,1 - ((pane.Height - frameTop) / frameTop)),
                                new Vector2(1,1 - ((pane.Height - frameTop) / frameTop)),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX, dY - frameTop, pieceWidth, pieceHeight, texCoords, colors, isSelected);
                        }
                        if (matBR.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matBR, Textures);
                            SetWindowTextureFlip(shader, matBR, window.WindowFrames[3].TextureFlip);

                            float pieceWidth = pane.Width - frameLeft;
                            float pieceHeight = frameBottom;

                            texCoords = new Vector2[]
                            {
                                new Vector2(1 - ((pane.Width - frameLeft) / frameLeft), 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(1 - ((pane.Width - frameLeft) / frameLeft), 1),
                            };

                            DrawQuad(gameWindow, dX + frameLeft, dY - pane.Height + frameBottom, pieceWidth, pieceHeight, texCoords, colors, isSelected);
                        }
                    }
                    break;
                case 8: //4 per corner, 4 per side
                    {
                        var matTL = window.WindowFrames[0].Material;
                        var matTR = window.WindowFrames[1].Material;
                        var matBL = window.WindowFrames[2].Material;
                        var matBR = window.WindowFrames[3].Material;

                        var matL = window.WindowFrames[4].Material;
                        var matR = window.WindowFrames[5].Material;
                        var matT = window.WindowFrames[6].Material;
                        var matB = window.WindowFrames[7].Material;

                        //Todo check this
                        if (window.UseOneMaterialForAll)
                        {
                         /*   matTR = matTL;
                            matBL = matTL;
                            matBR = matTL;
                            matT = matTL;
                            matB = matTL;
                            matL = matTL;
                            matR = matTL;*/
                        }

                        if (matTL.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matTL, Textures);
                            SetWindowTextureFlip(shader, matTL, window.WindowFrames[0].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX, dY, frameLeft, frameTop, texCoords, colors, isSelected);
                        }

                        if (matTR.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matTR, Textures);
                            SetWindowTextureFlip(shader, matTR, window.WindowFrames[1].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX + pane.Width - frameRight, dY, frameRight, frameTop, texCoords, colors, isSelected);
                        }

                        if (matBL.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matBL, Textures);
                            SetWindowTextureFlip(shader, matBL, window.WindowFrames[2].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX, dY - pane.Height + frameTop, frameLeft, frameBottom, texCoords, colors, isSelected);
                        }

                        if (matBR.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matBR, Textures);
                            SetWindowTextureFlip(shader, matBR, window.WindowFrames[3].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX + pane.Width - frameLeft, dY - pane.Height + frameBottom, frameRight, frameBottom, texCoords, colors, isSelected);
                        }

                        if (matT.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matT, Textures);
                            SetWindowTextureFlip(shader, matT, window.WindowFrames[4].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX + frameLeft, dY, contentWidth, frameTop, texCoords, colors, isSelected);
                        }

                        if (matB.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matB, Textures);
                            SetWindowTextureFlip(shader, matB, window.WindowFrames[5].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(1-((pane.Width - frameLeft) / frameLeft), 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(1-((pane.Width - frameLeft) / frameLeft), 1),
                            };

                            DrawQuad(gameWindow, dX + frameRight, dY - (pane.Height - frameBottom), contentWidth, frameTop, texCoords, colors, isSelected);
                        }

                        if (matL.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matL, Textures);
                            SetWindowTextureFlip(shader, matL, window.WindowFrames[6].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0,1-((pane.Height - frameTop) / frameTop)),
                                new Vector2(1,1-((pane.Height - frameTop) / frameTop)),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX, dY - frameTop, frameLeft, contentHeight, texCoords, colors, isSelected);
                        }

                        if (matR.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matR, Textures);
                            SetWindowTextureFlip(shader, matR, window.WindowFrames[7].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1,(pane.Height - frameBottom) / frameBottom),
                                new Vector2(0,(pane.Height - frameBottom) / frameBottom),
                            };

                            DrawQuad(gameWindow, dX + (pane.Width - frameRight), dY - frameTop, frameRight, contentHeight, texCoords, colors, isSelected);
                        }
                    }
                    break;
            }

            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.UseProgram(0);
        }

        private static void SetWindowTextureFlip(BxlytShader shader, BxlytMaterial mat, WindowFrameTexFlip flip)
        {
            if (mat is Revolution.Material)
                ((Revolution.Material)mat).Shader.SetInt("flipTexture", (int)flip);
            else 
                shader.SetInt("flipTexture", (int)flip);
        }

        private static void GetTextureSize(BxlytMaterial pane, Dictionary<string, STGenericTexture> Textures, out float width, out float height)
        {
            width = 0;
            height = 0;
            if (pane.TextureMaps.Length > 0)
            {
                if (Textures.ContainsKey(pane.TextureMaps[0].Name))
                {
                    var tex = Textures[pane.TextureMaps[0].Name];
                    width = tex.Width;
                    height = tex.Height;
                }
            }
        }

        private static float DrawnVertexX(float width, OriginX originX)
        {
            switch (originX)
            {
                case OriginX.Center: return -width / 2.0f;
                case OriginX.Right: return -width;
                default: return 0.0f;
            }
        }

        private static float DrawnVertexY(float height, OriginY originX)
        {
            switch (originX)
            {
                case OriginY.Center: return height / 2.0f;
                case OriginY.Bottom: return height;
                default: return 0.0f;
            }
        }

        private static void DrawQuad(bool gameWindow, float x, float y, float w, float h, Vector2[] texCoords, Color[] colors, bool isSelected, bool hasTextures = true)
        {
            if (!gameWindow && !Runtime.LayoutEditor.IsGamePreview)
            {
                GL.Disable(EnableCap.Texture2D);
                GL.Disable(EnableCap.AlphaTest);
                GL.Disable(EnableCap.Blend);

                GL.LineWidth(0.5f);
                GL.Begin(PrimitiveType.LineLoop);
                GL.Color4(isSelected ? Color.Red : Color.Green);
                GL.Vertex2(x, y);
                GL.Vertex2(x + w, y);
                GL.Vertex2(x + w, y - h);
                GL.Vertex2(x, y - h);
                GL.End();
                GL.LineWidth(1f);

                GL.Enable(EnableCap.AlphaTest);
                GL.Enable(EnableCap.Blend);
                GL.Enable(EnableCap.Texture2D);
            }

            if (!hasTextures && !gameWindow)
            {
                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex2(x, y);
                GL.Vertex2(x + w, y);
                GL.Vertex2(x + w, y - h);
                GL.Vertex2(x, y - h);
                GL.End();
            }
            else
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Color4(colors[0]);
                GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[0].X, texCoords[0].Y);
                GL.Vertex2(x, y);
                GL.Color4(colors[1]);
                GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[1].X, texCoords[1].Y);
                GL.Vertex2(x + w, y);
                GL.Color4(colors[2]);
                GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[2].X, texCoords[2].Y);
                GL.Vertex2(x + w, y - h);
                GL.Color4(colors[3]);
                GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[3].X, texCoords[3].Y);
                GL.Vertex2(x, y - h);
                GL.End();
            }
        }

        enum FrameType
        {
            LeftTop,
            Left,
            LeftBottom,
            Top,
            Bottom,
            RightTop,
            Right,
            RightBottom,
        }

        private static Vector2 GetFramePosition(uint paneWidth, uint paneHeight, IWindowPane window, FrameType type)
        {


            Vector2 pos = new Vector2();
            switch (type)
            {
                case FrameType.LeftTop:

                    break;
            }
            return pos;
        }

        private static void SetupShaders(BasePane pane, BxlytMaterial mat, Dictionary<string, STGenericTexture> textures)
        {
            if (mat is Cafe.Material)
            {
                ShaderLoader.CafeShader.Enable();
                BflytShader.SetMaterials(ShaderLoader.CafeShader, (Cafe.Material)mat, pane, textures);
            }
            else if (mat is Revolution.Material)
            {
                RenderBRLYTMaterial(pane, (Revolution.Material)mat, textures);
            }
            else if (mat is CTR.Material)
            {
                ShaderLoader.CtrShader.Enable();
                BclytShader.SetMaterials(ShaderLoader.CtrShader, (CTR.Material)mat, pane, textures);
            }
            else if (mat is GCBLO.Material)
            {
                ShaderLoader.BLOShader.Enable();
                BloShader.SetMaterials(ShaderLoader.CtrShader, (GCBLO.Material)mat, pane, textures);
            }
        }

        //Todo make a method to get the shaders and set the values that way
        //Shader loading for brlyt is still experimental and not done the same way
        private static void SetupTextboxShader(BasePane pane, BxlytMaterial mat)
        {
            if (mat is Cafe.Material)
            {
                ShaderLoader.CafeShader.SetInt($"numTextureMaps", 1);
                ShaderLoader.CafeShader.SetInt($"textures0", 1);
                ShaderLoader.CafeShader.SetInt($"hasTexture0", 1);
            }
            else if (mat is CTR.Material)
            {
                ShaderLoader.CtrShader.SetInt($"numTextureMaps", 1);
                ShaderLoader.CtrShader.SetInt($"textures0", 1);
                ShaderLoader.CtrShader.SetInt($"hasTexture0", 1);
            }
            else if (mat is Revolution.Material)
            {
                //Make sure the text pane texture count is 1 to bind bitmap font to
                if (((Revolution.Material)mat).Shader.TextureCount != 1) {
                    ((Revolution.Material)mat).Shader.TextureCount = 1;
                    ((Revolution.Material)mat).Shader.Compile();
                    Console.WriteLine($"Updating text pane texture renderer {pane.Name}");
                }

                //Reload transforms
                for (int i = 0; i < 3; i++) {
                    var matTranslate = Matrix4.CreateTranslation(0 / 1 - 0.5f, 0 / 1 - 0.5f, 0);
                    ((Revolution.Material)mat).Shader.SetMatrix4(String.Format("textureTransforms[{0}]", i), ref matTranslate);
                }

                //Load ids and enable the bitmap font texture
                ((Revolution.Material)mat).Shader.SetInt($"numTextureMaps", 1);
                ((Revolution.Material)mat).Shader.SetInt($"textures0", 1);
                ((Revolution.Material)mat).Shader.SetInt($"hasTexture0", 1);
            }
        }

        private static void RenderWindowContent(BasePane pane, uint sizeX, uint sizeY, BxlytWindowContent content,
            byte effectiveAlpha, Dictionary<string, STGenericTexture> Textures)
        {
            var mat = content.Material;
            var rect = pane.CreateRectangle(sizeX, sizeY);

            SetupShaders(pane, mat, Textures);

            Vector2[] texCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1), 
                new Vector2(0,0),
                new Vector2(1,0)
                };

            Color[] colors = new Color[] {
                content.ColorBottomLeft.Color,
                content.ColorBottomRight.Color,
                content.ColorTopRight.Color,
                content.ColorTopLeft.Color,
                };

            if (content.TexCoords.Count > 0)
            {
                texCoords = new Vector2[] {
                        content.TexCoords[0].BottomLeft.ToTKVector2(),
                        content.TexCoords[0].BottomRight.ToTKVector2(),
                        content.TexCoords[0].TopRight.ToTKVector2(),
                        content.TexCoords[0].TopLeft.ToTKVector2(),
                   };
            }

            GL.Begin(PrimitiveType.Quads);
            GL.Color4(colors[0]);
            GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[0].X, texCoords[0].Y);
            GL.Vertex2(rect.BottomLeftPoint);
            GL.Color4(colors[1]);
            GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[1].X, texCoords[1].Y);
            GL.Vertex2(rect.BottomRightPoint);
            GL.Color4(colors[2]);
            GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[2].X, texCoords[2].Y);
            GL.Vertex2(rect.TopRightPoint);
            GL.Color4(colors[3]);
            GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[3].X, texCoords[3].Y);
            GL.Vertex2(rect.TopLeftPoint);
            GL.End();
        }

        public static bool BindGLTexture(BxlytTextureRef tex, STGenericTexture texture)
        {
            if (texture.RenderableTex == null || !texture.RenderableTex.GLInitialized)
                texture.LoadOpenGLTexture();

            //If the texture is still not initialized then return
            if (!texture.RenderableTex.GLInitialized)
                return false;

            GL.BindTexture(TextureTarget.Texture2D, texture.RenderableTex.TexID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleR, ConvertChannel(texture.RedChannel));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleG, ConvertChannel(texture.GreenChannel));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleB, ConvertChannel(texture.BlueChannel));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleA, ConvertChannel(texture.AlphaChannel));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ConvertTextureWrap(tex.WrapModeU));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ConvertTextureWrap(tex.WrapModeV));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ConvertMagFilterMode(tex.MaxFilterMode));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ConvertMinFilterMode(tex.MinFilterMode));

            return true;
        }

        private static int ConvertChannel(STChannelType type)
        {
            switch (type)
            {
                case STChannelType.Red: return (int)TextureSwizzle.Red;
                case STChannelType.Green: return (int)TextureSwizzle.Green;
                case STChannelType.Blue: return (int)TextureSwizzle.Blue;
                case STChannelType.Alpha: return (int)TextureSwizzle.Alpha;
                case STChannelType.Zero: return (int)TextureSwizzle.Zero;
                case STChannelType.One: return (int)TextureSwizzle.One;
                default: return 0;
            }
        }


        public static BlendingFactor ConvertBlendFactor(BxlytBlendMode.GX2BlendFactor blendFactor)
        {
            switch (blendFactor)
            {
                case BxlytBlendMode.GX2BlendFactor.DestAlpha: return BlendingFactor.DstAlpha;
                case BxlytBlendMode.GX2BlendFactor.DestColor: return BlendingFactor.DstColor;
                case BxlytBlendMode.GX2BlendFactor.DestInvAlpha: return BlendingFactor.OneMinusDstAlpha;
                case BxlytBlendMode.GX2BlendFactor.DestInvColor: return BlendingFactor.OneMinusDstColor;
                case BxlytBlendMode.GX2BlendFactor.Factor0: return BlendingFactor.Zero;
                case BxlytBlendMode.GX2BlendFactor.Factor1: return BlendingFactor.One;
                case BxlytBlendMode.GX2BlendFactor.SourceAlpha: return BlendingFactor.SrcAlpha;
                case BxlytBlendMode.GX2BlendFactor.SourceColor: return BlendingFactor.SrcColor;
                case BxlytBlendMode.GX2BlendFactor.SourceInvAlpha: return BlendingFactor.OneMinusSrcAlpha;
                case BxlytBlendMode.GX2BlendFactor.SourceInvColor: return BlendingFactor.OneMinusSrcColor;
                default: return BlendingFactor.Zero;
            }
        }

        public static LogicOp ConvertLogicOperation(BxlytBlendMode.GX2LogicOp blendOp)
        {
            switch (blendOp)
            {
                case BxlytBlendMode.GX2LogicOp.And: return LogicOp.And;
                case BxlytBlendMode.GX2LogicOp.Clear: return LogicOp.Clear;
                case BxlytBlendMode.GX2LogicOp.Copy: return LogicOp.Copy;
                case BxlytBlendMode.GX2LogicOp.Equiv: return LogicOp.Equiv;
                case BxlytBlendMode.GX2LogicOp.Inv: return LogicOp.Invert;
                case BxlytBlendMode.GX2LogicOp.Nand: return LogicOp.Nand;
                case BxlytBlendMode.GX2LogicOp.NoOp: return LogicOp.Noop;
                case BxlytBlendMode.GX2LogicOp.Nor: return LogicOp.Nor;
                case BxlytBlendMode.GX2LogicOp.Or: return LogicOp.Or;
                case BxlytBlendMode.GX2LogicOp.RevAnd: return LogicOp.AndReverse;
                case BxlytBlendMode.GX2LogicOp.RevOr: return LogicOp.OrReverse;
                case BxlytBlendMode.GX2LogicOp.Set: return LogicOp.Set;
                case BxlytBlendMode.GX2LogicOp.Xor: return LogicOp.Xor;
                case BxlytBlendMode.GX2LogicOp.Disable:
                    GL.Disable(EnableCap.ColorLogicOp);
                    return LogicOp.Noop;
                default: return LogicOp.Noop;

            }
        }

        public static AlphaFunction ConvertAlphaFunc(GfxAlphaFunction alphaFunc)
        {
            switch (alphaFunc)
            {
                case GfxAlphaFunction.Always: return AlphaFunction.Always;
                case GfxAlphaFunction.Equal: return AlphaFunction.Equal;
                case GfxAlphaFunction.Greater: return AlphaFunction.Greater;
                case GfxAlphaFunction.GreaterOrEqual: return AlphaFunction.Gequal;
                case GfxAlphaFunction.Less: return AlphaFunction.Less;
                case GfxAlphaFunction.LessOrEqual: return AlphaFunction.Lequal;
                case GfxAlphaFunction.Never: return AlphaFunction.Never;
                case GfxAlphaFunction.NotEqual: return AlphaFunction.Notequal;
                default: return AlphaFunction.Always;
            }
        }

        public static BlendEquationMode ConvertBlendOperation(BxlytBlendMode.GX2BlendOp blendOp)
        {
            switch (blendOp)
            {
                case BxlytBlendMode.GX2BlendOp.Add: return BlendEquationMode.FuncAdd;
                case BxlytBlendMode.GX2BlendOp.ReverseSubtract: return BlendEquationMode.FuncReverseSubtract;
                case BxlytBlendMode.GX2BlendOp.SelectMax: return BlendEquationMode.Max;
                case BxlytBlendMode.GX2BlendOp.SelectMin: return BlendEquationMode.Min;
                case BxlytBlendMode.GX2BlendOp.Subtract: return BlendEquationMode.FuncSubtract;
                case BxlytBlendMode.GX2BlendOp.Disable:
                    GL.Disable(EnableCap.Blend);
                    return BlendEquationMode.FuncAdd;
                default: return BlendEquationMode.FuncAdd;
            }
        }

        enum TextureSwizzle
        {
            Zero = All.Zero,
            One = All.One,
            Red = All.Red,
            Green = All.Green,
            Blue = All.Blue,
            Alpha = All.Alpha,
        }

        private static STColor8[] SetupVertexColors(BasePane pane,
            STColor8 ColorBottomRight, STColor8 ColorBottomLeft, 
            STColor8 ColorTopLeft, STColor8 ColorTopRight)
        {
            STColor8[] outColors = new STColor8[4];
            outColors[0] = ColorBottomLeft;
            outColors[1] = ColorBottomRight;
            outColors[2] = ColorTopRight;
            outColors[3] = ColorTopLeft;

            foreach (var animItem in pane.animController.PaneVertexColors)
            {
                switch (animItem.Key)
                {
                    case LVCTarget.LeftBottomRed: outColors[0].R = (byte)animItem.Value; break;
                    case LVCTarget.LeftBottomGreen: outColors[0].G = (byte)animItem.Value; break;
                    case LVCTarget.LeftBottomBlue: outColors[0].B = (byte)animItem.Value; break;
                    case LVCTarget.LeftBottomAlpha: outColors[0].A = (byte)animItem.Value; break;
                    case LVCTarget.RightBottomRed: outColors[1].R = (byte)animItem.Value; break;
                    case LVCTarget.RightBottomGreen: outColors[1].G = (byte)animItem.Value; break;
                    case LVCTarget.RightBottomBlue: outColors[1].B = (byte)animItem.Value; break;
                    case LVCTarget.RightBottomAlpha: outColors[1].A = (byte)animItem.Value; break;
                    case LVCTarget.RightTopRed: outColors[2].R = (byte)animItem.Value; break;
                    case LVCTarget.RightTopGreen: outColors[2].G = (byte)animItem.Value; break;
                    case LVCTarget.RightTopBlue: outColors[2].B = (byte)animItem.Value; break;
                    case LVCTarget.RightTopAlpha: outColors[2].A = (byte)animItem.Value; break;
                    case LVCTarget.LeftTopRed: outColors[3].R = (byte)animItem.Value; break;
                    case LVCTarget.LeftTopGreen: outColors[3].G = (byte)animItem.Value; break;
                    case LVCTarget.LeftTopBlue: outColors[3].B = (byte)animItem.Value; break;
                    case LVCTarget.LeftTopAlpha: outColors[3].A = (byte)animItem.Value; break;
                }
            }

            return outColors;
        }

        public static void DrawRectangle(BasePane pane, bool gameWindow, CustomRectangle rect, Vector2[] texCoords,
           Color[] colors, bool useLines = true, byte alpha = 255, bool selectionOutline = false)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                float outAlpha = BasePane.MixColors(colors[i].A, (float)(alpha * (float)alpha) / 255f);
                colors[i] = Color.FromArgb(Utils.FloatToIntClamp(outAlpha), colors[i]);
            }

            if (LayoutEditor.UseLegacyGL)
            {
                if (useLines)
                {
                    GL.Begin(PrimitiveType.LineLoop);
                    GL.Color4(selectionOutline ? Color.Red : colors[0]);
                    GL.Vertex2(rect.BottomLeftPoint);
                    GL.Vertex2(rect.BottomRightPoint);
                    GL.Vertex2(rect.TopRightPoint);
                    GL.Vertex2(rect.TopLeftPoint);
                    GL.End();
                }
                else
                {
                     int numTexCoord = texCoords.Length >= 4 ? 4 / texCoords.Length : 0;

                    GL.Begin(PrimitiveType.Quads);
                    GL.Color4(colors[0]);
                    for (int i = 0; i < numTexCoord; i++)
                        GL.MultiTexCoord2(TextureUnit.Texture0 + i, texCoords[0 + (i * 4)].X, texCoords[0 + (i * 4)].Y);
                    GL.Vertex2(rect.BottomLeftPoint);
                    GL.Color4(colors[1]);
                    for (int i = 0; i < numTexCoord; i++)
                        GL.MultiTexCoord2(TextureUnit.Texture0 + i, texCoords[1 + (i * 4)].X, texCoords[1 + (i * 4)].Y);
                    GL.Vertex2(rect.BottomRightPoint);
                    GL.Color4(colors[2]);
                    for (int i = 0; i < numTexCoord; i++)
                        GL.MultiTexCoord2(TextureUnit.Texture0 + i, texCoords[2 + (i * 4)].X, texCoords[2 + (i * 4)].Y);
                    GL.Vertex2(rect.TopRightPoint);
                    GL.Color4(colors[3]);
                    for (int i = 0; i < numTexCoord; i++)
                        GL.MultiTexCoord2(TextureUnit.Texture0 + i, texCoords[3 + (i * 4)].X, texCoords[3 + (i * 4)].Y);
                    GL.Vertex2(rect.TopLeftPoint);
                    GL.End();
                }
            }
            else
            {
                if (pane.renderablePane == null)
                    pane.renderablePane = new RenderablePane();

                Vector3[] vertices = new Vector3[4];
                vertices[0] = new Vector3(rect.BottomLeftPoint.X, rect.BottomLeftPoint.Y, 0);
                vertices[1] = new Vector3(rect.BottomRightPoint.X, rect.BottomRightPoint.Y, 0);
                vertices[2] = new Vector3(rect.TopRightPoint.X, rect.TopRightPoint.Y, 0);
                vertices[3] = new Vector3(rect.TopLeftPoint.X, rect.TopLeftPoint.Y, 0);
                Vector4[] vertexColors = new Vector4[4];

            //    pane.renderablePane.Render(vertices, vertexColors, texCoords);
            }
        }
    }
}

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
using LayoutBXLYT.Cafe;

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

        public static void DrawPictureBox(BasePane pane, bool gameWindow, byte effectiveAlpha, Dictionary<string, STGenericTexture> Textures, bool isSelected)
        {
            if (!Runtime.LayoutEditor.DisplayPicturePane)
                return;

            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0)
                };

            if (pane is Cafe.BFLYT.PIC1)
            {
                var pic1Pane = pane as Cafe.BFLYT.PIC1;

                STColor8[] stColors = SetupVertexColors(pane,
                     pic1Pane.ColorBottomRight, pic1Pane.ColorBottomLeft,
                     pic1Pane.ColorTopLeft, pic1Pane.ColorTopRight);

               Color[] Colors = new Color[] {
                    stColors[0].Color,
                    stColors[1].Color,
                    stColors[2].Color,
                    stColors[3].Color,
                };

                var mat = pic1Pane.Material;
                if (mat.Shader == null)
                {
                    mat.Shader = new BflytShader(mat);
                    mat.Shader.Compile();
                }

                mat.Shader.Enable();
                ((BflytShader)mat.Shader).SetMaterials(pic1Pane,Textures);

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

                mat.Shader.Disable();
            }
            else if (pane is BCLYT.PIC1)
            {
                var pic1Pane = pane as BCLYT.PIC1;

                Color[] Colors = new Color[] {
                    pic1Pane.ColorBottomLeft.Color,
                    pic1Pane.ColorBottomRight.Color,
                    pic1Pane.ColorTopRight.Color,
                    pic1Pane.ColorTopLeft.Color,
                };

                var mat = pic1Pane.Material;
                if (mat.Shader == null)
                {
                    mat.Shader = new BclytShader(mat);
                    mat.Shader.Compile();
                }

                mat.Shader.Enable();
                ((BclytShader)mat.Shader).SetMaterials(pane,Textures);

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

                mat.Shader.Disable();
            }
            else if (pane is BRLYT.PIC1)
            {
                var pic1Pane = pane as BRLYT.PIC1;

                Color[] Colors = new Color[] {
                    pic1Pane.ColorBottomLeft.Color,
                    pic1Pane.ColorBottomRight.Color,
                    pic1Pane.ColorTopRight.Color,
                    pic1Pane.ColorTopLeft.Color,
                };

                var mat = pic1Pane.Material;
                if (mat.Shader == null)
                {
                    mat.Shader = new BrlytShader(mat);
                    mat.Shader.Compile();
                }

                mat.Shader.Enable();
                ((BrlytShader)mat.Shader).SetMaterials(pane, Textures);

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

                mat.Shader.Disable();
            }


            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.PopAttrib();
            GL.UseProgram(0);
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

            BxlytToGL.DrawRectangle(pane, gameWindow, pane.Rectangle, TexCoords, Colors, false, effectiveAlpha);
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

        public static void DrawTextbox(BasePane pane, bool gameWindow, Bitmap fontBitmap, byte effectiveAlpha,
            Dictionary<string, STGenericTexture> Textures, List<BasePane> SelectedPanes, bool updateBitmap, bool isSelected)
        {
            var textBox = (ITextPane)pane;

            if (updateBitmap)
                BindFontBitmap(pane, fontBitmap);

            var mat = textBox.Material as BFLYT.Material;
            if (mat.Shader == null)
            {
                mat.Shader = new BflytShader(mat);
                mat.Shader.Compile();
            }

            mat.Shader.Enable();
            ((BflytShader)mat.Shader).SetMaterials(pane, Textures);

            GL.ActiveTexture(TextureUnit.Texture0 + 1);
            mat.Shader.SetInt($"numTextureMaps", 1);
            mat.Shader.SetInt($"textures0", 1);
            mat.Shader.SetInt($"hasTexture0", 1);
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

            Color color = Color.White;
            Color[] Colors = new Color[] {
                color,
                color,
                color,
                color,
                };

            DrawRectangle(pane, gameWindow, pane.Rectangle, texCoords, Colors, false, effectiveAlpha, isSelected);

            mat.Shader.Disable();

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
        //Note i still need to fix UV coordinates being flips and transformed!
        public static void DrawWindowPane(BasePane pane,bool gameWindow, byte effectiveAlpha, Dictionary<string, STGenericTexture> Textures, bool isSelected)
        {
            if (!Runtime.LayoutEditor.DisplayWindowPane)
                return;

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
                        window.Content.TexCoords[0].BottomLeft.ToTKVector2(),
                        window.Content.TexCoords[0].BottomRight.ToTKVector2(),
                        window.Content.TexCoords[0].TopRight.ToTKVector2(),
                        window.Content.TexCoords[0].TopLeft.ToTKVector2(),
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

            //Apply pane alpha
            for (int i = 0; i < colors.Length; i++)
            {
                uint setalpha = (uint)((colors[i].A * effectiveAlpha) / 255);
                colors[i] = Color.FromArgb((int)setalpha, colors[i]);
            }

            if (!window.NotDrawnContent && window.WindowKind != WindowKind.HorizontalNoContent)
            {
                SetupShaders(pane, window.Content.Material, Textures);
                DrawQuad(gameWindow, dX + frameLeft - window.StretchLeft,
                          dY - frameTop + window.StretchTop,
                          contentWidth,
                          contentHeight,
                          texCoords, colors);
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



            switch (window.FrameCount)
            {
                case 2: //2 frame. 2 textures for corners sides (horizontal)
                case 1: //1 frame. 1 texture for corners (around) or sides (horizontal)
                    {
                        var windowFrame = window.WindowFrames[0];
                        SetupShaders(pane, windowFrame.Material, Textures);

                        //2 sides, no corners
                        if (window.WindowKind == WindowKind.Horizontal)
                        {
                            //Left

                            texCoords = new Vector2[]
                            {
                                new Vector2(1, 0),
                                new Vector2(0, 0),
                                new Vector2(0, 1),
                                new Vector2(1, 1),
                            };

                            colors = new Color[] {
                                 window.Content.ColorTopRight.Color,
                                 window.Content.ColorTopRight.Color,
                                 window.Content.ColorBottomRight.Color,
                                 window.Content.ColorBottomRight.Color,
                                };

                            DrawQuad(gameWindow, dX + frameRight + contentWidth, dY, frameRight, pane.Height, texCoords, colors);

                            //Right

                            if (window.FrameCount == 2)
                                SetupShaders(pane, window.WindowFrames[1].Material, Textures);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            colors = new Color[] {
                                 window.Content.ColorTopLeft.Color,
                                 window.Content.ColorTopLeft.Color,
                                 window.Content.ColorBottomLeft.Color,
                                 window.Content.ColorBottomLeft.Color,
                                };

                            DrawQuad(gameWindow, dX, dY, frameLeft, pane.Height, texCoords, colors);
                        }
                        else if (window.WindowKind == WindowKind.HorizontalNoContent)
                        {
                            //Right

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

                            DrawQuad(gameWindow, dX + frameLeft, dY, pane.Width - frameLeft, pane.Height, texCoords, colors);

                            //Left

                            if (window.FrameCount == 2)
                                SetupShaders(pane, window.WindowFrames[1].Material, Textures);

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

                            DrawQuad(gameWindow, dX, dY, pane.Width - frameLeft, pane.Height, texCoords, colors);
                        }
                        else if (window.WindowKind == WindowKind.Around)
                        {

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

                            DrawQuad(gameWindow, dX, dY, pieceWidth, pieceHeight, texCoords, colors);

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

                            DrawQuad(gameWindow, dX + pane.Width - frameRight, dY, pieceWidth, pieceHeight, texCoords, colors);

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

                            DrawQuad(gameWindow, dX, dY - frameTop, pieceWidth, pieceHeight, texCoords, colors);

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

                            DrawQuad(gameWindow, dX + frameLeft, dY - pane.Height + frameBottom, pieceWidth, pieceHeight, texCoords, colors);
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

                            matTL.Shader.SetInt("flipTexture", (int)window.WindowFrames[0].TextureFlip);

                            float pieceWidth = pane.Width - frameRight;
                            float pieceHeight = frameTop;

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX, dY, pieceWidth, pieceHeight, texCoords, colors);
                        }
                        if (matTR.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matTR, Textures);

                            matTR.Shader.SetInt("flipTexture", (int)window.WindowFrames[1].TextureFlip);

                            float pieceWidth = frameRight;
                            float pieceHeight = pane.Height - frameBottom;

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1,(pane.Height - frameTop) / frameTop),
                                new Vector2(0,(pane.Height - frameTop) / frameTop),
                            };

                            DrawQuad(gameWindow, dX + pane.Width - frameRight, dY, pieceWidth, pieceHeight, texCoords, colors);
                        }
                        if (matBL.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matBL, Textures);

                            matBL.Shader.SetInt("flipTexture", (int)window.WindowFrames[2].TextureFlip);

                            float pieceWidth = frameLeft;
                            float pieceHeight = pane.Height - frameTop;

                            texCoords = new Vector2[]
                            {
                                new Vector2(0,1 - ((pane.Height - frameTop) / frameTop)),
                                new Vector2(1,1 - ((pane.Height - frameTop) / frameTop)),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX, dY - frameTop, pieceWidth, pieceHeight, texCoords, colors);
                        }
                        if (matBR.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matBR, Textures);

                            matBR.Shader.SetInt("flipTexture", (int)window.WindowFrames[3].TextureFlip);

                            float pieceWidth = pane.Width - frameLeft;
                            float pieceHeight = frameBottom;

                            texCoords = new Vector2[]
                            {
                                new Vector2(1 - ((pane.Width - frameLeft) / frameLeft), 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(1 - ((pane.Width - frameLeft) / frameLeft), 1),
                            };

                            DrawQuad(gameWindow, dX + frameLeft, dY - pane.Height + frameBottom, pieceWidth, pieceHeight, texCoords, colors);
                        }
                    }
                    break;
                case 8: //4 per corner, 4 per side
                    {
                        var matTL = window.WindowFrames[0].Material;
                        var matTR = window.WindowFrames[1].Material;
                        var matBL = window.WindowFrames[2].Material;
                        var matBR = window.WindowFrames[3].Material;

                        var matT = window.WindowFrames[4].Material;
                        var matB = window.WindowFrames[5].Material;
                        var matL = window.WindowFrames[6].Material;
                        var matR = window.WindowFrames[7].Material;

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
                            matTL.Shader.SetInt("flipTexture", (int)window.WindowFrames[0].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX, dY, frameLeft, frameTop, texCoords, colors);
                        }

                        if (matTR.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matTR, Textures);
                            matTR.Shader.SetInt("flipTexture", (int)window.WindowFrames[1].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX + pane.Width - frameRight, dY, frameRight, frameTop, texCoords, colors);
                        }

                        if (matBL.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matBL, Textures);
                            matBL.Shader.SetInt("flipTexture", (int)window.WindowFrames[2].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX, dY - pane.Height + frameTop, frameLeft, frameBottom, texCoords, colors);
                        }

                        if (matBR.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matBR, Textures);
                            matBR.Shader.SetInt("flipTexture", (int)window.WindowFrames[3].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX + pane.Width - frameLeft, dY - pane.Height + frameBottom, frameRight, frameBottom, texCoords, colors);
                        }

                        if (matT.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matT, Textures);
                            matT.Shader.SetInt("flipTexture", (int)window.WindowFrames[4].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 0),
                                new Vector2((pane.Width - frameLeft) / frameLeft, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX + frameLeft, dY, contentWidth, frameTop, texCoords, colors);
                        }

                        if (matB.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matB, Textures);
                            matB.Shader.SetInt("flipTexture", (int)window.WindowFrames[5].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(1-((pane.Width - frameLeft) / frameLeft), 0),
                                new Vector2(1, 0),
                                new Vector2(1, 1),
                                new Vector2(1-((pane.Width - frameLeft) / frameLeft), 1),
                            };

                            DrawQuad(gameWindow, dX + frameRight, dY - (pane.Height - frameBottom), contentWidth, frameTop, texCoords, colors);
                        }

                        if (matL.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matL, Textures);
                            matL.Shader.SetInt("flipTexture", (int)window.WindowFrames[6].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0,1-((pane.Height - frameTop) / frameTop)),
                                new Vector2(1,1-((pane.Height - frameTop) / frameTop)),
                                new Vector2(1, 1),
                                new Vector2(0, 1),
                            };

                            DrawQuad(gameWindow, dX, dY - frameTop, frameLeft, contentHeight, texCoords, colors);
                        }

                        if (matR.TextureMaps.Length > 0)
                        {
                            SetupShaders(pane, matR, Textures);
                            matR.Shader.SetInt("flipTexture", (int)window.WindowFrames[7].TextureFlip);

                            texCoords = new Vector2[]
                            {
                                new Vector2(0, 0),
                                new Vector2(1, 0),
                                new Vector2(1,(pane.Height - frameBottom) / frameBottom),
                                new Vector2(0,(pane.Height - frameBottom) / frameBottom),
                            };

                            DrawQuad(gameWindow, dX + (pane.Width - frameRight), dY - frameTop, frameRight, contentHeight, texCoords, colors);
                        }
                    }
                    break;
            }


            //Lastly draw a selection box if selected

            if (isSelected)
            {
                if (!Runtime.LayoutEditor.IsGamePreview && !gameWindow)
                    DrawSelectionBox(pane, isSelected);
            }
        }

        public static void DrawSelectionBox(BasePane pane, bool isSelected)
        {
            var rect = pane.CreateRectangle();
            rect = rect.RotateZ(pane.Rotate.Z);

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.Texture2D);
            GL.UseProgram(0);

            GL.Begin(PrimitiveType.LineLoop);
            GL.Color4(isSelected ? Color.Red : Color.Green);
            GL.Vertex2(rect.LeftPoint, rect.BottomPoint);
            GL.Vertex2(rect.RightPoint, rect.BottomPoint);
            GL.Vertex2(rect.RightPoint, rect.TopPoint);
            GL.Vertex2(rect.LeftPoint, rect.TopPoint);
            GL.End();

            if (isSelected)
            {
                var transformed = rect;
                var leftTop = new Vector2(transformed.LeftPoint, transformed.TopPoint);
                var left = new Vector2(transformed.LeftPoint, (transformed.BottomPoint + transformed.TopPoint) / 2);
                var leftBottom = new Vector2(transformed.LeftPoint, transformed.BottomPoint);
                var rightTop = new Vector2(transformed.RightPoint, transformed.TopPoint);
                var right = new Vector2(transformed.RightPoint, (transformed.BottomPoint + transformed.TopPoint) / 2);
                var rightBottom = new Vector2(transformed.RightPoint, transformed.BottomPoint);
                var top = new Vector2((transformed.RightPoint + transformed.LeftPoint) / 2, transformed.TopPoint);
                var bottom = new Vector2((transformed.RightPoint + transformed.LeftPoint) / 2, transformed.BottomPoint);

                DrawEdgeSquare(leftTop);
                DrawEdgeSquare(left);
                DrawEdgeSquare(leftBottom);
                DrawEdgeSquare(rightTop);
                DrawEdgeSquare(right);
                DrawEdgeSquare(rightBottom);
                DrawEdgeSquare(top);
                DrawEdgeSquare(bottom);
            }


            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.Texture2D);
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

        private static void DrawQuad(bool gameWindow, float x, float y, float w, float h, Vector2[] texCoords, Color[] colors)
        {
            if (!gameWindow && !Runtime.LayoutEditor.IsGamePreview)
            {
                GL.Disable(EnableCap.AlphaTest);
                GL.Disable(EnableCap.Blend);

                GL.LineWidth(0.5f);
                GL.Begin(PrimitiveType.LineLoop);
                GL.Color4(Color.Green);
                GL.Vertex2(x, y);
                GL.Vertex2(x + w, y);
                GL.Vertex2(x + w, y - h);
                GL.Vertex2(x, y - h);
                GL.End();
                GL.LineWidth(1f);

                GL.Enable(EnableCap.AlphaTest);
                GL.Enable(EnableCap.Blend);
            }

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
            if (mat.Shader == null)
            {
                if (mat is Cafe.BFLYT.Material)
                {
                    mat.Shader = new BflytShader((Cafe.BFLYT.Material)mat);
                    mat.Shader.Compile();
                }
            }

            mat.Shader.Enable();
            if (mat is Cafe.BFLYT.Material)
                ((BflytShader)mat.Shader).SetMaterials(pane, textures);
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
            GL.Vertex2(rect.LeftPoint, rect.BottomPoint);
            GL.Color4(colors[1]);
            GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[1].X, texCoords[1].Y);
            GL.Vertex2(rect.RightPoint, rect.BottomPoint);
            GL.Color4(colors[2]);
            GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[2].X, texCoords[2].Y);
            GL.Vertex2(rect.RightPoint, rect.TopPoint);
            GL.Color4(colors[3]);
            GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[3].X, texCoords[3].Y);
            GL.Vertex2(rect.LeftPoint, rect.TopPoint);
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


        public static BlendingFactor ConvertBlendFactor(BlendMode.GX2BlendFactor blendFactor)
        {
            switch (blendFactor)
            {
                case BlendMode.GX2BlendFactor.DestAlpha: return BlendingFactor.DstAlpha;
                case BlendMode.GX2BlendFactor.DestColor: return BlendingFactor.DstColor;
                case BlendMode.GX2BlendFactor.DestInvAlpha: return BlendingFactor.OneMinusDstAlpha;
                case BlendMode.GX2BlendFactor.DestInvColor: return BlendingFactor.OneMinusDstColor;
                case BlendMode.GX2BlendFactor.Factor0: return BlendingFactor.Zero;
                case BlendMode.GX2BlendFactor.Factor1: return BlendingFactor.One;
                case BlendMode.GX2BlendFactor.SourceAlpha: return BlendingFactor.SrcAlpha;
                case BlendMode.GX2BlendFactor.SourceColor: return BlendingFactor.SrcColor;
                case BlendMode.GX2BlendFactor.SourceInvAlpha: return BlendingFactor.OneMinusSrcAlpha;
                case BlendMode.GX2BlendFactor.SourceInvColor: return BlendingFactor.OneMinusSrcColor;
                default: return BlendingFactor.Zero;
            }
        }

        public static LogicOp ConvertLogicOperation(BlendMode.GX2LogicOp blendOp)
        {
            switch (blendOp)
            {
                case BlendMode.GX2LogicOp.And: return LogicOp.And;
                case BlendMode.GX2LogicOp.Clear: return LogicOp.Clear;
                case BlendMode.GX2LogicOp.Copy: return LogicOp.Copy;
                case BlendMode.GX2LogicOp.Equiv: return LogicOp.Equiv;
                case BlendMode.GX2LogicOp.Inv: return LogicOp.Invert;
                case BlendMode.GX2LogicOp.Nand: return LogicOp.Nand;
                case BlendMode.GX2LogicOp.NoOp: return LogicOp.Noop;
                case BlendMode.GX2LogicOp.Nor: return LogicOp.Nor;
                case BlendMode.GX2LogicOp.Or: return LogicOp.Or;
                case BlendMode.GX2LogicOp.RevAnd: return LogicOp.AndReverse;
                case BlendMode.GX2LogicOp.RevOr: return LogicOp.OrReverse;
                case BlendMode.GX2LogicOp.Set: return LogicOp.Set;
                case BlendMode.GX2LogicOp.Xor: return LogicOp.Xor;
                case BlendMode.GX2LogicOp.Disable:
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

        public static BlendEquationMode ConvertBlendOperation(BlendMode.GX2BlendOp blendOp)
        {
            switch (blendOp)
            {
                case BlendMode.GX2BlendOp.Add: return BlendEquationMode.FuncAdd;
                case BlendMode.GX2BlendOp.ReverseSubtract: return BlendEquationMode.FuncReverseSubtract;
                case BlendMode.GX2BlendOp.SelectMax: return BlendEquationMode.Max;
                case BlendMode.GX2BlendOp.SelectMin: return BlendEquationMode.Min;
                case BlendMode.GX2BlendOp.Subtract: return BlendEquationMode.FuncSubtract;
                case BlendMode.GX2BlendOp.Disable:
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
                float outAlpha = BasePane.MixColors(colors[i].A, alpha);
                colors[i] = Color.FromArgb(Utils.FloatToIntClamp(outAlpha), colors[i]);
            }

            if (LayoutEditor.UseLegacyGL)
            {
                if (useLines)
                {
                    GL.Begin(PrimitiveType.LineLoop);
                    GL.Color4(selectionOutline ? Color.Red : colors[0]);
                    GL.Vertex2(rect.LeftPoint, rect.BottomPoint);
                    GL.Vertex2(rect.RightPoint, rect.BottomPoint);
                    GL.Vertex2(rect.RightPoint, rect.TopPoint);
                    GL.Vertex2(rect.LeftPoint, rect.TopPoint);
                    GL.End();
                }
                else
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Color4(colors[0]);
                    GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[0].X, texCoords[0].Y);
                    GL.Vertex2(rect.LeftPoint, rect.BottomPoint);
                    GL.Color4(colors[1]);
                    GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[1].X, texCoords[1].Y);
                    GL.Vertex2(rect.RightPoint, rect.BottomPoint);
                    GL.Color4(colors[2]);
                    GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[2].X, texCoords[2].Y);
                    GL.Vertex2(rect.RightPoint, rect.TopPoint);
                    GL.Color4(colors[3]);
                    GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[3].X, texCoords[3].Y);
                    GL.Vertex2(rect.LeftPoint, rect.TopPoint);
                    GL.End();
                }

                if (!Runtime.LayoutEditor.IsGamePreview && !gameWindow)
                {
                    DrawSelectionBox(pane, selectionOutline);
                }
            }
            else
            {
                if (pane.renderablePane == null)
                    pane.renderablePane = new RenderablePane();

                Vector3[] vertices = new Vector3[4];
                vertices[0] = new Vector3(rect.LeftPoint, rect.BottomPoint, 0);
                vertices[1] = new Vector3(rect.RightPoint, rect.BottomPoint, 0);
                vertices[2] = new Vector3(rect.RightPoint, rect.TopPoint, 0);
                vertices[3] = new Vector3(rect.LeftPoint, rect.TopPoint, 0);
                Vector4[] vertexColors = new Vector4[4];

            //    pane.renderablePane.Render(vertices, vertexColors, texCoords);
            }
        }

        private static void DrawEdgeSquare(Vector2 position)
        {
            float scale = 5;

            GL.Begin(PrimitiveType.LineLoop);
            GL.Color4(Color.Red);
            GL.Vertex2(position.X + -1 * scale, position.Y + -1 * scale);
            GL.Vertex2(position.X + 1 * scale, position.Y + -1 * scale);
            GL.Vertex2(position.X + 1 * scale, position.Y + 1 * scale);
            GL.Vertex2(position.X + -1 * scale, position.Y + 1 * scale);
            GL.End();
        }
    }
}

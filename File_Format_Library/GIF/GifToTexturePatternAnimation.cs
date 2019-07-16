using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using Toolbox.Library.Animations;
using Toolbox.Library;
using Bfres.Structs;
using Syroot.NintenTools.NSW.Bfres;
using ResU = Syroot.NintenTools.Bfres;

namespace FirstPlugin.Forms
{
    public class GifToTexturePatternAnimation
    {
        public GifToTexturePatternAnimation(string FileName, BFRESGroupNode TargetFTEXFolder, FTXP ftxp)
        {
            string TextureName = System.IO.Path.GetFileNameWithoutExtension(FileName);

            string ext = Utils.GetExtension(FileName);

            ImageKeyFrame[] Images = new ImageKeyFrame[0];

            if (ext == ".gif")
            {
                Image gifImg = Image.FromFile(FileName);

                //Add all the images
                Images = GetImages(gifImg);
            }
            if (ext == ".png" || ext == ".apng")
            {
                APNG png = new APNG();
                png.Load(FileName);
                for (int i = 0; i < png.NumEmbeddedPNG; i++)
                {
                    Images[i] = new ImageKeyFrame()
                    {
                        Frame = i,
                        Image = png.ToBitmap(i)
                    };
                }
            }

            TargetFTEXFolder.ImportTexture(Images, TextureName);

            //Now load the key data to the animation
            ftxp.TexPatternAnim = new ResU.TexPatternAnim();
            ftxp.TexPatternAnim.Name = ftxp.Text;
            ftxp.TexPatternAnim.Path = "";
            ftxp.TexPatternAnim.FrameCount = Images.Length;
            ftxp.TexPatternAnim.TextureRefs = new ResU.ResDict<ResU.TextureRef>();
            foreach (ImageKeyFrame key in Images)
            {
                string name = $"{TextureName}{key.Frame}";

                ftxp.TexPatternAnim.TextureRefs.Add($"{TextureName}{key.Frame}",
                    new ResU.TextureRef()
                    {
                        Name = $"{name}",
                        Texture = ((FTEX)TargetFTEXFolder.ResourceNodes[name]).texture,
                    });
            }

            var material = new ResU.TexPatternMatAnim();
            material.Name = "NewMaterial";

            material.PatternAnimInfos = new List<ResU.PatternAnimInfo>();

            material.BaseDataList.Add(0);

            ResU.PatternAnimInfo info = new ResU.PatternAnimInfo();
            info.CurveIndex = 0;
            info.SubBindIndex = -1;
            info.Name = "_a0";
            material.PatternAnimInfos.Add(info);

            ftxp.TexPatternAnim.TexPatternMatAnims.Add(material);
            ResU.AnimCurve curve = new ResU.AnimCurve();
            curve.AnimDataOffset = 0;
            curve.CurveType = ResU.AnimCurveType.StepInt;
            curve.Delta = 0;
            curve.EndFrame = Images.Length;
            curve.StartFrame = 0;
            curve.FrameType = ResU.AnimCurveFrameType.Byte;
            curve.Scale = 1;
            curve.Offset = 0;
            curve.Frames = new float[(int)curve.EndFrame];

            for (int i = 0; i < curve.EndFrame; i++)
            {
                curve.Frames[i] = Images[i].Frame;
            }

            curve.Keys = new float[(int)curve.Frames.Length, 1];
            for (int i = 0; i < (ushort)curve.Keys.Length; i++)
            {
                int index = ftxp.TexPatternAnim.TextureRefs.IndexOf($"{TextureName}{Images[i].Frame}");
                curve.Keys[i, 0] = index;
            }

            material.Curves.Add(curve);

            ftxp.UpdateMaterialBinds();
        }

        public GifToTexturePatternAnimation(string FileName, BNTX TargetBNTX, FMAA fmaa)
        {
            string TextureName = System.IO.Path.GetFileNameWithoutExtension(FileName);

            Image gifImg = Image.FromFile(FileName);

            //Add all the images
            var images = GetImages(gifImg);
            TargetBNTX.ImportTexture(images, TextureName);

            //Now load the key data to the animation
            fmaa.MaterialAnim = new Syroot.NintenTools.NSW.Bfres.MaterialAnim();
            fmaa.MaterialAnim.Name = fmaa.Text;
            fmaa.MaterialAnim.Path = "";
            fmaa.MaterialAnim.Loop = true;
            fmaa.MaterialAnim.FrameCount = images.Length;
            fmaa.MaterialAnim.TextureNames = new List<string>();
            foreach (ImageKeyFrame key in images)
            {
                fmaa.MaterialAnim.TextureNames.Add($"{TextureName}{key.Frame}");
            }

            var material = new MaterialAnimData();
            material.Name = "NewMaterial";

            material.TexturePatternAnimInfos = new List<TexturePatternAnimInfo>();
            material.TexturePatternCurveIndex = 0;
            material.BeginVisalConstantIndex = 0;

            TexturePatternAnimInfo info = new TexturePatternAnimInfo();
            info.CurveIndex = 0;
            info.SubBindIndex = -1;
            info.BeginConstant = ushort.MaxValue;
            info.Name = "_a0";
            material.TexturePatternAnimInfos.Add(info);

            fmaa.MaterialAnim.MaterialAnimDataList.Add(material);
            AnimCurve curve = new AnimCurve();
            curve.AnimDataOffset = 0;
            curve.CurveType = AnimCurveType.StepInt;
            curve.Delta = 0;
            curve.EndFrame = images.Length;
            curve.StartFrame = 0;
            curve.FrameType = AnimCurveFrameType.Byte;
            curve.Scale = 1;
            curve.Offset = 0;
            curve.Frames = new float[(int)curve.EndFrame];

            for (int i = 0; i < curve.EndFrame; i++)
            {
                curve.Frames[i] = images[i].Frame;
            }

            curve.Keys = new float[(int)curve.Frames.Length, 1];
            for (int i = 0; i < (ushort)curve.Keys.Length; i++)
            {
                int index = fmaa.MaterialAnim.TextureNames.IndexOf($"{TextureName}{images[i].Frame}");
                curve.Keys[i, 0] = index;
            }

            material.Curves.Add(curve);

            fmaa.UpdateMaterialBinds();
        }

        private ImageKeyFrame[] GetImages(Image image)
        {
            FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);
            // Number of frames
            int frameCount = image.GetFrameCount(dimension);
            // Return an Image at a certain index

            ImageKeyFrame[] images = new ImageKeyFrame[frameCount];
            for (int frame = 0; frame < frameCount; frame++)
            {
                images[frame] = new ImageKeyFrame();
                images[frame].Image = new Bitmap(image);
                images[frame].Frame = frame;

                image.SelectActiveFrame(dimension, frame);
            }
            return images;
        }
    }
}

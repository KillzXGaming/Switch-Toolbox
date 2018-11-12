using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using System.Drawing;
using GL_Core.Interfaces;

namespace Switch_Toolbox.Library
{
    //Thanks to Smash Forge for a few of these!
   // https://github.com/jam1garner/Smash-Forge/blob/master/Smash%20Forge/Runtime.cs

    public class Runtime
    {
        public static List<AbstractGlDrawable> abstractGlDrawables = new List<AbstractGlDrawable>();

        public static DockState objectListDockState = DockState.DockLeft;

        public static bool RenderModels = true;
        public static bool RenderModelSelection = true;
        public static bool RenderModelWireframe = false;
        public static ViewportShading viewportShading;
        public static bool IsDebugMode = false; //Enables experimental features and other things to debug. 

        public static bool enableVSync = false;
        public static float floorSize = 30f;
        public static Color floorColor = Color.Gray;
        public static FloorStyle floorStyle = FloorStyle.WireFrame;
        public static PictureBoxBG pictureBoxStyle = PictureBoxBG.Checkerboard;
        public static float previewScale = 1.0f;

        public static bool renderFloorLines = true;

        //Viewport Background
        public static BackgroundStyle backgroundStyle = BackgroundStyle.Gradient;
        public static bool renderBackGround = true;
        public static string backgroundTexFilePath = "";
        public static Color backgroundGradientTop = Color.FromArgb(255, 26, 26, 26);
        public static Color backgroundGradientBottom = Color.FromArgb(255, 77, 77, 77);
        public static float zoomspeed = 1.25f;
        public static float zoomModifierScale = 2.0f;
        public static bool cameraLight = false;
        public static bool DisplayPolyCount = true;
        public static float PolyCount = 0;
        public static float VertCount = 0;

        public static bool enableOpenTKDebugOutput = false;

        public static bool OpenStartupWindow = true;

        // Toggle Render Passes
        public static bool renderDiffuse = true;
        public static bool renderFresnel = true;
        public static bool renderSpecular = true;
        public static bool renderReflection = true;
        public static bool renderBoundingBoxes = true;
        public static bool renderNormalMap = true;
        public static bool renderVertColor = true;
        public static bool renderBfresPbr = false;

        public static bool stereoscopy = false;
        public static bool UseLegacyGL = false;

        public static bool useNormalMap = true;

        public static CameraMovement cameraMovement;
        public static CameraView cameraView;

        public static ThumbnailSize thumbnailSize = ThumbnailSize.Small;

        public static float CameraNear = 0.1f;
        public static float CameraFar = 100000.0f;
        public static ActiveGame activeGame = ActiveGame.SMO;

        public enum ActiveGame
        {
            SMO,
            MK8D,
            ARMs,
            Splatoon2,
            BOTW,
            KSA,
        }

        public enum PictureBoxBG
        {
            Checkerboard,
            Black,
        }

        public enum CameraMovement
        {
            Inspect,
            Walk,
        }
        public enum CameraView
        {
            Perspective,
            Orthographic,
        }

        public enum ThumbnailSize
        {
            Small,
            Medium,
            Large
        }

        public enum BackgroundStyle
        {
            Gradient = 0,
            UserTexture = 1,
            Solid = 2,
        }

        public enum FloorStyle
        {
            WireFrame = 0,
            UserTexture = 1,
            Solid = 2,
        }

        public enum ViewportShading
        {
            Default = 0,
            Normal = 1,
            Lighting = 2,
            Diffuse = 3,
            NormalMap = 4,
            VertColor = 5,
            AmbientOcclusion = 6,
            UVCoords = 7,
            UVTestPattern = 8,
            Tangents = 9,
            Bitangents = 10,
            LightMap = 11,
            SelectedBoneWeights = 12,
            SpecularMap = 13,
            ShadowMap = 14,
            MetalnessMap = 15,
            RoughnessMap = 16,
            SubSurfaceScatteringMap = 17,
            EmmissionMap = 18,
        }
        public enum UVChannel
        {
            Channel1 = 1,
            Channel2 = 2,
            Channel3 = 3
        }

        // Debug Shading
        public static bool renderR = true;
        public static bool renderG = true;
        public static bool renderB = true;
        public static bool renderAlpha = true;
        public static UVChannel uvChannel = UVChannel.Channel1;


        // OpenGL System Information
        public static string renderer = "";
        public static string openGLVersion = "";
        public static string GLSLVersion = "";
    }
}

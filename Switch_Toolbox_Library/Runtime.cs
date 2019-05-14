using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.EditorDrawables;
using System.Windows.Forms;

namespace Switch_Toolbox.Library
{
    //Thanks to Smash Forge for a few of these!
   // https://github.com/jam1garner/Smash-Forge/blob/master/Smash%20Forge/Runtime.cs

    public class Runtime
    {
        public static string Mk8GamePath = "";
        public static string Mk8dGamePath = "";
        public static string SmoGamePath = "";
        public static string TpGamePath = "";
        public static string BotwGamePath = "";

        public class ImageEditor
        {
            public static bool ShowPropertiesPanel = true;
            public static bool DisplayVertical = false;


            public static bool DisplayAlpha = true;
            public static bool UseComponetSelector = true;

            public static bool EnableImageZoom = true;
            public static bool EnablePixelGrid = false;
        }
        public class ObjectEditor
        {
            public static bool OpenModelsOnOpen = false;

            public static Point Location = new Point(364, 0);

            public static int ListPanelWidth;
        }

        //Used for experimental edits
        public static bool useEditDebugMode = false;
        public static bool UseEditDebugMode
        {
            get
            {
                if (useEditDebugMode == false)
                {
                  var result =  MessageBox.Show("This feature is experimental and could lead to potential crashes. Are you sure you want to continue?",
                        "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.DefaultDesktopOnly);

                    if (result == DialogResult.Yes)
                        useEditDebugMode = true;
                }
                return useEditDebugMode;
            }
        }

        public static int SelectedBoneIndex = -1;

        //These are so we can load multiple drawables in the viewport
        //Each viewport can carry it's own set of drawables
        //When a viewport is made it will add a ViewportEditor instance and add drawables
        //These will be switchable in settings to load specic objects in the scene or display all of them at once
        public static List<ViewportEditor> viewportEditors = new List<ViewportEditor>();
        public class ViewportEditor
        {
            public List<EditableObject> editableDrawables = new List<EditableObject>();
            public List<AbstractGlDrawable> staticDrawables = new List<AbstractGlDrawable>();

            public static int Width = 638;
        }

        public static TEX_FORMAT PreferredTexFormat = TEX_FORMAT.BC1_UNORM_SRGB;

        public static string ExecutableDir;

        public static Form MainForm;

        public static bool UseDebugDomainExceptionHandler;
        public static bool DisableUpdatePrompt;

        public static bool MaximizeMdiWindow = false;

        public static bool AddFilesToActiveObjectEditor = true;

        public class PBR
        {
            public static bool UseSkybox = false;
            public static bool UseDiffuseSkyTexture = true;
            
            public static string DiffuseCubeMapPath = "";
            public static string SpecularCubeMapPath = "";
            public static string BRSFMapPath = "";
        }

        public static GridSettings gridSettings = new GridSettings();
        public class GridSettings
        {
            public float CellSize = 1.0f;
            public uint CellAmount = 10;
            public Color color = Color.FromArgb(90, 90, 90);
        }

        public static float MaxCameraSpeed = 0.1f;

        public static int Yaz0CompressionLevel = 3;
        public static bool RenderModels = true;
        public static bool RenderModelSelection = true;
        public static bool RenderModelWireframe = false;
        public static ViewportShading viewportShading;
        public static bool IsDebugMode = true; //Enables experimental features and other things to debug. 
        public static bool EnableVersionCheck = true;
        public static bool EnablePBR = true;

        public static bool enableVSync = false;
        public static float floorSize = 30f;
        public static Color floorColor = Color.Gray;
        public static FloorStyle floorStyle = FloorStyle.WireFrame;
        public static PictureBoxBG pictureBoxStyle = PictureBoxBG.Checkerboard;
        public static float previewScale = 1.0f;

        public static bool renderFloorLines = true;

        public static bool UseOpenGL = true; //Removes all acess to opengl functionality. Useful for old PCs
        public static bool DisplayViewport = true; //Only displays it in editors if true

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

        public static bool OpenStartupWindow = false;

        // Toggle Render Passes
        public static bool renderDiffuse = true;
        public static bool renderFresnel = true;
        public static bool renderSpecular = true;
        public static bool renderReflection = true;
        public static bool renderBoundingBoxes = false;
        public static bool renderNormalMap = true;
        public static bool renderVertColor = true;
        public static bool renderBfresPbr = false;
        public static bool renderNormalsPoints = false;
        public static bool renderBones = true;
        public static bool renderFog = true;
        
        public static float bonePointSize = 0.2f;

        public static bool boneXrayDisplay = false;

        public static float normalsLineLength = 1;

        public static bool stereoscopy = false;
        public static bool UseLegacyGL = false;
        public static bool OpenTKInitialized = false;

        public static bool useNormalMap = true;

        public static CameraMovement cameraMovement;
        public static CameraView cameraView;

        public static ThumbnailSize thumbnailSize = ThumbnailSize.Small;

        public static float CameraNear = 0.1f;
        public static float CameraFar = 100000.0f;
        public static ActiveGame activeGame = ActiveGame.SMO;

        public static string ProgramVersion = "0.0.0";
        public static string CompileDate = "0/0/0000";
        public static string CommitInfo = "";

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
            White,
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

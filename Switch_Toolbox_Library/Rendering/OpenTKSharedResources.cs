using System;
using System.Runtime.ExceptionServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using SFGraphics.GLObjects.Shaders;

namespace Switch_Toolbox.Library.Rendering
{
    public static class OpenTKSharedResources
    {
        public enum SharedResourceStatus
        {
            Initialized,
            Failed,
            Unitialized
        }

        public static SharedResourceStatus SetupStatus
        {
            get { return setupStatus; }
        }
        private static SharedResourceStatus setupStatus = SharedResourceStatus.Unitialized;

        // Keep a context around to avoid setting up after making each context.
        public static GameWindow dummyResourceWindow;

        public static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

        private static DebugProc debugProc;

        [HandleProcessCorruptedStateExceptions]
        public static void InitializeSharedResources()
        {
            // Only setup once. This is checked multiple times to prevent crashes.
            if (setupStatus == SharedResourceStatus.Initialized)
                return;

            try
            {
                // Make a permanent context to share resources.
                GraphicsContext.ShareContexts = true;
                dummyResourceWindow = CreateGameWindowContext();

                if (Runtime.enableOpenTKDebugOutput)
                    EnableOpenTKDebugOutput();

                RenderTools.LoadTextures();
                GetOpenGLSystemInfo();
                ShaderTools.SetUpShaders();

                setupStatus = SharedResourceStatus.Initialized;
            }
            catch (AccessViolationException)
            {
                // Context creation failed.
                setupStatus = SharedResourceStatus.Failed;
            }
        }

        public static void EnableOpenTKDebugOutput()
        {
#if DEBUG
            // This isn't free, so skip this step when not debugging.
            // TODO: Only works with Intel integrated.
            if (SFGraphics.Tools.OpenGLExtensions.IsAvailable("GL_KHR_debug"))
            {
                GL.Enable(EnableCap.DebugOutput);
                GL.Enable(EnableCap.DebugOutputSynchronous);
                debugProc = DebugCallback;
                GL.DebugMessageCallback(debugProc, IntPtr.Zero);
                int[] ids = { };
                GL.DebugMessageControl(DebugSourceControl.DontCare, DebugTypeControl.DontCare,
                    DebugSeverityControl.DontCare, 0, ids, true);
            }
#endif
        }

        private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string debugMessage = Marshal.PtrToStringAnsi(message, length);
            Debug.WriteLine(String.Format("{0} {1} {2}", severity, type, debugMessage));
        }

        public static GameWindow CreateGameWindowContext(int width = 640, int height = 480)
        {
            GraphicsMode mode = new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 0, 0, ColorFormat.Empty, 1);

            // TODO: Versions higher than 300 do not work for some reason.
            GameWindow gameWindow = new GameWindow(width, height, mode, "", OpenTK.GameWindowFlags.Default, OpenTK.DisplayDevice.Default, 3, 0, GraphicsContextFlags.Default);

            gameWindow.Visible = false;
            gameWindow.MakeCurrent();
            return gameWindow;
        }

        private static void GetOpenGLSystemInfo()
        {
            Runtime.renderer = GL.GetString(StringName.Renderer);
            Runtime.openGLVersion = GL.GetString(StringName.Version);
            Runtime.GLSLVersion = GL.GetString(StringName.ShadingLanguageVersion);
        }
    }
}

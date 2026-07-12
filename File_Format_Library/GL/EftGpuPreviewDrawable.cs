using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;

namespace FirstPlugin
{
    /// <summary>
    /// Viewport adapter for the GPU emitter preview: hosts an EftGpuPreview (raw GL, camera-agnostic)
    /// as a GL_EditorFramework drawable. It supplies the camera (the uniform-bank view/projection
    /// convention, row-major rows for column vectors, is the TRANSPOSE of the control's OpenTK
    /// row-vector matrices) and the playback clock (60fps wall time, repainting like the software
    /// preview). GL cleanup is deferred to the next Draw of any instance, where a context is
    /// current; EftGpuPreview.Dispose skips the objects that context does not own.
    /// </summary>
    public class EftGpuPreviewDrawable : AbstractGlDrawable
    {
        readonly EftGpuPreview preview;
        readonly EftEmitterRender.PlaybackClock clock = new EftEmitterRender.PlaybackClock();
        bool live = true;
        bool framed;

        // Starting orbit, applied once at first framing (after FrameSelect sets target and distance).
        // The framework camera orbits CameraTarget with camRotX = yaw, camRotY = pitch
        // (mtxCam = T(-target) * RotY(camRotX) * RotX(camRotY) * T(0,0,-dist)). A slight 3/4 look-down
        // keeps flat effects (smoke/dust discs, ground planes) from spawning edge-on; interaction,
        // zoom and pan are unchanged.
        const float CAM_YAW   = 0.5f;    // ~28 deg around (a 3/4 side view)
        const float CAM_PITCH = 0.45f;   // ~26 deg looking DOWN from above the effect

        static readonly List<EftGpuPreview> dead = new List<EftGpuPreview>();

        public EftGpuPreviewDrawable(byte[] vtxGroup, byte[] fragGroup, byte[] payload,
                                     IList<EftGpuPreview.TextureInput> artTextures,
                                     float[] meshVerts = null, float[] meshNormals = null, int[] meshIndices = null)
        {
            preview = new EftGpuPreview(vtxGroup, fragGroup, payload, artTextures, meshVerts, meshNormals, meshIndices);
        }

        public string Error { get { return preview.Error; } }

        /// <summary>Frame the camera on this emitter's extent when it is first drawn. The editor sets it only for
        /// the first emitter of a session, so switching emitters afterwards leaves the user's view where it was.</summary>
        public bool AutoFrame = true;

        /// <summary>Raised from Draw (UI thread) the first time the inner preview reports an
        /// init/render error; the editor shows it instead of a silent blank pane.</summary>
        public Action<string> ErrorReported;
        bool errorSent;

        public override void Prepare(GL_ControlModern control) { }
        public override void Prepare(GL_ControlLegacy control) { }
        public override void Draw(GL_ControlLegacy control, Pass pass) { }

        public override void Draw(GL_ControlModern control, Pass pass)
        {
            lock (dead)
            {
                foreach (var p in dead) p.Dispose();
                dead.Clear();
            }
            if (!live || pass != Pass.TRANSPARENT) return;
            // 60fps wall-clock playback, independent of the monitor's refresh rate and of paint load
            // (see EftEmitterRender.PlaybackClock)
            preview.Render(Transpose(control.CameraMatrix), Transpose(control.ProjectionMatrix), clock.Advance());
            GL.UseProgram(0);   //the framework's shader wrapper re-binds per pass; leave no raw program current
            if (!errorSent && preview.Error != null)
            {
                errorSent = true;
                try { if (ErrorReported != null) ErrorReported(preview.Error); } catch { }
            }
            // auto-fit the camera to the effect's own extent once it is known (faithful world
            // units vary from item glints to 30-unit haze volumes; the software preview frames
            // the same way)
            if (AutoFrame && !framed && preview.BoundsRadius > 0)
            {
                framed = true;
                try { control.FrameSelect(new List<Vector4> { new Vector4(0f, 0f, 0f, preview.BoundsRadius * 1.5f) }); } catch { }
                // orbit to a 3/4 look-down starting angle (target/distance already set by FrameSelect)
                try { control.CamRotX = CAM_YAW; control.CamRotY = CAM_PITCH; } catch { }
            }
            // the repaint drives the playback clock; paused while another window is in front
            try { EftEmitterRender.RequestNextFrame(control); } catch { }
        }

        static double[] Transpose(Matrix4 m)
        {
            var d = new double[16];
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    d[r * 4 + c] = m[c, r];
            return d;
        }

        /// <summary>Hand the inner preview's GL objects to the next Draw for deletion, which runs with a
        /// context current (Dispose itself skips any object that context does not own).</summary>
        public void QueueDispose()
        {
            lock (dead)
            {
                if (!live) return;
                live = false;
                dead.Add(preview);
            }
        }
    }
}

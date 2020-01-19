using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;
using Toolbox.Library.Animations;

namespace Toolbox.Library.Forms
{
    public class CurveEditor : Viewport2D
    {
        public List<STAnimationTrack> Tracks = new List<STAnimationTrack>();

        public static float ScaleX = 5;
        public static float ScaleY = 5;

        public CurveEditor()
        {
        }

        public override List<IPickable2DObject> GetPickableObjects()
        {
            List<IPickable2DObject> points = new List<IPickable2DObject>();
            return points;
        }

        public override void RenderScene()
        {
            DrawFrameCounter();
            DrawKeyCurves();
        }

        private void DrawKeyCurves()
        {
            if (Tracks.Count == 0) return;

            foreach (var track in Tracks)
            {
                if (track.KeyFrames.Count == 0)
                    continue;

                var frameCount = track.KeyFrames.Max(x => x.Frame);

                var firstKey = track.KeyFrames.FirstOrDefault();
                if (track.Name.EndsWith("R"))
                    GL.Color4(Color.Red);
                if (track.Name.EndsWith("G"))
                    GL.Color4(Color.Green);
                if (track.Name.EndsWith("B"))
                    GL.Color4(Color.Blue);
                if (track.Name.EndsWith("A"))
                    GL.Color4(Color.Gray);

                if (track.Name.EndsWith("X"))
                    GL.Color4(Color.Red);
                if (track.Name.EndsWith("Y"))
                    GL.Color4(Color.Green);
                if (track.Name.EndsWith("Z"))
                    GL.Color4(Color.Blue);
                if (track.Name.EndsWith("w"))
                    GL.Color4(Color.Orange);

                GL.Begin(PrimitiveType.LineStrip);

                //Draw curve
                for (int frame = 0; frame < frameCount; frame++)
                {
                    var value = track.GetFrameValue(frame);
                    var position = new Vector2(frame * ScaleX, value);
                    GL.Vertex2(position);
                }

                GL.End();

                float scale = Math.Max(2, (1 - Camera.Zoom) * 15);

                //Draw points
                for (int i = 0; i < track.KeyFrames.Count; i++)
                {
                    var point = new Vector3(track.KeyFrames[i].Frame * CurveEditor.ScaleX, track.KeyFrames[i].Value, 0);
                    Render2D.DrawFilledCircle(point, Color.White, scale, 180, true);
                }
            }
        }

        private void DrawFrameCounter()
        {
            float Scale = 1 - Camera.Zoom + 5;

            GL.PushAttrib(AttribMask.ColorBufferBit);
            GL.Color3(Color.FromArgb(80, 80, 80));
            for (int i = -1000; i < 1000; i++)
            {
                float x = i * ScaleX;

                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(x, -5.0f);
                GL.Vertex2(x, 5.0f);
                GL.End();
            }

            for (int i = -1000; i < 1000; i++)
            {
                float y = i * ScaleY;

                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(-5.0f, y);
                GL.Vertex2(5.0f, y);
                GL.End();
            }

            GL.Color3(Color.FromArgb(150, 150, 150));
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(-1000 * ScaleX, 0.0f);
            GL.Vertex2(1000 * ScaleX, 0.0f);
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(0.0f, -1000 * ScaleY);
            GL.Vertex2(0.0f, 1000 * ScaleY);
            GL.End();

            GL.PopAttrib();
        }
    }
}

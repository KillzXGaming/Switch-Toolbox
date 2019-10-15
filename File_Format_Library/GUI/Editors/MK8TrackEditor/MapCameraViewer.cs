using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;
using System.Drawing;
using FirstPlugin.Turbo;
using OpenTK;

namespace FirstPlugin.Turbo
{
    public class MapCameraViewer : Viewport2D
    {
        private KCL CollisionFile;
        private KCLRendering2D KCLRender;
        private Course_MapCamera_bin MapCamera;

        public void LoadCollision(KCL kcl) {
            CollisionFile = kcl;
            KCLRender = new KCLRendering2D(kcl);
        }

        public void LoadCameraFile(Course_MapCamera_bin camera) {
            MapCamera = camera;
        }

        public override void RenderSceme()
        {
            if (MapCamera == null) return;

            GL.PushMatrix();

            GL.Scale(0.1f, 0.1f ,1.0f);

            var cam = MapCamera.cameraData;

            if (CollisionFile != null)
                DrawCollision(CollisionFile);

            Render2D.DrawRectangle(cam.BoundingWidth, cam.BoundingHeight, Color.White, true);

            Render2D.DrawCircle(new Vector2(cam.PositionX, cam.PositionY), Color.Red);
            Render2D.DrawCircle(new Vector2(cam.TargetX, cam.TargetY), Color.Green);

            GL.PopMatrix();
        }

        private void DrawCollision(KCL kcl) {
            KCLRender.Draw(Camera.ModelViewMatrix);
        }
    }
}

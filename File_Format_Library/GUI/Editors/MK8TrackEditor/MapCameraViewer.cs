using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library.IO;
using Toolbox.Library;
using System.Drawing;
using FirstPlugin.Turbo;
using OpenTK;

namespace FirstPlugin.Turbo
{
    public class CameraPoint : IPickable2DObject
    {
        public bool IsTarget = false;

        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }

        public Vector3 Translate
        {
            get
            {
                Vector3 translate = new Vector3();
                if (IsTarget)
                    translate = new Vector3(cameraData.TargetX,
                        cameraData.TargetY, cameraData.TargetZ);
                else
                    translate = new Vector3(cameraData.PositionX,
                   cameraData.PositionY, cameraData.PositionZ);
                return translate;
            }
            set
            {
                if (IsTarget)
                {
                    cameraData.TargetX = value.X;
                    cameraData.TargetY = value.Y;
                    cameraData.TargetZ = value.Z;
                }
                else
                {
                    cameraData.PositionX = value.X;
                    cameraData.PositionY = value.Y;
                    cameraData.PositionZ = value.Z;
                }
            }
        }

        public bool IsHit(float X, float Y)
        {
            return new STRectangle(Translate.X, Translate.Z, 500, 500).IsHit((int)X, (int)Y);
        }

        public void PickTranslate(float X, float Y, float Z)
        {
            Translate -= new Vector3(X,Z,Y);
        }

        public void PickRotate(float X, float Y, float Z) { }
        public void PickScale(float X, float Y, float Z) { }

        Course_MapCamera_bin.CameraData cameraData;

        public CameraPoint(Course_MapCamera_bin.CameraData data, bool isTarget)
        {
            cameraData = data;
            IsTarget = isTarget;
        }

        public void Draw()
        {
            GL.Disable(EnableCap.DepthTest);

            Color color = Color.Red;
            if (IsTarget)
                color = Color.Green;
            if (IsHovered)
                color = Color.Yellow;
            if (IsSelected)
                color = Color.Blue;

            Render2D.DrawFilledCircle(new Vector3(Translate.X, Translate.Z, Translate.Y), color, 300, 40, true);

            GL.Enable(EnableCap.DepthTest);
        }
    }

    public class MapCameraViewer : Viewport2D
    {
        public override float PreviewScale => 0.1f;

        public byte CameraMapTransparency = 140;

        private MK8MapCameraEditor ParentEditor;
        private Course_MapCamera_bin MapCamera;

        private List<CameraPoint> CameraPoints = new List<CameraPoint>();

        private STGenericTexture MapCameraTexture;

        public void LoadCameraFile(Course_MapCamera_bin camera, MK8MapCameraEditor editor) {
            MapCamera = camera;
            ParentEditor = editor;

            CameraPoints.Add(new CameraPoint(camera.cameraData, true));
            CameraPoints.Add(new CameraPoint(camera.cameraData, false));

            //Try to load the mini map
            var folder = System.IO.Path.GetDirectoryName(camera.FilePath);
            if (System.IO.File.Exists($"{folder}/course_maptexture.bflim"))
            {
                var fileFormat = STFileLoader.OpenFileFormat($"{folder}/course_maptexture.bflim");
                if (fileFormat is BFLIM)
                    MapCameraTexture = fileFormat as BFLIM;
            }
            if (System.IO.File.Exists($"{folder}/course_maptexture.bntx"))
            {
                var fileFormat = STFileLoader.OpenFileFormat($"{folder}/course_maptexture.bntx");
                if (fileFormat is BNTX)
                    MapCameraTexture = ((BNTX)fileFormat).Textures.Values.FirstOrDefault();
            }
        }

        public override List<IPickable2DObject> GetPickableObjects()
        {
            var picks = new List<IPickable2DObject>();
            foreach (var point in CameraPoints)
                picks.Add(point);

            return picks;
        }

        private float GetAngle()
        {
            var data = MapCamera.cameraData;

            float xDiff = data.PositionX - data.TargetX;
            float yDiff = data.PositionZ - data.TargetZ;
            return (float)(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
        }

        public override void RenderScene()
        {
            if (MapCamera == null) return;

            GL.DepthFunc(DepthFunction.Lequal);

            var cam = MapCamera.cameraData;

            GL.PushMatrix();

            foreach (var kcl in ParentEditor.CollisionObjects)
                kcl.Renderer.Draw(Camera.ModelViewMatrix);

            //Draw mini map and bind texture

            var angle = GetAngle();
            var scaleX = cam.PositionX - cam.TargetX;
            var scaleY = cam.PositionZ - cam.TargetZ;

            GL.PushMatrix();
            GL.Rotate(angle + -90, Vector3.UnitZ);
            GL.Enable(EnableCap.Texture2D);
            GL.DepthFunc(DepthFunction.Always);

            if (MapCameraTexture != null)
            {
                if (MapCameraTexture.RenderableTex == null || !MapCameraTexture.RenderableTex.GLInitialized)
                    MapCameraTexture.LoadOpenGLTexture();
                MapCameraTexture.RenderableTex?.Bind();
            }

            Render2D.DrawRectangle(cam.BoundingWidth, cam.BoundingHeight,
                Color.FromArgb(CameraMapTransparency, Color.White), false);

            GL.PopMatrix();
            GL.PopMatrix();

            GL.DepthFunc(DepthFunction.Lequal);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);

            Render2D.DrawRectangle(cam.BoundingWidth, cam.BoundingHeight, Color.Red, true);

            GL.Enable(EnableCap.DepthTest);

            foreach (var obj in CameraPoints)
                obj.Draw();
        }
    }
}

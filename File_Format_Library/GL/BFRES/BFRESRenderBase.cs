using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.Rendering;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using Bfres.Structs;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace FirstPlugin
{
    public class BFRESRenderBase : AbstractGlDrawable, IMeshContainer
    {
        public Matrix4 ModelTransform = Matrix4.Identity;

        // gl buffer objects
        internal int vbo_position;
        internal int ibo_elements;

        public BFRES ResFileNode;

        public List<STGenericObject> Meshes
        {
            get
            {
                List<STGenericObject> meshes = new List<STGenericObject>();
                for (int m = 0; m < models.Count; m++)
                {
                    for (int s = 0; s < models[m].shapes.Count; s++)
                        meshes.Add(models[m].shapes[s]);
                }
                return meshes;
            }
        }

        private List<FMDL> _models = new List<FMDL>();
        public List<FMDL> models
        {
            get
            {
                return _models;
            }
        }

        internal bool Disposing = false;

        public void UpdateModelList()
        {
            _models.Clear();
            foreach (var node in ResFileNode.Nodes)
            {
                if (node is BFRESGroupNode &&
                    ((BFRESGroupNode)node).Type == BRESGroupType.Models)
                {
                    foreach (FMDL mdl in ((BFRESGroupNode)node).Nodes)
                        _models.Add(mdl);
                }
            }
        }

        private void TransformBones()
        {
            for (int mdl = 0; mdl < models.Count; mdl++)
            {
                for (int b = 0; b < models[mdl].Skeleton.bones.Count; b++)
                {
                    models[mdl].Skeleton.bones[b].ModelMatrix = ModelTransform;
                }
            }
        }

        internal void GenerateBuffers()
        {
            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out ibo_elements);

            TransformBones();

            UpdateVertexData();
            UpdateTextureMaps();
        }

        public void Destroy()
        {
            bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;

            if (!buffersWereInitialized)
                return;

            GL.DeleteBuffer(vbo_position);
            GL.DeleteBuffer(ibo_elements);

            Disposing = true;
        }

        public virtual void UpdateVertexData()
        {

        }

        public virtual void UpdateTextureMaps()
        {
            if (!Runtime.OpenTKInitialized)
                return;

            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                if (!bntx.AllGLInitialized)
                {
                    foreach (var tex in bntx.Textures)
                    {
                        if (tex.Value.RenderableTex != null && !tex.Value.RenderableTex.GLInitialized)
                            tex.Value.LoadOpenGLTexture();
                    }
                }
            }
            foreach (BFRESGroupNode ftexCont in PluginRuntime.ftexContainers)
            {
                foreach (var tex in ftexCont.ResourceNodes)
                {
                    if (!((FTEX)tex.Value).RenderableTex.GLInitialized)
                        ((FTEX)tex.Value).LoadOpenGLTexture();
                }
            }
        }


        public void UpdateSingleMaterialTextureMaps(FMAT mat)
        {
            if (!Runtime.OpenTKInitialized)
                return;

            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                foreach (var t in mat.TextureMaps)
                {
                    if (bntx.Textures.ContainsKey(t.Name))
                    {
                        if (!bntx.Textures[t.Name].RenderableTex.GLInitialized)
                            bntx.Textures[t.Name].LoadOpenGLTexture();
                    }
                }
            }

            LibraryGUI.UpdateViewport();
        }

        public override void Prepare(GL_ControlModern control)
        {
        }

        public override void Prepare(GL_ControlLegacy control)
        {
        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            if (Disposing || pass == Pass.TRANSPARENT) return;

            bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
            if (!buffersWereInitialized)
                GenerateBuffers();

            if (!Runtime.OpenTKInitialized)
                return;

            Matrix4 mvpMat = control.ModelMatrix * control.CameraMatrix * control.ProjectionMatrix;

            Matrix4 invertedCamera = Matrix4.Identity;
            if (invertedCamera.Determinant != 0)
                invertedCamera = mvpMat.Inverted();

            Vector3 lightDirection = new Vector3(0f, 0f, -1f);
            Vector3 difLightDirection = Vector3.TransformNormal(lightDirection, invertedCamera).Normalized();

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);

            foreach (var model in models)
            {
                foreach (var shape in model.shapes)
                {
                    if (Runtime.RenderModels && model.Checked && shape.Checked)
                    {
                        var mat = shape.GetMaterial();

                        List<int> faces = shape.lodMeshes[shape.DisplayLODIndex].getDisplayFace();

                        GL.Begin(PrimitiveType.Triangles);
                        foreach (var index in faces)
                        {
                            Vertex vert = shape.vertices[index];
                            float normal = Vector3.Dot(difLightDirection, vert.nrm) * 0.5f + 0.5f;
                            GL.Color3(new Vector3(normal));
                            GL.TexCoord2(vert.uv0);
                            GL.Vertex3(vert.pos);
                        }
                        GL.End();
                    }
                }
            }

            GL.Enable(EnableCap.Texture2D);
        }

        public void CenterCamera(GL_ControlBase control)
        {
            if (!Runtime.FrameCamera)
                return;

            var spheres = new List<Vector4>();
            for (int mdl = 0; mdl < models.Count; mdl++)
            {
                for (int shp = 0; shp < models[mdl].shapes.Count; shp++)
                {
                    var vertexPositions = models[mdl].shapes[shp].vertices.Select(x => x.pos).Distinct();
                    spheres.Add(control.GenerateBoundingSphere(vertexPositions));
                }
            }

            control.FrameSelect(spheres);
        }

        public static Vector4 GenerateBoundingSphere(IEnumerable<Vector4> boundingSpheres)
        {
            // The initial max/min should be the first point.
            Vector3 min = boundingSpheres.FirstOrDefault().Xyz - new Vector3(boundingSpheres.FirstOrDefault().W);
            Vector3 max = boundingSpheres.FirstOrDefault().Xyz + new Vector3(boundingSpheres.FirstOrDefault().W);

            // Calculate the end points using the center and radius
            foreach (var sphere in boundingSpheres)
            {
                min = Vector3.ComponentMin(min, sphere.Xyz - new Vector3(sphere.W));
                max = Vector3.ComponentMax(max, sphere.Xyz + new Vector3(sphere.W));
            }

            return GetBoundingSphereFromSpheres(min, max);
        }
        private static Vector4 GetBoundingSphereFromSpheres(Vector3 min, Vector3 max)
        {
            Vector3 lengths = max - min;
            float maxLength = Math.Max(lengths.X, Math.Max(lengths.Y, lengths.Z));
            Vector3 center = (max + min) / 2.0f;
            float radius = maxLength / 2.0f;
            return new Vector4(center, radius);
        }

        public override void Draw(GL_ControlModern control, Pass pass) {
        }
    }
}

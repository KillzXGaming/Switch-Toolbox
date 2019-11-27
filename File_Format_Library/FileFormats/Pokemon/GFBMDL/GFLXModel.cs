using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlatBuffers.Gfbmdl;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;
using FirstPlugin.Forms;

namespace FirstPlugin
{
    public class GFLXModel
    {
        public GFBMDL ParentFile;

        public List<GFLXMaterialData> GenericMaterials = new List<GFLXMaterialData>();
        public List<GFLXMesh> GenericMeshes = new List<GFLXMesh>();

        public STSkeleton Skeleton = new STSkeleton();

        public Model Model;

        public List<string> Textures = new List<string>();
        public List<string> Shaders = new List<string>();

        public T LoadEditor<T>() where T :
            System.Windows.Forms.UserControl, new()
        {
            return ParentFile.LoadEditor<T>();
        }

        public void UpdateVertexData(bool updateViewport) {
            ParentFile.Renderer.UpdateVertexData();

            if (updateViewport)
                LibraryGUI.UpdateViewport();
        }

        public void LoadFile(Model model, GFBMDL file, GFBMDL_Render Renderer) {
            Model = model;
            ParentFile = file;

            for (int t = 0; t < Model.TextureNamesLength; t++)
                Textures.Add(Model.TextureNames(t));

            for (int s = 0; s < Model.ShaderNamesLength; s++)
                Shaders.Add(Model.ShaderNames(s));

            for (int m = 0; m < Model.MaterialsLength; m++) {
                var mat = Model.Materials(m).Value;
                GenericMaterials.Add(new GFLXMaterialData(this, mat));
            }

            List<int> SkinningIndices = new List<int>();

            for (int b = 0; b < Model.BonesLength; b++) {
                var bone = Model.Bones(b).Value;
                Skeleton.bones.Add(new GFLXBone(this, bone));

                if (bone.SkinCheck == null)
                    SkinningIndices.Add(b);
            }

            Skeleton.reset();
            Skeleton.update();

            for (int g = 0; g < Model.GroupsLength; g++) {
                var group = Model.Groups(g).Value;
                var mesh = Model.Meshes(g).Value;

                OpenTK.Matrix4 transform = OpenTK.Matrix4.Identity;

                GFLXMesh genericMesh = new GFLXMesh(this, group, mesh);
                genericMesh.Checked = true;
                genericMesh.ImageKey = "model";
                genericMesh.SelectedImageKey = "model";

                int boneIndex = (int)group.BoneID;
                if (boneIndex < Skeleton.bones.Count && boneIndex > 0)
                {
                    genericMesh.BoneIndex = boneIndex;
                    transform = Skeleton.bones[boneIndex].Transform;

                    genericMesh.Text = Skeleton.bones[boneIndex].Text;
                }

             //   if (group.MeshID < Skeleton.bones.Count && group.MeshID > 0)
             //       genericMesh.Text = Skeleton.bones[(int)group.MeshID].Text;

                Renderer.Meshes.Add(genericMesh);
                GenericMeshes.Add(genericMesh);

                //Load the vertex data
                genericMesh.vertices = GFLXMeshBufferHelper.LoadVertexData(mesh, transform, SkinningIndices);
                genericMesh.FlipUvsVertical();

                //Load faces
                for (int p = 0; p < mesh.PolygonsLength; p++)
                {
                    var poly = mesh.Polygons(p).Value;

                    var polygonGroup = new STGenericPolygonGroup();
                    polygonGroup.MaterialIndex = (int)poly.MaterialID;
                    genericMesh.PolygonGroups.Add(polygonGroup);

                    for (int f = 0; f < poly.DataLength; f++)
                        polygonGroup.faces.Add((int)poly.Data(f));
                }
            }
        }


        public void SaveFile(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream)) {
                writer.Write(Model.ByteBuffer.ToFullArray());
            }
        }
    }

    public class GFLXMaterialData : STGenericMaterial
    {
        private Material Material;
        private GFLXModel ParentModel;

        public Dictionary<string, GFLXSwitchParam> SwitchParams = new Dictionary<string, GFLXSwitchParam>();
        public Dictionary<string, GFLXValueParam> ValueParams = new Dictionary<string, GFLXValueParam>();
        public Dictionary<string, GFLXColorParam> ColorParams = new Dictionary<string, GFLXColorParam>();

        public override void OnClick(TreeView treeView) {
            var editor = ParentModel.LoadEditor<GFLXMaterialEditor>();
            editor.LoadMaterial(this);
        }

        public GFLXMaterialData(GFLXModel parent, Material mat)
        {
            ParentModel = parent;
            Material = mat;

            Text = mat.Name;

            for (int i = 0; i < Material.SwitchesLength; i++) {
                var val = Material.Switches(i).Value;
                SwitchParams.Add(val.Name, new GFLXSwitchParam(val));
            }

            for (int i = 0; i < Material.ValuesLength; i++) {
                var val = Material.Values(i).Value;
                ValueParams.Add(val.Name, new GFLXValueParam(val));
            }

            for (int i = 0; i < Material.ColorsLength; i++) {
                var val = Material.Colors(i).Value;
                ColorParams.Add(val.Name, new GFLXColorParam(val));
            }

            int textureUnit = 1;
            for (int t = 0; t < Material.TexturesLength; t++)
            {
                var tex = Material.Textures(t).Value;
                string name = ParentModel.Textures[tex.Index];

                STGenericMatTexture matTexture = new STGenericMatTexture();
                matTexture.Name = name;
                matTexture.SamplerName = tex.Name;
                matTexture.textureUnit = textureUnit++;
                matTexture.WrapModeS = STTextureWrapMode.Mirror;
                matTexture.WrapModeT = STTextureWrapMode.Repeat;
                TextureMaps.Add(matTexture);

                switch (tex.Name)
                {
                    case "Col0Tex":
                        matTexture.Type = STGenericMatTexture.TextureType.Diffuse;
                        break;
                    case "EmissionMaskTex":
                        //     matTexture.Type = STGenericMatTexture.TextureType.Emission;
                        break;
                    case "LyCol0Tex":
                        break;
                    case "NormalMapTex":
                        matTexture.WrapModeT = STTextureWrapMode.Repeat;
                        matTexture.WrapModeT = STTextureWrapMode.Repeat;
                        matTexture.Type = STGenericMatTexture.TextureType.Normal;
                        break;
                    case "AmbientTex":
                        break;
                    case "LightTblTex":
                        break;
                    case "SphereMapTex":
                        break;
                    case "EffectTex":
                        break;
                }
            }
        }
    }

    public class GFLXBone : STBone
    {
        private Bone Bone;
        private GFLXModel ParentModel;

        public GFLXBone(GFLXModel parent, Bone bone) : base(parent.Skeleton)
        {
            ParentModel = parent;
            Bone = bone;

            Text = bone.Name;
            parentIndex = bone.Parent;

            if (bone.Translation != null)
            {
                position = new float[3]
                { bone.Translation.Value.X,
                  bone.Translation.Value.Y,
                  bone.Translation.Value.Z
                };
            }

            if (bone.Rotation != null)
            {
                rotation = new float[4]
                { bone.Rotation.Value.X,
                  bone.Rotation.Value.Y,
                  bone.Rotation.Value.Z,
                  1.0f,
                };
            }

            if (bone.Scale != null)
            {
                scale = new float[3]
                { bone.Scale.Value.X,
                  bone.Scale.Value.Y,
                  bone.Scale.Value.Z
                 };
            }
        }
    }

    public class GFLXColorParam
    {
        private MatColor Param;

        public GFLXColorParam(MatColor param)
        {
            Param = param;
        }

        public string Name
        {
            get { return Param.Name; }
            set
            {
                Param.ByteBuffer.PutStringUTF8(0, value);
            }
        }

        public OpenTK.Vector3 Value
        {
            get { return new OpenTK.Vector3(
                Param.Color.Value.R,
                Param.Color.Value.G,
                Param.Color.Value.B);
            }
            set
            {
                var vec3 = value;

                Param.Color.Value.ByteBuffer.PutFloat(0, vec3.X);
                Param.Color.Value.ByteBuffer.PutFloat(1, vec3.Y);
                Param.Color.Value.ByteBuffer.PutFloat(2, vec3.Z);
            }
        }
    }

    public class GFLXValueParam
    {
        private MatFloat Param;

        public GFLXValueParam(MatFloat param) {
            Param = param;
        }

        public string Name
        {
            get { return Param.Name; }
            set {
                Param.ByteBuffer.PutStringUTF8(0, value);
            }
        }

        public float Value
        {
            get { return Param.Value; }
            set {
                Param.ByteBuffer.PutFloat(1, value);
            }
        }
    }

    public class GFLXSwitchParam
    {
        private MatSwitch Param;

        public GFLXSwitchParam(MatSwitch param)
        {
            Param = param;
        }

        public string Name
        {
            get { return Param.Name; }
            set
            {
                Param.ByteBuffer.PutStringUTF8(0, value);
            }
        }

        public bool Value
        {
            get { return Param.Value; }
            set
            {
                Param.ByteBuffer.PutByte(1, value ? (byte)1 : (byte)0);
            }
        }
    }
}

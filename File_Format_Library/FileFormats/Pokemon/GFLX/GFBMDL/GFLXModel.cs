using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;
using FirstPlugin.Forms;
using FirstPlugin.GFMDLStructs;
using Newtonsoft.Json;

namespace FirstPlugin
{
    public class GFLXModel
    {
        public GFBMDL ParentFile;

        public List<GFLXMaterialData> GenericMaterials = new List<GFLXMaterialData>();
        public List<GFLXMesh> GenericMeshes = new List<GFLXMesh>();

        public STSkeleton Skeleton = new STSkeleton();

        public Model Model { get; set; }

        public IList<string> Textures
        {
            get { return Model.TextureNames; }
            set { Model.TextureNames = value; }
        }

        public IList<string> Shaders
        {
            get { return Model.ShaderNames; }
            set { Model.ShaderNames = value; }
        }

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

            Renderer.Meshes.Clear();

            for (int m = 0; m < Model.Materials?.Count; m++) {
                GenericMaterials.Add(new GFLXMaterialData(this, Model.Materials[m]));
            }

            List<int> SkinningIndices = new List<int>();

            for (int b = 0; b < Model.Bones?.Count; b++) {
                var bone = Model.Bones[b];
                Skeleton.bones.Add(new GFLXBone(this, bone));

                if (bone.RigidCheck == null)
                    SkinningIndices.Add(b);
            }

            Skeleton.reset();
            Skeleton.update();

            for (int g = 0; g < Model.Groups?.Count; g++) {
                var group = Model.Groups[g];
                var mesh = Model.Meshes[g];

                OpenTK.Matrix4 transform = OpenTK.Matrix4.Identity;

                GFLXMesh genericMesh = new GFLXMesh(this, group, mesh);
                genericMesh.Checked = true;
                genericMesh.ImageKey = "model";
                genericMesh.SelectedImageKey = "model";

                int boneIndex = (int)group.BoneIndex;
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
                genericMesh.Transform = transform;
                genericMesh.vertices = GFLXMeshBufferHelper.LoadVertexData(mesh, transform, SkinningIndices);
                genericMesh.FlipUvsVertical();

                //Load faces
                for (int p = 0; p < mesh.Polygons?.Count; p++)
                {
                    var poly = mesh.Polygons[p];

                    var polygonGroup = new STGenericPolygonGroup();
                    polygonGroup.MaterialIndex = (int)poly.MaterialIndex;
                    genericMesh.PolygonGroups.Add(polygonGroup);

                    if (GenericMaterials.Count > poly.MaterialIndex)
                        polygonGroup.Material = GenericMaterials[(int)poly.MaterialIndex];

                    for (int f = 0; f < poly.Faces?.Count; f++)
                        polygonGroup.faces.Add((int)poly.Faces[f]);
                }
            }
        }


        public void SaveFile(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream)) {
                writer.Write(FlatBufferConverter.SerializeFrom<Model>(Model, "gfbmdl"));
            }
        }

        public List<int> GenerateSkinningIndices()
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < Model.Bones?.Count; i++)
            {
                if (Model.Bones[i].RigidCheck == null)
                    indices.Add(Model.Bones.IndexOf(Model.Bones[i]));
            }
            return indices;
        }

        public GFMDLStructs.BoundingBox GenerateBoundingBox(STGenericObject mesh)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            for (int v = 0; v < mesh.vertices.Count; v++)
            {
                minX = Math.Min(minX, mesh.vertices[v].pos.X);
                minY = Math.Min(minY, mesh.vertices[v].pos.Y);
                minZ = Math.Min(minZ, mesh.vertices[v].pos.Z);
                maxX = Math.Max(maxX, mesh.vertices[v].pos.X);
                maxY = Math.Max(maxY, mesh.vertices[v].pos.Y);
                maxZ = Math.Max(maxZ, mesh.vertices[v].pos.Z);
            }

            var bounding = new GFMDLStructs.BoundingBox();
            bounding.MinX = minX;
            bounding.MinY = minY;
            bounding.MinZ = minZ;
            bounding.MaxX = maxX;
            bounding.MaxY = maxY;
            bounding.MaxZ = maxZ;
            return bounding;
        }

        public void GenerateBoundingBox()
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            foreach (var mesh in GenericMeshes)
            {
                for (int v = 0; v < mesh.vertices.Count; v++)
                {
                    minX = Math.Min(minX, mesh.vertices[v].pos.X);
                    minY = Math.Min(minY, mesh.vertices[v].pos.Y);
                    minZ = Math.Min(minZ, mesh.vertices[v].pos.Z);
                    maxX = Math.Max(maxX, mesh.vertices[v].pos.X);
                    maxY = Math.Max(maxY, mesh.vertices[v].pos.Y);
                    maxZ = Math.Max(maxZ, mesh.vertices[v].pos.Z);
                }
            }

            Model.Bounding = new GFMDLStructs.BoundingBox();
            Model.Bounding.MinX = minX;
            Model.Bounding.MinY = minY;
            Model.Bounding.MinZ = minZ;
            Model.Bounding.MaxX = maxX;
            Model.Bounding.MaxY = maxY;
            Model.Bounding.MaxZ = maxZ;
        }
    }

    public class GFLXMaterialAnimController
    {
        public Dictionary<string, bool> SwitchParams = new Dictionary<string, bool>();
        public Dictionary<string, float> ValueParams = new Dictionary<string, float>();
        public Dictionary<string, OpenTK.Vector3> ColorParams = new Dictionary<string, OpenTK.Vector3>();

        public bool IsVisible { get; set; } = true;

        public void ResetAnims()
        {
            ColorParams.Clear();
            SwitchParams.Clear();
            ValueParams.Clear();
            IsVisible = true;
        }
    }

    public class GFLXMaterialData : STGenericMaterial, IContextMenuNode
    {
        private Material Material;
        public GFLXModel ParentModel;

        public GFLXMaterialAnimController AnimController = new GFLXMaterialAnimController();

        public Dictionary<string, GFLXSwitchParam> SwitchParams = new Dictionary<string, GFLXSwitchParam>();
        public Dictionary<string, GFLXValueParam> ValueParams = new Dictionary<string, GFLXValueParam>();
        public Dictionary<string, GFLXColorParam> ColorParams = new Dictionary<string, GFLXColorParam>();

        public override void OnClick(TreeView treeView) {
            var editor = ParentModel.LoadEditor<GFLXMaterialEditor>();
            editor.LoadMaterial(this);
        }

        public string ConvertToJson() {
            return JsonConvert.SerializeObject(Material, Formatting.Indented);
        }

        public void ConvertFromJson(string text)
        {
            int index = ParentModel.Model.Materials.IndexOf(Material);
            Material = JsonConvert.DeserializeObject<Material>(text);
            ParentModel.Model.Materials[index] = Material;

            ReloadMaterial();

            var editor = ParentModel.LoadEditor<GFLXMaterialEditor>();
            editor.LoadMaterial(this);

        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Export", null, ExportAction, Keys.Control | Keys.E));
            Items.Add(new ToolStripMenuItem("Replace", null, ReplaceAction, Keys.Control | Keys.R));
            return Items.ToArray();
        }

        private void ExportAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.json;";
            sfd.FileName = Text;
            sfd.DefaultExt = "json";
            if (sfd.ShowDialog() == DialogResult.OK) {
                string json = JsonConvert.SerializeObject(Material, Formatting.Indented);
                System.IO.File.WriteAllText(sfd.FileName, json);
            }
        }

        private void ReplaceAction(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.json;";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                int index = ParentModel.Model.Materials.IndexOf(Material);

                Material = JsonConvert.DeserializeObject<Material>(
                    System.IO.File.ReadAllText(ofd.FileName));

                ParentModel.Model.Materials[index] = Material;

                ReloadMaterial();

                var editor = ParentModel.LoadEditor<GFLXMaterialEditor>();
                editor.LoadMaterial(this);
            }
        }

        public bool HasVertexColors()
        {
            //A quick hack. Maps typically have no switches but have vertex colors as diffuse.
            return Material.Switches.Count == 0;
        }

        public GFLXMaterialData(GFLXModel parent, Material mat)
        {
            ParentModel = parent;
            Material = mat;
            Text = mat.Name;

            ReloadMaterial();
        }

        private void ReloadMaterial()
        {
            SwitchParams.Clear();
            ValueParams.Clear();
            ColorParams.Clear();
            TextureMaps.Clear();

            for (int i = 0; i < Material.Switches?.Count; i++)
            {
                var val = Material.Switches[i];
                SwitchParams.Add(val.Name, new GFLXSwitchParam(val));
            }

            for (int i = 0; i < Material.Values?.Count; i++)
            {
                var val = Material.Values[i];
                ValueParams.Add(val.Name, new GFLXValueParam(val));
            }

            for (int i = 0; i < Material.Colors?.Count; i++)
            {
                var val = Material.Colors[i];
                ColorParams.Add(val.Name, new GFLXColorParam(val));
            }

            int textureUnit = 1;
            for (int t = 0; t < Material.TextureMaps?.Count; t++)
            {
                var tex = Material.TextureMaps[t];
                string name = ParentModel.Textures[(int)tex.Index];

                GFLXTextureMap matTexture = new GFLXTextureMap();
                matTexture.gflxTextureMap = tex;
                matTexture.Name = name;
                matTexture.Transform = new STTextureTransform();
                matTexture.SamplerName = tex.Sampler;
                matTexture.textureUnit = textureUnit++;
                matTexture.WrapModeS = STTextureWrapMode.Mirror;
                matTexture.WrapModeT = STTextureWrapMode.Repeat;
                TextureMaps.Add(matTexture);

                if (tex.Params != null)
                {
                    matTexture.WrapModeS = GFLXTextureMap.ConvertWrap(tex.Params.WrapModeX);
                    matTexture.WrapModeT = GFLXTextureMap.ConvertWrap(tex.Params.WrapModeY);
                }

                switch (tex.Sampler)
                {
                    case "Texture0112`":
                    case "BaseColor0":
                    case "Col0Tex":
                    case "L0ColTex":
                        matTexture.Type = STGenericMatTexture.TextureType.Diffuse;
                        break;
                    case "EmissionMaskTex":
                        //     matTexture.Type = STGenericMatTexture.TextureType.Emission;
                        break;
                    case "LyCol0Tex":
                        break;
                    case "NormalMapTex":
                        matTexture.Type = STGenericMatTexture.TextureType.Normal;
                        break;
                    case "AmbientTex":
                        if (SwitchParams.ContainsKey("AmbientMapEnable")) {
                            if (SwitchParams["AmbientMapEnable"].Value)
                                matTexture.Type = STGenericMatTexture.TextureType.AO;
                        }
                        break;
                    case "LightTblTex":
                        break;
                    case "SphereMapTex":
                        break;
                    case "EffectTex":
                        break;
                }


                float ScaleX = 1;
                float ScaleY = 1;
                float TransX = 0;
                float TransY = 0;

                //Lookup the uniforms and apply a transform to the texture map
                //Used for the UV viewer
                if (matTexture.Type == STGenericMatTexture.TextureType.Diffuse)
                {
                    if (ValueParams.ContainsKey("ColorUVScaleU"))
                        ScaleX = ValueParams["ColorUVScaleU"].Value;
                    if (ValueParams.ContainsKey("ColorUVScaleV"))
                        ScaleY = ValueParams["ColorUVScaleV"].Value;

                    if (ValueParams.ContainsKey("ColorUVTranslateU"))
                        TransX += ValueParams["ColorUVTranslateU"].Value;
                    if (ValueParams.ContainsKey("ColorUVTranslateV"))
                        TransY += ValueParams["ColorUVTranslateV"].Value;

                    if (ValueParams.ContainsKey("ColorBaseU"))
                        TransX += ValueParams["ColorBaseU"].Value;
                    if (ValueParams.ContainsKey("ColorBaseV"))
                        TransY += ValueParams["ColorBaseV"].Value;
                }
                if (matTexture.Type == STGenericMatTexture.TextureType.Normal)
                {

                }

                matTexture.Transform.Scale = new OpenTK.Vector2(ScaleX, ScaleY);
                matTexture.Transform.Translate = new OpenTK.Vector2(TransX, TransY);
            }
        }
    }

    public class GFLXTextureMap : STGenericMatTexture
    {
        public TextureMap gflxTextureMap;

        public override STGenericTexture GetTexture()
        {
            foreach (var bntx in PluginRuntime.bntxContainers)
            {
                if (bntx.Textures.ContainsKey(Name))
                    return bntx.Textures[Name];
            }

            return base.GetTexture();
        }

        public static STTextureWrapMode ConvertWrap(uint value)
        {
            switch (value)
            {
                case 0: return STTextureWrapMode.Repeat;
                case 1: return STTextureWrapMode.Clamp;
                case 2: return STTextureWrapMode.Mirror;
                default: return STTextureWrapMode.Repeat;
            }
        }
    }

    public class GFLXBone : STBone
    {
        public Bone Bone;
        public GFLXModel ParentModel;

        public bool HasSkinning => Bone.RigidCheck == null;

        public GFLXBone(GFLXModel parent, Bone bone) : base(parent.Skeleton)
        {
            ParentModel = parent;
            Bone = bone;

            Text = bone.Name;
            parentIndex = bone.Parent;
            UseSegmentScaleCompensate = bone.SegmentScale;

            if (bone.Translation != null)
            {
                Position = new OpenTK.Vector3(
                  bone.Translation.X,
                  bone.Translation.Y,
                  bone.Translation.Z
                );
            }

            if (bone.Rotation != null)
            {
                EulerRotation = new OpenTK.Vector3(
                  bone.Rotation.X,
                  bone.Rotation.Y,
                  bone.Rotation.Z
                );
            }

            if (bone.Scale != null)
            {
                Scale = new OpenTK.Vector3(
                  bone.Scale.X,
                  bone.Scale.Y,
                  bone.Scale.Z
                 );
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
            set { Param.Name = value; }
        }

        public OpenTK.Vector3 Value
        {
            get { return new OpenTK.Vector3(
                Param.Color.R,
                Param.Color.G,
                Param.Color.B);
            }
            set
            {
                var vec3 = value;

                Param.Color.R = vec3.X;
                Param.Color.G = vec3.Y;
                Param.Color.B = vec3.Z;
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
            set { Param.Name = value; }
        }

        public float Value
        {
            get { return Param.Value; }
            set { Param.Value = value; }
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
            set { Param.Name = value; }
        }

        public bool Value
        {
            get { return Param.Value; }
            set { Param.Value = value; }
        }
    }
}

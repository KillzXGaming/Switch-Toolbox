using System;
using System.Collections.Generic;
using System.IO;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Rendering;
using Switch_Toolbox.Library.Forms;
using ResU = Syroot.NintenTools.Bfres;
using ResUGX2 = Syroot.NintenTools.Bfres.GX2;
using ResGFX = Syroot.NintenTools.NSW.Bfres.GFX;
using FirstPlugin;
using OpenTK;
using FirstPlugin.Forms;

namespace Bfres.Structs
{
    public class FMATFolder : TreeNodeCustom
    {
        public FMATFolder()
        {
            Text = "Materials";
            Name = "FmatFolder";

            ContextMenuStrip = new STContextMenuStrip();

            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Import Material", null, ImportAction, Keys.Control | Keys.I));
            ContextMenuStrip.Items.Add(new ToolStripSeparator());
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export All Materials", null, ExportAllAction, Keys.Control | Keys.A));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Replace (From Folder)",null, ReplaceBatchAction, Keys.Control | Keys.R));
        }

        public void ExportAllAction(object sender, EventArgs args) { ExportAll(); }
        public void ReplaceBatchAction(object sender, EventArgs args) { ReplaceBatch(); }
        public void ImportAction(object sender, EventArgs args) { Import(); }

        public void ExportAll()
        {
            bool IncludeTextureMaps = false;

            FolderSelectDialog sfd = new FolderSelectDialog();

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = sfd.SelectedPath;

                foreach (FMAT mat in Nodes)
                {
                   mat.Export(folderPath + '\\' + mat.Text + ".bfmat", IncludeTextureMaps);
                }
            }
        }
        public void ReplaceBatch()
        {
            FolderSelectDialog ofd = new FolderSelectDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = ofd.SelectedPath;

                foreach (var file in Directory.GetFiles(folderPath))
                {
                    string Name = Path.GetFileNameWithoutExtension(file);
                    if (((FMDL)Parent).materials.ContainsKey(Name) &&
                        (file.EndsWith(".bfmat")))
                    {
                        ((FMDL)Parent).materials[Name].Replace(file, false);
                    }
                }
            }
            LibraryGUI.Instance.UpdateViewport();
        }

        public void Import()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Bfres Material |*.bfmat;";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                    ((FMDL)Parent).AddMaterials(file, false);
            }
        }
        public override void OnClick(TreeView treeView)
        {

        }
    }
    public class FMAT : STGenericMaterial
    {
        public FMAT()
        {
            Checked = true;
            ImageKey = "material";
            SelectedImageKey = "material";

            ContextMenuStrip = new STContextMenuStrip();

            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export", null, ExportAction, Keys.Control | Keys.E));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Replace", null, ReplaceAction, Keys.Control | Keys.R));

            ContextMenuStrip.Items.Add(new ToolStripSeparator());
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Copy", null, CopyAction, Keys.Control | Keys.C));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.Control | Keys.N));
            ContextMenuStrip.Items.Add(new ToolStripSeparator());
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Delete", null, DeleteAction, Keys.Control | Keys.N));
        }

        protected void ExportAction(object sender, EventArgs args) { Export(); }
        protected void ReplaceAction(object sender, EventArgs args) { Replace(); }
        protected void CopyAction(object sender, EventArgs args) { Copy(); }
        protected void RenameAction(object sender, EventArgs args) { Rename(); }
        protected void DeleteAction(object sender, EventArgs args) { Delete(); }

        public int GetOptionValue(string Option)
        {
            int value = -1;
            if (shaderassign.options.ContainsKey(Option))
                int.TryParse(shaderassign.options[Option], out value);
            
            return value;
        }

        public void Delete()
        {
            string MappedNames = "";
            var model = GetParentModel();

            int CurrentIndex = Parent.Nodes.IndexOf(this);

            if (model.materials.Count == 1 && model.shapes.Count > 0)
            {
                MessageBox.Show("A single material must exist if any objects exist!", "Material Delete",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var shape in model.shapes)
            {
                if (shape.GetMaterial() == this)
                    MappedNames += $"{shape.Text}\n";
            }
            if (MappedNames != "")
            {
                var result = STOptionsDialog.Show("Shapes are mapped to this material. Are you sure you want to remove this? (Will default to first material)",
                    "Material Delete", MappedNames);

                if (result == DialogResult.Yes)
                    RemoveMaterial(model, CurrentIndex);
            }
            else {
                RemoveMaterial(model, CurrentIndex);
            }
        }

        private void RemoveMaterial(FMDL model, int CurrentIndex)
        {
            //Adjust all the indices properly based on this current index
            foreach (var shape in model.shapes)
            {
                //If there are indices higher than this index, shift them
                if (shape.MaterialIndex > CurrentIndex)
                {
                    shape.MaterialIndex -= 1;
                }
            }

            model.materials.Remove(Text);
            Parent.Nodes.Remove(this);
        }

        public FMDL GetParentModel()
        {
            return ((FMDL)Parent.Parent);
        }

        public ResFile GetResFile()
        {
            return ((FMDL)Parent.Parent).GetResFile();
        }
        public ResU.ResFile GetResFileU()
        {
            return ((FMDL)Parent.Parent).GetResFileU();
        }

        public bool Enabled { get; set; } = true;
        public bool isTransparent = false;

        public override void OnClick(TreeView treeView)
        {
            UpdateEditor();
        }
        public void UpdateEditor()
        {
            ((BFRES)Parent.Parent.Parent.Parent).LoadEditors(this);
        }

        public void UpdateTextureMaps()
        {
           ((BFRES)Parent.Parent.Parent.Parent).BFRESRender.UpdateSingleMaterialTextureMaps(this);
        }

        public BFRESRender GetRenderer()
        {
           return ((BFRES)Parent.Parent.Parent.Parent).BFRESRender;
        }

        public bool IsNormalMapTexCoord2()
        {
            //for BOTW if it uses UV layer 2 for normal maps use second UV map
            if (shaderassign.options.ContainsKey("uking_texture2_texcoord"))
            {
                float value = float.Parse(shaderassign.options["uking_texture2_texcoord"]);
                return (value == 1);
            }

            //For 3D world
            if (shaderassign.options.ContainsKey("cIsEnableNormalMap"))
            {
                float value = float.Parse(shaderassign.options["cIsEnableNormalMap"]);
                return (value == 1);
            }

            return false;
        }

        public void SetActiveGame()
        {
            Runtime.activeGame = Runtime.ActiveGame.KSA;

            string ShaderName = shaderassign.ShaderArchive;
            string ShaderModel = shaderassign.ShaderModel;

            if (ShaderName == null || ShaderModel == null)
                return;

            if (ShaderName == "alRenderMaterial" || ShaderName == "alRenderCloudLayer" || ShaderName == "alRenderSky")
                Runtime.activeGame = Runtime.ActiveGame.SMO;
            else if (ShaderName == "Turbo_UBER")
                Runtime.activeGame = Runtime.ActiveGame.MK8D;
            else if (ShaderName.Contains("uking_mat"))
                Runtime.activeGame = Runtime.ActiveGame.BOTW;
            else if (ShaderName.Contains("Blitz_UBER"))
                Runtime.activeGame = Runtime.ActiveGame.Splatoon2;
            else
                Runtime.activeGame = Runtime.ActiveGame.KSA;
        }
        private void Rename()
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ((FMDL)Parent.Parent).materials.Remove(Text);
                Text = dialog.textBox1.Text;
                ((FMDL)Parent.Parent).materials.Add(Text, this);
            }
        }
        private void Copy()
        {
            ((FMDL)Parent.Parent).CopyMaterial(this);
        }
        private void Export()
        {
            bool IncludeTextureMaps = true;

            if (MaterialU != null)
                BfresWiiU.SetMaterial(this, MaterialU, GetResFileU());
            else 
                BfresSwitch.SetMaterial(this, Material);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.bfmat;";

            sfd.DefaultExt = ".bfmat";
            sfd.FileName = Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Export(sfd.FileName, IncludeTextureMaps);
            }
        }
        public void Export(string path, bool IncludeTextureMaps)
        {
            if (GetResFileU() != null)
                MaterialU.Export(path, GetResFileU());
            else
                Material.Export(path, GetResFile());

            if (IncludeTextureMaps)
            {

            }
        }
        private void Replace()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfmat;";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Replace(ofd.FileName, false);
            }
        }
        public void Replace(string path, bool UseReplaceDialog)
        {
            if (GetResFileU() != null)
            {
                if (UseReplaceDialog)
                {
                    MaterialReplaceDialog dialog = new MaterialReplaceDialog();
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        var mat = new ResU.Material();
                        mat.Import(path, GetResFileU());

                        if (PluginRuntime.MaterialReplace.SwapRenderInfos)
                            MaterialU.RenderInfos = mat.RenderInfos;
                        if (PluginRuntime.MaterialReplace.SwapShaderOptions)
                            MaterialU.ShaderAssign = mat.ShaderAssign;
                        if (PluginRuntime.MaterialReplace.SwapShaderParams)
                        {
                            MaterialU.ShaderParamData = mat.ShaderParamData;
                            MaterialU.ShaderParams = mat.ShaderParams;
                        }
                        if (PluginRuntime.MaterialReplace.SwapTextures)
                        {
                            MaterialU.ShaderAssign.SamplerAssigns = mat.ShaderAssign.SamplerAssigns;
                            MaterialU.TextureRefs = mat.TextureRefs;
                            MaterialU.Samplers = mat.Samplers;
                        }
                        if (PluginRuntime.MaterialReplace.SwapUserData)
                            MaterialU.UserData = mat.UserData;
                    }
                }
                else
                {
                    MaterialU.Import(path, GetResFileU());
                    MaterialU.Name = Text;
                    BfresWiiU.ReadMaterial(this, MaterialU);
                }
            }
            else
            {
                if (UseReplaceDialog)
                {
                    MaterialReplaceDialog dialog = new MaterialReplaceDialog();
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        var mat = new Material();
                        mat.Import(path);

                        if (PluginRuntime.MaterialReplace.SwapRenderInfos)
                        {
                            Material.RenderInfoDict = mat.RenderInfoDict;
                            Material.RenderInfos = mat.RenderInfos;
                        }
                        if (PluginRuntime.MaterialReplace.SwapShaderOptions)
                            Material.ShaderAssign = mat.ShaderAssign;
                        if (PluginRuntime.MaterialReplace.SwapShaderParams)
                        {
                            Material.ShaderParamData = mat.ShaderParamData;
                            Material.ShaderParams = mat.ShaderParams;
                            Material.ShaderParamDict = mat.ShaderParamDict;
                        }
                        if (PluginRuntime.MaterialReplace.SwapTextures)
                        {
                            Material.ShaderAssign.SamplerAssigns = mat.ShaderAssign.SamplerAssigns;
                            Material.TextureRefs = mat.TextureRefs;
                            Material.Samplers = mat.Samplers;
                            Material.SamplerDict = mat.SamplerDict;
                            Material.SamplerSlotArray = mat.SamplerSlotArray;
                            Material.TextureSlotArray = mat.TextureSlotArray;
                        }
                        if (PluginRuntime.MaterialReplace.SwapUserData)
                        {
                            Material.UserDatas = mat.UserDatas;
                            Material.UserDataDict = mat.UserDataDict;
                        }
                    }
                }
                else
                {
                    Material.Import(path);
                    Material.Name = Text;
                    BfresSwitch.ReadMaterial(this, Material);
                }
            }
        }

        public Dictionary<string, float[]> anims = new Dictionary<string, float[]>();
        public List<BfresRenderInfo> renderinfo = new List<BfresRenderInfo>();
        public Dictionary<string, BfresShaderParam> matparam = new Dictionary<string, BfresShaderParam>();
        public Dictionary<string, BfresShaderParam> animatedMatParams = new Dictionary<string, BfresShaderParam>();

        public Material Material;
        public ResU.Material MaterialU;

        public ShaderAssign shaderassign = new ShaderAssign();

        public class ShaderAssign
        {
            public string ShaderModel = "";
            public string ShaderArchive = "";

            public Dictionary<string, string> options = new Dictionary<string, string>();
            public Dictionary<string, string> samplers = new Dictionary<string, string>();
            public Dictionary<string, string> attributes = new Dictionary<string, string>();
        }
        public bool HasDiffuseMap = false;
        public bool HasNormalMap = false;
        public bool HasSpecularMap = false;
        public bool HasEmissionMap = false;
        public bool HasDiffuseLayer = false;
        public bool HasTeamColorMap = false; //Splatoon uses this (TLC)
        public bool HasTransparencyMap = false;
        public bool HasShadowMap = false;
        public bool HasAmbientOcclusionMap = false;
        public bool HasLightMap = false;
        public bool HasSphereMap = false;
        public bool HasSubSurfaceScatteringMap = false;

        //PBR (Switch) data
        public bool HasMetalnessMap = false;
        public bool HasRoughnessMap = false;
        public bool HasMRA = false;
    }
    public class BfresShaderParam
    {
        public ushort DependedIndex;
        public ushort DependIndex;

        public ShaderParamType Type;
        public string Name { get; set; }

        public bool HasPadding;
        public int PaddingLength = 0;

        public float[] ValueFloat;
        public bool[] ValueBool;
        public uint[] ValueUint;
        public int[] ValueInt;
        public byte[] ValueReserved;

        public Srt2D ValueSrt2D;
        public Srt3D ValueSrt3D;
        public TexSrt ValueTexSrt;
        public TexSrtEx ValueTexSrtEx;

        //If a data set is not defined then defaults in this to save back properly
        //Note this may be rarely needed or not at all
        public byte[] Value_Unk;

        private void ReadSRT2D(FileReader reader)
        {
            ValueSrt2D = new Srt2D();
            ValueSrt2D.Scaling = reader.ReadVec2SY();
            ValueSrt2D.Rotation = reader.ReadSingle();
            ValueSrt2D.Translation = reader.ReadVec2SY();
        }
        private void ReadSRT3D(FileReader reader)
        {
            ValueSrt3D = new Srt3D();
            ValueSrt3D.Scaling = reader.ReadVec3SY();
            ValueSrt3D.Rotation = reader.ReadVec3SY();
            ValueSrt3D.Translation = reader.ReadVec3SY();
        }
        private void ReadTexSrt(FileReader reader)
        {
            ValueTexSrt = new TexSrt();
            ValueTexSrt.Mode = reader.ReadEnum<TexSrtMode>(false);
            ValueTexSrt.Scaling = reader.ReadVec2SY();
            ValueTexSrt.Rotation = reader.ReadSingle();
            ValueTexSrt.Translation = reader.ReadVec2SY();
        }
        private void ReadTexSrtEx(FileReader reader)
        {
            ValueTexSrtEx = new TexSrtEx();
            ValueTexSrtEx.Mode = reader.ReadEnum<TexSrtMode>(true);
            ValueTexSrtEx.Scaling = reader.ReadVec2SY();
            ValueTexSrtEx.Rotation = reader.ReadSingle();
            ValueTexSrtEx.Translation = reader.ReadVec2SY();
            ValueTexSrtEx.MatrixPointer = reader.ReadUInt32();
        }
        public ShaderParamType GetTypeWiiU(ResU.ShaderParamType type)
        {
            return (ShaderParamType)System.Enum.Parse(typeof(ShaderParamType), type.ToString());
        }
        public ResU.ShaderParamType SetTypeWiiU(ShaderParamType type)
        {
            return (ResU.ShaderParamType)System.Enum.Parse(typeof(ResU.ShaderParamType), type.ToString());
        }

        public void ReadValue(FileReader reader, int Size)
        {
            switch (Type)
            {
                case ShaderParamType.Bool:
                case ShaderParamType.Bool2:
                case ShaderParamType.Bool3:
                case ShaderParamType.Bool4:
                    ValueBool = reader.ReadBooleans(Size / sizeof(bool)); break;
                case ShaderParamType.Float:
                case ShaderParamType.Float2:
                case ShaderParamType.Float3:
                case ShaderParamType.Float4:
                case ShaderParamType.Float2x2:
                case ShaderParamType.Float2x3:
                case ShaderParamType.Float2x4:
                case ShaderParamType.Float4x2:
                case ShaderParamType.Float4x3:
                case ShaderParamType.Float4x4:
                    ValueFloat = reader.ReadSingles(Size / sizeof(float)); break;
                case ShaderParamType.Int:
                case ShaderParamType.Int2:
                case ShaderParamType.Int3:
                case ShaderParamType.Int4:
                    ValueInt = reader.ReadInt32s(Size / sizeof(int)); break;
                case ShaderParamType.Reserved2:
                case ShaderParamType.Reserved3:
                case ShaderParamType.Reserved4:
                    ValueReserved = reader.ReadBytes(Size / sizeof(byte)); break;
                case ShaderParamType.Srt2D:
                    ReadSRT2D(reader); break;
                case ShaderParamType.Srt3D:
                    ReadSRT3D(reader); break;
                case ShaderParamType.TexSrt:
                    ReadTexSrt(reader); break;
                case ShaderParamType.TexSrtEx:
                    ReadTexSrtEx(reader); break;
                case ShaderParamType.UInt:
                case ShaderParamType.UInt2:
                case ShaderParamType.UInt3:
                case ShaderParamType.UInt4:
                    ValueUint = reader.ReadUInt32s(Size / sizeof(uint)); break;
                // Invalid
                default:
                    throw new ArgumentException($"Invalid {nameof(ShaderParamType)} {Type}.",
               nameof(Type));
            }
        }
        public void WriteValue(FileWriter writer)
        {
            switch (Type)
            {
                case ShaderParamType.Bool:
                case ShaderParamType.Bool2:
                case ShaderParamType.Bool3:
                case ShaderParamType.Bool4:
                    writer.Write(ValueBool); break;
                case ShaderParamType.Float:
                case ShaderParamType.Float2:
                case ShaderParamType.Float3:
                case ShaderParamType.Float4:
                case ShaderParamType.Float2x2:
                case ShaderParamType.Float2x3:
                case ShaderParamType.Float2x4:
                case ShaderParamType.Float4x2:
                case ShaderParamType.Float4x3:
                case ShaderParamType.Float4x4:
                    writer.Write(ValueFloat); break;
                case ShaderParamType.Int:
                case ShaderParamType.Int2:
                case ShaderParamType.Int3:
                case ShaderParamType.Int4:
                    writer.Write(ValueInt); break;
                case ShaderParamType.Reserved2:
                case ShaderParamType.Reserved3:
                case ShaderParamType.Reserved4:
                    writer.Write(ValueInt); break;
                case ShaderParamType.Srt2D:
                    WriteSRT2D(writer); break;
                case ShaderParamType.Srt3D:
                    WriteSRT3D(writer); break;
                case ShaderParamType.TexSrt:
                    WriteTexSrt(writer); break;
                case ShaderParamType.TexSrtEx:
                    WriteTexSrtEx(writer); break;
                case ShaderParamType.UInt:
                case ShaderParamType.UInt2:
                case ShaderParamType.UInt3:
                case ShaderParamType.UInt4:
                    writer.Write(ValueUint); break;
                // Invalid
                default:
                    throw new ArgumentException($"Invalid {nameof(ShaderParamType)} {Type}.",
               nameof(Type));
            }
            Console.WriteLine("PaddingLength " + PaddingLength);
            if (PaddingLength > 0)
                writer.Write(new byte[PaddingLength]);
        }
        private void WriteSRT2D(FileWriter writer)
        {
            writer.Write(ValueSrt2D.Scaling);
            writer.Write(ValueSrt2D.Rotation);
            writer.Write(ValueSrt2D.Translation);
        }
        private void WriteSRT3D(FileWriter writer)
        {
            writer.Write(ValueSrt3D.Scaling);
            writer.Write(ValueSrt3D.Rotation);
            writer.Write(ValueSrt3D.Translation);
        }
        private void WriteTexSrt(FileWriter writer)
        {
            writer.Write((uint)ValueTexSrt.Mode);
            writer.Write(ValueTexSrt.Scaling);
            writer.Write(ValueTexSrt.Rotation);
            writer.Write(ValueTexSrt.Translation);
        }
        private void WriteTexSrtEx(FileWriter writer)
        {
            writer.Write((uint)ValueTexSrtEx.Mode);
            writer.Write(ValueTexSrtEx.Scaling);
            writer.Write(ValueTexSrtEx.Rotation);
            writer.Write(ValueTexSrtEx.Translation);
            writer.Write(ValueTexSrtEx.MatrixPointer);
        }
    }
    public class BfresRenderInfo
    {
        public string Name;
        public long DataOffset;
        public RenderInfoType Type;
        public int ArrayLength;

        //Data Section by "Type"

        public int[] ValueInt;
        public string[] ValueString;
        public float[] ValueFloat;

        public RenderInfoType GetTypeWiiU(ResU.RenderInfoType type)
        {
            return (RenderInfoType)System.Enum.Parse(typeof(RenderInfoType), type.ToString());
        }
        public ResU.RenderInfoType SetTypeWiiU(RenderInfoType type)
        {
            return (ResU.RenderInfoType)System.Enum.Parse(typeof(ResU.RenderInfoType), type.ToString());
        }

    }

    public class MatTextureWrapper : TreeNodeCustom
    {
        public MatTexture textureMap;

        public MatTextureWrapper(string key, string text, MatTexture texture)
        {
            Name = key;
            Text = text;
            textureMap = texture;

            ImageKey = "TextureMaterialMap";
            SelectedImageKey = "TextureMaterialMap";
        }

        public override void OnClick(TreeView treeView)
        {
            UpdateEditor();
        }
        public void UpdateEditor()
        {
            ((BFRES)Parent.Parent.Parent.Parent.Parent).LoadEditors(this);
        }
    }

    public class MatTexture : STGenericMatTexture
    {
        public string animatedTexName = "";

        //Note samplers will get converted to another sampler type sometimes in the shader assign section
        //Use this string if not empty for our bfres fragment shader to produce the accurate affects
        //An example of a conversion maybe be like a1 - t0 so texture gets used as a transparent map/alpha texture
        public string FragShaderSampler = "";

        public Sampler switchSampler;
        public ResUGX2.TexSampler wiiUSampler;

        public float MinLod;
        public float MaxLod;
        public float BiasLod;

        public MatTexture()
        {

        }
    }
    public class SwitchSamplerInfo
    {
        public ResGFX.TexClamp TexClampX;
        public ResGFX.TexClamp TexClampY;
        public ResGFX.TexClamp TexClampZ;


    }
    public class WiiUSamplerInfo
    {
        public ResUGX2.GX2TexClamp ClampX;
        public ResUGX2.GX2TexClamp ClampY;
        public ResUGX2.GX2TexClamp ClampZ;
        public ResUGX2.GX2TexXYFilterType MagFilter;
        public ResUGX2.GX2TexXYFilterType MinFilter;
        public ResUGX2.GX2TexZFilterType ZFilter;
        public ResUGX2.GX2TexMipFilterType MipFilter;
        public ResUGX2.GX2TexAnisoRatio MaxAnisotropicRatio;
        public ResUGX2.GX2TexBorderType BorderType;
        public ResUGX2.GX2CompareFunction DepthCompareFunc;

        public bool DepthCompareEnabled;

        public void Load(ResUGX2.TexSampler texSampler)
        {
            BorderType = texSampler.BorderType;
            ClampX = texSampler.ClampX;
            ClampY = texSampler.ClampY;
            ClampZ = texSampler.ClampZ;
            DepthCompareEnabled = texSampler.DepthCompareEnabled;
            DepthCompareFunc = texSampler.DepthCompareFunc;
            MinFilter = texSampler.MinFilter;
            MipFilter = texSampler.MipFilter;
            ZFilter = texSampler.ZFilter;
        }
        public ResUGX2.TexSampler Save(MatTexture matTex)
        {
            var texSampler = new ResUGX2.TexSampler();
            texSampler.BorderType = BorderType;
            texSampler.ClampX = ClampX;
            texSampler.ClampY = ClampY;
            texSampler.ClampZ = ClampZ;
            texSampler.DepthCompareEnabled = DepthCompareEnabled;
            texSampler.DepthCompareFunc = DepthCompareFunc;
            texSampler.MinFilter = MinFilter;
            texSampler.MipFilter = MipFilter;
            texSampler.ZFilter = ZFilter;

            texSampler.LodBias = matTex.BiasLod;
            texSampler.MaxLod = matTex.MaxLod;
            texSampler.MinLod = matTex.minFilter;

            return texSampler;
        }
    }
}

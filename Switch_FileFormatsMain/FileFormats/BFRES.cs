using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.IO;
using Bfres.Structs;

namespace FirstPlugin
{
    public class BFRES : IFileFormat
    {
        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "BFRES", "*BFRES", "*BFRES" };
        public string[] Extension { get; set; } = new string[] { "*.bfres", "*.sbfres" };
        public string Magic { get; set; } = "FRES";
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        private TreeNodeFile eitorRoot;
        public TreeNodeFile EditorRoot
        {
            get
            {
                return eitorRoot;
            }
            set
            {
                this.eitorRoot = value;
            }
        }
        private void SaveFile()
        {
            List<IFileFormat> formats = new List<IFileFormat>();
            formats.Add(this);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(formats);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                SaveCompressFile(Save(), sfd.FileName, Alignment);
            }
        }
        private void SaveCompressFile(byte[] data, string FileName, int Alignment = 0, bool EnableDialog = true)
        {
            if (EnableDialog && CompressionType != CompressionType.None)
            {
                DialogResult save = MessageBox.Show($"Compress file as {CompressionType}?", "File Save", MessageBoxButtons.YesNo);

                if (save == DialogResult.Yes)
                {
                    switch (CompressionType)
                    {
                        case CompressionType.Yaz0:
                            data = EveryFileExplorer.YAZ0.Compress(data, Runtime.Yaz0CompressionLevel, (uint)Alignment);
                            break;
                        case CompressionType.Lz4f:
                            data = STLibraryCompression.Type_LZ4F.Compress(data);
                            break;
                        case CompressionType.Lz4:
                            break;
                    }
                }
            }
            File.WriteAllBytes(FileName, data);
            MessageBox.Show($"File has been saved to {FileName}");
            Cursor.Current = Cursors.Default;
        }

        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public int Alignment { get; set; } = 0;
        public string FilePath { get; set; }
        public static bool IsWiiU = false;

        BFRESRender bfres;
        public void Load()
        {
            IsActive = true;
            CanSave = true;

            using (FileReader reader = new FileReader(new MemoryStream(Data)))
            {
                reader.Seek(4);
                if (reader.ReadInt32() != 0x20202020)
                {
                    IsWiiU = true;
                }
                reader.Close();
            }

            bfres = new BFRESRender();
            bfres.ResFileNode = new ResourceFile(this);
            bfres.ResFileNode.BFRESRender = bfres;
            bfres.SaveFile = SaveFile;

            EditorRoot = bfres.ResFileNode;

            if (IsWiiU)
            {
                bfres.LoadFile(new Syroot.NintenTools.Bfres.ResFile(new System.IO.MemoryStream(Data)));
            }
            else
            {
                bfres.LoadFile(new Syroot.NintenTools.NSW.Bfres.ResFile(new System.IO.MemoryStream(Data)));
            }

            Runtime.abstractGlDrawables.Add(bfres);
        }
        public void Unload()
        {
            bfres.Destroy();
            bfres.ResFileNode.Nodes.Clear();
        }

        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();

            if (IsWiiU)
                SaveWiiU(mem);
            else
                SaveSwitch(mem);

            return mem.ToArray();
        }
        private void SaveSwitch(MemoryStream mem)
        {
            int CurMdl = 0;
            foreach (FMDL model in bfres.models)
            {
                bfres.resFile.Models[CurMdl].Shapes.Clear();
                bfres.resFile.Models[CurMdl].VertexBuffers.Clear();
                bfres.resFile.Models[CurMdl].Materials.Clear();

                int i = 0;
                var duplicates = model.shapes.GroupBy(c => c.Text).Where(g => g.Skip(1).Any()).SelectMany(c => c);
                foreach (var shape in duplicates)
                    shape.Text += i++;

                foreach (FSHP shape in model.shapes)
                {
                    CheckMissingTextures(shape);
                    BfresSwitch.SetShape(shape, shape.Shape);

                    bfres.resFile.Models[CurMdl].Shapes.Add(shape.Shape);
                    bfres.resFile.Models[CurMdl].VertexBuffers.Add(shape.VertexBuffer);
                    shape.Shape.VertexBufferIndex = (ushort)(bfres.resFile.Models[CurMdl].VertexBuffers.Count - 1);

                    SetShaderAssignAttributes(shape.GetMaterial().shaderassign, shape);
                }
                foreach (FMAT mat in model.materials.Values)
                {
                    BfresSwitch.SetMaterial(mat, mat.Material);
                    bfres.resFile.Models[CurMdl].Materials.Add(mat.Material);
                }
                CurMdl++;
            }
            bfres.resFile.SkeletalAnims.Clear();
            if (EditorRoot.Nodes.ContainsKey("FSKA"))
            {
                foreach (BfresSkeletonAnim ska in EditorRoot.Nodes["FSKA"].Nodes)
                {
                    bfres.resFile.SkeletalAnims.Add(ska.SkeletalAnim);
                }
            }

            ErrorCheck();

            BfresSwitch.WriteExternalFiles(bfres.resFile, EditorRoot);
            bfres.resFile.Save(mem);
        }
        private void SaveWiiU(MemoryStream mem)
        {
            bfres.resFileU.Save(mem);

            int CurMdl = 0;
            foreach (FMDL model in bfres.models)
            {
                bfres.resFileU.Models[CurMdl].Shapes.Clear();
                bfres.resFileU.Models[CurMdl].VertexBuffers.Clear();
                bfres.resFileU.Models[CurMdl].Materials.Clear();

                int i = 0;
                var duplicates = model.shapes.GroupBy(c => c.Text).Where(g => g.Skip(1).Any()).SelectMany(c => c);
                foreach (var shape in duplicates)
                    shape.Text += i++;

                foreach (FSHP shape in model.shapes)
                {
                    CheckMissingTextures(shape);
                    BfresWiiU.SetShape(shape, shape.ShapeU);

                    bfres.resFileU.Models[CurMdl].Shapes.Add(shape.Text, shape.ShapeU);
                    bfres.resFileU.Models[CurMdl].VertexBuffers.Add(shape.VertexBufferU);
                    shape.ShapeU.VertexBufferIndex = (ushort)(bfres.resFileU.Models[CurMdl].VertexBuffers.Count - 1);

                    SetShaderAssignAttributes(shape.GetMaterial().shaderassign, shape);
                }
                foreach (FMAT mat in model.materials.Values)
                {
                    BfresWiiU.SetMaterial(mat, mat.MaterialU);
                    bfres.resFileU.Models[CurMdl].Materials.Add(mat.Text, mat.MaterialU);
                }
                CurMdl++;
            }
        }

        private void SetShaderAssignAttributes(FMAT.ShaderAssign shd, FSHP shape)
        {
            foreach (var att in shape.vertexAttributes)
            {
                if (!shd.attributes.ContainsValue(att.Name) && !shd.attributes.ContainsKey(att.Name))
                    shd.attributes.Add(att.Name, att.Name);
            }
            foreach (var tex in shape.GetMaterial().textures)
            {
                if (!shd.samplers.ContainsValue(tex.SamplerName))
                    shd.attributes.Add(tex.SamplerName, tex.SamplerName);
            }
        }


        private void SetDuplicateShapeName(FSHP shape)
        {
            DialogResult dialogResult = MessageBox.Show($"A shape {shape.Text} already exists with that name", "", MessageBoxButtons.OK);

            if (dialogResult == DialogResult.OK)
            {
                RenameDialog renameDialog = new RenameDialog();
                renameDialog.Text = "Rename Texture";
                if (renameDialog.ShowDialog() == DialogResult.OK)
                {
                    shape.Text = renameDialog.textBox1.Text;
                }
            }
        }

        bool ImportMissingTextures = false;
        private void CheckMissingTextures(FSHP shape)
        {
            foreach (BinaryTextureContainer bntx in PluginRuntime.bntxContainers)
            {
                foreach (MatTexture tex in shape.GetMaterial().textures)
                {
                    if (!bntx.Textures.ContainsKey(tex.Name))
                    {
                        if (!ImportMissingTextures)
                        {
                            DialogResult result = MessageBox.Show("Missing textures found! Would you like to use placeholders?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                ImportMissingTextures = true;
                            }
                            else
                            {
                                return;
                            }
                        }

                        if (ImportMissingTextures)
                            bntx.ImportPlaceholderTexture(tex.Name);
                    }
                }
            }
        }

        public void ErrorCheck()
        {
            if (bfres != null)
            {
                List<Errors> Errors = new List<Errors>();
                foreach (FMDL model in bfres.models)
                {
                    foreach (FSHP shp in model.shapes)
                    {
                        Syroot.NintenTools.NSW.Bfres.VertexBuffer vtx = shp.VertexBuffer;
                        Syroot.NintenTools.NSW.Bfres.Material mat = shp.GetMaterial().Material;
                        Syroot.NintenTools.NSW.Bfres.ShaderAssign shdr = mat.ShaderAssign;

                        for (int att = 0; att < vtx.Attributes.Count; att++)
                        {
                            if (!shdr.AttribAssigns.Contains(vtx.Attributes[att].Name))
                                MessageBox.Show($"Error! Attribute {vtx.Attributes[att].Name} is unlinked!");
                        }
                        for (int att = 0; att < mat.TextureRefs.Count; att++)
                        {
                            if (!shdr.SamplerAssigns.Contains(mat.SamplerDict.GetKey(att))) //mat.SamplerDict[att]
                                MessageBox.Show($"Error! Sampler {mat.SamplerDict.GetKey(att)} is unlinked!");
                        }
                    }
                }
             //   ErrorList errorList = new ErrorList();
             //   errorList.LoadList(Errors);
            //    errorList.Show();
            }
        }
        public class Errors
        {
            public string Section = "None";
            public string Section2 = "None";
            public string Message = "";
            public string Type = "Unkown";
        }
    }
}

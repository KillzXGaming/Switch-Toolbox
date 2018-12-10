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
using ResU = Syroot.NintenTools.Bfres;
using Syroot.NintenTools.NSW.Bfres;

namespace FirstPlugin
{
    public class BFRES : TreeNodeFile, IFileFormat
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
        public string FilePath { get; set; }

        public IFileInfo IFileInfo { get; set; }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public int Alignment { get; set; } = 0;
        public bool IsWiiU
        {
            get
            {
                if (Data == null)
                    return false;

                using (FileReader reader = new FileReader(new MemoryStream(Data)))
                {
                    reader.Seek(4);
                    if (reader.ReadInt32() != 0x20202020)
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
        }

        public BFRESRender BFRESRender;
        public void Load()
        {
            IsActive = true;
            CanSave = true;



            ImageKey = "bfres";
            SelectedImageKey = "bfres";

            ContextMenu = new ContextMenu();
            MenuItem save = new MenuItem("Save");
            ContextMenu.MenuItems.Add(save);
            save.Click += Save;

            MenuItem newMenu = new MenuItem("New");
            MenuItem import = new MenuItem("Import");
            //       ContextMenu.MenuItems.Add(newMenu);
            //       ContextMenu.MenuItems.Add(import);

            MenuItem rename = new MenuItem("Rename");
            ContextMenu.MenuItems.Add(rename);
            rename.Click += Rename;
            MenuItem remove = new MenuItem("Remove");
            ContextMenu.MenuItems.Add(remove);
            remove.Click += Remove;

            if (Parent == null)
                remove.Enabled = false;

            if (IsWiiU)
            {

            }
            else
            {
                MenuItem model = new MenuItem("Model");
                MenuItem fska = new MenuItem("Skeletal Animation");
                MenuItem fmaa = new MenuItem("Material Animation");
                MenuItem bonevis = new MenuItem("Bone Visual Animation");
                MenuItem shape = new MenuItem("Shape Animation");
                MenuItem scene = new MenuItem("Scene Animation");
                MenuItem embedded = new MenuItem("Embedded File");
                MenuItem texture = new MenuItem("Texture File");
                texture.Click += NewTextureFile;
                newMenu.MenuItems.Add(model);
                newMenu.MenuItems.Add(fska);
                newMenu.MenuItems.Add(fmaa);
                newMenu.MenuItems.Add(bonevis);
                newMenu.MenuItems.Add(shape);
                newMenu.MenuItems.Add(scene);
                newMenu.MenuItems.Add(embedded);
                newMenu.MenuItems.Add(texture);

                MenuItem importmodel = new MenuItem("Model");
                MenuItem importfska = new MenuItem("Skeletal Animation");
                MenuItem importfmaa = new MenuItem("Material Animation");
                MenuItem importbonevis = new MenuItem("Bone Visual Animation");
                MenuItem importshape = new MenuItem("Shape Animation");
                MenuItem importscene = new MenuItem("Scene Animation");
                MenuItem importembedded = new MenuItem("Embedded File");
                MenuItem importtexture = new MenuItem("Texture File");
                import.MenuItems.Add(importmodel);
                import.MenuItems.Add(importfska);
                import.MenuItems.Add(importfmaa);
                import.MenuItems.Add(importbonevis);
                import.MenuItems.Add(importshape);
                import.MenuItems.Add(importscene);
                import.MenuItems.Add(importembedded);
                import.MenuItems.Add(importtexture);
            }

            BFRESRender = new BFRESRender();
            BFRESRender.ResFileNode = this;

            if (IsWiiU)
            {
                BFRESRender.LoadFile(new Syroot.NintenTools.Bfres.ResFile(new System.IO.MemoryStream(Data)));
            }
            else
            {
                BFRESRender.LoadFile(new Syroot.NintenTools.NSW.Bfres.ResFile(new System.IO.MemoryStream(Data)));
            }

            Runtime.abstractGlDrawables.Add(BFRESRender);
        }
        public void Unload()
        {
            BFRESRender.Destroy();
            BFRESRender.ResFileNode.Nodes.Clear();
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

        public ResFile resFile = null;
        public ResU.ResFile resFileU = null;

        public TreeNode TextureFolder = new TreeNode("Textures");

        public override void OnClick(TreeView treeView)
        {
            //If has models
            if (Nodes.ContainsKey("FMDL"))
            {
                LibraryGUI.Instance.LoadViewport(Viewport.Instance);
                Viewport.Instance.gL_ControlModern1.MainDrawable = BFRESRender;

                BFRESRender.UpdateVertexData();
            }
        }
        public void Load(ResU.ResFile res)
        {
            resFileU = res;

            Text = resFileU.Name;

            if (resFileU.Models.Count > 0)
                Nodes.Add(new FmdlFolder());
            if (resFileU.Textures.Count > 0)
                AddFTEXTextures(resFileU);
            if (resFileU.SkeletalAnims.Count > 0)
                AddSkeletonAnims(resFileU);
            if (resFileU.ShaderParamAnims.Count > 0)
                Nodes.Add(new FshuFolder());
            if (resFileU.ColorAnims.Count > 0)
                Nodes.Add(new FshuColorFolder());
            if (resFileU.TexSrtAnims.Count > 0)
                Nodes.Add(new TexSrtFolder());
            if (resFileU.TexPatternAnims.Count > 0)
                Nodes.Add(new TexPatFolder());
            if (resFileU.ShapeAnims.Count > 0)
                Nodes.Add(new FshpaFolder());
            if (resFileU.BoneVisibilityAnims.Count > 0)
                Nodes.Add(new FbnvFolder());
            if (resFileU.SceneAnims.Count > 0)
                Nodes.Add(new FscnFolder());
            if (resFileU.ExternalFiles.Count > 0)
                Nodes.Add(new EmbeddedFilesFolder());

            foreach (var anim in resFileU.ShaderParamAnims)
                Nodes["FSHA"].Nodes.Add(anim.Key);
            foreach (var anim in resFileU.ColorAnims)
                Nodes["FSHAColor"].Nodes.Add(anim.Key);
            foreach (var anim in resFileU.TexSrtAnims)
                Nodes["TEXSRT"].Nodes.Add(anim.Key);
            foreach (var anim in resFileU.TexPatternAnims)
                Nodes["TEXPAT"].Nodes.Add(anim.Key);

            int ext = 0;
            foreach (var extfile in resFileU.ExternalFiles)
            {
                string Name = extfile.Key;

                FileReader f = new FileReader(extfile.Value.Data);
                string Magic = f.ReadMagic(0, 4);
                if (Magic == "FSHA")
                {
                    Nodes["EXT"].Nodes.Add(new BfshaFileData(extfile.Value.Data, Name));
                }
                else
                    Nodes["EXT"].Nodes.Add(new ExternalFileData(extfile.Value.Data, Name));

                f.Dispose();
                f.Close();

                ext++;
            }
        }
        public void Load(ResFile res)
        {
            resFile = res;

            Text = resFile.Name;
            UpdateTree(resFile);

            foreach (ShapeAnim anim in resFile.ShapeAnims)
                Nodes["FSHPA"].Nodes.Add(anim.Name);
            foreach (VisibilityAnim anim in resFile.BoneVisibilityAnims)
                Nodes["FBNV"].Nodes.Add(anim.Name);

            int ext = 0;
            foreach (ExternalFile extfile in resFile.ExternalFiles)
            {
                string Name = resFile.ExternalFileDict.GetKey(ext);

                FileReader f = new FileReader(extfile.Data);
                string Magic = f.ReadMagic(0, 4);
                if (Magic == "BNTX")
                {
                    BNTX bntx = new BNTX();
                    bntx.Data = extfile.Data;
                    bntx.FileName = Name;
                    bntx.Load();
                    bntx.IFileInfo.InArchive = true;
                    Nodes["EXT"].Nodes.Add(bntx);
                }
                else if (Magic == "FSHA")
                {
                    Nodes["EXT"].Nodes.Add(new BfshaFileData(extfile.Data, Name));
                }
                else
                    Nodes["EXT"].Nodes.Add(new ExternalFileData(extfile.Data, Name));

                f.Dispose();
                f.Close();

                ext++;
            }
        }
        private void NewTextureFile(object sender, EventArgs args)
        {
            string Name = "textures";
            for (int i = 0; i < resFile.ExternalFiles.Count; i++)
            {
                if (resFile.ExternalFileDict.GetKey(i) == Name)
                    Name = Name + i;
            }
            if (!Nodes.ContainsKey("EXT"))
            {
                Nodes.Add(new EmbeddedFilesFolder());
            }
            BNTX bntx = new BNTX();
            bntx.Data = new byte[0];
            bntx.FileName = "textures";
            Nodes["EXT"].Nodes.Add(bntx);
        }
        private void NewEmbeddedFile(object sender, EventArgs args)
        {
        }
        private void Save(object sender, EventArgs args)
        {
            List<IFileFormat> formats = new List<IFileFormat>();
            formats.Add(this);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(formats);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName, Alignment);
            }
        }
        private void Rename(object sender, EventArgs args)
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Text = dialog.textBox1.Text;
            }
        }
        private void Remove(object sender, EventArgs args)
        {
            BFRESRender.DisposeFile();
        }
        private void UpdateTree(ResFile resFile)
        {
            if (resFile.Models.Count > 0)
                Nodes.Add(new FmdlFolder());
            if (resFile.SkeletalAnims.Count > 0)
                AddSkeletonAnims(resFile);
            if (resFile.MaterialAnims.Count > 0)
                AddMaterialAnims(resFile);
            if (resFile.ShapeAnims.Count > 0)
                AddShapeAnims(resFile);
            if (resFile.BoneVisibilityAnims.Count > 0)
                AddBoneVisAnims(resFile);
            if (resFile.SceneAnims.Count > 0)
                AddSceneAnims(resFile);
            if (resFile.ExternalFiles.Count > 0)
                Nodes.Add(new EmbeddedFilesFolder());
        }
        private void AddFTEXTextures(ResU.ResFile resFile)
        {
            FTEXContainer ftexContainer = new FTEXContainer();
            Nodes.Add(ftexContainer);
            foreach (ResU.Texture tex in resFile.Textures.Values)
            {
                string TextureName = tex.Name;
                FTEX texture = new FTEX();
                ftexContainer.Nodes.Add(texture);
                texture.Read(tex);
                ftexContainer.Textures.Add(texture.Text, texture);
            }
            PluginRuntime.ftexContainers.Add(ftexContainer);
        }
        private void AddSkeletonAnims(ResU.ResFile resFile)
        {
            FskaFolder fksaFolder = new FskaFolder();
            Nodes.Add(fksaFolder);
            foreach (ResU.SkeletalAnim ska in resFile.SkeletalAnims.Values)
            {
                BfresSkeletonAnim skeletonAnim = new BfresSkeletonAnim(ska.Name);
                skeletonAnim.Read(ska, resFile);
                fksaFolder.Nodes.Add(skeletonAnim);
            }
        }
        private void AddSkeletonAnims(ResFile resFile)
        {
            FskaFolder fksaFolder = new FskaFolder();
            Nodes.Add(fksaFolder);
            foreach (SkeletalAnim ska in resFile.SkeletalAnims)
            {
                BfresSkeletonAnim skeletonAnim = new BfresSkeletonAnim(ska.Name);
                skeletonAnim.Read(ska, resFile);
                fksaFolder.Nodes.Add(skeletonAnim);
            }
        }
        private void AddSceneAnims(ResU.ResFile resFile)
        {
            FscnFolder FSCN = new FscnFolder();
            Nodes.Add(FSCN);
        }
        private void AddSceneAnims(ResFile resFile)
        {
            FscnFolder fscnFolder = new FscnFolder();
            Nodes.Add(fscnFolder);
            foreach (var scn in resFile.SceneAnims)
            {
                FSCN sceneAnim = new FSCN();
                sceneAnim.Text = scn.Name;
                sceneAnim.Read(scn);
                fscnFolder.Nodes.Add(sceneAnim);
            }
        }
        private void AddMaterialAnims(ResFile resFile)
        {
            FmaaFolder fmaaFolder = new FmaaFolder();
            Nodes.Add(fmaaFolder);
            foreach (var fmaa in resFile.MaterialAnims)
            {
                FMAA materialAnim = new FMAA();
                materialAnim.Text = fmaa.Name;
                materialAnim.BFRESRender = BFRESRender;
                materialAnim.Read(fmaa);
                fmaaFolder.Nodes.Add(materialAnim);
            }
        }
        private void AddShapeAnims(ResFile resFile)
        {
            FshpaFolder fshaFolder = new FshpaFolder();
            Nodes.Add(fshaFolder);
            foreach (var fsha in resFile.ShapeAnims)
            {
                FSHA shapeAnim = new FSHA();
                shapeAnim.Text = fsha.Name;
                shapeAnim.Read(fsha);
                fshaFolder.Nodes.Add(shapeAnim);
            }
        }
        private void AddBoneVisAnims(ResFile resFile)
        {
            FbnvFolder fbnvFolder = new FbnvFolder();
            Nodes.Add(fbnvFolder);
            foreach (var fbnv in resFile.BoneVisibilityAnims)
            {
                FBNV boneVis = new FBNV();
                boneVis.Text = fbnv.Name;
                boneVis.Read(fbnv);
                fbnvFolder.Nodes.Add(boneVis);
            }
        }
        private void SaveSwitch(MemoryStream mem)
        {
            var resFile = BFRESRender.ResFileNode.resFile;

            resFile.Models.Clear();
            resFile.SkeletalAnims.Clear();
            resFile.MaterialAnims.Clear();
            resFile.SceneAnims.Clear();
            resFile.ShapeAnims.Clear();
            resFile.BoneVisibilityAnims.Clear();
            resFile.ModelDict.Clear();
            resFile.SkeletalAnimDict.Clear();
            resFile.MaterialAnimDict.Clear();
            resFile.SceneAnimDict.Clear();
            resFile.ShapeAnimDict.Clear();
            resFile.BoneVisibilityAnimDict.Clear();


            int CurMdl = 0;
            if (Nodes.ContainsKey("FMDL"))
            {
                foreach (FMDL model in Nodes["FMDL"].Nodes)
                    resFile.Models.Add(BfresSwitch.SetModel(model));
            }
            if (Nodes.ContainsKey("FSKA"))
            {
                foreach (BfresSkeletonAnim ska in Nodes["FSKA"].Nodes)
                    resFile.SkeletalAnims.Add(ska.SkeletalAnim);
            }
            if (Nodes.ContainsKey("FMAA"))
            {
                foreach (FMAA fmaa in Nodes["FMAA"].Nodes)
                    resFile.MaterialAnims.Add(fmaa.MaterialAnim);
            }
            if (Nodes.ContainsKey("FBNV"))
            {
                foreach (FBNV fbnv in Nodes["FBNV"].Nodes)
                    resFile.BoneVisibilityAnims.Add(fbnv.VisibilityAnim);
            }
            if (Nodes.ContainsKey("FSHPA"))
            {
                foreach (FSHA fsha in Nodes["FSHPA"].Nodes)
                    resFile.ShapeAnims.Add(fsha.ShapeAnim);
            }
            if (Nodes.ContainsKey("FSCN"))
            {
                foreach (FSCN fscn in Nodes["FSCN"].Nodes)
                    resFile.SceneAnims.Add(fscn.SceneAnim);
            }

            ErrorCheck();

            BfresSwitch.WriteExternalFiles(resFile, this);
            resFile.Save(mem);
        }
        private void SaveWiiU(MemoryStream mem)
        {
            var resFileU = BFRESRender.ResFileNode.resFileU;
            resFileU.Models.Clear();
       //     resFileU.SkeletalAnims.Clear();
       //     resFileU.SceneAnims.Clear();
       //     resFileU.ShapeAnims.Clear();
     //       resFileU.BoneVisibilityAnims.Clear();
            resFileU.Textures.Clear();


            int CurMdl = 0;
            if (Nodes.ContainsKey("FMDL"))
            {
                foreach (FMDL model in Nodes["FMDL"].Nodes)
                    resFileU.Models.Add(model.Text, BfresWiiU.SetModel(model));
            }
            if (Nodes.ContainsKey("FTEX"))
            {
                foreach (FTEX tex in Nodes["FTEX"].Nodes)
                {
                    tex.texture.Name = tex.Text;
                    resFileU.Textures.Add(tex.Text, tex.texture);
                }
            }
            else
                throw new Exception("Failed to find textures");

            ErrorCheck();
            resFileU.Save(mem);
        }

        public static void SetShaderAssignAttributes(FMAT.ShaderAssign shd, FSHP shape)
        {
            foreach (var att in shape.vertexAttributes)
            {
                if (!shd.attributes.ContainsValue(att.Name) && !shd.attributes.ContainsKey(att.Name))
                    shd.attributes.Add(att.Name, att.Name);
            }
            foreach (var tex in shape.GetMaterial().TextureMaps)
            {
                if (!shd.samplers.ContainsValue(((MatTexture)tex).SamplerName))
                    shd.samplers.Add(((MatTexture)tex).SamplerName, ((MatTexture)tex).SamplerName);
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

        public static void CheckMissingTextures(FSHP shape)
        {
            bool ImportMissingTextures = false;
            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                foreach (MatTexture tex in shape.GetMaterial().TextureMaps)
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
            if (BFRESRender != null)
            {
                List<Errors> Errors = new List<Errors>();
                foreach (FMDL model in BFRESRender.models)
                {
                    foreach (FSHP shp in model.shapes)
                    {
                        if (!IsWiiU)
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
                        else
                        {
                            Syroot.NintenTools.Bfres.VertexBuffer vtx = shp.VertexBufferU;
                            Syroot.NintenTools.Bfres.Material mat = shp.GetMaterial().MaterialU;
                            Syroot.NintenTools.Bfres.ShaderAssign shdr = mat.ShaderAssign;

                            for (int att = 0; att < vtx.Attributes.Count; att++)
                            {
                                if (!shdr.AttribAssigns.ContainsKey(vtx.Attributes[att].Name))
                                    MessageBox.Show($"Error! Attribute {vtx.Attributes[att].Name} is unlinked!");
                            }
                            for (int att = 0; att < mat.TextureRefs.Count; att++)
                            {
                                string samp = "";
                                mat.Samplers.TryGetKey(mat.Samplers[att], out samp);
                                if (!shdr.SamplerAssigns.ContainsKey(samp)) //mat.SamplerDict[att]
                                    MessageBox.Show($"Error! Sampler {samp} is unlinked!");
                            }
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

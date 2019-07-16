using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using Syroot.NintenTools.NSW.Bfres.Helpers;
using OpenTK;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using WeifenLuo.WinFormsUI.Docking;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using ResU = Syroot.NintenTools.Bfres;
using ResUGX2 = Syroot.NintenTools.Bfres.GX2;
using ResGFX = Syroot.NintenTools.NSW.Bfres.GFX;
using FirstPlugin;

namespace Bfres.Structs
{
    public class Misc
    {
        public static List<string> HackyTextureList = new List<string>(new string[] {
           "Alb", "alb", "Base", "base", "bonbon.167300917","Eye.00","EyeIce.00", "FaceDummy", "Eye01.17", "Dee.00",
            "rainbow.758540574", "Mucus._1700670200", "Eye.11", "CapTail00","eye.0","pallet_texture","Mark.930799313","InEye.1767598300","Face.00",
            "ThunderHair_Thunder_BaseColor.1751853236","FireHair_Thunder_BaseColor._162539711","IceHair_Thunder_BaseColor.674061150","BodyEnemy.1866226988",
            "Common_Scroll01._13827715"
        });
    }

    public class ResourceFile : TreeNodeFile
    {
        public BFRESRender BFRESRender;

        public TreeNode TextureFolder = new TreeNode("Textures");
        public ResourceFile(IFileFormat handler)
        {
            ImageKey = "bfres";
            SelectedImageKey = "bfres";
            FileHandler = handler;

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

            if (BFRES.IsWiiU)
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
                newMenu .MenuItems.Add(fska);
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
            
        }
        public override void OnClick(TreeView treeView)
        {
            //If has models
            if (Nodes.ContainsKey("FMDLFolder"))
            {
                if (Nodes["FMDLFolder"].Nodes.ContainsKey("FshpFolder"))
                {

                }
                LibraryGUI.Instance.LoadViewport(Viewport.Instance);
                Viewport.Instance.gL_ControlModern1.MainDrawable = BFRESRender;

                BFRESRender.UpdateVertexData();
            }
        }
        public void Load(ResU.ResFile resFile)
        {
            Text = resFile.Name;

            if (resFile.Models.Count > 0)
                Nodes.Add(new FmdlFolder());
            if (resFile.Textures.Count > 0)
                AddFTEXTextures(resFile);
            if (resFile.SkeletalAnims.Count > 0)
                AddSkeletonAnims(resFile);
            if (resFile.ShaderParamAnims.Count > 0)
                Nodes.Add(new FshaFolder());
            if (resFile.ColorAnims.Count > 0)
                Nodes.Add(new FshaColorFolder());
            if (resFile.TexSrtAnims.Count > 0)
                Nodes.Add(new TexSrtFolder());
            if (resFile.TexPatternAnims.Count > 0)
                Nodes.Add(new TexPatFolder());
            if (resFile.ShapeAnims.Count > 0)
                Nodes.Add(new FshpaFolder());
            if (resFile.BoneVisibilityAnims.Count > 0)
                Nodes.Add(new FbnvFolder());
            if (resFile.SceneAnims.Count > 0)
                Nodes.Add(new FscnFolder());
            if (resFile.ExternalFiles.Count > 0)
                Nodes.Add(new EmbeddedFilesFolder());

            foreach (var anim in resFile.ShaderParamAnims)
                Nodes["FSHA"].Nodes.Add(anim.Key);
            foreach (var anim in resFile.ColorAnims)
                Nodes["FSHAColor"].Nodes.Add(anim.Key);
            foreach (var anim in resFile.TexSrtAnims)
                Nodes["TEXSRT"].Nodes.Add(anim.Key);
            foreach (var anim in resFile.TexPatternAnims)
                Nodes["TEXPAT"].Nodes.Add(anim.Key);

            int ext = 0;
            foreach (var extfile in resFile.ExternalFiles)
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
        public void Load(ResFile resFile)
        {
            Text = resFile.Name;
            UpdateTree(resFile);

            foreach (MaterialAnim anim in resFile.MaterialAnims)
                Nodes["FMAA"].Nodes.Add(anim.Name);
            foreach (ShapeAnim anim in resFile.ShapeAnims)
                Nodes["FSHPA"].Nodes.Add(anim.Name);
            foreach (VisibilityAnim anim in resFile.BoneVisibilityAnims)
                Nodes["FBNV"].Nodes.Add(anim.Name);
            foreach (SceneAnim anim in resFile.SceneAnims)
                Nodes["FSCN"].Nodes.Add(anim.Name);

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
                    Nodes["EXT"].Nodes.Add(bntx.EditorRoot);
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
            for (int i = 0; i < BFRESRender.resFile.ExternalFiles.Count; i++)
            {
                if (BFRESRender.resFile.ExternalFileDict.GetKey(i) == Name)
                    Name = Name + i;
            }
            if (!Nodes.ContainsKey("EXT"))
            {
                Nodes.Add(new EmbeddedFilesFolder());
            }
            BNTX bntx = new BNTX();
            bntx.Data = new byte[0];
            BinaryTextureContainer bntxTreeNode = new BinaryTextureContainer(new byte[0], "textures", BFRESRender.resFile.Name);
            Nodes["EXT"].Nodes.Add(bntxTreeNode);

        }
        private void NewEmbeddedFile(object sender, EventArgs args)
        {
        }
        private void Save(object sender, EventArgs args)
        {
            ((BFRES)FileHandler).SaveFile();
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
                Nodes.Add(new FmmaFolder());
            if (resFile.ShapeAnims.Count > 0)
                Nodes.Add(new FshpaFolder());
            if (resFile.BoneVisibilityAnims.Count > 0)
                Nodes.Add(new FbnvFolder());
            if (resFile.SceneAnims.Count > 0)
                Nodes.Add(new FscnFolder());
            if (resFile.ExternalFiles.Count > 0)
                Nodes.Add(new EmbeddedFilesFolder());
        }
        private void AddFTEXTextures(ResU.ResFile resFile)
        {
            FTEXContainer ftexContainer = new FTEXContainer();
            foreach (ResU.Texture tex in resFile.Textures.Values)
            {
                string TextureName = tex.Name;
                FTEX texture = new FTEX();
                texture.Read(tex);
                ftexContainer.Nodes.Add(texture);
                ftexContainer.Textures.Add(texture.Text, texture);
            }
            PluginRuntime.ftexContainers.Add(ftexContainer);
            Nodes.Add(ftexContainer);
        }
        private void AddSkeletonAnims(ResU.ResFile resFile)
        {
            FskaFolder FSKA = new FskaFolder();
            FSKA.LoadAnimations(resFile, BFRESRender);
            Nodes.Add(FSKA);
        }
        private void AddSkeletonAnims(ResFile resFile)
        {
            FskaFolder FSKA = new FskaFolder();
            FSKA.LoadAnimations(resFile, BFRESRender);
            Nodes.Add(FSKA);
        }
    }
    public class FshpaFolder : TreeNodeCustom
    {
        public FshpaFolder()
        {
            Text = "Shape Animations";
            Name = "FSHPA";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
}

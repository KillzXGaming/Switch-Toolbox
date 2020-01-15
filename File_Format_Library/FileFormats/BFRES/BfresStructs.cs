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
    public class FskaFolder : AnimationGroupNode
    {
        public FskaFolder()
        {
            Text = "Skeleton Animations";
            Name = "FSKA";
        }
        public void LoadAnimations(ResU.ResFile resFile, BFRESRender BFRESRender)
        {
            foreach (var ska in resFile.SkeletalAnims.Values)
            {
                BfresSkeletonAnim skeletonAnim = new BfresSkeletonAnim(ska.Name);
                skeletonAnim.BFRESRender = BFRESRender;
                skeletonAnim.Read(ska, resFile);
                Nodes.Add(skeletonAnim);
            }
        }
        public void LoadAnimations(ResFile resFile, BFRESRender BFRESRender)
        {
            foreach (SkeletalAnim ska in resFile.SkeletalAnims)
            {
                BfresSkeletonAnim skeletonAnim = new BfresSkeletonAnim(ska.Name);
                skeletonAnim.BFRESRender = BFRESRender;
                skeletonAnim.Read(ska, resFile);
                Nodes.Add(skeletonAnim);
            }
        }
     /*   public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }*/
    }
    public class FmdlFolder : TreeNodeCustom
    {
        public FmdlFolder()
        {
            Text = "Models";
            Name = "FMDLFolder";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
    public class FmmaFolder : TreeNodeCustom
    {
        public FmmaFolder()
        {
            Text = "Material Animations";
            Name = "FMAA";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
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
    public class FbnvFolder : TreeNodeCustom
    {
        public FbnvFolder()
        {
            Text = "Bone Visabilty Animations";
            Name = "FBNV";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
    public class FscnFolder : TreeNodeCustom
    {
        public FscnFolder()
        {
            Text = "Scene Animations";
            Name = "FSCN";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
    public class EmbeddedFilesFolder : TreeNodeCustom
    {
        public EmbeddedFilesFolder()
        {
            Text = "Embedded Files";
            Name = "EXT";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
    public class TexPatFolder : TreeNodeCustom
    {
        public TexPatFolder()
        {
            Text = "Texture Pattern Animations";
            Name = "TEXPAT";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
    public class TexSrtFolder : TreeNodeCustom
    {
        public TexSrtFolder()
        {
            Text = "Texture SRT Animations";
            Name = "TEXSRT";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
    public class FshaFolder : TreeNodeCustom
    {
        public FshaFolder()
        {
            Text = "Shader Parameter Animations";
            Name = "FSHA";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
    public class FshaColorFolder : TreeNodeCustom
    {
        public FshaColorFolder()
        {
            Text = "Color Animations";
            Name = "FSHAColor";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
    public class BfshaFileData : TreeNode
    {
        public byte[] Data;
        public BfshaFileData(byte[] data, string Name)
        {
            Text = Name;
            ImageKey = "bfsha";
            SelectedImageKey = "bfsha";
            Data = data;

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Import;
        }


        private void Import(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Data = System.IO.File.ReadAllBytes(ofd.FileName);
            }
        }

        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All files(*.*)|*.*";
            sfd.DefaultExt = System.IO.Path.GetExtension(Text);
            sfd.FileName = Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllBytes(sfd.FileName, Data);

            }
        }
    }
    public class ExternalFileData : TreeNode
    {
        public byte[] Data;
        public ExternalFileData(byte[] data, string Name)
        {
            Text = Name;
            ImageKey = "folder";
            Data = data;

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Import;
        }


        private void Import(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Data = System.IO.File.ReadAllBytes(ofd.FileName);
            }
        }

        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All files(*.*)|*.*";

            sfd.DefaultExt = System.IO.Path.GetExtension(Text);
            sfd.FileName = Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllBytes(sfd.FileName, Data);

            }
        }
    }
    public class BfresSkeletonAnim : Animation
    {
        public enum TrackType
        {
            XSCA = 0x4,
            YSCA = 0x8,
            ZSCA = 0xC,
            XPOS = 0x10,
            YPOS = 0x14,
            ZPOS = 0x18,
            XROT = 0x20,
            YROT = 0x24,
            ZROT = 0x28,
        }
        public SkeletalAnim SkeletalAnim;
        public BFRESRender BFRESRender;

        public BfresSkeletonAnim()
        {
            ImageKey = "skeletonAnimation";
            SelectedImageKey = "skeletonAnimation";

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Replace;
        }
        public BfresSkeletonAnim(string name)
        {
            Text = name;

            ImageKey = "skeletonAnimation";
            SelectedImageKey = "skeletonAnimation";

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Replace;
        }
        public void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.bfska;|" +
             "Bfres Object (shape/vertices) |*.bfska|" +
             "All files(*.*)|*.*";
            sfd.FileName = Text;
            sfd.DefaultExt = ".bfska";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SkeletalAnim.Export(sfd.FileName, BFRESRender.resFile);
            }
        }
        public void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfska;|" +
             "Bfres Object (shape/vertices) |*.bfska|" +
             "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SkeletalAnim.Import(ofd.FileName);
            }
            SkeletalAnim.Name = Text;
        }

        public static List<Animation> SkeletonAnimations = new List<Animation>();

        public void Read(ResU.SkeletalAnim ska, ResU.ResFile b)
        {

        }
        public void Read(SkeletalAnim ska, ResFile b)
        {
            FrameCount = ska.FrameCount;
            SkeletalAnim = ska;

            foreach (BoneAnim bn in ska.BoneAnims)
            {
                FSKANode bonean = new FSKANode(bn);

                Animation.KeyNode bone = new Animation.KeyNode("");
                Bones.Add(bone);
                if (ska.FlagsRotate == SkeletalAnimFlagsRotate.EulerXYZ)
                    bone.RotType = Animation.RotationType.EULER;
                else
                    bone.RotType = Animation.RotationType.QUATERNION;

                bone.Text = bonean.Text;


                for (int Frame = 0; Frame < ska.FrameCount; Frame++)
                {
                    if (Frame == 0)
                    {
                        if (bn.FlagsBase.HasFlag(BoneAnimFlagsBase.Scale))
                        {
                            bone.XSCA.Keys.Add(new KeyFrame() { Frame = 0, Value = bonean.sca.X });
                            bone.YSCA.Keys.Add(new KeyFrame() { Frame = 0, Value = bonean.sca.Y });
                            bone.ZSCA.Keys.Add(new KeyFrame() { Frame = 0, Value = bonean.sca.Z });
                        }
                        if (bn.FlagsBase.HasFlag(BoneAnimFlagsBase.Rotate))
                        {
                            bone.XROT.Keys.Add(new KeyFrame() { Frame = 0, Value = bonean.rot.X });
                            bone.YROT.Keys.Add(new KeyFrame() { Frame = 0, Value = bonean.rot.Y });
                            bone.ZROT.Keys.Add(new KeyFrame() { Frame = 0, Value = bonean.rot.Z });
                            bone.WROT.Keys.Add(new KeyFrame() { Frame = 0, Value = bonean.rot.W });
                        }
                        if (bn.FlagsBase.HasFlag(BoneAnimFlagsBase.Translate))
                        {
                            bone.XPOS.Keys.Add(new KeyFrame() { Frame = 0, Value = bonean.pos.X });
                            bone.YPOS.Keys.Add(new KeyFrame() { Frame = 0, Value = bonean.pos.Y });
                            bone.ZPOS.Keys.Add(new KeyFrame() { Frame = 0, Value = bonean.pos.Z });
                        }
                    }
                    foreach (FSKATrack track in bonean.tracks)
                    {
                        KeyFrame frame = new KeyFrame();
                        frame.InterType = Animation.InterpolationType.HERMITE;
                        frame.Frame = Frame;

                        FSKAKey left = track.GetLeft(Frame);
                        FSKAKey right = track.GetRight(Frame);
                        float value;

                        value = Animation.Hermite(Frame, left.frame, right.frame, 0, 0, left.unk1, right.unk1);

                        // interpolate the value and apply
                        switch (track.flag)
                        {
                            case (int)TrackType.XPOS: frame.Value = value; bone.XPOS.Keys.Add(frame); break;
                            case (int)TrackType.YPOS: frame.Value = value; bone.YPOS.Keys.Add(frame); break;
                            case (int)TrackType.ZPOS: frame.Value = value; bone.ZPOS.Keys.Add(frame); break;
                            case (int)TrackType.XROT: frame.Value = value; bone.XROT.Keys.Add(frame); break;
                            case (int)TrackType.YROT: frame.Value = value; bone.YROT.Keys.Add(frame); break;
                            case (int)TrackType.ZROT: frame.Value = value; bone.ZROT.Keys.Add(frame); break;
                            case (int)TrackType.XSCA: frame.Value = value; bone.XSCA.Keys.Add(frame); break;
                            case (int)TrackType.YSCA: frame.Value = value; bone.YSCA.Keys.Add(frame); break;
                            case (int)TrackType.ZSCA: frame.Value = value; bone.ZSCA.Keys.Add(frame); break;
                        }
                    }
                }
            }

        }
        public class FSKANode
        {
            public int flags;
            public int flags2;
            public int stride;
            public int BeginRotate;
            public int BeginTranslate;
            public long offBase;
            public int trackCount;
            public int trackFlag;
            public long offTrack;
            public string Text;

            public Vector3 sca, pos;
            public Vector4 rot;
            public List<FSKATrack> tracks = new List<FSKATrack>();

            public FSKANode(BoneAnim b)
            {
                Text = b.Name;

                sca = new Vector3(b.BaseData.Scale.X, b.BaseData.Scale.Y, b.BaseData.Scale.Z);
                rot = new Vector4(b.BaseData.Rotate.X, b.BaseData.Rotate.Y, b.BaseData.Rotate.Z, b.BaseData.Rotate.W);
                pos = new Vector3(b.BaseData.Translate.X, b.BaseData.Translate.Y, b.BaseData.Translate.Z);

                foreach (AnimCurve tr in b.Curves)
                {

                    FSKATrack t = new FSKATrack();
                    t.flag = (int)tr.AnimDataOffset;
                    tracks.Add(t);

                    float tanscale = tr.Delta;
                    if (tanscale == 0)
                        tanscale = 1;

                    for (int i = 0; i < (ushort)tr.Frames.Length; i++)
                    {
                        if (tr.CurveType == AnimCurveType.Cubic)
                        {
                            int framedata = (int)tr.Frames[i];
                            float keydata = tr.Offset + ((tr.Keys[i, 0] * tr.Scale));
                            float keydata2 = tr.Offset + ((tr.Keys[i, 1] * tr.Scale));
                            float keydata3 = tr.Offset + ((tr.Keys[i, 2] * tr.Scale));
                            float keydata4 = tr.Offset + ((tr.Keys[i, 3] * tr.Scale));

                        }
                        if (tr.KeyType == AnimCurveKeyType.Int16)
                        {

                        }
                        else if (tr.KeyType == AnimCurveKeyType.Single)
                        {

                        }
                        else if (tr.KeyType == AnimCurveKeyType.SByte)
                        {

                        }
                        t.keys.Add(new FSKAKey()
                        {
                            frame = (int)tr.Frames[i],
                            unk1 = tr.Offset + ((tr.Keys[i, 0] * tr.Scale)),
                            unk2 = tr.Offset + ((tr.Keys[i, 1] * tr.Scale)),
                            unk3 = tr.Offset + ((tr.Keys[i, 2] * tr.Scale)),
                            unk4 = tr.Offset + ((tr.Keys[i, 3] * tr.Scale)),
                        });
                    }
                }
            }
        }

        public class FSKATrack
        {
            public short type;
            public short keyCount;
            public int flag;
            public int unk2;
            public int padding1;
            public int padding2;
            public int padding3;
            public float frameCount;
            public float scale, init, unkf3;
            public long offtolastKeys, offtolastData;
            public List<FSKAKey> keys = new List<FSKAKey>();

            public int offset;

            public FSKAKey GetLeft(int frame)
            {
                FSKAKey prev = keys[0];

                for (int i = 0; i < keys.Count - 1; i++)
                {
                    FSKAKey key = keys[i];
                    if (key.frame > frame && prev.frame <= frame)
                        break;
                    prev = key;
                }

                return prev;
            }
            public FSKAKey GetRight(int frame)
            {
                FSKAKey cur = keys[0];
                FSKAKey prev = keys[0];

                for (int i = 1; i < keys.Count; i++)
                {
                    FSKAKey key = keys[i];
                    cur = key;
                    if (key.frame > frame && prev.frame <= frame)
                        break;
                    prev = key;
                }

                return cur;
            }
        }

        public class FSKAKey
        {
            public int frame;
            public float unk1, unk2, unk3, unk4;

            public int offset;
        }
    }
    public struct DisplayVertex
    {
        // Used for rendering.
        public Vector3 pos;
        public Vector3 nrm;
        public Vector3 tan;
        public Vector3 bit;
        public Vector2 uv;
        public Vector4 col;
        public Vector4 node;
        public Vector4 weight;
        public Vector2 uv2;
        public Vector2 uv3;
        public Vector3 pos1;
        public Vector3 pos2;

        public static int Size = 4 * (3 + 3 + 3 + 3 + 2 + 4 + 4 + 4 + 2 + 2 + 3 + 3);
    }

    public class FSHPFolder : TreeNodeCustom
    {
        public FSHPFolder()
        {
            Text = "Objects";
            Name = "FshpFolder";

            ContextMenu = new ContextMenu();
            MenuItem import = new MenuItem("Add Object");
            ContextMenu.MenuItems.Add(import);
            import.Click += Import;
            MenuItem exportAll = new MenuItem("Export All Objects");
            ContextMenu.MenuItems.Add(exportAll);
            exportAll.Click += ExportAll;
            MenuItem clear = new MenuItem("Clear All Objects");
            ContextMenu.MenuItems.Add(clear);
            clear.Click += Clear;
        }
        public void Clear(object sender, EventArgs args)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all objects? This cannot be undone!", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                Nodes.Clear();
                ((FMDL)Parent).shapes.Clear();
                ((FMDL)Parent).BFRESRender.UpdateVertexData();
            }
        }
        public void ExportAll(object sender, EventArgs args)
        {
            ((FMDL)Parent).ExportAll();
        }
        public void Import(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfobj;*.fbx;*.dae; *.obj;*.csv;|" +
             "Bfres Object (shape/vertices) |*.bfobj|" +
             "FBX |*.fbx|" +
             "DAE |*.dae|" +
             "OBJ |*.obj|" +
             "CSV |*.csv|" +
             "All files(*.*)|*.*";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                    ((FMDL)Parent).AddOjects(file, false);
            }
        }


        public override void OnClick(TreeView treeView)
        {

        }
    }
    public class FMATFolder : TreeNodeCustom
    {
        public FMATFolder()
        {
            Text = "Materials";
            Name = "FmatFolder";

            ContextMenu = new ContextMenu();
            MenuItem import = new MenuItem("Add Material");
            ContextMenu.MenuItems.Add(import);
            import.Click += Import;
        }
        public void Import(object sender, EventArgs args)
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
    public class FSKL : STSkeleton
    {
        public int[] Node_Array;
        public fsklNode node;
        public class fsklNode : TreeNodeCustom
        {
            public Skeleton Skeleton;
            public ResU.Skeleton SkeletonU;

            public BFRESRender BFRESRender;
            public fsklNode()
            {
                Text = "Skeleton";
                ImageKey = "skeleton";
                SelectedImageKey = "skeleton";


                ContextMenu = new ContextMenu();
                MenuItem export = new MenuItem("Export");
                ContextMenu.MenuItems.Add(export);
                export.Click += Export;
                MenuItem replace = new MenuItem("Replace");
                ContextMenu.MenuItems.Add(replace);
                replace.Click += Replace;
            }
            public void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Bfres Skeleton|*.bfskl;";
                sfd.FileName = Text;
                sfd.DefaultExt = ".bfskl";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Skeleton.Export(sfd.FileName, BFRESRender.resFile);
                }
            }
            public void Replace(object sender, EventArgs args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Supported Formats|*.bfska;|" +
                 "Bfres Object (shape/vertices) |*.bfska|" +
                 "All files(*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Skeleton.Import(ofd.FileName);
                }
            }

            public override void OnClick(TreeView treeView)
            {

            }

        }
        public FSKL()
        {

        }
        public FSKL(Skeleton skl)
        {
            node = new fsklNode();
            node.Skeleton = skl;
            BfresSwitch.SetSkeleton(node, skl, this);
        }
        public FSKL(ResU.Skeleton skl)
        {
            node = new fsklNode();
            node.SkeletonU = skl;
            BfresWiiU.SetSkeleton(node, skl, this);
        }
    }
    public class BfresBone : STBone
    {
        public bool IsVisable = true;
        public BoneFlagsBillboard billboard;
        public BoneFlagsRotation rotationFlags;
        public BoneFlagsTransform transformFlags;

        public Bone Bone;
        public ResU.Bone BoneU;

        public BFRESRender BFRESRender;
        public BfresBone()
        {
            ImageKey = "bone";
            SelectedImageKey = "bone";


            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Replace;
        }
        public void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Bfres Bone|*.bfbn;";
            sfd.FileName = Text;
            sfd.DefaultExt = ".bfbn";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Bone.Export(sfd.FileName, BFRESRender.resFile);
            }
        }
        public void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfska;|" +
             "Bfres Object (shape/vertices) |*.bfska|" +
             "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Bone.Import(ofd.FileName);
            }
            Bone.Name = Text;
        }

        public BfresBone(STSkeleton skeleton)
        {
            skeletonParent = skeleton;
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadBoneEditor(this);
        }
    }
    public class FMDL : STGenericModel
    {
        public List<FSHP> shapes = new List<FSHP>();
        public Dictionary<string, FMAT> materials = new Dictionary<string, FMAT>();
        public BFRESRender BFRESRender;
        public Model Model;
        public ResU.Model ModelU;

        public FMDL()
        {
            ImageKey = "model";
            SelectedImageKey = "model";

            Nodes.Add(new FSHPFolder());
            Nodes.Add(new FMATFolder());

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export Model");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace Model");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Replace;
            MenuItem calcTansBitans = new MenuItem("Calculate Tangents/Bitangents");
            ContextMenu.MenuItems.Add(calcTansBitans);
            calcTansBitans.Click += CalcTansBitansAllShapes;
            MenuItem normals = new MenuItem("Normals");
            ContextMenu.MenuItems.Add(normals);
            MenuItem smoothNormals = new MenuItem("Smooth");
            normals.MenuItems.Add(smoothNormals);
            smoothNormals.Click += SmoothNormals;
            MenuItem recalculateNormals = new MenuItem("Recalculate");
            normals.MenuItems.Add(recalculateNormals);
            recalculateNormals.Click += RecalculateNormals;

            MenuItem rename = new MenuItem("Rename");
            ContextMenu.MenuItems.Add(rename);
            rename.Click += Rename;
        }
        private void SmoothNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (FSHP shp in shapes)
            {
                bool HasNormals = shp.vertexAttributes.Any(x => x.Name == "_n0");
                if (HasNormals)
                    shp.SmoothNormals();

                shp.SaveVertexBuffer();
            }
            BFRESRender.UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        private void RecalculateNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (FSHP shp in shapes)
            {
                bool HasNormals = shp.vertexAttributes.Any(x => x.Name == "_n0");
                if (HasNormals)
                    shp.CalculateNormals();

                shp.SaveVertexBuffer();
            }
            BFRESRender.UpdateVertexData();
            Cursor.Current = Cursors.Default;
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
        private void CalcTansBitansAllShapes(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (FSHP shp in shapes)
            {
                bool HasTans = shp.vertexAttributes.Any(x => x.Name == "_t0");
                bool HasBiTans = shp.vertexAttributes.Any(x => x.Name == "_b0");

                if (!shp.HasUV0())
                {
                    MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!HasBiTans)
                {
                    DialogResult dialogResult2 = MessageBox.Show("Mesh does not have bitangents. Do you want to create them? (will make file size bigger)", "", MessageBoxButtons.YesNo);

                    FSHP.VertexAttribute att2 = new FSHP.VertexAttribute();
                    att2.Name = "_b0";
                    att2.Format = ResGFX.AttribFormat.Format_10_10_10_2_SNorm;

                    if (dialogResult2 == DialogResult.Yes)
                    {
                        if (!HasBiTans)
                            shp.vertexAttributes.Add(att2);
                    }
                }

                if (!HasTans)
                {
                    DialogResult dialogResult = MessageBox.Show("Mesh does not have tangets. Do you want to create them? (will make file size bigger)", "", MessageBoxButtons.YesNo);

                    FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                    att.Name = "_t0";
                    att.Format = ResGFX.AttribFormat.Format_10_10_10_2_SNorm;


                    if (dialogResult == DialogResult.Yes)
                    {
                        if (!HasTans)
                            shp.vertexAttributes.Add(att);
                    }
                }

                shp.CalculateTangentBitangent();
                shp.SaveVertexBuffer();
            }

            BFRESRender.UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        public void CopyMaterial(FMAT selectedMaterial)
        {
            CopyMaterialMenu menu = new CopyMaterialMenu();
            menu.LoadMaterials(selectedMaterial.Text, BFRESRender.models);
            if (menu.ShowDialog() == DialogResult.OK)
            {
                foreach (TreeNode mdl in menu.materialTreeView.Nodes)
                {
                    foreach (TreeNode n in mdl.Nodes)
                    {
                        if (n.Checked)
                        {
                            if (materials.ContainsKey(n.Text))
                                SetCopiedMaterialData(menu, selectedMaterial, materials[n.Text]);
                        }
                    }
                }
                Viewport.Instance.UpdateViewport();
            }
        }
        private void SetCopiedMaterialData(CopyMaterialMenu menu,
            FMAT selectedMaterial, FMAT targetMaterial)
        {
            targetMaterial.Material.Flags = selectedMaterial.Material.Flags;
            targetMaterial.Material.UserDatas = selectedMaterial.Material.UserDatas;
            targetMaterial.Material.UserDataDict = selectedMaterial.Material.UserDataDict;

            if (menu.chkBoxRenderInfo.Checked)
            {
                targetMaterial.Material.RenderInfoDict = selectedMaterial.Material.RenderInfoDict;
                targetMaterial.Material.RenderInfos = selectedMaterial.Material.RenderInfos;
            }
            if (menu.chkBoxShaderOptions.Checked)
            {
                targetMaterial.Material.ShaderAssign = selectedMaterial.Material.ShaderAssign;
            }
            if (menu.chkBoxShaderParams.Checked)
            {
                targetMaterial.Material.ShaderParamData = selectedMaterial.Material.ShaderParamData;
                targetMaterial.Material.ShaderParamDict = selectedMaterial.Material.ShaderParamDict;
                targetMaterial.Material.ShaderParams = selectedMaterial.Material.ShaderParams;
                targetMaterial.Material.VolatileFlags = selectedMaterial.Material.VolatileFlags;
            }
            if (menu.chkBoxTextures.Checked)
            {
                targetMaterial.Material.SamplerDict = selectedMaterial.Material.SamplerDict;
                targetMaterial.Material.Samplers = selectedMaterial.Material.Samplers;
                targetMaterial.Material.SamplerSlotArray = selectedMaterial.Material.SamplerSlotArray;
                targetMaterial.Material.TextureSlotArray = selectedMaterial.Material.TextureSlotArray;
                targetMaterial.Material.TextureRefs = selectedMaterial.Material.TextureRefs;
            }
            targetMaterial.ReadMaterial(targetMaterial.Material);
        }
        public void ExportAll()
        {
            FolderSelectDialog sfd = new FolderSelectDialog();

            List<string> Formats = new List<string>();
            Formats.Add("Bfres object (.bfobj)");
            Formats.Add("CSV (.csv)");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = sfd.SelectedPath;

                TextureFormatExport form = new TextureFormatExport(Formats);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    foreach (FSHP shp in shapes)
                    {
                        if (form.Index == 0)
                            shp.ExportBinaryObject(folderPath + '\\' + shp.Text + ".bfobj");
                    }
                }
            }
        }
        public void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.bfmdl;*.fbx;*.dae; *.obj;*.csv;|" +
             "Bfres Model|*.bfmdl|" +
             "FBX |*.fbx|" +
             "DAE |*.dae|" +
             "OBJ |*.obj|" +
             "CSV |*.csv|" +
             "All files(*.*)|*.*";
            sfd.DefaultExt = ".bfobj";
            sfd.FileName = Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                ext = ext.ToLower();

                switch (ext)
                {
                    case ".bfmdl":
                        Model.Export(sfd.FileName, BFRESRender.resFile);
                        break;
                    case ".csv":
                        CsvModel csv = new CsvModel();
                        foreach (FSHP shape in shapes)
                        {
                            STGenericObject obj = new STGenericObject();
                            obj.ObjectName = shape.Text;
                            obj.vertices = shape.vertices;
                            obj.faces = shape.lodMeshes[shape.DisplayLODIndex].faces;
                            csv.objects.Add(obj);

                            int CurVtx = 0;
                            foreach (Vertex v in shape.vertices)
                            {
                                if (v.boneIds[0] != 0)
                                    obj.vertices[CurVtx].boneNames.Add(shape.GetBoneNameFromIndex(this, v.boneIds[0]));
                                if (v.boneIds[1] != 0)
                                    obj.vertices[CurVtx].boneNames.Add(shape.GetBoneNameFromIndex(this, v.boneIds[1]));
                                if (v.boneIds[2] != 0)
                                    obj.vertices[CurVtx].boneNames.Add(shape.GetBoneNameFromIndex(this, v.boneIds[2]));
                                if (v.boneIds[3] != 0)
                                    obj.vertices[CurVtx].boneNames.Add(shape.GetBoneNameFromIndex(this, v.boneIds[3]));

                                CurVtx++;
                            }
                        }
                        System.IO.File.WriteAllBytes(sfd.FileName, csv.Save());
                        break;
                    default:
                        AssimpData assimp = new AssimpData();
                        assimp.SaveFromModel(this, sfd.FileName);
                        break;
                }
            }
        }

        public void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfobj;*.fbx;*.dae;*.obj;*.csv;|" +
             "Bfres Object (shape/vertices) |*.bfobj|" +
             "FBX |*.fbx|" +
             "DAE |*.dae|" +
             "OBJ |*.obj|" +
             "CSV |*.csv|" +
             "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AddOjects(ofd.FileName);
            }

        }
        //Function addes shapes, vertices and meshes
        public void AddOjects(string FileName, bool Replace = true)
        {
            if (Replace)
            {
                shapes.Clear();

                Nodes["FshpFolder"].Nodes.Clear();
            }

            int MatStartIndex = materials.Count;
            string ext = System.IO.Path.GetExtension(FileName);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".bfobj":
                    Cursor.Current = Cursors.WaitCursor;

                    Shape shpS = new Shape();
                    VertexBuffer vertexBuffer = new VertexBuffer();
                    shpS.Import(FileName, vertexBuffer);

                    FSHP shapeS = new FSHP();
                    shapeS.Shape = shpS;
                    shapeS.BFRESRender = BFRESRender;
                    BfresSwitch.ReadShapesVertices(shapeS, shpS, vertexBuffer, this);
                    shapes.Add(shapeS);
                    Nodes["FshpFolder"].Nodes.Add(shapeS);
                    Cursor.Current = Cursors.Default;
                    break;
                case ".bfmdl":
                    Cursor.Current = Cursors.WaitCursor;
                    shapes.Clear();
                    Model mdl = new Model();
                    mdl.Import(FileName, BFRESRender.resFile);
                    mdl.Name = Text;
                    shapes.Clear();
                    Nodes["FshpFolder"].Nodes.Clear();
                    foreach (Shape shp in mdl.Shapes)
                    {
                        FSHP shape = new FSHP();
                        shape.Shape = shp;
                        BfresSwitch.ReadShapesVertices(shape, shp, mdl.VertexBuffers[shp.VertexBufferIndex], this);
                        shapes.Add(shape);
                        Nodes["FshpFolder"].Nodes.Add(shape);
                    }
                    Cursor.Current = Cursors.Default;
                    break;
                case ".csv":
                    CsvModel csvModel = new CsvModel();
                    csvModel.LoadFile(FileName, true);

                    if (csvModel.objects.Count == 0)
                    {
                        MessageBox.Show("No models found!");
                        return;
                    }
                    BfresModelImportSettings csvsettings = new BfresModelImportSettings();
                    csvsettings.DisableMaterialEdits();
                    csvsettings.SetModelAttributes(csvModel.objects[0]);
                    if (csvsettings.ShowDialog() == DialogResult.OK)
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        foreach (STGenericObject obj in csvModel.objects)
                        {
                            FSHP shape = new FSHP();
                            shape.VertexBufferIndex = shapes.Count;
                            shape.vertices = obj.vertices;
                            shape.MaterialIndex = 0;
                            shape.VertexSkinCount = obj.GetMaxSkinInfluenceCount();
                            shape.vertexAttributes = csvsettings.CreateNewAttributes();
                            shape.boneIndx = 0;
                            shape.Text = obj.ObjectName;
                            shape.lodMeshes = obj.lodMeshes;
                            shape.CreateNewBoundingBoxes();
                            shape.CreateBoneList(obj, this);
                            shape.CreateIndexList(obj, this);
                            shape.ApplyImportSettings(csvsettings, GetMaterial(shape.MaterialIndex));
                            shape.SaveShape();
                            shape.SaveVertexBuffer();
                            shape.BFRESRender = BFRESRender;
                            shape.BoneIndices = new List<ushort>();

                            Nodes["FshpFolder"].Nodes.Add(shape);
                            shapes.Add(shape);
                        }
                        Cursor.Current = Cursors.Default;
                    }
                    break;
                default:
                    AssimpData assimp = new AssimpData();
                    assimp.LoadFile(FileName);

                    if (assimp.objects.Count == 0)
                    {
                        MessageBox.Show("No models found!");
                        return;
                    }
                    BfresModelImportSettings settings = new BfresModelImportSettings();

                    if (BFRES.IsWiiU)
                        settings.DisableMaterialEdits();

                    settings.SetModelAttributes(assimp.objects[0]);
                    if (settings.ShowDialog() == DialogResult.OK)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        if (!BFRES.IsWiiU && Replace)
                        {
                            materials.Clear();
                            Nodes["FmatFolder"].Nodes.Clear();
                            MatStartIndex = 0;
                        }

                        if (!BFRES.IsWiiU)
                        {
                            foreach (STGenericMaterial mat in assimp.materials)
                            {
                                FMAT fmat = new FMAT();
                                fmat.Material = new Material();
                                if (settings.ExternalMaterialPath != string.Empty)
                                {
                                    fmat.Material.Import(settings.ExternalMaterialPath);
                                    fmat.ReadMaterial(fmat.Material);
                                }

                                fmat.Text = mat.Text;
                                //Setup placeholder textures
                                //Note we can't add/remove samplers so we must fill these slots
                                foreach (var t in fmat.textures)
                                {
                                    t.wrapModeS = 0;
                                    t.wrapModeT = 0;

                                    switch (t.Type)
                                    {
                                        case STGenericMatTexture.TextureType.Diffuse:
                                            t.Name = "Basic_Alb";
                                            break;
                                        case STGenericMatTexture.TextureType.Emission:
                                            t.Name = "Basic_Emm";
                                            break;
                                        case STGenericMatTexture.TextureType.Normal:
                                            t.Name = "Basic_Nrm";
                                            break;
                                        case STGenericMatTexture.TextureType.Specular:
                                            t.Name = "Basic_Spm";
                                            break;
                                        case STGenericMatTexture.TextureType.SphereMap:
                                            t.Name = "Basic_Sphere";
                                            break;
                                        case STGenericMatTexture.TextureType.Metalness:
                                            t.Name = "Basic_Mtl";
                                            break;
                                        case STGenericMatTexture.TextureType.Roughness:
                                            t.Name = "Basic_Rgh";
                                            break;
                                        case STGenericMatTexture.TextureType.MRA:
                                            t.Name = "Basic_MRA";
                                            break;
                                        case STGenericMatTexture.TextureType.Shadow:
                                            t.Name = "Basic_Bake_st0";
                                            break;
                                        case STGenericMatTexture.TextureType.Light:
                                            t.Name = "Basic_Bake_st1";
                                            break;
                                    }
                                }

                                if (PluginRuntime.bntxContainers.Count > 0)
                                {
                                    foreach (var node in Parent.Parent.Nodes["EXT"].Nodes)
                                    {
                                        if (node is BinaryTextureContainer)
                                        {
                                            var bntx = (BinaryTextureContainer)node;

                                            bntx.ImportBasicTextures("Basic_Alb");
                                            bntx.ImportBasicTextures("Basic_Nrm");
                                            bntx.ImportBasicTextures("Basic_Spm");
                                            bntx.ImportBasicTextures("Basic_Sphere");
                                            bntx.ImportBasicTextures("Basic_Mtl");
                                            bntx.ImportBasicTextures("Basic_Rgh");
                                            bntx.ImportBasicTextures("Basic_MRA");
                                            bntx.ImportBasicTextures("Basic_Bake_st0");
                                            bntx.ImportBasicTextures("Basic_Bake_st1");
                                            bntx.ImportBasicTextures("Basic_Emm");
                                        }
                                    }
                                }

                                foreach (var tex in mat.TextureMaps)
                                {
                                    foreach (var t in fmat.textures)
                                    {
                                        if (t.Type == tex.Type)
                                        {
                                            t.Name = tex.Name;
                                            t.wrapModeS = tex.wrapModeS;
                                            t.wrapModeT = tex.wrapModeT;
                                            t.wrapModeW = tex.wrapModeW;
                                            t.Type = tex.Type;
                                        }
                                    }
                                }
                                fmat.Material.Name = Text;
                                fmat.SetMaterial(fmat.Material);

                                List<string> keyList = new List<string>(materials.Keys);
                                fmat.Text = Utils.RenameDuplicateString(keyList, fmat.Text);

                                materials.Add(fmat.Text, fmat);
                                Nodes["FmatFolder"].Nodes.Add(fmat);
                            }
                        }
                 
                        foreach (STGenericObject obj in assimp.objects)
                        {
                            FSHP shape = new FSHP();
                            shape.VertexBufferIndex = shapes.Count;
                            shape.vertices = obj.vertices;
                            shape.VertexSkinCount = obj.MaxSkinInfluenceCount;
                            shape.vertexAttributes = settings.CreateNewAttributes();
                            shape.boneIndx = obj.BoneIndex;
                            shape.MaterialIndex = obj.MaterialIndex + MatStartIndex;

                            if (BFRES.IsWiiU)
                                shape.MaterialIndex = 0;

                            shape.Text = obj.ObjectName;
                            shape.lodMeshes = obj.lodMeshes;
                            shape.CreateNewBoundingBoxes();
                            shape.CreateBoneList(obj, this);
                            shape.CreateIndexList(obj, this);
                            shape.ApplyImportSettings(settings, GetMaterial(shape.MaterialIndex));
                            shape.SaveShape();
                            shape.SaveVertexBuffer();
                            shape.BFRESRender = BFRESRender;
                            shape.BoneIndices = new List<ushort>();

                            List<string> keyList = shapes.Select(o => o.Text).ToList();
                            shape.Text = Utils.RenameDuplicateString(keyList, shape.Text);

                            Nodes["FshpFolder"].Nodes.Add(shape);
                            shapes.Add(shape);
                        }
                        Cursor.Current = Cursors.Default;
                    }
                    break;
            }
            BFRESRender.UpdateVertexData();
        }
        public FMAT GetMaterial(int index)
        {
            return materials.Values.ElementAt(index);
        }
        public void AddMaterials(string FileName, bool Replace = true)
        {
            string ext = System.IO.Path.GetExtension(FileName);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".bfmat":
                    Cursor.Current = Cursors.WaitCursor;
                    if (Replace)
                    {
                        materials.Clear();
                        Nodes["FmatFolder"].Nodes.Clear();
                    }
                    FMAT mat = new FMAT();
                    mat.Material = new Material();
                    mat.Material.Import(FileName);
                    mat.ReadMaterial(mat.Material);
                    mat.BFRESRender = BFRESRender;
                    mat.Text = mat.Material.Name;

                    materials.Add(mat.Text, mat);
                    Nodes["FmatFolder"].Nodes.Add(mat);
                    break;
            }
        }
        public override void OnClick(TreeView treeView)
        {

        }
        private void CreateSkeleton()
        {

        }
        private void CreateBones(STBone bone)
        {
            Bone bn = new Bone();
            bn.BillboardIndex = (ushort)bone.BillboardIndex;
            bn.Flags = BoneFlags.Visible;
            bn.FlagsRotation = BoneFlagsRotation.EulerXYZ;
            bn.FlagsTransform = BoneFlagsTransform.None;
            bn.FlagsTransformCumulative = BoneFlagsTransformCumulative.None;
            bn.Name = bone.Text;
            bn.RigidMatrixIndex = 0;
            bn.Rotation = new Syroot.Maths.Vector4F(bone.rotation[0],
                bone.rotation[1], bone.rotation[2], bone.rotation[3]);
            bn.Position = new Syroot.Maths.Vector3F(bone.position[0],
                bone.position[1], bone.position[2]);
            bn.Scale = new Syroot.Maths.Vector3F(bone.scale[0],
               bone.scale[1], bone.scale[2]);
            bn.UserData = new List<UserData>();
            bn.UserDataDict = new ResDict();
        }

        public FSKL Skeleton
        {
            get
            {
                return skeleton;
            }
            set
            {
                skeleton = value;
            }
        }
        private FSKL skeleton = new FSKL();
    }
    public class FMAT : STGenericMaterial
    {
        public FMAT()
        {
            Checked = true;
            ImageKey = "material";
            SelectedImageKey = "material";

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Replace;
            MenuItem copy = new MenuItem("Copy");
            ContextMenu.MenuItems.Add(copy);
            copy.Click += Copy;
            MenuItem rename = new MenuItem("Rename");
            ContextMenu.MenuItems.Add(rename);
            rename.Click += Rename;
        }

        public BFRESRender BFRESRender;
        public bool Enabled = true;

        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadMatEditor(this);
        }
        public bool EditorIsActive(DockContent dock)
        {
            foreach (Control ctrl in dock.Controls)
            {
                if (ctrl is FMATEditor)
                {
                    ((FMATEditor)ctrl).LoadMaterial(this, BFRESRender);
                    return true;
                }
            }
            return false;
        }

        public void SetActiveGame()
        {
            string ShaderName = shaderassign.ShaderArchive;
            string ShaderModel = shaderassign.ShaderModel;

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
        private void Rename(object sender, EventArgs args)
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
        private void Copy(object sender, EventArgs args)
        {
            ((FMDL)Parent.Parent).CopyMaterial(this);
        }
        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.bfmat;";

            sfd.DefaultExt = ".bfmat";
            sfd.FileName = Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Material.Export(sfd.FileName, BFRESRender.resFile);
            }
        }
        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfmat;";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Material.Import(ofd.FileName);
                Material.Name = Text;

                BfresSwitch.ReadMaterial(this, Material);
            }
        }

        public Dictionary<string, float[]> anims = new Dictionary<string, float[]>();
        public Dictionary<string, int> Samplers = new Dictionary<string, int>();
        public List<MatTexture> textures = new List<MatTexture>();
        public List<BfresRenderInfo> renderinfo = new List<BfresRenderInfo>();
        public List<SamplerInfo> samplerinfo = new List<SamplerInfo>();
        public Dictionary<string, BfresShaderParam> matparam = new Dictionary<string, BfresShaderParam>();

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
        public class SamplerInfo
        {
            public int WrapModeU;
            public int WrapModeV;
            public int WrapModeW;
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
        public ShaderParamType Type;
        public string Name;

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
    public class MatTexture : STGenericMatTexture
    {
        public int hash;
        public string SamplerName;
        //Note samplers will get converted to another sampler type sometimes in the shader assign section
        //Use this string if not empty for our bfres fragment shader to produce the accurate affects
        //An example of a conversion maybe be like a1 - t0 so texture gets used as a transparent map/alpha texture
        public string FragShaderSampler = "";

        public MatTexture()
        {

        }
    }
    public class FSHP : STGenericObject
    {
        public FSHP()
        {
            Checked = true;
            ImageKey = "mesh";
            SelectedImageKey = "mesh";

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export Mesh");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace Mesh");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Replace;
            MenuItem remove = new MenuItem("Delete Mesh");
            ContextMenu.MenuItems.Add(remove);
            remove.Click += Remove;
            MenuItem calcTansBitans = new MenuItem("Recalulate Tangents/Bitangents");
            ContextMenu.MenuItems.Add(calcTansBitans);
            calcTansBitans.Click += CalcTansBitans;
            MenuItem flipUVsY = new MenuItem("Flip UVs (Vertical)");
            ContextMenu.MenuItems.Add(flipUVsY);
            flipUVsY.Click += FlipUvsVertical;
            MenuItem flipUVsX = new MenuItem("Flip UVs (Horizontal)");
            ContextMenu.MenuItems.Add(flipUVsX);
            flipUVsX.Click += FlipUvsHorizontal;
            MenuItem normals = new MenuItem("Normals");
            ContextMenu.MenuItems.Add(normals);
            MenuItem smoothNormals = new MenuItem("Smooth");
            normals.MenuItems.Add(smoothNormals);
            smoothNormals.Click += SmoothNormals;
            MenuItem recalculateNormals = new MenuItem("Recalculate");
            normals.MenuItems.Add(recalculateNormals);
            recalculateNormals.Click += RecalculateNormals;

            MenuItem matEditor = new MenuItem("Open Material Editor");
            ContextMenu.MenuItems.Add(matEditor);
            matEditor.Click += OpenMaterialEditor;

            MenuItem rename = new MenuItem("Rename");
            ContextMenu.MenuItems.Add(rename);
            rename.Click += Rename;
        }

        public BFRESRender BFRESRender;
        public FMATEditor editor;
        public int ModelIndex; //For getting the model the shape is in

        public VertexBuffer VertexBuffer;
        public Shape Shape;
        public ResU.VertexBuffer VertexBufferU;
        public ResU.Shape ShapeU;

        public FMAT GetMaterial()
        {
            return ((FMDL)Parent.Parent).materials.Values.ElementAt(MaterialIndex);
        }
        public void SetMaterial(FMAT material)
        {
            ((FMDL)Parent.Parent).materials[material.Text] = material;
        }

        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadShapeEditor(this);
        }
        private void SmoothNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            SmoothNormals();
            SaveVertexBuffer();
            BFRESRender.UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        private void RecalculateNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            CalculateNormals();
            SaveVertexBuffer();
            BFRESRender.UpdateVertexData();
            Cursor.Current = Cursors.Default;
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
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove this object? This cannot be undone!", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                ((FMDL)Parent.Parent).shapes.Remove(this);
                ((FMDL)Parent.Parent).BFRESRender.UpdateVertexData();
                Parent.Nodes.Remove(this);
            }
        }
        public void ApplyImportSettings(BfresModelImportSettings settings, FMAT mat)
        {
            if (settings.FlipUVsVertical)
            {
                foreach (Vertex v in vertices)
                {
                    v.uv0 = new Vector2(v.uv0.X, 1 - v.uv0.Y);
                }
            }
            if (settings.RecalculateNormals)
            {
                CalculateNormals();
            }
            if (settings.Rotate90DegreesY)
            {
                TransformPosition(Vector3.Zero, new Vector3(90, 0, 0), new Vector3(1));
            }
            if (settings.Rotate90DegreesNegativeY)
            {
                TransformPosition(Vector3.Zero, new Vector3(-90, 0, 0), new Vector3(1));
            }
            if (settings.EnableTangents)
            {
                try
                {
                    CalculateTangentBitangent();
                }
                catch
                {
                    MessageBox.Show($"Failed to generate tangents for mesh {Text}");
                }
            }
            if (settings.SetDefaultParamData)
            {
                foreach (var param in mat.matparam.Values)
                {
                    switch (param.Name)
                    {
                        case "const_color0":
                        case "const_color1":
                        case "const_color2":
                        case "const_color3":
                        case "base_color_mul_color":
                        case "uniform0_mul_color":
                        case "uniform1_mul_color":
                        case "uniform2_mul_color":
                        case "uniform3_mul_color":
                        case "uniform4_mul_color":
                        case "proc_texture_2d_mul_color":
                        case "proc_texture_3d_mul_color":
                        case "displacement1_color":
                        case "ripple_emission_color":
                        case "hack_color":
                        case "stain_color":
                        case "displacement_color":
                            param.ValueFloat = new float[] {1,1,1,1 };
                            break;
                        case "gsys_bake_st0":
                            param.ValueFloat = new float[] { 1, 1, 0, 0 };
                            break;
                        case "gsys_bake_st1":
                            param.ValueFloat = new float[] { 1, 1, 0, 0 };
                            break;
                    }
                }
            }
        }
        private void OpenMaterialEditor(object sender, EventArgs args)
        {
            FormLoader.LoadMatEditor(GetMaterial());
        }
        private void CalcTansBitans(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;

            bool HasTans = vertexAttributes.Any(x => x.Name == "_t0");
            bool HasBiTans = vertexAttributes.Any(x => x.Name == "_b0");

            if (!HasUV0())
            {
                MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!HasBiTans)
            {
                DialogResult dialogResult2 = MessageBox.Show("Mesh does not have bitangents. Do you want to create them? (will make file size bigger)", "", MessageBoxButtons.YesNo);

                VertexAttribute att2 = new VertexAttribute();
                att2.Name = "_b0";
                att2.Format = ResGFX.AttribFormat.Format_10_10_10_2_SNorm;

                if (dialogResult2 == DialogResult.Yes)
                {
                    if (!HasBiTans)
                        vertexAttributes.Add(att2);
                }
            }

            if (!HasTans)
            {
                DialogResult dialogResult = MessageBox.Show("Mesh does not have tangets. Do you want to create them? (will make file size bigger)", "", MessageBoxButtons.YesNo);

                VertexAttribute att = new VertexAttribute();
                att.Name = "_t0";
                att.Format = ResGFX.AttribFormat.Format_10_10_10_2_SNorm;


                if (dialogResult == DialogResult.Yes)
                {
                    if (!HasTans)
                        vertexAttributes.Add(att);
                }
            }

            CalculateTangentBitangent();
            SaveVertexBuffer();
            BFRESRender.UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        public bool HasAttributeUV0()
        {
            return vertexAttributes.Any(x => x.Name == "_u0");
        }
        public bool HasAttributeUV1()
        {
            return vertexAttributes.Any(x => x.Name == "_u1");
        }
        public bool HasAttributeUV2()
        {
            return vertexAttributes.Any(x => x.Name == "_u2");
        }
        public void FlipUvsVertical(object sender, EventArgs args)
        {
            if (!HasUV0())
            {
                MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FlipUvsVertical();
            SaveVertexBuffer();
            BFRESRender.UpdateVertexData();
        }
        public void FlipUvsHorizontal(object sender, EventArgs args)
        {
            if (!HasUV0())
            {
                MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FlipUvsHorizontal();
            SaveVertexBuffer();
            BFRESRender.UpdateVertexData();
        }
        public void ExportMaterials(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Materials|*.bfmat;";
            sfd.DefaultExt = ".bfmat";
            sfd.FileName = GetMaterial().Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                GetMaterial().Material.Export(sfd.FileName, BFRESRender.resFile);
            }
        }
        public void ReplaceMaterials(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Materials|*.bfmat;";
            ofd.DefaultExt = ".bfmat";
            ofd.FileName = GetMaterial().Text;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                GetMaterial().Material.Import(ofd.FileName);
            }
        }
        public void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.bfobj;*.fbx;*.dae; *.obj;|" +
             "Bfres Object (shape/vertices) |*.bfobj|" +
             "FBX |*.fbx|" +
             "DAE |*.dae|" +
             "OBJ |*.obj|" +
             "All files(*.*)|*.*";
            sfd.DefaultExt = ".bfobj";
            sfd.FileName = Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                ext = ext.ToLower();

                switch (ext)
                {
                    case ".bfobj":
                        ExportBinaryObject(sfd.FileName);
                        break;
                    default:
                        AssimpData assimp = new AssimpData();
                        assimp.SaveFromObject(vertices, lodMeshes[DisplayLODIndex].faces, Text, sfd.FileName);
                        break;
                }
            }
        }
        public void ExportBinaryObject(string FileName)
        {
            Shape.Export(FileName, BFRESRender.resFile);
        }
        public void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfobj;*.fbx;*.dae; *.obj;|" +
             "Bfres Object (shape/vertices) |*.bfobj|" +
             "FBX |*.fbx|" +
             "DAE |*.dae|" +
             "OBJ |*.obj|" +
             "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(ofd.FileName);
                ext = ext.ToLower();

                switch (ext)
                {
                    case ".bfobj":
                        Shape shp = new Shape();
                        shp.Import(ofd.FileName, VertexBuffer);
                        shp.Name = Text;
                        shp.MaterialIndex = (ushort)MaterialIndex;
                        BfresSwitch.ReadShapesVertices(this, shp, VertexBuffer, BFRESRender.models[ModelIndex]);
                        break;
                    default:
                        AssimpData assimp = new AssimpData();
                        assimp.LoadFile(ofd.FileName);
                        AssimpMeshSelector selector = new AssimpMeshSelector();
                        selector.LoadMeshes(assimp, Index);

                        if (selector.ShowDialog() == DialogResult.OK)
                        {
                            if (assimp.objects.Count == 0)
                            {
                                MessageBox.Show("No models found!");
                                return;
                            }
                            BfresModelImportSettings settings = new BfresModelImportSettings();
                            settings.SetModelAttributes(assimp.objects[0]);
                            if (settings.ShowDialog() == DialogResult.OK)
                            {
                                GenericObject obj = selector.GetSelectedMesh();

                                Cursor.Current = Cursors.WaitCursor;
                                VertexBufferIndex = obj.VertexBufferIndex;
                                vertices = obj.vertices;
                                CreateBoneList(obj, (FMDL)Parent.Parent);
                                VertexSkinCount = obj.MaxSkinInfluenceCount;
                                vertexAttributes = settings.CreateNewAttributes();
                                lodMeshes = obj.lodMeshes;
                                CreateNewBoundingBoxes();
                                SaveShape();
                                SaveVertexBuffer();
                                Cursor.Current = Cursors.Default;
                            }
                        }
                        break;
                }
                BFRESRender.UpdateVertexData();
            }
        }
        public void CreateIndexList(GenericObject ob, FMDL mdl = null)
        {
            BoneIndices = new List<ushort>();

            List<string> boneNames = new List<string>();
            foreach (Vertex v in ob.vertices)
            {
                foreach (string bn in v.boneNames)
                {
                    if (!boneNames.Contains(bn))
                        boneNames.Add(bn);
                }
            }

            int index = 0;
            foreach (STBone bone in mdl.Skeleton.bones)
            {
                foreach (string bnam in boneNames)
                {
                    if (bone.Text == bnam)
                    {
                        BoneIndices.Add((ushort)index);
                    }
                }
                index++;
            }
        }
        public void CreateBoneList(GenericObject ob, FMDL mdl)
        {
            string[] nodeArrStrings = new string[mdl.Skeleton.Node_Array.Length];

            int CurNode = 0;
            foreach (int thing in mdl.Skeleton.Node_Array)
                nodeArrStrings[CurNode++] = mdl.Skeleton.bones[thing].Text;


            foreach (Vertex v in ob.vertices)
            {
                foreach (string bn in v.boneNames)
                {
                    foreach (var defBn in nodeArrStrings.Select((Value, Index) => new { Value, Index }))
                    {
                        if (bn == defBn.Value)
                        {
                            v.boneIds.Add(defBn.Index);
                        }
                    }
                }
            }
        }
        public void CreateNewBoundingBoxes()
        {
            boundingBoxes.Clear();
            boundingRadius.Clear();
            foreach (LOD_Mesh mesh in lodMeshes)
            {
                BoundingBox box = CalculateBoundingBox();
                boundingBoxes.Add(box);
                boundingRadius.Add((float)(box.Center.Length + box.Extend.Length));
                foreach (LOD_Mesh.SubMesh sub in mesh.subMeshes)
                    boundingBoxes.Add(box);
            }
        }
        private BoundingBox CalculateBoundingBox()
        {
            Vector3 Max = new Vector3();
            Vector3 Min = new Vector3();

            Min = Max = vertices[0].pos;

            Min = CalculateBBMin(vertices);
            Max = CalculateBBMax(vertices);
            Vector3 center = (Max + Min);
            Vector3 extend = Max - Min;

            return new BoundingBox() { Center = center, Extend = extend };
        }
        private Vector3 CalculateBBMin(List<Vertex> positionVectors)
        {
            Vector3 minimum = new Vector3();
            foreach (Vertex vtx in positionVectors)
            {
                if (vtx.pos.X < minimum.X) minimum.X = vtx.pos.X;
                if (vtx.pos.Y < minimum.Y) minimum.Y = vtx.pos.Y;
                if (vtx.pos.Z < minimum.Z) minimum.Z = vtx.pos.Z;
            }

            return minimum;
        }

        private Vector3 CalculateBBMax(List<Vertex> positionVectors)
        {
            Vector3 maximum = new Vector3();
            foreach (Vertex vtx in positionVectors)
            {
                if (vtx.pos.X > maximum.X) maximum.X = vtx.pos.X;
                if (vtx.pos.Y > maximum.Y) maximum.Y = vtx.pos.Y;
                if (vtx.pos.Z > maximum.Z) maximum.Z = vtx.pos.Z;
            }

            return maximum;
        }

        private void UpdateShaderAssignAttributes(FMAT material)
        {
            material.shaderassign.samplers.Clear();
            foreach (VertexAttribute att in vertexAttributes)
            {
                material.shaderassign.attributes.Add(att.Name, att.Name);
            }
        }

        public int[] Faces;
        public List<ushort> BoneIndices = new List<ushort>();

        // for drawing
        public int[] display;
        public int VertexSkinCount;
        public int DisplayId;
        public int boneIndx;
        public int VertexBufferIndex;
        public int TargetAttribCount;

        public List<float> boundingRadius = new List<float>();
        public List<BoundingBox> boundingBoxes = new List<BoundingBox>();
        public class BoundingBox
        {
            public Vector3 Center;
            public Vector3 Extend;
        }
        public int DisplayLODIndex = 0;

        public List<VertexAttribute> vertexAttributes = new List<VertexAttribute>();
        public class VertexAttribute
        {
            public string Name;
            public ResGFX.AttribFormat Format;

            public override string ToString()
            {
                return Name;
            }

            public ResGFX.AttribFormat GetTypeWiiU(ResUGX2.GX2AttribFormat type)
            {
                return (ResGFX.AttribFormat)System.Enum.Parse(typeof(ResGFX.AttribFormat), $"{type.ToString()}");
            }
            public ResUGX2.GX2AttribFormat SetTypeWiiU(ResGFX.AttribFormat type)
            {
                return (ResUGX2.GX2AttribFormat)System.Enum.Parse(typeof(ResUGX2.GX2AttribFormat), type.ToString());
            }
        }
        public void SaveShape()
        {
            if (!BFRES.IsWiiU)
                Shape = BfresSwitch.SaveShape(this);
            else
                ShapeU = BfresWiiU.SaveShape(this);
        }
        public IList<ushort> GetIndices()
        {
            IList<ushort> indices = new List<ushort>();

            List<string> BoneNodes = new List<string>();
            foreach (Vertex vtx in vertices)
            {

            }
            return indices;
        }
        public void SaveVertexBuffer()
        {
            if (BFRES.IsWiiU)
            {
                BfresWiiU.SaveVertexBuffer(this);
                return;
            }

            VertexBufferHelper helpernx = new VertexBufferHelper(new VertexBuffer(), Syroot.BinaryData.ByteOrder.LittleEndian);
            List<VertexBufferHelperAttrib> atrib = new List<VertexBufferHelperAttrib>();
            UpdateVertices();

            foreach (VertexAttribute att in vertexAttributes)
            {
                if (att.Name == "_p0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = verts.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_n0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = norms.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_u0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = uv0.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_u1")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = uv1.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_u2")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = uv2.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_w0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = weights.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_i0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = boneInd.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_b0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = bitans.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_t0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = tans.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_c0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = colors.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
            }
            if (atrib.Count == 0)
            {
                MessageBox.Show("Attributes are empty?");
                return;
            }
            helpernx.Attributes = atrib;
            VertexBuffer = helpernx.ToVertexBuffer();
        }

        internal List<Syroot.Maths.Vector4F> verts = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> norms = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> uv0 = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> uv1 = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> uv2 = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> tans = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> bitans = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> weights = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> boneInd = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> colors = new List<Syroot.Maths.Vector4F>();

        public string GetBoneNameFromIndex(FMDL mdl, int index)
        {
            if (index == 0)
                return "";

            return mdl.Skeleton.bones[mdl.Skeleton.Node_Array[index]].Text;
        }

        public void UpdateVertices()
        {
            //     CalculateTangentBitangent();

            foreach (Vertex vtx in vertices)
            {
                if (VertexSkinCount == 0 || VertexSkinCount == 1)
                {
                    //     Console.WriteLine("Old " + vtx.pos);
                    //  vtx.pos = TransformLocal(vtx.pos);
                    //  vtx.nrm = TransformLocal(vtx.nrm, false);
                    //     Console.WriteLine("New " + vtx.pos);
                }
                //       Console.WriteLine($"Weight count {vtx.boneWeights.Count}");
                //      Console.WriteLine($"Index count {vtx.boneIds.Count}");

                verts.Add(new Syroot.Maths.Vector4F(vtx.pos.X, vtx.pos.Y, vtx.pos.Z, 1.0f));
                norms.Add(new Syroot.Maths.Vector4F(vtx.nrm.X, vtx.nrm.Y, vtx.nrm.Z, 0));
                uv0.Add(new Syroot.Maths.Vector4F(vtx.uv0.X, vtx.uv0.Y, 0, 0));
                uv1.Add(new Syroot.Maths.Vector4F(vtx.uv1.X, vtx.uv1.Y, 0, 0));
                uv2.Add(new Syroot.Maths.Vector4F(vtx.uv2.X, vtx.uv2.Y, 0, 0));
                tans.Add(new Syroot.Maths.Vector4F(vtx.tan.X, vtx.tan.Y, vtx.tan.Z, vtx.tan.W));
                bitans.Add(new Syroot.Maths.Vector4F(vtx.bitan.X, vtx.bitan.Y, vtx.bitan.Z, vtx.bitan.W));


                if (vtx.boneWeights.Count == 0)
                {
                    vtx.boneWeights.Add(0);
                    vtx.boneWeights.Add(0);
                    vtx.boneWeights.Add(0);
                    vtx.boneWeights.Add(0);
                }
                if (vtx.boneWeights.Count == 1)
                {
                    vtx.boneWeights.Add(0);
                    vtx.boneWeights.Add(0);
                    vtx.boneWeights.Add(0);
                }
                if (vtx.boneWeights.Count == 2)
                {
                    vtx.boneWeights.Add(0);
                    vtx.boneWeights.Add(0);
                }
                if (vtx.boneWeights.Count == 3)
                {
                    vtx.boneWeights.Add(0);
                }
                if (vtx.boneIds.Count == 0)
                {
                    vtx.boneIds.Add(0);
                    vtx.boneIds.Add(0);
                    vtx.boneIds.Add(0);
                    vtx.boneIds.Add(0);
                }
                if (vtx.boneIds.Count == 1)
                {
                    vtx.boneIds.Add(0);
                    vtx.boneIds.Add(0);
                    vtx.boneIds.Add(0);
                }
                if (vtx.boneIds.Count == 2)
                {
                    vtx.boneIds.Add(0);
                    vtx.boneIds.Add(0);
                }
                if (vtx.boneIds.Count == 3)
                {
                    vtx.boneIds.Add(0);
                }
                weights.Add(new Syroot.Maths.Vector4F(vtx.boneWeights[0], vtx.boneWeights[1], vtx.boneWeights[2], vtx.boneWeights[3]));
                boneInd.Add(new Syroot.Maths.Vector4F(vtx.boneIds[0], vtx.boneIds[1], vtx.boneIds[2], vtx.boneIds[3]));
                colors.Add(new Syroot.Maths.Vector4F(vtx.col.X, vtx.col.Y, vtx.col.Z, vtx.col.W));
            }
        }

        public List<DisplayVertex> CreateDisplayVertices()
        {
            // rearrange faces
            display = lodMeshes[DisplayLODIndex].getDisplayFace().ToArray();

            List<DisplayVertex> displayVertList = new List<DisplayVertex>();

            if (lodMeshes[DisplayLODIndex].faces.Count <= 3)
                return displayVertList;

            foreach (Vertex v in vertices)
            {
                DisplayVertex displayVert = new DisplayVertex()
                {
                    pos = v.pos,
                    nrm = v.nrm,
                    tan = v.tan.Xyz,
                    bit = v.bitan.Xyz,
                    col = v.col,
                    uv = v.uv0,
                    uv2 = v.uv1,
                    uv3 = v.uv2,
                    node = new Vector4(
                         v.boneIds.Count > 0 ? v.boneIds[0] : -1,
                         v.boneIds.Count > 1 ? v.boneIds[1] : -1,
                         v.boneIds.Count > 2 ? v.boneIds[2] : -1,
                         v.boneIds.Count > 3 ? v.boneIds[3] : -1),
                    weight = new Vector4(
                         v.boneWeights.Count > 0 ? v.boneWeights[0] : 0,
                         v.boneWeights.Count > 1 ? v.boneWeights[1] : 0,
                         v.boneWeights.Count > 2 ? v.boneWeights[2] : 0,
                         v.boneWeights.Count > 3 ? v.boneWeights[3] : 0),
                };

                displayVertList.Add(displayVert);


                /*   Console.WriteLine($"---------------------------------------------------------------------------------------");
                   Console.WriteLine($"Position   {displayVert.pos.X} {displayVert.pos.Y} {displayVert.pos.Z}");
                   Console.WriteLine($"Normal     {displayVert.nrm.X} {displayVert.nrm.Y} {displayVert.nrm.Z}");
                   Console.WriteLine($"Binormal   {displayVert.bit.X} {displayVert.bit.Y} {displayVert.bit.Z}");
                   Console.WriteLine($"Tanget     {displayVert.tan.X} {displayVert.tan.Y} {displayVert.tan.Z}");
                   Console.WriteLine($"Color      {displayVert.col.X} {displayVert.col.Y} {displayVert.col.Z} {displayVert.col.W}");
                   Console.WriteLine($"UV Layer 1 {displayVert.uv.X} {displayVert.uv.Y}");
                   Console.WriteLine($"UV Layer 2 {displayVert.uv2.X} {displayVert.uv2.Y}");
                   Console.WriteLine($"UV Layer 3 {displayVert.uv3.X} {displayVert.uv3.Y}");
                   Console.WriteLine($"Bone Index {displayVert.node.X} {displayVert.node.Y} {displayVert.node.Z} {displayVert.node.W}");
                   Console.WriteLine($"Weights    {displayVert.weight.X} {displayVert.weight.Y} {displayVert.weight.Z} {displayVert.weight.W}");
                   Console.WriteLine($"---------------------------------------------------------------------------------------");*/
            }

            return displayVertList;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using ResU = Syroot.NintenTools.Bfres;
using FirstPlugin;
using OpenTK;

namespace Bfres.Structs
{

    public class FSKL : STSkeleton
    {
        public int[] Node_Array;
        public List<Matrix3x4> matrices = new List<Matrix3x4>();
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
                    Skeleton.Export(sfd.FileName, ((FMDL)Parent).GetResFile());
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
            BfresSwitch.ReadSkeleton(node, skl, this);
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
        public ResFile GetResFile()
        {
            //ResourceFile -> FMDL -> Material Folder -> this
            return ((FMDL)Parent.Parent).GetResFile();
        }
        public void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Bfres Bone|*.bfbn;";
            sfd.FileName = Text;
            sfd.DefaultExt = ".bfbn";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Bone.Export(sfd.FileName, GetResFile());
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
}

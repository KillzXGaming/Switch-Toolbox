using System;
using OpenTK;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using ResU = Syroot.NintenTools.Bfres;
using FirstPlugin; 
using Switch_Toolbox.Library.NodeWrappers;
using Switch_Toolbox.Library.Forms;

namespace Bfres.Structs
{
    public class FSKL : STSkeleton
    {
        public int[] Node_Array;
        public fsklNode node;

        public List<Matrix4> GetInverseMatrices()
        {
            List<Matrix4> matrices = new List<Matrix4>();

            foreach (var bn in bones)
            {
                Console.WriteLine(bn.Text);
                Console.WriteLine(bn.CalculateSmoothMatrix());
                matrices.Add(bn.CalculateSmoothMatrix());
            }

            return matrices;
        }

        public void AddBone(Bone bone)
        {
            node.Skeleton.Bones.Add(bone);
        }

        public void AddBone(ResU.Bone bone)
        {
            node.SkeletonU.Bones.Add(bone.Name, bone);
        }

        public class fsklNode : STGenericWrapper
        {
            public FSKL fskl;

            public Skeleton Skeleton;
            public ResU.Skeleton SkeletonU;

            public override string ExportFilter => FileFilters.GetFilter(typeof(FSKL));
            public override string ImportFilter => FileFilters.GetFilter(typeof(FSKL));

            public BFRESRender GetRenderer()
            {
               return ((FMDL)Parent.Parent).GetRenderer();
            }
            public fsklNode()
            {
                Text = "Skeleton";
                Name = "Skeleton";

                ImageKey = "skeleton";
                SelectedImageKey = "skeleton";

                CanDelete = false;
                CanRename = false;
            }

            public override void LoadContextMenus()
            {
                ContextMenuStrip = new STContextMenuStrip();
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Import Bone", null, ImportAction, Keys.Control | Keys.I));
                ContextMenuStrip.Items.Add(new ToolStripSeparator());
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export Skeleton", null, ExportAction, Keys.Control | Keys.E));
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Replace Skeleton", null, ReplaceAction, Keys.Control | Keys.R));
            }

            public override void Replace(string FileName)
            {
                string extension = System.IO.Path.GetExtension(FileName);

                if (extension == ".bfskl")
                {
                    if (SkeletonU != null)
                    {
                        SkeletonU = new ResU.Skeleton();
                        SkeletonU.Import(FileName, GetResFileU());
                        Nodes.Clear();
                        fskl.bones.Clear();
                        BfresWiiU.ReadSkeleton(this, SkeletonU, fskl);
                    }
                    else
                    {
                        Skeleton = new Skeleton();
                        Skeleton.Import(FileName);
                        Nodes.Clear();
                        fskl.bones.Clear();
                        BfresSwitch.ReadSkeleton(this, Skeleton, fskl);
                    }
                }
            }

            public ResFile GetResFile()
            {
                return ((BFRESGroupNode)Parent).GetResFile();
            }

            public ResU.ResFile GetResFileU()
            {
                return ((BFRESGroupNode)Parent).GetResFileU();
            }

            public override void Import(string[] FileNames)
            {
                foreach (var FileName in FileNames)
                {

                    string extension = System.IO.Path.GetExtension(FileName);

                    if (extension == ".bfbon")
                    {
                        BfresBone bn = new BfresBone();

                        if (SkeletonU != null)
                        {
                            ResU.Bone bone = new ResU.Bone();
                            bone.Import(FileName, GetResFileU());

                            BfresWiiU.ReadBone(bn, bone, false);
                        }
                        else
                        {
                            Bone bone = new Bone();
                            bone.Import(FileName);

                            BfresSwitch.ReadBone(bn, bone, false);
                        }
                    }
                }
            }

            public override void Clear()
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all bones? This cannot be undone!", "", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Nodes.Clear();
                }
            }

            public override void Export(string FileName)
            {
                if (SkeletonU != null)
                    SkeletonU.Export(FileName, ((FMDL)Parent).GetResFileU());
                else
                    Skeleton.Export(FileName, ((FMDL)Parent).GetResFile());
            }

            public override void OnClick(TreeView treeView)
            {
                UpdateEditor();
            }
            public void UpdateEditor()
            {
                ((BFRES)Parent.Parent.Parent).LoadEditors(this);
            }
        }
        public FSKL()
        {

        }
        public FSKL(Skeleton skl)
        {
            node = new fsklNode();
            node.fskl = this;
            node.Skeleton = skl;
            BfresSwitch.ReadSkeleton(node, skl, this);
        }
        public FSKL(ResU.Skeleton skl)
        {
            node = new fsklNode();
            node.fskl = this;
            node.SkeletonU = skl;
            BfresWiiU.ReadSkeleton(node, skl, this);
        }
    }
    public class BfresBone : STBone
    {
        public bool UseSmoothMatrix { get; set; }

        public bool UseRigidMatrix { get; set; }

        public string BoneName
        {
            get
            {
                if (BoneU != null) return BoneU.Name;
                else return Bone.Name;
            }
            set
            {
                if (BoneU != null) BoneU.Name = value;
                else Bone.Name = value;
                Text = value;
            }
        }

        public bool FlagVisible = true;

        public ResU.ResDict<ResU.UserData> UserDataU;
        public List<UserData> UserData;

        public Bone Bone;
        public ResU.Bone BoneU;

        public BFRESRender BFRESRender;
        public BfresBone()
        {
            Load();
        }

        private void Load()
        {
            ImageKey = "bone";
            SelectedImageKey = "bone";
            Checked = true;

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.Control | Keys.I));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("New Child Bone", null, NewAction, Keys.Control | Keys.I));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Import Child Bone", null, ImportAction, Keys.Control | Keys.I));
            ContextMenuStrip.Items.Add(new ToolStripSeparator());
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export Bone", null, ExportAction, Keys.Control | Keys.I));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Replace Bone", null, ReplaceAction, Keys.Control | Keys.I));
        }

        protected void ExportAction(object sender, EventArgs args) { Export(); }
        protected void ImportAction(object sender, EventArgs args) { ImportChild(); }
        protected void ReplaceAction(object sender, EventArgs args) { Replace(); }
        protected void NewAction(object sender, EventArgs args) { NewChild(); }
        protected void RenameAction(object sender, EventArgs args) { Rename(); }

        public void Rename()
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK) { BoneName = dialog.textBox1.Text; }
        }

        public void CloneBaseInstance(STBone genericBone)
        {
            Text = genericBone.Text;

            position = new float[3];
            rotation = new float[4];
            scale = new float[3];

            position[0] = genericBone.position[0];
            position[1] = genericBone.position[1];
            position[2] = genericBone.position[2];
            rotation[0] = genericBone.rotation[0];
            rotation[1] = genericBone.rotation[1];
            rotation[2] = genericBone.rotation[2];
            rotation[3] = genericBone.rotation[3];
            scale[0] = genericBone.scale[0];
            scale[1] = genericBone.scale[1];
            scale[2] = genericBone.scale[2];
            RotationType = genericBone.RotationType;
            parentIndex = genericBone.parentIndex;
            RigidMatrixIndex = genericBone.RigidMatrixIndex;
            SmoothMatrixIndex = genericBone.SmoothMatrixIndex;
        }

        public void GenericToBfresBone()
        {
            if (BoneU != null)
            {
                BoneU.Position = new Syroot.Maths.Vector3F(position[0], position[1], position[2]);
                BoneU.Scale = new Syroot.Maths.Vector3F(scale[0], scale[1], scale[2]);
                BoneU.Rotation = new Syroot.Maths.Vector4F(rotation[0], rotation[1], rotation[2], rotation[3]);
                BoneU.Name = Text;
                BoneU.Flags = FlagVisible ? ResU.BoneFlags.Visible : 0;
                BoneU.ParentIndex = (ushort)parentIndex;
                BoneU.SmoothMatrixIndex = SmoothMatrixIndex;
                BoneU.RigidMatrixIndex = RigidMatrixIndex;

                SetTransforms();
            }
            else
            {
                Bone.Position = new Syroot.Maths.Vector3F(position[0], position[1], position[2]);
                Bone.Scale = new Syroot.Maths.Vector3F(scale[0], scale[1], scale[2]);
                Bone.Rotation = new Syroot.Maths.Vector4F(rotation[0], rotation[1], rotation[2], rotation[3]);
                Bone.Name = Text;
                Bone.Flags = FlagVisible ? BoneFlags.Visible : 0;
                Bone.ParentIndex = (ushort)parentIndex;
                Bone.SmoothMatrixIndex = SmoothMatrixIndex;
                Bone.RigidMatrixIndex = RigidMatrixIndex;

                SetTransforms();
            }
        }

        public void SetTransforms()
        {
            if (BoneU != null)
            {
                BoneU.TransformRotateZero = (GetRotation() == Quaternion.Identity);
                BoneU.TransformScaleOne = (GetScale() == Vector3.Zero);
                BoneU.TransformTranslateZero = (GetPosition() == Vector3.Zero);
            }
            else
            {
                Bone.TransformRotateZero = (GetRotation() == Quaternion.FromEulerAngles(0,0,0));
                Bone.TransformScaleOne = (GetScale() == Vector3.One);
                Bone.TransformTranslateZero = (GetPosition() == Vector3.Zero);
            }
        }

        public ResFile GetResFile()
        {
            return ((FMDL)((FSKL)skeletonParent).node.Parent).GetResFile();
        }

        public ResU.ResFile GetResFileU()
        {
            return ((FMDL)((FSKL)skeletonParent).node.Parent).GetResFileU();
        }

        public void NewChild()
        {
            BfresBone bn = new BfresBone(skeletonParent);

            if (BoneU != null)
            {
                BoneU = new ResU.Bone();
                BoneU.Name = CheckDuplicateBoneNames("NewBone");

                BfresWiiU.ReadBone(bn, BoneU, false);
                Nodes.Add(bn);
                skeletonParent.bones.Add(bn);

                BoneU.ParentIndex = (ushort)bn.parentIndex;
                ((FSKL)skeletonParent).AddBone(BoneU);
            }
            else
            {
                Bone = new Bone();
                Bone.Name = CheckDuplicateBoneNames("NewBone");

                BfresSwitch.ReadBone(bn, Bone, false);

                Nodes.Add(bn);
                skeletonParent.bones.Add(bn);

                ((FSKL)skeletonParent).AddBone(Bone);
            }
        }

        public void ImportChild()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = FileFilters.BONE;
            ofd.FileName = Text;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                BfresBone bn = new BfresBone(skeletonParent);

                if (BoneU != null)
                {
                    BoneU = new ResU.Bone();
                    BoneU.Import(ofd.FileName, GetResFileU());
                    BoneU.Name = CheckDuplicateBoneNames(BoneU.Name);

                    BfresWiiU.ReadBone(bn, BoneU, false);
                    Nodes.Add(bn);
                    skeletonParent.bones.Add(bn);
                    ((FSKL)skeletonParent).AddBone(BoneU);
                }
                else
                {
                    Bone = new Bone();
                    Bone.Import(ofd.FileName);
                    Bone.Name = CheckDuplicateBoneNames(Bone.Name);

                    BfresSwitch.ReadBone(bn, Bone, false);
                    Nodes.Add(bn);
                    skeletonParent.bones.Add(bn);
                    ((FSKL)skeletonParent).AddBone(Bone);
                }
            }
        }

        int dupedIndex = 0;
        private string CheckDuplicateBoneNames(string BoneName)
        {
            foreach (var bone in skeletonParent.bones)
            {
                if (bone.Text == BoneName)
                {
                    BoneName = $"{BoneName}{dupedIndex++}";
                    return CheckDuplicateBoneNames(BoneName);
                }
            }
            return BoneName;
        }

        public void Export()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = FileFilters.BONE;
            sfd.FileName = Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Bone.Export(sfd.FileName, GetResFile());
            }
        }
        public void Replace()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = FileFilters.BONE;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Bone.Import(ofd.FileName);
            }
            Bone.Name = Text;
        }

        public BfresBone(STSkeleton skeleton)
        {
            Load();

            skeletonParent = skeleton;
        }
        public override void OnClick(TreeView treeView)
        {
            UpdateEditor();
        }
        public void UpdateEditor()
        {
            ((BFRES)((FSKL)skeletonParent).node.Parent.Parent.Parent).LoadEditors(this);
        }
    }
}

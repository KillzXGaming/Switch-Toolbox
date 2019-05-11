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
using Switch_Toolbox.Library.IO;

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

        public void CalculateIndices()
        {
            if (node.SkeletonU != null)
            {
                var Skeleton = node.SkeletonU;

                if (Skeleton.InverseModelMatrices == null)
                    Skeleton.InverseModelMatrices = new List<Syroot.Maths.Matrix3x4>();
                if (Skeleton.MatrixToBoneList == null)
                    Skeleton.MatrixToBoneList = new List<ushort>();

                //Generate index list
                List<ushort> SmoothIndices = new List<ushort>();
                List<Syroot.Maths.Matrix3x4> SmoothMatrices = new List<Syroot.Maths.Matrix3x4>();
                List<ushort> RigidIndices = new List<ushort>();

                ushort SmoothIndex = 0;
                foreach (BfresBone bn in bones)
                {
                    ushort BoneIndex = 0;
                    foreach (var Bone in Skeleton.Bones.Values)
                    {
                        if (bn.Text == Bone.Name)
                        {
                            if (bn.UseSmoothMatrix || bn.SmoothMatrixIndex != -1)
                            {
                                bn.SmoothMatrixIndex = (short)SmoothIndex++;
                                Bone.SmoothMatrixIndex = bn.SmoothMatrixIndex;
                                SmoothIndices.Add(BoneIndex);

                                var mat = MatrixExenstion.GetMatrixInverted(bn);
                                SmoothMatrices.Add(mat);
                            }
                        }
                        BoneIndex++;
                    }
                }

                //Rigid Indices come after smooth indices. Start from the last smooth index
                ushort RigidIndex = (ushort)(SmoothIndices.Count);
                foreach (BfresBone bn in bones)
                {
                    ushort BoneIndex = 0;
                    foreach (var Bone in Skeleton.Bones.Values)
                    {
                        if (bn.Text == Bone.Name)
                        {
                            if (bn.UseRigidMatrix || bn.RigidMatrixIndex != -1)
                            {
                                bn.RigidMatrixIndex = (short)RigidIndex++;
                                Bone.RigidMatrixIndex = bn.RigidMatrixIndex;
                                RigidIndices.Add(BoneIndex);
                            }
                        }
                        BoneIndex++;
                    }
                }

                //Rigid indices at the end
                var AllIndices = SmoothIndices.Concat(RigidIndices).ToList();
                Skeleton.MatrixToBoneList = AllIndices.ToArray();
                Skeleton.InverseModelMatrices = SmoothMatrices;
            }
            else
            {
                var Skeleton = node.Skeleton;

                if (Skeleton.InverseModelMatrices == null)
                    Skeleton.InverseModelMatrices = new List<Syroot.Maths.Matrix3x4>();
                if (Skeleton.MatrixToBoneList == null)
                    Skeleton.MatrixToBoneList = new List<ushort>();

                //Generate index list
                List<ushort> SmoothIndices = new List<ushort>();
                List<Syroot.Maths.Matrix3x4> SmoothMatrices = new List<Syroot.Maths.Matrix3x4>();
                List<ushort> RigidIndices = new List<ushort>();

                ushort SmoothIndex = 0;

                foreach (BfresBone bn in bones)
                {
                    ushort BoneIndex = 0;
                    foreach (var Bone in Skeleton.Bones)
                    {
                        if (bn.Text == Bone.Name)
                        {
                            if (bn.UseSmoothMatrix || bn.SmoothMatrixIndex != -1)
                            {
                                bn.SmoothMatrixIndex = (short)SmoothIndex++;
                                Bone.SmoothMatrixIndex = bn.SmoothMatrixIndex;
                                SmoothIndices.Add(BoneIndex);

                                var mat = MatrixExenstion.GetMatrixInverted(bn);
                                SmoothMatrices.Add(mat);
                            }
                        }
                        BoneIndex++;
                    }
                }


                //Rigid Indices come after smooth indices. Start from the last smooth index
                ushort RigidIndex = (ushort)(SmoothIndices.Count);
                foreach (BfresBone bn in bones)
                {
                    ushort BoneIndex = 0;
                    foreach (var Bone in Skeleton.Bones)
                    {
                        if (bn.Text == Bone.Name)
                        {
                            if (bn.UseRigidMatrix || bn.RigidMatrixIndex != -1)
                            {
                                bn.RigidMatrixIndex = (short)RigidIndex++;
                                Bone.RigidMatrixIndex = bn.RigidMatrixIndex;
                                RigidIndices.Add(BoneIndex);
                            }
                        }
                        BoneIndex++;
                    }
                }

                //Rigid indices at the end
                var AllIndices = SmoothIndices.Concat(RigidIndices).ToList();
                Skeleton.MatrixToBoneList = AllIndices.ToArray();
                Skeleton.InverseModelMatrices = SmoothMatrices;
            }
        }

        public void AddBone(Bone bone)
        {
            node.Skeleton.Bones.Add(bone);
        }

        public void AddBone(ResU.Bone bone)
        {
            node.SkeletonU.Bones.Add(bone.Name, bone);
        }

        public FMDL GetModelParent()
        {
            return (FMDL)node.Parent;
        }

        public class fsklNode : STGenericWrapper
        {
            public FSKL fskl;

            public Skeleton Skeleton;
            public ResU.Skeleton SkeletonU;

            public override string ExportFilter => FileFilters.GetFilter(typeof(FSKL));
            public override string ImportFilter => FileFilters.GetFilter(typeof(BfresBone));
            public override string ReplaceFilter => FileFilters.GetFilter(typeof(FSKL));

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
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export All Bones", null, ExportAllAction, Keys.Control | Keys.B));
                ContextMenuStrip.Items.Add(new ToolStripSeparator());
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Replace Matching Bones (From Skeleton)", null, ReplaceMatchingFileAction, Keys.Control | Keys.S));
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Replace Matching Bones (From Folder)", null, ReplaceMatchingFolderAction, Keys.Control | Keys.F));
                ContextMenuStrip.Items.Add(new ToolStripSeparator());
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export Skeleton", null, ExportAction, Keys.Control | Keys.E));
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Replace Skeleton", null, ReplaceAction, Keys.Control | Keys.R));
            }

            protected void ExportAllAction(object sender, EventArgs args) { ExportAl(); }
            protected void ReplaceMatchingFileAction(object sender, EventArgs args) { ReplaceMatchingFile(); }
            protected void ReplaceMatchingFolderAction(object sender, EventArgs args) { ReplaceMatchingFolder(); }

            public void ExportAl()
            {
                if (Nodes.Count <= 0)
                    return;

                string formats = FileFilters.BONE;

                string[] forms = formats.Split('|');

                List<string> Formats = new List<string>();

                for (int i = 0; i < forms.Length; i++)
                {
                    if (i > 1 || i == (forms.Length - 1)) //Skip lines with all extensions
                    {
                        if (!forms[i].StartsWith("*"))
                            Formats.Add(forms[i]);
                    }
                }

                FolderSelectDialog sfd = new FolderSelectDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = sfd.SelectedPath;

                    BatchFormatExport form = new BatchFormatExport(Formats);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        string extension = form.GetSelectedExtension();

                        foreach (BfresBone node in fskl.bones)
                        {
                            ((BfresBone)node).Export($"{folderPath}\\{node.Text}{extension}");
                        }
                    }
                }
            }

            public void ReplaceMatchingFile( )
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.DefaultExt = ReplaceFilter;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string extension = System.IO.Path.GetExtension(ofd.FileName);

                    if (extension == ".bfskl")
                    {
                        if (SkeletonU != null)
                        {
                            ResU.Skeleton SkeltonTemp = new ResU.Skeleton();
                            SkeltonTemp.Import(ofd.FileName, GetResFileU());
                          
                            foreach (BfresBone bone in fskl.bones)
                            {
                                if (SkeltonTemp.Bones.ContainsKey(bone.Text))
                                    bone.CopyData(SkeltonTemp.Bones[bone.Text]);
                            }

                            BfresWiiU.ReadSkeleton(this, SkeletonU, fskl);
                        }
                        else
                        {
                            Skeleton SkeltonTemp = new Skeleton();
                            SkeltonTemp.Import(ofd.FileName);

                            foreach (BfresBone bone in fskl.bones)
                            {
                                if (SkeltonTemp.BoneDict.ContainsKey(bone.Text))
                                {
                                    int index = SkeltonTemp.BoneDict.GetIndex(bone.Text);
                                    bone.CopyData(SkeltonTemp.Bones[index]);
                                }
                            }

                            BfresSwitch.ReadSkeleton(this, Skeleton, fskl);
                        }
                    }
                }
            }

            public void ReplaceMatchingFolder()
            {
                FolderSelectDialog ofd = new FolderSelectDialog();

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in System.IO.Directory.GetFiles(ofd.SelectedPath))
                    {
                        string extension = System.IO.Path.GetExtension(file);

                        if (extension == ".bfbon")
                        {
                            if (SkeletonU != null)
                            {
                                ResU.Bone BoneTemp = new ResU.Bone();
                                BoneTemp.Import(file, GetResFileU());

                                foreach (BfresBone bone in fskl.bones)
                                {
                                    if (BoneTemp.Name == bone.Text)
                                        bone.CopyData(BoneTemp);
                                }

                                BfresWiiU.ReadSkeleton(this, SkeletonU, fskl);
                            }
                            else
                            {
                                Bone BoneTemp = new Bone();
                                BoneTemp.Import(file);

                                foreach (BfresBone bone in fskl.bones)
                                {
                                    if (BoneTemp.Name == bone.Text)
                                        bone.CopyData(BoneTemp);
                                }

                                BfresSwitch.ReadSkeleton(this, Skeleton, fskl);
                            }
                        }
                    }
                }
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
                return ((FMDL)Parent).GetResFile();
            }

            public ResU.ResFile GetResFileU()
            {
                return ((FMDL)Parent).GetResFileU();
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
                            bone.ParentIndex = -1;

                            BfresWiiU.ReadBone(bn, bone, false);
                        }
                        else
                        {
                            Bone bone = new Bone();
                            bone.Import(FileName);
                            bone.ParentIndex = -1;

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

        public void CopyData(ResU.Bone newBone)
        {
            BoneU.BillboardIndex = newBone.BillboardIndex;
            BoneU.Flags = newBone.Flags;
            BoneU.FlagsBillboard = newBone.FlagsBillboard;
            BoneU.FlagsRotation = newBone.FlagsRotation;
            BoneU.FlagsTransform = newBone.FlagsTransform;
            BoneU.FlagsTransformCumulative = newBone.FlagsTransformCumulative;
            BoneU.InverseMatrix = newBone.InverseMatrix;
            BoneU.ParentIndex = newBone.ParentIndex;
            BoneU.RigidMatrixIndex = newBone.RigidMatrixIndex;
            BoneU.SmoothMatrixIndex = newBone.SmoothMatrixIndex;
            BoneU.Position = newBone.Position;
            BoneU.Rotation = newBone.Rotation;
            BoneU.Scale = newBone.Scale;
            BoneU.UserData = newBone.UserData;
        }
        public void CopyData(Bone newBone)
        {
            Bone.BillboardIndex = newBone.BillboardIndex;
            Bone.Flags = newBone.Flags;
            Bone.FlagsBillboard = newBone.FlagsBillboard;
            Bone.FlagsRotation = newBone.FlagsRotation;
            Bone.FlagsTransform = newBone.FlagsTransform;
            Bone.FlagsTransformCumulative = newBone.FlagsTransformCumulative;
            Bone.ParentIndex = newBone.ParentIndex;
            Bone.RigidMatrixIndex = newBone.RigidMatrixIndex;
            Bone.SmoothMatrixIndex = newBone.SmoothMatrixIndex;
            Bone.Position = newBone.Position;
            Bone.Rotation = newBone.Rotation;
            Bone.Scale = newBone.Scale;
            Bone.UserData = newBone.UserData;
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
        protected void DeleteAction(object sender, EventArgs args) { Delete(); }

        public void Rename()
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK) { RenameBone(dialog.textBox1.Text); }
        }

        //Method to readjust the dictionaries for the skeleton
        public void RenameBone(string Name)
        {
            if (((FSKL)skeletonParent).node.SkeletonU != null)
            {
                BoneU.Name = Name;
                Text = Name;
                BoneName = Name;

                //Adjust dictionaries
                var Skeleton = ((FSKL)skeletonParent).node.SkeletonU;
                Skeleton.Bones.Remove(BoneU);
                Skeleton.Bones.Add(Name, BoneU);
            }
            else
            {
                Bone.Name = Name;
                Text = Name;
                BoneName = Name;
            }
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
                BoneU.ParentIndex = (short)parentIndex;
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
                Bone.ParentIndex = (short)parentIndex;
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

                BoneU.ParentIndex = (short)bn.parentIndex;
                ((FSKL)skeletonParent).AddBone(BoneU);
            }
            else
            {
                Bone = new Bone();
                Bone.Name = CheckDuplicateBoneNames("NewBone");

                BfresSwitch.ReadBone(bn, Bone, false);
                Nodes.Add(bn);
                skeletonParent.bones.Add(bn);

                Bone.ParentIndex = (short)bn.parentIndex;
                ((FSKL)skeletonParent).AddBone(Bone);
            }
        }

        public void Delete()
        {
            //For these to work
            //Shift bone node array in skeleton
            //Shift all indices in each shape's bone list
            //Shift every index in all the vertices indices from the changed node array
            //Shift every bone index in the shape
            //Shift each smooth index in each bone
            //Shift each rigid index in each bone
            //Remove the existing inverse matrix from the list

            string MappedNames = "";
            var model = ((FSKL)skeletonParent).GetModelParent();

            int CurrentIndex = GetIndex();

            if (model.Skeleton.bones.Count == 1)
            {
                MessageBox.Show("A single bone must exist in every model!", "Bone Delete",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var shape in model.shapes)
            {
                if (shape.BoneIndex == CurrentIndex)
                    MappedNames += $"{shape.Text}\n";

                if (shape.BoneIndices.Contains((ushort)CurrentIndex))
                    MappedNames += $"{shape.Text}\n";
            }
            if (MappedNames != "")
            {
                var result = STOptionsDialog.Show("Shapes are mapped to this bone. Are you sure you want to remove this? (Will default to first bone)",
                    "Bone Delete", MappedNames);

                if (result == DialogResult.Yes)
                    RemoveBone(model, CurrentIndex);
            }
            else
            {
                RemoveBone(model, CurrentIndex);
            }
        }

        private void RemoveBone(FMDL model, int CurrentIndex)
        {

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

                    BoneU.ParentIndex = (short)bn.parentIndex;
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

                    BoneU.ParentIndex = (short)bn.parentIndex;
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
                Export(sfd.FileName);
        }

        public void Export(string FileName)
        {
            string extension = System.IO.Path.GetExtension(FileName);

            switch (extension)
            {
                default:
                    if (BoneU != null)
                        BoneU.Export(FileName, GetResFileU());
                    else
                        Bone.Export(FileName, GetResFile());
                    break;
            }
        }

        public void Replace()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = FileFilters.BONE;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (BoneU != null)
                {
                    BoneU.Import(ofd.FileName, GetResFileU());
                    BoneU.Name = Text;
                }
                else
                {
                    Bone.Import(ofd.FileName);
                    Bone.Name = Text;
                }
            }
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

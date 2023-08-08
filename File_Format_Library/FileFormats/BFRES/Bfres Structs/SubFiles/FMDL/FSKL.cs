using System;
using OpenTK;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms;
using Toolbox.Library;
using ResU = Syroot.NintenTools.Bfres;
using FirstPlugin; 
using Toolbox.Library.NodeWrappers;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;

namespace Bfres.Structs
{
    public class FSKL : STSkeleton
    {
        public override int[] BoneIndices => Node_Array;

        public int[] Node_Array;
        public fsklNode node;

        public void CalculateIndices()
        {
            if (node.SkeletonU != null)
            {
                var Skeleton = node.SkeletonU;

                if (Skeleton.InverseModelMatrices == null)
                    Skeleton.InverseModelMatrices = new List<Syroot.Maths.Matrix3x4>();

                List<Syroot.Maths.Matrix3x4> SmoothMatrices = new List<Syroot.Maths.Matrix3x4>();

                foreach (var Bone in Skeleton.Bones.Values)
                    Bone.InverseMatrix = new Syroot.Maths.Matrix3x4(1,0,0,0,
                                                                    0,1,0,0,
                                                                    0,0,1,0);
                foreach (BfresBone bn in bones)
                {
                    ushort BoneIndex = 0;
                    foreach (var Bone in Skeleton.Bones.Values)
                    {
                        if (bn.Text == Bone.Name)
                        {
                            if (bn.UseSmoothMatrix)
                            {
                                var mat = MatrixExenstion.GetMatrixInverted(bn);
                                Bone.InverseMatrix = mat;
                                SmoothMatrices.Add(mat);
                            }
                        }
                        BoneIndex++;
                    }
                }
                
                Skeleton.InverseModelMatrices = SmoothMatrices;
            }
            else
            {
                var Skeleton = node.Skeleton;

                if (Skeleton.InverseModelMatrices == null)
                    Skeleton.InverseModelMatrices = new List<Syroot.Maths.Matrix3x4>();

                List<Syroot.Maths.Matrix3x4> SmoothMatrices = new List<Syroot.Maths.Matrix3x4>();
                foreach (BfresBone bn in bones)
                {
                    ushort BoneIndex = 0;
                    foreach (var Bone in Skeleton.Bones)
                    {
                        if (bn.Text == Bone.Name)
                        {
                            if (bn.UseSmoothMatrix || bn.SmoothMatrixIndex != -1)
                            {
                                var mat = MatrixExenstion.GetMatrixInverted(bn);
                                SmoothMatrices.Add(mat);
                            }
                        }
                        BoneIndex++;
                    }
                }
                Skeleton.InverseModelMatrices = SmoothMatrices;
            }
        }

        public bool IsIndexRigid(int index)
        {
            //Get the bone and see if the rigid index matches
            int BoneIndex = Node_Array[index];
            return bones[BoneIndex].RigidMatrixIndex != -1;
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

        public class fsklNode : STGenericWrapper, IContextMenuNode
        {
            public FSKL fskl;

            public Skeleton Skeleton;
            public ResU.Skeleton SkeletonU;

            public override string ExportFilter => FileFilters.GetFilter(typeof(FSKL));
            public override string ImportFilter => FileFilters.GetFilter(typeof(BfresBone));
            public override string ReplaceFilter => FileFilters.GetFilter(typeof(FSKL));

            public BFRESRenderBase GetRenderer()
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

            public ToolStripItem[] GetContextMenuItems()
            {
                return new ToolStripItem[]
                {
                    new ToolStripMenuItem("New Bone", null, NewBoneAction, Keys.Control | Keys.N),
                    new ToolStripMenuItem("Import Bone", null, ImportAction, Keys.Control | Keys.I),
                    new ToolStripMenuItem("Export All Bones", null, ExportAllAction, Keys.Control | Keys.B),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Replace Matching Bones (From Skeleton)", null, ReplaceMatchingFileAction, Keys.Control | Keys.S),
                    new ToolStripMenuItem("Replace Matching Bones (From Folder)", null, ReplaceMatchingFolderAction, Keys.Control | Keys.F),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Export Skeleton", null, ExportAction, Keys.Control | Keys.E),
                    new ToolStripMenuItem("Replace Skeleton", null, ReplaceAction, Keys.Control | Keys.R),
                };
            }

            protected void NewBoneAction(object sender, EventArgs args) { NewChildBone(); }
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
                    if (extension == ".csv")
                    {
                        using (var reader = new System.IO.StringReader(System.IO.File.ReadAllText(ofd.FileName)))
                        {
                            string value = reader.ReadLine();
                            if (value != "Bones Geometry")
                                return;

                            float X = 0;
                            float Y = 0;
                            float Z = 0;
                            float W = 0;
                            while (true)
                            {
                                string line = reader.ReadLine();
                                if (line != null)
                                {
                                    foreach (BfresBone bone in fskl.bones)
                                    {
                                        if (bone.Text == line)
                                        {
                                            string name = line;
                                            string scaleStr = reader.ReadLine();
                                            string rotationStr = reader.ReadLine();
                                            string translationStr = reader.ReadLine();

                                            string[] valuesS = scaleStr.Replace("\n", "").Replace("\r", "").Split(',');
                                            string[] valuesR = rotationStr.Replace("\n", "").Replace("\r", "").Split(',');
                                            string[] valuesT = translationStr.Replace("\n", "").Replace("\r", "").Split(',');

                                            Syroot.Maths.Vector3F translate;
                                            Syroot.Maths.Vector3F scale;
                                            Syroot.Maths.Vector4F rotate;

                                            float.TryParse(valuesT[0], out X);
                                            float.TryParse(valuesT[1], out Y);
                                            float.TryParse(valuesT[2], out Z);
                                            translate = new Syroot.Maths.Vector3F(X,Y,Z);

                                            float.TryParse(valuesR[0], out X);
                                            float.TryParse(valuesR[1], out Y);
                                            float.TryParse(valuesR[2], out Z);
                                            float.TryParse(valuesR[3], out W);
                                            rotate = new Syroot.Maths.Vector4F(X, Y, Z,W);

                                            float.TryParse(valuesS[0], out X);
                                            float.TryParse(valuesS[1], out Y);
                                            float.TryParse(valuesS[2], out Z);
                                            scale = new Syroot.Maths.Vector3F(X, Y, Z);

                                            if (bone.BoneU != null) {
                                                bone.BoneU.Position = translate;
                                                bone.BoneU.Scale = scale;
                                                bone.BoneU.Rotation = rotate;
                                            }
                                            else {
                                                bone.Bone.Position = translate;
                                                bone.Bone.Scale = scale;
                                                bone.Bone.Rotation = rotate;
                                            }
                                        }
                                    }
                                }
                                else
                                    break;
                            }

                            if (SkeletonU != null)
                                BfresWiiU.ReadSkeleton(this, SkeletonU, fskl);
                            else
                                BfresSwitch.ReadSkeleton(this, Skeleton, fskl);

                            LibraryGUI.UpdateViewport();
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
                                BoneTemp.SmoothMatrixIndex = -1;
                                BoneTemp.RigidMatrixIndex = -1;

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
                                BoneTemp.SmoothMatrixIndex = -1;
                                BoneTemp.RigidMatrixIndex = -1;

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
                    //Todo regenerate indices.
                    //This will just fix swapping the same bonesets with slightly adjusted indices
                    if (SkeletonU != null)
                    {
                        var indices = this.SkeletonU.MatrixToBoneList;

                        SkeletonU = new ResU.Skeleton();
                        SkeletonU.Import(FileName, GetResFileU());
                        SkeletonU.MatrixToBoneList = indices;

                        Nodes.Clear();
                        fskl.bones.Clear();
                        BfresWiiU.ReadSkeleton(this, SkeletonU, fskl);
                    }
                    else
                    {
                        var indices = this.SkeletonU.MatrixToBoneList;

                        Skeleton = new Skeleton();
                        Skeleton.Import(FileName);
                        SkeletonU.MatrixToBoneList = indices;

                        Nodes.Clear();
                        fskl.bones.Clear();
                        BfresSwitch.ReadSkeleton(this, Skeleton, fskl);
                    }
                }
            }

            private void SwapFromCsv()
            {

            }

            public ResFile GetResFile()
            {
                return ((FMDL)Parent).GetResFile();
            }

            public ResU.ResFile GetResFileU()
            {
                return ((FMDL)Parent).GetResFileU();
            }

            public void NewChildBone()
            {
                BfresBone bone = new BfresBone(fskl);
                bone.parentIndex = -1;

                if (SkeletonU != null)
                {
                    bone.BoneU = new ResU.Bone();
                    bone.BoneU.Name = CheckDuplicateBoneNames("NewBone");

                    BfresWiiU.ReadBone(bone, bone.BoneU, false);

                    Nodes.Add(bone);
                    fskl.bones.Add(bone);

                    bone.BoneU.ParentIndex = (short)bone.parentIndex;
                    fskl.AddBone(bone.BoneU);
                }
                else
                {
                    bone.Bone = new Bone();
                    bone.Bone.Name = CheckDuplicateBoneNames("NewBone");

                    BfresSwitch.ReadBone(bone, bone.Bone, false);

                    Nodes.Add(bone);
                    fskl.bones.Add(bone);

                    bone.Bone.ParentIndex = (short)bone.parentIndex;
                    fskl.AddBone(bone.Bone);
                }
            }

            int dupedIndex = 0;
            private string CheckDuplicateBoneNames(string BoneName)
            {
                foreach (var bone in fskl.bones)
                {
                    if (bone.Text == BoneName)
                    {
                        BoneName = $"{BoneName}{dupedIndex++}";
                        return CheckDuplicateBoneNames(BoneName);
                    }
                }
                return BoneName;
            }

            public override void Import(string[] FileNames)
            {
                foreach (var FileName in FileNames)
                {
                    string extension = System.IO.Path.GetExtension(FileName);

                    if (extension == ".bfbon")
                    {
                        BfresBone bn = new BfresBone(fskl);

                        List<string> boneKeys = fskl.bones.Select(i => i.Text).ToList();
                        if (SkeletonU != null)
                        {
                            ResU.Bone bone = new ResU.Bone();
                            bone.Import(FileName, GetResFileU());
                            bone.ParentIndex = -1;
                            bone.SmoothMatrixIndex = -1;
                            bone.RigidMatrixIndex = -1;
                            bone.Name = Utils.RenameDuplicateString(boneKeys, bone.Name);

                            BfresWiiU.ReadBone(bn, bone, false);

                            Nodes.Add(bn);
                            fskl.bones.Add(bn);

                            bn.BoneU.ParentIndex = (short)bn.parentIndex;
                            fskl.AddBone(bn.BoneU);
                        }
                        else
                        {
                            Bone bone = new Bone();
                            bone.Import(FileName);
                            bone.ParentIndex = -1;
                            bone.SmoothMatrixIndex = -1;
                            bone.RigidMatrixIndex = -1;
                            bone.Name = Utils.RenameDuplicateString(boneKeys, bone.Name);

                            BfresSwitch.ReadBone(bn, bone, false);

                            Nodes.Add(bn);
                            fskl.bones.Add(bn);

                            bn.Bone.ParentIndex = (short)bn.parentIndex;
                            fskl.AddBone(bn.Bone);
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
    public class BfresBone : STBone, IContextMenuNode
    {
        public bool UseSmoothMatrix => SmoothMatrixIndex != -1;

        public bool UseRigidMatrix => RigidMatrixIndex != -1;

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
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.Control | Keys.R));
            Items.Add(new ToolStripSeparator());
            Items.Add(new ToolStripMenuItem("New Child Bone", null, NewAction, Keys.Control | Keys.N));
            Items.Add(new ToolStripMenuItem("Import Child Bone", null, ImportAction, Keys.Control | Keys.I));
            Items.Add(new ToolStripSeparator());
            Items.Add(new ToolStripMenuItem("Export Bone", null, ExportAction, Keys.Control | Keys.E));
            Items.Add(new ToolStripMenuItem("Replace Bone", null, ReplaceAction, Keys.Control | Keys.R));
            return Items.ToArray();
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

        //Method to re-adjust the dictionaries for the skeleton
        public void RenameBone(string Name)
        {
            if (((FSKL)skeletonParent).node.SkeletonU != null)
            {
                var Skeleton = ((FSKL)skeletonParent).node.SkeletonU;

                if (Skeleton.Bones.ContainsKey(Name))
                {
                    MessageBox.Show("A bone with the same name exits! Make sure to use a unique name!", 
                        "Bone Rename", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                BoneU.Name = Name;
                Text = Name;
                BoneName = Name;

                //Adjust dictionary
                UpdateBoneKeys(Skeleton.Bones.Values.ToList());
            }
            else
            {
                Bone.Name = Name;
                Text = Name;
                BoneName = Name;
            }
        }

        private void UpdateBoneKeys(List<ResU.Bone> bones)
        {
            var Skeleton = ((FSKL)skeletonParent).node.SkeletonU;

            Skeleton.Bones.Clear();
            for (int i = 0; i < bones.Count; i++)
                Skeleton.Bones.Add(bones[i].Name, bones[i]);
        }

        public void CloneBaseInstance(STBone genericBone)
        {
            Text = genericBone.Text;

            Position = new OpenTK.Vector3(
                genericBone.Position.X,
                genericBone.Position.Y,
                genericBone.Position.Z);
            EulerRotation = new OpenTK.Vector3(
                genericBone.EulerRotation.X,
                genericBone.EulerRotation.Y,
                genericBone.EulerRotation.Z);
            Scale = new OpenTK.Vector3(
                genericBone.Scale.X,
                genericBone.Scale.Y,
                genericBone.Scale.Z);

            RotationType = genericBone.RotationType;
            parentIndex = genericBone.parentIndex;
            RigidMatrixIndex = genericBone.RigidMatrixIndex;
            SmoothMatrixIndex = genericBone.SmoothMatrixIndex;
        }

        public void GenericToBfresBone()
        {
            if (BoneU != null)
            {
                BoneU.Position = new Syroot.Maths.Vector3F(Position.X, Position.Y, Position.Z);
                BoneU.Scale = new Syroot.Maths.Vector3F(Scale.X, Scale.Y, Scale.Z);
                if (RotationType == BoneRotationType.Euler)
                    BoneU.Rotation = new Syroot.Maths.Vector4F(EulerRotation.X, EulerRotation.Y, EulerRotation.Z, 1.0f);
                else
                    BoneU.Rotation = new Syroot.Maths.Vector4F(Rotation.X, Rotation.Y, Rotation.Z, Rotation.W);
                BoneU.Name = Text;
                BoneU.Flags = FlagVisible ? ResU.BoneFlags.Visible : 0;
                BoneU.ParentIndex = (short)parentIndex;
                BoneU.SmoothMatrixIndex = SmoothMatrixIndex;
                BoneU.RigidMatrixIndex = RigidMatrixIndex;

                SetTransforms();
            }
            else
            {
                Bone.Position = new Syroot.Maths.Vector3F(Position.X, Position.Y, Position.Z);
                Bone.Scale = new Syroot.Maths.Vector3F(Scale.X, Scale.Y, Scale.Z);
                if (RotationType == BoneRotationType.Euler)
                    Bone.Rotation = new Syroot.Maths.Vector4F(EulerRotation.X, EulerRotation.Y, EulerRotation.Z, 1.0f);
                else
                    Bone.Rotation = new Syroot.Maths.Vector4F(Rotation.X, Rotation.Y, Rotation.Z, Rotation.W);
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
                BoneU.TransformRotateZero = BoneU.Rotation == new Syroot.Maths.Vector4F(0,0,0,1);
                BoneU.TransformScaleOne = BoneU.Scale == Syroot.Maths.Vector3F.One;
                BoneU.TransformTranslateZero = BoneU.Position == Syroot.Maths.Vector3F.Zero;
            }
            else
            {
                Bone.TransformRotateZero = Bone.Rotation == new Syroot.Maths.Vector4F(0, 0, 0, 1);
                Bone.TransformScaleOne = Bone.Scale == Syroot.Maths.Vector3F.One;
                Bone.TransformTranslateZero = Bone.Position == Syroot.Maths.Vector3F.Zero;
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
                    BoneU.SmoothMatrixIndex = -1;
                    BoneU.RigidMatrixIndex = -1;
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
                    Bone.SmoothMatrixIndex = -1;
                    Bone.RigidMatrixIndex = -1;
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

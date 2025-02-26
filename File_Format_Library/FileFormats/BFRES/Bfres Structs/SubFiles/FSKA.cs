using System;
using System.Linq;
using System.Collections.Generic;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms;
using Toolbox.Library;
using ResU = Syroot.NintenTools.Bfres;
using FirstPlugin;
using OpenTK;
using Toolbox.Library.Animations;
using Toolbox.Library.Forms;
using SELib;
using FirstPlugin.Forms;
using static Toolbox.Library.Animations.Animation;

namespace Bfres.Structs
{
    public class FSKA : Animation, IContextMenuNode
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
            WROT = 0x2C,
        }
        public SkeletalAnim SkeletalAnim;
        public ResU.SkeletalAnim SkeletalAnimU;

        public FSKA()
        {
            Initialize();
        }

        public void Initialize()
        {
            ImageKey = "skeletonAnimation";
            SelectedImageKey = "skeletonAnimation";

            CanRename = true;
            CanReplace = true;
            CanExport = true;
            CanDelete = true;

            OpenAnimationData();
        }

        public override ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.AddRange(base.GetContextMenuItems());
            Items.Add(new ToolStripMenuItem("Copy", null, CopyAction, Keys.Control | Keys.C));
            Items.Add(new ToolStripMenuItem("New Bone Target", null, NewAction, Keys.Control | Keys.W));
            return Items.ToArray();
        }

        protected void CopyAction(object sender, EventArgs e) { CopyAnimForm(); }

        protected void NewAction(object sender, EventArgs e) { NewBoneAnim(); }

        public void NewBoneAnim()
        {
            var boneAnim = new BoneAnimNode("NewBoneTarget", true);

            if (Nodes.Count <= 0)
            {
                foreach (var bone in Bones)
                {
                    Nodes.Add(bone);
                }
                this.Expand();
            }

            Nodes.Add(boneAnim);
            Bones.Add(boneAnim);

            if (SkeletalAnimU != null)
            {
                boneAnim.BoneAnimU = new ResU.BoneAnim() { Name = boneAnim.Text };
            }
            else
            {
                boneAnim.BoneAnim = new BoneAnim() { Name = boneAnim.Text };
            }
        }

        public void CopyAnimForm()
        {
            BfresAnimationCopy copyDlg = new BfresAnimationCopy();

            List<FSKA> anims = new List<FSKA>();
            foreach (var node in Parent.Nodes)
                anims.Add((FSKA)node);

            copyDlg.LoadAnimationSections(this, anims);
            if (copyDlg.ShowDialog() == DialogResult.OK)
            {
                foreach (var anim in copyDlg.GetAnimationCopies()) {
                    anim.CopyAnimation(this, copyDlg.CopySettings, 
                        copyDlg.CopyBoneAnims, copyDlg.CopyUserData);
                }
            }
        }

        public void CopyAnimation(FSKA target, bool copySettings, 
            bool copyBoneAnims, bool copyUserData)
        {
            if (SkeletalAnim != null)
            {
                SkeletalAnim.Copy(target.SkeletalAnim, new SkeletalAnim.CopyFilter()
                {
                    CopyBoneAnims = copyBoneAnims,
                    CopySettings = copySettings,
                    CopyUserData = copyUserData,
                });
            }
            else
            {
                SkeletalAnimU.Copy(target.SkeletalAnimU, new ResU.SkeletalAnim.CopyFilter()
                {
                    CopyBoneAnims = copyBoneAnims,
                    CopySettings = copySettings,
                    CopyUserData = copyUserData,
                });
            }

            OpenAnimationData();
        }

        public ResFile GetResFile() {
            return ((BFRESGroupNode)Parent).GetResFile();
        }

        public ResU.ResFile GetResFileU() {
            return ((BFRESGroupNode)Parent).GetResFileU();
        }

        public override void OnClick(TreeView treeView) => UpdateEditor();

        public override void OnDoubleMouseClick(TreeView treeview)
        {
            if (Nodes.Count <= 0)
            {
                foreach (var bone in Bones)
                {
                    bone.LoadMenus();
                    Nodes.Add(bone);
                }
                this.Expand();
            }
        }

        public void UpdateEditor() {

            ((BFRES)Parent?.Parent?.Parent)?.LoadEditors(this);
        }

        public override string ExportFilter => FileFilters.GetFilter(typeof(FSKA),null, true);
        public override string ReplaceFilter => FileFilters.GetFilter(typeof(FSKA));

        public override void Export(string FilePath)
        {
            Export(FilePath, null);
        }

        public void Export(string FilePath, STSkeleton backupSkeleton)
        {
            Console.WriteLine($"Anim Export: {FilePath}");
            string sourcePath = "";
            if (SkeletalAnimU != null)
                sourcePath = SkeletalAnimU.Path;
            else if (SkeletalAnim != null)
                sourcePath = SkeletalAnim.Path;

                string ext = Utils.GetExtension(FilePath);
            if (ext == ".bfska")
            {
                if (GetResFileU() != null)
                {
                    SkeletalAnimU.Export(FilePath, GetResFileU());
                }
                else
                {
                    SkeletalAnim.Export(FilePath, GetResFile());
                }
            }
            else if (ext == ".json")
            {
                if (SkeletalAnimU != null)
                    System.IO.File.WriteAllText(FilePath, Newtonsoft.Json.JsonConvert.SerializeObject(SkeletalAnimU,
                        Newtonsoft.Json.Formatting.Indented));
                else
                    System.IO.File.WriteAllText(FilePath, Newtonsoft.Json.JsonConvert.SerializeObject(SkeletalAnim,
                        Newtonsoft.Json.Formatting.Indented));
            }
            else if (ext == ".chr0")
            {
                if (SkeletalAnimU != null)
                    BrawlboxHelper.FSKAConverter.Fska2Chr0(BfresPlatformConverter.FSKAConvertWiiUToSwitch(SkeletalAnimU), FilePath);
                else
                    BrawlboxHelper.FSKAConverter.Fska2Chr0(SkeletalAnim, FilePath);
            }
            else
            {
                // The export functions of the following formats require a skeleton.
                // Try to find best matching skeleton in the visible viewport skeletons,
                // or the animation's nodegroup, if it's in the nodetree (when this function
                // is called by right click->Export)
                STSkeleton skeleton = GetActiveSkeleton();
                if (skeleton == null)
                    skeleton = backupSkeleton; // When this func is called from Tools->Batch Export Animations, backupSkeleton is the one from the .sbfres.
                if (skeleton == null)
                {
                    throw new Exception($"{Text}No skeleton found.\n  Load a skeleton in the viewport with a skeleton matching the animations.");
                }
                var missingBones = new HashSet<string>();
                foreach (KeyNode boneAnim in Bones)
                {
                    STBone bone = skeleton.GetBone(boneAnim.Text);
                    if (bone == null)
                    {
                        missingBones.Add(boneAnim.Text);
                    }
                }
                
                if (ext == ".smd")
                    SMD.Save(this, skeleton, FilePath);
                else if (ext == ".anim")
                    ANIM.CreateANIM(FilePath, this, skeleton);
                else if (ext == ".seanim")
                {
                    SEANIM.SaveAnimation(FilePath, this, skeleton);
                }

                if (missingBones.Count > 0)
                    throw new Exception($"{Text}: Discarded animation of {missingBones.Count} bones:\n  {string.Join("\n  ", missingBones)}\n  Load a skeleton in the viewport with a skeleton matching the animations.");
            }
        }

        private STSkeleton GetActiveSkeleton()
        {
            STSkeleton perfectSkeleton = GetPerfectMatchSkeleton_InViewport();
            if (perfectSkeleton != null)
                return perfectSkeleton;

            perfectSkeleton = GetPerfectMatchSkeleton_InOwnNodeGroup();
            if (perfectSkeleton != null)
                return perfectSkeleton;

            //If those didn't return, resort to the first model of the nodegroup,
            //even though it's an imperfect match.
            if (Parent == null)
                return null;

            var render = ((BFRES)Parent.Parent.Parent).BFRESRender;
            if (render != null && render.models.Count == 1)
                return render.models[0].Skeleton;

            return null;
        }

        public STSkeleton GetPerfectMatchSkeleton_InViewport()
        {
            //Try to find a skeleton matching this animation out of visible viewport skeletons.
            var viewport = LibraryGUI.GetActiveViewport();
            if (viewport == null)
                return null;

            var containers = viewport.GetActiveContainers();
            if (containers == null || containers.Count == 0)
                return null;

            var skeletons = new List<STSkeleton>();

            foreach (var container in containers)
            {
                foreach (var drawable in container.Drawables)
                {
                    if (drawable is STSkeleton)
                        skeletons.Add((STSkeleton)drawable);
                }
            }

            return GetPerfectMatchSkeleton(skeletons);
        }

        public STSkeleton GetPerfectMatchSkeleton_InOwnNodeGroup()
        {
            if (Parent == null)
                return null;

            //Check parent renderer and find skeleton
            var render = ((BFRES)Parent.Parent.Parent).BFRESRender;
            if (render == null)
                return null;

            var skeletons = (from model in render.models select (STSkeleton)model.Skeleton).ToList();

            STSkeleton bestMatchSkeleton = GetPerfectMatchSkeleton(skeletons);
            if (bestMatchSkeleton != null)
                return bestMatchSkeleton;

            return null;
        }

        public STSkeleton GetPerfectMatchSkeleton(List<STSkeleton> skeletons)
        {
            //Search multiple FMDL to find matching bones
            foreach (var skeleton in skeletons)
            {
                //Check if all the bones in the animation are present in the skeleton
                bool areAllBonesPresent = skeleton.bones.Count > 0;
                foreach (var bone in Bones)
                {
                    var animBone = skeleton.GetBone(bone.Text);

                    if (animBone == null)
                    {
                        areAllBonesPresent = false;
                        break;
                    }
                }
                if (areAllBonesPresent)
                    return skeleton;
            }

            return null;
        }

        public override void Replace(string FileName) {
            Replace(FileName, GetResFile(), GetResFileU());
        }

        public void Replace(string FileName, ResFile resFileNX, ResU.ResFile resFileU)
        {
            string ext = Utils.GetExtension(FileName);
            if (ext == ".bfska")
            {
                bool IsSwitch = BfresUtilies.IsSubSectionSwitch(FileName);

                if (resFileU != null)
                {
                    //If it's a switch animation try to conver it to wii u
                    if (IsSwitch)
                    {
                        var ska = new SkeletalAnim();
                        ska.Import(FileName);
                        SkeletalAnimU = BfresPlatformConverter.FSKAConvertSwitchToWiiU(ska);
                        SkeletalAnimU.Name = Text;
                        LoadAnim(SkeletalAnimU);
                    }
                    else
                    {
                        SkeletalAnimU.Import(FileName, resFileU);
                        SkeletalAnimU.Name = Text;
                        LoadAnim(SkeletalAnimU);
                    }
                }
                else
                {
                    if (IsSwitch)
                    {
                        SkeletalAnim.Import(FileName);
                        SkeletalAnim.Name = Text;
                        LoadAnim(SkeletalAnim);
                    }
                    else
                    {
                        //Create a new wii u skeletal anim and try to convert it instead
                        var ska = new ResU.SkeletalAnim();
                        ska.Import(FileName, new ResU.ResFile());
                        SkeletalAnim = BfresPlatformConverter.FSKAConvertWiiUToSwitch(ska);
                        SkeletalAnim.Name = Text;
                        LoadAnim(SkeletalAnim);
                    }
                }
            }
            else if (ext == ".anim")
            {
                 FromAnim(FileName);
            }
            else if (ext == ".seanim")
            {
                STSkeleton skeleton = GetActiveSkeleton();

                if (skeleton != null)
                {
                    var ska = FromGeneric(SEANIM.Read(FileName, skeleton));
                    ska.Loop = this.CanLoop;
                    UpdateAnimation(ska);
                }
                else
                    STErrorDialog.Show("No matching skeleton bones found to assign!", "Skeleton Importer", "");
            }
            else if (ext == ".smd")
            {
                STSkeleton skeleton = GetActiveSkeleton();

                if (skeleton != null)
                {
                    var ska = FromGeneric(SMD.Read(FileName, skeleton));
                    ska.Loop = this.CanLoop;
                    UpdateAnimation(ska);
                }
                else
                     STErrorDialog.Show("No matching skeleton bones found to assign!", "Skeleton Importer", "");
            }
            else if (ext == ".chr0")
            {
                FromChr0(FileName, resFileU != null);
            }
            else if (ext == ".dae")
            {
             //   FromAssimp(FileName, resFileU != null);
            }
            else if (ext == ".fbx")
            {
             //   FromAssimp(FileName, resFileU != null);
            }
        }

        private void FromAssimp(string FileName, bool IsWiiU)
        {
            var anims = AssimpData.ImportAnimations(FileName);
            for (int i = 0; i < anims.Length; i++)
            {
                if (IsWiiU)
                {
                    SkeletalAnimU = BfresPlatformConverter.FSKAConvertSwitchToWiiU(FromGeneric(anims[i]));
                    LoadAnim(SkeletalAnimU);
                }
                else
                {
                    SkeletalAnim = FromGeneric(anims[i]);
                    LoadAnim(SkeletalAnim);
                }

                break;
            }
        }

        private void UpdateAnimation(SkeletalAnim ska)
        {
            ska.Name = Text;

            if (SkeletalAnimU != null)
            {
                SkeletalAnimU = BfresPlatformConverter.FSKAConvertSwitchToWiiU(ska);
                LoadAnim(SkeletalAnimU);
            }
            else
            {
                SkeletalAnim = ska;
                LoadAnim(SkeletalAnim);
            }
        }

        public SkeletalAnim FromGeneric(Animation anim)
        {
            SkeletalAnim ska = new SkeletalAnim();
            ska.FrameCount = anim.FrameCount;
            ska.Name = anim.Text;
            ska.Path = "";
            ska.FlagsScale = SkeletalAnimFlagsScale.Maya;
            ska.FlagsRotate = SkeletalAnimFlagsRotate.EulerXYZ;
            ska.Loop = anim.CanLoop;
            ska.Baked = false;

            for (int b = 0; b < anim.Bones.Count; b++)
                ska.BoneAnims.Add(GenericBoneAnimToBfresBoneAnim(anim.Bones[b], b));
            
            ska.BakedSize = CalculateBakeSize(ska);
            ska.BindIndices = GenerateIndices(ska.BoneAnims.Count);
            return ska;
        }

        private BoneAnim GenericBoneAnimToBfresBoneAnim(Animation.KeyNode boneNode, int index)
        {
            BoneAnim boneAnim = new BoneAnim();
            boneAnim.Name = boneNode.Text;
            var posx = boneNode.XPOS.GetValue(0);
            var posy = boneNode.YPOS.GetValue(0);
            var posz = boneNode.ZPOS.GetValue(0);
            var scax = boneNode.XSCA.GetValue(0);
            var scay = boneNode.YSCA.GetValue(0);
            var scaz = boneNode.ZSCA.GetValue(0);
            var rotx = boneNode.XROT.GetValue(0);
            var roty = boneNode.YROT.GetValue(0);
            var rotz = boneNode.ZROT.GetValue(0);
            var rotw = boneNode.WROT.GetValue(0);

            BoneAnimData boneBaseData = new BoneAnimData();
            boneBaseData.Translate = new Syroot.Maths.Vector3F(posx, posy, posz);
            boneBaseData.Scale = new Syroot.Maths.Vector3F(scax, scay, scaz);
            boneBaseData.Rotate = new Syroot.Maths.Vector4F(rotx, roty, rotz, rotw);
            boneAnim.BaseData = boneBaseData;
            boneAnim.BeginBaseTranslate = 0;
            boneAnim.BeginTranslate = 6;
            boneAnim.BeginRotate = 3;
            boneAnim.Curves = new List<AnimCurve>();
            boneAnim.FlagsBase = BoneAnimFlagsBase.Translate | BoneAnimFlagsBase.Scale | BoneAnimFlagsBase.Rotate;

            if (boneBaseData.Rotate == new Syroot.Maths.Vector4F(0, 0, 0, 1))
                boneAnim.FlagsTransform |= BoneAnimFlagsTransform.RotateZero;
            if (boneBaseData.Translate == new Syroot.Maths.Vector3F(0, 0, 0))
                boneAnim.FlagsTransform |= BoneAnimFlagsTransform.TranslateZero;
            if (boneBaseData.Scale == new Syroot.Maths.Vector3F(1, 1, 1))
                boneAnim.FlagsTransform |= BoneAnimFlagsTransform.ScaleOne;
            if (IsUniform(boneBaseData.Scale))
                boneAnim.FlagsTransform |= BoneAnimFlagsTransform.ScaleUniform;
            if (index != 0) //root bone
                boneAnim.FlagsTransform |= BoneAnimFlagsTransform.SegmentScaleCompensate;

            if (boneNode.XSCA.HasAnimation())
            {
                boneAnim.FlagsCurve |= BoneAnimFlagsCurve.ScaleX;
                var curve = SetAnimationCurve(boneNode.XSCA, (uint)FSKA.TrackType.XSCA);
                if (curve != null)
                    boneAnim.Curves.Add(curve);
            }
            if (boneNode.YSCA.HasAnimation())
            {
                boneAnim.FlagsCurve |= BoneAnimFlagsCurve.ScaleY;
                var curve = SetAnimationCurve(boneNode.YSCA, (uint)FSKA.TrackType.YSCA);
                if (curve != null)
                    boneAnim.Curves.Add(curve);
            }
            if (boneNode.ZSCA.HasAnimation())
            {
                boneAnim.FlagsCurve |= BoneAnimFlagsCurve.ScaleZ;
                var curve = SetAnimationCurve(boneNode.ZSCA, (uint)FSKA.TrackType.ZSCA);
                if (curve != null)
                    boneAnim.Curves.Add(curve);
            }
            if (boneNode.XROT.HasAnimation())
            {
                boneAnim.FlagsCurve |= BoneAnimFlagsCurve.RotateX;
                var curve = SetAnimationCurve(boneNode.XROT, (uint)FSKA.TrackType.XROT);
                if (curve != null)
                    boneAnim.Curves.Add(curve);
            }
            if (boneNode.YROT.HasAnimation())
            {
                boneAnim.FlagsCurve |= BoneAnimFlagsCurve.RotateY;
                var curve = SetAnimationCurve(boneNode.YROT, (uint)FSKA.TrackType.YROT);
                if (curve != null)
                    boneAnim.Curves.Add(curve);
            }
            if (boneNode.ZROT.HasAnimation())
            {
                boneAnim.FlagsCurve |= BoneAnimFlagsCurve.RotateZ;
                var curve = SetAnimationCurve(boneNode.ZROT, (uint)FSKA.TrackType.ZROT);
                if (curve != null)
                    boneAnim.Curves.Add(curve);
            }
            if (boneNode.WROT.HasAnimation())
            {
                boneAnim.FlagsCurve |= BoneAnimFlagsCurve.RotateW;
                var curve = SetAnimationCurve(boneNode.WROT, (uint)FSKA.TrackType.WROT);
                if (curve != null)
                    boneAnim.Curves.Add(curve);
            }
            if (boneNode.XPOS.HasAnimation())
            {
                boneAnim.FlagsCurve |= BoneAnimFlagsCurve.TranslateX;
                var curve = SetAnimationCurve(boneNode.XPOS, (uint)FSKA.TrackType.XPOS);
                if (curve != null)
                    boneAnim.Curves.Add(curve);
            }
            if (boneNode.YPOS.HasAnimation())
            {
                boneAnim.FlagsCurve |= BoneAnimFlagsCurve.TranslateY;
                var curve = SetAnimationCurve(boneNode.YPOS, (uint)FSKA.TrackType.YPOS);
                if (curve != null)
                    boneAnim.Curves.Add(curve);
            }
            if (boneNode.ZPOS.HasAnimation())
            {
                boneAnim.FlagsCurve |= BoneAnimFlagsCurve.TranslateZ;
                var curve = SetAnimationCurve(boneNode.ZPOS, (uint)FSKA.TrackType.ZPOS);
                if (curve != null)
                    boneAnim.Curves.Add(curve);
            }

            return boneAnim;
        }

        private static bool IsUniform(Syroot.Maths.Vector3F value)
        {
            return value.X == value.Y && value.X == value.Z;
        }

        private static bool IsInt(float value) => value == Math.Truncate(value);
        private static void QuantizeCurveData(AnimCurve curve)
        {
            float MaxFrame = 0;
            float MaxValues = 0;

            List<bool> IntegerValues = new List<bool>();
            for (int frame = 0; frame < curve.Frames.Length; frame++)
            {
                MaxFrame = Math.Max(MaxFrame, curve.Frames[frame]);

                if (curve.CurveType == AnimCurveType.Linear)
                {
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 0]);
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 1]);

                    IntegerValues.Add(IsInt(curve.Keys[frame, 0]));
                    IntegerValues.Add(IsInt(curve.Keys[frame, 1]));
                }
                else if (curve.CurveType == AnimCurveType.Cubic)
                {
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 0]);
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 1]);
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 2]);
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 3]);

                    IntegerValues.Add(IsInt(curve.Keys[frame, 0]));
                    IntegerValues.Add(IsInt(curve.Keys[frame, 1]));
                    IntegerValues.Add(IsInt(curve.Keys[frame, 2]));
                    IntegerValues.Add(IsInt(curve.Keys[frame, 3]));
                }
                else
                {
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 0]);

                    IntegerValues.Add(IsInt(curve.Keys[frame, 0]));
                }

                int ConvertedInt = Convert.ToInt32(MaxValues);

            }

            if (MaxFrame < Byte.MaxValue)
                curve.FrameType = AnimCurveFrameType.Byte;
            else if (MaxFrame < Int16.MaxValue)
                curve.FrameType = AnimCurveFrameType.Decimal10x5;
            else
                curve.FrameType = AnimCurveFrameType.Single;


            if (IntegerValues.Any(x => x == false))
            {
                curve.KeyType = AnimCurveKeyType.Single;
            }
            else
            {
                if (MaxValues < Byte.MaxValue)
                    curve.KeyType = AnimCurveKeyType.SByte;
                else if (MaxFrame < Int16.MaxValue)
                    curve.KeyType = AnimCurveKeyType.Int16;
                else
                    curve.KeyType = AnimCurveKeyType.Single;
            }
        }

        private static AnimCurve SetAnimationCurve(Animation.KeyGroup keyGroup, uint DataOffset)
        {
            if (keyGroup.Keys.Count <= 1)
                return null;

            AnimCurve curve = new AnimCurve();
            curve.Frames = new float[(int)keyGroup.Keys.Count];
            curve.FrameType = AnimCurveFrameType.Single;
            curve.KeyType = AnimCurveKeyType.Single;
            curve.AnimDataOffset = DataOffset;
            curve.Scale = 1;
            curve.StartFrame = 0;
            curve.Offset = 0;

            var keyFrame = keyGroup.GetKeyFrame(0);

            if (keyFrame.InterType == InterpolationType.HERMITE)
            {
                curve.CurveType = AnimCurveType.Cubic;
                curve.Keys = new float[keyGroup.Keys.Count, 4];
                curve.Frames = new float[keyGroup.Keys.Count];

                for (int k = 0; k < keyGroup.Keys.Count; k++)
                {
                    float Delta = 0;
                    float frame = keyGroup.Keys[k].Frame;

                    if (k < keyGroup.Keys.Count - 1)
                        Delta = keyGroup.GetValue(k + 1) - keyGroup.GetValue(k);

                    float value = keyGroup.GetValue(frame);
                    curve.Keys[k, 0] = value;
                    curve.Keys[k, 1] = 0;
                    curve.Keys[k, 2] = 0;
                    curve.Keys[k, 3] = Delta;

                    curve.Frames[k] = keyGroup.Keys[k].Frame;
                }
            }
            else if (keyFrame.InterType == InterpolationType.LINEAR)
            {
                curve.CurveType = AnimCurveType.Linear;
                curve.Keys = new float[keyGroup.Keys.Count, 2];
                curve.Frames = new float[keyGroup.Keys.Count];

                for (int k = 0; k < keyGroup.Keys.Count; k++)
                {
                    float frame = keyGroup.Keys[k].Frame;

                    float Delta = 0;

                    if (k < keyGroup.Keys.Count - 1)
                        Delta = keyGroup.GetValue(k + 1) - keyGroup.GetValue(k);

                    curve.Keys[k, 0] = keyGroup.GetValue(frame);
                    curve.Keys[k, 1] = Delta;
                    curve.Frames[k] = frame;
                }
            }
            else if (keyFrame.InterType == InterpolationType.STEPBOOL)
            {
                curve.CurveType = AnimCurveType.StepBool;
                curve.Keys = new float[keyGroup.Keys.Count, 1];
            }

            else
            {
                curve.CurveType = AnimCurveType.StepInt;
                curve.Keys = new float[keyGroup.Keys.Count, 1];
                curve.Frames = new float[keyGroup.Keys.Count];

                for (int k = 0; k < keyGroup.Keys.Count; k++)
                {
                    float frame = keyGroup.Keys[k].Frame;

                    curve.Keys[k, 0] = keyGroup.GetValue(frame);
                    curve.Frames[k] = frame;
                }
            }

            //Difference of last and first key value
            if (curve.Keys.Length > 0)
                curve.Delta = curve.Keys[keyGroup.Keys.Count - 1, 0] - curve.Keys[0, 0];

            curve.EndFrame = curve.Frames.Max();

            QuantizeCurveData(curve);
            return curve;
        }

        private ushort[] GenerateIndices(int count)
        {
            var indices = new ushort[count];
            for (int i = 0; i < count; i++)
                indices[i] = ushort.MaxValue;

            return indices;
        }

        //This hasn't been necessary so far but i do need todo this
        public uint CalculateBakeSize(SkeletalAnim ska)
        {
            return 0;
        }

        public FSKA(ResU.SkeletalAnim ska) { LoadAnim(ska); }
        public FSKA(SkeletalAnim ska) { LoadAnim(ska); }

        public override void OpenAnimationData()
        {
            if (SkeletalAnimU != null)
                LoadAnimData(SkeletalAnimU);
            else if (SkeletalAnim != null)
                LoadAnimData(SkeletalAnim);
        }

        private void LoadAnimData(SkeletalAnim ska)
        {
            Nodes.Clear();
            Bones.Clear();

            CanLoop = ska.FlagsAnimSettings.HasFlag(SkeletalAnimFlags.Looping);

            for (int i = 0; i < ska.BoneAnims.Count; i++)
            {
                var bn = ska.BoneAnims[i];

                BoneAnimNode bone = new BoneAnimNode(bn.Name, false);
                bone.BoneAnim = bn;
                bone.UseSegmentScaleCompensate = bn.ApplySegmentScaleCompensate;

                Bones.Add(bone);
              //  Nodes.Add(bone);

                if (ska.FlagsRotate == SkeletalAnimFlagsRotate.EulerXYZ)
                    bone.RotType = Animation.RotationType.EULER;
                else
                    bone.RotType = Animation.RotationType.QUATERNION;

                if (bn.UseScale)
                {
                    bone.XSCA.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Scale.X, IsKeyed = true });
                    bone.YSCA.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Scale.Y, IsKeyed = true });
                    bone.ZSCA.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Scale.Z, IsKeyed = true });
                }
                if (bn.UseRotation)
                {
                    bone.XROT.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Rotate.X, IsKeyed = true });
                    bone.YROT.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Rotate.Y, IsKeyed = true });
                    bone.ZROT.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Rotate.Z, IsKeyed = true });
                    bone.WROT.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Rotate.W, IsKeyed = true });
                }
                if (bn.UseTranslation)
                {
                    bone.XPOS.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Translate.X, IsKeyed = true });
                    bone.YPOS.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Translate.Y, IsKeyed = true });
                    bone.ZPOS.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Translate.Z, IsKeyed = true });
                }

                for (int curve = 0; curve < bn.Curves.Count; curve++)
                {
                    Animation.KeyGroup keyGroup = CurveHelper.CreateTrack(bn.Curves[curve]);
                    keyGroup.AnimDataOffset = bn.Curves[curve].AnimDataOffset;
                    switch (keyGroup.AnimDataOffset)
                    {
                        case (int)TrackType.XPOS: bone.XPOS = keyGroup; break;
                        case (int)TrackType.YPOS: bone.YPOS = keyGroup; break;
                        case (int)TrackType.ZPOS: bone.ZPOS = keyGroup; break;
                        case (int)TrackType.XROT: bone.XROT = keyGroup; break;
                        case (int)TrackType.YROT: bone.YROT = keyGroup; break;
                        case (int)TrackType.ZROT: bone.ZROT = keyGroup; break;
                        case (int)TrackType.WROT: bone.WROT = keyGroup; break;
                        case (int)TrackType.XSCA: bone.XSCA = keyGroup; break;
                        case (int)TrackType.YSCA: bone.YSCA = keyGroup; break;
                        case (int)TrackType.ZSCA: bone.ZSCA = keyGroup; break;
                        default: throw new Exception("Unknown Anim Offset " + keyGroup.AnimDataOffset);
                    }
                }
            }
        }
        private void LoadAnimData(ResU.SkeletalAnim ska)
        {
            Nodes.Clear();
            Bones.Clear();

            CanLoop = ska.FlagsAnimSettings.HasFlag(ResU.SkeletalAnimFlags.Looping);

            foreach (ResU.BoneAnim bn in ska.BoneAnims)
            {
                BoneAnimNode bone = new BoneAnimNode(bn.Name, false);
                bone.BoneAnimU = bn;
                bone.UseSegmentScaleCompensate = bn.ApplySegmentScaleCompensate;

                Bones.Add(bone);
            //    Nodes.Add(bone);

                if (ska.FlagsRotate == ResU.SkeletalAnimFlagsRotate.EulerXYZ)
                    bone.RotType = Animation.RotationType.EULER;
                else
                    bone.RotType = Animation.RotationType.QUATERNION;


                if (bn.FlagsBase.HasFlag(ResU.BoneAnimFlagsBase.Scale))
                {
                    bone.XSCA.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Scale.X, IsKeyed = true });
                    bone.YSCA.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Scale.Y, IsKeyed = true });
                    bone.ZSCA.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Scale.Z, IsKeyed = true });
                }
                if (bn.FlagsBase.HasFlag(ResU.BoneAnimFlagsBase.Rotate))
                {
                    bone.XROT.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Rotate.X, IsKeyed = true });
                    bone.YROT.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Rotate.Y, IsKeyed = true });
                    bone.ZROT.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Rotate.Z, IsKeyed = true });
                    bone.WROT.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Rotate.W, IsKeyed = true });
                }
                if (bn.FlagsBase.HasFlag(ResU.BoneAnimFlagsBase.Translate))
                {
                    bone.XPOS.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Translate.X, IsKeyed = true });
                    bone.YPOS.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Translate.Y, IsKeyed = true });
                    bone.ZPOS.Keys.Add(new KeyFrame() { Frame = 0, Value = bn.BaseData.Translate.Z, IsKeyed = true });
                }

                for (int curve = 0; curve < bn.Curves.Count; curve++)
                {
                    Animation.KeyGroup keyGroup = CurveHelper.CreateTrackWiiU(bn.Curves[curve]);
                    keyGroup.AnimDataOffset = bn.Curves[curve].AnimDataOffset;
                    switch (keyGroup.AnimDataOffset)
                    {
                        case (int)TrackType.XPOS: bone.XPOS = keyGroup; break;
                        case (int)TrackType.YPOS: bone.YPOS = keyGroup; break;
                        case (int)TrackType.ZPOS: bone.ZPOS = keyGroup; break;
                        case (int)TrackType.XROT: bone.XROT = keyGroup; break;
                        case (int)TrackType.YROT: bone.YROT = keyGroup; break;
                        case (int)TrackType.ZROT: bone.ZROT = keyGroup; break;
                        case (int)TrackType.WROT: bone.WROT = keyGroup; break;
                        case (int)TrackType.XSCA: bone.XSCA = keyGroup; break;
                        case (int)TrackType.YSCA: bone.YSCA = keyGroup; break;
                        case (int)TrackType.ZSCA: bone.ZSCA = keyGroup; break;
                        default: throw new Exception("Unknown Anim Offset " + keyGroup.AnimDataOffset);
                    }
                }
            }
        }

        private void LoadAnim(ResU.SkeletalAnim ska)
        {
            Text = ska.Name;

            FrameCount = ska.FrameCount;
            SkeletalAnimU = ska;

            Initialize();
        }
        private void LoadAnim(SkeletalAnim ska)
        {
            Text = ska.Name;

            FrameCount = ska.FrameCount;
            SkeletalAnim = ska;

            Initialize();
        }

        public void FromAnim(string FileName)
        {
            if (SkeletalAnimU != null)
            {
                var SkeletalAnimNX = BrawlboxHelper.FSKAConverter.Anim2Fska(FileName);
                SkeletalAnimU = BfresPlatformConverter.FSKAConvertSwitchToWiiU(SkeletalAnimNX);
                SkeletalAnimU.Name = Text;
                LoadAnim(SkeletalAnimU);
            }
            else
            {
                SkeletalAnim = BrawlboxHelper.FSKAConverter.Anim2Fska(FileName);
                SkeletalAnim.Name = Text;
                LoadAnim(SkeletalAnim);
            }
        }

        public void FromChr0(string FileName, bool IsWiiU)
        {
            if (IsWiiU)
            {
                var SkeletalAnimNX = BrawlboxHelper.FSKAConverter.Chr02Fska(FileName);
                SkeletalAnimU = BfresPlatformConverter.FSKAConvertSwitchToWiiU(SkeletalAnimNX);
                SkeletalAnimU.Name = Text;
                LoadAnim(SkeletalAnimU);
            }
            else
            {
                SkeletalAnim = BrawlboxHelper.FSKAConverter.Chr02Fska(FileName);
                SkeletalAnim.Name = Text;
                LoadAnim(SkeletalAnim);
            }
        }



        private void CreateCurveData(AnimCurve curve, List<SEAnimFrame> keys)
        {
           for (int k = 0; k < keys.Count; k++)
            {
                
            }
        }

        public class BoneAnimNode : KeyNode
        {
            public override void OnClick(TreeView treeView) => UpdateEditor();

            public void UpdateEditor()
            {
                ((BFRES)Parent.Parent.Parent.Parent).LoadEditors(this);
            }

            public BoneAnim BoneAnim;
            public ResU.BoneAnim BoneAnimU;

            public ResU.BoneAnim SaveDataU(bool IsEdited)
            {
                BoneAnimU.Name = Text;
                if (IsEdited)
                {

                }
                return BoneAnimU;
            }

            public BoneAnim SaveData(bool IsEdited)
            {
                BoneAnim.Name = Text;
                if (IsEdited)
                {

                }
                return BoneAnim;
            }

            public BoneAnimNode(string bname, bool LoadContextMenus) : base(bname, LoadContextMenus)
            {

            }
        }
    }
}

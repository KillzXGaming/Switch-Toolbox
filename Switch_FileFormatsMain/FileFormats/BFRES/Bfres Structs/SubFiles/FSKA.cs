using System;
using System.Linq;
using System.Collections.Generic;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using ResU = Syroot.NintenTools.Bfres;
using FirstPlugin;
using OpenTK;
using Switch_Toolbox.Library.Animations;
using Switch_Toolbox.Library.Forms;
using SELib;

namespace Bfres.Structs
{
    public class FSKA : Animation
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

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("New Bone Target", null, NewAction, Keys.Control | Keys.W));
            LoadFileMenus(false);

            OpenAnimationData();
        }

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

            ((BFRES)Parent.Parent.Parent).LoadEditors(this);
        }

        public override string ExportFilter => FileFilters.GetFilter(typeof(FSKA), true);
        public override string ReplaceFilter => FileFilters.GetFilter(typeof(FSKA));

        public override void Export(string FileName)
        {
            string ext = Utils.GetExtension(FileName);
            if (ext == ".bfska")
            {
                if (GetResFileU() != null)
                {
                    SkeletalAnimU.Export(FileName, GetResFileU());
                }
                else
                {
                    SkeletalAnim.Export(FileName, GetResFile());
                }
            }
            else if (ext == ".chr0")
            {
                STSkeleton skeleton = GetActiveSkeleton();

                if (GetResFileU() != null)
                    BrawlboxHelper.FSKAConverter.Fska2Chr0(ConvertWiiUToSwitch(SkeletalAnimU), FileName);
                else
                    BrawlboxHelper.FSKAConverter.Fska2Chr0(SkeletalAnim, FileName);

                //  BrawlboxHelper.FSKAConverter.Fska2Chr0(this, skeleton, FileName);
            }
            else if (ext == ".smd")
            {
                STSkeleton skeleton = GetActiveSkeleton();

                if (skeleton != null)
                    SMD.Save(this, skeleton, FileName);
                else
                    throw new Exception("No skeleton found to assign!");
            }
            else if (ext == ".seanim")
            {
                STSkeleton skeleton = GetActiveSkeleton();

                if (skeleton != null)
                    SEANIM.SaveAnimation(FileName, this, skeleton);
                else
                    throw new Exception("No skeleton found to assign!");
            }
        }

        private STSkeleton GetActiveSkeleton()
        {
            var viewport = LibraryGUI.Instance.GetActiveViewport();
            if (viewport != null)
            {
                foreach (var drawable in viewport.scene.objects)
                {
                    if (drawable is STSkeleton)
                    {
                        foreach (var bone in Bones)
                        {
                            var animBone = ((STSkeleton)drawable).GetBone(bone.Text);

                            if (animBone != null)
                                return (STSkeleton)drawable;
                        }
                    }
                }
            }
            else
            {
                foreach (var model in ((BFRES)Parent.Parent.Parent).BFRESRender.models)
                {
                    foreach (var bone in Bones)
                    {
                        var animBone = model.Skeleton.GetBone(bone.Text);

                        if (animBone != null)
                            return model.Skeleton;
                    }
                }
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
                        SkeletalAnimU = ConvertSwitchToWiiU(ska);
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
                        SkeletalAnim = ConvertWiiUToSwitch(ska);
                        SkeletalAnim.Name = Text;
                        LoadAnim(SkeletalAnim);
                    }
                }
            }
            else if (ext == ".seanim")
            {
                FromSeanim(FileName);
            }
            else if (ext == ".chr0")
            {
                FromChr0(FileName);
            }
        }

        public static SkeletalAnim ConvertWiiUToSwitch(ResU.SkeletalAnim skeletalAnimU)
        {
            SkeletalAnim ska = new SkeletalAnim();
            ska.Name = skeletalAnimU.Name;
            ska.Path = skeletalAnimU.Path;
            ska.FrameCount = skeletalAnimU.FrameCount;
            ska.FlagsScale = SkeletalAnimFlagsScale.None;

            if (skeletalAnimU.FlagsScale.HasFlag(ResU.SkeletalAnimFlagsScale.Maya))
                ska.FlagsScale = SkeletalAnimFlagsScale.Maya;
            if (skeletalAnimU.FlagsScale.HasFlag(ResU.SkeletalAnimFlagsScale.Softimage))
                ska.FlagsScale = SkeletalAnimFlagsScale.Softimage;
            if (skeletalAnimU.FlagsScale.HasFlag(ResU.SkeletalAnimFlagsScale.Standard))
                ska.FlagsScale = SkeletalAnimFlagsScale.Standard;

            ska.FrameCount = skeletalAnimU.FrameCount;
            ska.BindIndices = skeletalAnimU.BindIndices;
            ska.BakedSize = skeletalAnimU.BakedSize;
            ska.Loop = skeletalAnimU.Loop;
            ska.Baked = skeletalAnimU.Baked;
            foreach (var boneAnimU in skeletalAnimU.BoneAnims)
            {
                var boneAnim = new BoneAnim();
                ska.BoneAnims.Add(boneAnim);
                boneAnim.Name = boneAnimU.Name;
                boneAnim.BeginRotate = boneAnimU.BeginRotate;
                boneAnim.BeginTranslate = boneAnimU.BeginTranslate;
                boneAnim.BeginBaseTranslate = boneAnimU.BeginBaseTranslate;
                var baseData = new BoneAnimData();
                baseData.Translate = boneAnimU.BaseData.Translate;
                baseData.Scale = boneAnimU.BaseData.Scale;
                baseData.Rotate = boneAnimU.BaseData.Rotate;
                baseData.Flags = boneAnimU.BaseData.Flags;
                boneAnim.BaseData = baseData;
                boneAnim.FlagsBase = (BoneAnimFlagsBase)boneAnimU.FlagsBase;
                boneAnim.FlagsCurve = (BoneAnimFlagsCurve)boneAnimU.FlagsCurve;
                boneAnim.FlagsTransform = (BoneAnimFlagsTransform)boneAnimU.FlagsTransform;

                foreach (var curveU in boneAnimU.Curves)
                {
                    AnimCurve curve = new AnimCurve();
                    curve.AnimDataOffset = curveU.AnimDataOffset;
                    curve.CurveType = (AnimCurveType)curveU.CurveType;
                    curve.Delta = curveU.Delta;
                    curve.EndFrame = curveU.EndFrame;
                    curve.Frames = curveU.Frames;
                    curve.Keys = curveU.Keys;
                    curve.KeyStepBoolData = curveU.KeyStepBoolData;
                    curve.KeyType = (AnimCurveKeyType)curveU.KeyType;
                    curve.FrameType = (AnimCurveFrameType)curveU.FrameType;
                    curve.StartFrame = curveU.StartFrame;
                    curve.Scale = curveU.Scale;
                    if (curve.Offset.GetType() == typeof(float))
                        curve.Offset = (float)curveU.Offset;
                    if (curve.Offset.GetType() == typeof(uint))
                        curve.Offset = (uint)curveU.Offset;
                    if (curve.Offset.GetType() == typeof(int))
                        curve.Offset = (int)curveU.Offset;

                    boneAnim.Curves.Add(curve);
                }
            }

            return ska;
        }

        public static ResU.SkeletalAnim ConvertSwitchToWiiU(SkeletalAnim skeletalAnimNX)
        {
            ResU.SkeletalAnim ska = new ResU.SkeletalAnim();
            ska.Name = skeletalAnimNX.Name;
            ska.Path = skeletalAnimNX.Path;
            ska.FrameCount = skeletalAnimNX.FrameCount;
            ska.FlagsScale = ResU.SkeletalAnimFlagsScale.None;

            if (skeletalAnimNX.FlagsScale.HasFlag(SkeletalAnimFlagsScale.Maya))
                ska.FlagsScale = ResU.SkeletalAnimFlagsScale.Maya;
            if (skeletalAnimNX.FlagsScale.HasFlag(SkeletalAnimFlagsScale.Softimage))
                ska.FlagsScale = ResU.SkeletalAnimFlagsScale.Softimage;
            if (skeletalAnimNX.FlagsScale.HasFlag(SkeletalAnimFlagsScale.Standard))
                ska.FlagsScale = ResU.SkeletalAnimFlagsScale.Standard;

            ska.FrameCount = skeletalAnimNX.FrameCount;
            ska.BindIndices = skeletalAnimNX.BindIndices;
            ska.BakedSize = skeletalAnimNX.BakedSize;
            ska.Loop = skeletalAnimNX.Loop;
            ska.Baked = skeletalAnimNX.Baked;
            foreach (var boneAnimNX in skeletalAnimNX.BoneAnims)
            {
                var boneAnimU = new ResU.BoneAnim();
                ska.BoneAnims.Add(boneAnimU);
                boneAnimU.Name = boneAnimNX.Name;
                boneAnimU.BeginRotate = boneAnimNX.BeginRotate;
                boneAnimU.BeginTranslate = boneAnimNX.BeginTranslate;
                boneAnimU.BeginBaseTranslate = boneAnimNX.BeginBaseTranslate;
                var baseData = new ResU.BoneAnimData();
                baseData.Translate = boneAnimNX.BaseData.Translate;
                baseData.Scale = boneAnimNX.BaseData.Scale;
                baseData.Rotate = boneAnimNX.BaseData.Rotate;
                baseData.Flags = boneAnimNX.BaseData.Flags;
                boneAnimU.BaseData = baseData;
                boneAnimU.FlagsBase = (ResU.BoneAnimFlagsBase)boneAnimNX.FlagsBase;
                boneAnimU.FlagsCurve = (ResU.BoneAnimFlagsCurve)boneAnimNX.FlagsCurve;
                boneAnimU.FlagsTransform = (ResU.BoneAnimFlagsTransform)boneAnimNX.FlagsTransform;

                foreach (var curveNX in boneAnimNX.Curves)
                {
                    ResU.AnimCurve curve = new ResU.AnimCurve();
                    curve.AnimDataOffset = curveNX.AnimDataOffset;
                    curve.CurveType = (ResU.AnimCurveType)curveNX.CurveType;
                    curve.Delta = curveNX.Delta;
                    curve.EndFrame = curveNX.EndFrame;
                    curve.Frames = curveNX.Frames;
                    curve.Keys = curveNX.Keys;
                    curve.KeyStepBoolData = curveNX.KeyStepBoolData;
                    curve.KeyType = (ResU.AnimCurveKeyType)curveNX.KeyType;
                    curve.FrameType = (ResU.AnimCurveFrameType)curveNX.FrameType;
                    curve.StartFrame = curveNX.StartFrame;
                    curve.Scale = curveNX.Scale;
                    if (curve.Offset.GetType() == typeof(float))
                        curve.Offset = (float)curveNX.Offset;
                    if (curve.Offset.GetType() == typeof(uint))
                        curve.Offset = (uint)curveNX.Offset;
                    if (curve.Offset.GetType() == typeof(int))
                        curve.Offset = (int)curveNX.Offset;

                    boneAnimU.Curves.Add(curve);
                }
            }

            return ska;
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
                Nodes.Add(bone);

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
                        case (int)TrackType.XPOS: bone.XPOS.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.YPOS: bone.YPOS.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.ZPOS: bone.ZPOS.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.XROT: bone.XROT.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.YROT: bone.YROT.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.ZROT: bone.ZROT.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.WROT: bone.WROT.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.XSCA: bone.XSCA.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.YSCA: bone.YSCA.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.ZSCA: bone.ZSCA.Keys.AddRange(keyGroup.Keys); break;
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
                        case (int)TrackType.XPOS: bone.XPOS.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.YPOS: bone.YPOS.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.ZPOS: bone.ZPOS.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.XROT: bone.XROT.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.YROT: bone.YROT.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.ZROT: bone.ZROT.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.WROT: bone.WROT.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.XSCA: bone.XSCA.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.YSCA: bone.YSCA.Keys.AddRange(keyGroup.Keys); break;
                        case (int)TrackType.ZSCA: bone.ZSCA.Keys.AddRange(keyGroup.Keys); break;
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

        public void FromChr0(string FileName)
        {
            if (SkeletalAnimU != null)
            {
                var SkeletalAnimNX = BrawlboxHelper.FSKAConverter.Chr02Fska(FileName);
                SkeletalAnimU = ConvertSwitchToWiiU(SkeletalAnimNX);
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

        public void FromSeanim(string FileName)
        {
            SEAnim anim = SEAnim.Read(FileName);

            if (GetResFileU() != null)
            {
                SkeletalAnimU = new ResU.SkeletalAnim();
                SkeletalAnimU.FrameCount = anim.FrameCount;
                SkeletalAnimU.FlagsScale = ResU.SkeletalAnimFlagsScale.Maya;
                SkeletalAnimU.FlagsRotate = ResU.SkeletalAnimFlagsRotate.EulerXYZ;
                SkeletalAnimU.Loop = anim.Looping;
                SkeletalAnimU.Name = System.IO.Path.GetFileNameWithoutExtension(FileName);
                SkeletalAnimU.Path = "";
            }
            else
            {
                SkeletalAnim = new SkeletalAnim();
                SkeletalAnim.FrameCount = anim.FrameCount;
                SkeletalAnim.FlagsScale = SkeletalAnimFlagsScale.Maya;
                SkeletalAnim.FlagsRotate = SkeletalAnimFlagsRotate.EulerXYZ;
                SkeletalAnim.Loop = anim.Looping;
                SkeletalAnim.Name = System.IO.Path.GetFileNameWithoutExtension(FileName);
                SkeletalAnim.Path = "";
            }

            for (int i = 0; i < anim.Bones.Count; i++)
            {
                if (GetResFileU() != null)
                {
                   var BoneAnimU = new ResU.BoneAnim();
                    BoneAnimU.Name = anim.Bones[i];
                    SkeletalAnimU.BoneAnims.Add(BoneAnimU);
                    SkeletalAnimU.BindIndices[i] = ushort.MaxValue;
                    bool IsRoot = false;

                    if (!IsRoot)
                    {
                        BoneAnimU.ApplyIdentity = false;
                        BoneAnimU.ApplyRotateTranslateZero = false;
                        BoneAnimU.ApplyRotateZero = false;
                        BoneAnimU.ApplyScaleOne = true;
                        BoneAnimU.ApplyScaleVolumeOne = true;
                        BoneAnimU.ApplyScaleUniform = true;
                        BoneAnimU.ApplySegmentScaleCompensate = true;
                        BoneAnimU.ApplyTranslateZero = false;
                    }
                    else
                    {
                        BoneAnimU.ApplyIdentity = true;
                        BoneAnimU.ApplyRotateTranslateZero = true;
                        BoneAnimU.ApplyRotateZero = true;
                        BoneAnimU.ApplyScaleOne = true;
                        BoneAnimU.ApplyScaleVolumeOne = true;
                        BoneAnimU.ApplyScaleUniform = true;
                        BoneAnimU.ApplySegmentScaleCompensate = false;
                        BoneAnimU.ApplyTranslateZero = true;
                    }
                }
                else
                {
                    var BoneAnim = new BoneAnim();
                    BoneAnim.Name = anim.Bones[i];
                    SkeletalAnim.BoneAnims.Add(BoneAnim);
                    SkeletalAnim.BindIndices[i] = ushort.MaxValue;

                    //Set base data and curves
                    var baseData = new BoneAnimData();
                    if (anim.AnimationPositionKeys.ContainsKey(anim.Bones[i]) &&
                        anim.AnimationPositionKeys[anim.Bones[i]].Count > 0)
                    {
                        BoneAnim.FlagsBase |= BoneAnimFlagsBase.Translate;
                        var keys = anim.AnimationPositionKeys[anim.Bones[i]];
                        var data = (SELib.Utilities.Vector3)keys[0].Data;

                        baseData.Translate = new Syroot.Maths.Vector3F((float)data.X, (float)data.Y, (float)data.Z);

                        if (keys.Count > 1)
                        {
                            AnimCurve curve = new AnimCurve();
                            BoneAnim.Curves.Add(curve);
                            CreateCurveData(curve, keys);
                        }
                    }
                    if (anim.AnimationRotationKeys.ContainsKey(anim.Bones[i]) &&
                        anim.AnimationRotationKeys[anim.Bones[i]].Count > 0)
                    {
                        BoneAnim.FlagsBase |= BoneAnimFlagsBase.Rotate;
                        var keys = anim.AnimationPositionKeys[anim.Bones[i]];
                        var data = (SELib.Utilities.Quaternion)keys[0].Data;

                        baseData.Rotate = new Syroot.Maths.Vector4F((float)data.X, (float)data.Y, (float)data.Z, (float)data.W);

                        if (keys.Count > 1)
                        {
                            AnimCurve curve = new AnimCurve();
                            BoneAnim.Curves.Add(curve);
                            CreateCurveData(curve, keys);
                        }
                    }
                    if (anim.AnimationScaleKeys.ContainsKey(anim.Bones[i]) &&
                        anim.AnimationScaleKeys[anim.Bones[i]].Count > 0)
                    {
                        BoneAnim.FlagsBase |= BoneAnimFlagsBase.Scale;
                        var keys = anim.AnimationPositionKeys[anim.Bones[i]];
                        var data = (SELib.Utilities.Vector3)keys[0].Data;

                        baseData.Scale = new Syroot.Maths.Vector3F((float)data.X, (float)data.Y, (float)data.Z);

                        if (keys.Count > 1)
                        {
                            AnimCurve curve = new AnimCurve();
                            BoneAnim.Curves.Add(curve);
                            CreateCurveData(curve, keys);
                        }
                    }


                    //Set transforms
                    bool IsRoot = false;
                    if (!IsRoot)
                    {
                        BoneAnim.ApplyIdentity = false;
                        BoneAnim.ApplyRotateTranslateZero = false;
                        BoneAnim.ApplyRotateZero = false;
                        BoneAnim.ApplyScaleOne = true;
                        BoneAnim.ApplyScaleVolumeOne = true;
                        BoneAnim.ApplyScaleUniform = true;
                        BoneAnim.ApplySegmentScaleCompensate = true;
                        BoneAnim.ApplyTranslateZero = false;
                    }
                    else
                    {
                        BoneAnim.ApplyIdentity = true;
                        BoneAnim.ApplyRotateTranslateZero = true;
                        BoneAnim.ApplyRotateZero = true;
                        BoneAnim.ApplyScaleOne = true;
                        BoneAnim.ApplyScaleVolumeOne = true;
                        BoneAnim.ApplyScaleUniform = true;
                        BoneAnim.ApplySegmentScaleCompensate = false;
                        BoneAnim.ApplyTranslateZero = true;
                    }
                }
            }

            for (int frame = 0; frame < anim.FrameCount; frame++)
            {

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

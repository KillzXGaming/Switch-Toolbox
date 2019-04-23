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

        public override string ExportFilter => FileFilters.GetFilter(typeof(FSKA));
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
                foreach (var model in ((BFRES)Parent.Parent.Parent.Parent).BFRESRender.models)
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
                if (resFileU != null)
                {
                    SkeletalAnimU.Import(FileName, resFileU);
                    SkeletalAnimU.Name = Text;
                    LoadAnim(SkeletalAnimU);
                }
                else
                {
                    SkeletalAnim.Import(FileName);
                    SkeletalAnim.Name = Text;
                    LoadAnim(SkeletalAnim);
                }
            }
            else if (ext == ".seanim")
            {
                FromSeanim(FileName);
            }
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

            public ResU.BoneAnim SaveDataU()
            {
                BoneAnimU.Name = Text;

                return BoneAnimU;
            }

            public BoneAnim SaveData()
            {
                BoneAnim.Name = Text;

                return BoneAnim;
            }

            public BoneAnimNode(string bname, bool LoadContextMenus) : base(bname, LoadContextMenus)
            {

            }
        }
    }
}

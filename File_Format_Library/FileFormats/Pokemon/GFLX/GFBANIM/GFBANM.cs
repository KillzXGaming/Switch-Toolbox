using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using OpenTK;
using System.Reflection;
using Toolbox.Library.IO.FlatBuffer;
using Toolbox.Library.Animations;
using FirstPlugin.Forms;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class GFBANM : TreeNodeFile, IFileFormat, IAnimationContainer, IConvertableTextFormat
    {
        public STAnimation AnimationController => AnimationData;

        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "GFBANM" };
        public string[] Extension { get; set; } = new string[] { "*.gfbanm" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.GetExtension(FileName) == ".gfbanm";
        }

        public override void OnClick(TreeView treeView)
        {
            ViewportEditor editor = (ViewportEditor)LibraryGUI.GetActiveContent(typeof(ViewportEditor));
            if (editor == null)
            {
                editor = new ViewportEditor(true);
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }
        }

        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Json;
        public bool CanConvertBack => false;

        public string ConvertToString()
        {
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                    if (file.FileName == FileName)
                        return FlatBufferConverter.DeserializeToJson(new System.IO.MemoryStream(file.FileData), "gfbanm");
            }
            else
                return FlatBufferConverter.DeserializeToJson(FilePath, "gfbanm");

            return "";
        }

        public void ConvertFromString(string text)
        {
        }

        #endregion

        public Type[] Types
        {   
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        Animation AnimationData;
        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            Gfbanim.Animation anim = Gfbanim.Animation.GetRootAsAnimation(
                new FlatBuffers.ByteBuffer(stream.ToBytes()));

            AnimationData = new Animation();
            AnimationData.FrameCount = anim.AnimConfig.Value.KeyFrames;

            if (anim.Bones.HasValue) {
                for (int i = 0; i < anim.Bones.Value.BonesLength; i++)
                    AnimationData.LoadBoneGroup(anim.Bones.Value.Bones(i).Value);
            }
            if (anim.Materials.HasValue) {
                for (int i = 0; i < anim.Materials.Value.MaterialsLength; i++)
                    AnimationData.LoadMaterialGroup(anim.Materials.Value.Materials(i).Value);
            }
            if (anim.Groups.HasValue) {
                for (int i = 0; i < anim.Groups.Value.GroupsLength; i++)
                    AnimationData.LoadVisibilyGroup(anim.Groups.Value.Groups(i).Value);
            }
            if (anim.Triggers.HasValue) {
                for (int i = 0; i < anim.Triggers.Value.TriggersLength; i++)
                    AnimationData.LoadTriggerGroup(anim.Triggers.Value.Triggers(i).Value);
            }
        }   

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public class MeshAnimationController
        {
            public bool IsVisible = true;
        }

        public class Animation : STSkeletonAnimation
        {
            private GFBMDL ActiveModel
            {
                get
                {
                    GFBMDL model = null;
                    foreach (var container in ObjectEditor.GetDrawableContainers()) {
                        foreach (var drawable in container.Drawables)
                        {
                            if (!drawable.Visible)
                                continue;

                            if (drawable is GFBMDL_Render)
                                model = ((GFBMDL_Render)drawable).GfbmdlFile;
                        }
                    }
                    return model;
                }
            }

            public override STSkeleton GetActiveSkeleton()
            {
                if (ActiveModel == null) return null;

                return ActiveModel.Model.Skeleton;
            }

            public override void NextFrame()
            {
                if (Frame > FrameCount || ActiveModel == null) return;

                var skeleton = ActiveModel.Model.Skeleton;
                if (skeleton == null)
                    return;

                if (Frame == 0)
                    skeleton.reset();

                bool Updated = false; // no need to update skeleton of animations that didn't change
                foreach (var animGroup in AnimGroups)
                {
                    if (animGroup is VisibiltyGroup && ActiveModel != null)
                    {
                        var node = animGroup as VisibiltyGroup;
                        foreach (var mesh in ActiveModel.Model.GenericMeshes) {
                            if (animGroup.Name == mesh.Text)
                            {
                                var value = node.BooleanTrack.GetFrameValue(Frame);
                                mesh.AnimationController.IsVisible = value == 1; 
                            }
                        }
                    }
                    if (animGroup is MaterialGroup && ActiveModel != null)
                    {
                        foreach (var mat in ActiveModel.Model.GenericMaterials) {
                            if (animGroup.Name == mat.Text)
                            {

                            }
                        }
                    }
                    if (animGroup is BoneGroup)
                    {
                        var node = animGroup as BoneGroup;

                        STBone b = null;

                        b = skeleton.GetBone(node.Name);
                        if (b == null) continue;

                        Updated = true;

                        if (node.TranslateX.HasKeys)
                            b.pos.X = node.TranslateX.GetFrameValue(Frame);
                        if (node.TranslateY.HasKeys)
                            b.pos.Y = node.TranslateY.GetFrameValue(Frame);
                        if (node.TranslateZ.HasKeys)
                            b.pos.Z = node.TranslateZ.GetFrameValue(Frame);

                        if (node.ScaleX.HasKeys)
                            b.sca.X = node.ScaleX.GetFrameValue(Frame);
                        else b.sca.X = 1;
                        if (node.ScaleY.HasKeys)
                            b.sca.Y = node.ScaleY.GetFrameValue(Frame);
                        else b.sca.Y = 1;
                        if (node.ScaleZ.HasKeys)
                            b.sca.Z = node.ScaleZ.GetFrameValue(Frame);
                        else b.sca.Z = 1;

                        if (node.RotationX.HasKeys || node.RotationY.HasKeys || node.RotationZ.HasKeys)
                        {
                            STKeyFrame xL = GetFVGFL(node.RotationX, Frame);
                            STKeyFrame yL = GetFVGFL(node.RotationY, Frame);
                            STKeyFrame zL = GetFVGFL(node.RotationZ, Frame);
                            STKeyFrame xR = GetFVGFR(node.RotationX, Frame);
                            STKeyFrame yR = GetFVGFR(node.RotationY, Frame);
                            STKeyFrame zR = GetFVGFR(node.RotationZ, Frame);

                            short value1 = (short)xL.Value;
                            short value2 = (short)yL.Value;
                            short value3 = (short)zL.Value;

                            Quaternion rotL = PackedToQuat(value1, value2, value3);

                            short value1r = (short)xR.Value;
                            short value2r = (short)yR.Value;
                            short value3r = (short)zR.Value;

                            Quaternion rotR = PackedToQuat(value1r, value2r, value3r);

                            float weight = (Frame - xL.Frame) / (xR.Frame - xL.Frame);
                            if (xR.Frame - xL.Frame == 0)   //NaN if divided by zero
                            {
                                weight = 0;
                            }

                            b.rot = Quaternion.Slerp(rotL, rotR, weight);
                        }
                        else
                        {
                            b.rot = EulerToQuat(b.EulerRotation.Z, b.EulerRotation.Y, b.EulerRotation.X);
                        }
                    }
                }

                if (Updated)
                {
                    skeleton.update();
                }
            }

            public STKeyFrame GetFVGFL(STAnimationTrack t, float frame, float startFrame = 0)
            {
                if (t.KeyFrames.Count == 0) return new STKeyFrame(frame, 0);
                if (t.KeyFrames.Count == 1) return t.KeyFrames[0];

                STKeyFrame LK = t.KeyFrames.First();

                float Frame = frame - startFrame;

                foreach (STKeyFrame keyFrame in t.KeyFrames)
                {
                    if (keyFrame.Frame <= Frame) LK = keyFrame;
                }

                return LK;
            }

            public STKeyFrame GetFVGFR(STAnimationTrack t, float frame, float startFrame = 0)
            {
                if (t.KeyFrames.Count == 0) return new STKeyFrame(frame, 0);
                if (t.KeyFrames.Count == 1) return t.KeyFrames[0];

                STKeyFrame RK = t.KeyFrames.Last();

                float Frame = frame - startFrame;

                foreach (STKeyFrame keyFrame in t.KeyFrames)
                {
                    if (keyFrame.Frame >= Frame && keyFrame.Frame < RK.Frame) RK = keyFrame;
                }

                return RK;
            }

            // private static readonly ushort _flagsMask = 0b11000011_11111111;

            private static short UnpackS15(short u15)
            {
                int sign = (u15 >> 14) & 1;
                u15 &= 0x3FFF;
                if (sign == 0) u15 -= 0x4000;
                return u15;
            }

            static int[][] QUATERNION_SWIZZLES =  {
                  new int[] { 0, 3, 2, 1 }, new int[] { 3, 0, 2, 1 },
                  new int[] { 3, 2, 0, 1 }, new int[] { 3, 2, 1, 0 }
            };

            private static Quaternion PackedToQuat(short z, short y, short x)
            {

                const int count = 15;
                const int BASE = (1 << count) - 1;
                float maxval = 1 / (0x399E * (float)Math.Sqrt(2.0)); // such obvious, so constant, wow

                long cq = x & 0xFFFF;
                cq <<= 16;
                cq |= ((uint)y) & 0xFFFF;
                cq <<= 16;
                cq |= ((uint)z) & 0xFFFF;


                short extra = (short)(cq & 0x7);

                long num = cq >> 3;

                x = UnpackS15((short)((num >> (count * 2)) & BASE));
                y = UnpackS15((short)((num >> (count * 1)) & BASE));
                z = UnpackS15((short)((num >> (count * 0)) & BASE));

                float fx = x * maxval;
                float fy = y * maxval;
                float fz = z * maxval;

                float[] quat = {
                    (float)Math.Sqrt(1 - fx * fx - fy * fy - fz * fz),
                    fx,
                    fy,
                    fz };

                int[] qmap = QUATERNION_SWIZZLES[(extra & 3)];
                Quaternion q = new Quaternion(quat[qmap[0]], quat[qmap[1]], quat[qmap[2]], quat[qmap[3]]);
                if ((extra >> 2) != 0) q *= -1;

                return q;

            }

            public static Quaternion EulerToQuat(float z, float y, float x)
            {
                Quaternion xRotation = Quaternion.FromAxisAngle(Vector3.UnitX, x);
                Quaternion yRotation = Quaternion.FromAxisAngle(Vector3.UnitY, y);
                Quaternion zRotation = Quaternion.FromAxisAngle(Vector3.UnitZ, z);
                return (zRotation * yRotation * xRotation);
            }
                
            public void LoadBoneGroup(Gfbanim.Bone boneAnim)
            {
                BoneGroup groupAnim = new BoneGroup();
                groupAnim.Name = boneAnim.Name;
                AnimGroups.Add(groupAnim);

                //Tracks use 3 types
                // Fixed/constant
                // Dynamic (baked and multiple keys, no frames)
                // Framed (multiple keys and frames)

                List<float> Frames = new List<float>();

                switch (boneAnim.RotateType)
                {
                    case Gfbanim.QuatTrack.DynamicQuatTrack:
                        {
                            var rotate = boneAnim.Rotate<Gfbanim.DynamicQuatTrack>();
                            if (rotate.HasValue)
                            {
                                var values = GfbanimKeyFrameLoader.LoadRotationTrack(rotate.Value);
                                groupAnim.RotationX = values[0];
                                groupAnim.RotationY = values[1];
                                groupAnim.RotationZ = values[2];
                            }
                        }
                        break;
                    case Gfbanim.QuatTrack.FixedQuatTrack:
                        {
                            var rotate = boneAnim.Rotate<Gfbanim.FixedQuatTrack>();
                            if (rotate.HasValue)
                            {
                                var vec = rotate.Value.Value.Value;
                                groupAnim.RotationX.KeyFrames.Add(new STKeyFrame(0, GfbanimKeyFrameLoader.ConvertRotation(vec.X)));
                                groupAnim.RotationY.KeyFrames.Add(new STKeyFrame(0, GfbanimKeyFrameLoader.ConvertRotation(vec.Y)));
                                groupAnim.RotationZ.KeyFrames.Add(new STKeyFrame(0, GfbanimKeyFrameLoader.ConvertRotation(vec.Z)));
                            }
                        }
                        break;
                    case Gfbanim.QuatTrack.FramedQuatTrack16:
                        {
                            var rotate = boneAnim.Rotate<Gfbanim.FramedQuatTrack16>();
                            if (rotate.HasValue)
                            {
                                var values = GfbanimKeyFrameLoader.LoadRotationTrack(rotate.Value);
                                groupAnim.RotationX = values[0];
                                groupAnim.RotationY = values[1];
                                groupAnim.RotationZ = values[2];
                            }
                        }
                        break;
                    case Gfbanim.QuatTrack.FramedQuatTrack8:
                        {
                            var rotate = boneAnim.Rotate<Gfbanim.FramedQuatTrack8>();
                            if (rotate.HasValue)
                            {
                                var values = GfbanimKeyFrameLoader.LoadRotationTrack(rotate.Value);
                                groupAnim.RotationX = values[0];
                                groupAnim.RotationY = values[1];
                                groupAnim.RotationZ = values[2];
                            }
                        }
                        break;
                }
                switch (boneAnim.ScaleType)
                {
                    case Gfbanim.VectorTrack.DynamicVectorTrack:
                        {
                            var scale = boneAnim.Scale<Gfbanim.DynamicVectorTrack>();
                            if (scale.HasValue)
                            {
                                var values = GfbanimKeyFrameLoader.LoadVectorTrack(scale.Value);
                                groupAnim.ScaleX = values[0];
                                groupAnim.ScaleY = values[1];
                                groupAnim.ScaleZ = values[2];
                            }
                        }
                        break;
                    case Gfbanim.VectorTrack.FramedVectorTrack16:
                        {
                            var scale = boneAnim.Scale<Gfbanim.FramedVectorTrack16>();
                            if (scale.HasValue)
                            {
                                var values = GfbanimKeyFrameLoader.LoadVectorTrack(scale.Value);
                                groupAnim.ScaleX = values[0];
                                groupAnim.ScaleY = values[1];
                                groupAnim.ScaleZ = values[2];
                            }
                        }
                        break;
                    case Gfbanim.VectorTrack.FramedVectorTrack8:
                        {
                            var scale = boneAnim.Scale<Gfbanim.FramedVectorTrack8>();
                            if (scale.HasValue)
                            {
                                var values = GfbanimKeyFrameLoader.LoadVectorTrack(scale.Value);
                                groupAnim.ScaleX = values[0];
                                groupAnim.ScaleY = values[1];
                                groupAnim.ScaleZ = values[2];
                            }
                        }
                        break;
                    case Gfbanim.VectorTrack.FixedVectorTrack:
                        {
                            var scale = boneAnim.Scale<Gfbanim.FixedVectorTrack>();
                            if (scale.HasValue)
                            {
                                var vec = scale.Value.Value.Value;
                                groupAnim.ScaleX.KeyFrames.Add(new STKeyFrame(0, vec.X));
                                groupAnim.ScaleY.KeyFrames.Add(new STKeyFrame(0, vec.Y));
                                groupAnim.ScaleZ.KeyFrames.Add(new STKeyFrame(0, vec.Z));
                            }
                        }
                        break;
                }
                switch (boneAnim.TranslateType)
                {
                    case Gfbanim.VectorTrack.DynamicVectorTrack:
                        {
                            var trans = boneAnim.Translate<Gfbanim.DynamicVectorTrack>();
                            if (trans.HasValue)
                            {
                                var values = GfbanimKeyFrameLoader.LoadVectorTrack(trans.Value);
                                groupAnim.TranslateX = values[0];
                                groupAnim.TranslateY = values[1];
                                groupAnim.TranslateZ = values[2];
                            }
                        }
                        break;
                    case Gfbanim.VectorTrack.FramedVectorTrack16:
                        {
                            var trans = boneAnim.Translate<Gfbanim.FramedVectorTrack16>();
                            if (trans.HasValue)
                            {
                                var values = GfbanimKeyFrameLoader.LoadVectorTrack(trans.Value);
                                groupAnim.TranslateX = values[0];
                                groupAnim.TranslateY = values[1];
                                groupAnim.TranslateZ = values[2];
                            }
                        }
                        break;
                    case Gfbanim.VectorTrack.FramedVectorTrack8:
                        {
                            var trans = boneAnim.Translate<Gfbanim.FramedVectorTrack8>();
                            if (trans.HasValue)
                            {
                                var values = GfbanimKeyFrameLoader.LoadVectorTrack(trans.Value);
                                groupAnim.TranslateX = values[0];
                                groupAnim.TranslateY = values[1];
                                groupAnim.TranslateZ = values[2];
                            }
                        }
                        break;
                    case Gfbanim.VectorTrack.FixedVectorTrack:
                        {
                            var trans = boneAnim.Translate<Gfbanim.FixedVectorTrack>();
                            if (trans.HasValue)
                            {
                                var vec = trans.Value.Value.Value;
                                groupAnim.TranslateX.KeyFrames.Add(new STKeyFrame(0, vec.X));
                                groupAnim.TranslateY.KeyFrames.Add(new STKeyFrame(0, vec.Y));
                                groupAnim.TranslateZ.KeyFrames.Add(new STKeyFrame(0, vec.Z));
                            }
                        }
                        break;
                }
            }

            public void LoadMaterialGroup(Gfbanim.Material matAnim)
            {
                for (int i = 0; i < matAnim.SwitchesLength; i++)
                {
                    var switchTrack = matAnim.Switches(i).Value;
                  
                }
                for (int i = 0; i < matAnim.ValuesLength; i++)
                {
                    var valueTrack = matAnim.Values(i).Value;

                }
                for (int i = 0; i < matAnim.VectorsLength; i++)
                {
                    var vectorTrack = matAnim.Vectors(i).Value;

                }
            }

            public void LoadVisibilyGroup(Gfbanim.Group visAnim)
            {
                VisibiltyGroup groupAnim = new VisibiltyGroup();
                groupAnim.Name = visAnim.Name;
                AnimGroups.Add(groupAnim);

                if (visAnim.ValueType == Gfbanim.BooleanTrack.FixedBooleanTrack) {
                    var track = visAnim.Value<Gfbanim.FixedBooleanTrack>();
                    if (track.HasValue)
                    {
                        var animTrack = new STAnimationTrack();
                        animTrack.KeyFrames.Add(new STKeyFrame(0, track.Value.Value));
                        groupAnim.BooleanTrack = animTrack;
                    }
                }
                else if (visAnim.ValueType == Gfbanim.BooleanTrack.DynamicBooleanTrack) {
                    var track = visAnim.Value<Gfbanim.DynamicBooleanTrack>();
                    if (track.HasValue) {
                        groupAnim.BooleanTrack = GfbanimKeyFrameLoader.LoadBooleanTrack(track.Value);
                    }
                }
                else if(visAnim.ValueType == Gfbanim.BooleanTrack.FramedBooleanTrack) {
                    var track = visAnim.Value<Gfbanim.FramedBooleanTrack>();
                    if (track.HasValue)
                    {
                        groupAnim.BooleanTrack = GfbanimKeyFrameLoader.LoadBooleanTrack(track.Value);
                    }
                }
            }

            public void LoadTriggerGroup(Gfbanim.Trigger triggerAnim)
            {

            }

            private STAnimationTrack LoadTrack(float[] Frames, float[] Values)
            {
                STAnimationTrack track = new STAnimationTrack();
                track.InterpolationType = STInterpoaltionType.Linear;
                for (int i = 0; i < Values?.Length; i++)
                {
                    STKeyFrame keyFrame = new STKeyFrame();
                    keyFrame.Value = Values[i];
                    keyFrame.Value = Frames[i];
                    track.KeyFrames.Add(keyFrame);
                }
                return track;
            }
        }

        public class BoneGroup : STAnimGroup
        {
            public STAnimationTrack TranslateX = new STAnimationTrack();
            public STAnimationTrack TranslateY = new STAnimationTrack();
            public STAnimationTrack TranslateZ = new STAnimationTrack();

            public STAnimationTrack RotationX = new STAnimationTrack();
            public STAnimationTrack RotationY = new STAnimationTrack();
            public STAnimationTrack RotationZ = new STAnimationTrack();
            public STAnimationTrack RotationW = new STAnimationTrack();

            public STAnimationTrack ScaleX = new STAnimationTrack();
            public STAnimationTrack ScaleY = new STAnimationTrack();
            public STAnimationTrack ScaleZ = new STAnimationTrack();

            public override List<STAnimationTrack> GetTracks()
            {
                List<STAnimationTrack> anims = new List<STAnimationTrack>();
                anims.Add(TranslateX); anims.Add(TranslateY); anims.Add(TranslateZ);
                anims.Add(RotationX); anims.Add(RotationY); anims.Add(RotationZ);
                anims.Add(ScaleX); anims.Add(ScaleY); anims.Add(ScaleZ);
                return anims;
            }
        }

        public class MaterialGroup : STAnimGroup
        {

        }

        public class MaterialSwitchGroup : STAnimGroup
        {
            public STAnimationTrack SwitchTrack = new STAnimationTrack();

            public override List<STAnimationTrack> GetTracks() {
                return new List<STAnimationTrack>() { SwitchTrack };
            }
        }

        public class MaterialValueGroup : STAnimGroup
        {
            public STAnimationTrack ValueTrack = new STAnimationTrack();

            public override List<STAnimationTrack> GetTracks() {
                return new List<STAnimationTrack>() { ValueTrack };
            }
        }

        public class MaterialVectorGroup : STAnimGroup
        {
            public STAnimationTrack VectorX = new STAnimationTrack();
            public STAnimationTrack VectorY = new STAnimationTrack();
            public STAnimationTrack VectorZ = new STAnimationTrack();

            public override List<STAnimationTrack> GetTracks()
            {
                return new List<STAnimationTrack>()
                { VectorX, VectorY, VectorZ };
            }
        }

        public class VisibiltyGroup : STAnimGroup
        {
            public STAnimationTrack BooleanTrack = new STAnimationTrack();

            public override List<STAnimationTrack> GetTracks() {
                return new List<STAnimationTrack>() { BooleanTrack };
            }
        }

        public class TriggerGroup : STAnimGroup
        {

        }
    }
}

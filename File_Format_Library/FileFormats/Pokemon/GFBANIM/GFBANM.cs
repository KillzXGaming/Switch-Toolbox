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

namespace FirstPlugin
{
    public class GFBANM : TreeNodeFile, IFileFormat, IAnimationContainer
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

        public class Animation : STSkeletonAnimation
        {
            public override void NextFrame()
            {
                if (Frame > FrameCount) return;

                var skeleton = GetActiveSkeleton();
                if (skeleton == null) return;

                if (Frame == 0)
                    skeleton.reset();

                bool Updated = false; // no need to update skeleton of animations that didn't change
                foreach (var animGroup in AnimGroups)
                {
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
                            float x = node.RotationX.HasKeys ? node.RotationX.GetFrameValue(Frame) : b.rotation[0];
                            float y = node.RotationY.HasKeys ? node.RotationY.GetFrameValue(Frame) : b.rotation[1];
                            float z = node.RotationZ.HasKeys ? node.RotationZ.GetFrameValue(Frame) : b.rotation[2];

                            var Rotation = new Vector3(x / 0xffff, y/ 0xffff, z / 0xffff).Normalized();
                            b.rot = EulerToQuat(Rotation.Z, Rotation.Y, Rotation.X);

                            //   b.rot = EulerToQuat(Rotation.Z, Rotation.Y, Rotation.X);

                            float Angle = Rotation.Length;
                            Console.WriteLine($"{node.Name} Angle {Angle}");

                        //     b.rot = Angle > 0
                        //        ? Quaternion.FromAxisAngle(Vector3.Normalize(Rotation), Angle)
                        //        : Quaternion.Identity;
                        }
                        else
                        {
                            b.rot = EulerToQuat(b.rotation[2], b.rotation[1], b.rotation[0]);
                        }
                    }
                }

                if (Updated)
                {
                    skeleton.update();
                }
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

                Console.WriteLine($"BoneGroup {groupAnim.Name }");

                switch (boneAnim.RotateType)
                {
                    case Gfbanim.QuatTrack.DynamicQuatTrack:
                        {
                            var rotate = boneAnim.Rotate<Gfbanim.DynamicQuatTrack>();
                            if (rotate.HasValue)
                            {
                                var values = LoadRotationTrack(rotate.Value);
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
                                groupAnim.RotationX.KeyFrames.Add(new STKeyFrame(0, ConvertRotation(vec.X)));
                                groupAnim.RotationY.KeyFrames.Add(new STKeyFrame(0, ConvertRotation(vec.Y)));
                                groupAnim.RotationZ.KeyFrames.Add(new STKeyFrame(0, ConvertRotation(vec.Z)));
                            }
                        }
                        break;
                    case Gfbanim.QuatTrack.FramedQuatTrack:
                        {
                            var rotate = boneAnim.Rotate<Gfbanim.FramedQuatTrack>();
                            if (rotate.HasValue)
                            {
                                var values = LoadRotationTrack(rotate.Value);
                                groupAnim.RotationX = values[0];
                                groupAnim.RotationY = values[1];
                                groupAnim.RotationZ = values[2];
                            }
                        }
                        break;
                }
                switch (boneAnim.ScaleType)
                {
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
                switch (boneAnim.ScaleType)
                {
                    case Gfbanim.VectorTrack.DynamicVectorTrack:
                        {
                            var scale = boneAnim.Scale<Gfbanim.DynamicVectorTrack>();
                            if (scale.HasValue)
                            {
                                var values = LoadVectorTrack(scale.Value);
                                groupAnim.ScaleX = values[0];
                                groupAnim.ScaleY = values[1];
                                groupAnim.ScaleZ = values[2];
                            }
                        }
                        break;
                    case Gfbanim.VectorTrack.FramedVectorTrack:
                        {
                            var scale = boneAnim.Scale<Gfbanim.FramedVectorTrack>();
                            if (scale.HasValue)
                            {
                                var values = LoadVectorTrack(scale.Value);
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
                                var values = LoadVectorTrack(trans.Value);
                                groupAnim.TranslateX = values[0];
                                groupAnim.TranslateY = values[1];
                                groupAnim.TranslateZ = values[2];
                            }
                        }
                        break;
                    case Gfbanim.VectorTrack.FramedVectorTrack:
                        {
                            var trans = boneAnim.Translate<Gfbanim.FramedVectorTrack>();
                            if (trans.HasValue)
                            {
                                var values = LoadVectorTrack(trans.Value);
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

            public float ConvertRotation(ushort val)
            {
                return val;
            }

            public STAnimationTrack[] LoadRotationTrack(Gfbanim.DynamicQuatTrack dynamicTrack)
            {
                STAnimationTrack[] tracks = new STAnimationTrack[3];
                tracks[0] = new STAnimationTrack(STInterpoaltionType.Linear);
                tracks[1] = new STAnimationTrack(STInterpoaltionType.Linear);
                tracks[2] = new STAnimationTrack(STInterpoaltionType.Linear);

                for (int i = 0; i < dynamicTrack.ValuesLength; i++)
                {
                    var quat = dynamicTrack.Values(i).Value;
                    tracks[0].KeyFrames.Add(new STKeyFrame(i, ConvertRotation(quat.X)));
                    tracks[1].KeyFrames.Add(new STKeyFrame(i, ConvertRotation(quat.Y)));
                    tracks[2].KeyFrames.Add(new STKeyFrame(i, ConvertRotation(quat.Z)));

                    Console.WriteLine($"{i} { ConvertRotation(quat.X)} { ConvertRotation(quat.Y)} {ConvertRotation(quat.Z)}");
                }
                return tracks;
            }

            public STAnimationTrack[] LoadVectorTrack(Gfbanim.DynamicVectorTrack dynamicTrack)
            {
                STAnimationTrack[] tracks = new STAnimationTrack[3];
                tracks[0] = new STAnimationTrack(STInterpoaltionType.Linear);
                tracks[1] = new STAnimationTrack(STInterpoaltionType.Linear);
                tracks[2] = new STAnimationTrack(STInterpoaltionType.Linear);

                for (int i = 0; i < dynamicTrack.ValuesLength; i++)
                {
                    var vec = dynamicTrack.Values(i).Value;
                    tracks[0].KeyFrames.Add(new STKeyFrame(i, vec.X));
                    tracks[1].KeyFrames.Add(new STKeyFrame(i, vec.Y));
                    tracks[2].KeyFrames.Add(new STKeyFrame(i, vec.Z));
                }
                return tracks;
            }

            public STAnimationTrack[] LoadVectorTrack(Gfbanim.FramedVectorTrack framedTrack)
            {
                ushort[] frames = framedTrack.GetFramesArray(); 

                STAnimationTrack[] tracks = new STAnimationTrack[3];
                tracks[0] = new STAnimationTrack(STInterpoaltionType.Linear);
                tracks[1] = new STAnimationTrack(STInterpoaltionType.Linear);
                tracks[2] = new STAnimationTrack(STInterpoaltionType.Linear);

                for (int i = 0; i < framedTrack.ValuesLength; i++)
                {
                    var vec = framedTrack.Values(i).Value;
                    int frame = i;

                    if (i < frames?.Length) frame = frames[i];

                    tracks[0].KeyFrames.Add(new STKeyFrame(frame, vec.X));
                    tracks[1].KeyFrames.Add(new STKeyFrame(frame, vec.Y));
                    tracks[2].KeyFrames.Add(new STKeyFrame(frame, vec.Z));
                }
                return tracks;
            }

            public STAnimationTrack[] LoadRotationTrack(Gfbanim.FramedQuatTrack framedTrack)
            {
                ushort[] frames = framedTrack.GetFramesArray();

                STAnimationTrack[] tracks = new STAnimationTrack[3];
                tracks[0] = new STAnimationTrack(STInterpoaltionType.Linear);
                tracks[1] = new STAnimationTrack(STInterpoaltionType.Linear);
                tracks[2] = new STAnimationTrack(STInterpoaltionType.Linear);

                for (int i = 0; i < framedTrack.ValuesLength; i++)
                {
                    var quat = framedTrack.Values(i).Value;
                    int frame = i;

                    if (i < frames?.Length) frame = frames[i];

                    tracks[0].KeyFrames.Add(new STKeyFrame(frame, ConvertRotation(quat.X)));
                    tracks[1].KeyFrames.Add(new STKeyFrame(frame, ConvertRotation(quat.Y)));
                    tracks[2].KeyFrames.Add(new STKeyFrame(frame, ConvertRotation(quat.Z)));
                }
                return tracks;
            }

            public void LoadMaterialGroup(Gfbanim.Material matAnim)
            {

            }

            public void LoadVisibilyGroup(Gfbanim.Group visAnim)
            {

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

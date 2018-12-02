using System;
using System.Collections.Generic;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using ResU = Syroot.NintenTools.Bfres;
using FirstPlugin;
using OpenTK;

namespace Bfres.Structs
{
    public class FskaFolder : AnimationGroupNode
    {
        public FskaFolder()
        {
            Text = "Skeleton Animations";
            Name = "FSKA";

            ContextMenu = new ContextMenu();
            MenuItem import = new MenuItem("Import");
            ContextMenu.MenuItems.Add(import);
            import.Click += Import;
            MenuItem exportAll = new MenuItem("Export All");
            ContextMenu.MenuItems.Add(exportAll);
            exportAll.Click += ExportAll;
            MenuItem clear = new MenuItem("Clear");
            ContextMenu.MenuItems.Add(clear);
            clear.Click += Clear;
        }
        public void Import(object sender, EventArgs args)
        {

        }
        public void ExportAll(object sender, EventArgs args)
        {
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = sfd.SelectedPath;
                foreach (BfresSkeletonAnim fska in Nodes)
                {
                    string FileName = folderPath + '\\' + fska.Text + ".bfska";
                    ((BfresSkeletonAnim)fska).SkeletalAnim.Export(FileName, fska.GetResFile());
                }
            }
        }
        private void Clear(object sender, EventArgs args)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all skeletal animations? This cannot be undone!", "", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Nodes.Clear();
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
        public ResFile GetResFile()
        {
            //ResourceFile -> FMDL -> Material Folder -> this
            return ((BFRES)Parent.Parent).resFile;
        }
        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.bfska;";
            sfd.FileName = Text;
            sfd.DefaultExt = ".bfska";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SkeletalAnim.Export(sfd.FileName, GetResFile());
            }
        }
        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfska;";

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
}

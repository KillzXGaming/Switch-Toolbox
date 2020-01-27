using System;
using System.Collections.Generic;
using Toolbox.Library;
using System.Windows.Forms;
using FirstPlugin;
using Syroot.NintenTools.NSW.Bfres;
using FirstPlugin.Forms;
using Toolbox.Library.Animations;
using ResU = Syroot.NintenTools.Bfres;

namespace Bfres.Structs
{
    public class FVIS : VisibilityAnimation
    {
        public VisibilityAnim VisibilityAnim;
        public ResU.VisibilityAnim VisibilityAnimU;

        public FVIS()
        {

        }

        private void Initialize()
        {
            ImageKey = "visibilityAnim";
            SelectedImageKey = "visibilityAnim";

            CanRename = true;
            CanReplace = true;
            CanExport = true;
            CanDelete = true;
        }

        public ResFile GetResFile() {
            return ((BFRESGroupNode)Parent).GetResFile();
        }

        public ResU.ResFile GetResFileU() {
            return ((BFRESGroupNode)Parent).GetResFileU();
        }

        public override void OnClick(TreeView treeView) => UpdateEditor();

        public void UpdateEditor(){
            ((BFRES)Parent?.Parent?.Parent)?.LoadEditors(this);
        }

        public override string ExportFilter => FileFilters.GetFilter(typeof(FVIS));

        public override void Export(string FileName)
        {
            if (VisibilityAnim != null)
                VisibilityAnim.Export(FileName, GetResFile());
            else
                VisibilityAnimU.Export(FileName, GetResFileU());
        }

        public override void Replace(string FileName)
        {
            Replace(FileName, GetResFile(), GetResFileU());
        }

        public void Replace(string FileName, ResFile resFileNX, ResU.ResFile resFileU)
        {
            if (resFileNX != null)
            {
                VisibilityAnim.Import(FileName);
                VisibilityAnim.Name = Text;
            }
            else
            {
                VisibilityAnimU.Import(FileName, resFileU);
                VisibilityAnimU.Name = Text;
            }
        }

        public void SaveAnimData()
        {
            if (VisibilityAnimU != null)
            {
                VisibilityAnimU.Name = Text;
            }
            else
            {
                VisibilityAnim.Name = Text;
            }

            SaveData();
        }

        public void SaveData()
        {
            if (!IsEdited) //Use original data instead of generic data from editors
                return;

            if (VisibilityAnimU != null)
            {
                VisibilityAnimU.Names = BoneNames;
                VisibilityAnimU.Path = "";

            }
            else
            {
                VisibilityAnim.Names = BoneNames;
                VisibilityAnim.Path = "";
                VisibilityAnim.FrameCount = FrameCount;
                VisibilityAnim.BaseDataList = new bool[BoneNames.Count];

                int boneIndex = 0;
                foreach (BooleanKeyGroup value in Values)
                {
                    if (!value.Constant)
                    {
                        AnimCurve curve = new AnimCurve();
                        curve.AnimDataOffset = (uint)BoneNames.IndexOf(value.Text);
                        curve.Scale = value.Scale;
                        curve.Offset = value.Offset;
                        curve.KeyType = AnimCurveKeyType.SByte;
                        curve.FrameType = CurveHelper.GetFrameType((uint)FrameCount);
                        curve.CurveType = AnimCurveType.StepBool;

                        if (IsBaked)
                            curve.CurveType = AnimCurveType.BakedBool;

                        curve.Delta = value.Delta;
                        curve.EndFrame = value.EndFrame;
                        curve.StartFrame = value.StartFrame;

                        List<bool> KeyBooleans = new List<bool>();
                        List<float> KeyFrames = new List<float>();

                        for (int frame = 0; frame < value.Keys.Count; frame++)
                        {
                            bool currentValue = value.Keys[frame].Visible;
                            float currentFrame = value.Keys[frame].Frame;

                            if (frame > 0)
                            {
                                bool previousValue = value.Keys[frame - 1].Visible;
                                if (previousValue != currentValue)
                                {
                                    KeyFrames.Add(currentFrame);
                                    KeyBooleans.Add(currentValue);
                                }
                            }
                            else
                            {
                                KeyFrames.Add(currentFrame);
                                KeyBooleans.Add(currentValue);
                                VisibilityAnim.BaseDataList[boneIndex] = currentValue;
                            }
                        }

                        curve.KeyStepBoolData = KeyBooleans.ToArray();

                        for (int frame = 0; frame < FrameCount; frame++)
                        {
                        }
                    }
                    else
                    {
                        //Else for constant types it's only base values
                    }

                    boneIndex++;
                }


            }
        }

        public FVIS(ResU.VisibilityAnim anim) { Initialize(); LoadAnim(anim); }
        public FVIS(VisibilityAnim anim) { Initialize(); LoadAnim(anim); }

        private void LoadAnim(ResU.VisibilityAnim vis)
        {
            VisibilityAnimU = vis;
            FrameCount = vis.FrameCount;

            Text = vis.Name;

            if (vis.BaseDataList == null)
                vis.BaseDataList = new bool[0];

            if (vis.Names == null)
                vis.Names = new string[0];

            BaseValues = vis.BaseDataList;

            foreach (var name in vis.Names)
                BoneNames.Add(name);

            for (int curve = 0; curve < vis.Curves.Count; curve++)
            {
                Values.Add(CurveHelper.CreateBooleanTrackWiiU(vis.Curves[curve]));
            }
        }

        private void LoadAnim(VisibilityAnim vis)
        {
            VisibilityAnim = vis;
            FrameCount = vis.FrameCount;

            Text = vis.Name;

            if (vis.BaseDataList == null)
                vis.BaseDataList = new bool[0];

            if (vis.Names == null)
                vis.Names = new string[0];

            BaseValues = vis.BaseDataList;

            foreach (var name in vis.Names)
                BoneNames.Add(name);

            for (int curve = 0; curve < vis.Curves.Count; curve++)
            {
                var track = CurveHelper.CreateBooleanTrack(vis.Curves[curve]);
                track.Text = BoneNames[(int)track.AnimDataOffset];
                Values.Add(track);
            }
        }

        public override void NextFrame(Viewport viewport)
        {
            if (viewport.scene == null)
                return;

            //Loop through each drawable bfres in the active viewport to display the anim
            foreach (var drawable in viewport.scene.staticObjects)
            {
                if (drawable is BFRESRender)
                    HideBones(((BFRESRender)drawable).models);
            }
        }

        private void HideBones(List<FMDL> models)
        {
            foreach (var model in models)
            {
                if (Frame == 0)
                {
                    foreach (var bone in model.Skeleton.bones)
                            bone.Visible = true;

                    //Hide base values
                    int index = 0;
                    foreach (var baseVal in BaseValues)
                    {
                        string boneName = BoneNames[index++];
                        foreach (var bone in model.Skeleton.bones)
                            if (bone.Text == boneName)
                                bone.Visible = baseVal;
                    }
                }

                //Hide values for each curve
                foreach (var group in Values)
                {
                    string boneName = group.Text;
                    foreach (var bone in model.Skeleton.bones)
                    {
                        if (bone.Text == boneName)
                        {
                            bone.Visible = group.GetValue(Frame);

                            Console.WriteLine($"{Frame} {bone.Text} {bone.Visible}");
                        }
                    }
                }
            }
        }
    }
}

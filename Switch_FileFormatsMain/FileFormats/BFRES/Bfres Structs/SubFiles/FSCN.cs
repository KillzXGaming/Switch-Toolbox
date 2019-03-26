using System;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using FirstPlugin.Forms;
using FirstPlugin;
using Syroot.NintenTools.NSW.Bfres;
using ResU = Syroot.NintenTools.Bfres;

using Switch_Toolbox.Library.Animations;
using Switch_Toolbox.Library.NodeWrappers;

namespace Bfres.Structs
{
    public class FSCN : STGenericWrapper
    {
        public SceneAnim SceneAnim;
        public ResU.SceneAnim SceneAnimU;

        public FSCN()
        {
            Initialize();
        }

        public void Initialize()
        {
            CanRename = true;
            CanReplace = true;
            CanExport = true;
            CanDelete = true;

            ImageKey = "materialAnim";
            SelectedImageKey = "materialAnim";

            ContextMenuStrip = new ContextMenuStrip();
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("New Camera Animation", null, NewCameraAction, Keys.Control | Keys.C));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("New Light Animation", null, NewLightAction, Keys.Control | Keys.L));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("New Fog Animation", null, NewFogAction, Keys.Control | Keys.F));
            LoadFileMenus(false);
        }

        protected void NewCameraAction(object sender, EventArgs e) { NewCameraAnim(); }
        protected void NewLightAction(object sender, EventArgs e) { NewLightAnim(); }
        protected void NewFogAction(object sender, EventArgs e) { NewFogAnim(); }


        public void NewCameraAnim()
        {
            BfresCameraAnim cameraAnim = new BfresCameraAnim();
            Nodes.Add(cameraAnim);
        }

        public void NewLightAnim()
        {
            BfresLightAnim lightAnim = new BfresLightAnim();
            Nodes.Add(lightAnim);
        }

        public void NewFogAnim()
        {
            BfresFogAnim fogAnim = new BfresFogAnim();
            Nodes.Add(fogAnim);
        }

        public ResFile GetResFile() {
            return ((BFRESGroupNode)Parent).GetResFile();
        }

        public ResU.ResFile GetResFileU() {
            return ((BFRESGroupNode)Parent).GetResFileU();
        }

        public override void OnClick(TreeView treeView) => UpdateEditor();

        public void UpdateEditor(){
            ((BFRES)Parent.Parent.Parent).LoadEditors(this);
        }

        public override void Export(string FileName)
        {
            SceneAnim.Export(FileName, ((BFRESGroupNode)Parent).GetResFile());
        }


        public override void Replace(string FileName) {
            Replace(FileName, GetResFile(), GetResFileU());
        }

        public void Replace(string FileName, ResFile resFileNX, ResU.ResFile resFileU)
        {
            if (resFileNX != null)
            {
                SceneAnim.Import(FileName);
                SceneAnim.Name = Text;
            }
            else
            {
                SceneAnimU.Import(FileName, resFileU);
                SceneAnimU.Name = Text;
            }
        }

        public FSCN(ResU.SceneAnim scn) { Initialize(); LoadAnim(scn); }
        public FSCN(SceneAnim scn) { Initialize(); LoadAnim(scn); }

        private void LoadAnim(ResU.SceneAnim scn)
        {
            Text = scn.Name;

            SceneAnimU = scn;

            foreach (var cameraAnim in scn.CameraAnims.Values)
            {
                BfresCameraAnim anim = new BfresCameraAnim();
                anim.LoadAnim(cameraAnim);
                Nodes.Add(anim);
            }
            foreach (var lightAnim in scn.LightAnims.Values)
            {
                BfresLightAnim anim = new BfresLightAnim();
                anim.LoadAnim(lightAnim);
                Nodes.Add(anim);
            }
            foreach (var fogAnim in scn.FogAnims.Values)
            {
                BfresFogAnim anim = new BfresFogAnim();
                anim.LoadAnim(fogAnim);
                Nodes.Add(anim);
            }
        }
        private void LoadAnim(SceneAnim scn)
        {
            Text = scn.Name;

            SceneAnim = scn;

            foreach (var cameraAnim in scn.CameraAnims)
            {
                BfresCameraAnim anim = new BfresCameraAnim();
                anim.LoadAnim(cameraAnim);
                Nodes.Add(anim);
            }
            foreach (var lightAnim in scn.LightAnims)
            {
                BfresLightAnim anim = new BfresLightAnim();
                anim.LoadAnim(lightAnim);
                Nodes.Add(anim);
            }
            foreach (var fogAnim in scn.FogAnims)
            {
                BfresFogAnim anim = new BfresFogAnim();
                anim.LoadAnim(fogAnim);
                Nodes.Add(anim);
            }
        }

        public class BfresCameraAnim : CameraAnimation
        {
            public CameraAnim CameraAnim;
            public ResU.CameraAnim CameraAnimU;

            public void Initialize()
            {
                CanRename = true;
                CanReplace = true;
                CanExport = true;
                CanDelete = true;

                ContextMenuStrip = new ContextMenuStrip();
            }

            public void LoadAnim(CameraAnim anim)
            {
                Initialize();

                CameraAnim = anim;

                Text = anim.Name;

                FrameCount = anim.FrameCount;
                AspectRatio = anim.BaseData.AspectRatio;
                ClipFar = anim.BaseData.ClipFar;
                ClipNear = anim.BaseData.ClipNear;
                FieldOfView = anim.BaseData.FieldOfView;
                Position = Utils.ToVec3(anim.BaseData.Position);
                Rotation = Utils.ToVec3(anim.BaseData.Rotation);
                Twist = anim.BaseData.Twist;
                if (anim.Flags.HasFlag(CameraAnimFlags.EulerZXY))
                    settings |= Settings.EulerZXY;
                if (anim.Flags.HasFlag(CameraAnimFlags.Perspective))
                    settings |= Settings.Perspective;

                for (int curve = 0; curve < anim.Curves.Count; curve++)
                {
                    Animation.KeyGroup keyGroup = CurveHelper.CreateTrack(anim.Curves[curve]);
                    keyGroup.AnimDataOffset = anim.Curves[curve].AnimDataOffset;
                    Values.Add(new Animation.KeyGroup()
                    {
                        AnimDataOffset = keyGroup.AnimDataOffset,
                        Keys = keyGroup.Keys,
                    });
                }
            }

            public void LoadAnim(ResU.CameraAnim anim)
            {
                Initialize();

                CameraAnimU = anim;

                Text = anim.Name;

                FrameCount = anim.FrameCount;
                AspectRatio = anim.BaseData.AspectRatio;
                ClipFar = anim.BaseData.ClipFar;
                ClipNear = anim.BaseData.ClipNear;
                FieldOfView = anim.BaseData.FieldOfView;
                Position = Utils.ToVec3(anim.BaseData.Position);
                Rotation = Utils.ToVec3(anim.BaseData.Rotation);
                Twist = anim.BaseData.Twist;
                if (anim.Flags.HasFlag(CameraAnimFlags.EulerZXY))
                    settings |= Settings.EulerZXY;
                if (anim.Flags.HasFlag(CameraAnimFlags.Perspective))
                    settings |= Settings.Perspective;

                for (int curve = 0; curve < anim.Curves.Count; curve++)
                {
                    Animation.KeyGroup keyGroup = CurveHelper.CreateTrackWiiU(anim.Curves[curve]);
                    keyGroup.AnimDataOffset = anim.Curves[curve].AnimDataOffset;
                    Values.Add(new Animation.KeyGroup()
                    {
                        AnimDataOffset = keyGroup.AnimDataOffset,
                        Keys = keyGroup.Keys,
                    });
                }
            }

            public override void OnClick(TreeView treeview) {
                UpdateEditor();
            }

            public void UpdateEditor() {
                ((BFRES)Parent.Parent.Parent.Parent).LoadEditors(this);
            }
        }

        public class BfresLightAnim : LightAnimation
        {
            public void Initialize()
            {
                CanRename = true;
                CanReplace = true;
                CanExport = true;
                CanDelete = true;

                ContextMenuStrip = new ContextMenuStrip();
            }

            public void LoadAnim(LightAnim anim)
            {
                Initialize();
                Text = anim.Name;
            }
            public void LoadAnim(ResU.LightAnim anim)
            {
                Initialize();
                Text = anim.Name;
            }

            public override void OnClick(TreeView treeview) {
                UpdateEditor();
            }


            public void UpdateEditor() {
                ((BFRES)Parent.Parent.Parent.Parent).LoadEditors(this);
            }
        }

        public class BfresFogAnim : FogAnimation
        {
            public void Initialize()
            {
                CanRename = true;
                CanReplace = true;
                CanExport = true;
                CanDelete = true;

                ContextMenuStrip = new ContextMenuStrip();
            }

            public void LoadAnim(FogAnim anim)
            {
                Initialize();
                Text = anim.Name;
            }
            public void LoadAnim(ResU.FogAnim anim)
            {
                Initialize();
                Text = anim.Name;
            }

            public override void OnClick(TreeView treeview) {
                UpdateEditor();
            }

            public void UpdateEditor() {
                ((BFRES)Parent.Parent.Parent.Parent).LoadEditors(this);
            }
        }
    }
}

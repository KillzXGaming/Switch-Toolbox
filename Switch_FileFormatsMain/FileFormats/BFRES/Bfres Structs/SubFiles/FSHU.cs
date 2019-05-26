using System.Collections.Generic;
using Switch_Toolbox.Library;
using System.Windows.Forms;
using Syroot.NintenTools.Bfres;
using FirstPlugin.Forms;
using FirstPlugin;
using Bfres.Structs;
using System;
using System.Linq;
using Switch_Toolbox.Library.Animations;

namespace Bfres.Structs
{
    public class FSHU : MaterialAnimation
    {
        public ShaderParamAnim ShaderParamAnim;

        public void Initialize()
        {
            ImageKey = "materialAnim";
            SelectedImageKey = "materialAnim";

            ContextMenuStrip = new ContextMenuStrip();
            LoadFileMenus(false);
        }
        protected void NewAction(object sender, EventArgs e) { NewMaterialAnim(); }

        public void NewMaterialAnim()
        {
            var mat = new MaterialAnimEntry("NewMaterialTarget");
            mat.LoadMaterial(new ShaderParamMatAnim() { Name = mat.Text });
            Materials.Add(mat);
        }

        public override string ExportFilter => FileFilters.GetFilter(typeof(FSHU), AnimType, true);
        public override string ReplaceFilter => FileFilters.GetFilter(typeof(FSHU), AnimType);

        public override void OnClick(TreeView treeView) => UpdateEditor();

        public void UpdateEditor(){
            ((BFRES)Parent.Parent.Parent).LoadEditors(this);
        }

        public ResFile GetResFile() {
            return ((BFRESGroupNode)Parent).GetResFileU();
        }

        public override void Replace(string FileName) {
            Replace(FileName, GetResFile());
        }

        public void Replace(string FileName, ResFile resFile)
        {
            ShaderParamAnim = new ShaderParamAnim();

            string ext = Utils.GetExtension(FileName);
            if (ext == ".bfshu")
            {
                ShaderParamAnim.Import(FileName, resFile, ShaderParamAnimType.ShaderParameter);
                ShaderParamAnim.Name = Text;
                LoadAnim(ShaderParamAnim, AnimationType.ShaderParam);
            }
            if (ext == ".bfcsh")
            {
                ShaderParamAnim.Import(FileName, resFile, ShaderParamAnimType.Color);
                ShaderParamAnim.Name = Text;
                LoadAnim(ShaderParamAnim, AnimationType.Color);
            }
            if (ext == ".bftsh")
            {
                ShaderParamAnim.Import(FileName, resFile, ShaderParamAnimType.TextureSRT);
                ShaderParamAnim.Name = Text;
                LoadAnim(ShaderParamAnim, AnimationType.TexturePattern);
            }
            if (ext == ".clr0")
            {
                ShaderParamAnim = BrawlboxHelper.FSHUConverter.Clr02Fshu(FileName);
                ShaderParamAnim.Name = Text;
                LoadAnim(ShaderParamAnim, AnimationType.Color);
            }
        }

        public void SaveAnimData()
        {
            bool IsEdited = false;

            ShaderParamAnim.Name = Text;
            ShaderParamAnim.ShaderParamMatAnims.Clear();
            for (int i = 0; i < Materials.Count; i++)
            {
                ShaderParamAnim.ShaderParamMatAnims.Add(((MaterialAnimEntry)Materials[i]).SaveData(IsEdited));
            }
        }
            
        public override void Export(string FileName)
        {
            string ext = Utils.GetExtension(FileName);
            if (ext == ".bfshu")
            {
                ShaderParamAnim.Export(FileName, GetResFile(), ShaderParamAnimType.ShaderParameter);
            }
            if (ext == ".bfcsh")
            {
                ShaderParamAnim.Export(FileName, GetResFile(), ShaderParamAnimType.Color);
            }
            if (ext == ".bftsh")
            {
                ShaderParamAnim.Export(FileName, GetResFile(), ShaderParamAnimType.TextureSRT);
            }
            if (ext == ".clr0")
            {
            }
        }

        public FSHU(ShaderParamAnim anim, AnimationType type) {
            LoadAnim(anim ,type);
        }   

        public void LoadAnim(ShaderParamAnim anim, AnimationType type)
        {
            Initialize();

            Text = anim.Name;

            AnimType = type;

            FrameCount = anim.FrameCount;
            ShaderParamAnim = anim;
            foreach (ShaderParamMatAnim matAnim in anim.ShaderParamMatAnims)
            {
                MaterialAnimEntry matNode = new MaterialAnimEntry(matAnim.Name);
                matNode.materialAnimData = matAnim;
                Materials.Add(matNode);

                //Param info determines which curves to use for each param
                //Add the curves and keys for left/right for interpolating after
                foreach (var param in matAnim.ParamAnimInfos)
                {
                    BfresParamAnim paramInfo = new BfresParamAnim(param.Name);
                    paramInfo.Type = type;
                    matNode.Params.Add(paramInfo);

                    //Get constant anims
                    for (int constant = 0; constant < param.ConstantCount; constant++)
                    {
                        Animation.KeyGroup keyGroup = new Animation.KeyGroup();
                        keyGroup.Keys.Add(new Animation.KeyFrame()
                        {
                            InterType = InterpolationType.CONSTANT,
                            Frame = 0,
                            Value = matAnim.Constants[constant].Value,
                        });

                        paramInfo.Values.Add(new Animation.KeyGroup()
                        {
                            AnimDataOffset = matAnim.Constants[constant].AnimDataOffset,
                            Keys = keyGroup.Keys,
                        });

                        if (paramInfo.Text.Contains("Color") || paramInfo.Text.Contains("color"))
                            paramInfo.Type = AnimationType.Color;
                    }

                    for (int curve = 0; curve < param.FloatCurveCount + param.IntCurveCount; curve++)
                    {
                        int index = curve + param.BeginCurve;

                        Animation.KeyGroup keyGroup = CurveHelper.CreateTrackWiiU(matAnim.Curves[index]);
                        keyGroup.AnimDataOffset = matAnim.Curves[index].AnimDataOffset;
                        paramInfo.Values.Add(new Animation.KeyGroup()
                        {
                            AnimDataOffset = keyGroup.AnimDataOffset,
                            Keys = keyGroup.Keys,
                        });

                        if (paramInfo.Text.Contains("Color") || paramInfo.Text.Contains("color"))
                            paramInfo.Type = AnimationType.Color;
                    }
                }
            }
        }

        public class MaterialAnimEntry : Material
        {
            public ShaderParamMatAnim materialAnimData;

            public MaterialAnimEntry(string name) : base(name)
            {
            }

            public void LoadMaterial(ShaderParamMatAnim data)
            {
                materialAnimData = data;
            }

            public ShaderParamMatAnim SaveData(bool IsEdited)
            {
                materialAnimData.Name = Text;
                if (IsEdited)
                {

                }
                return materialAnimData;
            }
        }

        public class BfresParamAnim : ParamKeyGroup
        {
            public BfresParamAnim(string Name)
            {
                Text = Name;
                ImageKey = "material";
                SelectedImageKey = "material";
            }
        }
    }
}

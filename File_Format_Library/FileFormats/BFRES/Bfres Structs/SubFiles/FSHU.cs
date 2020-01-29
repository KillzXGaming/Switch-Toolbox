using System.Collections.Generic;
using Toolbox.Library;
using System.Windows.Forms;
using Syroot.NintenTools.Bfres;
using FirstPlugin.Forms;
using FirstPlugin;
using Bfres.Structs;
using System;
using System.Linq;
using Toolbox.Library.Animations;

namespace Bfres.Structs
{
    public class FSHU : MaterialAnimation
    {
        public ShaderParamAnim ShaderParamAnim;

        public void Initialize()
        {
            ImageKey = "materialAnim";
            SelectedImageKey = "materialAnim";

            CanRename = true;
            CanReplace = true;
            CanExport = true;
            CanDelete = true;
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
            ((BFRES)Parent?.Parent?.Parent)?.LoadEditors(this);
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
                LoadAnim(ShaderParamAnim, AnimType);
            }
            else if (ext == ".bfcsh")
            {
                ShaderParamAnim.Import(FileName, resFile, ShaderParamAnimType.Color);
                ShaderParamAnim.Name = Text;
                LoadAnim(ShaderParamAnim, AnimType);
            }
            else if (ext == ".bftsh")
            {
                ShaderParamAnim.Import(FileName, resFile, ShaderParamAnimType.TextureSRT);
                ShaderParamAnim.Name = Text;
                LoadAnim(ShaderParamAnim, AnimType);
            }
            else if (ext == ".bfmaa")
            {
                var fmaa = new Syroot.NintenTools.NSW.Bfres.MaterialAnim(); ;
                fmaa.Import(FileName);
                ShaderParamAnim = BfresPlatformConverter.FSHUConvertSwitchToWiiU(fmaa);
                ShaderParamAnim.Name = Text;
                LoadAnim(ShaderParamAnim, AnimType);
            }
            else if (ext == ".yaml")
            {
                var fmaa = new Syroot.NintenTools.NSW.Bfres.MaterialAnim();
                fmaa = YamlFmaa.FromYaml(FileName);
                ShaderParamAnim = BfresPlatformConverter.FSHUConvertSwitchToWiiU(fmaa);
                ShaderParamAnim.Name = Text;
                LoadAnim(ShaderParamAnim, AnimType);
            }
            else if (ext == ".clr0")
            {
                ShaderParamAnim = BrawlboxHelper.FSHUConverter.Clr02Fshu(FileName);
                ShaderParamAnim.Name = Text;
                LoadAnim(ShaderParamAnim, AnimType);
            }

            UpdateEditor();
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
            else if (ext == ".bfcsh")
            {
                ShaderParamAnim.Export(FileName, GetResFile(), ShaderParamAnimType.Color);
            }
            else if (ext == ".bftsh")
            {
                ShaderParamAnim.Export(FileName, GetResFile(), ShaderParamAnimType.TextureSRT);
            }
            else if (ext == ".bfmaa")
            {
                var fmaa = BfresPlatformConverter.FSHUConvertWiiUToSwitch(ShaderParamAnim);
                fmaa.Export(FileName, new Syroot.NintenTools.NSW.Bfres.ResFile());
            }
            else if (ext == ".yaml")
            {
                var yaml = YamlFmaa.ToYaml(FileName, BfresPlatformConverter.FSHUConvertWiiUToSwitch(ShaderParamAnim), this.AnimType);
                System.IO.File.WriteAllText(FileName, yaml);
            }
            else if (ext == ".clr0")
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

            Materials.Clear();
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

        public override void NextFrame(Viewport viewport)
        {
            if (Frame > FrameCount) return;

            //Loop through each drawable bfres in the active viewport to display the anim
            foreach (var drawable in viewport.scene.staticObjects)
            {
                if (drawable is BFRESRender)
                    LoadMaterialAnimation(((BFRESRender)drawable).models);
            }
        }

        private void LoadMaterialAnimation(List<FMDL> models)
        {
            //Loop through each FMDL's materials until it matches the material anim
            foreach (var model in models)
            {
                foreach (Material matAnim in Materials)
                {
                    if (model.materials.ContainsKey(matAnim.Text))
                        EditMaterial(model.materials[matAnim.Text], matAnim);
                }
            }
        }

        private void EditMaterial(FMAT material, Material matAnim)
        {
            SetShaderParamAnimation(material, matAnim);
        }

        private void SetShaderParamAnimation(FMAT material, Material matAnim)
        {
            if (matAnim.Params.Count == 0)
                return;

            Console.WriteLine("Playing anim " + Frame);

            //Loop through param list for shader param anims
            //These store a list of values with offsets to the value
            //Then we'll update the active texture
            foreach (FSHU.BfresParamAnim paramAnim in matAnim.Params)
            {
                if (material.matparam.ContainsKey(paramAnim.Text))
                {
                    //First we get the active param that we want to alter
                    BfresShaderParam prm = material.matparam[paramAnim.Text];
                    BfresShaderParam animatedprm = new BfresShaderParam();
                    if (!material.animatedMatParams.ContainsKey(prm.Name))
                        material.animatedMatParams.Add(prm.Name, animatedprm);
                    animatedprm.Name = prm.Name;
                    animatedprm.ValueUint = prm.ValueUint;
                    animatedprm.ValueBool = prm.ValueBool;
                    animatedprm.ValueFloat = prm.ValueFloat;
                    animatedprm.ValueSrt2D = prm.ValueSrt2D;
                    animatedprm.ValueSrt3D = prm.ValueSrt3D;
                    animatedprm.ValueTexSrt = prm.ValueTexSrt;
                    animatedprm.ValueTexSrtEx = prm.ValueTexSrtEx;

                    foreach (var group in paramAnim.Values)
                    {
                        //The offset determines the value starting from 0.
                        int index = 0;
                        if (prm.ValueFloat != null)
                        {
                            if (group.AnimDataOffset != 0)
                                index = (int)group.AnimDataOffset / sizeof(float);

                            animatedprm.ValueFloat[index] = group.GetValue(Frame);
                        }
                        if (prm.ValueInt != null)
                        {

                        }
                        if (prm.ValueUint != null)
                        {

                        }
                        if (prm.ValueBool != null)
                        {

                        }
                        if (prm.Type == Syroot.NintenTools.NSW.Bfres.ShaderParamType.Srt3D)
                        {
                            if (group.AnimDataOffset == 0) prm.ValueSrt3D.Scaling.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 4) prm.ValueSrt3D.Scaling.Y = group.GetValue(Frame);
                            if (group.AnimDataOffset == 8) prm.ValueSrt3D.Scaling.Z = group.GetValue(Frame);
                            if (group.AnimDataOffset == 12) prm.ValueSrt3D.Rotation.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 16) prm.ValueSrt3D.Rotation.Y = group.GetValue(Frame);
                            if (group.AnimDataOffset == 20) prm.ValueSrt3D.Rotation.Z = group.GetValue(Frame);
                            if (group.AnimDataOffset == 24) prm.ValueSrt3D.Translation.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 28) prm.ValueSrt3D.Translation.Y = group.GetValue(Frame);
                            if (group.AnimDataOffset == 32) prm.ValueSrt3D.Translation.Z = group.GetValue(Frame);
                        }
                        if (prm.Type == Syroot.NintenTools.NSW.Bfres.ShaderParamType.Srt2D)
                        {
                            if (group.AnimDataOffset == 0) prm.ValueSrt2D.Scaling.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 4) prm.ValueSrt2D.Scaling.Y = group.GetValue(Frame);
                            if (group.AnimDataOffset == 8) prm.ValueSrt2D.Rotation = group.GetValue(Frame);
                            if (group.AnimDataOffset == 12) prm.ValueSrt2D.Translation.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 16) prm.ValueSrt2D.Translation.X = group.GetValue(Frame);
                        }
                        if (prm.Type == Syroot.NintenTools.NSW.Bfres.ShaderParamType.TexSrt)
                        {
                            if (group.AnimDataOffset == 0) prm.ValueTexSrt.Mode = (Syroot.NintenTools.NSW.Bfres.TexSrtMode)(uint)group.GetValue(Frame);
                            if (group.AnimDataOffset == 4) prm.ValueTexSrt.Scaling.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 8) prm.ValueTexSrt.Scaling.Y = group.GetValue(Frame);
                            if (group.AnimDataOffset == 12) prm.ValueTexSrt.Rotation = group.GetValue(Frame);
                            if (group.AnimDataOffset == 16) prm.ValueTexSrt.Translation.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 20) prm.ValueTexSrt.Translation.Y = group.GetValue(Frame);
                        }
                        if (prm.Type == Syroot.NintenTools.NSW.Bfres.ShaderParamType.TexSrtEx)
                        {
                            if (group.AnimDataOffset == 0) prm.ValueTexSrtEx.Mode = (Syroot.NintenTools.NSW.Bfres.TexSrtMode)(uint)group.GetValue(Frame);
                            if (group.AnimDataOffset == 4) prm.ValueTexSrtEx.Scaling.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 8) prm.ValueTexSrtEx.Scaling.Y = group.GetValue(Frame);
                            if (group.AnimDataOffset == 12) prm.ValueTexSrtEx.Rotation = group.GetValue(Frame);
                            if (group.AnimDataOffset == 16) prm.ValueTexSrtEx.Translation.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 20) prm.ValueTexSrtEx.Translation.Y = group.GetValue(Frame);
                            if (group.AnimDataOffset == 24) prm.ValueTexSrtEx.MatrixPointer = (uint)group.GetValue(Frame);
                        }
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

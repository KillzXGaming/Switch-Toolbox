using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using FirstPlugin;
using Syroot.NintenTools.NSW.Bfres;
using FirstPlugin.Forms;
using Toolbox.Library.Animations;

namespace Bfres.Structs
{
    public class FMAA : MaterialAnimation
    {
        public override AnimationType AnimType
        {
            get
            {
                if (MaterialAnim != null)
                {
                    string nameType = MaterialAnim.Name.Split('_').Last();

                    if (MaterialAnim.TextureNames?.Count > 0)
                        return AnimationType.TexturePattern;
                    else if (nameMappedTypes.ContainsKey($"_{nameType}"))
                        return nameMappedTypes[$"_{nameType}"];
                    else
                        return AnimationType.ShaderParam;
                }

                return AnimationType.ShaderParam;
            }
        }

        private Dictionary<string, AnimationType> nameMappedTypes = new Dictionary<string, AnimationType>
        {
            {"_fsp", AnimationType.ShaderParam },
            {"_sp", AnimationType.ShaderParam },
            {"_ftp", AnimationType.TexturePattern },
            {"_tp", AnimationType.TexturePattern },
            {"_fts", AnimationType.TextureSrt },
            {"_ts", AnimationType.TextureSrt },
            {"_fcl", AnimationType.Color },
            {"_cl", AnimationType.Color },
            {"_fvm", AnimationType.Visibilty },
            {"_vm", AnimationType.Visibilty },
        };

        public override void OnDoubleMouseClick(TreeView treeview)
        {
            if (this.AnimType != AnimationType.TexturePattern)
                return;

            var ParentFolder = this.Parent;

            BfresTexturePatternEditor form = new BfresTexturePatternEditor(ParentFolder.Nodes);
            form.LoadAnim(this);
            form.Show();
        }

        public class MaterialAnimEntry : Material
        {
            public FMAA matAnimWrapper;

            public MaterialAnimData MaterialAnimData;

            public MaterialAnimEntry(string name) : base(name)
            {
           
            }

            public void LoadMaterial(MaterialAnimData data, FMAA materialAnimation)
            {
                MaterialAnimData = data;
                matAnimWrapper = materialAnimation;
            }

            public void CreateSampler(string Name, bool IsConstant)
            {
                var mat = MaterialAnimData;

                var SamplerInfo = new TexturePatternAnimInfo();
                if (IsConstant)
                {
                    SamplerInfo.BeginConstant = (ushort)mat.Constants.Count;
                    SamplerInfo.CurveIndex = (ushort)65535;
                }
                else
                {
                    SamplerInfo.BeginConstant = (ushort)65535;
                    SamplerInfo.CurveIndex = (ushort)mat.Curves.Count;
                }
                mat.TexturePatternAnimInfos.Add(SamplerInfo);

                BfresSamplerAnim sampler = new BfresSamplerAnim(SamplerInfo.Name, matAnimWrapper, this);
                Samplers.Add(sampler);

            }

            public MaterialAnimData SaveData(bool IsEdited)
            {
                MaterialAnimData.Name = Text;
                if (IsEdited)
                {

                }
                return MaterialAnimData;
            }
        }

        public void UpdateMaterialBinds()
        {
            MaterialAnim.TextureBindArray = new List<long>();

            ushort[] binds = new ushort[MaterialAnim.MaterialAnimDataList.Count];

            for (int i = 0; i < binds.Length; i++)
                binds[i] = ushort.MaxValue;
            for (int i = 0; i < MaterialAnim.TextureNames.Count; i++)
                MaterialAnim.TextureBindArray.Add(- 1);

            MaterialAnim.BindIndices = binds;
        }

        public override void UpdateAnimationData()
        {
            MaterialAnim.FrameCount = FrameCount;
            MaterialAnim.TextureNames = Textures;
        }

        public void SaveAnimData()
        {
            MaterialAnim.FrameCount = FrameCount;
            MaterialAnim.TextureNames = Textures;
            MaterialAnim.Name = Text;

            int TexturePatternCurveIndex = 0;
            int ParamCurveIndex = 0;

            MaterialAnim.MaterialAnimDataList.Clear();
            foreach (MaterialAnimEntry mat in Materials)
            {
                mat.MaterialAnimData.Name = mat.Text;
                MaterialAnim.MaterialAnimDataList.Add(mat.MaterialAnimData);

                if (IsEdited)
                {
                    //Set default indices as unused for now
                    mat.MaterialAnimData.BeginVisalConstantIndex = -1;
                    mat.MaterialAnimData.ShaderParamCurveIndex = -1;
                    mat.MaterialAnimData.TexturePatternCurveIndex = -1;
                    mat.MaterialAnimData.VisalCurveIndex = -1;
                    mat.MaterialAnimData.VisualConstantIndex = -1;

                    //Load our first curve index if a sampler or param has one
                  //  SavePatternInfos(data, mat, TexturePatternCurveIndex);
                 //   SaveParamInfos(data, mat, ParamCurveIndex);
                }
            }

            UpdateMaterialBinds();
        }

        private void SavePatternInfos(MaterialAnimData data, Material mat, int TexturePatternCurveIndex)
        {
            bool SetBeginCurve = false;
            foreach (var sampler in mat.Samplers)
            {
                //If constant, create a constant instance with the first value set
                if (sampler.Constant)
                {
                    AnimConstant animConstant = new AnimConstant();
                    animConstant.AnimDataOffset = 0;
                    animConstant.Value = sampler.GetValue(0);
                    data.Constants.Add(animConstant);
                }
                else if (sampler.Keys.Count > 0)
                {
                    //If a sampler is not constant and uses curve data, then we need to set our begin curve index
                    if (!SetBeginCurve)
                    {
                        data.TexturePatternCurveIndex += TexturePatternCurveIndex;
                        data.BeginVisalConstantIndex += 0; //This is usually 0 so set it

                        SetBeginCurve = true;
                    }

                    //Create a curve to store all the index data in for each frame
                    AnimCurve curve = new AnimCurve();
                    curve.AnimDataOffset = 0;

                    //Set our data types. Pattern and visibilty are always the same type
                    if (AnimType == AnimationType.TexturePattern)
                        curve.CurveType = AnimCurveType.StepInt;
                    else if (AnimType == AnimationType.Visibilty)
                        curve.CurveType = AnimCurveType.StepBool;
                    else
                    {
                        if (sampler.InterpolationType == InterpolationType.HERMITE)
                            curve.CurveType = AnimCurveType.Cubic;
                        if (sampler.InterpolationType == InterpolationType.LINEAR)
                            curve.CurveType = AnimCurveType.Linear;
                        if (sampler.InterpolationType == InterpolationType.STEP)
                            curve.CurveType = AnimCurveType.StepInt;
                        if (sampler.InterpolationType == InterpolationType.STEPBOOL)
                            curve.CurveType = AnimCurveType.StepBool;
                    }

                    curve.Delta = 0;
                    curve.EndFrame = FrameCount;
                    curve.StartFrame = 0;
                    curve.FrameType = AnimCurveFrameType.Byte;
                    curve.Scale = 1;
                    curve.Offset = 0;

                    List<float> Frames = new List<float>();
                    List<float> Keys = new List<float>();

                    for (int frame = 0; frame < sampler.Keys.Count; frame++)
                    {
                        float currentIndex = sampler.GetValue(frame);
                        if (frame > 0)
                        {
                            float previousIndex = sampler.GetValue(frame - 1);
                            if (currentIndex != previousIndex)
                            {
                                //Add key frame if texture not loaded previously
                                Frames.Add(frame);
                                Keys.Add(currentIndex);
                            }
                        }
                        else
                        {
                            //Add the first key frame value. We would add base values here but switch has none
                            Frames.Add(frame);
                            Keys.Add(currentIndex);
                        }
                    }


                    for (int i = 0; i <Keys.Count; i++)
                    {
                        curve.Keys[i, 0] = Keys[i];
                    }

                    curve.Frames = Frames.ToArray();

                    TexturePatternCurveIndex += 1;
                }
            }
        }

        private void SaveParamInfos(MaterialAnimData data, Material mat, int ParamCurveIndex)
        {
            foreach (var param in mat.Params)
            {
                //Set our begin curve index
                bool SetBeginCurve = false;
                if (!SetBeginCurve)
                {
                    data.ShaderParamCurveIndex += ParamCurveIndex;
                    data.BeginVisalConstantIndex += 0; //This is usually 0 so set it

                    SetBeginCurve = true;
                }

                foreach (var value in  param.Values)
                {
                    if (value.Constant)
                    {
                        AnimConstant animConstant = new AnimConstant();
                        animConstant.AnimDataOffset = 0;
                        animConstant.Value = value.GetValue(0); //Set our constant value
                        data.Constants.Add(animConstant);
                    }
                    else if (value.Keys.Count > 0) //Else create curves for each param value
                    {
                        //If constant, create a constant instance with the first value set
                        AnimCurve curve = new AnimCurve();
                        curve.AnimDataOffset = value.AnimDataOffset;
                        curve.Scale = value.Scale;
                        curve.Offset = value.Offset;

                        data.Curves.Add(curve);
                    }
                }
            }
        }

        public void UpdateMaterials(Material mat)
        {

        }

        public BFRESRender BFRESRender;
        public MaterialAnim MaterialAnim;
        public FMAA()
        {
            Initialize();
        }

        public void Initialize()
        {
            ImageKey = "materialAnim";
            SelectedImageKey = "materialAnim";

            CanRename = true;
            CanReplace = true;
            CanExport = true;
            CanDelete = true;

            Materials.Clear();
        }

        protected void NewAction(object sender, EventArgs e) { NewMaterialAnim(); }

        public void NewMaterialAnim()
        {
            var mat = new MaterialAnimEntry("NewMaterialTarget");
            mat.LoadMaterial(new MaterialAnimData() { Name = mat.Text }, this);
            Materials.Add(mat);
        }

        public override void OnClick(TreeView treeView) => UpdateEditor();

        public void UpdateEditor(){
            ((BFRES)Parent?.Parent?.Parent)?.LoadEditors(this);
        }

        public ResFile GetResFile()
        {
            return ((BFRESGroupNode)Parent).GetResFile();
        }

        public FMAA(MaterialAnim anim) { LoadAnim(anim); }

        private void LoadAnim(MaterialAnim anim)
        {
            Initialize();

            MaterialAnim = anim;
            FrameCount = MaterialAnim.FrameCount;
            Text = anim.Name;

            Textures.Clear();
            if (anim.TextureNames != null)
            {
                foreach (var name in anim.TextureNames)
                    Textures.Add(name);
            }

            Materials.Clear();
            foreach (var matanim in anim.MaterialAnimDataList)
            {
                var mat = new MaterialAnimEntry(matanim.Name);
                mat.MaterialAnimData = matanim;
                Materials.Add(mat);

                foreach (var param in matanim.ParamAnimInfos)
                {
                    FSHU.BfresParamAnim paramInfo = new FSHU.BfresParamAnim(param.Name);
                    mat.Params.Add(paramInfo);

                    paramInfo.Type = AnimationType.ShaderParam;

                    //There is no better way to determine if the param is a color type afaik
                    if (param.Name.Contains("Color") || param.Name.Contains("color") || param.Name == "multi_tex_reg2")
                        paramInfo.Type = AnimationType.Color;
                    else if (AnimType == AnimationType.TexturePattern)
                        paramInfo.Type = AnimationType.TexturePattern;
                    else if (AnimType == AnimationType.TextureSrt)
                        paramInfo.Type = AnimationType.TextureSrt;
                    else 
                        paramInfo.Type = AnimationType.ShaderParam;

                    //Get constant anims
                    for (int constant = 0; constant < param.ConstantCount; constant++)
                    {
                        int index = constant + param.BeginConstant;

                        Animation.KeyGroup keyGroup = new Animation.KeyGroup();
                        keyGroup.Keys.Add(new Animation.KeyFrame()
                        {
                            InterType = InterpolationType.CONSTANT,
                            Frame = 0,
                            Value = matanim.Constants[index].Value,
                        });

                        paramInfo.Values.Add(new Animation.KeyGroup()
                        {
                            
                            AnimDataOffset = matanim.Constants[index].AnimDataOffset,
                            Keys = keyGroup.Keys,
                        });
                    }

                    for (int curve = 0; curve < param.FloatCurveCount + param.IntCurveCount; curve++)
                    {
                        int index = curve + param.BeginCurve;

                        Animation.KeyGroup keyGroup = CurveHelper.CreateTrack(matanim.Curves[index]);
                        keyGroup.AnimDataOffset = matanim.Curves[index].AnimDataOffset;

                        paramInfo.Values.Add(new Animation.KeyGroup()
                        {
                            AnimDataOffset = keyGroup.AnimDataOffset,
                            Keys = keyGroup.Keys,
                        });
                    }
                }

                foreach (TexturePatternAnimInfo SamplerInfo in matanim.TexturePatternAnimInfos)
                {
                    BfresSamplerAnim sampler = new BfresSamplerAnim(SamplerInfo.Name, this, mat);
                    mat.Samplers.Add(sampler);


                    int textureIndex = 0;

                    if (SamplerInfo.BeginConstant != 65535)
                    {
                        textureIndex = matanim.Constants[SamplerInfo.BeginConstant].Value;

                        sampler.Keys.Add(new Animation.KeyFrame() { Frame = 0, Value = textureIndex });
                        sampler.Constant = true;
                        sampler.AnimDataOffset = matanim.Constants[SamplerInfo.BeginConstant].AnimDataOffset;
                    }
                    if (SamplerInfo.CurveIndex != 65535)
                    {
                        int index = (int)SamplerInfo.CurveIndex;

                        Animation.KeyGroup keyGroup = CurveHelper.CreateTrack(matanim.Curves[index], true);

                        sampler.AnimDataOffset = matanim.Curves[index].AnimDataOffset;
                        sampler.Keys = keyGroup.Keys;
                    }
                }
            }
        }


        public class BfresSamplerAnim : SamplerKeyGroup
        {
            FMAA MatAnimWrapper;
            MaterialAnimEntry MatWrapper;

            public BfresSamplerAnim(string Name, FMAA materialAnimation, MaterialAnimEntry material) : base(materialAnimation)
            {
                MatWrapper = material;
                MatAnimWrapper = materialAnimation;
                Text = Name;
            }

            private AnimCurve CreateAnimCurve()
            {
                //Create empty curve
                AnimCurve curve = new AnimCurve();
                curve.Offset = 0;
                curve.StartFrame = 0;
                curve.EndFrame = MatAnimWrapper.FrameCount;
                curve.CurveType = AnimCurveType.StepInt;
                curve.AnimDataOffset = 0;
                curve.Scale = 1;

                //Start with sbyte. Increase only if index is > 255
                curve.KeyType = AnimCurveKeyType.SByte; 

                //Set by frame count
                curve.FrameType = CurveHelper.GetFrameType((uint)MatAnimWrapper.FrameCount);

                return curve;
            }

            public override STGenericTexture GetActiveTexture(int index)
            {
                string name = "";
                try
                {
                    name = MatAnimWrapper.Textures[(int)index];
                }
                catch
                {
                    throw new Exception("Index out of range " + index);
                }

                var bfres = (BFRES)MatAnimWrapper.Parent.Parent.Parent;
                var BfresTextures = bfres.GetBNTX;
                if (BfresTextures != null)
                {
                    if (BfresTextures.Textures.ContainsKey(name))
                        return BfresTextures.Textures[name];
                }

                foreach (var bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(name))
                        return bntx.Textures[name];
                }
                return null;
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
            //Alright material animations have a total of 5 types.
            //SRT (type of shader param)
            //Color (type of shader param)
            //Shader param
            //Visibily animation
            //Texture pattern animation

            SetMaterialVisualAnimation(material, matAnim);
            SetShaderParamAnimation(material, matAnim);
            SetTextureAnimation(material, matAnim);
        }

        private void SetMaterialVisualAnimation(FMAT material, Material matAnim)
        {
        }

        private void SetShaderParamAnimation(FMAT material, Material matAnim)
        {
            if (matAnim.Params.Count == 0)
                return;

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
                        if (prm.Type == ShaderParamType.Srt3D)
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
                        if (prm.Type == ShaderParamType.Srt2D)
                        {
                            if (group.AnimDataOffset == 0) prm.ValueSrt2D.Scaling.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 4) prm.ValueSrt2D.Scaling.Y = group.GetValue(Frame);
                            if (group.AnimDataOffset == 8) prm.ValueSrt2D.Rotation = group.GetValue(Frame);
                            if (group.AnimDataOffset == 12) prm.ValueSrt2D.Translation.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 16) prm.ValueSrt2D.Translation.X = group.GetValue(Frame);
                        }
                        if (prm.Type == ShaderParamType.TexSrt)
                        {
                            if (group.AnimDataOffset == 0) prm.ValueTexSrt.Mode = (TexSrtMode)(uint)group.GetValue(Frame);
                            if (group.AnimDataOffset == 4) prm.ValueTexSrt.Scaling.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 8) prm.ValueTexSrt.Scaling.Y = group.GetValue(Frame);
                            if (group.AnimDataOffset == 12) prm.ValueTexSrt.Rotation = group.GetValue(Frame);
                            if (group.AnimDataOffset == 16) prm.ValueTexSrt.Translation.X = group.GetValue(Frame);
                            if (group.AnimDataOffset == 20) prm.ValueTexSrt.Translation.Y = group.GetValue(Frame);
                        }
                        if (prm.Type == ShaderParamType.TexSrtEx)
                        {
                            if (group.AnimDataOffset == 0) prm.ValueTexSrtEx.Mode = (TexSrtMode)(uint)group.GetValue(Frame);
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

        private void SetTextureAnimation(FMAT material, Material matAnim)
        {
            if (matAnim.Samplers.Count == 0)
                return;

            //Loop through sampler list for texture anims
            //These store a list of indices (step curve) to grab a texture name list
            //Then we'll update the active texture
            foreach (SamplerKeyGroup samplerAnim in matAnim.Samplers)
            {
                foreach (MatTexture texture in material.TextureMaps)
                {
                    if (texture.SamplerName == samplerAnim.Text)
                    {
                        texture.textureState = STGenericMatTexture.TextureState.Animated;
                        texture.animatedTexName = samplerAnim.GetActiveTextureNameByFrame(Frame);
                    }
                }
            }
        }

        public override string ExportFilter => FileFilters.GetFilter(typeof(FMAA));
        public override string ReplaceFilter => FileFilters.GetFilter(typeof(FMAA));

        public override void Export(string FileName)
        {
            string ext = Utils.GetExtension(FileName);

            if (ext == ".bfmaa")
            {
                MaterialAnim.Export(FileName, GetResFile());
            }
            else if (ext == ".yaml")
            {
               System.IO.File.WriteAllText(FileName, YamlFmaa.ToYaml(FileName, MaterialAnim, AnimType));
            }
        }
        public override void Replace(string FileName)
        {
            string ext = Utils.GetExtension(FileName);

            if (ext == ".bfmaa")
            {
                MaterialAnim.Import(FileName);
                MaterialAnim.Name = Text;
                LoadAnim(MaterialAnim);
            }
            else if (ext == ".yaml")
            {
                MaterialAnim = YamlFmaa.FromYaml(FileName);
                MaterialAnim.Name = Text;
                LoadAnim(MaterialAnim);
            }
            else if (ext == ".gif" || ext == ".png" || ext == ".apng")
            {
                BNTX bntx = PluginRuntime.bntxContainers[0];
                GifToTexturePatternAnimation anim = new GifToTexturePatternAnimation(FileName, bntx, this);
                MaterialAnim.Name = Text;
                LoadAnim(MaterialAnim);
            }

            UpdateEditor();
        }
    }
}

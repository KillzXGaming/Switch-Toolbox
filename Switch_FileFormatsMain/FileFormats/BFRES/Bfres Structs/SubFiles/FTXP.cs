using System;
using System.Windows.Forms;
using System.Collections.Generic;
using FirstPlugin;
using FirstPlugin.Forms;
using Syroot.NintenTools.Bfres;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Animations;

namespace Bfres.Structs
{
    public class FTXP : MaterialAnimation
    {
        public TexPatternAnim TexPatternAnim;

        public override void OnClick(TreeView treeView) => UpdateEditor();

        public void UpdateEditor() {
            ((BFRES)Parent.Parent.Parent).LoadEditors(this);
        }

        public ResFile GetResFile() {
            return ((BFRESGroupNode)Parent).GetResFileU();
        }

        public FTXP(TexPatternAnim anim) { LoadAnim(anim); }

        public void UpdateMaterialBinds()
        {
            ushort[] binds = new ushort[TexPatternAnim.TexPatternMatAnims.Count];
            for (int i = 0; i < binds.Length; i++)
                binds[i] = ushort.MaxValue;

            TexPatternAnim.BindIndices = binds;
        }

        private void LoadAnim(TexPatternAnim anim)
        {
            CanReplace = true;
            CanExport = true;
            CanDelete = true;
            CanRename = true;

            Text = anim.Name;

            TexPatternAnim = anim;
            FrameCount = anim.FrameCount;

            if (anim.TextureRefNames != null)
            {
                foreach (var tex in anim.TextureRefNames)
                    Textures.Add(tex.Name);
            }

            if (anim.TextureRefs != null)
            {
                foreach (var tex in anim.TextureRefs)
                    Textures.Add(tex.Key);
            }


            foreach (TexPatternMatAnim matanim in anim.TexPatternMatAnims)
            {
                var mat = new MaterialAnimEntry(matanim.Name);
                mat.TexPatternMatAnim = matanim;
                Materials.Add(mat);

                foreach (PatternAnimInfo SamplerInfo in matanim.PatternAnimInfos)
                {
                    BfresSamplerAnim sampler = new BfresSamplerAnim(SamplerInfo.Name, this);
                    mat.Samplers.Add(sampler);

                    int textureIndex = 0;

                    if (SamplerInfo.SubBindIndex != -1)
                    {
                        textureIndex = SamplerInfo.SubBindIndex;

                        var group = new Animation.KeyGroup();
                        group.Keys.Add(new Animation.KeyFrame() { Frame = 0, Value = textureIndex });
                        group.Constant = true;

                        sampler.group = group;
                    }
                    if (SamplerInfo.CurveIndex != -1)
                    {
                        int index = (int)SamplerInfo.CurveIndex;

                        Animation.KeyGroup keyGroup = CurveHelper.CreateTrackWiiU(matanim.Curves[index]);
                        keyGroup.AnimDataOffset = matanim.Curves[index].AnimDataOffset;
                        sampler.group = new KeyGroup()
                        {
                            AnimDataOffset = keyGroup.AnimDataOffset,
                            Keys = keyGroup.Keys,
                        };

                        foreach (var ind in keyGroup.Keys)
                        {
                            Console.WriteLine($"{SamplerInfo.Name} {ind.Value}");
                        }
                    }
                }
            }
        }

        public void SaveAnimData()
        {
            TexPatternAnim.Name = Text;
            TexPatternAnim.TexPatternMatAnims.Clear();

            bool IsEdited = false;

            for (int i = 0; i < Materials.Count; i++)
            {
                TexPatternAnim.TexPatternMatAnims.Add(((MaterialAnimEntry)Materials[i]).SaveData(IsEdited));
            }
        }

        public class MaterialAnimEntry : Material
        {
            public TexPatternMatAnim TexPatternMatAnim;

            public MaterialAnimEntry(string name) : base(name)
            {
            }

            public void LoadMaterial(TexPatternMatAnim data)
            {
                TexPatternMatAnim = data;
            }

            public TexPatternMatAnim SaveData(bool IsEdited)
            {
                TexPatternMatAnim.Name = Text;
                if (IsEdited)
                {

                }
                return TexPatternMatAnim;
            }
        }

        public class BfresSamplerAnim : Material.Sampler
        {
            FTXP AnimWrapper;

            public BfresSamplerAnim(string Name, FTXP ftxp)
            {
                Text = Name;
                ImageKey = "texture";
                SelectedImageKey = "texture";

                AnimWrapper = ftxp;
            }

            public override string GetActiveTextureName(int frame)
            {
                uint val = (uint)group.GetValue(frame);
                return AnimWrapper.Textures[(int)val];
            }

            public override STGenericTexture GetActiveTexture(int frame)
            {
                uint val = (uint)group.GetValue(frame);

                foreach (var ftexFolder in PluginRuntime.ftexContainers)
                {
                    try
                    {
                        string name = GetActiveTextureName(frame);

                        if (ftexFolder.ResourceNodes.ContainsKey(name))
                            return (STGenericTexture)ftexFolder.ResourceNodes[name];
                    }
                    catch
                    {
                        throw new Exception("Index out of range " + val);
                    }
                }
                return null;
            }
        }

        public override void NextFrame(Viewport viewport)
        {
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
                        SetTextureAnimation(model.materials[matAnim.Text], matAnim);
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
            foreach (Material.Sampler samplerAnim in matAnim.Samplers)
            {
                foreach (MatTexture texture in material.TextureMaps)
                {
                    if (texture.SamplerName == samplerAnim.Text)
                    {
                        texture.textureState = STGenericMatTexture.TextureState.Animated;

                        uint index = (uint)samplerAnim.group.GetValue(Frame);
                        texture.animatedTexName = Textures[(int)index];
                    }
                }
            }
        }

        public override string ExportFilter => FileFilters.GetFilter(typeof(FTXP));

        public override void Export(string FileName)
        {
            TexPatternAnim.Export(FileName, GetResFile());
        }

        public override void Replace(string FileName) {
            Replace(FileName, GetResFile());
        }

        public void Replace(string FileName, ResFile resFile)
        {
            string ext = Utils.GetExtension(FileName);

            if (ext == ".bftxp")
            {
                TexPatternAnim.Import(FileName, resFile);
                TexPatternAnim.Name = Text;
                LoadAnim(TexPatternAnim);
            }
            else if (ext == ".gif")
            {
                BFRESGroupNode ftexFolder = PluginRuntime.ftexContainers[0];
                GifToTexturePatternAnimation anim = new GifToTexturePatternAnimation(FileName, ftexFolder, this);
                TexPatternAnim.Name = Text;
                LoadAnim(TexPatternAnim);
            }
        }
    }
}

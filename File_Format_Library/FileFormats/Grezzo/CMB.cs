using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;
using FirstPlugin.Forms;
using ZeldaLib.CtrModelBinary.Types;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.PICA.Commands;

namespace FirstPlugin
{
    public class CMB : TreeNodeFile, IFileFormat, IContextMenuNode, IExportableModel
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "*CTR Model Binary" };
        public string[] Extension { get; set; } = new string[] { "*.cmb" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, "cmb ");
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

        public IEnumerable<STGenericObject> ExportableMeshes => Renderer.Meshes;
        public IEnumerable<STGenericMaterial> ExportableMaterials => Materials;
        public IEnumerable<STGenericTexture> ExportableTextures => Renderer.TextureList;
        public STSkeleton ExportableSkeleton => Skeleton;

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Save", null, SaveAction, Keys.Control | Keys.S));
            return Items.ToArray();
        }

        private void SaveAction(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(this);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        bool DrawablesLoaded = false;
        public override void OnClick(TreeView treeView)
        {
            LoadEditor<STPropertyGrid>();
        }

        public T LoadEditor<T>() where T : UserControl, new()
        {
            ViewportEditor editor = (ViewportEditor)LibraryGUI.GetActiveContent(typeof(ViewportEditor));
            if (editor == null)
            {
                editor = new ViewportEditor(true);
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }
            if (!DrawablesLoaded)
            {
                ObjectEditor.AddContainer(DrawableContainer);
                DrawablesLoaded = true;
            }
            if (Runtime.UseOpenGL)
                editor.LoadViewport(DrawableContainer);

            foreach (var subControl in editor.GetEditorPanel().Controls)
                if (subControl.GetType() == typeof(T))
                    return subControl as T;

            T control = new T();
            control.Dock = DockStyle.Fill;
            editor.LoadEditor(control);
            return control;
        }

        public CMB_Renderer Renderer;
        public DrawableContainer DrawableContainer = new DrawableContainer();
        List<STGenericMaterial> Materials = new List<STGenericMaterial>();

        public ZeldaLib.CtrModelBinary.CMB cmb = new ZeldaLib.CtrModelBinary.CMB();
        STTextureFolder texFolder;
        public STSkeleton Skeleton;

        public void Load(System.IO.Stream stream)
        {
            CanSave = false;

            Renderer = new CMB_Renderer();
            DrawableContainer.Drawables.Add(Renderer);


            Skeleton = new STSkeleton();
            //These models/skeletons come out massive so scale them with an overridden scale
            Skeleton.PreviewScale = Renderer.PreviewScale;
            Skeleton.BonePointScale = 40;
            Renderer.Skeleton = Skeleton;

            DrawableContainer.Drawables.Add(Skeleton);


            cmb.ReadCMB(stream);

            Text = cmb.Header.Name;

            DrawableContainer.Name = Text;

            //Load textures
            if (cmb.TexturesChunk != null)
            {
                texFolder = new TextureFolder("Texture");
                TreeNode meshFolder = new TreeNode("Meshes");
                TreeNode materialFolder = new TreeNode("Materials");
                TreeNode skeletonFolder = new TreeNode("Skeleton");

                bool HasTextures = cmb.TexturesChunk.Textures != null &&
                     cmb.TexturesChunk.Textures.Count != 0;

                bool HasMeshes = cmb.MeshesChunk.SHP.SEPDs != null &&
                     cmb.MeshesChunk.SHP.SEPDs.Count != 0;

                bool HasSkeleton = cmb.SkeletonChunk != null &&
                    cmb.SkeletonChunk.Bones.Count != 0;

                bool HasMaterials = cmb.MaterialsChunk != null &&
                   cmb.MaterialsChunk.Materials.Count != 0;

                if (HasSkeleton)
                {
                    var bonesOrdered = cmb.SkeletonChunk.Bones.OrderBy(x => x.ID).ToList();
                    foreach (var bone in bonesOrdered)
                    {
                        STBone genericBone = new STBone(Skeleton);
                        genericBone.parentIndex = bone.ParentID;
                        genericBone.Checked = true;

                        genericBone.Text = $"Bone {bone.ID}";
                        genericBone.RotationType = STBone.BoneRotationType.Euler;

                        genericBone.Position = new OpenTK.Vector3(
                            bone.Translation.X,
                            bone.Translation.Y,
                            bone.Translation.Z
                        );
                        genericBone.EulerRotation = new OpenTK.Vector3(
                                      bone.Rotation.X,
                                      bone.Rotation.Y,
                                      bone.Rotation.Z
                        );
                        genericBone.Scale = new OpenTK.Vector3(
                                      bone.Scale.X,
                                      bone.Scale.Y,
                                      bone.Scale.Z
                        );
                        Skeleton.bones.Add(genericBone);
                    }

                    foreach (var bone in Skeleton.bones)
                    {
                        if (bone.Parent == null)
                            skeletonFolder.Nodes.Add(bone);
                    }

                    Skeleton.reset();
                    Skeleton.update();
                }

                if (HasTextures)
                {
                    int texIndex = 0;
                    foreach (var tex in cmb.TexturesChunk.Textures)
                    {

                        var texWrapper = new CTXB.TextureWrapper(new CTXB.Texture());
                        texWrapper.Text = $"Texture {texIndex++}";
                        texWrapper.ImageKey = "texture";
                        texWrapper.SelectedImageKey = texWrapper.ImageKey;

                        if (tex.Name != string.Empty)
                            texWrapper.Text = tex.Name;

                        texWrapper.Width = tex.Width;
                        texWrapper.Height = tex.Height;
                        CTXB.Texture.TextureFormat Format = (CTXB.Texture.TextureFormat)((tex.DataType << 16) | tex.ImageFormat);

                        texWrapper.Format = CTR_3DS.ConvertPICAToGenericFormat(CTXB.Texture.FormatList[Format]);
                        texWrapper.ImageData = tex.ImageData;
                        texFolder.Nodes.Add(texWrapper);

                        Renderer.TextureList.Add(texWrapper);
                    }
                }

                if (HasMaterials)
                {
                    int materialIndex = 0;
                    foreach (var mat in cmb.MaterialsChunk.Materials)
                    {
                        H3DMaterial H3D = ToH3DMaterial(mat);

                        CMBMaterialWrapper material = new CMBMaterialWrapper(mat, this, H3D);
                        material.Text = $"Material {materialIndex++}";
                        materialFolder.Nodes.Add(material);
                        Materials.Add(material);

                        bool HasDiffuse = false;
                        foreach (var tex in mat.TextureMappers)
                        {
                            if (tex.TextureID != -1)
                            {
                                CMBTextureMapWrapper matTexture = new CMBTextureMapWrapper(tex, this);
                                matTexture.TextureIndex = tex.TextureID;
                                material.TextureMaps.Add(matTexture);

                                if (tex.TextureID < Renderer.TextureList.Count && tex.TextureID >= 0)
                                {
                                    matTexture.Name = Renderer.TextureList[tex.TextureID].Text;
                                    material.Nodes.Add(matTexture.Name);
                                }

                                if (!HasDiffuse && matTexture.Name != "bg_syadowmap") //Quick hack till i do texture env stuff
                                {
                                    matTexture.Type = STGenericMatTexture.TextureType.Diffuse;
                                    HasDiffuse = true;
                                }
                            }
                        }
                    }
                }

                if (HasMeshes)
                {
                    int MeshIndex = 0;
                    foreach (var mesh in cmb.MeshesChunk.MSHS.Meshes)
                    {
                        STGenericMaterial mat = Materials[mesh.MaterialIndex];

                        CmbMeshWrapper genericMesh = new CmbMeshWrapper(mat);
                        genericMesh.Text = $"Mesh_{MeshIndex++}_ID_{mesh.VisIndex}";
                        genericMesh.MaterialIndex = mesh.MaterialIndex;

                        var shape = cmb.MeshesChunk.SHP.SEPDs[mesh.SEPDIndex];
                        genericMesh.Mesh = shape;

                        List<ushort> SkinnedBoneTable = new List<ushort>();
                        foreach (var prim in shape.PRMS)
                        {
                            if (prim.BoneIndices != null) {
                                SkinnedBoneTable.AddRange(prim.BoneIndices);
                            }
                        }

                        //Now load the vertex and face data

                        foreach (var prm in shape.PRMS)
                        {
                            if (shape.HasPosition)
                            {
                                int VertexCount = prm.VertexCount;
                                for (int v = 0; v < VertexCount; v++)
                                {
                                    Vertex vert = new Vertex();
                                    vert.pos = new OpenTK.Vector3(
                                        prm.Vertices.Position[v].X,
                                        prm.Vertices.Position[v].Y,
                                        prm.Vertices.Position[v].Z);
                                    if(shape.HasNormal)
                                    {
                                        vert.nrm = new OpenTK.Vector3(
                                        prm.Vertices.Normal[v].X,
                                        prm.Vertices.Normal[v].Y,
                                        prm.Vertices.Normal[v].Z).Normalized();
                                    }

                                    if (shape.HasColor)
                                    {
                                        vert.col = new OpenTK.Vector4(
                                        prm.Vertices.Color[v].X,
                                        prm.Vertices.Color[v].Y,
                                        prm.Vertices.Color[v].Z,
                                        prm.Vertices.Color[v].W).Normalized();
                                    }

                                    if (shape.HasUV0){
                                        vert.uv0 = new OpenTK.Vector2(prm.Vertices.UV0[v].X, -prm.Vertices.UV0[v].Y + 1);
                                    }

                                    if (shape.HasUV1){
                                        vert.uv1 = new OpenTK.Vector2(prm.Vertices.UV1[v].X, -prm.Vertices.UV1[v].Y + 1);
                                    }

                                    if (shape.HasUV2){
                                        vert.uv2 = new OpenTK.Vector2(prm.Vertices.UV2[v].X, -prm.Vertices.UV2[v].Y + 1);
                                    }

                                    if (prm.SkinningMode == SkinningMode.Smooth)
                                    {
                                        //Indices
                                        if (shape.HasIndices)
                                        {
                                            if (shape.BoneDimensionCount >= 1)
                                                vert.boneIds.Add((int)prm.Vertices.BoneIndices[v].X);
                                            if (shape.BoneDimensionCount >= 2)
                                                vert.boneIds.Add((int)prm.Vertices.BoneIndices[v].Y);
                                            if (shape.BoneDimensionCount >= 3)
                                                vert.boneIds.Add((int)prm.Vertices.BoneIndices[v].Z);
                                            if (shape.BoneDimensionCount >= 4)
                                                vert.boneIds.Add((int)prm.Vertices.BoneIndices[v].W);
                                        }

                                        //Weights
                                        if(shape.HasWeights)
                                        {
                                            if (shape.BoneDimensionCount >= 1)
                                                vert.boneWeights.Add(prm.Vertices.BoneWeights[v].X);
                                            if (shape.BoneDimensionCount >= 2)
                                                vert.boneWeights.Add(prm.Vertices.BoneWeights[v].Y);
                                            if (shape.BoneDimensionCount >= 3)
                                                vert.boneWeights.Add(prm.Vertices.BoneWeights[v].Z);
                                            if (shape.BoneDimensionCount >= 4)
                                                vert.boneWeights.Add(prm.Vertices.BoneWeights[v].W);
                                        }
                                    }

                                    if (prm.SkinningMode == SkinningMode.Rigid)
                                    {
                                        int boneId = (int)prm.Vertices.BoneIndices[v].X;
                                        vert.boneIds.Add(boneId);
                                        vert.boneWeights.Add(1);

                                        vert.pos = OpenTK.Vector3.TransformPosition(vert.pos, Skeleton.bones[boneId].Transform);
                                        if(shape.HasNormal)
                                            vert.nrm = OpenTK.Vector3.TransformNormal(vert.nrm, Skeleton.bones[boneId].Transform);
                                    }

                                    if (prm.SkinningMode == SkinningMode.Mixed)
                                    {
                                        int boneId = prm.BoneIndices[0];
                                        vert.boneIds.Add(boneId);
                                        vert.boneWeights.Add(1);

                                        vert.pos = OpenTK.Vector3.TransformPosition(vert.pos, Skeleton.bones[boneId].Transform);
                                        if (shape.HasNormal)
                                            vert.nrm = OpenTK.Vector3.TransformNormal(vert.nrm, Skeleton.bones[boneId].Transform);
                                    }

                                    genericMesh.vertices.Add(vert);
                                }
                            }

                            STGenericPolygonGroup group = new STGenericPolygonGroup();
                            genericMesh.PolygonGroups.Add(group);

                            for (int i = 0; i < prm.FaceIndices.Count; i++)
                            {
                                group.faces.Add((int)prm.FaceIndices[i].X);
                                group.faces.Add((int)prm.FaceIndices[i].Y);
                                group.faces.Add((int)prm.FaceIndices[i].Z);
                            }
                        }

                        Renderer.Meshes.Add(genericMesh);
                        meshFolder.Nodes.Add(genericMesh);
                    }
                }

                if (meshFolder.Nodes.Count > 0)
                    Nodes.Add(meshFolder);

                if (skeletonFolder.Nodes.Count > 0)
                    Nodes.Add(skeletonFolder);

                if (materialFolder.Nodes.Count > 0)
                    Nodes.Add(materialFolder);

                if (texFolder.Nodes.Count > 0)
                    Nodes.Add(texFolder);
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            var f = new FileWriter(stream);
            f.Write(cmb.WriteFile("", cmb, false).ToArray());
        }

        public enum CMBVersion
        {
            OOT3DS,
            MM3DS,
            LM3DS,
        }

        public class CMBMaterialWrapper : CtrLibrary.H3DMaterialWrapper
        {
            private CMB ParentCMB;

            public Material CMBMaterial { get; set; }

            public override List<STGenericObject> FindMappedMeshes()
            {
                List<STGenericObject> meshes = new List<STGenericObject>();
                foreach (var mesh in ParentCMB.Renderer.Meshes)
                    if (mesh.GetMaterial() == this)
                        meshes.Add(mesh);
                return meshes;
            }

            public CMBMaterialWrapper(Material material, CMB cmb, H3DMaterial h3D) : base(h3D)
            {
                ParentCMB = cmb;
                CMBMaterial = material;
            }

            public override void OnClick(TreeView treeView)
            {
                var editor = ParentCMB.LoadEditor<CtrLibrary.Forms.BCHMaterialEditor>();
                editor.LoadMaterial(this);

                /*   STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));
                   if (editor == null)
                   {
                       editor = new STPropertyGrid();
                       LibraryGUI.LoadEditor(editor);
                   }
                   editor.Text = Text;
                   editor.Dock = DockStyle.Fill;
                   editor.LoadProperty(Material, null);*/
            }
        }

        public class CMBTextureMapWrapper : STGenericMatTexture
        {
            public CMB CMBParent;

            public int TextureIndex { get; set; }

            public TextureMapper TextureMapData;

            public override STGenericTexture GetTexture()
            {
                foreach (var tex in CMBParent.Renderer.TextureList)
                    if (tex.Text == this.Name)
                        return tex;

                return null;
            }

            public CMBTextureMapWrapper(TextureMapper texMap, CMB cmb)
            {
                CMBParent = cmb;
                TextureMapData = texMap;

                this.WrapModeS = ConvertWrapMode(TextureMapData.WrapU);
                this.WrapModeT = ConvertWrapMode(TextureMapData.WrapV);
                this.MinFilter = ConvertMinFilterMode(TextureMapData.TextureMinFilter);
                this.MagFilter = ConvertMagFilterMode(TextureMapData.TextureMagFilter);
            }

            private STTextureMinFilter ConvertMinFilterMode(ZeldaLib.CtrModelBinary.Types.TextureMinFilter PicaFilterMode)
            {
                switch (PicaFilterMode)
                {
                    case ZeldaLib.CtrModelBinary.Types.TextureMinFilter.Linear: return STTextureMinFilter.Linear;
                    case ZeldaLib.CtrModelBinary.Types.TextureMinFilter.Linear_Mipmap_Linear: return STTextureMinFilter.LinearMipMapNearest;
                    case ZeldaLib.CtrModelBinary.Types.TextureMinFilter.Linear_Mipmap_Nearest: return STTextureMinFilter.NearestMipmapLinear;
                    case ZeldaLib.CtrModelBinary.Types.TextureMinFilter.Nearest: return STTextureMinFilter.Nearest;
                    case ZeldaLib.CtrModelBinary.Types.TextureMinFilter.Nearest_Mipmap_Linear: return STTextureMinFilter.NearestMipmapLinear;
                    case ZeldaLib.CtrModelBinary.Types.TextureMinFilter.Nearest_Mipmap_Nearest: return STTextureMinFilter.NearestMipmapNearest;
                    default: return 0;
                }
            }

            private STTextureMagFilter ConvertMagFilterMode(ZeldaLib.CtrModelBinary.Types.TextureMagFilter PicaFilterMode)
            {
                switch (PicaFilterMode)
                {
                    case ZeldaLib.CtrModelBinary.Types.TextureMagFilter.Linear: return STTextureMagFilter.Linear;
                    case ZeldaLib.CtrModelBinary.Types.TextureMagFilter.Nearest: return STTextureMagFilter.Nearest;
                    default: return 0;
                }
            }

            private STTextureWrapMode ConvertWrapMode(TextureWrap PicaWrapMode)
            {
                switch (PicaWrapMode)
                {
                    case TextureWrap.Repeat: return STTextureWrapMode.Repeat;
                    case TextureWrap.Mirror: return STTextureWrapMode.Mirror;
                    case TextureWrap.ClampToBorder: return STTextureWrapMode.Clamp;
                    case TextureWrap.ClampToEdge: return STTextureWrapMode.Clamp;
                    default: return STTextureWrapMode.Repeat;
                }
            }
        }

        public class CmbMeshWrapper : GenericRenderedObject
        {
            public ZeldaLib.CtrModelBinary.CMB_SEPD Mesh { get; set; }

            STGenericMaterial material;

            public CmbMeshWrapper(STGenericMaterial mat) {
                material = mat;
            }

            public override STGenericMaterial GetMaterial()
            {
                return material;
            }
        }

        private class TextureFolder : STTextureFolder, ITextureContainer
        {
            public bool DisplayIcons => true;

            public List<STGenericTexture> TextureList
            {
                get
                {
                    List<STGenericTexture> textures = new List<STGenericTexture>();
                    foreach (STGenericTexture node in Nodes)
                        textures.Add(node);

                    return textures;
                }
                set { }
            }

            public TextureFolder(string text) : base(text)
            {

            }
        }

        public H3DMaterial ToH3DMaterial(Material mat)
        {
            H3DMaterial h3dMaterial = new H3DMaterial();
            var matParams = h3dMaterial.MaterialParams;

            if (mat.IsVertexLightingEnabled)
                matParams.Flags |= H3DMaterialFlags.IsVertexLightingEnabled;
            if (mat.IsFragmentLightingEnabled)
                matParams.Flags |= H3DMaterialFlags.IsFragmentLightingEnabled;
            if (mat.IsHemiSphereLightingEnabled)
                matParams.Flags |= H3DMaterialFlags.IsHemiSphereLightingEnabled;
            if (mat.IsHemiSphereOcclusionEnabled)
                matParams.Flags |= H3DMaterialFlags.IsHemiSphereOcclusionEnabled;

            switch (mat.LayerConfig)
            {
                case LayerConfig.LayerConfig0: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig0; break;
                case LayerConfig.LayerConfig1: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig1; break;
                case LayerConfig.LayerConfig2: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig2; break;
                case LayerConfig.LayerConfig3: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig3; break;
                case LayerConfig.LayerConfig4: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig4; break;
                case LayerConfig.LayerConfig5: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig5; break;
                case LayerConfig.LayerConfig6: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig6; break;
                case LayerConfig.LayerConfig7: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig7; break;
            }

            for (int i = 0; i < mat.TextureMappers?.Count; i++)
            {
                string texture = GetTextureName(mat.TextureMappers[i].TextureID);
                if (texture == string.Empty)
                    continue;

                if (i == 0) h3dMaterial.Texture0Name = texture;
                if (i == 0) h3dMaterial.Texture1Name = texture;
                if (i == 0) h3dMaterial.Texture2Name = texture;

                h3dMaterial.TextureMappers[i].WrapU = ConvertWrapMode(mat.TextureMappers[i].WrapU);
                h3dMaterial.TextureMappers[i].WrapV = ConvertWrapMode(mat.TextureMappers[i].WrapV);
                h3dMaterial.TextureMappers[i].MagFilter = ConvertTexMagFilter(mat.TextureMappers[i].TextureMagFilter);
                h3dMaterial.TextureMappers[i].MinFilter = ConvertTexMinFilter(mat.TextureMappers[i].TextureMinFilter);
                h3dMaterial.TextureMappers[i].LODBias = mat.TextureMappers[i].LODBias;
                h3dMaterial.TextureMappers[i].MinLOD = (byte)(mat.TextureMappers[i].MinLODBias / 255);
                h3dMaterial.TextureMappers[i].BorderColor = new SPICA.Math3D.RGBA(
                    mat.TextureMappers[i].BorderColor.R,
                    mat.TextureMappers[i].BorderColor.G,
                    mat.TextureMappers[i].BorderColor.B,
                    mat.TextureMappers[i].BorderColor.A);

                matParams.TextureCoords[i].TransformType = H3DTextureTransformType.DccMaya;
                matParams.TextureCoords[i].MappingType = H3DTextureMappingType.UvCoordinateMap;

                matParams.TextureCoords[i].Scale = new System.Numerics.Vector2(
                    mat.TextureCoords[i].Scale.X, mat.TextureCoords[i].Scale.Y);
                matParams.TextureCoords[i].Translation = new System.Numerics.Vector2(
                    mat.TextureCoords[i].Translation.X, mat.TextureCoords[i].Translation.Y);

                matParams.TextureCoords[i].Rotation = mat.TextureCoords[i].Rotation;
            }

            matParams.DiffuseColor = ConvertRGBA(mat.DiffuseColor);
            matParams.Specular0Color = ConvertRGBA(mat.Specular0Color);
            matParams.Specular1Color = ConvertRGBA(mat.Specular1Color);
            matParams.EmissionColor = ConvertRGBA(mat.EmissionColor);
            matParams.Constant0Color = ConvertRGBA(mat.ConstantColors[0]);
            matParams.Constant1Color = ConvertRGBA(mat.ConstantColors[1]);
            matParams.Constant2Color = ConvertRGBA(mat.ConstantColors[2]);
            matParams.Constant3Color = ConvertRGBA(mat.ConstantColors[3]);
            matParams.Constant4Color = ConvertRGBA(mat.ConstantColors[4]);
            matParams.Constant5Color = ConvertRGBA(mat.ConstantColors[5]);
            matParams.BlendColor = ConvertRGBA(mat.BlendColor);
            matParams.TexEnvBufferColor = ConvertRGBA(mat.BufferColor);

            if (mat.CullMode == CullMode.Back)
                matParams.FaceCulling = PICAFaceCulling.BackFace;
            else if (mat.CullMode == CullMode.Front)
                matParams.FaceCulling = PICAFaceCulling.FrontFace;
            else
                matParams.FaceCulling = PICAFaceCulling.Never;

            matParams.AlphaTest.Enabled = mat.AlphaTest.Enabled;
            matParams.AlphaTest.Function = ConvertTestFunction(mat.AlphaTest.Function);
            matParams.AlphaTest.Reference = mat.AlphaTest.Reference;
            matParams.BlendFunction.ColorSrcFunc = ConvertBlendFunc(mat.BlendFunction.ColorSrcFunc);
            matParams.BlendFunction.ColorDstFunc = ConvertBlendFunc(mat.BlendFunction.ColorDstFunc);
            matParams.BlendFunction.AlphaSrcFunc = ConvertBlendFunc(mat.BlendFunction.AlphaSrcFunc);
            matParams.BlendFunction.AlphaDstFunc = ConvertBlendFunc(mat.BlendFunction.AlphaDstFunc);

            for (int i = 0; i < mat.TexEnvStages.Count; i++)
            {
                var combiner = mat.TexEnvStages[i];
                var h3dStage = new PICATexEnvStage();
                    h3dStage.Source.Color[0] = ConvertCombinerSrc[combiner.Source.Color[0]];
                    h3dStage.Source.Color[1] = ConvertCombinerSrc[combiner.Source.Color[1]];
                    h3dStage.Source.Color[2] = ConvertCombinerSrc[combiner.Source.Color[2]];
                    h3dStage.Source.Alpha[0] = ConvertCombinerSrc[combiner.Source.Alpha[0]];
                    h3dStage.Source.Alpha[1] = ConvertCombinerSrc[combiner.Source.Alpha[1]];
                    h3dStage.Source.Alpha[2] = ConvertCombinerSrc[combiner.Source.Alpha[2]];
                    h3dStage.Operand.Alpha[0] = ConvertConvertCombinerAlphaOp[combiner.Operand.Alpha[0]];
                    h3dStage.Operand.Alpha[1] = ConvertConvertCombinerAlphaOp[combiner.Operand.Alpha[1]];
                    h3dStage.Operand.Alpha[2] = ConvertConvertCombinerAlphaOp[combiner.Operand.Alpha[2]];
                    h3dStage.Operand.Color[0] = ConvertConvertCombinerColorOp[combiner.Operand.Color[0]];
                    h3dStage.Operand.Color[1] = ConvertConvertCombinerColorOp[combiner.Operand.Color[1]];
                    h3dStage.Operand.Color[2] = ConvertConvertCombinerColorOp[combiner.Operand.Color[2]];
                    h3dStage.Scale.Color = ConvertScale[combiner.Scale.Color];
                    h3dStage.Scale.Alpha = ConvertScale[combiner.Scale.Alpha];
                    h3dStage.Combiner.Alpha = ConvertConvertCombiner[combiner.Combiner.Alpha];
                    h3dStage.Combiner.Color = ConvertConvertCombiner[combiner.Combiner.Color];

                matParams.TexEnvStages[i] = h3dStage;
            }

            matParams.LUTInputAbsolute.Dist0 = mat.Distibution0SamplerIsAbs;
            matParams.LUTInputAbsolute.Dist1 = mat.Distibution1SamplerIsAbs;
            matParams.LUTInputAbsolute.ReflecR = mat.ReflectanceRSamplerIsAbs;
            matParams.LUTInputAbsolute.ReflecG = mat.ReflectanceGSamplerIsAbs;
            matParams.LUTInputAbsolute.ReflecB = mat.ReflectanceBSamplerIsAbs;
            matParams.LUTInputAbsolute.Fresnel = mat.FresnelSamplerIsAbs;

            return h3dMaterial;
        }

        private Dictionary<TextureCombinerMode, PICATextureCombinerMode> ConvertConvertCombiner =
         new Dictionary<TextureCombinerMode, PICATextureCombinerMode>()
         {
                    { TextureCombinerMode.Add, PICATextureCombinerMode.Add },
                    { TextureCombinerMode.AddMult, PICATextureCombinerMode.AddMult },
                    { TextureCombinerMode.AddSigned, PICATextureCombinerMode.AddSigned },
                    { TextureCombinerMode.DotProduct3Rgb, PICATextureCombinerMode.DotProduct3Rgb },
                    { TextureCombinerMode.DotProduct3Rgba, PICATextureCombinerMode.DotProduct3Rgba },
                    { TextureCombinerMode.Interpolate, PICATextureCombinerMode.Interpolate },
                    { TextureCombinerMode.Modulate, PICATextureCombinerMode.Modulate },
                    { TextureCombinerMode.MultAdd, PICATextureCombinerMode.MultAdd },
                    { TextureCombinerMode.Replace, PICATextureCombinerMode.Replace },
                    { TextureCombinerMode.Subtract, PICATextureCombinerMode.Subtract },
         };

        private Dictionary<TextureCombinerSource, PICATextureCombinerSource> ConvertCombinerSrc =
            new Dictionary<TextureCombinerSource, PICATextureCombinerSource>()
            {
                    { TextureCombinerSource.Constant, PICATextureCombinerSource.Constant },
                    { TextureCombinerSource.FragmentPrimaryColor, PICATextureCombinerSource.FragmentPrimaryColor },
                    { TextureCombinerSource.FragmentSecondaryColor, PICATextureCombinerSource.FragmentSecondaryColor },
                    { TextureCombinerSource.Previous, PICATextureCombinerSource.Previous },
                    { TextureCombinerSource.PreviousBuffer, PICATextureCombinerSource.PreviousBuffer },
                    { TextureCombinerSource.PrimaryColor, PICATextureCombinerSource.PrimaryColor },
                    { TextureCombinerSource.Texture0, PICATextureCombinerSource.Texture0 },
                    { TextureCombinerSource.Texture1, PICATextureCombinerSource.Texture1 },
                    { TextureCombinerSource.Texture2, PICATextureCombinerSource.Texture2 },
                    { TextureCombinerSource.Texture3, PICATextureCombinerSource.Texture3 },
            };

        private Dictionary<TextureCombinerAlphaOp, PICATextureCombinerAlphaOp> ConvertConvertCombinerAlphaOp =
            new Dictionary<TextureCombinerAlphaOp, PICATextureCombinerAlphaOp>()
            {
                    { TextureCombinerAlphaOp.OneMinusAlpha, PICATextureCombinerAlphaOp.OneMinusAlpha },
                    { TextureCombinerAlphaOp.OneMinusRed, PICATextureCombinerAlphaOp.OneMinusRed },
                    { TextureCombinerAlphaOp.OneMinusGreen, PICATextureCombinerAlphaOp.OneMinusGreen },
                    { TextureCombinerAlphaOp.OneMinusBlue, PICATextureCombinerAlphaOp.OneMinusBlue },
                    { TextureCombinerAlphaOp.Alpha, PICATextureCombinerAlphaOp.Alpha },
                    { TextureCombinerAlphaOp.Red, PICATextureCombinerAlphaOp.Red },
                    { TextureCombinerAlphaOp.Green, PICATextureCombinerAlphaOp.Green },
                    { TextureCombinerAlphaOp.Blue, PICATextureCombinerAlphaOp.Blue },
            };

        private Dictionary<TextureCombinerColorOp, PICATextureCombinerColorOp> ConvertConvertCombinerColorOp =
            new Dictionary<TextureCombinerColorOp, PICATextureCombinerColorOp>()
            {
                    { TextureCombinerColorOp.OneMinusColor, PICATextureCombinerColorOp.OneMinusColor },
                    { TextureCombinerColorOp.OneMinusAlpha, PICATextureCombinerColorOp.OneMinusAlpha },
                    { TextureCombinerColorOp.OneMinusRed, PICATextureCombinerColorOp.OneMinusRed },
                    { TextureCombinerColorOp.OneMinusGreen, PICATextureCombinerColorOp.OneMinusGreen },
                    { TextureCombinerColorOp.OneMinusBlue, PICATextureCombinerColorOp.OneMinusBlue },
                    { TextureCombinerColorOp.Alpha, PICATextureCombinerColorOp.Alpha },
                    { TextureCombinerColorOp.Color, PICATextureCombinerColorOp.Color },
                    { TextureCombinerColorOp.Red, PICATextureCombinerColorOp.Red },
                    { TextureCombinerColorOp.Green, PICATextureCombinerColorOp.Green },
                    { TextureCombinerColorOp.Blue, PICATextureCombinerColorOp.Blue },
            };

        private Dictionary<TextureCombinerScale, PICATextureCombinerScale> ConvertScale =
            new Dictionary<TextureCombinerScale, PICATextureCombinerScale>()
            {
                    { TextureCombinerScale.One, PICATextureCombinerScale.One },
                    { TextureCombinerScale.Two, PICATextureCombinerScale.Two },
                    { TextureCombinerScale.Four, PICATextureCombinerScale.Four },
            };

        private PICABlendFunc ConvertBlendFunc(BlendFunc factor)
        {
            switch (factor)
            {
                case BlendFunc.ConstantAlpha: return PICABlendFunc.ConstantAlpha;
                case BlendFunc.ConstantColor: return PICABlendFunc.ConstantColor;
                case BlendFunc.DestinationAlpha: return PICABlendFunc.DestinationAlpha;
                case BlendFunc.DestinationColor: return PICABlendFunc.DestinationColor;
                case BlendFunc.One: return PICABlendFunc.One;
                case BlendFunc.OneMinusConstantAlpha: return PICABlendFunc.OneMinusConstantAlpha;
                case BlendFunc.OneMinusConstantColor: return PICABlendFunc.OneMinusConstantColor;
                case BlendFunc.OneMinusDestinationAlpha: return PICABlendFunc.OneMinusDestinationAlpha;
                case BlendFunc.OneMinusDestinationColor: return PICABlendFunc.OneMinusDestinationColor;
                case BlendFunc.OneMinusSourceAlpha: return PICABlendFunc.OneMinusSourceAlpha;
                case BlendFunc.OneMinusSourceColor: return PICABlendFunc.OneMinusSourceColor;
                case BlendFunc.SourceAlpha: return PICABlendFunc.SourceAlpha;
                case BlendFunc.SourceColor: return PICABlendFunc.SourceColor;
                case BlendFunc.SourceAlphaSaturate: return PICABlendFunc.SourceAlphaSaturate;
                case BlendFunc.Zero: return PICABlendFunc.Zero;
                default: return PICABlendFunc.Zero;
            }
        }

        private PICATestFunc ConvertTestFunction(TestFunc func)
        {
            switch (func)
            {
                case TestFunc.Always: return PICATestFunc.Always;
                case TestFunc.Equal: return PICATestFunc.Equal;
                case TestFunc.Gequal: return PICATestFunc.Gequal;
                case TestFunc.Greater: return PICATestFunc.Greater;
                case TestFunc.Lequal: return PICATestFunc.Lequal;
                case TestFunc.Less: return PICATestFunc.Less;
                case TestFunc.Never: return PICATestFunc.Never;
                case TestFunc.Notequal: return PICATestFunc.Notequal;
                default: return PICATestFunc.Greater;
            }
        }

        private SPICA.Math3D.RGBA ConvertRGBA(RGBA color)
        {
            return new SPICA.Math3D.RGBA(color.R, color.G, color.B, color.A);
        }

        private H3DTextureMagFilter ConvertTexMagFilter(ZeldaLib.CtrModelBinary.Types.TextureMagFilter filterMode)
        {
            switch (filterMode)
            {
                case ZeldaLib.CtrModelBinary.Types.TextureMagFilter.Linear: return H3DTextureMagFilter.Linear;
                case ZeldaLib.CtrModelBinary.Types.TextureMagFilter.Nearest: return H3DTextureMagFilter.Nearest;
                default: return H3DTextureMagFilter.Linear;

            }
        }

        private H3DTextureMinFilter ConvertTexMinFilter(ZeldaLib.CtrModelBinary.Types.TextureMinFilter filterMode)
        {
            switch (filterMode)
            {
                case ZeldaLib.CtrModelBinary.Types.TextureMinFilter.Linear: return H3DTextureMinFilter.Linear;
                case ZeldaLib.CtrModelBinary.Types.TextureMinFilter.Linear_Mipmap_Linear: return H3DTextureMinFilter.LinearMipmapLinear;
                case ZeldaLib.CtrModelBinary.Types.TextureMinFilter.Linear_Mipmap_Nearest: return H3DTextureMinFilter.LinearMipmapNearest;
                case ZeldaLib.CtrModelBinary.Types.TextureMinFilter.Nearest: return H3DTextureMinFilter.Nearest;
                case ZeldaLib.CtrModelBinary.Types.TextureMinFilter.Nearest_Mipmap_Linear: return H3DTextureMinFilter.NearestMipmapLinear;
                case ZeldaLib.CtrModelBinary.Types.TextureMinFilter.Nearest_Mipmap_Nearest: return H3DTextureMinFilter.NearestMipmapNearest;
                default: return H3DTextureMinFilter.Linear;

            }
        }

        private PICATextureWrap ConvertWrapMode(TextureWrap wrapMode)
        {
            switch (wrapMode)
            {
                case TextureWrap.ClampToBorder: return PICATextureWrap.ClampToBorder;
                case TextureWrap.ClampToEdge: return PICATextureWrap.ClampToEdge;
                case TextureWrap.Mirror: return PICATextureWrap.Mirror;
                case TextureWrap.Repeat: return PICATextureWrap.Repeat;
                default: return PICATextureWrap.Repeat;
            }
        }

        private string GetTextureName(int index)
        {
            if (index != -1 && index < cmb.TexturesChunk?.Textures?.Count)
                return cmb.TexturesChunk.Textures[index].Name;
            else
                return "";
        }
    }
}

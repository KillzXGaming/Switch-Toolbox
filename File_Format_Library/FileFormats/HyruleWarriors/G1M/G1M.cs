using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using Toolbox.Library.Rendering;
using OpenTK;

namespace HyruleWarriors.G1M
{
    public class G1M : TreeNodeFile, IFileFormat, IExportableModel
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "G1M Model" };
        public string[] Extension { get; set; } = new string[] { "*.g1m" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "_M1G") || reader.CheckSignature(4, "G1M_");
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

        //Check for the viewport in the object editor
        //This is attached to it to load multiple file formats within the object editor to the viewer
        Viewport viewport
        {
            get
            {
                var editor = LibraryGUI.GetObjectEditor();
                return editor.GetViewport();
            }
            set
            {
                var editor = LibraryGUI.GetObjectEditor();
                editor.LoadViewport(value);
            }
        }

        bool DrawablesLoaded = false;
        public override void OnClick(TreeView treeView)
        {
            //Make sure opengl is enabled
            if (Runtime.UseOpenGL)
            {
                //Open the viewport
                if (viewport == null)
                {
                    viewport = new Viewport(ObjectEditor.GetDrawableContainers());
                    viewport.Dock = DockStyle.Fill;
                }

                //Make sure to load the drawables only once so set it to true!
                if (!DrawablesLoaded)
                {
                    ObjectEditor.AddContainer(DrawableContainer);
                    DrawablesLoaded = true;
                }

                //Reload which drawable to display
                viewport.ReloadDrawables(DrawableContainer);
                LibraryGUI.LoadEditor(viewport);

                viewport.Text = Text;
            }
        }

        public IEnumerable<STGenericObject> ExportableMeshes => Model.GenericMeshes;
        public IEnumerable<STGenericMaterial> ExportableMaterials => Model.Materials;
        public IEnumerable<STGenericTexture> ExportableTextures => TextureList;
        public STSkeleton ExportableSkeleton => G1MSkeleton.GenericSkeleton;

        public List<STGenericTexture> TextureList
        {
            get
            {
                //Export all textures that use the same archive
                List<STGenericTexture> textures = new List<STGenericTexture>();
                foreach (var container in FirstPlugin.PluginRuntime.G1TextureContainers)
                {
                    if (this.IFileInfo.ArchiveParent == container.IFileInfo.ArchiveParent) {
                        foreach (var texture in container.TextureList)
                            textures.Add(texture);
                    }
                }
                return textures;
            }
        }

        public G1M_Renderer Renderer;

        public DrawableContainer DrawableContainer = new DrawableContainer();

        public void Load(System.IO.Stream stream)
        {
            Read(new FileReader(stream));
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public G1MS G1MSkeleton { get; set; }
        public G1MG Model { get; set; }
        public NUNO NUNO { get; set; }
        public NUNV NUNV { get; set; }

        TreeNode meshNode;

        public void Read(FileReader reader)
        {
            Renderer = new G1M_Renderer();
            Renderer.G1MFile = this;
            DrawableContainer = new DrawableContainer();
            DrawableContainer.Name = FileName;
            DrawableContainer.Drawables.Add(Renderer);

            reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            string Magic = reader.ReadString(4);

            if (Magic == "_M1G")
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
            else
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            uint Version = reader.ReadUInt32();
            uint FileSize = reader.ReadUInt32();
            uint firstChunkOffset = reader.ReadUInt32();
            uint padding = reader.ReadUInt32();
            uint numChunks = reader.ReadUInt32();

            switch (Version)
            {
                case 0x30303334:break;
                case 0x30303335: break;
                case 0x30303336: break;
                case 0x30303337: break;
                default: break;
            }

            reader.SeekBegin(firstChunkOffset);
            for (int i = 0; i < numChunks; i++)
            {
                G1MChunkCommon chunk = new G1MChunkCommon();
                chunk.ChunkPosition = reader.Position;
                chunk.Magic = reader.ReadString(4, Encoding.ASCII);
                chunk.ChunkVersion = reader.ReadUInt32();
                chunk.ChunkSize = reader.ReadUInt32();

                Console.WriteLine("chunkMagic " +   chunk.Magic);
                if (chunk.Magic == "G1MF")
                {

                }
                else if (chunk.Magic == "SM1G" || chunk.Magic == "G1MS")
                {
                    G1MSkeleton = new G1MS(reader);
                    Renderer.Skeleton = G1MSkeleton.GenericSkeleton;
                    DrawableContainer.Drawables.Add(G1MSkeleton.GenericSkeleton);

                    TreeNode skeleton = new TreeNode("Skeleton");
                    Nodes.Add(skeleton);
                    foreach (var bn in G1MSkeleton.GenericSkeleton.bones)
                        if (bn.Parent == null)
                        {
                            skeleton.Nodes.Add(bn);
                        }
                }
                else if (chunk.Magic == "G1MM")
                {

                }
                else if (chunk.Magic == "G1MG")
                {
                    Model = new G1MG(reader);
                    Renderer.Meshes.AddRange(Model.GenericMeshes);

                    meshNode = new TreeNode("Meshes");
                    Nodes.Add(meshNode);
                    foreach (var mesh in Model.GenericMeshes)
                        meshNode.Nodes.Add(mesh);

                    if (G1MSkeleton != null)
                    {
                        foreach (var mesh in Model.GenericMeshes)
                        {
                            bool isSingleBind = false;

                            if (isSingleBind)
                            {
                                for (int v = 0; v < mesh.vertices.Count; v++)
                                {
                                    var boneId = mesh.vertices[v].boneIds[0];
                                    var transform = G1MSkeleton.GenericSkeleton.bones[boneId].Transform;
                                    mesh.vertices[v].pos = Vector3.TransformPosition(
                                        mesh.vertices[v].pos, transform);
                                    mesh.vertices[v].nrm = Vector3.TransformNormal(
                                        mesh.vertices[v].nrm, transform);
                                }
                            }
                        }
                    }
                }
                else if (chunk.Magic == "COLL")
                {

                }
                else if (chunk.Magic == "HAIR")
                {

                }
                else if (chunk.Magic == "NUNO")
                {
                    NUNO = new NUNO(reader, chunk.ChunkVersion);
                }
                else if (chunk.Magic == "NUNS")
                {

                }
                else if (chunk.Magic == "NUNV")
                {
                    NUNV = new NUNV(reader, chunk.ChunkVersion);
                }
                else if (chunk.Magic == "EXTR")
                {

                }
                else
                {

                }
                reader.SeekBegin(chunk.ChunkPosition + chunk.ChunkSize);
            }

            ComputeClothDrivers();
            SetLevelOfDetailGroups();
        }

        /// <summary>
        /// Computes mesh and bone cloth drivers
        /// </summary>
        public void ComputeClothDrivers()
        {
            var boneList = G1MSkeleton.GenericSkeleton.bones;

            var nunProps = new List<NUNO.NUNOType0303Struct>();
            uint nunoOffset = 0;
            if (NUNO != null)
            {
                nunoOffset = (uint)NUNO.NUNO0303StructList.Count;
                foreach (var nuno0303 in NUNO.NUNO0303StructList)
                    nunProps.Add(nuno0303);
            }
            if (NUNV != null) {
                foreach (var nuno0303 in NUNV.NUNV0303StructList)
                    nunProps.Add(nuno0303);
            }

            foreach (var prop in nunProps)
            {
                int boneStart = boneList.Count;
                var parentBone = Model.JointInfos[prop.BoneParentID - 1].JointIndices[0];

                GenericRenderedObject mesh = new GenericRenderedObject();
                mesh.Text = $"driver_{boneList.Count}";
                mesh.Checked = true;
                Renderer.Meshes.Add(mesh);
                meshNode.Nodes.Add(mesh);

                var polyGroup = new STGenericPolygonGroup();
                polyGroup.Material = new STGenericMaterial();
                polyGroup.Material.Text = "driver_cloth";
                polyGroup.PrimativeType = STPrimitiveType.Triangles;
                mesh.PolygonGroups.Add(polyGroup);

                for (int p = 0; p < prop.Points.Length; p++) {
                    var point = prop.Points[p];
                    var link = prop.Influences[p];

                    STBone b = new STBone(G1MSkeleton.GenericSkeleton);
                    b.Text = $"CP_{boneList.Count}";
                    b.FromTransform(OpenTK.Matrix4.Identity);
                    b.Position = point.Xyz;
                    b.parentIndex = link.P3;
                    if (b.parentIndex == -1)
                        b.parentIndex = (int)parentBone;
                    else
                    {
                        b.parentIndex += boneStart;
                        b.Position = OpenTK.Vector3.TransformPosition(
                            point.Xyz, G1MSkeleton.GenericSkeleton.GetBoneTransform((int)parentBone) *
                            G1MSkeleton.GenericSkeleton.GetBoneTransform(b.parentIndex).Inverted());
                    }

                    boneList.Add(b);

                    G1MSkeleton.GenericSkeleton.reset();
                    G1MSkeleton.GenericSkeleton.update();

                    mesh.vertices.Add(new Vertex()
                    {
                        pos = Vector3.TransformPosition(Vector3.Zero,
                        G1MSkeleton.GenericSkeleton.GetBoneTransform(boneList.Count - 1)),
                        boneWeights = new List<float>() { 1 },
                        boneIds = new List<int>() { boneList.Count - 1 },
                    });

                    if (link.P1 > 0 && link.P3 > 0)
                    {
                        polyGroup.faces.Add(p);
                        polyGroup.faces.Add(link.P1);
                        polyGroup.faces.Add(link.P3);
                    }
                    if (link.P2 > 0 && link.P4 > 0)
                    {
                        polyGroup.faces.Add(p);
                        polyGroup.faces.Add(link.P2);
                        polyGroup.faces.Add(link.P4);
                    }
                }

                mesh.CalculateNormals();
            }
        }

        private void SetLevelOfDetailGroups()
        {
            foreach (var group in Model.LodGroups[0].Meshes)
            {
                var isCloth = (group.ID & 0xF) == 1;
                var isPoint = (group.ID & 0xF) == 2;
                var NunoSection = (group.ID2 - (group.ID2 % 10000)) / 10000;

                foreach (var polyindex in group.Indices)
                {
                    if (polyindex > Model.SubMeshInfos.Length)
                        continue;

                    var poly = Model.SubMeshInfos[polyindex];
                    var mesh = Model.GenericMeshes[(int)polyindex];

                    mesh.Text = $"{group.Name}_" + (isPoint ? "point" : "") + (isCloth ? "cloth" : "") + $"_{polyindex}";
                    mesh.Text = mesh.Text.Replace(@"\p{C}+", string.Empty);

                    if (isPoint)
                    {
                        for (int i = 0; i < mesh.vertices.Count; i++)
                        {
                            var vert = mesh.vertices[i];
                            vert.pos = Vector3.TransformPosition(vert.pos,
                                G1MSkeleton.GenericSkeleton.GetBoneTransform((int)Model.JointInfos[poly.IndexIntoJointMap].JointIndices[0]));
                            vert.nrm = Vector3.TransformNormal(vert.nrm,
                                G1MSkeleton.GenericSkeleton.GetBoneTransform((int)Model.JointInfos[poly.IndexIntoJointMap].JointIndices[0]));

                            mesh.vertices[i] = vert;
                        }
                    }
                    if (isCloth)
                    {

                    }
                }
            }
        }
    }
}

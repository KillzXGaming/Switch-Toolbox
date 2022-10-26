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
using CafeLibrary.M2;
using System.IO;

namespace DKCTF
{
    public class CCharacter : TreeNodeFile
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Character Model" };
        public string[] Extension { get; set; } = new string[] { "*.char" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                bool IsForm = reader.CheckSignature(4, "RFRM");
                bool FormType = reader.CheckSignature(4, "CHAR", 20);

                return IsForm && FormType;
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

        public IEnumerable<STGenericObject> ExportableMeshes => Model.Objects;
        public IEnumerable<STGenericMaterial> ExportableMaterials => Model.Materials;
        public IEnumerable<STGenericTexture> ExportableTextures => TextureList;
        public STSkeleton ExportableSkeleton => Model.GenericSkeleton;

        public GenericModelRenderer Renderer;

        public DrawableContainer DrawableContainer = new DrawableContainer();

        public List<STGenericTexture> TextureList = new List<STGenericTexture>();

        public CHAR CharData { get; set; }

        public STGenericModel Model;

        TreeNode modelFolder = new TreeNode("Models");

        TreeNode skeletonFolder = new STTextureFolder("Skeleton");

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            Renderer = new GenericModelRenderer();
            CharData = new CHAR(stream);


            Nodes.Add(modelFolder);
            Nodes.Add(skeletonFolder);


            Model = ToGeneric();

            DrawableContainer = new DrawableContainer();
            DrawableContainer.Name = FileName;
            DrawableContainer.Drawables.Add(Renderer);
            DrawableContainer.Drawables.Add(Model.GenericSkeleton);
        }

        public void LoadModels(PAK pakFile)
        {
            var skeleton = pakFile.SkeletonFiles[CharData.SkeletonFileID.ToString()];
            var skel = new SKEL(new MemoryStream(skeleton.FileData));

            skeletonFolder.Nodes.Clear();
            foreach (var bone in skel.JointNames)
            {
                TreeNode boneNode = new TreeNode(bone);
                skeletonFolder.Nodes.Add(boneNode);
            }

            foreach (var model in CharData.Models)
            {
                var mod = pakFile.ModelFiles[model.FileID.ToString()].OpenFile() as CModel;
                mod.LoadTextures(pakFile.TextureFiles);

                modelFolder.Nodes.Add(mod);
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public STGenericModel ToGeneric()
        {
            var model = new STGenericModel();
            model.GenericSkeleton = new STSkeleton();

            List<GenericRenderedObject> meshes = new List<GenericRenderedObject>();
            List<STGenericMaterial> materials = new List<STGenericMaterial>();

            model.Objects = meshes;
            model.Name = this.FileName;
            return model;
        }
    }
}

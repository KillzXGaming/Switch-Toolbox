using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Rendering;
using Switch_Toolbox.Library.Forms;
using SuperBMDLib.Geometry;

namespace FirstPlugin
{
    public class BMDShapeWrapper : GenericRenderedObject
    {
        SuperBMDLib.Model ParentModel;
        Shape BMDShape;

        private STGenericMaterial material;
        public BMDShapeWrapper(Shape shape, SuperBMDLib.Model model, int Index)
        {
            BMDShape = shape;
            ParentModel = model;
            material = new STGenericMaterial();
        //   material.Text = $"Material {Index}";

            var mat = model.Materials.m_Materials[Index];

            int textureUnit = 1;
            if (mat.TextureIndices[0] != -1)
            {
                int texIndex = mat.TextureIndices[0];

                STGenericMatTexture matTexture = new STGenericMatTexture();
                matTexture.Name = ParentModel.Textures[texIndex].Name;
                matTexture.Type = STGenericMatTexture.TextureType.Diffuse;
                matTexture.textureUnit = textureUnit++;
                matTexture.wrapModeS = 0;
                matTexture.wrapModeT = 0;
                material.TextureMaps.Add(matTexture);
            }

         
        }

        public override STGenericMaterial GetMaterial()
        {
            return material;
        }

        public override void OnClick(TreeView treeView)
        {
            STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                LibraryGUI.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.LoadProperty(BMDShape, null);
        }
    }
}

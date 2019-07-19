using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;
using Toolbox.Library.Forms;
using SuperBMDLib.Geometry;

namespace FirstPlugin
{
    public class BMDShapeWrapper : GenericRenderedObject
    {
        SuperBMDLib.Model ParentModel;
        Shape BMDShape;

        private STGenericMaterial material;
        public BMDShapeWrapper(Shape shape, SuperBMDLib.Model model, BMDMaterialWrapper mat)
        {
            BMDShape = shape;
            ParentModel = model;
            material = mat;
        }

        public override STGenericMaterial GetMaterial()
        {
            return material;
        }

        public void SetMaterial(STGenericMaterial mat)
        {
            material = mat;
        }

        public override void OnClick(TreeView treeView)
        {
         /*   STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                LibraryGUI.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.LoadProperty(BMDShape, null);(*/
        }
    }
}

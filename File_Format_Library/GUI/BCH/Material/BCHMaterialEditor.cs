using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SPICA.Formats.CtrH3D.Model.Material;
using Toolbox.Library;
using Toolbox.Library.Forms;

namespace FirstPlugin.CtrLibrary.Forms
{
    public partial class BCHMaterialEditor : UserControl
    {
        private H3DMaterial ActiveMaterial;
        private H3DMaterialWrapper ActiveMaterialWrapper;

        private H3DMaterialParams ActiveMaterialParams => ActiveMaterial.MaterialParams;

        private int SelectedIndex = 0;

        public BCHMaterialEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
        }

        public void LoadMaterial(H3DMaterialWrapper wrapper) {
            ActiveMaterialWrapper = wrapper;
            ActiveMaterial = wrapper.Material;

            stTabControl1.SelectedIndex = SelectedIndex; 
            UpdateTab();
        }

        private void stTabControl1_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateTab();
        }

        private void UpdateTab()
        {
            if (stTabControl1.SelectedIndex == 0)
                AddTab<BCHMaterialGeneralEditor>(tabPage1);
            else if (stTabControl1.SelectedIndex == 1)
                AddTab<BCHTextureMapEditor>(tabPage2);
            else if (stTabControl1.SelectedIndex == 2)
                AddTab<BCHMaterialColorEditor>(tabPage6);
            else if (stTabControl1.SelectedIndex == 3)
                AddTab<BCHMaterialFragmentEditor>(tabPage3);
            else if (stTabControl1.SelectedIndex == 4)
                AddTab<BCHMaterialBlendingEditor>(tabPage4);
            else if (stTabControl1.SelectedIndex == 5)
                AddTab<BCHMaterialShaderParamEditor>(tabPage5);
            else if (stTabControl1.SelectedIndex == 6)
            {
                BCHUserDataEditor usdEditor = AddTab<BCHUserDataEditor>(tabPage7);
                usdEditor.LoadUserData(ActiveMaterial.MaterialParams.MetaData);
            }

            if (stTabControl1.SelectedIndex >= 0)
                SelectedIndex = stTabControl1.SelectedIndex;
        }

        private T AddTab<T>(TabPage page) where T : UserControl, new()
        {
            foreach (var control in stTabControl1.SelectedTab.Controls)
                if (control.GetType() == typeof(T))
                {
                    var currentEditor = control as T;
                    if (currentEditor is IMaterialLoader)
                        ((IMaterialLoader)currentEditor).LoadMaterial(ActiveMaterialWrapper);
                    return currentEditor;
                }

            T editor = new T();

            if (editor is IMaterialLoader)
                ((IMaterialLoader)editor).LoadMaterial(ActiveMaterialWrapper);

            page.Controls.Add(editor);

            return editor;
        }
    }
}

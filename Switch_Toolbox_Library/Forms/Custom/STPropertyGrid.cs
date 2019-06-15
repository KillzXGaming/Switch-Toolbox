using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    public partial class STPropertyGrid : UserControl
    {
        public STPropertyGrid()
        {
            InitializeComponent();

            propertyGrid1.PropertySort = PropertySort.Categorized;

            if (FormThemes.ActivePreset != FormThemes.Preset.White)
            {
                this.propertyGrid1.CategoryForeColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormForeColor;
                this.propertyGrid1.CategorySplitterColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
                this.propertyGrid1.CommandsActiveLinkColor = System.Drawing.Color.Red;
                this.propertyGrid1.CommandsBorderColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.MDIParentBackColor;
                this.propertyGrid1.DisabledItemForeColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.DisabledItemColor;
                this.propertyGrid1.HelpBackColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
                this.propertyGrid1.HelpBorderColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
                this.propertyGrid1.HelpForeColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormForeColor;
                this.propertyGrid1.LineColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
                this.propertyGrid1.ViewBackColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.MDIChildBorderColor;
                this.propertyGrid1.ViewBorderColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
                this.propertyGrid1.ViewForeColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormForeColor;
            }
        }

        public bool ShowHintDisplay
        {
            get
            {
                return propertyGrid1.HelpVisible;
            }
            set
            {
                propertyGrid1.HelpVisible = value;
            }
        }

        Action OnPropertyChanged;
        Action OnPropertyChanged2;

        public void LoadProperty(object selectedObject, Action onPropertyChanged = null, Action onPropertyChanged2 = null)
        {
            OnPropertyChanged = onPropertyChanged;
            OnPropertyChanged2 = onPropertyChanged2;

            propertyGrid1.SelectedObject = selectedObject;
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (OnPropertyChanged != null) OnPropertyChanged();
            if (OnPropertyChanged2 != null) OnPropertyChanged2();
        }
    }
}

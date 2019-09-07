using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class STPropertyGrid : UserControl
    {
        public STPropertyGrid()
        {
            InitializeComponent();

            propertyGrid1.PropertySort = PropertySort.Categorized;

            if (FormThemes.ActivePreset != FormThemes.Preset.White)
            {
                this.propertyGrid1.CategoryForeColor = Toolbox.Library.Forms.FormThemes.BaseTheme.FormForeColor;
                this.propertyGrid1.CategorySplitterColor = Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
                this.propertyGrid1.CommandsActiveLinkColor = System.Drawing.Color.Red;
                this.propertyGrid1.CommandsBorderColor = Toolbox.Library.Forms.FormThemes.BaseTheme.MDIParentBackColor;
                this.propertyGrid1.DisabledItemForeColor = Toolbox.Library.Forms.FormThemes.BaseTheme.DisabledItemColor;
                this.propertyGrid1.HelpBackColor = Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
                this.propertyGrid1.HelpBorderColor = Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
                this.propertyGrid1.HelpForeColor = Toolbox.Library.Forms.FormThemes.BaseTheme.FormForeColor;
                this.propertyGrid1.LineColor = Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
                this.propertyGrid1.ViewBackColor = Toolbox.Library.Forms.FormThemes.BaseTheme.MDIChildBorderColor;
                this.propertyGrid1.ViewBorderColor = Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
                this.propertyGrid1.ViewForeColor = Toolbox.Library.Forms.FormThemes.BaseTheme.FormForeColor;
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

        public void UpdateProperties()
        {
            propertyGrid1.Invalidate();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Console.WriteLine("PropertyValueChanged");

            OnPropertyChanged?.Invoke();
            OnPropertyChanged2?.Invoke();
        }
    }
}

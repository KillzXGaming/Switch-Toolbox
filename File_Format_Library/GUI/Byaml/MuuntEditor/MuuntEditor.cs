using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using WeifenLuo.WinFormsUI.Docking;
using FirstPlugin;

namespace FirstPlugin.MuuntEditor
{
    public partial class MuuntEditor : Form
    {
        public List<ObjectGroup> Groups = new List<ObjectGroup>();
        public EventHandler ObjectSelected;

        private List<IMuuntLoader> Plugins = new List<IMuuntLoader>();

        private dynamic ActiveByml;

        private MuuntEditor2D Viewport2D;
        private MuuntObjectList ObjectList;
        private MuuntPropertiesEditor PropertiesEditor;

        public MuuntEditor()
        {
            InitializeComponent();

            ThemeBase theme = new VS2015DarkTheme();
            if (FormThemes.ActivePreset == FormThemes.Preset.White)
                theme = new VS2015LightTheme();

            this.dockPanel1.Theme = theme;
            this.dockPanel1.BackColor = FormThemes.BaseTheme.FormBackColor;
            this.BackColor = FormThemes.BaseTheme.FormBackColor;

            Plugins.Add(new TrackMuuntLoader());

            ObjectSelected += OnObjectSelected;
        }

        public void LoadByaml(dynamic byml, string fileName) {
            ActiveByml = byml;
            LoadPlugins(byml, fileName);

            LoadPanels();
        }

        private void LoadPanels()
        {
            ShowObjectList();
            ShowPropertiesEditor();
        }

        private void LoadPlugins(dynamic byml, string fileName)
        {
            for (int i = 0; i < Plugins.Count; i++) {
                if (Plugins[i].Identify(byml, fileName)) {
                    Plugins[i].Load(byml);
                    Groups = Plugins[i].Groups;
                }
            }

            LoadViewport();
        }

        private void LoadViewport()
        {
            DockContent dockContent = new DockContent();
            Viewport2D = new MuuntEditor2D(this);
            Viewport2D.Dock = DockStyle.Fill;
            dockContent.Controls.Add(Viewport2D);

            dockContent.Show(dockPanel1, DockState.Document);
            dockContent.DockHandler.AllowEndUserDocking = false;
        }

        private void ShowObjectList()
        {
            if (ObjectList != null)
                return;

            ObjectList = new MuuntObjectList(this);
            ObjectList.LoadObjects(Groups);
            ObjectList.Show(dockPanel1, DockState.DockLeft);
        }

        private void ShowPropertiesEditor()
        {
            if (PropertiesEditor != null)
                return;

            PropertiesEditor = new MuuntPropertiesEditor(this);

            if (ObjectList != null)
                PropertiesEditor.Show(ObjectList.Pane, DockAlignment.Bottom, 0.5);
            PropertiesEditor.Show(dockPanel1, DockState.DockLeft);
        }

        private void OnObjectSelected(object sender, EventArgs e)
        {

        }
    }
}

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
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin.MuuntEditor
{
    public partial class MuuntEditor : Form
    {
        public List<MuuntCollisionObject> CollisionObjects = new List<MuuntCollisionObject>();

        public List<ObjectGroup> Groups = new List<ObjectGroup>();
        public EventHandler ObjectSelected;

        private List<IMuuntLoader> Plugins = new List<IMuuntLoader>();

        private dynamic ActiveByml;

        private DockContent ViewportDock;

        private MuuntEditor3D Viewport3D;
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
            ViewportDock = new DockContent();
            Viewport2D = new MuuntEditor2D(this);
            Viewport2D.Dock = DockStyle.Fill;
            ViewportDock.Controls.Add(Viewport2D);

            ViewportDock.Show(dockPanel1, DockState.Document);
            ViewportDock.DockHandler.AllowEndUserDocking = false;
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
            PropertiesEditor.ProperyChanged += OnPropertyChanged;

            if (ObjectList != null)
                PropertiesEditor.Show(ObjectList.Pane, DockAlignment.Bottom, 0.5);
            PropertiesEditor.Show(dockPanel1, DockState.DockLeft);
        }

        private void OnPropertyChanged(object sender, EventArgs e)
        {
            if (Viewport2D != null)
                Viewport2D.UpdateViewport();
            if (Viewport3D != null)
                Viewport3D.UpdateViewport();
        }

        private void OnObjectSelected(object sender, EventArgs e)
        {
            if (e is TreeViewEventArgs)
            {
                var node = ((TreeViewEventArgs)e).Node;
                if (node.Tag != null && PropertiesEditor != null)
                {
                    PropertiesEditor.LoadProperty(node.Tag);
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (Runtime.MuuntEditor.Enable3DViewport)
            {
                Runtime.MuuntEditor.Enable3DViewport = false;
                toolStripButton1.Image = Properties.Resources.ViewportIconDisable;
            }
            else
            {
                toolStripButton1.Image = Properties.Resources.ViewportIcon;
                Runtime.MuuntEditor.Enable3DViewport = true;
            }

            if (ViewportDock != null)
            {
                ViewportDock.Controls.Clear();
                if (Runtime.MuuntEditor.Enable3DViewport)
                {
                    if (Viewport3D == null)
                    {
                        Viewport3D = new MuuntEditor3D(this);
                        Viewport3D.Dock = DockStyle.Fill;
                    }
                    ViewportDock.Controls.Add(Viewport3D);
                }
                else
                {
                    if (Viewport2D == null)
                    {
                        Viewport2D = new MuuntEditor2D(this);
                        Viewport2D.Dock = DockStyle.Fill;
                    }
                    ViewportDock.Controls.Add(Viewport2D);
                }
            }
        }

        private void loadCollisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Viewport2D == null && Viewport3D == null) return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Utils.GetAllFilters(typeof(KCL));
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var fileFormat = STFileLoader.OpenFileFormat(ofd.FileName);
                if (fileFormat != null && fileFormat is KCL)
                    LoadCollision((KCL)fileFormat);
            }
        }

        private void LoadCollision(KCL kcl)
        {
            MuuntCollisionObject col = new MuuntCollisionObject();
            col.CollisionFile = kcl;
            col.Renderer = new KCLRendering2D(kcl);
            Viewport3D?.AddDrawable(kcl.DrawableContainer.Drawables[0]);
            CollisionObjects.Add(col);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.EditorDrawables;

namespace FirstPlugin.Forms
{
    public partial class BfresEditor : STUserControl, IViewportContainer
    {
        Viewport viewport
        {
            get
            {
               var editor = LibraryGUI.Instance.GetObjectEditor();
               return editor.GetViewport();
            }
            set
            {
                var editor = LibraryGUI.Instance.GetObjectEditor();
                editor.LoadViewport(value);
            }
        }

        AnimationPanel animationPanel;

        public BfresEditor(bool HasModels)
        {
            InitializeComponent();  

            STConsole stConsole = STConsole.Instance;
            stConsole.BorderStyle = BorderStyle.None;
            stConsole.Dock = DockStyle.Fill;
            tabPage4.Controls.Add(stConsole);
            
            animationPanel = new AnimationPanel();
            animationPanel.CurrentAnimation = null;
            animationPanel.Dock = DockStyle.Fill;
            timelineTabPage.Controls.Add(animationPanel);

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
            stTabControl2.myBackColor = FormThemes.BaseTheme.FormBackColor;


            if (viewport == null)
            {
                viewport = new Viewport();
                viewport.Dock = DockStyle.Fill;
            }
            stPanel5.Controls.Add(viewport);

            if (HasModels)
                stTabControl1.SelectedIndex = 1;
        }

        public UserControl GetActiveEditor(Type type)
        {
            foreach (var ctrl in splitContainer1.Panel2.Controls)
            {
                if (type == null)
                {
                    return (UserControl)ctrl;
                }

                if (ctrl.GetType() == type)
                {
                    return (UserControl)ctrl;
                }
            }
            return null;
        }

        public void LoadEditor(UserControl Control)
        {
            splitContainer1.Panel2.Controls.Clear();
            splitContainer1.Panel2.Controls.Add(Control);
        }

        public AnimationPanel GetAnimationPanel() => animationPanel;

        public Viewport GetViewport() => viewport;

        public void UpdateViewport()
        {
            if (viewport != null)
                viewport.UpdateViewport();
        }


        public bool IsLoaded = false;
        List<AbstractGlDrawable> Drawables;

        public void LoadViewport(List<AbstractGlDrawable> drawables, List<ToolStripMenuItem> customContextMenus = null)
        {
            Drawables = drawables;

            if (customContextMenus != null)
            {
                foreach (var menu in customContextMenus)
                    viewport.LoadCustomMenuItem(menu);
            }
        }

        public void AddDrawable(AbstractGlDrawable draw)
        {
            Drawables.Add(draw);

            if (!viewport.scene.staticObjects.Contains(draw) &&
                !viewport.scene.objects.Contains(draw))
            {
                viewport.AddDrawable(draw);
            }
        }

        public void RemoveDrawable(AbstractGlDrawable draw)
        {
            Drawables.Remove(draw);
            viewport.RemoveDrawable(draw);
        }

        public override void OnControlClosing()
        {
            animationPanel.ClosePanel();
        }

        public Action UpdateVertexData;
        public Action UpdateTextureMaps;

        public void OnLoadedTab()
        {
            //If a model was loaded we don't need to load the drawables again
            if (IsLoaded || Drawables == null)
                return;

            foreach (var draw in Drawables)
            {
                if (!viewport.scene.staticObjects.Contains(draw) &&
                    !viewport.scene.objects.Contains(draw))
                {
                    viewport.AddDrawable(draw);
                }
            }

            viewport.LoadObjects();

            IsLoaded = true;

            UpdateVertexData();
            UpdateTextureMaps();
        }

        private void stTabControl1_TabIndexChanged(object sender, EventArgs e)
        {
     
        }

        private void stTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stTabControl1.SelectedIndex == 1)
            {
                OnLoadedTab();
            }
        }
    }
}

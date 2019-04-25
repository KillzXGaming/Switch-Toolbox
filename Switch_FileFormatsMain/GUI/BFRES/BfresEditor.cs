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
        private bool _displayViewport = true;

        public bool DisplayViewport
        {
            get
            {
                return _displayViewport;
            }
            set
            {
                _displayViewport = value;
                SetupViewport();
            }
        }

        private void SetupViewport()
        {
            if (DisplayViewport == true && Runtime.UseOpenGL)
            {
                stPanel5.Controls.Add(viewport);
                splitContainer1.Panel1Collapsed = false;
                toggleViewportToolStripBtn.Image = Properties.Resources.ViewportIcon;

                if (viewport != null)
                    OnLoadedTab();
                else
                {
                    viewport = new Viewport();
                    viewport.Dock = DockStyle.Fill;
                    OnLoadedTab();
                }
            }
            else
            {
                stPanel5.Controls.Clear();
                splitContainer1.Panel1Collapsed = true;
                toggleViewportToolStripBtn.Image = Properties.Resources.ViewportIconDisable;
            }
        }

        Viewport viewport
        {
            get
            {
                if (!Runtime.UseOpenGL || !DisplayViewport)
                    return null;

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

            stTabControl2.myBackColor = FormThemes.BaseTheme.FormBackColor;

            //Always create an instance of the viewport unless opengl is disabled
            if (viewport == null && Runtime.UseOpenGL)
            {
                viewport = new Viewport();
                viewport.Dock = DockStyle.Fill;
            }

            //If the option is enabled by settings, and it has models display the viewport
            if (Runtime.UseOpenGL && Runtime.DisplayViewport && HasModels)
            {
                stPanel5.Controls.Add(viewport);
                DisplayViewport = true;
            }
            else
            {
                DisplayViewport = false;
                splitContainer1.Panel1Collapsed = true;
            }
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
            Control.Dock = DockStyle.Fill;

            splitContainer1.Panel2.Controls.Clear();
            splitContainer1.Panel2.Controls.Add(Control);
        }

        public AnimationPanel GetAnimationPanel() => animationPanel;

        public Viewport GetViewport() => viewport;

        public void UpdateViewport()
        {
            if (viewport != null && Runtime.UseOpenGL && Runtime.DisplayViewport)
                viewport.UpdateViewport();
        }


        public bool IsLoaded = false;

        List<AbstractGlDrawable> Drawables;

        public void LoadViewport(List<AbstractGlDrawable> drawables, List<ToolStripMenuItem> customContextMenus = null)
        {
            Drawables = drawables;

            if (!Runtime.UseOpenGL || !DisplayViewport)
                return;

            if (customContextMenus != null)
            {
                foreach (var menu in customContextMenus)
                    viewport.LoadCustomMenuItem(menu);
            }

            OnLoadedTab();
        }

        public void AddDrawable(AbstractGlDrawable draw)
        {
            Drawables.Add(draw);

            if (!Runtime.UseOpenGL || !Runtime.DisplayViewport)
            {
                IsLoaded = false;
                return;
            }

            if (!viewport.scene.staticObjects.Contains(draw) &&
                !viewport.scene.objects.Contains(draw))
            {
                viewport.AddDrawable(draw);
            }
        }

        public void RemoveDrawable(AbstractGlDrawable draw)
        {
            Drawables.Remove(draw);

            if (!Runtime.UseOpenGL || !Runtime.DisplayViewport)
            {
                IsLoaded = false;
                return;
            }

            viewport.RemoveDrawable(draw);
        }

        public override void OnControlClosing()
        {
            animationPanel.ClosePanel();
        }

        private void OnLoadedTab()
        {
            //If a model was loaded we don't need to load the drawables again
            if (IsLoaded || Drawables == null || !Runtime.UseOpenGL || !Runtime.DisplayViewport)
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
        }

        private void stTabControl1_TabIndexChanged(object sender, EventArgs e)
        {
     
        }

        private void stTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        bool IsTimelineVisable = true;
        int controlHeight = 0;
        private void stPanel1_DoubleClick(object sender, EventArgs e)
        {
            if (IsTimelineVisable)
            {
                controlHeight = stTabControl2.Height;
                IsTimelineVisable = false;
                stPanel1.Height -= (controlHeight - 25);
            }
            else
            {
                IsTimelineVisable = true;
                stPanel1.Height += (controlHeight + 25);
            }
        }

        private void toggleViewportToolStripBtn_Click(object sender, EventArgs e)
        {
            if (Runtime.DisplayViewport)
            {
                Runtime.DisplayViewport = false;
            }
            else
            {
                Runtime.DisplayViewport = true;
            }

            DisplayViewport = Runtime.DisplayViewport;
            Config.Save();
        }
    }
}

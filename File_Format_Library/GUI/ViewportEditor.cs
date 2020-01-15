using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.EditorDrawables;

namespace FirstPlugin.Forms
{
    public partial class ViewportEditor : STUserControl, IViewportContainer
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
                stPanel3.Controls.Add(viewport);
                splitContainer1.Panel1Collapsed = false;
                toggleViewportToolStripBtn.Image = Properties.Resources.ViewportIcon;

                if (viewport != null)
                    OnLoadedTab();
                else
                {
                    viewport = new Viewport(ObjectEditor.GetDrawableContainers());
                    viewport.Dock = DockStyle.Fill;
                    OnLoadedTab();
                }
            }
            else
            {
                stPanel3.Controls.Clear();
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

                var editor = LibraryGUI.GetObjectEditor();
                return editor.GetViewport();
            }
            set
            {
                var editor = LibraryGUI.GetObjectEditor();
                editor.LoadViewport(value);
            }
        }

        AnimationPanel animationPanel;

        public ViewportEditor(bool HasModels)
        {
            InitializeComponent();

            animationPanel = new AnimationPanel();
            animationPanel.CurrentAnimation = null;
            animationPanel.Dock = DockStyle.Fill;
            timelineTabPage.Controls.Add(animationPanel);

            //Always create an instance of the viewport unless opengl is disabled
            if (viewport == null && Runtime.UseOpenGL)
            {
                viewport = new Viewport(ObjectEditor.GetDrawableContainers());
                viewport.Dock = DockStyle.Fill;
            }

            //If the option is enabled by settings, and it has models display the viewport
            if (Runtime.UseOpenGL && Runtime.DisplayViewport && HasModels)
            {
                stPanel3.Controls.Add(viewport);
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

        public Panel GetEditorPanel() { return stPanel4; }

        public void LoadEditor(UserControl Control)
        {
            Control.Dock = DockStyle.Fill;

            stPanel4.Controls.Clear();
            stPanel4.Controls.Add(Control);
        }

        public AnimationPanel GetAnimationPanel() => animationPanel;

        public Viewport GetViewport() => viewport;

        public void UpdateViewport()
        {
            if (viewport != null && Runtime.UseOpenGL && Runtime.DisplayViewport)
                viewport.UpdateViewport();
        }


        public bool IsLoaded = false;

        public void LoadViewport(DrawableContainer ActiveDrawable, List<ToolStripMenuItem> customContextMenus = null)
        {
            if (!Runtime.UseOpenGL || !DisplayViewport)
                return;

            if (customContextMenus != null)
            {
                foreach (var menu in customContextMenus)
                    viewport.LoadCustomMenuItem(menu);
            }

            viewport.ReloadDrawables(ActiveDrawable);

            OnLoadedTab();
        }

        public override void OnControlClosing() {
            animationPanel.ClosePanel();
        }

        private void OnLoadedTab()
        {
            //If a model was loaded we don't need to load the drawables again
            if (IsLoaded ||!Runtime.UseOpenGL || !Runtime.DisplayViewport)
                return;

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

        private void dockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            STForm floatingForm = new STForm();
            floatingForm.AddControl(stPanel4);
            floatingForm.Show(this);
        }
    }
}

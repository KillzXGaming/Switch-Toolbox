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
            if (DisplayViewport == true && Runtime.UseViewport)
            {
                stPanel5.Controls.Add(viewport);
                splitContainer1.Panel1Collapsed = false;
                toggleViewportToolStripBtn.Image = Properties.Resources.ViewportIcon;
            }
            else
            {
                stPanel5.Controls.Clear();
                splitContainer1.Panel1Collapsed = true;
                toggleViewportToolStripBtn.Image = Properties.Resources.ViewportIconDisable;
                OnLoadedTab();
            }
        }

        Viewport viewport
        {
            get
            {
                if (!Runtime.UseViewport || !DisplayViewport)
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

            if (viewport == null && Runtime.UseViewport)
            {
                viewport = new Viewport();
                viewport.Dock = DockStyle.Fill;
            }

            if (Runtime.UseViewport && Runtime.DisplayViewport)
                stPanel5.Controls.Add(viewport);
            else
                splitContainer1.Panel1Collapsed = true;

            OnLoadedTab();

            if (HasModels && Runtime.DisplayViewport)
                DisplayViewport = true;
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
            if (viewport != null && Runtime.UseViewport)
                viewport.UpdateViewport();
        }


        public bool IsLoaded = false;
        List<AbstractGlDrawable> Drawables;

        public void LoadViewport(List<AbstractGlDrawable> drawables, List<ToolStripMenuItem> customContextMenus = null)
        {
            Drawables = drawables;

            if (!Runtime.UseViewport || !DisplayViewport)
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
            if (!Runtime.UseViewport)
                return;

            Drawables.Add(draw);

            if (!viewport.scene.staticObjects.Contains(draw) &&
                !viewport.scene.objects.Contains(draw))
            {
                viewport.AddDrawable(draw);
            }
        }

        public void RemoveDrawable(AbstractGlDrawable draw)
        {
            if (!Runtime.UseViewport)
                return;

            Drawables.Remove(draw);
            viewport.RemoveDrawable(draw);
        }

        public override void OnControlClosing()
        {
            animationPanel.ClosePanel();
        }

        public void OnLoadedTab()
        {
            //If a model was loaded we don't need to load the drawables again
            if (IsLoaded || Drawables == null || !Runtime.UseViewport)
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

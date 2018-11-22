using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using GL_Core;
using System.Windows.Forms;
using GL_Core.Cameras;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using WeifenLuo.WinFormsUI.Docking;

namespace Switch_Toolbox.Library
{
    public class LibraryGUI
    {
        private static LibraryGUI _instance;
        public static LibraryGUI Instance { get { return _instance == null ? _instance = new LibraryGUI() : _instance; } }

        public DockPanel dockPanel;

        public void LoadDockContent(Control control, DockState dockState)
        {
            DockContent content = new DockContent();
            content.Controls.Add(control);
            content.Show(dockPanel, dockState);
        }
        public bool IsContentDocked(Control control)
        {
            foreach (DockContent dockContent in dockPanel.Contents)
            {
                foreach (Control ctrl in dockContent.Controls)
                    if (ctrl == control)
                        return true;
            }
            return false;
        }
        public void LoadDockContent(DockContent DockContent, DockState dockState)
        {
            DockContent.Show(dockPanel, dockState);
        }
        public void LoadViewport(Viewport viewport)
        {
            if (dockPanel == null && IsContentActive(viewport))
                return;

            viewport.Show(dockPanel, DockState.Document);
        }
        public bool IsContentActive(DockContent dockContent)
        {
            return dockPanel.Contents.Contains(dockContent);
        }
    }
}

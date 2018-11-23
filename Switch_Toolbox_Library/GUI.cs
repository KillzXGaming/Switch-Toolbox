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
        public DockContent dockContent;

        public void LoadDockContent(Control control, DockState dockState)
        {
            dockContent = new DockContent();
            dockContent.Controls.Add(control);
            dockContent.Show(dockPanel, dockState);
        }
        public UserControl GetContentDocked(UserControl control)
        {
            foreach (DockContent dockContent in dockPanel.Contents)
            {
                foreach (Control ctrl in dockContent.Controls)
                    if (ctrl.GetType() == control.GetType())
                        return (UserControl)ctrl;
            }
            return null;
        }
        public DockContent GetContentDocked(DockContent DockContent)
        {
            foreach (DockContent dock in dockPanel.Contents)
                if (dock.GetType() == DockContent.GetType())
                    return dock;

            return null;
        }
        public void LoadDockContent(DockContent DockContent, DockState dockState)
        {
            dockContent = DockContent;
            dockContent.Show(dockPanel, dockState);
        }
        public void LoadViewport(Viewport viewport)
        {
            if (dockPanel == null && IsContentActive(viewport))
                return;

            viewport.Show(dockPanel, DockState.Document);
        }
        public bool IsContentActive(DockContent DockContent)
        {
            dockContent = DockContent;
            return dockPanel.Contents.Contains(dockContent);
        }
    }
}

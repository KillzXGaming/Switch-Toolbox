using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.GL_Core;
using System.Windows.Forms;
using GL_EditorFramework.StandardCameras;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Switch_Toolbox.Library.Forms;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.EditorDrawables;

namespace Switch_Toolbox.Library
{
    public class LibraryGUI
    {
        private static LibraryGUI _instance;
        public static LibraryGUI Instance { get { return _instance == null ? _instance = new LibraryGUI() : _instance; } }


        public List<EditableObject> editableObjects = new List<EditableObject>();

        public Viewport GetActiveViewport()
        {
            var viewport = GetActiveContent();

            if (viewport != null && viewport is IViewportContainer)
            {
                return (((IViewportContainer)viewport).GetViewport());
            }
            if (Runtime.MainForm.ActiveMdiChild is IViewportContainer)
            {
                return ((IViewportContainer)Runtime.MainForm.ActiveMdiChild).GetViewport();
            }
            if (viewport != null && viewport is Viewport)
            {
                return ((Viewport)viewport);
            }

            return null;
        }

        public AnimationPanel GetAnimationPanel()
        {
            var editor = GetActiveContent();
            if (editor != null && editor is IViewportContainer)
            {
                return (((IViewportContainer)editor).GetAnimationPanel());
            }
            return null;
        }

        public void UpdateViewport()
        {   
            Viewport viewport = GetActiveViewport();

            if (viewport != null)
                viewport.UpdateViewport();
        }
        public Form GetActiveForm()
        {
            return Runtime.MainForm.ActiveMdiChild;
        }
        public void CreateMdiWindow(STForm form, bool Show = true)
        {
            form.MdiParent = Application.OpenForms[0];

            if (Show)
                form.Show();
        }
        public ObjectEditor GetObjectEditor()
        {
            if (Runtime.MainForm.ActiveMdiChild is ObjectEditor)
            {
                return (ObjectEditor)Runtime.MainForm.ActiveMdiChild;
            }
            return null;
        }
        public void LoadViewportEditor(Control control)
        {
            control.Dock = DockStyle.Fill;

            var activeContent = GetActiveContent(typeof(ViewportDivider));
            if (activeContent == null)
            {
                activeContent = new ViewportDivider();
                activeContent.Dock = DockStyle.Fill;
                LoadEditor(activeContent);
            }

            if (control is Viewport)
            {
                ((ViewportDivider)activeContent).viewportPanel.Controls.Clear();
                ((ViewportDivider)activeContent).viewportPanel.Controls.Add(control);
            }
            else
            {
                ((ViewportDivider)activeContent).editorPanel.Controls.Clear();
                ((ViewportDivider)activeContent).editorPanel.Controls.Add(control);
            }
        }
        public void LoadEditor(Control control)
        {
            if (Runtime.MainForm.ActiveMdiChild is ObjectEditor)
            {
                ((ObjectEditor)Runtime.MainForm.ActiveMdiChild).LoadEditor(control);
            }
        }
        public UserControl GetActiveContent(Type type = null)
        {
            if (Runtime.MainForm.ActiveMdiChild is ObjectEditor)
            {
                foreach (var ctrl in ((ObjectEditor)Runtime.MainForm.ActiveMdiChild).stPanel2.Controls)
                {
                    if (type == null) {
                        return (UserControl)ctrl;
                    }
                    if (ctrl.GetType() == type) {
                        return (UserControl)ctrl;
                    }
                }
            }
            else
            {
                foreach (var ctrl in Runtime.MainForm.ActiveMdiChild.Controls)
                {
                    if (type == null && type == typeof(UserControl))
                    {
                        return (UserControl)ctrl;
                    }
                    if (ctrl.GetType() == type)
                    {
                        return (UserControl)ctrl;
                    }
                }
            }
            return null;
        }
        public void LoadViewport(Viewport viewport)
        {
 
        }
        public bool IsContentActive(UserControl control)
        {
            return ObjectEditor.Instance.Controls.Contains(control);
        }
    }
}

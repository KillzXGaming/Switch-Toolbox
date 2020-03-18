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
using Toolbox.Library.Forms;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.EditorDrawables;

namespace Toolbox.Library
{
    public class LibraryGUI
    {
        public static Viewport GetActiveViewport()
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

        public static AnimationPanel GetAnimationPanel()
        {
            var editor = GetActiveContent();
            if (editor != null && editor is IViewportContainer)
            {
                return (((IViewportContainer)editor).GetAnimationPanel());
            }
            return null;
        }

        public static void UpdateViewport()
        {   
            Viewport viewport = GetActiveViewport();

            if (viewport != null)
                viewport.UpdateViewport();
        }
        public static Form GetActiveForm()
        {
            return Runtime.MainForm.ActiveMdiChild;
        }
        public static void CreateMdiWindow(STForm form, bool Show = true)
        {
            ((IMdiContainer)Runtime.MainForm).AddChildContainer(form);
        }
        public static ObjectEditor GetObjectEditor()
        {
            if (Runtime.MainForm.ActiveMdiChild is ObjectEditor)
            {
                return (ObjectEditor)Runtime.MainForm.ActiveMdiChild;
            }
            return null;
        }
        public static void LoadViewportEditor(Control control)
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
        public static void LoadEditor(Control control)
        {
            if (Runtime.MainForm.ActiveMdiChild is ObjectEditor)
            {
                ((ObjectEditor)Runtime.MainForm.ActiveMdiChild).LoadEditor(control);
            }
        }
        public static UserControl GetActiveContent(Type type = null)
        {
            if (Runtime.MainForm.ActiveMdiChild == null)
                return null;

            if (Runtime.MainForm.ActiveMdiChild is ObjectEditor)
            {
                foreach (var ctrl in ((ObjectEditor)Runtime.MainForm.ActiveMdiChild).GetEditors())
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
                    else if (ctrl.GetType() == type)
                    {
                        return (UserControl)ctrl;
                    }
                    else
                    {
                        Console.WriteLine(ctrl);

                        foreach (var inter in ctrl.GetType().GetInterfaces())
                        {
                            Console.WriteLine(inter);

                            if (inter.IsGenericType && inter.GetGenericTypeDefinition() == type)
                            {
                                return (UserControl)ctrl;
                            }
                        }
                    }
                }
            }
            return null;
        }
        public static void LoadViewport(Viewport viewport)
        {
 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MDIController
{
    public static class MDIClientSupport
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_CLIENTEDGE = 0x200;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOREDRAW = 0x0008;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_FRAMECHANGED = 0x0020;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SWP_HIDEWINDOW = 0x0080;
        private const uint SWP_NOCOPYBITS = 0x0100;
        private const uint SWP_NOOWNERZORDER = 0x0200;
        private const uint SWP_NOSENDCHANGING = 0x0400;

        public static bool SetBevel(this Form form, bool show)
        {
            foreach (Control c in form.Controls)
            {
                MdiClient client = c as MdiClient;
                if (client != null)
                {
                    int windowLong = GetWindowLong(c.Handle, GWL_EXSTYLE);

                    if (show)
                    {
                        windowLong |= WS_EX_CLIENTEDGE;
                    }
                    else
                    {
                        windowLong &= ~WS_EX_CLIENTEDGE;
                    }

                    SetWindowLong(c.Handle, GWL_EXSTYLE, windowLong);

                    // Update the non-client area.
                    SetWindowPos(client.Handle, IntPtr.Zero, 0, 0, 0, 0,
                        SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER |
                        SWP_NOOWNERZORDER | SWP_FRAMECHANGED);

                    return true;
                }
            }
            return false;
        }

    }
}
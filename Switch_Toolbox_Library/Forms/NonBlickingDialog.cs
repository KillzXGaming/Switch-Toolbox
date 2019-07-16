using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Interop;

namespace Toolbox.Library.Forms
{
    public static class NoneBlockingDialog
    {
        private static DispatcherFrame frame;
        private static void closeHandler(object sender, EventArgs args)
        {
            frame.Continue = false;
        }

        public static DialogResult ShowDialogNonBlocking(this Form window, Form ownder)
        {
            frame = new DispatcherFrame();

            try
            {
                SetNativeEnabled(ownder.Handle, false);
                window.Closed += closeHandler;
                window.Show();

                Dispatcher.PushFrame(frame);
            }
            finally
            {
                window.Closed -= closeHandler;
                SetNativeEnabled(ownder.Handle, true);
            }
            return window.DialogResult;
        }

        const int GWL_STYLE = -16;
        const int WS_DISABLED = 0x08000000;

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        static void SetNativeEnabled(IntPtr hWnd, bool enabled)
        {
            SetWindowLong(hWnd, GWL_STYLE, GetWindowLong(hWnd, GWL_STYLE) & ~WS_DISABLED | (enabled ? 0 : WS_DISABLED));
        }
    }
}

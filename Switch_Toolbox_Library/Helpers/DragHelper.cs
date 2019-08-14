using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Toolbox.Library
{
    public class DragHelper
    {
        [DllImport("comctl32.dll")]
        public static extern bool InitCommonControls();

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_BeginDrag(
            IntPtr himlTrack, // Handler of the image list containing the image to drag
            int iTrack,       // Index of the image to drag 
            int dxHotspot,    // x-delta between mouse position and drag image
            int dyHotspot     // y-delta between mouse position and drag image
        );

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_DragMove(
            int x,   // X-coordinate (relative to the form,
                     // not the treeview) at which to display the drag image.
            int y   // Y-coordinate (relative to the form,
                 // not the treeview) at which to display the drag image.
        );

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern void ImageList_EndDrag();

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_DragEnter(
            IntPtr hwndLock,  // Handle to the control that owns the drag image.
            int x,            // X-coordinate (relative to the treeview)
                              // at which to display the drag image. 
            int y             // Y-coordinate (relative to the treeview)
                              // at which to display the drag image. 
        );

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_DragLeave(
            IntPtr hwndLock  // Handle to the control that owns the drag image.
        );

        [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImageList_DragShowNolock(
            bool fShow       // False to hide, true to show the image
        );

        static DragHelper()
        {
            InitCommonControls();
        }
    }
}

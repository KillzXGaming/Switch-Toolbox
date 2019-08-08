using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Toolbox.Library;

namespace Toolbox
{
    public partial class FileAssociationForm : STForm
    {
        public FileAssociationForm()
        {
            InitializeComponent();

            CanResize = false;
        }

        private void FileAssociationForm_Load(object sender, EventArgs e)
        {
            listViewCustom1.Items.Clear();

            foreach (Type t in FileManager.GetFileFormats())
            {
                dynamic item = new StaticDynamic(t);
                for (int i = 0; i < item.Extension.Length; i++)
                {
                    string Extension;
                    if (item.Description.Length - 1 < i)
                        Extension = $"{item.Description[0]} ({item.Extension[i]})";
                    else
                        Extension = $"{item.Description[i]} ({item.Extension[i]})";

                    listViewCustom1.Items.Add(Extension);
                }
            }
        }

        private void stButton1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewCustom1.Items)
            {
                if (item.Checked)
                {
                    string ext = item.Text.Split('(', ')')[1];
                    string des = item.Text.Split('(', ')')[0];
                    ext = ext.Replace("*", string.Empty);
                    des = des.Replace("*", string.Empty);

                    var filePath = Process.GetCurrentProcess().MainModule.FileName;
                    FileAssociations.EnsureAssociationsSet(
                          new FileAssociation
                          {
                              Extension = ext,
                              ProgId = "Switch Toolbox",
                              FileTypeDescription = des,
                              ExecutableFilePath = Application.ExecutablePath
                          });
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewCustom1.Items)
            {
                if (chkAll.Checked)
                {
                    item.Checked = true;
                }
                else
                {
                    item.Checked = false;
                }
            }
        }
    }

    public class FileAssociation
    {
        public string Extension { get; set; }
        public string ProgId { get; set; }
        public string FileTypeDescription { get; set; }
        public string ExecutableFilePath { get; set; }
    }

    public class FileAssociations
    {
        // needed so that Explorer windows get refreshed after the registry is updated
        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const int SHCNF_FLUSH = 0x1000;

        public static void EnsureAssociationsSet()
        {
            var filePath = Process.GetCurrentProcess().MainModule.FileName;
            EnsureAssociationsSet(
                new FileAssociation
                {
                    Extension = ".ucs",
                    ProgId = "UCS_Editor_File",
                    FileTypeDescription = "UCS File",
                    ExecutableFilePath = filePath
                });
        }

        public static void EnsureAssociationsSet(params FileAssociation[] associations)
        {
            bool madeChanges = false;
            foreach (var association in associations)
            {
                madeChanges |= SetAssociation(
                    association.Extension,
                    association.ProgId,
                    association.FileTypeDescription,
                    association.ExecutableFilePath);
            }

            if (madeChanges)
            {
                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public static bool SetAssociation(string extension, string progId, string fileTypeDescription, string applicationFilePath)
        {
            bool madeChanges = false;
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + extension, progId);
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + progId, fileTypeDescription);
            madeChanges |= SetKeyDefaultValue($@"Software\Classes\{progId}\shell\open\command", "\"" + applicationFilePath + "\" \"%1\"");
            return madeChanges;
        }

        private static bool SetKeyDefaultValue(string keyPath, string value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (key.GetValue(null) as string != value)
                {
                    key.SetValue(null, value);
                    return true;
                }
            }

            return false;
        }
    }
}

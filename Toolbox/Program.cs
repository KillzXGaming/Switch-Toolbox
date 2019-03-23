using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Toolbox
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Switch_Toolbox.Library.Runtime.ExecutableDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            string[] args = Environment.GetCommandLineArgs();

            List<string> Files = new List<string>();
            foreach (var arg in args)
            {
                if (arg != Application.ExecutablePath)
                    Files.Add(arg);
            }

            var domain = AppDomain.CurrentDomain;
            domain.AssemblyResolve += LoadAssembly;

            MainForm form = new MainForm();
            form.openedFiles = Files;

            Application.Run(form);
        }

        /// 
        /// Include externals dlls
        /// 
        private static Assembly LoadAssembly(object sender, ResolveEventArgs args)
        {
            Assembly result = null;
            if (args != null && !string.IsNullOrEmpty(args.Name))
    {
                //Get current exe fullpath
                FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);

                //Get folder of the executing .exe
                var folderPath = Path.Combine(info.Directory.FullName, "Lib");

                //Build potential fullpath to the loading assembly
                var assemblyName = args.Name.Split(new string[] { "," }, StringSplitOptions.None)[0];
                var assemblyExtension = "dll";
                var assemblyPath = Path.Combine(folderPath, string.Format("{0}.{1}", assemblyName, assemblyExtension));

                //Check if the assembly exists in our "Libs" directory
                if (File.Exists(assemblyPath))
                {
                    Console.WriteLine("Loading .dll " + assemblyPath);

                    //Load the required assembly using our custom path
                    result = Assembly.LoadFrom(assemblyPath);
                }
                else
                {
                    //Keep default loading
                    return args.RequestingAssembly;
                }
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octokit;
using System.Net;
using System.IO.Compression;
using System.IO;
using System.Globalization;
using System.Security.AccessControl;

namespace Updater
{
    class Program
    {
        static Release[] releases;

        static string execDirectory = "";
        static string folderDir = "";
        static bool foundRelease = false;

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            execDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            folderDir = execDirectory;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new GitHubClient(new ProductHeaderValue("ST_UpdateTool"));
            GetReleases(client).Wait();

            string versionTxt = Path.Combine(execDirectory, "Version.txt");
            if (!File.Exists(versionTxt))
                File.Create(versionTxt);

            string[] versionInfo = File.ReadLines(versionTxt).ToArray();

            string ProgramVersion = "";
            string CompileDate = "";

            string CommitInfo = "";
            if (versionInfo.Length >= 3)
            {
                ProgramVersion = versionInfo[0];
                CompileDate = versionInfo[1];
                CommitInfo = versionInfo[2];
            }

            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "-d":
                    case "--download":
                        Download(CompileDate);
                        break;
                    case "-i":
                    case "--install":
                        Install();
                        break;
                    case "-b":
                    case "--boot":
                        Boot();
                        Environment.Exit(0);
                        break;
                    case "-e":
                    case "--exit":
                        Environment.Exit(0);
                        break;
                }
            }
            Console.Read();
        }
        static void Boot()
        {
            Console.WriteLine("Booting...");

            Thread.Sleep(3000);
            System.Diagnostics.Process.Start(Path.Combine(folderDir, "Toolbox.exe"));
        }
        static void Install()
        {
            Console.WriteLine("Installing...");
            foreach (string dir in Directory.GetDirectories("master/"))
            {
                SetAccessRule(folderDir);
                SetAccessRule(dir);

                string dirName = new DirectoryInfo(dir).Name;
                string destDir = Path.Combine(folderDir, dirName + @"\");

                //Skip hash directory
                if (dirName.Equals("Hashes", StringComparison.CurrentCultureIgnoreCase))
                    continue;

                if (Directory.Exists(destDir))
                {
                    Directory.Delete(destDir, true);
                }

                if (Directory.Exists(destDir))
                    Directory.Delete(destDir, true);

                Directory.Move(dir, destDir);
            }
            foreach (string file in Directory.GetFiles("master/"))
            {
                if (file.Contains("Updater.exe") || file.Contains("Updater.exe.config")
                    || file.Contains("Updater.pdb") || file.Contains("Octokit.dll"))
                    continue;

                SetAccessRule(file);
                SetAccessRule(folderDir);

                string destFile = Path.Combine(folderDir, Path.GetFileName(file));
                if (File.Exists(destFile))
                    File.Delete(destFile);

                File.Move(file, destFile);
            }
        }

        static void SetAccessRule(string directory)
        {
            try
            {
                System.Security.AccessControl.DirectorySecurity sec = System.IO.Directory.GetAccessControl(directory);
                FileSystemAccessRule accRule = new FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, FileSystemRights.FullControl, AccessControlType.Allow);
                sec.AddAccessRule(accRule);
                Directory.SetAccessControl(directory, sec);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set access rule for directory '{directory}': {ex.Message}");
            }
        }


        static void Download(string CompileDate)
        {
            foreach (Release latest in releases)
            {
                Console.WriteLine("Checking Update");
                if (!foundRelease)
                {
                    if (!latest.Assets[0].UpdatedAt.ToString().Equals(CompileDate))
                    {
                        Console.WriteLine("Downloading release...");
                        bool IsDownloaded = DownloadedProgram(latest);

                        if (IsDownloaded)
                            Console.WriteLine("Downloaded update successfully!");
                        else
                            Console.WriteLine("Failed to download update!");
                    }
                }
                foundRelease = true;
            }
        }
        static bool DownloadedProgram(Release release)
        {
            return DownloadRelease("master",
                release.Assets[0].BrowserDownloadUrl,
                release.TagName,
                release.Assets[0].UpdatedAt.ToString(),
                release.TargetCommitish);
        }
        static bool DownloadRelease(string downloadName, string url, string ProgramVersion, string CompileDate, string CommitInfo)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(url, downloadName + ".zip");
                }
                if (Directory.Exists(downloadName + "/"))
                    Directory.Delete(downloadName + "/", true);
                ZipFile.ExtractToDirectory(downloadName + ".zip", downloadName + "/");

                //Zip not needed anymore
                File.Delete(downloadName + ".zip");
                string versionTxt = Path.Combine(Path.GetFullPath(downloadName + "/"), "Version.txt");

                using (StreamWriter writer = new StreamWriter(versionTxt))
                {
                    writer.WriteLine($"{ProgramVersion}");
                    writer.WriteLine($"{CompileDate}");
                    writer.WriteLine($"{CommitInfo}");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to download update! {ex.ToString()}");
                return false;
            }
        }
        static async Task GetReleases(GitHubClient client)
        {
            List<Release> Releases = new List<Release>();
            foreach (Release r in await client.Repository.Release.GetAll("KillzXGaming", "Switch-Toolbox"))
                Releases.Add(r);
            releases = Releases.ToArray();
        }
    }
}

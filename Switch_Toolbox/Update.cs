using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using Switch_Toolbox.Library;

namespace Switch_Toolbox
{
    public class UpdateProgram
    {
        static Release[] releases;
        public static bool CanUpdate = false;
        public static Release LatestRelease;

        public static void CheckLatest()
        {
           
            try
            {
                var client = new GitHubClient(new ProductHeaderValue("ST_UpdateTool"));
                GetReleases(client).Wait();

                foreach (Release latest in releases)
                {
                    Console.WriteLine(
                        "The latest release is tagged at {0} and is named {1} commit {2} date {3}",
                        latest.TagName,
                        latest.Name,
                        latest.TargetCommitish,
                        latest.Assets[0].UpdatedAt.ToString());

                    if (Runtime.CompileDate != latest.Assets[0].UpdatedAt.ToString())
                    {
                        CanUpdate = true;
                        LatestRelease = latest;
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get latest update\n{ex.ToString()}");
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

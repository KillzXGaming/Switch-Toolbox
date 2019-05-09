using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using Switch_Toolbox.Library;

namespace Toolbox
{
    public class UpdateProgram
    {
        static Release[] releases;
        public static bool CanUpdate = false;
        public static Release LatestRelease;
        public static List<GitHubCommit> CommitList = new List<GitHubCommit>();

        public static void CheckLatest()
        {
            try
            {
                var client = new GitHubClient(new ProductHeaderValue("ST_UpdateTool"));
                GetReleases(client).Wait();
                GetCommits(client).Wait();

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
        static async Task GetCommits(GitHubClient client)
        {
            var options = new ApiOptions
            {
                PageSize = 5,
                PageCount = 1
            };

            foreach (GitHubCommit c in await client.Repository.Commit.GetAll("KillzXGaming", "Switch-Toolbox",  options))
            {
                CommitList.Add(c);
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

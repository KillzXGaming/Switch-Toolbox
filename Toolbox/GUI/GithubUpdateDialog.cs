using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using Octokit;

namespace Toolbox
{
    public partial class GithubUpdateDialog : STForm
    {
        public GithubUpdateDialog()
        {
            InitializeComponent();
        }

        public void LoadCommits(List<GitHubCommit> Commits)
        {
            foreach (var commit in Commits)
            {
                listViewCustom1.Items.Add(commit.Commit.Message).SubItems.Add(commit.Commit.Author.Date.DateTime.ToString());
            }
        }
    }
}

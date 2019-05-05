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
    public partial class GithubIssueDialog : STForm
    {
        public GithubIssueDialog()
        {
            InitializeComponent();

            typeCB.Items.Add("Bug");
            typeCB.Items.Add("Feature");

            typeCB.SelectedIndex = 0;
        }

        public void CreateIssue()
        {
            if (titleTB.Text == String.Empty) {
                MessageBox.Show("Title must not be empty!", "Issue Dialog", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (titleTB.Text.Length < 5)
            {
                MessageBox.Show("Title too short! Atleast have 5 or more characters!", "Issue Dialog", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var client = new GitHubClient(new ProductHeaderValue("Switch-Toolbox"));
            var createIssue = new NewIssue(titleTB.Text);
            createIssue.Body = infoTB.Text;

            if (typeCB.GetSelectedText() == "Bug")
                createIssue.Labels.Add("bug");
            else
                createIssue.Labels.Add("enhancement");

            CreateIssue(client, createIssue).Wait();
        }

        static async Task CreateIssue(GitHubClient client, NewIssue createIssue)
        {
            var issue = await client.Issue.Create("fake", "test", createIssue);
        }

        private void titleTB_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

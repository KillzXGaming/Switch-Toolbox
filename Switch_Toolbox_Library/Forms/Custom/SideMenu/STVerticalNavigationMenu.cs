using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class STVerticalNavigationMenu : UserControl
    {
        private List<STVerticalMenuTabPage> TabPages = new List<STVerticalMenuTabPage>();

        public STVerticalMenuTabPage SelectedPage
        {
            get
            {
                int index = flowLayoutPanel1.TabIndex;
                if (index > 0 && index < TabPages.Count)
                    return TabPages[index];
                else
                    return null;
            }
        }

        public STVerticalNavigationMenu()
        {
            InitializeComponent();

            flowLayoutPanel1.FixedWidth = true;
            flowLayoutPanel1.TabIndexChanged += OnTabSelected;
            flowLayoutPanel1.BackColor = FormThemes.BaseTheme.FormBackColor;
            flowLayoutPanel1.ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        private void OnTabSelected(object sender, EventArgs e)
        {
            Console.WriteLine("OnTabSelected " + SelectedPage != null);

            var page = SelectedPage;
            if (page == null) return;

            splitContainer1.Panel2.Controls.Clear();
            splitContainer1.Panel2.Controls.Add(page.Control);
        }

        private void OnTabSelected2(object sender, EventArgs e)
        {
            Console.WriteLine("OnTabSelected2 " + SelectedPage != null);

            var selectedPage = SelectedPage;
            if (selectedPage == null) return;

            foreach (var page in TabPages)
            {
                if (page == selectedPage)
                    page.Select();
                else
                    page.Deselect();
            }
        }

        public void AddTab(STUserControl control)
        {
            STVerticalMenuTabPage page = new STVerticalMenuTabPage();
            page.TabMenu = new STVerticalTabMenuItem();
            page.TabMenu.TabSelected += OnTabSelected2;
            page.TabMenu.TabText = control.Text;
            page.Control = control;
            flowLayoutPanel1.Controls.Add(page.TabMenu);
        }
    }

    public class STVerticalMenuTabPage
    {
        public STUserControl Control = new STUserControl();
        public STVerticalTabMenuItem TabMenu = new STVerticalTabMenuItem();

        public void Select()
        {
            TabMenu.ButtonColor = FormThemes.BaseTheme.CheckBoxBackColor;
        }

        public void Deselect()
        {
            TabMenu.ButtonColor = FormThemes.BaseTheme.CheckBoxEnabledBackColor;
        }
    }
}

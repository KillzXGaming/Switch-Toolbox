using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using UKing.Actors;

namespace UKing.Actors.Forms
{
    public partial class BotwActorEditorControl : STUserControl
    {
        public BotwActorEditorControl()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
        }

        public void LoadActor(BotwActorLoader.ActorEntry entry)
        {
            stPropertyGrid1.LoadProperty(entry.Info);
        }
    }
}

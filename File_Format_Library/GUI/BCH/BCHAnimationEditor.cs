using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SPICA.Formats.CtrH3D.Animation;
using Toolbox.Library.Forms;

namespace FirstPlugin.CtrLibrary.Forms
{
    public partial class BCHAnimationEditor : UserControl
    {
        public BCHAnimationEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
        }

        public void LoadAnimation(H3DAnimation animation) {
            bchUserDataEditor1.LoadUserData(animation.MetaData);
        }
    }
}

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
using SPICA.Formats.CtrH3D.Model;

namespace FirstPlugin.CtrLibrary.Forms
{
    public partial class BCHBoneEditor : UserControl
    {
        public H3DBoneWrapper BoneWrapper;
        public H3DBone ActiveBone => BoneWrapper.Bone;

        public BCHBoneEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
        }

        public void LoadBone(H3DBoneWrapper wrapper)
        {
            BoneWrapper = wrapper;

            if (ActiveBone.MetaData != null)
                bchUserDataEditor1.LoadUserData(ActiveBone.MetaData);
        }
    }
}

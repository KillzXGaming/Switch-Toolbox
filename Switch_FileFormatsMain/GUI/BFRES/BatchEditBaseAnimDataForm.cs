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
using Switch_Toolbox.Library.Animations;

namespace FirstPlugin
{
    public partial class BatchEditBaseAnimDataForm : STForm
    {
        public float ScaleX => (float)scaleXUD.Value;
        public float ScaleY => (float)scaleXUD.Value;
        public float ScaleZ => (float)scaleXUD.Value;

        public string TargetBone => boneListCB.GetSelectedText();

        public BatchEditBaseAnimDataForm()
        {
            InitializeComponent();
        }

        public bool HasCustomScale
        {
            get
            {
                return ScaleX != 1 || ScaleY != 1 || ScaleZ != 1;
            }
        }

        public void LoadAnim(Animation anim)
        {
            foreach (var bone in anim.Bones)
                boneListCB.Items.Add(bone.Text);

            boneListCB.SelectedIndex = 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Animations;

namespace FirstPlugin.Forms
{
    public partial class VisibiltyAnimEditor : UserControl
    {
        public VisibiltyAnimEditor()
        {
            InitializeComponent();

            listViewCustom1.CanResizeList = false;
        }

        public void LoadAnim(VisibilityAnimation anim)
        {
            listViewCustom1.Items.Clear();
            listViewCustom1.Columns.Clear();

            ColumnHeader frameColumn = new ColumnHeader() { Text = $"Frame" };
            listViewCustom1.Columns.Add(frameColumn);

            foreach (var bone in anim.BoneNames)
            {
                ColumnHeader boneColumn = new ColumnHeader() { Text = $"{bone}" };
                listViewCustom1.Columns.Add(boneColumn);
            }

            if (anim.BaseValues != null && anim.BaseValues.Length > 0)
            {
                var item1 = new ListViewItem($"Base Value");
                listViewCustom1.Items.Add(item1);

                int curve = 1;
                foreach (var value in anim.BaseValues)
                {
                    item1.SubItems.Add(value.ToString());

                    if (value)
                        item1.SubItems[curve].BackColor = Color.FromArgb(70, 70, 70);
                    curve++;

                }
            }
    

            for (int Frame = 0; Frame < anim.FrameCount; Frame++)
            {
                if (anim.Values.Count == 0)
                    return;

                var item1 = new ListViewItem($"{Frame}");
                listViewCustom1.Items.Add(item1);
                item1.UseItemStyleForSubItems = false;

                int curve = 1;
                foreach (var track in anim.Values)
                {
                    bool value = track.GetValue(Frame);
                    item1.SubItems.Add(value.ToString());

                    if (value)
                        item1.SubItems[curve].BackColor = Color.FromArgb(70,70,70);

                    curve++;
                }
            }
        }
    }
}

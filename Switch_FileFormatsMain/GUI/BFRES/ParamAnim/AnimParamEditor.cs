using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syroot.NintenTools.Bfres;
using Switch_Toolbox.Library;
using Bfres.Structs;
using Switch_Toolbox.Library.Animations;

namespace FirstPlugin.Forms
{
    public partial class AnimParamEditor : UserControl
    {
        public AnimParamEditor()
        {
            InitializeComponent();
        }

        public enum ColorOffsets
        {
            Red = 0,
            Green = 4,
            Blue = 8,
            Alpha = 12,
        }

        public enum TexSRT
        {
            Mode = 0,
            ScaleX = 4,
            ScaleY = 8,
            Rotate = 12,
            TransX = 16,
            TransY = 20,
        }

        MaterialAnimation activeAnimation;
        MaterialAnimation.Material activeMaterial;

        public bool ShowKeyedOnly = true;

        public void LoadAnim(MaterialAnimation anim)
        {
            activeAnimation = anim;

            listViewCustom1.Items.Clear();
            listViewCustom1.Columns.Clear();
            materialCB.Items.Clear();

            foreach (var material in anim.Materials) {
                materialCB.Items.Add(material.Text);
            }

            if (materialCB.Items.Count > 0)
                materialCB.SelectedIndex = 0;
        }

        private void LoadParam(MaterialAnimation anim, MaterialAnimation.ParamKeyGroup paramAnim)
        {
            listViewCustom1.Columns.Clear();
            listViewCustom1.Items.Clear();

            ColumnHeader frameColumn = new ColumnHeader() { Text = $"Frame" };
            listViewCustom1.Columns.Add(frameColumn);

            if (paramAnim.Type == MaterialAnimation.AnimationType.Color)
            {
                ReadColorAnim(anim, paramAnim);
            }
            else if (anim.AnimType == MaterialAnimation.AnimationType.TextureSrt)
            {
                foreach (var track in paramAnim.Values)
                {
                    ColumnHeader valueColumn = new ColumnHeader() { Text = $"{(TexSRT)track.AnimDataOffset}" };
                    listViewCustom1.Columns.Add(valueColumn);
                }
                for (int Frame = 0; Frame <= anim.FrameCount; Frame++)
                {
                    if (IsKeyed(paramAnim, Frame))
                    {
                        var item1 = new ListViewItem($"{Frame}");
                        listViewCustom1.Items.Add(item1);

                        foreach (var track in paramAnim.Values)
                        {
                            var keyFrame = track.GetKeyFrame(Frame, false);

                            if (keyFrame != null)
                            {
                                if (track.AnimDataOffset == (uint)TexSRT.Mode)
                                    item1.SubItems.Add(((uint)keyFrame.Value).ToString());
                                else
                                    item1.SubItems.Add(keyFrame.Value.ToString());
                            }
                            else
                                item1.SubItems.Add("");
                        }
                    }
                }
            }
            else
            {
                int columnCount = paramAnim.Values.Count;
                for (int col = 0; col < columnCount; col++)
                {
                    ColumnHeader valueColumn = new ColumnHeader() { Text = $"Value {col}" };
                    listViewCustom1.Columns.Add(valueColumn);
                }
                for (int Frame = 0; Frame <= anim.FrameCount; Frame++)
                {
                    if (IsKeyed(paramAnim, Frame))
                    {
                        var item1 = new ListViewItem($"{Frame}");
                        listViewCustom1.Items.Add(item1);

                        foreach (var track in paramAnim.Values)
                        {
                            var keyFrame = track.GetKeyFrame(Frame);

                            if (keyFrame != null)
                                item1.SubItems.Add(keyFrame.Value.ToString());
                            else
                                item1.SubItems.Add("");

                        }
                    }
                }
            }
        }

        private bool IsKeyed(MaterialAnimation.ParamKeyGroup paramKeyGroup, int Frame)
        {
            foreach (var track in paramKeyGroup.Values)
            {
                if (track.GetKeyFrame(Frame, false) != null && track.GetKeyFrame(Frame).IsKeyed == true)
                    return true;
            }

            return false;
        }

        private void ReadColorAnim(MaterialAnimation anim, MaterialAnimation.ParamKeyGroup paramAnim)
        {

            ColumnHeader colorHeader = new ColumnHeader() { Text = "Color" };
            ColumnHeader alphaHeader = new ColumnHeader() { Text = "Alpha" };
            ColumnHeader RHeader = new ColumnHeader() { Text = "R" };
            ColumnHeader GHeader = new ColumnHeader() { Text = "G" };
            ColumnHeader BHeader = new ColumnHeader() { Text = "B" };
            ColumnHeader AHeader = new ColumnHeader() { Text = "A" };

            listViewCustom1.Columns.Add(colorHeader);
            listViewCustom1.Columns.Add(alphaHeader);
            listViewCustom1.Columns.Add(RHeader);
            listViewCustom1.Columns.Add(GHeader);
            listViewCustom1.Columns.Add(BHeader);
            listViewCustom1.Columns.Add(AHeader);

            for (int Frame = 0; Frame <= anim.FrameCount; Frame++)
            {
                var item1 = new ListViewItem($"{Frame}");
                item1.SubItems.Add("Color");
                item1.SubItems.Add("Alpha");
                item1.UseItemStyleForSubItems = false;

                int Red = 0, Green = 0, Blue = 0, Alpha = 255;
                foreach (var track in paramAnim.Values)
                {
                    Console.WriteLine((ColorOffsets)track.AnimDataOffset + " " + track.GetValue(Frame) + " " + paramAnim.UniformName);
                    if ((ColorOffsets)track.AnimDataOffset == ColorOffsets.Red)
                    {
                        Red = Utils.FloatToIntClamp(track.GetValue(Frame));
                        item1.SubItems.Add($"R {track.GetValue(Frame)}");
                    }
                    if ((ColorOffsets)track.AnimDataOffset == ColorOffsets.Green)
                    {
                        Green = Utils.FloatToIntClamp(track.GetValue(Frame));
                        item1.SubItems.Add($"G {track.GetValue(Frame)}");
                    }
                    if ((ColorOffsets)track.AnimDataOffset == ColorOffsets.Blue)
                    {
                        Blue = Utils.FloatToIntClamp(track.GetValue(Frame));
                        item1.SubItems.Add($"B {track.GetValue(Frame)}");
                    }
                    if ((ColorOffsets)track.AnimDataOffset == ColorOffsets.Alpha)
                    {
                        Alpha = Utils.FloatToIntClamp(track.GetValue(Frame));
                        item1.SubItems.Add($"A {track.GetValue(Frame)}");
                    }
                }
                item1.SubItems[1].BackColor = Color.FromArgb(Red, Green, Blue);
                item1.SubItems[2].BackColor = Color.FromArgb(Alpha, Alpha, Alpha);

                listViewCustom1.Items.Add(item1);
            }
        }

        private void stButton1_Click(object sender, EventArgs e)
        {

        }

        private void btnEditMaterial_Click(object sender, EventArgs e)
        {
            ParamPatternMaterialEditor editor = new ParamPatternMaterialEditor();
            editor.LoadAnim(activeAnimation);

            if (editor.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void btnEditSamplers_Click(object sender, EventArgs e)
        {

        }

        private void materialCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialCB.SelectedIndex >= 0)
            {
                activeMaterial = activeAnimation.Materials[materialCB.SelectedIndex];

                paramCB.Items.Clear();
                foreach (var param in activeMaterial.Params)
                {
                    paramCB.Items.Add(param.Text);
                }

                if (paramCB.Items.Count > 0)
                    paramCB.SelectedIndex = 0;
            }
        }

        private void paramCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (paramCB.SelectedIndex >= 0)
            {
                var activeParam = activeMaterial.Params[paramCB.SelectedIndex];
                LoadParam(activeAnimation, activeParam);
            }
        }
    }
}

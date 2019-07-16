using System;
using Toolbox.Library.Forms;
using Bfres.Structs;
using Syroot.NintenTools.NSW.Bfres.GFX;
using System.Linq;
namespace FirstPlugin.Forms
{
    public partial class VertexBufferEncodeEditor : STForm
    {
        public VertexBufferEncodeEditor()
        {
            InitializeComponent();

            CanResize = false;
        }

        public FSHP.VertexAttribute activeAttribute;

        public void LoadAttribute(FSHP.VertexAttribute attribute)
        {
            activeAttribute = attribute;

            SetFormat(attribute);
            //  bool HasFormat = formatCB.Items.Cast<ComboBoxItem>().Any(cbi => cbi.Content.Equals(attribute.Format.ToString()));

            nameTB.Text = attribute.Name;

            bool HasFormat = formatCB.Items.Contains(attribute.Format);

            if (!HasFormat)
            {
                formatCB.Items.Add(attribute.Format);
            }

            formatCB.SelectedItem = attribute.Format;
        }

        private void SetFormat(FSHP.VertexAttribute attribute)
        {
            var index = string.Concat(attribute.Name.ToArray().Reverse().TakeWhile(char.IsNumber).Reverse());

            if (attribute.Name == $"_p{index}")
            {
                formatCB.Items.Add(AttribFormat.Format_32_32_32_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_16_16_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_16_16_SNorm);
                formatCB.Items.Add(AttribFormat.Format_10_10_10_2_SNorm);
                formatCB.Items.Add(AttribFormat.Format_8_8_8_8_SNorm);
            }
            if (attribute.Name == $"_n{index}")
            {
                formatCB.Items.Add(AttribFormat.Format_32_32_32_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_16_16_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_16_16_SNorm);
                formatCB.Items.Add(AttribFormat.Format_10_10_10_2_SNorm);
                formatCB.Items.Add(AttribFormat.Format_8_8_8_8_SNorm);
            }
            if (attribute.Name == $"_i{index}")
            {
                formatCB.Items.Add(AttribFormat.Format_32_32_32_32_UInt);
                formatCB.Items.Add(AttribFormat.Format_16_16_16_16_UInt);
                formatCB.Items.Add(AttribFormat.Format_8_8_8_8_UInt);
                formatCB.Items.Add(AttribFormat.Format_32_32_32_UInt);
                formatCB.Items.Add(AttribFormat.Format_32_32_UInt);
                formatCB.Items.Add(AttribFormat.Format_16_16_UInt);
                formatCB.Items.Add(AttribFormat.Format_8_8_UInt);
                formatCB.Items.Add(AttribFormat.Format_32_UInt);
                formatCB.Items.Add(AttribFormat.Format_16_UInt);
                formatCB.Items.Add(AttribFormat.Format_8_UInt);
            }
            if (attribute.Name == $"_w{index}")
            {
                formatCB.Items.Add(AttribFormat.Format_32_32_32_32_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_16_16_UNorm);
                formatCB.Items.Add(AttribFormat.Format_8_8_8_8_UNorm);
                formatCB.Items.Add(AttribFormat.Format_32_32_32_Single);
                formatCB.Items.Add(AttribFormat.Format_32_32_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_UNorm);
                formatCB.Items.Add(AttribFormat.Format_8_8_UNorm);
                formatCB.Items.Add(AttribFormat.Format_8_UNorm);
            }
            if (attribute.Name == $"_t{index}")
            {
                formatCB.Items.Add(AttribFormat.Format_32_32_32_32_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_16_16_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_16_16_SNorm);
                formatCB.Items.Add(AttribFormat.Format_10_10_10_2_SNorm);
                formatCB.Items.Add(AttribFormat.Format_8_8_8_8_SNorm);
            }
            if (attribute.Name == $"_b{index}")
            {
                formatCB.Items.Add(AttribFormat.Format_32_32_32_32_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_16_16_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_16_16_SNorm);
                formatCB.Items.Add(AttribFormat.Format_10_10_10_2_SNorm);
                formatCB.Items.Add(AttribFormat.Format_8_8_8_8_SNorm);
            }
            if (attribute.Name == $"_u{index}")
            {
                formatCB.Items.Add(AttribFormat.Format_32_32_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_SNorm);
                formatCB.Items.Add(AttribFormat.Format_8_8_SNorm);
                formatCB.Items.Add(AttribFormat.Format_8_8_UNorm);
            }
            if (attribute.Name == $"_c{index}")
            {
                formatCB.Items.Add(AttribFormat.Format_32_32_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_Single);
                formatCB.Items.Add(AttribFormat.Format_16_16_SNorm);
                formatCB.Items.Add(AttribFormat.Format_10_10_10_2_SNorm);
                formatCB.Items.Add(AttribFormat.Format_8_8_SNorm);
            }
        }

        private void nameTB_TextChanged(object sender, EventArgs e)
        {
            activeAttribute.Name = nameTB.Text;
        }

        private void formatCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            activeAttribute.Format = (AttribFormat)formatCB.SelectedItem;
        }
    }
}

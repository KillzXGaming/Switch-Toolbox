using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syroot.NintenTools.NSW.Bfres;
using Switch_Toolbox.Library;
using OpenTK;
using Bfres.Structs;

namespace FirstPlugin
{
    public partial class SetParamDialog : Form
    {
        public SetParamDialog()
        {
            InitializeComponent();

            foreach (var type in Enum.GetValues(typeof(ShaderParamType)).Cast<ShaderParamType>())
                comboBox1.Items.Add(type);


            if (!Runtime.IsDebugMode)
            {
                comboBox1.Enabled = false;
                textBox1.ReadOnly = true;
            }
            colorLabel.Visible = false;
            pictureBox1.Visible = false;

        }

        BoolValuesPanel boolPanel;
        SRTValuesPanel SRTPanel;
        FloatsIntsValuePanel FloatIntsPanel;
        public BfresShaderParam activeParam;

        public void LoadParam(BfresShaderParam param)
        {
            panel1.Controls.Clear();

            activeParam = param;
            bool IsColor = activeParam.Name.Contains("Color") || activeParam.Name.Contains("color");

            if (IsColor)
            {
                colorLabel.Visible = true;
                pictureBox1.Visible = true;
                SetColorBox(activeParam);
            }

            comboBox1.SelectedItem = activeParam.Type;
            textBox1.Text = activeParam.Name;
            switch (activeParam.Type)
            {
                case ShaderParamType.Bool:
                case ShaderParamType.Bool2:
                case ShaderParamType.Bool3:
                case ShaderParamType.Bool4:
                    boolPanel = new BoolValuesPanel();
                    boolPanel.LoadValues(activeParam.ValueBool);
                    panel1.Controls.Add(boolPanel);
                    break;
                case ShaderParamType.Float:
                case ShaderParamType.Float2:
                case ShaderParamType.Float3:
                case ShaderParamType.Float4:
                case ShaderParamType.Float2x2:
                case ShaderParamType.Float2x3:
                case ShaderParamType.Float2x4:
                case ShaderParamType.Float3x2:
                case ShaderParamType.Float3x3:
                case ShaderParamType.Float3x4:
                case ShaderParamType.Float4x2:
                case ShaderParamType.Float4x3:
                case ShaderParamType.Float4x4:
                    FloatIntsPanel = new FloatsIntsValuePanel();
                    FloatIntsPanel.LoadValues(activeParam.ValueFloat);
                    panel1.Controls.Add(FloatIntsPanel);
                    break;
                case ShaderParamType.Int:
                case ShaderParamType.Int2:
                case ShaderParamType.Int3:
                case ShaderParamType.Int4:
                    FloatIntsPanel = new FloatsIntsValuePanel();
                    FloatIntsPanel.LoadValues(activeParam.ValueInt);
                    panel1.Controls.Add(FloatIntsPanel);
                    break;
                case ShaderParamType.Reserved2:
                case ShaderParamType.Reserved3:
                case ShaderParamType.Reserved4:
                    break;
                case ShaderParamType.Srt2D:
                    SRTPanel = new SRTValuesPanel();
                    SRTPanel.LoadValues(activeParam.ValueSrt2D);
                    panel1.Controls.Add(SRTPanel);
                    break;
                case ShaderParamType.Srt3D:
                    SRTPanel = new SRTValuesPanel();
                    SRTPanel.LoadValues(activeParam.ValueSrt3D);
                    panel1.Controls.Add(SRTPanel);
                    break;
                case ShaderParamType.TexSrt:
                    SRTPanel = new SRTValuesPanel();
                    SRTPanel.LoadValues(activeParam.ValueTexSrt);
                    panel1.Controls.Add(SRTPanel);
                    break;
                case ShaderParamType.TexSrtEx:
                    SRTPanel = new SRTValuesPanel();
                    SRTPanel.LoadValues(activeParam.ValueTexSrtEx);
                    panel1.Controls.Add(SRTPanel);
                    break;
                case ShaderParamType.UInt:
                case ShaderParamType.UInt2:
                case ShaderParamType.UInt3:
                case ShaderParamType.UInt4:
                    FloatIntsPanel = new FloatsIntsValuePanel();
                    FloatIntsPanel.LoadValues(activeParam.ValueUint);
                    panel1.Controls.Add(FloatIntsPanel);
                    break;
            }
        }
        public void SetColorBox(BfresShaderParam param)
        {
            Vector4 color = new Vector4();
            switch (param.Type)
            {
                case ShaderParamType.Float3:
                    color = new Vector4(param.ValueFloat[0], param.ValueFloat[1], param.ValueFloat[2], 1);
                    break;
                case ShaderParamType.Float4:
                    color = new Vector4(param.ValueFloat[0], param.ValueFloat[1], param.ValueFloat[2], param.ValueFloat[3]);
                    break;
            }

            int someIntX = (int)Math.Ceiling(color.X * 255);
            int someIntY = (int)Math.Ceiling(color.Y * 255);
            int someIntZ = (int)Math.Ceiling(color.Z * 255);
            int someIntW = (int)Math.Ceiling(color.W * 255);

            if (someIntX <= 255 && someIntY <= 255 && someIntZ <= 255 && someIntW <= 255)
            {
                pictureBox1.BackColor = Color.FromArgb(
            someIntW,
            someIntX,
            someIntY,
            someIntZ
            );
            }
        }

        public void SetValues()
        {
            if (boolPanel != null)
            {
                boolPanel.SetValues(activeParam);
            }
            if (SRTPanel != null)
            {
                SRTPanel.SetValues(activeParam);
            }
            if (FloatIntsPanel != null)
            {
                FloatIntsPanel.SetValues(activeParam);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ColorDialog clr = new ColorDialog();

            if (clr.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.BackColor = clr.Color;

                switch (activeParam.Type)
                {
                    case ShaderParamType.Float4:
                        activeParam.ValueFloat = new float[4];
                        activeParam.ValueFloat[0] = (float)clr.Color.R / 255;
                        activeParam.ValueFloat[1] = (float)clr.Color.G / 255;
                        activeParam.ValueFloat[2] = (float)clr.Color.B / 255;
                        activeParam.ValueFloat[3] = (float)clr.Color.A / 255;
                        break;
                    case ShaderParamType.Float3:
                        activeParam.ValueFloat = new float[3];
                        activeParam.ValueFloat[0] = (float)clr.Color.R / 255;
                        activeParam.ValueFloat[1] = (float)clr.Color.G / 255;
                        activeParam.ValueFloat[2] = (float)clr.Color.B / 255;
                        break;
                }
                LoadParam(activeParam);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                if (comboBox1.SelectedItem is ShaderParamType)
                {
          //          activeParam.Type = (ShaderParamType)comboBox1.SelectedItem;
          //          SetNewType();
          //          LoadParam(activeParam);
                }
            }
        }
        private void SetNewType()
        {
            switch (activeParam.Type)
            {
                case ShaderParamType.Bool:
                    activeParam.ValueBool = new bool[1];
                    break;
                case ShaderParamType.Bool2:
                    activeParam.ValueBool = new bool[2];
                    break;
                case ShaderParamType.Bool3:
                    activeParam.ValueBool = new bool[3];
                    break;
                case ShaderParamType.Bool4:
                    activeParam.ValueBool = new bool[4];
                    break;
                case ShaderParamType.Float:
                    activeParam.ValueFloat = new float[1];
                    break;
                case ShaderParamType.Float2:
                    activeParam.ValueFloat = new float[2];
                    break;
                case ShaderParamType.Float3:
                    activeParam.ValueFloat = new float[3];
                    break;
                case ShaderParamType.Float4:
                    activeParam.ValueFloat = new float[4];
                    break;
                case ShaderParamType.Float2x2:
                    activeParam.ValueFloat = new float[4];
                    break;
                case ShaderParamType.Float2x3:
                    activeParam.ValueFloat = new float[5];
                    break;
                case ShaderParamType.Float2x4:
                    activeParam.ValueFloat = new float[6];
                    break;
                case ShaderParamType.Float3x2:
                    activeParam.ValueFloat = new float[7];
                    break;
                case ShaderParamType.Float3x3:
                    activeParam.ValueFloat = new float[9];
                    break;
                case ShaderParamType.Float3x4:
                    activeParam.ValueFloat = new float[10];
                    break;
                case ShaderParamType.Float4x2:
                    activeParam.ValueFloat = new float[14];
                    break;
                case ShaderParamType.Float4x3:
                    activeParam.ValueFloat = new float[15];
                    break;
                case ShaderParamType.Float4x4:
                    activeParam.ValueFloat = new float[16];
                    break;
                case ShaderParamType.Int:
                    activeParam.ValueInt = new int[1];
                    break;
                case ShaderParamType.Int2:
                    activeParam.ValueInt = new int[2];
                    break;
                case ShaderParamType.Int3:
                    activeParam.ValueInt = new int[3];
                    break;
                case ShaderParamType.Int4:
                    activeParam.ValueInt = new int[4];
                    break;
                case ShaderParamType.Reserved2:
                    activeParam.ValueReserved = new byte[2];
                    break;
                case ShaderParamType.Reserved3:
                    activeParam.ValueReserved = new byte[3];
                    break;
                case ShaderParamType.Reserved4:
                    activeParam.ValueReserved = new byte[4];
                    break;
                case ShaderParamType.Srt2D:
                    activeParam.ValueSrt2D = new Srt2D();
                    activeParam.ValueSrt2D.Scaling = new Syroot.Maths.Vector2F(0, 0);
                    activeParam.ValueSrt2D.Translation = new Syroot.Maths.Vector2F(0,0);
                    activeParam.ValueSrt2D.Rotation = 0;
                    break;
                case ShaderParamType.Srt3D:
                    activeParam.ValueSrt3D = new Srt3D();
                    activeParam.ValueSrt3D.Scaling = new Syroot.Maths.Vector3F(0, 0, 0);
                    activeParam.ValueSrt3D.Translation = new Syroot.Maths.Vector3F(0, 0, 0);
                    activeParam.ValueSrt3D.Rotation = new Syroot.Maths.Vector3F(0, 0,0);
                    break;
                case ShaderParamType.TexSrt:
                    activeParam.ValueTexSrt = new TexSrt();
                    activeParam.ValueTexSrt.Mode = TexSrtMode.ModeMaya;
                    activeParam.ValueTexSrt.Scaling = new Syroot.Maths.Vector2F(0, 0);
                    activeParam.ValueTexSrt.Translation = new Syroot.Maths.Vector2F(0, 0);
                    activeParam.ValueTexSrt.Rotation = 0;
                    break;
                case ShaderParamType.TexSrtEx:
                    activeParam.ValueTexSrtEx = new TexSrtEx();
                    activeParam.ValueTexSrtEx.Mode = TexSrtMode.ModeMaya;
                    activeParam.ValueTexSrtEx.Scaling = new Syroot.Maths.Vector2F(0, 0);
                    activeParam.ValueTexSrtEx.Translation = new Syroot.Maths.Vector2F(0, 0);
                    activeParam.ValueTexSrtEx.Rotation = 0;
                    activeParam.ValueTexSrtEx.MatrixPointer = 0;
                    break;
                case ShaderParamType.UInt:
                    activeParam.ValueUint = new uint[1];
                    break;
                case ShaderParamType.UInt2:
                    activeParam.ValueUint = new uint[2];
                    break;
                case ShaderParamType.UInt3:
                    activeParam.ValueUint = new uint[3];
                    break;
                case ShaderParamType.UInt4:
                    activeParam.ValueUint = new uint[4];
                    break;
            }
        }
    }
}

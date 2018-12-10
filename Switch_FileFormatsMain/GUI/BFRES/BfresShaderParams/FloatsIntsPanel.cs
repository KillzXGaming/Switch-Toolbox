using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Bfres.Structs;

namespace FirstPlugin
{
    public partial class FloatsIntsValuePanel : UserControl
    {
        public FloatsIntsValuePanel()
        {
            InitializeComponent();
            HideControls();
        }
        public void SetValues(BfresShaderParam param)
        {
            switch (param.Type)
            {
                case Syroot.NintenTools.NSW.Bfres.ShaderParamType.Float:
                    param.ValueFloat = new float[] { (float)ValueUD1.Value };
                    break;
                case Syroot.NintenTools.NSW.Bfres.ShaderParamType.Float2:
                    param.ValueFloat = new float[] { (float)ValueUD1.Value, (float)ValueUD2.Value };
                    break;
                case Syroot.NintenTools.NSW.Bfres.ShaderParamType.Float3:
                    param.ValueFloat = new float[] { (float)ValueUD1.Value, (float)ValueUD2.Value,
                                                     (float)ValueUD3.Value };
                    break;
                case Syroot.NintenTools.NSW.Bfres.ShaderParamType.Float4:
                    param.ValueFloat = new float[] { (float)ValueUD1.Value, (float)ValueUD2.Value,
                                                     (float)ValueUD3.Value, (float)ValueUD4.Value };
                    break;
                case Syroot.NintenTools.NSW.Bfres.ShaderParamType.UInt:
                    param.ValueUint = new uint[] { (uint)ValueUD1.Value };
                    break;
                case Syroot.NintenTools.NSW.Bfres.ShaderParamType.UInt2:
                    param.ValueUint = new uint[] { (uint)ValueUD1.Value, (uint)ValueUD2.Value };
                    break;
                case Syroot.NintenTools.NSW.Bfres.ShaderParamType.UInt3:
                    param.ValueUint = new uint[] { (uint)ValueUD1.Value, (uint)ValueUD2.Value,
                                                   (uint)ValueUD3.Value };
                    break;
                case Syroot.NintenTools.NSW.Bfres.ShaderParamType.UInt4:
                    param.ValueUint = new uint[] { (uint)ValueUD1.Value, (uint)ValueUD2.Value,
                                                     (uint)ValueUD3.Value, (uint)ValueUD4.Value };
                    break;
                case Syroot.NintenTools.NSW.Bfres.ShaderParamType.Int:
                    param.ValueInt = new int[] { (int)ValueUD1.Value };
                    break;
                case Syroot.NintenTools.NSW.Bfres.ShaderParamType.Int2:
                    param.ValueInt = new int[] { (int)ValueUD1.Value, (int)ValueUD2.Value };
                    break;
                case Syroot.NintenTools.NSW.Bfres.ShaderParamType.Int3:
                    param.ValueInt = new int[] { (int)ValueUD1.Value, (int)ValueUD2.Value,
                                                 (int)ValueUD3.Value };
                    break;
                case Syroot.NintenTools.NSW.Bfres.ShaderParamType.Int4:
                    param.ValueInt = new int[] { (int)ValueUD1.Value, (int)ValueUD2.Value,
                                                 (int)ValueUD3.Value, (int)ValueUD4.Value };
                    break;
            }
        }
        public void LoadValues(float[] values)
        {
            if (values.Length >= 1)
            {
                ValueUD1.Value = (decimal)values[0];
            }
            if (values.Length >= 2)
            {
                ValueUD2.Value = (decimal)values[1];
                ValueUD2.Visible = true;
            }
            if (values.Length >= 3)
            {
                ValueUD3.Value = (decimal)values[2];
                ValueUD3.Visible = true;
            }
            if (values.Length >= 4)
            {
                ValueUD4.Value = (decimal)values[3];
                ValueUD4.Visible = true;
            }
            if (values.Length >= 5)
            {
                ValueUD5.Value = (decimal)values[4];
                ValueUD5.Visible = true;
            }
            if (values.Length >= 6)
            {
                ValueUD6.Value = (decimal)values[5];
                ValueUD6.Visible = true;
            }
            if (values.Length >= 7)
            {
                ValueUD7.Value = (decimal)values[6];
                ValueUD7.Visible = true;
            }
            if (values.Length >= 8)
            {
                ValueUD8.Value = (decimal)values[7];
                ValueUD8.Visible = true;
            }
            if (values.Length >= 9)
            {
                ValueUD9.Value = (decimal)values[8];
                ValueUD9.Visible = true;
            }
            if (values.Length >= 10)
            {
                ValueUD10.Value = (decimal)values[9];
                ValueUD10.Visible = true;
            }
            if (values.Length >= 11)
            {
                ValueUD11.Value = (decimal)values[10];
                ValueUD11.Visible = true;
            }
            if (values.Length >= 12)
            {
                ValueUD12.Value = (decimal)values[11];
                ValueUD12.Visible = true;
            }
            if (values.Length >= 13)
            {
                ValueUD13.Value = (decimal)values[12];
                ValueUD13.Visible = true;
            }
            if (values.Length >= 14)
            {
                ValueUD14.Value = (decimal)values[13];
                ValueUD14.Visible = true;
            }
            if (values.Length >= 15)
            {
                ValueUD15.Value = (decimal)values[14];
                ValueUD15.Visible = true;
            }
            if (values.Length >= 16)
            {
                ValueUD16.Value = (decimal)values[15];
                ValueUD16.Visible = true;
            }
        }
        public void LoadValues(uint[] values)
        {
            SetAllProperties(0, 2147483647, 0);

            if (values.Length >= 1)
            {
                ValueUD1.Value = (decimal)values[0];
            }
            if (values.Length >= 2)
            {
                ValueUD2.Value = (decimal)values[1];
                ValueUD2.Visible = true;
            }
            if (values.Length >= 3)
            {
                ValueUD3.Value = (decimal)values[2];
                ValueUD3.Visible = true;
            }
            if (values.Length >= 4)
            {
                ValueUD4.Value = (decimal)values[3];
                ValueUD4.Visible = true;
            }
            if (values.Length >= 5)
            {
                ValueUD5.Value = (decimal)values[4];
                ValueUD5.Visible = true;
            }
            if (values.Length >= 6)
            {
                ValueUD6.Value = (decimal)values[5];
                ValueUD6.Visible = true;
            }
            if (values.Length >= 7)
            {
                ValueUD7.Value = (decimal)values[6];
                ValueUD7.Visible = true;
            }
            if (values.Length >= 8)
            {
                ValueUD8.Value = (decimal)values[7];
                ValueUD8.Visible = true;
            }
            if (values.Length >= 9)
            {
                ValueUD9.Value = (decimal)values[8];
                ValueUD9.Visible = true;
            }
            if (values.Length >= 10)
            {
                ValueUD10.Value = (decimal)values[9];
                ValueUD10.Visible = true;
            }
            if (values.Length >= 11)
            {
                ValueUD11.Value = (decimal)values[10];
                ValueUD11.Visible = true;
            }
            if (values.Length >= 12)
            {
                ValueUD12.Value = (decimal)values[11];
                ValueUD12.Visible = true;
            }
            if (values.Length >= 13)
            {
                ValueUD13.Value = (decimal)values[12];
                ValueUD13.Visible = true;
            }
            if (values.Length >= 14)
            {
                ValueUD14.Value = (decimal)values[13];
                ValueUD14.Visible = true;
            }
            if (values.Length >= 15)
            {
                ValueUD15.Value = (decimal)values[14];
                ValueUD15.Visible = true;
            }
            if (values.Length >= 16)
            {
                ValueUD16.Value = (decimal)values[15];
                ValueUD16.Visible = true;
            }
        }
        public void LoadValues(int[] values)
        {
            SetAllProperties(-2147483647, 2147483647, 0);

            if (values.Length >= 1)
            {
                ValueUD1.Value = (decimal)values[0];
            }
            if (values.Length >= 2)
            {
                ValueUD2.Value = (decimal)values[1];
                ValueUD2.Visible = true;
            }
            if (values.Length >= 3)
            {
                ValueUD3.Value = (decimal)values[2];
                ValueUD3.Visible = true;
            }
            if (values.Length >= 4)
            {
                ValueUD4.Value = (decimal)values[3];
                ValueUD4.Visible = true;
            }
            if (values.Length >= 5)
            {
                ValueUD5.Value = (decimal)values[4];
                ValueUD5.Visible = true;
            }
            if (values.Length >= 6)
            {
                ValueUD6.Value = (decimal)values[5];
                ValueUD6.Visible = true;
            }
            if (values.Length >= 7)
            {
                ValueUD7.Value = (decimal)values[6];
                ValueUD7.Visible = true;
            }
            if (values.Length >= 8)
            {
                ValueUD8.Value = (decimal)values[7];
                ValueUD8.Visible = true;
            }
            if (values.Length >= 9)
            {
                ValueUD9.Value = (decimal)values[8];
                ValueUD9.Visible = true;
            }
            if (values.Length >= 10)
            {
                ValueUD10.Value = (decimal)values[9];
                ValueUD10.Visible = true;
            }
            if (values.Length >= 11)
            {
                ValueUD11.Value = (decimal)values[10];
                ValueUD11.Visible = true;
            }
            if (values.Length >= 12)
            {
                ValueUD12.Value = (decimal)values[11];
                ValueUD12.Visible = true;
            }
            if (values.Length >= 13)
            {
                ValueUD13.Value = (decimal)values[12];
                ValueUD13.Visible = true;
            }
            if (values.Length >= 14)
            {
                ValueUD14.Value = (decimal)values[13];
                ValueUD14.Visible = true;
            }
            if (values.Length >= 15)
            {
                ValueUD15.Value = (decimal)values[14];
                ValueUD15.Visible = true;
            }
            if (values.Length >= 16)
            {
                ValueUD16.Value = (decimal)values[15];
                ValueUD16.Visible = true;
            }
        }
        public void SetAllProperties(int Min, int Max, int DecimalPlaces)
        {
            ValueUD1.Minimum = Min; ValueUD1.Maximum = Max; ValueUD1.DecimalPlaces = DecimalPlaces;
            ValueUD2.Minimum = Min; ValueUD2.Maximum = Max; ValueUD2.DecimalPlaces = DecimalPlaces;
            ValueUD3.Minimum = Min; ValueUD3.Maximum = Max; ValueUD3.DecimalPlaces = DecimalPlaces;
            ValueUD4.Minimum = Min; ValueUD4.Maximum = Max; ValueUD4.DecimalPlaces = DecimalPlaces;
            ValueUD5.Minimum = Min; ValueUD5.Maximum = Max; ValueUD5.DecimalPlaces = DecimalPlaces;
            ValueUD6.Minimum = Min; ValueUD6.Maximum = Max; ValueUD6.DecimalPlaces = DecimalPlaces;
            ValueUD7.Minimum = Min; ValueUD7.Maximum = Max; ValueUD7.DecimalPlaces = DecimalPlaces;
            ValueUD8.Minimum = Min; ValueUD8.Maximum = Max; ValueUD8.DecimalPlaces = DecimalPlaces;
            ValueUD9.Minimum = Min; ValueUD9.Maximum = Max; ValueUD9.DecimalPlaces = DecimalPlaces;
            ValueUD10.Minimum = Min; ValueUD10.Maximum = Max; ValueUD10.DecimalPlaces = DecimalPlaces;
            ValueUD11.Minimum = Min; ValueUD11.Maximum = Max; ValueUD11.DecimalPlaces = DecimalPlaces;
            ValueUD12.Minimum = Min; ValueUD12.Maximum = Max; ValueUD12.DecimalPlaces = DecimalPlaces;
            ValueUD13.Minimum = Min; ValueUD13.Maximum = Max; ValueUD13.DecimalPlaces = DecimalPlaces;
            ValueUD14.Minimum = Min; ValueUD14.Maximum = Max; ValueUD14.DecimalPlaces = DecimalPlaces;
            ValueUD15.Minimum = Min; ValueUD15.Maximum = Max; ValueUD15.DecimalPlaces = DecimalPlaces;
            ValueUD16.Minimum = Min; ValueUD16.Maximum = Max; ValueUD16.DecimalPlaces = DecimalPlaces;
        }
        public void HideControls()
        {
            ValueUD2.Visible = false;
            ValueUD3.Visible = false;
            ValueUD4.Visible = false;
            ValueUD5.Visible = false;
            ValueUD6.Visible = false;
            ValueUD7.Visible = false;
            ValueUD8.Visible = false;
            ValueUD9.Visible = false;
            ValueUD10.Visible = false;
            ValueUD11.Visible = false;
            ValueUD12.Visible = false;
            ValueUD13.Visible = false;
            ValueUD14.Visible = false;
            ValueUD15.Visible = false;
            ValueUD16.Visible = false;
        }

        private void ValueUD_ValueChanged(object sender, EventArgs e)
        {
            Viewport.Instance.UpdateViewport();
        }
    }
}

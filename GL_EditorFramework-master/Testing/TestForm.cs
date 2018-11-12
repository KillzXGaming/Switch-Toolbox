using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GL_Core;

namespace Testing
{
	public partial class TestingForm : Form
	{
		public TestingForm()
		{
			InitializeComponent();
		}

		private void gL_ControlModern1_MouseClick(object sender, MouseEventArgs e)
		{
			if(gL_ControlModern1.MainDrawable==null)
				gL_ControlModern1.MainDrawable = new MarioTposeTest();
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			gL_ControlModern1.Stereoscopy = checkBox1.Checked;
		}
	}
}

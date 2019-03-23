using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GL_EditorFramework
{
	public partial class SceneListView : UserControl
	{
		public SceneListView()
		{
			
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);

			Graphics g = e.Graphics;

			Rectangle rect = new Rectangle(2, 2, Width - 4, 23);
			g.FillRectangle(SystemBrushes.ControlLight, rect);
			g.DrawRectangle(SystemPens.ControlDark, rect);
			int yoff = (int)(Font.GetHeight(e.Graphics.DpiX)/2f);
			g.DrawString("Category", Font, new SolidBrush(ForeColor), 4, 2+rect.Height/2-yoff);
			rect = new Rectangle(2, 32, Width - 4, Height - 34);
			g.FillRectangle(SystemBrushes.Window, rect);
			g.DrawRectangle(SystemPens.ControlDarkDark, rect);
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GL_EditorFramework.EditorDrawables;
using OpenTK;

namespace Testing
{
	public partial class TestingForm : Form
	{
		public TestingForm()
		{
			InitializeComponent();
		}

		private EditorScene scene;

		private Random rng = new Random();

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			scene = new EditorScene();

			listBox1.Items.Add("moving platform");
			scene.objects.Add(new AnimatedObject(new Vector3(0, -4, 0)));
			for (int i = 0; i<5; i++)
			{
				listBox1.Items.Add("block");
				scene.objects.Add(new SingleObject(new Vector3(i,0,0)));
			}

			gL_ControlModern1.MainDrawable = scene;
			gL_ControlModern1.ActiveCamera = new GL_EditorFramework.StandardCameras.InspectCamera(1f);

			gL_ControlLegacy1.MainDrawable = new SingleObject(new Vector3());

			scene.SelectionChanged += Scene_SelectionChanged;
			listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
		}

		private void Scene_SelectionChanged(object sender, EventArgs e)
		{
			listBox1.SelectedIndexChanged -= ListBox1_SelectedIndexChanged;
			listBox1.SelectedIndices.Clear();
			int i = 0;
			foreach(EditableObject o in scene.objects)
			{
				if(o.IsSelected())
					listBox1.SelectedIndices.Add(i);
				i++;
			}

			listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
		}

		private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			List<EditableObject> newSelection = new List<EditableObject>();
			foreach(int i in listBox1.SelectedIndices)
			{
				newSelection.Add(scene.objects[i]);
			}

			scene.SelectedObjects = newSelection;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Color rand = Color.FromArgb(rng.Next());
			listBox1.Items.Add("block"); //make sure to add the entry before you add an object because the SelectionChanged event will be fired
			scene.Add(new SingleObject(new Vector3()));
		}
	}
}

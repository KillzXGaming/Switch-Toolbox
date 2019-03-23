using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GL_EditorFramework.EditorDrawables
{
	public class EditorScene : AbstractGlDrawable
	{
		protected bool multiSelect;

		public EditableObject hovered = null;

		public int hoveredPart = 0;

		public List<EditableObject> objects = new List<EditableObject>();

		protected List<EditableObject> selectedObjects = new List<EditableObject>();

		public List<AbstractGlDrawable> staticObjects = new List<AbstractGlDrawable>();
		
		public event EventHandler SelectionChanged;

		private float draggingDepth;

		private GL_ControlBase control;

		public EditableObject.DeltaTransform deltaTransform = new EditableObject.DeltaTransform(new Vector3(), new Quaternion(), new Vector3(1,1,1));

		public EditorScene(bool multiSelect = true)
		{
			this.multiSelect = multiSelect;
		}

		public List<EditableObject> SelectedObjects
		{
			get => selectedObjects;
			set
			{
				uint var = 0;
				foreach (EditableObject obj in value)
				{
					if (!selectedObjects.Contains(obj)) //object wasn't selected before
						var |= obj.SelectDefault(control); //select it

					else //object stays selected 
						selectedObjects.Remove(obj); //filter out these to find all objects which are not selected anymore

					Console.WriteLine(obj);
				}

				foreach (EditableObject obj in selectedObjects) //now the selected objects are a list of objects to deselect
														  //which is fine because in the end they get overwriten anyway
				{
					var |= obj.DeselectAll(control); //Deselect them all
				}
				selectedObjects = value;

				if ((var & AbstractGlDrawable.REDRAW)>0)
					control.Refresh();
				if ((var & AbstractGlDrawable.REDRAW_PICKING) > 0)
					control.DrawPicking();
			}
		}
		
		public void ToogleSelected(EditableObject obj, bool isSelected)
		{
			uint var = 0;

			bool alreadySelected = selectedObjects.Contains(obj);
			if(alreadySelected && !isSelected)
			{
				var |= obj.DeselectAll(control);
				selectedObjects.Remove(obj);
			}
			else if(!alreadySelected && isSelected)
			{
				var |= obj.SelectDefault(control);
				selectedObjects.Add(obj);
			}
			
			if ((var & AbstractGlDrawable.REDRAW)>0)
				control.Refresh();
			if ((var & AbstractGlDrawable.REDRAW_PICKING) > 0)
				control.DrawPicking();
		}

		public void Add(params EditableObject[] objs)
		{
			uint var = 0;

			foreach (EditableObject selected in selectedObjects)
			{
				var |= selected.DeselectAll(control);
			}
			selectedObjects.Clear();

			foreach (EditableObject obj in objs)
			{
				objects.Add(obj);

				selectedObjects.Add(obj);
				var |= obj.SelectDefault(control);
			}
			SelectionChanged?.Invoke(this, new EventArgs());

			if ((var & AbstractGlDrawable.REDRAW) > 0)
				control.Refresh();
			if ((var & AbstractGlDrawable.REDRAW_PICKING) > 0)
				control.DrawPicking();
		}

		public void Delete(params EditableObject[] objs)
		{
			uint var = 0;

			bool selectionHasChanged = false;

			foreach (EditableObject obj in objs)
			{
				objects.Remove(obj);
				if (selectedObjects.Contains(obj))
				{
					var |= obj.DeselectAll(control);
					selectedObjects.Remove(obj);
				}
			}
			if(selectionHasChanged)
				SelectionChanged?.Invoke(this, new EventArgs());

			if ((var & AbstractGlDrawable.REDRAW) > 0)
				control.Refresh();
			if ((var & AbstractGlDrawable.REDRAW_PICKING) > 0)
				control.DrawPicking();
		}

		public void InsertAfter(int index, params EditableObject[] objs)
		{
			uint var = 0;

			foreach (EditableObject selected in selectedObjects)
			{
				var |= selected.DeselectAll(control);
			}
			selectedObjects.Clear();

			foreach (EditableObject obj in objs)
			{
				objects.Insert(index, obj);

				selectedObjects.Add(obj);
				var |= obj.SelectDefault(control);
				index++;
			}
			SelectionChanged?.Invoke(this, new EventArgs());

			if ((var & AbstractGlDrawable.REDRAW) > 0)
				control.Refresh();
			if ((var & AbstractGlDrawable.REDRAW_PICKING) > 0)
				control.DrawPicking();
		}

		public override void Draw(GL_ControlModern control, Pass pass)
		{
            foreach (EditableObject obj in objects)
			{
				if(obj.Visible)
					obj.Draw(control, pass, this);
			}
			foreach (AbstractGlDrawable obj in staticObjects)
			{
				obj.Draw(control, pass);
			}
		}

		public override void Draw(GL_ControlLegacy control, Pass pass)
		{
			foreach (EditableObject obj in objects)
			{
				if (obj.Visible)
					obj.Draw(control, pass, this);
			}
			foreach (AbstractGlDrawable obj in staticObjects)
			{
				obj.Draw(control, pass);
			}
		}

		public override void Prepare(GL_ControlModern control)
		{
			this.control = control;
			foreach (EditableObject obj in objects)
				obj.Prepare(control);
			foreach (AbstractGlDrawable obj in staticObjects)
				obj.Prepare(control);
		}

		public override void Prepare(GL_ControlLegacy control)
		{
			this.control = control;
			foreach (EditableObject obj in objects)
				obj.Prepare(control);
			foreach (AbstractGlDrawable obj in staticObjects)
				obj.Prepare(control);
		}

		public override uint MouseDown(MouseEventArgs e, I3DControl control)
		{
			uint var = 0;
			if (draggingDepth == -1 && e.Button == MouseButtons.Left && selectedObjects.Contains(hovered))
			{
				if(hovered.CanStartDragging())
					draggingDepth = control.PickingDepth;
			}
			foreach (EditableObject obj in objects)
			{
				var |= obj.MouseDown(e, control);
			}
			foreach (AbstractGlDrawable obj in staticObjects)
			{
				var |= obj.MouseDown(e, control);
			}
			return var;
		}

		public override uint MouseMove(MouseEventArgs e, Point lastMousePos, I3DControl control)
		{
			uint var = 0;

			foreach (EditableObject obj in objects)
			{
				var |= obj.MouseMove(e, lastMousePos, control);
			}
			foreach (AbstractGlDrawable obj in staticObjects)
			{
				var |= obj.MouseMove(e, lastMousePos, control);
			}

			if (draggingDepth != -1)
			{
				Vector3 translation = new Vector3();

				//code from Whitehole

				float deltaX = e.X - control.DragStartPos.X;
				float deltaY = e.Y - control.DragStartPos.Y;

				deltaX *= draggingDepth * control.FactorX;
				deltaY *= draggingDepth * control.FactorY;

				translation += Vector3.UnitX * deltaX * (float)Math.Cos(control.CamRotX);
				translation -= Vector3.UnitX * deltaY * (float)Math.Sin(control.CamRotX) * (float)Math.Sin(control.CamRotY);
				translation -= Vector3.UnitY * deltaY * (float)Math.Cos(control.CamRotY);
				translation += Vector3.UnitZ * deltaX * (float)Math.Sin(control.CamRotX);
				translation += Vector3.UnitZ * deltaY * (float)Math.Cos(control.CamRotX) * (float)Math.Sin(control.CamRotY);

				deltaTransform.Translation = translation;

				var |= REDRAW | NO_CAMERA_ACTION;

				var &= ~REPICK;
			}
			else
			{
				var |= REPICK;
			}
			return var;
		}

		public override uint MouseUp(MouseEventArgs e, I3DControl control)
		{
			uint var = 0;

			if (!(draggingDepth == -1)&&e.Button == MouseButtons.Left)
			{
				foreach (EditableObject obj in selectedObjects)
				{
					obj.ApplyTransformationToSelection(deltaTransform);
				}

				deltaTransform = new EditableObject.DeltaTransform(new Vector3(), new Quaternion(), new Vector3(1, 1, 1));
			}

			foreach (EditableObject obj in objects)
			{
				var |= obj.MouseUp(e, control);
			}
			foreach (AbstractGlDrawable obj in staticObjects)
			{
				var |= obj.MouseUp(e, control);
			}

			draggingDepth = -1;
			return var;
		}

		public override uint MouseClick(MouseEventArgs e, I3DControl control)
		{
			uint var = 0;
			foreach (EditableObject obj in objects)
			{
				var |= obj.MouseClick(e, control);
			}
			foreach (AbstractGlDrawable obj in staticObjects)
			{
				var |= obj.MouseClick(e, control);
			}

			if (!(e.Button == MouseButtons.Left))
				return var;

			if (!(multiSelect && OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.ShiftLeft)))
			{
				if (multiSelect)
				{
					if (!selectedObjects.Contains(hovered))
					{
						foreach (EditableObject selected in selectedObjects)
						{
							selected.DeselectAll(control);
						}
					}

					if (hovered != null && !selectedObjects.Contains(hovered))
					{
						selectedObjects.Clear();
						selectedObjects.Add(hovered);
						hovered.Select(hoveredPart,control);
						SelectionChanged?.Invoke(this, new EventArgs());
					}
					else if(hovered == null)
					{
						selectedObjects.Clear();
						SelectionChanged?.Invoke(this, new EventArgs());
					}
				}
				else
				{
					foreach (EditableObject selected in selectedObjects)
					{
						selected.DeselectAll(control);
					}

					if (hovered != null && !selectedObjects.Contains(hovered))
					{
						selectedObjects.Clear();
						selectedObjects.Add(hovered);
						hovered.Select(hoveredPart, control);
						SelectionChanged?.Invoke(this, new EventArgs());
					}
					else
					{
						selectedObjects.Clear();
						SelectionChanged?.Invoke(this, new EventArgs());
					}
				}
			}
			else
			{
				if (selectedObjects.Contains(hovered))
				{
					selectedObjects.Remove(hovered);
					hovered.Deselect(hoveredPart, control);
					SelectionChanged?.Invoke(this, new EventArgs());
				}
				else if(hovered != null)
				{
					selectedObjects.Add(hovered);
					hovered.Select(hoveredPart, control);
					SelectionChanged?.Invoke(this, new EventArgs());
				}
			}

			draggingDepth = -1; //because MouseClick implies that the Mouse Button is not pressed anymore

			var |= REDRAW;

			return var;
		}

		public override uint MouseWheel(MouseEventArgs e, I3DControl control)
		{
			uint var = 0;
			foreach (EditableObject obj in objects) {
				var |= obj.MouseWheel(e, control);
			}
			foreach (AbstractGlDrawable obj in staticObjects)
			{
				var |= obj.MouseWheel(e, control);
			}
			return var;
		}

		public override int GetPickableSpan()
		{
			int var = 0;
			foreach (EditableObject obj in objects)
				var += obj.GetPickableSpan();
			foreach (AbstractGlDrawable obj in staticObjects)
				var += obj.GetPickableSpan();
			return var;
		}

		public override uint MouseEnter(int index, I3DControl control)
		{
			int inObjectIndex = index;
			if(!(draggingDepth == -1))
				return 0;
			
			foreach (EditableObject obj in objects)
			{
				int span = obj.GetPickableSpan();
                Console.WriteLine(obj.ToString() + " span " + span.ToString());
                Console.WriteLine(obj.ToString() + " inObjectIndex " + inObjectIndex.ToString());

                if (inObjectIndex >= 0 && inObjectIndex < span)
				{
					hovered = obj;
					return obj.MouseEnter(inObjectIndex, control) | REDRAW;
				}
				inObjectIndex -= span;
			}

			foreach (AbstractGlDrawable obj in staticObjects)
			{
				int span = obj.GetPickableSpan();
				if (inObjectIndex >= 0 && inObjectIndex < span)
				{
					return obj.MouseEnter(inObjectIndex, control);
				}
				inObjectIndex -= span;
			}
			return 0;
		}

		public override uint MouseLeave(int index, I3DControl control)
		{
			int inObjectIndex = index;
			foreach (EditableObject obj in objects)
			{
				int span = obj.GetPickableSpan();
				if (inObjectIndex >= 0 && inObjectIndex < span)
				{
					return obj.MouseLeave(inObjectIndex, control);
				}
				inObjectIndex -= span;
			}

			foreach (AbstractGlDrawable obj in staticObjects)
			{
				int span = obj.GetPickableSpan();
				if (inObjectIndex >= 0 && inObjectIndex < span)
				{
					return obj.MouseLeave(inObjectIndex, control);
				}
				inObjectIndex -= span;
			}
			return 0;
		}

		public override uint MouseLeaveEntirely(I3DControl control)
		{
			hovered = null;
			return REDRAW;
		}

		public override uint KeyDown(KeyEventArgs e, I3DControl control)
		{
			uint var = 0;
			if(e.KeyCode == Keys.Z && selectedObjects.Count>0)
			{
				Vector3 sum = new Vector3();
				int index = 0;
				foreach (EditableObject selected in selectedObjects)
				{
					sum -= selected.GetSelectionCenter();
					index++;
				}
				sum /= index;
				control.CameraTarget = sum;

				var = REDRAW_PICKING;
			}else if (e.KeyCode == Keys.H && selectedObjects.Count > 0)
			{
				foreach (EditableObject selected in selectedObjects)
				{
					selected.Visible = e.Shift;
				}
				var = REDRAW_PICKING;
			}
			else if (e.Control && e.KeyCode == Keys.A)
			{
				if (e.Shift)
				{
					foreach (EditableObject selected in selectedObjects)
					{
						selected.DeselectAll(control);
					}
					selectedObjects.Clear();
					SelectionChanged?.Invoke(this, new EventArgs());
				}

				if (!e.Shift && multiSelect)
				{
					foreach (EditableObject obj in objects)
					{
						obj.SelectAll(control);
						selectedObjects.Add(obj);
					}
					SelectionChanged?.Invoke(this, new EventArgs());
				}
				var = REDRAW;
			}

			foreach (EditableObject obj in objects) {
				var |= obj.KeyDown(e, control);
			}
			foreach (AbstractGlDrawable obj in staticObjects)
			{
				var |= obj.KeyDown(e, control);
			}
			return var;
		}

		public override uint KeyUp(KeyEventArgs e, I3DControl control)
		{
			uint var = 0;
			foreach (EditableObject obj in objects) {
				var |= obj.KeyUp(e, control);
			}
			foreach (AbstractGlDrawable obj in staticObjects)
			{
				var |= obj.KeyUp(e, control);
			}
			return var;
		}

		public struct SelectInfo
		{
			public Vector3 LastPos;

			public SelectInfo(Vector3 LastPos)
			{
				this.LastPos = LastPos;
			}
		}

		public struct ObjID : IEquatable<ObjID>
		{
			public int ObjectIndex;
			public int SubObjectIndex;

			public static readonly ObjID None = new ObjID(-1, -1);

			public bool IsNone()
			{
				return (ObjectIndex == -1) || (SubObjectIndex == -1);
			}

			public ObjID(int ObjectIndex, int SubObjectIndex)
			{
				this.ObjectIndex = ObjectIndex;
				this.SubObjectIndex = SubObjectIndex;
			}

			public bool Equals(ObjID other)
			{
				return (ObjectIndex == other.ObjectIndex)&&(SubObjectIndex==other.SubObjectIndex);
			}

			public override int GetHashCode()
			{
				return (ObjectIndex << 32) + SubObjectIndex;
			}
		}

		public abstract class AbstractTransformAction
		{
			public abstract Vector3 newPos(Vector3 pos);

			public Quaternion deltaRotation;

			public Vector3 scale;

			public void SetDragDist(Point point) { }
			public void SetScrollDragDist(Point point, Point projX, Point projY, Point projZ) { }
		}

		public class TranslateAction : AbstractTransformAction
		{
			Vector3 translation = new Vector3();
			public override Vector3 newPos(Vector3 pos)
			{
				return pos + translation;
			}
		}
	}
}

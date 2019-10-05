using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.Maths;

namespace LayoutBXLYT
{
    public class LayoutUndoManager
    {
        protected Stack<IRevertAction> undoStack = new Stack<IRevertAction>();
        protected Stack<IRevertAction> redoStack = new Stack<IRevertAction>();

        public bool HasUndo => undoStack.Count > 0;
        public bool HasRedo => redoStack.Count > 0;

        public void Undo()
        {
            if (undoStack.Count > 0)
                redoStack.Push(undoStack.Pop().Revert());
        }

        public void Redo()
        {
            if (redoStack.Count > 0)
                undoStack.Push(redoStack.Pop().Revert());
        }

        public void AddToUndo(IRevertAction revertable)
        {
            undoStack.Push(revertable);
            redoStack.Clear();
        }

        public interface IRevertAction
        {
            IRevertAction Revert();
        }

        public class UndoActionTransform : IRevertAction
        {
            public Vector3F Translate;
            public Vector3F Rotate;
            public Vector2F Scale;

            public float Width;
            public float Height;

            public BasePane targetPane;


            public UndoActionTransform(BasePane pane)
            {
                targetPane = pane;
                Translate = pane.Translate;
                Scale = pane.Scale;
                Rotate = pane.Rotate;
                Width = pane.Width;
                Height = pane.Height;
            }

            public IRevertAction Revert()
            {
                targetPane.Translate = Translate;
                targetPane.Scale = Scale;
                targetPane.Rotate = Rotate;
                targetPane.Width = Width;
                targetPane.Height = Height;
                return this;
            }
        }
    }
}

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

        public class UndoActionPaneHide : IRevertAction
        {
            private bool hidePane = true;
            public List<BasePane> targetPanes = new List<BasePane>();
            public UndoActionPaneHide(List<BasePane> panes, bool hide = true)
            {
                targetPanes = panes;
                hidePane = hide;

                foreach (var pane in targetPanes)
                    pane.DisplayInEditor = !hidePane;
            }

            public IRevertAction Revert()
            {
                foreach (var pane in targetPanes)
                    pane.DisplayInEditor = hidePane;
                return this;
            }
        }

        public class UndoActionPaneDelete : IRevertAction
        {
            public BxlytHeader layoutFile;
            public List<PaneInfo> targetPanes = new List<PaneInfo>();
            public UndoActionPaneDelete(List<BasePane> panes, BxlytHeader header)
            {
                layoutFile = header;
                for (int i = 0; i < panes.Count; i++)
                    targetPanes.Add(new PaneInfo(panes[i], header.PaneLookup[panes[i].Parent.Name]));
            }

            public IRevertAction Revert()
            {
                for (int i = 0; i < targetPanes.Count; i++)
                    layoutFile.AddPane(targetPanes[i].Pane, targetPanes[i].Parent);
                return this;
            }

            public class PaneInfo
            {
                public BasePane Parent;
                public BasePane Pane;

                public PaneInfo(BasePane pane, BasePane parent)
                {
                    Pane = pane;
                    Parent = parent;
                }
            }
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

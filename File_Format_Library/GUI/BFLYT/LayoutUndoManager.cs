using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.Maths;
using Toolbox.Library;

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

            public List<BasePane> topMostPanes = new List<BasePane>();
            public List<PaneInfo> targetPanes = new List<PaneInfo>();
            public UndoActionPaneDelete(List<BasePane> panes, BxlytHeader header)
            {
                layoutFile = header;
                GetTopMostPanes(panes, topMostPanes, header.RootPane);

                for (int i = 0; i < panes.Count; i++)
                    targetPanes.Add(new PaneInfo(panes[i], panes[i].Parent));
            }

            //Loop through each pane in the heiarchy until it finds the first set of panes
            //The topmost panes are only going to be removed for adding with redo to be easier
            private void GetTopMostPanes(List<BasePane> panes, List<BasePane> topMost, BasePane root)
            {
                foreach (var child in root.Childern)
                {
                    if (panes.Contains(child))
                        topMost.Add(child);
                }

                if (topMost.Count == 0)
                {
                    foreach (var child in root.Childern)
                        GetTopMostPanes(panes, topMost, child);
                }
            }

            public IRevertAction Revert()
            {
                Console.WriteLine("Redo pane remove ");
                for (int i = 0; i < targetPanes.Count; i++)
                {
                    Console.WriteLine("mats " + targetPanes[i].Materials.Count);

                    //Try to add any linked materials. If there is none, the list won't add any
                    if (targetPanes[i].Materials.Count > 0)
                    {
                        var mat = targetPanes[i].Materials[0];
                        var pane = targetPanes[i].Pane;

                        if (pane is IPicturePane)
                        {
                            Console.WriteLine("IPicturePane pane ");

                            layoutFile.AddMaterial(mat, ((IPicturePane)pane).MaterialIndex);
                            ((IPicturePane)pane).Material = mat;
                        }
                        else if (pane is ITextPane)
                        {
                            layoutFile.AddMaterial(mat, ((ITextPane)pane).MaterialIndex);
                            ((ITextPane)pane).Material = mat;
                        }
                        else if (pane is IWindowPane)
                        {
                            var wnd = pane as IWindowPane;
                            layoutFile.AddMaterial(mat, wnd.Content.MaterialIndex);
                            wnd.Content.Material = mat;
                            for (int m = 0; m < wnd.WindowFrames.Count; m++)
                            {
                                mat = targetPanes[i].Materials[m + 1];

                                layoutFile.AddMaterial(mat, wnd.WindowFrames[m].MaterialIndex);
                                wnd.WindowFrames[m].Material = mat;
                            }
                        }
                    }

                    //Add the panes
                    if (!layoutFile.PaneLookup.ContainsKey(targetPanes[i].Pane.Name))
                        layoutFile.PaneLookup.Add(targetPanes[i].Pane.Name, targetPanes[i].Pane);
                }

                //Handle children via topmost list
                foreach (var pane in topMostPanes)
                    layoutFile.AddPane(pane, pane.Parent);

                return this;
            }

            public class PaneInfo
            {
                public BasePane Parent;
                public BasePane Pane;

                public List<BxlytMaterial> Materials = new List<BxlytMaterial>();

                public PaneInfo(BasePane pane, BasePane parent)
                {
                    Pane = pane;
                    Parent = parent;
                    if (parent == null)
                        throw new Exception("parent is null!");

                    //We need to add any materials that the material referenced
                    if (pane is IPicturePane)
                        Materials.Add(((IPicturePane)pane).Material);
                    else if (pane is ITextPane)
                        Materials.Add(((ITextPane)pane).Material);
                    else if (pane is IWindowPane)
                    {
                        var wnd = pane as IWindowPane;

                        Materials.Add(wnd.Content.Material);
                        foreach (var windowFrame in wnd.WindowFrames)
                            Materials.Add(windowFrame.Material);
                    }

                    Console.WriteLine($"Pane Undo {pane.Name} {pane} mats " + Materials.Count);
                }
            }
        }

        public class UndoActionTransform : IRevertAction
        {
            bool TransformChidlren = false;

            public List<PaneInfo> Panes = new List<PaneInfo>();

            public UndoActionTransform(List<BasePane> panes)
            {
                foreach (var pane in panes)
                {
                    TransformChidlren = Runtime.LayoutEditor.TransformChidlren;
                    Panes.Add(new PaneInfo()
                    {
                        targetPane = pane,
                        Translate = pane.Translate,
                        Scale = pane.Scale,
                        Rotate = pane.Rotate,
                        Width = pane.Width,
                        Height = pane.Height,
                    });
                }

            }

            public IRevertAction Revert()
            {
                foreach (var pane in Panes)
                {
                    if (!TransformChidlren)
                        pane.targetPane.KeepChildrenTransform(pane.Translate.X, pane.Translate.Y);

                    pane.targetPane.Translate = pane.Translate;
                    pane.targetPane.Scale = pane.Scale;
                    pane.targetPane.Rotate = pane.Rotate;
                    pane.targetPane.Width = pane.Width;
                    pane.targetPane.Height = pane.Height;
                }

                return this;
            }

            public class PaneInfo
            {
                public Vector3F Translate;
                public Vector3F Rotate;
                public Vector2F Scale;

                public float Width;
                public float Height;

                public BasePane targetPane;
            }
        }
    }
}

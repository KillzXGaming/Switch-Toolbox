using Gl_EditorFramework;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using Switch_Toolbox.Library.IO;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase;
using FirstPlugin.Turbo.CourseMuuntStructs;

namespace GL_EditorFramework.EditorDrawables
{
    public class RenderablePathPoint : EditableObject
    {
        public bool IsVisable = true;

        public bool CanConnect = true;

        public object NodeObject;

        public bool IsNormalTanTransform = false;

        protected Vector3 position = new Vector3(0, 0, 0);
        protected Vector3 scale = new Vector3(1, 1, 1);
        protected Vector3 rotate = new Vector3(0, 0, 0);

        protected Vector3 Normal = new Vector3(0, 0, 0);
        protected Vector3 Tangent = new Vector3(0, 0, 0);

        protected bool Selected = false;

        public override bool IsSelected() => Selected;

        public override bool IsSelected(int partIndex) => Selected;

        public Vector4 Color = new Vector4(0f, 0.25f, 1f, 1f);

        public RenderablePathPoint(Vector4 color, Vector3 pos, Vector3 rot, Vector3 sca, object nodeObject) {
            NodeObject = nodeObject;

            Color = color;
            UpdateTransform(pos, rot, sca);
        }

        public RenderablePathPoint(Vector3 pos, Vector3 normal, Vector3 tangent, Vector3 sca) {
            UpdateTransform(pos, normal, tangent, sca);
        }

        public void UpdateTransform(Vector3 pos, Vector3 rot, Vector3 sca)
        {
            position = pos;
            rotate = rot;
            scale = new Vector3(sca / 2);
        }

        public void UpdateTransform(Vector3 pos, Vector3 normal, Vector3 tangent, Vector3 scale)
        {
            IsNormalTanTransform = true;
            Normal = normal;
            Tangent = tangent;
            scale = new Vector3(scale / 2);
        }

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            if (pass == Pass.TRANSPARENT || !IsVisable)
                return;

            bool hovered = editorScene.hovered == this;

            if (IsNormalTanTransform)
            {
                control.UpdateModelMatrix(Matrix4.CreateScale(scale) *
                MatrixExenstion.CreateRotation(Normal, Tangent) *
                Matrix4.CreateTranslation(Selected ? editorScene.currentAction.newPos(Position) : Position));
            }
            else
            {
                control.UpdateModelMatrix(Matrix4.CreateScale(scale) *
                (Matrix4.CreateRotationX(rotate.X) *
                Matrix4.CreateRotationY(rotate.Y) *
                Matrix4.CreateRotationZ(rotate.Z)) *
                Matrix4.CreateTranslation(Selected ? editorScene.currentAction.newPos(Position) : Position));
            }

            Vector4 blockColor;
            Vector4 lineColor;

            if (hovered && Selected)
                lineColor = hoverColor;
            else if (hovered || Selected)
                lineColor = selectColor;
            else
                lineColor = Color;

            if (hovered && Selected)
                blockColor = Color * 0.5f + hoverColor * 0.5f;
            else if (hovered || Selected)
                blockColor = Color * 0.5f + selectColor * 0.5f;
            else
                blockColor = Color;

            Renderers.ColorBlockRenderer.Draw(control, pass, blockColor, lineColor, control.nextPickingColor());

        }

        public override void Draw(GL_ControlModern control, Pass pass)
        {
            if (pass == Pass.TRANSPARENT || !IsVisable)
                return;

            control.UpdateModelMatrix(Matrix4.CreateScale(0.5f) *
                Matrix4.CreateTranslation(position));

            Renderers.ColorBlockRenderer.Draw(control, pass, Color, Color, control.nextPickingColor());

        }

        public override void Draw(GL_ControlLegacy control, Pass pass, EditorSceneBase editorScene)
        {
            if (pass == Pass.TRANSPARENT || !IsVisable)
                return;

            bool hovered = editorScene.hovered == this;

            control.UpdateModelMatrix(Matrix4.CreateScale(0.5f) *
                Matrix4.CreateTranslation(Selected ? editorScene.currentAction.newPos(position) : position));

            Vector4 blockColor;
            Vector4 lineColor;

            if (hovered && Selected)
                lineColor = hoverColor;
            else if (hovered || Selected)
                lineColor = selectColor;
            else
                lineColor = Color;

            if (hovered && Selected)
                blockColor = Color * 0.5f + hoverColor * 0.5f;
            else if (hovered || Selected)
                blockColor = Color * 0.5f + selectColor * 0.5f;
            else
                blockColor = Color;

            Renderers.ColorBlockRenderer.Draw(control, pass, blockColor, lineColor, control.nextPickingColor());
        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            if (pass == Pass.TRANSPARENT || !IsVisable)
                return;

            control.UpdateModelMatrix(Matrix4.CreateScale(0.5f) *
                Matrix4.CreateTranslation(position));

            Renderers.ColorBlockRenderer.Draw(control, pass, Color, Color, control.nextPickingColor());

        }

        public override void Prepare(GL_ControlModern control)
        {
            Renderers.ColorBlockRenderer.Initialize();
        }

        public override void Prepare(GL_ControlLegacy control)
        {

        }

        public virtual void Translate(Vector3 lastPos, Vector3 translate, int subObj)
        {
            position = lastPos + translate;
            UpdateNodePosition();
        }

        public virtual void UpdatePosition(int subObj) {
        }

        public override bool CanStartDragging() => true;

        public override BoundingBox GetSelectionBox() => new BoundingBox(
            position.X - scale.X,
            position.X + scale.X,
            position.Y - scale.Y,
            position.Y + scale.Y,
            position.Z - scale.Z,
            position.Z + scale.Z
            );

        public override uint SelectAll(GL_ControlBase control)
        {
            Selected = true;
            return REDRAW;
        }

        public override uint SelectDefault(GL_ControlBase control)
        {
            Selected = true;
            return REDRAW;
        }

        public override uint Select(int partIndex, GL_ControlBase control)
        {
            Selected = true;
            return REDRAW;
        }

        public override uint Deselect(int partIndex, GL_ControlBase control)
        {
            Selected = false;
            return REDRAW;
        }

        public override uint DeselectAll(GL_ControlBase control)
        {
            Selected = false;
            return REDRAW;
        }

        public override void ApplyTransformActionToSelection(AbstractTransformAction transformAction)
        {
            position = transformAction.newPos(position);
            UpdateNodePosition();
        }

        private void UpdateNodePosition()
        {
            if (NodeObject is PathPoint)
            {
                ((PathPoint)NodeObject).Translate = position;
                ((PathPoint)NodeObject).Scale = scale;
                ((PathPoint)NodeObject).Rotate = rotate;

                if (((PathPoint)NodeObject).OnPathMoved != null)
                    ((PathPoint)NodeObject).OnPathMoved();
            }
        }

        public override Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
    }
}

using Gl_EditorFramework;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase;

namespace GL_EditorFramework.EditorDrawables
{
    //
    // Summary:
    //     An EditableObject that has only one selectable Part. It's represented by a blue block
    public class RenderablePathPoint : SingleObject
    {
        public RenderablePathPoint(Vector3 pos)
            : base(pos)
        {

        }
        protected Vector3 ScaleVec3 = new Vector3(0, 0, 0);
        protected Vector3 RotationVec3 = new Vector3(0, 0, 0);
        protected Vector3 Normal = new Vector3(0, 0, 0);
        protected Vector3 Tangent = new Vector3(0, 0, 0);

        bool IsNormalTanTransform = false;

        protected bool Selected = false;

        public override bool IsSelected() => Selected;

        public override bool IsSelected(int partIndex) => Selected;

        protected static Vector4 Color = new Vector4(0f, 0.25f, 1f, 1f);

        Matrix4 Transform;

        public RenderablePathPoint(Vector3 pos, Vector3 rot, Vector3 scale) : base(pos)
        {
            UpdateTransform(pos, rot, scale);
        }

        public RenderablePathPoint(Vector3 pos, Vector3 normal, Vector3 tangent, Vector3 scale) : base(pos)
        {
            UpdateTransform(pos, normal, tangent, scale);
        }

        public void UpdateTransform(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            Position = pos;
            RotationVec3 = rot;
            ScaleVec3 = new Vector3(scale / 2);
        }

        public void UpdateTransform(Vector3 pos, Vector3 normal, Vector3 tangent, Vector3 scale)
        {
            IsNormalTanTransform = true;
            Normal = normal;
            Tangent = tangent;
            ScaleVec3 = new Vector3(scale / 2);
        }


        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            if (pass == Pass.TRANSPARENT)
                return;

            bool hovered = editorScene.hovered == this;

            if (IsNormalTanTransform)
            {
                control.UpdateModelMatrix(Matrix4.CreateScale(ScaleVec3) *
                MatrixExenstion.CreateRotation(Normal, Tangent) *
                Matrix4.CreateTranslation(Selected ? editorScene.currentAction.newPos(Position) : Position));
            }
            else
            {
                control.UpdateModelMatrix(Matrix4.CreateScale(ScaleVec3) *
                (Matrix4.CreateRotationX(RotationVec3.X) *
                Matrix4.CreateRotationY(RotationVec3.Y) *
                Matrix4.CreateRotationZ(RotationVec3.Z)) *
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
            if (pass == Pass.TRANSPARENT)
                return;

            control.UpdateModelMatrix(Matrix4.CreateScale(0.5f) *
                Matrix4.CreateTranslation(position));

            Renderers.ColorBlockRenderer.Draw(control, pass, Color, Color, control.nextPickingColor());

        }

        public override void Draw(GL_ControlLegacy control, Pass pass, EditorSceneBase editorScene)
        {
            if (pass == Pass.TRANSPARENT)
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
            if (pass == Pass.TRANSPARENT)
                return;

            control.UpdateModelMatrix(Matrix4.CreateScale(0.5f) *
                Matrix4.CreateTranslation(position));

            Renderers.ColorBlockRenderer.Draw(control, pass, Color, Color, control.nextPickingColor());

        }

        public override void ApplyTransformActionToSelection(AbstractTransformAction transformAction)
        {
            Position = transformAction.newPos(Position);
            Scale = transformAction.newScale(Scale);
        }
    }
}

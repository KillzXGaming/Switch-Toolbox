using System;
using System.Collections.Generic;
using System.ComponentModel;
using OpenTK;
using GL_EditorFramework.EditorDrawables;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class ReturnPoint : BasePathPoint, IObject
    {
        public const string N_Normal = "Normal";
        public const string N_Position = "Position";
        public const string N_Tangent = "Tangent";
        public const string N_JugemIndex = "JugemIndex";
        public const string N_JugemPath = "JugemPath";
        public const string N_ReturnType = "ReturnType";
        public const string N_HasError = "hasError";

        public ReturnPoint(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");
        }

        private RenderablePathPoint renderablePoint;

        [Browsable(false)]
        public override RenderablePathPoint RenderablePoint
        {
            get
            {
                if (renderablePoint == null)
                    renderablePoint = new RenderablePathPoint(new Vector4(1f, 0.0f, 1f, 0.1f), Translate, Rotate, Scale, this);
                return renderablePoint;
            }
        }

        public int JugemIndex
        {
            get { return this[N_JugemIndex]; }
            set { this[N_JugemIndex] = value; }
        }

        public int JugemPath
        {
            get { return this[N_JugemPath]; }
            set { this[N_JugemPath] = value; }
        }

        public int ReturnType
        {
            get { return this[N_ReturnType]; }
            set { this[N_ReturnType] = value; }
        }

        public int HasError
        {
            get { return this[N_HasError]; }
            set { this[N_HasError] = value; }
        }

        public Vector3 Normal
        {
            get { return new Vector3(this[N_Normal]["X"], this[N_Normal]["Y"], this[N_Normal]["Z"]); ; }
            set
            {
                this[N_Normal]["X"] = value.X;
                this[N_Normal]["Y"] = value.Y;
                this[N_Normal]["Z"] = value.Z;
            }
        }

        public Vector3 Position
        {
            get { return new Vector3(this[N_Position]["X"], this[N_Position]["Y"], this[N_Position]["Z"]); ; }
            set
            {
                this[N_Position]["X"] = value.X;
                this[N_Position]["Y"] = value.Y;
                this[N_Position]["Z"] = value.Z;
            }
        }

        public Vector3 Tangent
        {
            get { return new Vector3(this[N_Tangent]["X"], this[N_Tangent]["Y"], this[N_Tangent]["Z"]); ; }
            set
            {
                this[N_Tangent]["X"] = value.X;
                this[N_Tangent]["Y"] = value.Y;
                this[N_Tangent]["Z"] = value.Z;
            }
        }
    }

}

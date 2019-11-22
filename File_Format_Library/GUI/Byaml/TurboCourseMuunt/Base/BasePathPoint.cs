using System;
using System.Collections.Generic;
using System.ComponentModel;
using OpenTK;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.Interfaces;
using FirstPlugin.MuuntEditor;
using Toolbox.Library;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class BasePathPoint : PropertyObject, IObject, IPickable2DObject, I3DDrawableContainer
    {
        public Action OnPathMoved;

        [Browsable(false)]
        public virtual RenderablePathPoint RenderablePoint
        {
            get
            {
                var point = new RenderablePathPoint(new Vector4(0f, 0.25f, 1f, 1f), Translate, Rotate, Scale, this);
                return point;
            }
        }

        public List<AbstractGlDrawable> Drawables
        {
            get
            {
                return new List<AbstractGlDrawable>() {
                    new TransformableObject(Translate, Quaternion.Identity, new Vector3(1))
                };
            }
            set
            {

            }
        }

        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }
        
        public virtual bool IsHit(float X, float Y)
        {
            return new STRectangle(Translate.X, Translate.Z, 40, 40).IsHit((int)X, (int)Y);

            for (int i = 0; i <= 300; i++)
            {
                double angle = 2 * Math.PI * i / 300;
                double x = Math.Cos(angle);
                double y = Math.Sin(angle);

                if (angle < 90)
                {

                }
                if (angle < 90 && angle < 180)
                {

                }
                if (angle < 270)
                {

                }
                if (angle < 360)
                {

                }
            }

            return false;
        }

        public void PickTranslate(float X, float Y, float Z)
        {
            Translate = new Vector3(Translate.X - X, Translate.Y - Z, Translate.Z - Y);
        }

        public void PickRotate(float X, float Y, float Z)
        {
            Rotate = new Vector3(Rotate.X - X, Rotate.Y - Z, Rotate.Z - Y);
        }

        public void PickScale(float X, float Y, float Z)
        {
            Scale = new Vector3(Scale.X - X, Scale.Y - Z, Scale.Z - Y);
        }

        public const string N_Translate = "Translate";
        public const string N_Rotate = "Rotate";
        public const string N_Scale = "Scale";
        public const string N_Id = "UnitIdNum";
        public const string N_ObjectID = "ObjId";

        public dynamic this[string name]
        {
            get
            {
                if (Prop.ContainsKey(name)) return Prop[name];
                else return null;
            }
            set
            {
                if (Prop.ContainsKey(name)) Prop[name] = value;
                else Prop.Add(name, value);
            }
        }

        public List<object> Properties = new List<object>();

        public List<PointID> NextPoints = new List<PointID>();
        public List<PointID> PrevPoints = new List<PointID>();

        public List<BaseControlPoint> ControlPoints = new List<BaseControlPoint>();

        [Category("Transform")]
        public Vector3 Rotate
        {
            get {
                if (this[N_Rotate] == null)
                    return new Vector3(0, 0, 0);

                return new Vector3(
               this[N_Rotate]["X"] != null ? this[N_Rotate]["X"] : 0,
               this[N_Rotate]["Y"] != null ? this[N_Rotate]["Y"] : 0,
               this[N_Rotate]["Z"] != null ? this[N_Rotate]["Z"] : 0);
            }
            set
            {
                this[N_Rotate]["X"] = value.X;
                this[N_Rotate]["Y"] = value.Y;
                this[N_Rotate]["Z"] = value.Z;
            }
        }

        [Category("Transform")]
        public Vector3 Scale
        {
            get
            {
                if (this[N_Scale] == null)
                    return new Vector3(1, 1, 1);

                return new Vector3(
               this[N_Scale]["X"] != null ? this[N_Scale]["X"] : 1,
               this[N_Scale]["Y"] != null ? this[N_Scale]["Y"] : 1,
               this[N_Scale]["Z"] != null ? this[N_Scale]["Z"] : 1);
            }
            set
            {
                if (this[N_Scale] != null)
                {
                    this[N_Scale]["X"] = value.X;
                    this[N_Scale]["Y"] = value.Y;
                    this[N_Scale]["Z"] = value.Z;
                }
            }
        }

        [Category("Transform")]
        public Vector3 Translate
        {
            get
            {
                if (this[N_Translate] == null)
                    return new Vector3(0, 0, 0);

                return new Vector3(
                 this[N_Translate]["X"] != null ? this[N_Translate]["X"] : 0,
                 this[N_Translate]["Y"] != null ? this[N_Translate]["Y"] : 0,
                 this[N_Translate]["Z"] != null ? this[N_Translate]["Z"] : 0);
            }
            set
            {
                this[N_Translate]["X"] = value.X;
                this[N_Translate]["Y"] = value.Y;
                this[N_Translate]["Z"] = value.Z;
            }
        }
    }

}

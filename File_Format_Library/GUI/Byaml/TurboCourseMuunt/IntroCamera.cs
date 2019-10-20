using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class IntroCamera : BasePathPoint
    {
        public const string N_UnitIdNum = "UnitIdNum";

        private RenderablePathPoint renderablePoint;

        [Browsable(false)]
        public override RenderablePathPoint RenderablePoint
        {
            get
            {
                if (renderablePoint == null)
                    renderablePoint = new RenderablePathPoint(new Vector4(1f, 0f, 0f, 1f), Translate, Rotate, new OpenTK.Vector3(30), this);
                return renderablePoint;
            }
        }

        public int UnitIdNum
        {
            get { return this[N_UnitIdNum] != null ? this[N_UnitIdNum] : -1; }
            set { this[N_UnitIdNum] = value; }
        }

        [Category("Properties")]
        public int CameraNum
        {
            get { return this["CameraNum"] != null ? this["CameraNum"] : -1; }
            set { if (value != -1) this["CameraNum"] = value; }
        }

        [Category("Properties")]
        public int CameraTime
        {
            get { return this["CameraTime"] != null ? this["CameraTime"] : -1; }
            set { if (value != -1) this["CameraTime"] = value; }
        }

        [Category("Properties")]
        public int CameraType
        {
            get { return this["CameraType"] != null ? this["CameraType"] : -1; }
            set { if (value != -1) this["CameraType"] = value; }
        }

        [Category("Properties")]
        public int Camera_AtPath
        {
            get { return this["Camera_AtPath"] != null ? this["Camera_AtPath"] : -1; }
            set { if (value != -1) this["Camera_AtPath"] = value; }
        }

        [Category("Properties")]
        public int Camera_Path
        {
            get { return this["Camera_Path"] != null ? this["Camera_Path"] : -1; }
            set { if (value != -1) this["Camera_Path"] = value; }
        }

        [Category("Properties")]
        public int Fovy
        {
            get { return this["Fovy"] != null ? this["Fovy"] : -1; }
            set { if (value != -1) this["Fovy"] = value; }
        }

        [Category("Properties")]
        public int FovySpeed
        {
            get { return this["FovySpeed"] != null ? this["FovySpeed"] : -1; }
            set { if (value != -1) this["FovySpeed"] = value; }
        }


        public IntroCamera(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");
        }
    }
}

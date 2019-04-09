using System;
using System.Collections.Generic;
using OpenTK;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework;

namespace Switch_Toolbox.Library.Rendering
{
    public class ProbeLighting : AbstractGlDrawable
    {
        public List<Entry> Entries = new List<Entry>();

        public class Entry
        {
            public string Name { get { return $"b_{Index}"; } }

            public uint Index = 0;
            public uint Type = 0;

            public Grid Grid = new Grid();

            //Index Buffer
            public uint IndexType = 1;
            public uint UsedIndexNum;
            public uint MaxIndexNum;
            public uint[] IndexBuffer = new uint[0];

            //Data Buffer
            public uint DataType = 0;
            public uint UsedShDataNum;
            public uint MaxShDataNum;
            public uint PerProbeNum = 27;
            public float[] DataBuffer = new float[0];
        }

        public class Grid
        {
            public Vector3 AABB_Max_Position;
            public Vector3 AABB_Min_Position;
            public Vector3 Voxel_Step_Position;
        }

        public override void Draw(GL_ControlModern control, Pass pass)
        {

        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {

        }

        public override void Prepare(GL_ControlModern control)
        {

        }

        public override void Prepare(GL_ControlLegacy control)
        {

        }
    }
}

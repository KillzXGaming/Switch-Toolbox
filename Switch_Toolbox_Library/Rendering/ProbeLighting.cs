using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
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
            public bool IsVisable = true;

            public string Name { get { return $"b_{Index}"; } }

            public uint Index = 0;
            public uint Type = 0;

            public Grid Grid = new Grid();

            //Index Buffer
            public uint IndexType = 1;
            public uint UsedIndexNum;
            public uint MaxIndexNum;
            public uint[] IndexBuffer = new uint[0];


            public ushort[] GetUint16BufferIndices()
            {
                if (!IsUint16Buffer())
                    throw new Exception("Buffer not a ushort buffer!");

                List<ushort> Indices = new List<ushort>();

                //Read the buffer data back as ushort
                for (int i = 0; i < IndexBuffer.Length; i++)
                {
                    byte[] bytes = BitConverter.GetBytes(IndexBuffer[i]);
                    Indices.Add(BitConverter.ToUInt16(bytes, 0));
                    Indices.Add(BitConverter.ToUInt16(bytes, 2));
                }

                return Indices.ToArray();
            }

            public bool IsUint16Buffer()
            {
                if (IndexBuffer.Length / 2 == UsedIndexNum)
                    return true;
                else return false;
            }   

            public bool IsUint32Buffer()
            {
                if (IndexBuffer.Length == UsedIndexNum)
                    return true;
                else return false;
            }

            //Data Buffer
            public uint DataType = 0;
            public uint UsedShDataNum;
            public uint MaxShDataNum;
            public uint PerProbeNum = 27;
            public float[] DataBuffer = new float[0];

            public bool IsHalfFloatBuffer()
            {
                if ((DataBuffer.Length * PerProbeNum) / 2 == UsedShDataNum)
                    return true;
                else return false;
            }

            public bool IsFloatBuffer()
            {
                if ((DataBuffer.Length * PerProbeNum) == UsedShDataNum)
                    return true;
                else return false;
            }

            //Color data in the data buffer
            public class ShereHermonic
            {
                //Usually 27
                public Vector3[] Coefficents;

                public ShereHermonic(int PerProbeNum) {
                    Coefficents = new Vector3[PerProbeNum];
                }
            }
        }

        public class Grid
        {
            public Vector3 GridColor;

            public Vector3 AABB_Max_Position;
            public Vector3 AABB_Min_Position;
            public Vector3 Voxel_Step_Position;
        }

        protected static ShaderProgram ProbeSHShaderProgram;

        int vbo_position;

        public void Destroy()
        {
            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
                return;

            GL.DeleteBuffer(vbo_position);
        }

        public List<Vector3> GetProbeVertices(int index)
        {
            var entry = Entries[index];

            var vertices = new List<Vector3>();

            Vector3 Max = entry.Grid.AABB_Max_Position;
            Vector3 Min = entry.Grid.AABB_Min_Position;
            Vector3 Step = entry.Grid.AABB_Min_Position;

            float GridWidth = Min.X + Max.X;
            float GirdHeight = Min.Y + Max.Y;

        /*    //Draw Z plane
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GirdHeight; y++)
                {
                    Vector3 position = new Vector3(Min.X + (x / Step.X) * (Max.X - Min.X),
                                                   Min.Y + (y / Step.Y) * (Max.Y - Min.Y),
                                                   Max.Z);

                    vertices.Add(position);
                }
            }*/

            return vertices;
        }

        public List<Vector3> GetBoundingVertices(int index)
        {
            var entry = Entries[index];

            var vertices = new List<Vector3>();

            Vector3 Max = entry.Grid.AABB_Max_Position;
            Vector3 Min = entry.Grid.AABB_Min_Position;

            vertices.Add(new Vector3(Min.X, Min.Y, Min.Z));
            vertices.Add(new Vector3(Min.X, Min.Y, Max.Z));
            vertices.Add(new Vector3(Min.X, Max.Y, Min.Z));
            vertices.Add(new Vector3(Min.X, Max.Y, Max.Z));
            vertices.Add(new Vector3(Max.X, Min.Y, Min.Z));
            vertices.Add(new Vector3(Max.X, Min.Y, Max.Z));
            vertices.Add(new Vector3(Max.X, Max.Y, Min.Z));
            vertices.Add(new Vector3(Max.X, Max.Y, Max.Z));

            return vertices;
        }

        public static int CellAmount;
        public static int CellSize;

        int CurrentIndex = 0;

        Vector3[] Vertices
        {
            get
            {
                return GetProbeVertices(CurrentIndex).ToArray();
            }
        }

        public void UpdateVertexData()
        {
            Vector3[] vertices = Vertices;

            GL.GenBuffers(1, out vbo_position);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(vertices.Length * Vector3.SizeInBytes),
                                   vertices, BufferUsageHint.StaticDraw);
        }

        public override void Draw(GL_ControlModern control, Pass pass)
        {
            if (pass == Pass.TRANSPARENT)
                return;

            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
                UpdateVertexData();

            if (!Runtime.OpenTKInitialized)
                return;

            control.CurrentShader = ProbeSHShaderProgram;
            control.UpdateModelMatrix(Matrix4.Identity);

            Matrix4 previewScale = Utils.TransformValues(Vector3.Zero, Vector3.Zero, Runtime.previewScale);

            ProbeSHShaderProgram.SetMatrix4x4("previewScale", ref previewScale);

            foreach (var entry in Entries)
            {
                if (!entry.IsVisable)
                    continue;

                Vector3[] vertices = GetBoundingVertices((int)entry.Index).ToArray();

                Vector3 Max = entry.Grid.AABB_Max_Position;
                Vector3 Min = entry.Grid.AABB_Min_Position;
                Vector3 Step = entry.Grid.Voxel_Step_Position;

                float gridHeight = Min.Y + Max.Y;
                float gridWidth = Min.X + Max.X;
                float gridDepth = Min.Z + Max.Z;


                //Draw bounding box
                GL.Color3(entry.Grid.GridColor);

                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex3(vertices[0]);
                GL.Vertex3(vertices[1]);
                GL.Vertex3(vertices[3]);
                GL.Vertex3(vertices[2]);
                GL.End();

                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex3(vertices[4]);
                GL.Vertex3(vertices[5]);
                GL.Vertex3(vertices[7]);
                GL.Vertex3(vertices[6]);
                GL.End();

                GL.Begin(PrimitiveType.Lines);
                GL.Vertex3(vertices[0]);
                GL.Vertex3(vertices[4]);
                GL.Vertex3(vertices[1]);
                GL.Vertex3(vertices[5]);
                GL.Vertex3(vertices[3]);
                GL.Vertex3(vertices[7]);
                GL.Vertex3(vertices[2]);
                GL.Vertex3(vertices[6]);
                GL.End();

                ProbeSHShaderProgram.EnableVertexAttributes();
                Draw(ProbeSHShaderProgram);
                ProbeSHShaderProgram.DisableVertexAttributes();
            }

            GL.UseProgram(0);
        }

        private void Attributes(ShaderProgram shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, 12, 0);
        }
        private void Uniforms(ShaderProgram shader)
        {
            shader.SetVector3("gridColor", ColorUtility.ToVector3(System.Drawing.Color.Black));
        }
        private void Draw(ShaderProgram shader)
        {
            Uniforms(shader);
            Attributes(shader);

            GL.DrawArrays(PrimitiveType.Points, 0, Vertices.Length);
        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {

        }

        public override void Prepare(GL_ControlModern control)
        {
            var Frag = new FragmentShader(
             @"#version 330
				uniform vec3 gridColor;
				out vec4 FragColor;

				void main(){
					FragColor = vec4(gridColor, 1);
				}");
            var Vert = new VertexShader(
              @"#version 330
				in vec3 vPosition;

	            uniform mat4 mtxMdl;
				uniform mat4 mtxCam;

				void main(){
					gl_Position = mtxMdl * mtxCam * vec4(vPosition.xyz, 1);
				}");

            ProbeSHShaderProgram = new ShaderProgram(Frag, Vert, control);
        }

        public override void Prepare(GL_ControlLegacy control)
        {

        }
    }
}

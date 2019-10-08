using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;

namespace FirstPlugin
{
    /// <summary>
    /// Collision from the KCL library turned into a mono script
    /// </summary>
    public class KclMonoscript
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<Prisim> Prisims = new List<Prisim>();

        public List<uint> Faces = new List<uint>();

        public class Prisim
        {
            public float Length;
            public ushort VertexIndex;
            public ushort DirectionIndex;
            public ushort NormalAIndex;
            public ushort NormalBIndex;
            public ushort NormalCIndex;
            public ushort CollisionType;
            public uint TriangleIndex;
        }

        public class Triangle
        {
            public Vector3 PointA;
            public Vector3 PointB;
            public Vector3 PointC;

            public Triangle(Vector3 A, Vector3 B, Vector3 C)
            {
                PointA = A;
                PointB = B;
                PointC = C;
            }

            public Vector3 Normal
            {
                get
                {
                    return Vector3.Cross(PointB - PointA, PointC - PointA).Normalized();
                }
            }
        }

        public Triangle GetTriangle(Prisim Plane)
        {
            Vector3 A = Vertices[Plane.VertexIndex];
            Vector3 CrossA = Vector3.Cross(Normals[Plane.NormalAIndex], Normals[Plane.DirectionIndex]);
            Vector3 CrossB = Vector3.Cross(Normals[Plane.NormalBIndex], Normals[Plane.DirectionIndex]);
            Vector3 B = A + CrossB * (Plane.Length / Vector3.Dot(CrossB, Normals[Plane.NormalCIndex]));
            Vector3 C = A + CrossA * (Plane.Length / Vector3.Dot(CrossA, Normals[Plane.NormalCIndex]));
            return new Triangle(A, B, C);
        }

        public void ReadKCL(string fileName)
        {
            using (var reader = new StreamReader(File.OpenRead(fileName)))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line.Contains("mPosData"))
                    {
                        reader.ReadLine();//Array
                        uint numVertex = GetValueUint(reader.ReadLine());

                        for (int i = 0; i < numVertex; i++)
                        {
                            reader.ReadLine();//Index value
                            reader.ReadLine();//Vector
                            float X = GetValue32(reader.ReadLine()); //X
                            float Y = GetValue32(reader.ReadLine()); //Y
                            float Z = GetValue32(reader.ReadLine()); //Z
                            Vertices.Add(new Vector3(X,Y,Z));
                        }
                    }
                    if (line.Contains("mNrmData"))
                    {
                        reader.ReadLine();//Array
                        uint numVertex = GetValueUint(reader.ReadLine());

                        for (int i = 0; i < numVertex; i++)
                        {
                            reader.ReadLine();//Index value
                            reader.ReadLine();//Vector
                            float X = GetValue32(reader.ReadLine()); //X
                            float Y = GetValue32(reader.ReadLine()); //Y
                            float Z = GetValue32(reader.ReadLine()); //Z
                            Normals.Add(new Vector3(X, Y, Z));
                        }
                    }
                    if (line.Contains("mPrismData"))
                    {
                        reader.ReadLine();//Array
                        uint numPrisims = GetValueUint(reader.ReadLine());
                        uint faceIndex = 0;
                        for (int i = 0; i < numPrisims; i++)
                        {
                            reader.ReadLine();//Index value
                            reader.ReadLine();//data

                            Prisim prisim = new Prisim();
                            prisim.Length = GetValue32(reader.ReadLine());
                            prisim.VertexIndex = GetValueUint16(reader.ReadLine());
                            prisim.DirectionIndex = GetValueUint16(reader.ReadLine());
                            prisim.NormalAIndex = GetValueUint16(reader.ReadLine());
                            prisim.NormalBIndex = GetValueUint16(reader.ReadLine());
                            prisim.NormalCIndex = GetValueUint16(reader.ReadLine());
                            prisim.CollisionType = GetValueUint16(reader.ReadLine());
                            prisim.TriangleIndex = GetValueUint(reader.ReadLine());
                            Prisims.Add(prisim);

                            Faces.Add(faceIndex);
                            Faces.Add(faceIndex + 1);
                            Faces.Add(faceIndex + 2);

                            faceIndex += 3;
                        }
                    }
                }
            }
        }

        private ushort GetValueUint16(string line)
        {
            string value = line.Split('=').Last();
            ushort valueU = 0;
            ushort.TryParse(value, out valueU);
            return valueU;
        }

        private uint GetValueUint(string line)
        {
            string value = line.Split('=').Last();
            uint valueU = 0;
            uint.TryParse(value, out valueU);
            return valueU;
        }

        private float GetValue32(string line)
        {
            string value = line.Split('=').Last();
            float valueF = 0;
            float.TryParse(value, out valueF);
            return valueF;
        }

        private string GetValue(string line)
        {
            return line.Split('=').Last();
        }
    }
}

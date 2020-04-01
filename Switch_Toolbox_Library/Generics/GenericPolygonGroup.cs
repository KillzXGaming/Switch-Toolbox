using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public enum STPrimitiveType
    {
        Points = 0,
        Lines = 1,
        LineStrips = 2,
        Triangles = 3,
        TrangleStrips,
        Quads,
    }

    public class STGenericPolygonGroup
    {
        public virtual STGenericMaterial Material { get; set; } = null;

        public int Offset { get; set; }
        public int MaterialIndex { get; set; } = -1;
        public int Index { get; set; }

        public List<int> faces = new List<int>();

        public STPrimitiveType PrimativeType = STPrimitiveType.Triangles;

        public int strip = 0x40;
        public int displayFaceSize = 0;

        public List<int> GetDisplayFace()
        {
            if ((strip >> 4) == 4)
            {
                displayFaceSize = faces.Count;
                return faces;
            }
            else
            {
                List<int> f = new List<int>();

                int startDirection = 1;
                int p = 0;
                int f1 = faces[p++];
                int f2 = faces[p++];
                int faceDirection = startDirection;
                int f3;
                do
                {
                    f3 = faces[p++];
                    if (f3 == 0xFFFF)
                    {
                        f1 = faces[p++];
                        f2 = faces[p++];
                        faceDirection = startDirection;
                    }
                    else
                    {
                        faceDirection *= -1;
                        if ((f1 != f2) && (f2 != f3) && (f3 != f1))
                        {
                            if (faceDirection > 0)
                            {
                                f.Add(f3);
                                f.Add(f2);
                                f.Add(f1);
                            }
                            else
                            {
                                f.Add(f2);
                                f.Add(f3);
                                f.Add(f1);
                            }
                        }
                        f1 = f2;
                        f2 = f3;
                    }
                } while (p < faces.Count);

                displayFaceSize = f.Count;
                return f;
            }
        }
    }

}

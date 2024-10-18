using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;
using System.Windows.Forms;


namespace Toolbox.Library.Rendering
{
    public enum STIndexFormat : uint
    {
        UnsignedByte = 0,
        UInt16 = 1,
        UInt32 = 2,
    }

    public class Vertex
    {   
        public Vector3 pos = new Vector3(0);
        public Vector3 nrm = new Vector3(0);
        public Vector4 col = new Vector4(1);
        public Vector4 col2 = new Vector4(1);
        public Vector4 col3 = new Vector4(1);
        public Vector4 col4 = new Vector4(1);

        public Vector2 uv0 = new Vector2(0);
        public Vector2 uv1 = new Vector2(0);
        public Vector2 uv2 = new Vector2(0);
        public Vector2 uv3 = new Vector2(0);

        public Vector4 tan = new Vector4(0);
        public Vector4 bitan = new Vector4(0);

        public List<int> boneIds = new List<int>();
        public List<float> boneWeights = new List<float>();

        public float normalW = 1;

        public List<string> boneNames = new List<string>();

        public List<Bone> boneList = new List<Bone>();
        public class Bone
        {
            public string Name;
            public int Index;
            public bool HasWeights;
            public List<BoneWeight> weights = new List<BoneWeight>();
        }
        public class BoneWeight
        {
            public float weight;
        }
        //For vertex morphing 
        public Vector3 pos1 = new Vector3();
        public Vector3 pos2 = new Vector3();

        public List<Vector4> Unknowns = new List<Vector4>();
    }
}

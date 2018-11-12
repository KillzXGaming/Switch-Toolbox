using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;

namespace Switch_Toolbox.Library
{
    public class AssimpHelper
    {
        public static void ToNumerics(Matrix4x4 matIn, out System.Numerics.Matrix4x4 matOut)
        {
            //Assimp matrices are column vector, so X,Y,Z axes are columns 1-3 and 4th column is translation.
            //Columns => Rows to make it compatible with numerics
            matOut = new System.Numerics.Matrix4x4(matIn.A1, matIn.B1, matIn.C1, matIn.D1, //X
                                                   matIn.A2, matIn.B2, matIn.C2, matIn.D2, //Y
                                                   matIn.A3, matIn.B3, matIn.C3, matIn.D3, //Z
                                                   matIn.A4, matIn.B4, matIn.C4, matIn.D4); //Translation
        }

        public static void FromNumerics(System.Numerics.Matrix4x4 matIn, out Matrix4x4 matOut)
        {
            //Numerics matrix are row vector, so X,Y,Z axes are rows 1-3 and 4th row is translation.
            //Rows => Columns to make it compatible with assimp

            //X
            matOut.A1 = matIn.M11;
            matOut.B1 = matIn.M12;
            matOut.C1 = matIn.M13;
            matOut.D1 = matIn.M14;

            //Y
            matOut.A2 = matIn.M21;
            matOut.B2 = matIn.M22;
            matOut.C2 = matIn.M23;
            matOut.D2 = matIn.M24;

            //Z
            matOut.A3 = matIn.M31;
            matOut.B3 = matIn.M32;
            matOut.C3 = matIn.M33;
            matOut.D3 = matIn.M34;

            //Translation
            matOut.A4 = matIn.M41;
            matOut.B4 = matIn.M42;
            matOut.C4 = matIn.M43;
            matOut.D4 = matIn.M44;
        }
    }
}

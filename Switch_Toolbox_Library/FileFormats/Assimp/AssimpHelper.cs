using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK;
using SN = System.Numerics;

namespace Toolbox.Library
{
    public static class AssimpHelper
    {
        public static Matrix4x4 GetBoneMatrix(STBone bone)
        {
            var pos = Matrix4x4.FromTranslation(new Vector3D(bone.position[0], bone.position[1], bone.position[2]));
            var rotx = Matrix4x4.FromRotationX(bone.rotation[0]);
            var roty = Matrix4x4.FromRotationY(bone.rotation[1]);
            var rotz = Matrix4x4.FromRotationZ(bone.rotation[2]);
            var sca = Matrix4x4.FromScaling(new Vector3D(bone.scale[0], bone.scale[1], bone.scale[2]));

            return sca * (rotx * roty * rotz) * pos;
        }

        public static string GetSaveFilter()
        {
            return "Supported Formats|*.dae;*.stl;*.obj; *.ply; *.x;*.3ds;*.json;|" +
             "DAE |*.dae|" +
             "OBJ |*.obj|" +
             "STL |*.stl|" +
             "PLY |*.ply|" +
             "X |*.x|" +
             "3DS |*.3ds|" +
             "JSON WebGL |*.json|" +
             "All files(*.*)|*.*";
        }

        public static Syroot.Maths.Matrix3x4 CalculateInverseMatrix(STBone bone)
        {
            return FromAssimpMatrix(AssimpCalculateInverseMatrix(bone));
        }

        public static Syroot.Maths.Matrix3x4 FromAssimpMatrix(Assimp.Matrix4x4 mat)
        {

            var mat4 = new Syroot.Maths.Matrix3x4();
            mat4.M11 = mat.A1;
            mat4.M12 = mat.A2;
            mat4.M13 = mat.A3;
            mat4.M14 = mat.A4;
            mat4.M21 = mat.B1;
            mat4.M22 = mat.B2;
            mat4.M23 = mat.B3;
            mat4.M24 = mat.B4;
            mat4.M31 = mat.C1;
            mat4.M32 = mat.C2;
            mat4.M33 = mat.C3;
            mat4.M34 = mat.C4;
          /*  mat4.M41 = mat.D1;
            mat4.M42 = mat.D2;
            mat4.M43 = mat.D3;
            mat4.M44 = mat.D4;*/

            return mat4;
        }

        public static Assimp.Matrix4x4 AssimpCalculateInverseMatrix(STBone bone)
        {
            Assimp.Matrix4x4 transf;

            //Get parent transform for a smooth matrix
            if (bone.Parent != null && bone.Parent is STBone)
                transf = AssimpCalculateInverseMatrix((STBone)bone.Parent);
            else
                transf = Assimp.Matrix4x4.Identity;

            //Now calculate the matrix with TK matrices
            var trans = Assimp.Matrix4x4.FromTranslation(new Vector3D(bone.position[0], bone.position[1], bone.position[2]));
            var scale = Assimp.Matrix4x4.FromScaling(new Vector3D(bone.scale[0], bone.scale[1], bone.scale[2]));
            var rotX = Assimp.Matrix4x4.FromRotationX(bone.rotation[0]);
            var rotY = Assimp.Matrix4x4.FromRotationY(bone.rotation[1]);
            var rotZ = Assimp.Matrix4x4.FromRotationZ(bone.rotation[2]);

            var result = scale * (rotX * rotY * rotZ) * trans;
            result.Inverse();

            return transf;

        }

        public static Vector3 FromVector(Vector3D vec)
        {
            Vector3 v;
            v.X = vec.X;
            v.Y = vec.Y;
            v.Z = vec.Z;
            return v;
        }

        public static Matrix4x4 ToMatrix4x4(this OpenTK.Matrix4 mat4)
        {
            Matrix4x4 outMat = new Matrix4x4(
                mat4.M11, mat4.M12, mat4.M13, mat4.M14,
                mat4.M21, mat4.M22, mat4.M23, mat4.M24,
                mat4.M31, mat4.M32, mat4.M33, mat4.M34,
                mat4.M41, mat4.M42, mat4.M43, mat4.M44);

            outMat.Transpose();
            return outMat;
        }

        public static OpenTK.Matrix4 TKMatrix(Assimp.Matrix4x4 input)
        {
            return new OpenTK.Matrix4(input.A1, input.B1, input.C1, input.D1,
                                       input.A2, input.B2, input.C2, input.D2,
                                       input.A3, input.B3, input.C3, input.D3,
                                       input.A4, input.B4, input.C4, input.D4);
        }
        public static Vector3 ToEular(OpenTK.Quaternion q)
        {
            Matrix4 mat = Matrix4.CreateFromQuaternion(q);
            float x, y, z;
            y = (float)Math.Asin(Clamp(mat.M13, -1, 1));

            if (Math.Abs(mat.M13) < 0.99999)
            {
                x = (float)Math.Atan2(-mat.M23, mat.M33);
                z = (float)Math.Atan2(-mat.M12, mat.M11);
            }
            else
            {
                x = (float)Math.Atan2(mat.M32, mat.M22);
                z = 0;
            }
            return new Vector3(x, y, z) * -1;
        }
        private static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
        public static OpenTK.Quaternion TKQuaternion(Assimp.Quaternion rot)
        {
            OpenTK.Quaternion quat = new OpenTK.Quaternion();
            quat.X = rot.X;
            quat.Y = rot.Y;
            quat.Z = rot.Z;
            quat.W = rot.W;
            return quat;
        }

        public static Matrix4x4 AssimpFromTKMatrix(Matrix4 tkMatrix)
        {
            Matrix4x4 m = new Matrix4x4();
            m.A1 = tkMatrix.M11;
            m.A2 = tkMatrix.M12;
            m.A3 = tkMatrix.M13;
            m.A4 = tkMatrix.M14;

            m.B1 = tkMatrix.M21;
            m.B2 = tkMatrix.M22;
            m.B3 = tkMatrix.M23;
            m.B4 = tkMatrix.M24;

            m.C1 = tkMatrix.M31;
            m.C2 = tkMatrix.M32;
            m.C3 = tkMatrix.M33;
            m.C4 = tkMatrix.M34;

            m.D1 = tkMatrix.M41;
            m.D2 = tkMatrix.M42;
            m.D3 = tkMatrix.M43;
            m.D4 = tkMatrix.M44;

            return m;
        }

        public static void ToNumerics(this Assimp.Matrix4x4 matIn, out SN.Matrix4x4 matOut)
        {
            //Assimp matrices are column vector, so X,Y,Z axes are columns 1-3 and 4th column is translation.
            //Columns => Rows to make it compatible with numerics
            matOut = new System.Numerics.Matrix4x4(matIn.A1, matIn.B1, matIn.C1, matIn.D1, //X
                                                   matIn.A2, matIn.B2, matIn.C2, matIn.D2, //Y
                                                   matIn.A3, matIn.B3, matIn.C3, matIn.D3, //Z
                                                   matIn.A4, matIn.B4, matIn.C4, matIn.D4); //Translation
        }

        public static void FromNumerics(this SN.Matrix4x4 matIn, out Assimp.Matrix4x4 matOut)
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

        public static Matrix4x4 FromNumerics(this SN.Matrix4x4 matIn)
        {
            Matrix4x4 matOut = new Matrix4x4();

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

            return matOut;
        }

        public static Vector3 ToEulerAngles(Assimp.Quaternion q)
        {
            float PI = (float)Math.PI;
            // Store the Euler angles in radians
            Vector3 pitchYawRoll = new Vector3();

            double sqw = q.W * q.W;
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;

            // If quaternion is normalised the unit is one, otherwise it is the correction factor
            double unit = sqx + sqy + sqz + sqw;
            double test = q.X * q.Y + q.Z * q.W;

            if (test > 0.499f * unit)
            {
                // Singularity at north pole
                pitchYawRoll.Y = 2f * (float)Math.Atan2(q.X, q.W);  // Yaw
                pitchYawRoll.X = PI * 0.5f;                         // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }
            else if (test < -0.499f * unit)
            {
                // Singularity at south pole
                pitchYawRoll.Y = -2f * (float)Math.Atan2(q.X, q.W); // Yaw
                pitchYawRoll.X = -PI * 0.5f;                        // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }
            pitchYawRoll.Y = (float)Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, sqx - sqy - sqz + sqw);       // Yaw
            pitchYawRoll.X = (float)Math.Asin(2 * test / unit);                                             // Pitch
            pitchYawRoll.Z = (float)Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, -sqx + sqy - sqz + sqw);      // Roll
            return pitchYawRoll;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;

namespace Switch_Toolbox.Library.IO
{
    public static class MatrixExenstion
    {
        public static Syroot.Maths.Matrix3x4 GetMatrixInverted(STBone bone)
        {
            return ToMatrix3x4(CalculateInverseMatrix(bone).inverse);
        }

        public class Matrices
        {
            public Matrix4x4 transform = Matrix4x4.Identity;
            public Matrix4x4 inverse = Matrix4x4.Identity;
        }

        public static Matrix4x4 CalculateTransformMatrix(STBone bone)
        {
            var trans = Matrix4x4.CreateTranslation(new Vector3(bone.position[0], bone.position[1], bone.position[2]));
            var scale = Matrix4x4.CreateScale(new Vector3(bone.scale[0], bone.scale[1], bone.scale[2]));

            Matrix4x4 quat = Matrix4x4.Identity;

            if (bone.RotationType == STBone.BoneRotationType.Euler)
                quat = Matrix4x4.CreateFromQuaternion(QuatFromEular(bone.rotation[0], bone.rotation[1], bone.rotation[2]));
            else
                quat = Matrix4x4.CreateFromQuaternion(QuatFromQuat(bone.rotation[0], bone.rotation[1], bone.rotation[2], bone.rotation[3]));

            return Matrix4x4.Multiply(quat, trans);
        }
        public static Matrices CalculateInverseMatrix(STBone bone)
        {
            var matrices = new Matrices();

            //Get parent transform for a smooth matrix
            if (bone.Parent != null && bone.Parent is STBone)
                matrices.transform *= CalculateInverseMatrix((STBone)bone.Parent).transform;
            else
                matrices.transform = Matrix4x4.Identity;

            //Now calculate the matrix with TK matrices
            var trans = Matrix4x4.CreateTranslation(new Vector3(bone.position[0], bone.position[1], bone.position[2]));
            var scale = Matrix4x4.CreateScale(new Vector3(bone.scale[0], bone.scale[1], bone.scale[2]));

            Matrix4x4 quat = Matrix4x4.Identity;

            if (bone.RotationType == STBone.BoneRotationType.Euler)
                quat = Matrix4x4.CreateFromQuaternion(QuatFromEular(bone.rotation[0], bone.rotation[1], bone.rotation[2]));
            else
                quat = Matrix4x4.CreateFromQuaternion(QuatFromQuat(bone.rotation[0], bone.rotation[1], bone.rotation[2], bone.rotation[3]));

            matrices.transform = Matrix4x4.Multiply(Matrix4x4.Multiply(quat, trans), matrices.transform);

            Matrix4x4 Inverse;
            Matrix4x4.Invert(matrices.transform, out Inverse);

            matrices.inverse = Inverse;

            return matrices;
        }

        public static Quaternion QuatFromQuat(float x, float y, float z, float w)
        {
            Quaternion q = new Quaternion();
            q.X = x;
            q.Y = y;
            q.Z = z;
            q.W = w;

            if (q.W < 0)
                q *= -1;

            return q;
        }

        public static Quaternion QuatFromEular(float x, float y, float z)
        {
            Quaternion xRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, x);
            Quaternion yRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, y);
            Quaternion zRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, z);

            Quaternion q = (zRotation * yRotation * xRotation);

            if (q.W < 0)
                q *= -1;

            //return xRotation * yRotation * zRotation;
            return q;
        }

        //Left-Handed
        public static Matrix4x4 ToMatrix4x4(this Syroot.Maths.Matrix3x4 mat)
        {
            return new Matrix4x4()
            {
                M11 = mat.M11,
                M21 = mat.M12,
                M31 = mat.M13,
                M41 = mat.M14,
                M12 = mat.M21,
                M22 = mat.M22,
                M32 = mat.M23,
                M42 = mat.M24,
                M13 = mat.M31,
                M23 = mat.M32,
                M33 = mat.M33,
                M43 = mat.M34,
                M14 = 0,
                M24 = 0,
                M34 = 0,
                M44 = 0
            };
        }

        //Left-Handed 
        public static Syroot.Maths.Matrix3x4 ToMatrix3x4(this Matrix4x4 mat)
        {
            if (mat.M11 == -0) mat.M11 = 0;
            if (mat.M12 == -0) mat.M12 = 0;
            if (mat.M13 == -0) mat.M13 = 0;
            if (mat.M14 == -0) mat.M14 = 0;
            if (mat.M21 == -0) mat.M21 = 0;
            if (mat.M22 == -0) mat.M22 = 0;
            if (mat.M23 == -0) mat.M23 = 0;
            if (mat.M24 == -0) mat.M24 = 0;
            if (mat.M31 == -0) mat.M31 = 0;
            if (mat.M32 == -0) mat.M32 = 0;
            if (mat.M33 == -0) mat.M33 = 0;
            if (mat.M34 == -0) mat.M34 = 0;

            return new Syroot.Maths.Matrix3x4()
            {
                M11 = mat.M11,
                M12 = mat.M21,
                M13 = mat.M31,
                M14 = mat.M41,
                M21 = mat.M12,
                M22 = mat.M22,
                M23 = mat.M32,
                M24 = mat.M42,
                M31 = mat.M13,
                M32 = mat.M23,
                M33 = mat.M33,
                M34 = mat.M43,

                /*    M11 = mat.M11,
                    M12 = mat.M12,
                    M13 = mat.M13,
                    M14 = mat.M14,
                    M21 = mat.M21,
                    M22 = mat.M22,
                    M23 = mat.M23,
                    M24 = mat.M24,
                    M31 = mat.M31,
                    M32 = mat.M32,
                    M33 = mat.M33,
                    M34 = mat.M34,*/
            };
        }
    }
}

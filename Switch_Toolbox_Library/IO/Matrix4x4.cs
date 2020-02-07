using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;

namespace Toolbox.Library.IO
{
    public static class MatrixExenstion
    {
        public static float Deg2Rad = (float)(System.Math.PI * 2) / 360;
        public static float Rad2Deg = (float)(360 / (System.Math.PI * 2));

            public static OpenTK.Vector3 QuaternionToEuler(OpenTK.Quaternion q1)
            {
                float sqw = q1.W * q1.W;
                float sqx = q1.X * q1.X;
                float sqy = q1.Y * q1.Y;
                float sqz = q1.Z * q1.Z;
                float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
                float test = q1.X * q1.W - q1.Y * q1.Z;
                OpenTK.Vector3 v;

           

                if (test > 0.4995f * unit)
                { // singularity at north pole
                    v.Y = 2f * (float)System.Math.Atan2(q1.X, q1.Y);
                    v.X = (float)System.Math.PI / 2;
                    v.Z = 0;
                    return NormalizeAngles(v * Rad2Deg);
                }
                if (test < -0.4995f * unit)
                { // singularity at south pole
                    v.Y = -2f * (float)System.Math.Atan2(q1.Y, q1.X);
                    v.X = (float)-System.Math.PI / 2;
                    v.Z = 0;
                    return NormalizeAngles(v * Rad2Deg);
                }
                Quaternion q = new Quaternion(q1.W, q1.Z, q1.X, q1.Y);
                v.Y = (float)Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (q.Z * q.Z + q.W * q.W));     // Yaw
                v.X = (float)Math.Asin(2f * (q.X * q.Z - q.W * q.Y));                             // Pitch
                v.Z = (float)Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (q.Y * q.Y + q.Z * q.Z));      // Roll
                return NormalizeAngles(v * Rad2Deg);
            }

        static OpenTK.Vector3 NormalizeAngles(OpenTK.Vector3 angles)
        {
            angles.X = NormalizeAngle(angles.X);
            angles.Y = NormalizeAngle(angles.Y);
            angles.Z = NormalizeAngle(angles.Z);
            return angles;
        }

        static float NormalizeAngle(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }

        public static OpenTK.Quaternion EulerToQuaternion(float yaw, float pitch, float roll)
        {
            yaw *= Deg2Rad;
            pitch *= Deg2Rad;
            roll *= Deg2Rad;
            float rollOver2 = roll * 0.5f;
            float sinRollOver2 = (float)Math.Sin((double)rollOver2);
            float cosRollOver2 = (float)Math.Cos((double)rollOver2);
            float pitchOver2 = pitch * 0.5f;
            float sinPitchOver2 = (float)Math.Sin((double)pitchOver2);
            float cosPitchOver2 = (float)Math.Cos((double)pitchOver2);
            float yawOver2 = yaw * 0.5f;
            float sinYawOver2 = (float)Math.Sin((double)yawOver2);
            float cosYawOver2 = (float)Math.Cos((double)yawOver2);
            OpenTK.Quaternion result = OpenTK.Quaternion.Identity;
            result.W = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.X = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.Y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
            result.Z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

            return result;
        }

        public static OpenTK.Matrix4 CreateRotation(OpenTK.Vector3 Normal, OpenTK.Vector3 Tangent)
        {
            var mat4 = OpenTK.Matrix4.Identity;
            var vec3 = OpenTK.Vector3.Cross(Normal, Tangent);

            mat4.M11 = Tangent.X;
            mat4.M21 = Tangent.Y;
            mat4.M31 = Tangent.Z;
            mat4.M12 = Normal.X;
            mat4.M22 = Normal.Y;
            mat4.M32 = Normal.Z;
            mat4.M13 = vec3.X;
            mat4.M23 = vec3.Y;
            mat4.M33 = vec3.Z;

            return mat4;
        }

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
            var trans = Matrix4x4.CreateTranslation(new Vector3(bone.Position.X, bone.Position.Y, bone.Position.Z));
            var scale = Matrix4x4.CreateScale(new Vector3(bone.Scale.X, bone.Scale.Y, bone.Scale.Z));

            Matrix4x4 quat = Matrix4x4.Identity;

            if (bone.RotationType == STBone.BoneRotationType.Euler)
                quat = Matrix4x4.CreateFromQuaternion(QuatFromEular(bone.EulerRotation.X, bone.EulerRotation.Y, bone.EulerRotation.Z));
            else
                quat = Matrix4x4.CreateFromQuaternion(QuatFromQuat(bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W));

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

            matrices.transform = Matrix4x4.Multiply(CalculateTransformMatrix(bone), matrices.transform);

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
            };
        }

        public static OpenTK.Matrix4 ToTKMatrix4x4(this Matrix4x4 mat)
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
            if (mat.M41 == -0) mat.M41 = 0;
            if (mat.M42 == -0) mat.M42 = 0;
            if (mat.M43 == -0) mat.M43 = 0;
            if (mat.M44 == -0) mat.M44 = 0;

            return new OpenTK.Matrix4()
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
                M34 = mat.M44,
                M41 = mat.M14,
                M42 = mat.M24,
                M43 = mat.M34,
                M44 = mat.M44,
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK;

namespace Switch_Toolbox.Library
{
    public class AssimpHelper
    {
        public static OpenTK.Matrix4 TKMatrix(Assimp.Matrix4x4 input)
        {
            return new OpenTK.Matrix4(input.A1, input.B1, input.C1, input.D1,
                                       input.A2, input.B2, input.C2, input.D2,
                                       input.A3, input.B3, input.C3, input.D3,
                                       input.A4, input.B4, input.C4, input.D4);

       /*     return new OpenTK.Matrix4()
            {
                M11 = input.A1,
                M12 = input.A2,
                M13 = input.A3,
                M14 = input.A4,
                M21 = input.B1,
                M22 = input.B2,
                M23 = input.B3,
                M24 = input.B4,
                M31 = input.C1,
                M32 = input.C2,
                M33 = input.C3,
                M34 = input.C4,
                M41 = input.D1,
                M42 = input.D2,
                M43 = input.D3,
                M44 = input.D4
            };*/
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

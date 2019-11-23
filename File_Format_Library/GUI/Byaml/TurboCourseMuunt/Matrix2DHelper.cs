using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class Matrix2DHelper
    { 
        public static Vector3 RotatePoint(Vector3 translate, float X, float Y, float Z, Vector3 rotate)
        {
            Matrix4 rotationX = Matrix4.CreateRotationX(rotate.X);
            Matrix4 rotationY = Matrix4.CreateRotationY(rotate.Z);
            Matrix4 rotationZ = Matrix4.CreateRotationZ(-rotate.Y);

            Matrix4 transMat = Matrix4.CreateTranslation(new Vector3(translate.X, translate.Z, -translate.Y));
            Matrix4 comb = (rotationX * rotationY * rotationZ) * transMat;
            Vector3 pos = new Vector3(X, Y, Z);
            return Vector3.TransformPosition(pos, comb);
        }
    }
}

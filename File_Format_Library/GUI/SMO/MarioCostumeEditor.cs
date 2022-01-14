using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirstPlugin
{
    public class MarioCostumeEditor
    {
        public static Matrix4 SetTransform(string fileName)
        {
            Vector3 position;
            Vector3 scale;
            Vector3 rotation;

            if (fileName.Contains("Mario"))
            {
                if (fileName.Contains("Face"))
                    {
                    Console.WriteLine("Positioning Face Mesh.....");
                    position = new Vector3(0, 97.0f, 0);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, 0, 0);
                }
                if (fileName.Contains("Head"))
                {
                    Console.WriteLine("Positioning Head Mesh.....");
                    position = new Vector3(0, 97.0f, 0);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, 0, 0);
                }
                if (fileName.Contains("HandL"))
                {
                    Console.WriteLine("Positioning HandR Mesh.....");
                    position = new Vector3(48.877f, 82.551f, -3.3f);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, 90f, 0);
                }
                if (fileName.Contains("HandR"))
                {
                    Console.WriteLine("Positioning HandR Mesh.....");
                    position = new Vector3(-48.877f, 82.551f, -3.3f);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, -90f, 0);
                }
                if (fileName.Contains("Eye"))
                {
                    Console.WriteLine("Positioning Eye Mesh.....");
                    position = new Vector3(0, 97.0f, 0);
                    scale = new Vector3(1, 1, 1);
                rotation = new Vector3(0, 0f, 0);
                }
                if (fileName.Contains("Hair"))
                {
                    Console.WriteLine("Positioning Hair Mesh.....");
                    position = new Vector3(0, 97.0f, 0);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, 0f, 0);
                }
                if (fileName.Contains("Skirt"))
                {
                    Console.WriteLine("Positioning Skirt Mesh.....");
                    position = new Vector3(0, 56.0f, 0);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, 0f, 0);
                }
                if (fileName.Contains("Tail"))
                {
                    Console.WriteLine("Positioning Tail Mesh.....");
                    position = new Vector3(0, 56.0f, 0);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, 0f, 0);
                }
                if (fileName.Contains("Shell"))
                {
                    Console.WriteLine("Positioning Shell Mesh.....");
                    position = new Vector3(0, 75.0f, 0);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, 0f, 0);
                }
                if (fileName.Contains("aHakama"))
                {
                    Console.WriteLine("Positioning Hakama Mesh.....");
                    position = new Vector3(0, 61.0f, -3.0f);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, 0f, 0);
                }
                if (fileName.Contains("Under"))
                {
                    Console.WriteLine("Positioning Under Mesh.....");
                    position = new Vector3(0, 56.0f, 0);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, 0f, 0);
                }
                if (fileName.Contains("PonchoPoncho"))
                {
                    Console.WriteLine("Positioning Poncho Mesh.....");
                    position = new Vector3(0, 60.5f, -4.0f);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, 0f, 0);
                }
                if (fileName.Contains("PonchoGuitar"))
                {
                    Console.WriteLine("Positioning Guitar Mesh.....");
                    position = new Vector3(48.877f, 0, -12.3f);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, 90f, 0);
                }
                else
                {
                    position = new Vector3(0, 0, 0);
                    scale = new Vector3(1, 1, 1);
                    rotation = new Vector3(0, 0, 0);
                }
            }

            Matrix4 positionMat = Matrix4.CreateTranslation(position);
            Matrix4 rotXMat = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X));
            Matrix4 rotYMat = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y));
            Matrix4 rotZMat = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z));
            Matrix4 scaleMat = Matrix4.CreateScale(scale);
            return scaleMat * (rotXMat * rotYMat * rotZMat) * positionMat;
        }
    }
}

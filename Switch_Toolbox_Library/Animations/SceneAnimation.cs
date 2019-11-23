using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Toolbox.Library.Animations
{
    public class CameraAnimation : Animation
    {
        //Base values
        public float ClipNear;
        public float ClipFar;
        public float AspectRatio;
        public float FieldOfView;
        public float Twist;

        public Vector3 Position;
        public Vector3 Rotation;

        public bool IsLooping;

        public List<Animation.KeyGroup> Values = new List<Animation.KeyGroup>();

        public Settings settings;

        public enum Settings
        {
            EulerZXY,
            Perspective,
        }

        //Offsets to get data from  "Values"
        public enum CameraOffsetType
        {
            ClipNear = 0,
            ClipFar = 4,
            AspectRatio = 8,
            FieldOFView = 12,
            PositionX = 16,
            PositionY = 20,
            PositionZ = 24,
            RotationX = 28,
            RotationY = 32,
            RotationZ = 36,
            Twist = 40,
        }

        public virtual void NextFrame(Viewport viewport)
        {
            if (Frame >= FrameCount) return;

            float posX = Position.X;
            float posY = Position.Y;
            float posZ = Position.Z;
            float rotX = Rotation.X;
            float rotY = Rotation.Y;
            float rotZ = Rotation.Z;

            if (Frame != 0)
            {
                foreach (var curve in Values)
                {
                    switch ((CameraOffsetType)curve.AnimDataOffset)
                    {
                        case CameraOffsetType.ClipNear: ClipNear = curve.GetValue(Frame); break;
                        case CameraOffsetType.ClipFar: ClipFar = curve.GetValue(Frame); break;
                        case CameraOffsetType.AspectRatio: AspectRatio = curve.GetValue(Frame); break;
                        case CameraOffsetType.FieldOFView: FieldOfView = curve.GetValue(Frame); break;
                        case CameraOffsetType.PositionX: posX = curve.GetValue(Frame); break;
                        case CameraOffsetType.PositionY: posY = curve.GetValue(Frame); break;
                        case CameraOffsetType.PositionZ: posZ = curve.GetValue(Frame); break;
                        case CameraOffsetType.RotationX: rotX = curve.GetValue(Frame); break;
                        case CameraOffsetType.RotationY: rotY = curve.GetValue(Frame); break;
                        case CameraOffsetType.RotationZ: rotZ = curve.GetValue(Frame); break;
                        case CameraOffsetType.Twist: Twist = curve.GetValue(Frame); break;
                    }
                }
            }



            if (viewport.GL_Control != null)
            {
                Console.WriteLine($"Camera {Frame} {ClipNear} {ClipFar} {AspectRatio} {FieldOfView} {new Vector3(posX, posY, posZ)} {new Vector3(rotX, rotY, rotZ)}");

                var matrix = CalculateMatrix(new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ), Twist);
                var quat = matrix.ExtractRotation();

                viewport.GL_Control.CameraTarget = new Vector3(posX, posY, posZ);
                viewport.GL_Control.CamRotX = MathHelper.DegreesToRadians(rotX);
                viewport.GL_Control.CamRotY = MathHelper.DegreesToRadians(rotY);
                viewport.GL_Control.Fov = FieldOfView;
                viewport.GL_Control.ZNear = ClipNear;
                viewport.GL_Control.ZFar = ClipFar;

                //Add frames to the playing animation
                Frame += 1f;

                //Reset it when it reaches the total frame count
                if (Frame >= FrameCount)
                {
                    Frame = 0;
                }
            }
        }

        public Matrix4 CalculateMatrix(Vector3 Rotation, Vector3 Position, float twist)
        {
            if (settings.HasFlag(Settings.EulerZXY))
            {
                return Matrix4.CreateFromQuaternion(STSkeleton.FromEulerAngles(Rotation.Z, Rotation.Y, Rotation.X));
            }
            else
            {
                float rad2deg = (float)(180.0 / Math.PI); 

                var c = Matrix4.CreateRotationZ(twist);

                var rotation = LookAtAngles(Position) * rad2deg;

                return Matrix4.CreateFromQuaternion(STSkeleton.FromEulerAngles(rotation.Z, rotation.Y, rotation.X)) * c;
            }
        }

        public float _x;
        public float _y;
        public float _z;

        public Vector3 LookatAngles()
        {
            return new Vector3((float)Math.Atan2(_y, Math.Sqrt(_x * _x + _z * _z)), (float)Math.Atan2(-_x, -_z), 0.0f);
        }

        public Vector3 LookAtAngles(Vector3 origin)
        {
           return LookatAngles();
        }
    }

    public class LightAnimation : Animation
    {
        //Base values
        public int Enable;

        public Vector3 Position;
        public Vector3 Rotation;

        public Vector2 DistanceAttn;
        public Vector2 AngleAttn;

        public Vector3 Color0;
        public Vector3 Color1;

        public bool IsLooping;

        public List<Animation.KeyGroup> Values = new List<Animation.KeyGroup>();

        //Offsets to get data from  "Values"
        public enum FogOffsetType
        {
            Enable = 0,
            PositionX = 4,
            PositionY = 8,
            PositionZ = 12,
            RotationX = 16,
            RotationY = 20,
            RotationZ = 24,
            DistanceAttnX = 28,
            DistanceAttnY = 32,
            AngleAttnX = 36,
            AngleAttnY = 40,
            Color0R = 44,
            Color0G = 48,
            Color0B = 52,
            Color1R = 56,
            Color1G = 60,
            Color1B = 64,
        }

        public virtual void NextFrame(Viewport viewport)
        {
            if (Frame >= FrameCount) return;
        }
    }

    public class FogAnimation : Animation
    {
        //Base values
        public Vector2 DistanceAttn;

        public Vector3 Color;

        public bool IsLooping;

        public List<Animation.KeyGroup> Values = new List<Animation.KeyGroup>();

        //Offsets to get data from  "Values"
        public enum FogOffsetType
        {
            DistanceAttnX = 0,
            DistanceAttnY = 4,
            ColorR = 8,
            ColorG = 12,
            ColorB = 16,
        }

        public virtual void NextFrame(Viewport viewport)
        {
            if (Frame >= FrameCount) return;

            foreach (var value in Values)
            {
                switch ((FogOffsetType)value.AnimDataOffset)
                {
                    case FogOffsetType.ColorR:
                        break;
                    case FogOffsetType.ColorG:
                        break;
                    case FogOffsetType.ColorB:
                        break;
                    case FogOffsetType.DistanceAttnX:
                        break;
                    case FogOffsetType.DistanceAttnY:
                        break;
                }
            }
        }
    }
}

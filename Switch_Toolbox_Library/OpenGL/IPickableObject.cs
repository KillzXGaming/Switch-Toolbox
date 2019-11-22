using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public interface IPickable2DObject
    {
        bool IsHit(float X, float Y);
        bool IsSelected { get; set; }
        bool IsHovered { get; set; }
        void PickTranslate(float X, float Y, float Z);
        void PickRotate(float X, float Y, float Z);
        void PickScale(float X, float Y, float Z);
    }
}

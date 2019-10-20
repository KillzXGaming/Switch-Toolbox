using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public class STRectangle
    {
        public int LeftPoint;
        public int RightPoint;
        public int TopPoint;
        public int BottomPoint;

        public STRectangle(float posX, float posY, float width, float height)
        {
            LeftPoint =    (int)(posX - (width / 2));
            RightPoint =   (int)(posX + (width / 2));
            TopPoint =     (int)(posY + (height / 2));
            BottomPoint =  (int)(posY - (height / 2));
        }

        public STRectangle(int left, int right, int top, int bottom)
        {
            LeftPoint = left;
            RightPoint = right;
            TopPoint = top;
            BottomPoint = bottom;
        }

        public bool IsHit(int X, int Y)
        {
            bool isInBetweenX = (X > LeftPoint) && (X < RightPoint) ||
                                (X < LeftPoint) && (X > RightPoint);

            bool isInBetweenY = (Y > BottomPoint) && (Y < TopPoint) ||
                                (Y < BottomPoint) && (Y > TopPoint);

            if (isInBetweenX && isInBetweenY)
                return true;
            else
                return false;
        }
    }
}

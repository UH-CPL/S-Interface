using System;
using System.Collections.Generic;
using System.Text;

namespace deformTracker
{
    public class InterestPoint
    {
        public int x;
        public int y;
        public int distX;
        public int distY;

        public InterestPoint()
        {
            x = 0;
            y = 0;
            distX = 0;
            distY = 0;
        }

        public InterestPoint(int setX, int setY, int setDistX, int setDistY)
        {
            x = setX;
            y = setY;
            distX = setDistX;
            distY = setDistY;
        }
    }
}

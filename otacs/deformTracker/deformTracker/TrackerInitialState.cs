using System;
using System.Collections.Generic;
using System.Text;

namespace deformTracker
{
    public class TrackerInitialState
    {
        public int trackerInitialWidth;
        public int trackerInitialHeight;
        public int trackerInitialCenterX;
        public int trackerInitialCenterY;

        public TrackerInitialState(int width, int height, int centerX, int centerY)
        {
            trackerInitialWidth = width;
            trackerInitialHeight = height;
            trackerInitialCenterX = centerX;
            trackerInitialCenterY = centerY;
        }
    }
}

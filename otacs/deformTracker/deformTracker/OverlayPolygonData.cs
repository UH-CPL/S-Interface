using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deformTracker
{
    public class OverlayPolygonData
    {
        public float[] x;
        public float[] y;


        public OverlayPolygonData()
        {
            x = new float[5];
            y = new float[5];
        }
    }
}

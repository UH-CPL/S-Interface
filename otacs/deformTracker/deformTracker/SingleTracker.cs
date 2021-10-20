using System;
using System.Collections.Generic;
using System.Text;


namespace deformTracker
{
   
    public class SingleTracker
    {
        public int colInd;
        public int rowInd;
        public int width;
        public int height;
        //private int downSampledWidth;
        //private int downSampledHeight;
        public int initialCenterX;
        public int initialCenterY;
        public int centerX;
        public int centerY;
        public float rotation;
        public int previousCenterX;
        public int previousCenterY;
        public int previousRotationIndex;
        public int rotationIndex;
        public float bestMatchScore;
        public float[,] originalTemplate;
        public float[,] currentTemplate;
        public float[,] bestROI;
        public bool isSurvivor;
        public int searchRangeX = 1;
        public int searchRangeY = 1;
        public double distToPoint;
        public bool useInitialT;


        public SingleTracker(int singleTrackerHeight, int singleTrackerWidth)
        {
            isSurvivor = true;
            width = singleTrackerWidth;
            height = singleTrackerHeight;
            originalTemplate = new float[singleTrackerHeight, singleTrackerWidth];
            currentTemplate = new float[singleTrackerHeight, singleTrackerWidth];
            bestROI = new float[singleTrackerHeight, singleTrackerWidth];
        }

   

        public void initializeTemplates(float[] normalizedImageData, float[,] originalTemplate, float[,] currentTemplate, ImageInfo imageInfo)
        {
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    int theAddress = (w + centerX - width /2) * imageInfo.colorbands + (h + centerY - height / 2) * imageInfo.stride;
                    if (theAddress >= imageInfo.dataLength || theAddress < 0)
                    {
                        currentTemplate[h, w] = 255;
                        originalTemplate[h, w] = 255;
                        bestROI[h, w] = 255;

                    }
                    else
                    {
                        currentTemplate[h, w] = normalizedImageData[theAddress];
                        originalTemplate[h, w] = normalizedImageData[theAddress];
                        bestROI[h, w] = normalizedImageData[theAddress];

                    }
                }
            }

        }




    }
}

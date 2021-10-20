/////////////////////////////////////////////////////////////////////////////////////////////////
// File:
//			
// Function:
//			
// Author: 
//			Jin Fei <jinfei@cs.uh.edu>
// Date:
//			Nov 16, 2007
// Version:
//			1.0.0
/////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Collections;

namespace BreathImageProcessing
{


    public class BreathIPL
    {

        #region public properties

        public int Top
        {
            set
            {
                _top = value;
            }

            get
            {
                return _top;
            }

        }

        public int Left
        {
            set
            {
                _left = value;
            }

            get
            {
                return _left;
            }

        }

        public int TROIWidth
        {
            set  {
                _width = value;
            }

            get
            {
                return _width;
            }

        }

        public int MROIWidth
        {
            set
            {
                _mroiWidth = value;
            }

            get
            {
                return _mroiWidth;
            }

        }

        public int TROIHeight
        {
            set
            {
                _height = value;
            }

            get
            {
                return _height;
            }

        }

        public int MROIHeight
        {
            set
            {
                _mroiHeight = value;
            }

            get
            {
                return _mroiHeight;
            }

        }

        public int TotalFrames
        {
            set
            {
                _totalframes = value;
            }

            get
            {
                return _totalframes;
            }

        }

        public float[] TROIData
        {
            set
            {
                _thermaldata = value;
            }

            get
            {
                return _thermaldata;
            }
        }


        public float[] MROIData
        {
            set
            {
                _dataMROI = value;
            }

            get
            {
                return _dataMROI;
            }
        }

        public byte[] MROIGrayscaleData
        {
            get
            {
                if (this.mroiGreyLevel == null)
                    mroiConvertGreyLevel();
                return this.mroiGreyLevel;
            }
        }

        public byte[] TROIGrayscaleData
        {
            get
            {
                if (this.troiGreyLevel == null)
                    troiConvertGreyLevel();
                return this.troiGreyLevel;
            }
        }

        #endregion


        #region private field

        //TROI
        private int _top;
        private int _left;
        private int _width;
        private int _height;
        private int _totalframes;

        private int _mroiWidth;
        private int _mroiHeight;

        //MROI from sobel edge
       // private Point[] ptMROI;

        //current frame
        //private int _frameNumber;

        private float[] _thermaldata;
        private float[] _dataMROI;

        private float minTemperature;
        private float maxTemperature;
        private float mroiMinTemperature;
        private float mroiMaxTemperature;

        //edge detector
        private int[,] GX = new int[3, 3];
        private int[,] GY = new int[3, 3];

        //greylevel image
        private byte[] troiGreyLevel;
        private byte[] mroiGreyLevel;

        #endregion


        #region public field

        public readonly float highestTempratureValue;
        public readonly float lowestTempratureValue;
        //gradient
        public byte[] imageXGradient;
        public byte[] imageYGradient;
        //projection
        public int[] imageXGradientProjection;
        public int[] imageYGradientProjection;

        //Bitmap bmp;

        #endregion


        #region constructor

        /// <summary>
        /// constructor
        /// </summary>
        public BreathIPL()
        {
            highestTempratureValue = 38.0f;
            lowestTempratureValue = 18.0f;

            minTemperature = 1000000;
            maxTemperature = -1000000;

            /* 3x3 GX Sobel mask.  Ref: www.cee.hw.ac.uk/hipr/html/sobel.html */
            GX[0,0] = -1; GX[0,1] = 0; GX[0,2] = 1;
            GX[1,0] = -2; GX[1,1] = 0; GX[1,2] = 2;
            GX[2,0] = -1; GX[2,1] = 0; GX[2,2] = 1;

            /* 3x3 GY Sobel mask.  Ref: www.cee.hw.ac.uk/hipr/html/sobel.html */
            GY[0,0] = 1; GY[0,1] = 2; GY[0,2] = 1;
            GY[1,0] = 0; GY[1,1] = 0; GY[1,2] = 0;
            GY[2,0] = -1; GY[2,1] = -2; GY[2,2] = -1;

            this.troiGreyLevel = null;
            this.mroiGreyLevel = null;
        }

        #endregion


        #region public methods

        /// <summary>
        /// sobel edge detection & projects
        /// </summary>
        public void sobelEdge()
        { 
            if (imageXGradient == null)
                imageXGradient = new byte[_width * _height];

            if (imageYGradient == null)
                imageYGradient = new byte[_width * _height];

            if (imageXGradientProjection == null)
                imageXGradientProjection = new int[_width];

            if (imageYGradientProjection == null)
                imageYGradientProjection = new int[_height];

            //greylevel
            troiConvertGreyLevel();

            #region Sobel edge detection

            //sobel edge detection
            int i, j, x, y; 
            for (y = 0; y < _height; y++)  
            {
                for(x=0; x< _width; x++)  
                {
                    int sumX = 0;
                    int sumY = 0;
                    int sumXY = 0;

                     // image boundaries
                    if (y == 0 || y == _height - 1)
                    {
                        sumX = 0;
                        sumY = 0;
                        sumXY = 0;
                    }
                    else if (x == 0 || x == _width - 1)
                    {
                        sumX = 0;
                        sumY = 0;
                        sumXY = 0;
                    }
                    else   // convolution starts here
                    {
                        for (i = -1; i <= 1; i++)  //x gradient (vertical contrast - GX )
                        {
                            for (j = -1; j <= 1; j++)
                            {
                                sumX = sumX + troiGreyLevel[(y + j) * _width + x + i] * GX[i + 1,j + 1];
                            }
                        }

                        for (i = -1; i <= 1; i++)  //y gradient (horizontal  contrast - GY )
                        {
                            for (j = -1; j <= 1; j++)
                            {
                                sumY = sumY + troiGreyLevel[(y + j) * _width + x + i] * GY[i + 1,j + 1];
                            }
                        }

                        if (sumX < 0) sumX = Math.Abs(sumX); //sumX = 0;
                        if (sumX > 255) sumX = 255;
                        //binary output
                        //if (sumX > 127) 
                        //    sumX = 255;
                        //else
                        //    sumX = 0;
                        imageXGradient[y * _width + x] = (byte)sumX;

                        if (sumY < 0) sumY = Math.Abs(sumY); //sumY = 0;
                        if (sumY > 255) sumY = 255;
                        ////binary output
                        //if (sumY > 127)
                        //    sumY = 255;
                        //else
                        //    sumY = 0;
                        imageYGradient[y * _width + x] = (byte)sumY;

                        sumXY = Math.Abs(sumX) + Math.Abs(sumY); //gradient magnitude
                    }

                    if (sumXY < 0) sumXY = Math.Abs(sumXY); //sumXY = 0;
                    if (sumXY > 255) sumXY = 255;

                }//for x
            }//for y


            #endregion

            //y gradient projection  (horizontal contrast - GY )
            for (x = 0; x < _width; x++)
            {
                for (y = 0; y < _height; y++)
                {
                    imageXGradientProjection[x] += imageYGradient[y * _width + x];
                }
            }
            //x gradient projection  (vertical contrast - GX )
            for ( y = 0; y < _height; y++)
            {
                for ( x = 0; x < _width; x++)
                {
                    imageYGradientProjection[y] += imageXGradient[y * _width + x];
                }
            }

        }//edge

        /// <summary>
        /// Convert data from float to byte data type
        /// </summary>
        private void troiConvertGreyLevel()
        {
            if (troiGreyLevel == null)
                troiGreyLevel = new byte[_width * _height];

            //the max and min temperature
            troiMinMaxTemperature();

            float temp_map, temp;

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    //temp = imageFloatData[i * width + j];
                    //if (temp > highestTempratureValue) temp = highestTempratureValue;
                    //else if (temp < lowestTempratureValue) temp = lowestTempratureValue;
                    temp = _thermaldata[i * _width + j];

                    temp_map = (temp - minTemperature) * 255 / (maxTemperature - minTemperature);
                    troiGreyLevel[i * _width + j] = (byte)temp_map;
                }
            }
        }

        private void mroiConvertGreyLevel()
        {
            if (_dataMROI == null)
                return;

            if (mroiGreyLevel == null)
                mroiGreyLevel = new byte[_mroiWidth * _mroiHeight];

            //the max and min temperature
            mroiMinMaxTemperature();

            float temp_map, temp;

            for (int i = 0; i < _mroiHeight; i++)
            {
                for (int j = 0; j < _mroiWidth; j++)
                {
                    //temp = imageFloatData[i * width + j];
                    //if (temp > highestTempratureValue) temp = highestTempratureValue;
                    //else if (temp < lowestTempratureValue) temp = lowestTempratureValue;
                    temp = _dataMROI[i * _mroiWidth + j];

                    temp_map = (temp - mroiMinTemperature) * 255 / (mroiMaxTemperature - mroiMinTemperature);
                    mroiGreyLevel[i * _mroiWidth + j] = (byte)temp_map;
                }
            }
        }


        #endregion


        #region private methods

        /// <summary>
        /// Read temperature data
        /// </summary>
        private void troiMinMaxTemperature()
        {

            float temp;
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    temp = _thermaldata[i * _width + j];

                    if (minTemperature > temp && temp >= lowestTempratureValue) minTemperature = temp;
                    else if (maxTemperature < temp && temp <= highestTempratureValue) maxTemperature = temp;
                }
            }
        }

        private void mroiMinMaxTemperature()
        {

            float temp;
            for (int i = 0; i < _mroiHeight; i++)
            {
                for (int j = 0; j < _mroiWidth; j++)
                {
                    temp = _dataMROI[i * _mroiWidth + j];

                    if (mroiMinTemperature > temp && temp >= lowestTempratureValue) mroiMinTemperature = temp;
                    else if (mroiMaxTemperature < temp && temp <= highestTempratureValue) mroiMaxTemperature = temp;
                }
            }
        }


        #endregion



    }//end of class


}//end of namespace



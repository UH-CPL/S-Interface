/////////////////////////////////////////////////////////////////////////////////////////////////
// File:
//			
// Function:
//			
// Author: 
//			Jin Fei
// Date:
//			Nov 16, 2007
// Version:
//			1.0.0
/////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace BreathImageProcessing
{


    #region Save Record

    public class SaveRecord
    {

        #region Member Variable

        string strFilename;
        StringBuilder objBuffer;
        int iLastLineIndex;
        int iCounter;

        #endregion
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public SaveRecord(string strFilename)
        {
            iLastLineIndex = 0;

            iCounter = 0;

            this.strFilename = strFilename;

            // Remove a file if it exists
            if (File.Exists(strFilename)) File.Delete(strFilename);

            this.objBuffer = new StringBuilder();

            // Create file header
            objBuffer.AppendLine("C,D")  // 1st line
                    .AppendLine()  // 2nd line
                    .AppendLine()  // 3rd line
                    .AppendLine()  // 4th line
                    .AppendLine()  // 5th line
                    .AppendLine()  // 6th line
                    .AppendLine()  // 7th line
                    .AppendLine("\tSecond\toC\tBpm\tWavelet Scale")
                    .AppendLine("Frame\tTime\tTemp\tBreathing Rate\tPower Scale");
        }

        #endregion


        #region Public Methods

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        ///  Save to file in the future
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="strEntry"></param>
        /// <param name="strHeader"></param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        public void saveRecordToFile(string filename, string strEntry, string strHeader)
        {
                Debug.WriteLine(strEntry);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        ///  Save data to a memory-locating buffer
        /// </summary>
        /// <param name="objParams"></param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        public void saveRecordToBuffer(object[] objParams)
        {
            iLastLineIndex = objBuffer.Length;

            foreach (object p in objParams)
                objBuffer.Append(p.ToString()).Append("\t");
            objBuffer.AppendLine();
            iCounter++;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        /// <summary>
        ///  Save the memory buffer to a physical file
        /// </summary>
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////		
        public void flushBufferToFile()
        {
            // Open an I/O stream
            if (!string.IsNullOrEmpty(strFilename))
            {
                StreamWriter objRecordFile = new StreamWriter(strFilename, true);

                // Write the buffer to the stream
                objRecordFile.Write(objBuffer.ToString());
                objRecordFile.Flush();
                objRecordFile.Close();

                // Clear the buffer for new data
                objBuffer.Remove(0, objBuffer.Length);
                iLastLineIndex = 0;
                iCounter = 0;
            }
        }

        public void removeLastRecord() 
        {
            if (objBuffer.Length - iLastLineIndex + 1 > 0)
                objBuffer.Remove(iLastLineIndex, objBuffer.Length - iLastLineIndex);
        }

        public int getRows()
        { return iCounter; }

        #endregion

    }

    #endregion


    #region Breath Computation


    public class BreathComputation
    {

        #region private fields

        private const int histBin = 256;
        private float[] fWeight;

        private const int secPerMin = 60;
        private const double mexhCenterFrequency = 0.25;

        private readonly double smallNumber;
        //private readonly double lowestRate;
        //private readonly double highestRate;

        private int _windowSize; 
        private int _finalWindowSize;
        private int _sampleRate;

        private int _scaleSize; 
        private double _stopChannelPower; 


        private ArrayList  _slidingWindow = new ArrayList();
        private ArrayList _slidingWindowData = new ArrayList();

        private double _meanMROITemperature;
        private double _rateComputed;

        private float[] _thermaldata;
        private float[] _thermalGradient;

        private double _averageWndTemperature;
        private double _stdWndTemperature;

        private double _lastMaxPowerScale;

        public bool computeOnce = true;

        //public double maxNormTemp = -10000;

        Quadratic ClipQ = new Quadratic();
        Linear ClipL = new Linear();
        #endregion


        #region public fields

        #endregion


        #region public properties

        public double MeanMROITemerature
        {
            get
            {
                return _meanMROITemperature;
            }
            set
            {
                _meanMROITemperature = value;
            }
        }

        public double RateComputed
        {
            get
            {
                return _rateComputed;
            }
            set
            {
                _rateComputed = value;
            }
        }

        public double StopChannelPower
        {
            get
            {
                return _stopChannelPower;
            }
            set
            {
                _stopChannelPower = value;
            }
        }



        public float[] ThermalData
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

        public double MaxPowerScale 
        {
            get { return _lastMaxPowerScale; }
        }

        public int SampleRate
        {
            get { return _sampleRate; }
            set { _sampleRate = value; }
        }
        #endregion



        #region constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public BreathComputation()
        {
            smallNumber = 0.0000001;
            //lowestRate = 15.0;
            //highestRate = 40.0;
        }
        #endregion



        #region public method

        public ArrayList getSlidingWindownRef() { return _slidingWindow; }

        public object computeMeanMROITemperature()
        {
            if (_thermaldata.Length <= 0) _meanMROITemperature = 0.0;

            //====================================
            // Average computation
            //====================================
            double total = 0.0;
            double count = 0.0;
            for (int i = 0; i < _thermaldata.Length; i++)
            {
                if (_thermaldata[i] > 0)
                {
                    total += _thermaldata[i];
                    count++;
                }
            }
            _meanMROITemperature = total / count;

            //=====================================
            // NOT WORKING PROPERLY
            // Reverse histogram-based expectation
            //=====================================
            //float[] fBin = histogram();
            //_meanMROITemperature = 0.0;
            //for (int i = 0; i < fBin.Length; i++)
            //{
            //    _meanMROITemperature = _meanMROITemperature + fBin[i] * fWeight[(histBin - 1) - i];
            //}
            
            return _meanMROITemperature;
        }


        // Compute a discrete histogram for a frame
        // return weight indices of thermal data 
        protected float[] histogram()
        { 
            if (fWeight == null)
                fWeight = new float[histBin];

            for (int i = 0; i < histBin; i++)
                fWeight[i] = (float)0.0;

            float[] fBin = new float[histBin];

            float fMin = (float)100.0;
            float fMax = (float)0.0;
            // find min/max
            for (int i = 0; i < _thermaldata.Length; i++)
            {
                if (_thermaldata[i] > 0)
                {
                    if (fMin > _thermaldata[i]) fMin = _thermaldata[i];
                    if (fMax < _thermaldata[i]) fMax = _thermaldata[i];
                }
            }

            // discretize thermal data
            int iIdx;
            float fDel = (float)(fMax - fMin) / (histBin-1);
            for (int i = 0; i < _thermaldata.Length; i++)
            {
                if (_thermaldata[i] > 0)
                {

                    iIdx = (int)Math.Round((_thermaldata[i] - fMin) / fDel);
                    fWeight[iIdx] = fWeight[iIdx] + 1;
                    fBin[iIdx] = fDel * iIdx + fMin;
                    //iWeightIndices[i] = iIdx;
                }
            }

            // compute histogram
            for (int i = 0; i < fWeight.Length; i++)
            {
                fWeight[i] = (float)fWeight[i] / _thermaldata.Length;
            }
            return fBin;
        }

        /// <summary>
        /// the mean MROI temp.
        /// </summary>
        //public void getMeanMROITemperature()
        //{

        //    if (_thermaldata.Length <= 0) _meanMROITemperature = 0.0;

        //    double total = 0.0;
        //    double count = 0.0;

        //    for (int i = 0; i < _thermaldata.Length; i++)
        //    {
        //        if (_thermaldata[i] > 0)
        //        {
        //            total += _thermaldata[i];
        //            count++;
        //        }
        //    }

        //    _meanMROITemperature = total / count;

        //    //sliding window
        //    slideMeanMROITemperature();
        //}

        public void resetBuffer()
        {
            _slidingWindow.Clear();
            _slidingWindowData.Clear();
        }


        /// <summary>
        /// init computation
        /// </summary>
        /// <param name="firstStageSize"></param>
        /// <param name="stages"></param>
        /// <param name="sampleRate"></param>
        public void initBreathComputation(int firstStageSize, int finalStageSize, int sampleRate)
        {
                _windowSize = firstStageSize;
                _finalWindowSize = finalStageSize;
                _sampleRate = sampleRate;
        }


        public void initBreathComputation(int firstStageSize, int finalStageSize, int sampleRate, int cwtScale)
        {
            initBreathComputation(firstStageSize, finalStageSize, sampleRate);
            _scaleSize = cwtScale;
        }


        /// <summary>
        /// compute the cwt
        /// </summary>
        public void breathRateCWT()
        {

            int count = _slidingWindow.Count;
            if( count>=  _windowSize ) 
	        {
                
                double[] dataFFT = new double[2 * _windowSize];
                
                getSlidingWindowData();

                //Remove the DC component
                //RemoveDC_Component();
                RemoveDC_ComponentQFit();

		        Normalization(); //Normalize the temperature

		        initFFTData(dataFFT); //Change to FFT computation format
		        FFT1D(dataFFT, 1); //FFT algo.

                double[] powerSpectrum = new double[_scaleSize];
                double[] powerSpectrumA = new double[_scaleSize];


                double[] mexhFFT = new double[2*_windowSize];
                double[] CWTData = new double[2*_windowSize];

                for(int jj = 0; jj<_scaleSize; jj++)
                {

    		        initMexhData(mexhFFT, jj+1, _windowSize); 
		            FFT1D(mexhFFT, 1); 

                    for (int i = 0; i < 2 * _windowSize; i += 2)
                    {
                        //(a +bi)*(c+di) = (ac - bd) + (ad + bc)i, i^2 = -1
                        CWTData[i] = dataFFT[i] * mexhFFT[i] - dataFFT[i + 1] * mexhFFT[i + 1];  //(ac - bd)
                        CWTData[i + 1] = dataFFT[i] * mexhFFT[i + 1] + dataFFT[i + 1] * mexhFFT[i]; //(ad + bc)
                    }	
                    FFT1D(CWTData, -1); 

                    double scalePower = 0.0;
                    for(int i=0; i<2*_windowSize; i+=2)
                    {
                        double re = CWTData[i];
                        double im = CWTData[i + 1];

                        scalePower += re * re;
                    }

                    powerSpectrum[jj] = scalePower / (jj+1);
                    //powerSpectrumA[jj] = scalePower / _windowSize; 

                }

                
                double maxPower = 0.0;
                double maxPowerScale = _scaleSize;
                
                for (int i = 0; i < _scaleSize; i++)
                {
                    if (powerSpectrum[i] > maxPower)
                    {
                        maxPower = powerSpectrum[i];
                        maxPowerScale = i;
                    }
                }

                
                if(maxPowerScale !=_scaleSize)
                    maxPowerScale++;

                _lastMaxPowerScale = maxPowerScale;
                _rateComputed = mexhCenterFrequency * secPerMin*_sampleRate / maxPowerScale; 
                _stopChannelPower = powerSpectrum[_scaleSize - 1] / 100000;


                if (count != _finalWindowSize)
                {
                    if (count == 2 * _windowSize - 1 && 2 * _windowSize <= _finalWindowSize)
                        _windowSize = 1 * _windowSize;
                }

                
                dataFFT = null;
                mexhFFT = null;
                CWTData = null;
                powerSpectrum = null;

                _slidingWindowData.Clear();

            }

        }

            /// <summary>
            ///  wavelet
            /// </summary>
            /// <param name="mexhFFT"></param>
            /// <param name="scale"></param>
            /// <param name="windowSize"></param>
            private void initMexhData(double[] mexhFFT, int scale, int windowSize)
            {

                double c = 2/( Math.Sqrt(3)* Math.Pow( Math.PI, 0.25) );

	            int i = 0;
	            for(int j=(0-windowSize/2); j<windowSize/2; j++)
	            {
                    double x = 1.0 * j / scale; 
                    double temp = c * Math.Exp(-x*x / 2) * (1 - x*x);
                    mexhFFT[i] = temp;
                    mexhFFT[i + 1] = 0.0;

                    //if (maxNormTemp != -10000)
                    //{
                    //    mexhFFT[i] = temp * maxNormTemp;
                    //}
                    
		            i+=2;
	            }            
            }

        #endregion



        #region private method

        /// <summary>
        /// temp.
        /// </summary>
        //private void slideMeanMROITemperature()
        //{

        //    if (_slidingWindow.Count >= _finalWindowSize)
        //    {
        //        _slidingWindow.RemoveAt(0);
        //        _slidingWindow.Add(_meanMROITemperature);
        //    }
        //    else
        //    {
        //        _slidingWindow.Add(_meanMROITemperature);
        //    }
        //}


        private void getSlidingWindowData()
        {

            int offset = _slidingWindow.Count - _windowSize;

            if( offset < 0 ) return;

            for (int i = 0; i < _windowSize; i++)
                _slidingWindowData.Add( _slidingWindow[offset+i] );
        }


        /// <summary>
        /// Average temp. in sliding window
        /// </summary>
        /// <returns></returns>
        private void getAverageWndTemperature()
        {

	        double sum = 0.0;

            if (_slidingWindowData.Count == _windowSize)
            {
                for (int i = 0; i < _windowSize; i++)
                {
                    double temp = Convert.ToDouble(_slidingWindowData[i]);
                    sum += temp;
                }

                _averageWndTemperature = sum / _windowSize;
            }
            else
                _averageWndTemperature = 0.0;
        }


        /// <summary>
        /// Standard deviation in the sliding window
        /// </summary>
        /// <returns></returns>
        private void getStdWndTemperature()
        {

	        double sum = 0.0;

            if (_slidingWindowData.Count == _windowSize)
            {
                for (int i = 0; i < _windowSize; i++)
                {
                    double temp = Convert.ToDouble(_slidingWindowData[i]);
                    sum += (temp - _averageWndTemperature) * (temp - _averageWndTemperature);
                }

                _stdWndTemperature = sum / _windowSize;

            }
            else
                _stdWndTemperature = 0.0;

        }


        /// <summary>
        /// Normalize the temperature data
        /// </summary>
        private void Normalization()
        {

            //if (computeOnce)
            {
                this.getAverageWndTemperature();
                this.getStdWndTemperature();
                computeOnce = false;
            }
            //maxNormTemp = -10000;
            for (int i = 0; i < _slidingWindowData.Count; i++)
            {
                if (_stdWndTemperature > smallNumber)
                {
                    double temp = Convert.ToDouble(_slidingWindowData[i]);
                    _slidingWindowData[i] = (temp - _averageWndTemperature) / _stdWndTemperature;
                }
                else
                {_slidingWindowData[i] = 0.0;}

                //if (maxNormTemp <= (double)_slidingWindowData[i])
                //{ maxNormTemp = (double)_slidingWindowData[i]; }
            }

        }

   
        private void RemoveDC_Component() //Linear fitting 
        {

            float[] rawData = new float[_slidingWindowData.Count];
            float[] TimeInSec = new float[_slidingWindowData.Count];
            float[] y = new float[_slidingWindowData.Count];

            for (int i = 0; i < _slidingWindowData.Count; i++)
            {
                rawData[i] = (float)Convert.ToDouble(_slidingWindowData[i]);
                TimeInSec[i] = (1 / 25f) * (float)i; // 1/fps = 1/25

            }

            ClipL.LinearFitting(rawData.Length, TimeInSec, rawData);
            float slope = (float)ClipL.getSlope();
            float intercept = (float)ClipL.getIntercept();

            for (int i = 0; i < _slidingWindowData.Count; i++)
            {
                double temp = Convert.ToDouble(_slidingWindowData[i]);
                y[i] = (slope * TimeInSec[i]) + intercept; //y = slope*x+intercept
                _slidingWindowData[i] = temp - y[i];

            }

        }

        private void RemoveDC_ComponentQFit() //Quadratic fitting 
        {
            float[] rawData = new float[_slidingWindowData.Count];
            float[] TimeInSec = new float[_slidingWindowData.Count];
            float[] y = new float[_slidingWindowData.Count];

            for (int i = 0; i < _slidingWindowData.Count; i++)
            {
                rawData[i] = (float)Convert.ToDouble(_slidingWindowData[i]);
                TimeInSec[i] = i+1;//(1 / 25f) * (float)i; // 1/fps = 1/25

            }

            //Compute Quadratic parameters
            ClipQ.ComputeQuadraticFitting(rawData.Length, TimeInSec, rawData);

            float b0 = (float)ClipQ.get_b0();
            float b1 = (float)ClipQ.get_b1();
            float b2 = (float)ClipQ.get_b2();

            for (int i = 0; i < _slidingWindowData.Count; i++)
            {
                double temp = Convert.ToDouble(_slidingWindowData[i]);
                y[i] = b0 + (b1*TimeInSec[i]) + (b2*TimeInSec[i]*TimeInSec[i]); //y = b0 + b1*x + b2*x^2; 
                _slidingWindowData[i] = temp - y[i];

            }

            //==================================================
            // Alter Opt's value among 1, 2, 3 to experiment different schemes
            //int Opt = 1;
            //double Quad_MSE_Thres = 0.1 * 0.1; // Allow each estimate differ from ground truth 0.1
            //double Lin_MSE_Thres = 0.1 * 0.1; // Allow each estimate differ from ground truth 0.1
            //double Lin_Slope_Thres = (double)(Math.PI * 15.0) / 180.0; // Allow slope varies with [-15o, 15o];

            //switch(Opt)
            //{
            //    case 1:
            //        //==================================================
            //        // OPTION 1: rely on mean square error to whether or not
            //        // proceed on quadratic fitting
            //        //==================================================
            //        // Perform quadratic fitting if the mean square error is less than a threshold
            //        if (ClipQ.get_msqError() < Quad_MSE_Thres)
            //        {
            //            float b0 = (float)ClipQ.get_b0();
            //            float b1 = (float)ClipQ.get_b1();
            //            float b2 = (float)ClipQ.get_b2();

            //            for (int i = 0; i < _slidingWindowData.Count; i++)
            //            {
            //                double temp = Convert.ToDouble(_slidingWindowData[i]);
            //                y[i] = b0 + (b1*TimeInSec[i]) + (b2*TimeInSec[i]*TimeInSec[i]); //y = b0 + b1*x + b2*x^2; 
            //                _slidingWindowData[i] = temp - y[i];

            //            }
            //        }
            //        break;

            //    case 2:
            //        //==================================================
            //        // OPTION 2: compute linear fitting and rely on the 
            //        // mean square error to whether or not proceed on 
            //        // quadratic fitting
            //        //==================================================
            //        ClipL.LinearFitting(rawData.Length, TimeInSec, rawData);
            //        // Perform quadratic fitting if the mean square error is less than a threshold
            //        if (ClipL.getmsqError() > Lin_MSE_Thres)
            //        {
            //            float b0 = (float)ClipQ.get_b0();
            //            float b1 = (float)ClipQ.get_b1();
            //            float b2 = (float)ClipQ.get_b2();

            //            for (int i = 0; i < _slidingWindowData.Count; i++)
            //            {
            //                double temp = Convert.ToDouble(_slidingWindowData[i]);
            //                y[i] = b0 + (b1*TimeInSec[i]) + (b2*TimeInSec[i]*TimeInSec[i]); //y = b0 + b1*x + b2*x^2; 
            //                _slidingWindowData[i] = temp - y[i];

            //            }
            //        }
            //        break;

            //    case 3:
            //        //==================================================
            //        // OPTION 3: compute linear fitting and rely on the 
            //        // slope to whether or not proceed on quadratic fitting
            //        //==================================================
            //        ClipL.LinearFitting(rawData.Length, TimeInSec, rawData);
            //        if (Math.Abs(ClipL.getSlope()) > Lin_Slope_Thres)
            //        {
            //            float b0 = (float)ClipQ.get_b0();
            //            float b1 = (float)ClipQ.get_b1();
            //            float b2 = (float)ClipQ.get_b2();

            //            for (int i = 0; i < _slidingWindowData.Count; i++)
            //            {
            //                double temp = Convert.ToDouble(_slidingWindowData[i]);
            //                y[i] = b0 + (b1*TimeInSec[i]) + (b2*TimeInSec[i]*TimeInSec[i]); //y = b0 + b1*x + b2*x^2; 
            //                _slidingWindowData[i] = temp - y[i];

            //            }
            //        }
            //        break;
            //}
        }


 
        /// <summary>
        /// prepare data for FFT computation
        /// </summary>
        /// <param name="dataFFT"></param>
        private void initFFTData(double[] dataFFT)
        {

	        if(_slidingWindowData.Count !=  _windowSize) return;  

	        int i = 0;
	        for(int j=0; j<_windowSize; j++)
	        {
                double temp = (double)_slidingWindowData[j];
		        dataFFT[i] = temp;
		        dataFFT[i+1]=0.0;
		        i+=2;
	        }

        }

        /// <summary>
        /// Compute central gradient estimate
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private float[] gradient(float[] a)
        {
            if (a.Length < 3)
                return null;

            if (_thermalGradient == null)
                _thermalGradient = new float[a.Length];
            _thermalGradient[0] = a[1] - a[0];
            _thermalGradient[a.Length - 1] = a[a.Length - 1] - a[a.Length - 2];

            for (int i = 1; i < a.Length - 1; i++)
            {
                _thermalGradient[i] = (float)(a[i + 1] - a[i - 1]) / 2;
            }

            return _thermalGradient;
        }

        /// <summary>
        /// Swap a's & b's value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private  void SWAP(ref double a,  ref double b)
	    {
            double dum=a; 
            a=b; 
            b=dum;
        }

        /// <summary>
        /// Fast Fourier Transform
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isign"></param>
        private void FFT1D(double[] data, int isign)
        {
            int n, mmax, m, j, istep, i;
            double wtemp, wr, wpr, wpi, wi, theta, tempr, tempi;

            int nn = data.Length / 2;
            n = nn << 1;
            j = 1;
            for (i = 1; i < n; i += 2)
            {
                if (j > i)
                {
                    SWAP(ref data[j - 1], ref data[i - 1]);
                    SWAP(ref data[j], ref data[i]);
                }
                m = nn;

                while (m >= 2 && j > m)
                {
                    j -= m;
                    m >>= 1;
                }
                j += m;
            }

            //here begin the Danielson-Lanczos section of the routine.
            mmax = 2;
            while (n > mmax)
            {
                istep = mmax << 1;
                theta = isign * (2 * Math.PI / mmax);
                wtemp = Math.Sin(0.5 * theta);
                wpr = -2.0 * wtemp * wtemp;
                wpi = Math.Sin(theta);
                wr = 1.0;
                wi = 0.0;
                for (m = 1; m < mmax; m += 2)
                {
                    for (i = m; i < n; i += istep)
                    {
                        j = i + mmax;
                        tempr = wr * data[j - 1] - wi * data[j];
                        tempi = wr * data[j] + wi * data[j - 1];
                        data[j - 1] = data[i - 1] - tempr;
                        data[j] = data[i] - tempi;
                        data[i - 1] += tempr;
                        data[i] += tempi;

                    }
                    wr = (wtemp = wr) * wpr - wi * wpi + wr;
                    wi = wi * wpr + wtemp * wpi + wi;

                }
                mmax = istep;
            }
            return;
        }

       #endregion

    }

    #endregion
}


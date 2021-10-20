using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;

namespace PlugIn
{
    public class SampledItem
    {
        internal double dTimestamp;
        internal object oComputedData;
        internal long iFrameNumber;

        public SampledItem(double dOffsetInSecond, object data, long frameNo) { dTimestamp = dOffsetInSecond; oComputedData = data; iFrameNumber = frameNo; }
    }
    
    public class Sampling
    {
        private long iPrevFrameSlidingIndex;
        private double dPrevFrameTimestamp;
        private long iSlidingIndex;
        private double dLeftSlidingTimestamp;
        private double dRightSlidingTimestamp;
        private double dSamplingInterval;
        private double dOffset;

        private ArrayList oSampledData;
        private int iSampledDataCapacity;
        private object oCrtData;
        private long lPrevFrameNumber;

        private List<SampledItem> oSampledItems;
        private bool bRemoveLast;

        public delegate object ComputeFrame();

        private ComputeFrame _mComputeMethod;

        private double dDebugTime;
        private int iLastC;

       // public int NoInterpolationFrames;
       // public int totalNumberOfFrames;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dSamplingInterval"></param>
        /// <param name="oSampledDataQueue"> Queue of computed sampled data. 
        /// An element is taken off from the begin of the queue and added at the end </param>
        /// <param name="iCapacity"> iCapacity = -1 denotes unlimited queue </param>
        /// <param name="mComputeMethod"> A method doing some computation on data sampled </param>
        public Sampling(double dSamplingInterval, ArrayList oSampledDataQueue, int iCapacity, ComputeFrame mComputeMethod)
        {
            iPrevFrameSlidingIndex = 0;
            iSlidingIndex = 0;
            dPrevFrameTimestamp = 0.0;
            dLeftSlidingTimestamp = 0.0;
            dOffset = 0.0;
            this.dSamplingInterval = dSamplingInterval;
            dRightSlidingTimestamp = dSamplingInterval + dLeftSlidingTimestamp;
            oSampledData = oSampledDataQueue;
            iSampledDataCapacity = iCapacity;
            _mComputeMethod = mComputeMethod;
            oCrtData = null;
            oSampledItems = new List<SampledItem>();

            dDebugTime = 0.0;
            //NoInterpolationFrames = 0;
            //totalNumberOfFrames = 0;
        }

        public void setTimeOffsetInterval(double d)
        {
            dOffset = d;
            iSlidingIndex = 1;
            dLeftSlidingTimestamp = dOffset;
            dRightSlidingTimestamp = dLeftSlidingTimestamp + dSamplingInterval;
        }

        public bool checkRemoveLast() { return bRemoveLast; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lProcessedFrameNumber"></param>
        /// <param name="lCurrentFrameNumber"></param>
        /// <param name="dCurrentFrameTimestamp"></param>
        /// <param name="dOffsetInterval"></param>
        public SampledItem[] sampleWithInterpolate(ref long lProcessedFrameNumber, long lCurrentFrameNumber, double dCurrentFrameTimestamp)
        {
            int iFrameDiff = (int)(lCurrentFrameNumber - lProcessedFrameNumber);
            double dTimeStep = (double)(dCurrentFrameTimestamp - dPrevFrameTimestamp) / iFrameDiff;

            // Compute current data
            object oCurrent = _mComputeMethod();

            // TODO: Modified for a specific problem
            // Temperature difference
            double dPrevTemp;
            double dTempStep;

            bRemoveLast = false;

            if (oSampledData.Count > 0)
            {
                dPrevTemp = (double)oSampledData[oSampledData.Count - 1];
                dTempStep = (double)((double)oCurrent - dPrevTemp) / iFrameDiff;

                oSampledItems.Clear();
                SampledItem [] oSis;
                int iPrevCounter = oSampledData.Count;
                double c = 0;
                for (int i = 1; i <= iFrameDiff; i++)
                {
                    oCrtData = (object)(dPrevTemp + (dTempStep * i));
                    oSis = sampleWithoutInterpolate(lProcessedFrameNumber + i, dPrevFrameTimestamp + dTimeStep);

                    //if (oSis != null) 
                    //{
                    //    if (iPrevCounter == oSampledData.Count)
                    //        if (oSampledItems.Count > 0) oSampledItems.RemoveAt(oSampledItems.Count - 1);
                    //        else bRemoveLast = true;
                    //    oSampledItems.Add(oSis[0]);
                    //    iPrevCounter = oSampledData.Count;
                    //}
                    oSampledItems.Add(new SampledItem(dPrevFrameTimestamp, oCrtData, lProcessedFrameNumber + i));
                    
                }

                oCrtData = null;
            }
            else {
                sampleWithoutInterpolate(lCurrentFrameNumber, dCurrentFrameTimestamp);
            }
            lProcessedFrameNumber = lPrevFrameNumber;

            return oSampledItems.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lCurrentFrameNumber"></param>
        /// <param name="dCurrentFrameTimestamp"></param>
        public SampledItem[] sampleWithoutInterpolate(long lCurrentFrameNumber, double dCurrentFrameTimestamp)
        {

            //lPrevFrameNumber = lCurrentFrameNumber;
            //dPrevFrameTimestamp = dCurrentFrameTimestamp;
            //if (NoInterpolationFrames >= 20 && oCrtData != null) 
            ////if (totalNumberOfFrames > 0 &&   oCrtData != null)
            //{
            //    NoInterpolationFrames = 0;
            //    oCrtData = null;
            //}

            object oCurrentData = (oCrtData != null)?oCrtData:_mComputeMethod();
            // For debug
            //if (dDebugTime == 0.0) { dDebugTime = dCurrentFrameTimestamp; iLastC = 0; }

            if (dCurrentFrameTimestamp < dLeftSlidingTimestamp)
            { 
                // Do nothing
                return null;
            }
            else if (dCurrentFrameTimestamp >= dLeftSlidingTimestamp && dCurrentFrameTimestamp <= dRightSlidingTimestamp)
            { 
                if (iPrevFrameSlidingIndex == iSlidingIndex)
                {
                    // Replace S(rec - 1) with S(rec)
                    if (oSampledData.Count > 0)
                    {
                        oSampledData.RemoveAt(oSampledData.Count - 1);
                        iLastC--;
                    }
                }
                iLastC++;
                addSampledData(oCurrentData);
                lPrevFrameNumber = lCurrentFrameNumber;
                dPrevFrameTimestamp = dCurrentFrameTimestamp;
                iPrevFrameSlidingIndex = iSlidingIndex;
            }
            else
            {
                // T(rec) > RightSlidingTimestamp
                if (iPrevFrameSlidingIndex == iSlidingIndex)
                {
                    double dTmp1 = Math.Abs(dPrevFrameTimestamp - dRightSlidingTimestamp);
                    double dTmp2 = Math.Abs(dCurrentFrameTimestamp - dRightSlidingTimestamp);
                    if (dTmp1.CompareTo(dTmp2) > 0)
                    {
                        // Replace S(rec - 1) with S(rec)
                        oSampledData.RemoveAt(oSampledData.Count - 1);
                        // addSampledData(oCurrentData);
                        iPrevFrameSlidingIndex = iSlidingIndex;
                    }
                    else
                    {
                        // addSampledData(oCurrentData);
                        iPrevFrameSlidingIndex = iSlidingIndex + 1;
                        iLastC++;
                    }
                }
                else
                {
                    iLastC++;
                    iPrevFrameSlidingIndex = iSlidingIndex;
                }

                addSampledData(oCurrentData);
                dPrevFrameTimestamp = dCurrentFrameTimestamp;
                lPrevFrameNumber = lCurrentFrameNumber;
                increaseSlidingIndex();
            }

            // For debug
            if (dCurrentFrameTimestamp - dDebugTime >= 1.0)
            {
                dDebugTime = dCurrentFrameTimestamp;
                iLastC = 0;
            }

            return new SampledItem[1] { new SampledItem(dCurrentFrameTimestamp, oCurrentData, lCurrentFrameNumber) };
        }

        /// <summary>
        /// 
        /// </summary>
        private void increaseSlidingIndex()
        {
            iSlidingIndex++;
            dLeftSlidingTimestamp = dRightSlidingTimestamp;
            dRightSlidingTimestamp = dLeftSlidingTimestamp + dSamplingInterval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        private void addSampledData(object o)
        {

            if (oSampledData.Count >= iSampledDataCapacity)
            {
                oSampledData.RemoveAt(0);
                oSampledData.Add(o);
            }
            else
            {
                oSampledData.Add(o);
            }
        }
    }
}

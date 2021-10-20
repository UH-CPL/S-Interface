using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeGraph
{
    class Biofeedback_Selfstart2
    {
        List<float> listRawMeasurements = new List<float>();
        List<float> listDataPoints = new List<float>();
        public List<float> ListDataPoints
        {
            get
            {
                return listDataPoints;
            }
        }

        List<float> listPCn = new List<float>();
        public List<float> ListPCn
        {
            get
            {
                return listPCn;
            }
        }

        int curTime = 0;
        float Xn = 0;
        float Sn2 = 0;
        float XnMinus1 = 0;
        float Sn2Minus1 = 0;
        float Wn = 0;
        int stressTime = 0;
        float Tn = 0;
        float CDFn = 0;
        float Un = 0;
        float PCn = 0;
        float NCn = 0;

        public const int THR_INITIALWAITTIME = 20;
        public const int THR_MAXSTRESSTIME = 45;
        public const int THR_MINSTRESSTIME = 5;
        public const float THR_KPLUS = 0.5f;
        public const float THR_HPLUS = 5.07f;
        public const float THR_KMINUS = 0.5f;
        public const float THR_HMINUS = -5.07f;

        bool isAlreadyAlerted = false;
        bool isStressed = false;
        public bool IsStressed
        {
            get
            {
                return isStressed;
            }
        }
        bool isStressDetected = false;
        bool isMarkedToTurnoffStress = false;

        bool iSStimulus = false;
        public bool ISStimulus
        {
            get
            {
                return iSStimulus;
            }
            set
            {
                iSStimulus = value;
            }
        }
        int stimulusFrameCount = 0;

        REngine engine = REngine.GetInstance();



        public Biofeedback_Selfstart2()
        {
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();
        }

        public void CheckStress(float measurement, float newTime)
        {
            if ((int)newTime > curTime)
            {
                if (ISStimulus)
                {
                    stimulusFrameCount++;
                }

                curTime = (int)newTime;

                if(listRawMeasurements.Count > 2)
                {
                    float dp = 0;
                    foreach (float value in listRawMeasurements)
                    {
                        dp += value;
                    }
                    dp /= listRawMeasurements.Count;
                    dp = (float)Math.Log(dp);
                    listDataPoints.Add(dp);


                    listRawMeasurements.Clear();

                    CalculateRunningMeanVariance();

                    if (stimulusFrameCount <= THR_INITIALWAITTIME)
                    {
                        isStressed = false;

                        listPCn.Add(float.MaxValue);
                    }
                    else
                    {
                        CalculateTn();
                        CalculateCDFn();
                        CalculateUn();
                        CalculatePCn();
                        listPCn.Add(PCn);

                        if(isMarkedToTurnoffStress)
                        {
                            if (stressTime > THR_MINSTRESSTIME)
                            {
                                isStressDetected = false;
                                isStressed = false;
                                isMarkedToTurnoffStress = false;
                            }
                            else
                            {
                                stressTime++;
                            }
                        }
                        else
                        {
                            if (!isStressDetected)
                            {
                                if (!isAlreadyAlerted)
                                {
                                    isStressDetected = CompareThresholds();
                                }
                            }
                            else
                            {
                                stressTime++;
                                if (stressTime > THR_MINSTRESSTIME && !isAlreadyAlerted)
                                {
                                    isStressed = true;
                                    isAlreadyAlerted = true;
                                    stressTime = 0;
                                }

                                if (stressTime < THR_MAXSTRESSTIME)
                                {
                                    CalculateNCn();

                                    if (NCn < THR_HMINUS)
                                    {
                                        isStressDetected = false;
                                        stressTime = 0;
                                        if (isStressed)
                                        {
                                            isMarkedToTurnoffStress = true;
                                        }  
                                    }
                                }
                                else
                                {
                                    isStressDetected = false;
                                    isMarkedToTurnoffStress = true;
                                    stressTime = 0;
                                }
                            }
                        }

                        
                    }
                }
                

            }
            else
            {
                listRawMeasurements.Add(measurement);
            }
        }

        void CalculateRunningMeanVariance()
        {
            if (listDataPoints.Count > 0)
            {
                XnMinus1 = Xn;
                Sn2Minus1 = Sn2;

                if (listDataPoints.Count == 1)
                {
                    Xn = listDataPoints[0];
                    Wn = 0;
                    Sn2 = 0;
                }
                else
                {
                    float newDataPoint = listDataPoints[listDataPoints.Count - 1];
                    float newXn = Xn + ((newDataPoint - Xn) / listDataPoints.Count);

                    Wn = Wn + (((listDataPoints.Count - 1) * (float)Math.Pow(newDataPoint - Xn, 2.0)) / listDataPoints.Count);

                    Sn2 = Wn / (listDataPoints.Count - 1.0f);

                    Xn = newXn;
                }
            }
        }

        void CalculateTn()
        {
            Tn = (listDataPoints[listDataPoints.Count - 1] - XnMinus1) / (float)Math.Sqrt(Sn2Minus1);
        }

        bool CompareThresholds()
        {
            if(PCn > THR_HPLUS)
            {
                return true;
            }

            return false;
        }



        void CalculateCDFn()
        {
            float n = (float)listDataPoints.Count;
            float t = Tn * (float)Math.Sqrt((n - 1) / n);

            engine.Initialize();

            int df = listDataPoints.Count - 2;
            string input = "pt(" + t.ToString() + ",df=" + df.ToString() + ")";
            //calculate
            CharacterVector vector = engine.Evaluate(input).AsCharacter();
            string result = vector[0];

            CDFn = float.Parse(result);
        }

        void CalculateUn()
        {
            try
            {
                engine.Initialize();

                string input = "qnorm(" + CDFn.ToString("0.00") + ")";
                //calculate
                CharacterVector vector = engine.Evaluate(input).AsCharacter();
                string result = vector[0];

                Un = float.Parse(result);
            }
            catch
            {
                Un = 0;
            }
        }

        void CalculatePCn()
        {
            PCn = Math.Max(0.0f, Un - THR_KPLUS + PCn);
        }

        void CalculateNCn()
        {
            NCn = Math.Min(0.0f, Un + THR_KMINUS + NCn);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;

namespace RealTimeGraph
{
    class Biofeedback_Selfstart1
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

        List<float> listGroup1DataPoints = new List<float>();
        List<float> listGroup2DataPoints = new List<float>();
        List<float> listKn = new List<float>();
        public List<float> ListKn
        {
            get
            {
                return listKn;
            }
        }

        List<float> listLn = new List<float>();
        public List<float> ListLn
        {
            get
            {
                return listLn;
            }
        }

        int curTime = 0;
        float Xn = 0;
        float Sn2 = 0;
        float Wn = 0;
        float XnMinus1 = 0;
        float Sn2Minus1 = 0;
        float X1 = 0;
        float S12 = 0;
        float X2 = 0;
        float S22 = 0;
        float Tn = 0;
        float Vn = 0;

        public const int THR_WINDOWCOUNT = 5;
        public const int THR_INITIALWAITTIME = 20;
        public const int THR_MINSTRESSTIME = 5;
        public const int THR_MAXSTRESSTIME = 45;

        bool isAlreadyAlerted = false;
        bool isStressed = false;
        public bool IsStressed
        {
            get
            {
                return isStressed;
            }
        }

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
        bool isStressDetected = false;

        int stressTime = 0;

        REngine engine = REngine.GetInstance();

        public Biofeedback_Selfstart1()
        {
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();
        }

        public void CheckStress(float measurement, float newTime)
        {
            if((int)newTime > curTime)
            {
                if(ISStimulus)
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
                    dp = (float) Math.Log(dp);
                    listDataPoints.Add(dp);


                    listRawMeasurements.Clear();

                    CalculateRunningMeanVariance();

                    if (stimulusFrameCount <= THR_INITIALWAITTIME)
                    {
                        listKn.Add(float.MaxValue);
                        listLn.Add(float.MaxValue);
                        isStressed = false;
                    }
                    else
                    {
                        if (!!isStressDetected)
                        {
                            if (!isAlreadyAlerted)
                            {
                                CalculateThresholds();

                                if (listDataPoints.Count > THR_INITIALWAITTIME + THR_WINDOWCOUNT + 1)
                                {
                                    isStressDetected = CompareThresholds();
                                }
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

                                for (int i = 0; i < listDataPoints.Count - THR_WINDOWCOUNT; i++)
                                {
                                    listGroup1DataPoints.Add(listDataPoints[i]);
                                }
                                CalculateGroup1MeanVariance();

                                for (int i = listDataPoints.Count - THR_WINDOWCOUNT; i < listDataPoints.Count; i++)
                                {
                                    listGroup2DataPoints.Add(listDataPoints[i]);
                                }
                            }

                            if (stressTime < THR_MAXSTRESSTIME)
                            {
                                stressTime++;

                                listGroup2DataPoints.Add(listDataPoints[listDataPoints.Count - 1]);
                                CalculateGroup2MeanVariance();
                                CalculateTnVn();

                                engine.Initialize();
                                string input = "qt(0.95, df=" + ((int)Math.Round(Vn)).ToString() + ")";

                                //calculate
                                CharacterVector vector = engine.Evaluate(input).AsCharacter();
                                string result = vector[0];
                                float Tvn = float.Parse(result);

                                if (Tn < Tvn)
                                {
                                    isStressed = false;
                                }
                            }
                            else
                            {
                                if (isStressed)
                                    isStressed = false;
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

                    Wn = Wn + (((listDataPoints.Count - 1) * (float) Math.Pow(newDataPoint - Xn, 2.0))/ listDataPoints.Count);

                    Sn2 = Wn / (listDataPoints.Count - 1.0f);

                    Xn = newXn;
                }
            } 
        }

        void CalculateThresholds()
        {
            float Kn = XnMinus1 + 2.0f * (float)Math.Sqrt(Sn2Minus1);
            float Ln = XnMinus1 + 3.0f * (float)Math.Sqrt(Sn2Minus1);

            listKn.Add(Kn);
            listLn.Add(Ln);
        }

        bool CompareThresholds()
        {
            int start = listDataPoints.Count - THR_WINDOWCOUNT;
            int KnCount = 0;
            int LnCount = 0;

            for(int i=start; i<start+THR_WINDOWCOUNT; i++)
            {
                if(listDataPoints[i] > listKn[start - 1])
                {
                    KnCount++;
                }

                if (listDataPoints[i] > listLn[start - 1])
                {
                    LnCount++;
                }
            }

            if ((KnCount >= 4) || (LnCount >= 2))
                return true;
                
            return false;
        }

        void CalculateGroup1MeanVariance()
        {
            X1 = 0;
            S12 = 0;

            for (int i = 0; i < listGroup1DataPoints.Count; i++)
            {
                X1 += listGroup1DataPoints[i];
            }
            X1 /= (float)listGroup1DataPoints.Count;

            for (int i = 0; i < listGroup1DataPoints.Count; i++)
            {
                S12 += (float)Math.Pow(listGroup1DataPoints[i] - X1, 2);
            }
            S12 /= listGroup1DataPoints.Count;
        }

        void CalculateGroup2MeanVariance()
        {
            X2 = 0;
            S22 = 0;

            for (int i = 0; i < listGroup2DataPoints.Count; i++)
            {
                X2 += listGroup2DataPoints[i];
            }
            X2 /= (float)listGroup2DataPoints.Count;

            for (int i = 0; i < listGroup2DataPoints.Count; i++)
            {
                S22 += (float)Math.Pow(listGroup2DataPoints[i] - X2, 2);
            }
            S22 /= listGroup2DataPoints.Count;
        }

        void CalculateTnVn()
        {
            Tn = (X2 - X1) / (float) Math.Sqrt((S12 / (float)listGroup1DataPoints.Count) + (S22 / (float)listGroup2DataPoints.Count));

            float numerator = (float) Math.Pow((S12 / (float)listGroup1DataPoints.Count) + (S22 / (float)listGroup2DataPoints.Count), 2.0f);
            float denominator = (float)Math.Pow((S12 / (float)listGroup1DataPoints.Count), 2.0f) / ((float)listGroup1DataPoints.Count - 1.0f)
                                +(float)Math.Pow((S22 / (float)listGroup2DataPoints.Count), 2.0f) / ((float)listGroup2DataPoints.Count - 1.0f);
            Vn = numerator / denominator;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace RealTimeGraph
{
    class Biofeedback_P1
    {
        List<float> normalDriveOriginalSignal = new List<float>();
        List<float> normalDriveWindowSignal = new List<float>();
        const int WINDOWSIZE = 30;

        float lcl = 0;
        public float LCL
        {
            get
            {
                return lcl;
            }
            set
            {
                lcl = value;
            }
        }

        float ucl = 0;
        public float UCL
        {
            get
            {
                return ucl;
            }
            set
            {
                ucl = value;
            }
        }

        string subjectName;
        public string SubjectName
        {
            get
            {
                return subjectName;
            }
            set
            {
                subjectName = value;
            }
        }

        public bool ReadNormalDriveSignal(string fileName)
        {
            string normalFilename = null;
            if (fileName.Contains("Cognitive"))
            {
                normalFilename = fileName.Replace("Cognitive", "Normal");
            }
            else if (fileName.Contains("Motoric"))
            {
                normalFilename = fileName.Replace("Motoric", "Normal");
            }
            else if (fileName.Contains("Final"))
            {
                normalFilename = fileName.Replace("Final", "Normal");
            }
            else if (fileName.Contains("Failure"))
            {
                normalFilename = fileName.Replace("Failure", "Normal");
            }

            normalFilename += ".pp";
            if (!File.Exists(normalFilename))
            {
                return false;
            }

            SubjectName = Path.GetFileNameWithoutExtension(normalFilename);
            int startIndex = SubjectName.IndexOf("Subject");
            int endIndex = SubjectName.IndexOf("_");
            SubjectName = SubjectName.Substring(startIndex, endIndex - startIndex);

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;

            int rCnt = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(normalFilename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;
            if (range.Rows.Count < 100)
            {
                return false;
            }

            for (rCnt = 3; rCnt <= range.Rows.Count; rCnt++)
            {
                float value = (float)(range.Cells[rCnt, 3] as Excel.Range).Value2;
                normalDriveOriginalSignal.Add(value);
            }

            for (int i = 0; i < normalDriveOriginalSignal.Count - WINDOWSIZE; i++)
            {
                float avg = 0;
                for (int j = i; j < i + WINDOWSIZE; j++)
                {
                    avg += normalDriveOriginalSignal[j];
                }
                avg /= (float)WINDOWSIZE;

                normalDriveWindowSignal.Add(avg);
            }

            xlWorkBook.Close(true, null, null);
            xlApp.Quit();

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            return true;
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }

        public void CalculateTresholds()
        {
            float mean = 0, stdDev = 0;

            for (int i = 0; i < normalDriveWindowSignal.Count; i++)
            {
                mean += normalDriveWindowSignal[i];
            }
            mean /= (float)normalDriveWindowSignal.Count;

            for (int i = 0; i < normalDriveWindowSignal.Count; i++)
            {
                stdDev += (float) Math.Pow(normalDriveWindowSignal[i] - mean, 2);
            }
            stdDev /= normalDriveWindowSignal.Count;
            stdDev = (float) Math.Sqrt(stdDev);

            LCL = mean - 3 * stdDev;
            UCL = mean + 3 * stdDev;

            if (normalDriveWindowSignal.Count > 100)
            {
                int count = 0;
                foreach (float dataPoint in normalDriveWindowSignal)
                {
                    if ((dataPoint < LCL) || (dataPoint > UCL))
                    {
                        count++;
                    }
                }

                if (count > normalDriveWindowSignal.Count / 20)
                {
                    for (int i = 0; i < normalDriveWindowSignal.Count; i++)
                    {
                        float dataPoint = normalDriveWindowSignal[i];
                        if ((dataPoint < LCL) || (dataPoint > UCL))
                        {
                            normalDriveWindowSignal.RemoveAt(i);
                        }
                    }

                    CalculateTresholds();
                }
            }

        }

        public int WindowSize()
        {
            return WINDOWSIZE;
        }
    }
}

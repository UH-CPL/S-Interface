using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PluginInterface
{
    public enum PinCategory
    {
        Critical,
        Optional
    }

    public interface IPlugin
    {
        IPluginHost Host { get; set; }
        int MyID { get; set; }

        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }
        ArrayList InputPins { get; }
        ArrayList OutputPins { get; }

        System.Windows.Forms.UserControl MainInterface { get; }

        void Initialize();
        void Dispose();

        void Process(IPin pin, IPinData input);

    }

    public interface IPluginHost
    {
        IPin LoadOrCreatePin(string name, PinCategory category, Type[] requiredInterfaces);
        void SendData(IPin pin, IPinData data, IPlugin Plugin);
        void SignalCriticalProcessingIsFinished(IPin pin, IPlugin Plugin);
    }

    public interface IPin
    {
        // Methods
        Type[] GetOptionalInterfaces();
        Type[] GetRequiredInterfaces();

        // Properties
        PinCategory Category { get; }
        bool Connected { get; }
        //IFrameFilter Filter { get; }
        string Name { get; }
    }

    public interface IPinData
    {
        // Properties
        //int Cycle { get; }
        string Description { get; }
    }

    public interface IIntegerData : IPinData
    {
        // Properties
        int Value { get; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    public class IntegerData : IIntegerData, IPinData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        protected int value;

        public IntegerData()
        {
        }

        public IntegerData(int setValue)
        {
            value = setValue;
        }

        public int Value
        {
            get
            {
                return value;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Float Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }


    public interface IFloatData : IPinData
    {
        // Properties
        float Value { get; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    public class FloatData : IFloatData, IPinData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        protected float floatvalue;

        public FloatData()
        {
        }

        public FloatData(float setValue)
        {
            floatvalue = setValue;
        }

        public float Value
        {
            get
            {
                return floatvalue;
            }
            set
            {
                floatvalue = value;
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Float Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }





    public interface ILongData : IPinData
    {
        // Properties
        long Value { get; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    public class LongData : ILongData, IPinData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        protected long value;

        public LongData()
        {
        }

        public LongData(long setValue)
        {
            value = setValue;
        }

        public long Value
        {
            get
            {
                return value;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Float Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }


    public interface IStringData : IPinData
    {
        // Properties
        string Data { get; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    public class StringData : IStringData, IPinData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        protected string strData;

        public StringData()
        {
        }

        public StringData(string setValue)
        {
            strData = setValue;
        }

        public string Data
        {
            get
            {
                return strData;
            }
            set
            {
                strData = value;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Float Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }

    public interface IBmpData : IPinData
    {
        // Properties
        Bitmap Value { get; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    public class BmpData : IBmpData, IPinData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        protected Bitmap bitmap;

        public BmpData()
        {
        }

        public BmpData(Bitmap setValue)
        {
            bitmap = setValue;
        }

        public Bitmap Value
        {
            get
            {
                return bitmap;
            }
            set
            {
                bitmap = value;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Float Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }

    public interface IDateTimeData : IPinData
    {
        // Properties
        DateTime Value { get; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    public class DateTimeData : IDateTimeData, IPinData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        protected DateTime value;

        public DateTimeData()
        {
        }

        public DateTimeData(DateTime setValue)
        {
            value = setValue;
        }

        public DateTime Value
        {
            get
            {
                return value;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Float Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }

    public interface IThermalData : IPinData
    {
        // Properties
        float[] Data { get; set; }
        int Height { get; set; }
        int Width { get; set; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    public class ThermalData : IThermalData, IPinData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        protected float[] data;
        protected int height;
        protected int width;

        public ThermalData()
        {
        }

        public ThermalData(float[] setData, int setHeight, int setWidth)
        {
            /*
            data = new float[setData.Length];
            for (int i = 0; i < setData.Length; i++)
            {
                data[i] = setData[i];
            }
             */

            data = setData;

            height = setHeight;
            width = setWidth;
        }

        public void SetFields(float[] setData, int setHeight, int setWidth)
        {
            data = setData;

            height = setHeight;
            width = setWidth;
        }

        public float[] Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }

        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Float Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }


    public interface IImageData : IPinData
    {
        // Properties
        byte[] Data { get; }
        int Height { get; }
        int Width { get; }
        int Scale { get; set; }

    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    public class ImageData : IImageData, IPinData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        protected byte[] data;
        protected int height;
        protected int width;
        protected int scale;

        public ImageData()
        {
        }

        public ImageData(byte[] setData, int setHeight, int setWidth)
        {
            /*
            data = new byte[setData.Length];
            for (int i = 0; i < setData.Length; i++)
            {
                data[i] = setData[i];
            }
             */

            //data = Array.Copy(setData, data, setData.Length);

            data = setData;

            height = setHeight;
            width = setWidth;
            scale = 1;
        }

        public byte[] Data
        {
            get
            {
                return data;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Image Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }


    public interface IBoolData : IPinData
    {
        bool Data { get; }
    }

    public class BoolData : IBoolData
    {
        bool bData;

        public bool Data
        {
            get { return bData; }
            set { bData = value; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Boolean Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

    }


    public interface IFilterData : IPinData
    {
        // Properties
        int Cycle { get; }

    }

    public class FilterData : IFilterData
    {
        int iCycle;
        string strDesc;

        public string Description
        {
            get { return strDesc; }
            set { strDesc = value; }
        }

        public int Cycle
        {
            get { return iCycle; }
            set { iCycle = value; }
        }

    }



    public enum MouseEventCategory { MouseUp = 1, MouseDown, MouseMove, MouseIn, MouseOut };


    public interface IMouseEventData : IFilterData, IPinData
    {
        MouseEventArgs Data { get; }
        MouseEventCategory Category { get; }
        int NormalizedXLocation { get; }
        int NormalizedYLocation { get; }

    }

    public class MouseEventData : FilterData, IMouseEventData
    {
        private MouseEventArgs mData;
        private MouseEventCategory eCat;
        private int mNormalizedXLocation;
        private int mNormalizedYLocation;

        string strDesc;

        public int NormalizedYLocation
        {
            get { return mNormalizedYLocation; }
            set { mNormalizedYLocation = value; }
        }
        public int NormalizedXLocation
        {
            get { return mNormalizedXLocation; }
            set { mNormalizedXLocation = value; }
        }

        public string Description
        {
            get { return strDesc; }
            set { strDesc = value; }
        }

        public MouseEventCategory Category
        {
            get { return eCat; }
            set { eCat = value; }
        }

        public MouseEventArgs Data
        {
            get { return mData; }
            set { mData = value; }
        }
    }


    public interface IKeyEventData : IPinData
    {
        KeyEventArgs Data { get; }

    }

    public class KeyEventData : IKeyEventData
    {
        private KeyEventArgs mData;

        string strDesc;



        public string Description
        {
            get { return strDesc; }
            set { strDesc = value; }
        }



        public KeyEventArgs Data
        {
            get { return mData; }
            set { mData = value; }
        }
    }



    public interface IOverlayData : IPinData
    {
        IPlugin Source { get; }
        IPlugin Destination { get; }
    }

    public class OverlayData : IOverlayData
    {
        IPlugin mSource;
        IPlugin mDestination;
        string strDesc;

        public string Description
        {
            get { return strDesc; }
            set { strDesc = value; }
        }

        public IPlugin Source
        {
            get { return mSource; }
            set { mSource = value; }
        }

        public IPlugin Destination
        {
            get { return mDestination; }
            set { mDestination = value; }
        }
    }

    public interface IRectangleData : IPinData
    {
        double CenterX { get; }

        double CenterY { get; }

        double Height { get; }

        double Rotation { get; }

        double Value { get; }

        double Width { get; }
    }

    public class RectangleData : IRectangleData
    {
        double centerX;
        double centerY;
        double height;
        double rotation;
        double somevalue;
        double width;
        string strDesc;

        public string Description
        {
            get { return strDesc; }
            set { strDesc = value; }
        }

        public double CenterX
        {
            get { return centerX; }
            set { centerX = value; }
        }

        public double CenterY
        {
            get { return centerY; }
            set { centerY = value; }
        }

        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        public double Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public double Value
        {
            get { return somevalue; }
            set { somevalue = value; }
        }

        public double Width
        {
            get { return width; }
            set { width = value; }
        }


    }

    public interface IListPolygonData : IPinData
    {
        void addItem(PolygonData m);
        List<PolygonData> MList { get; }
    }

    public class ListPolygonData : IListPolygonData
    {
        string strDesc;
        List<PolygonData> mList;
        public ListPolygonData()
        {
           mList = new List<PolygonData>();  
        }

        public List<PolygonData> MList 
        {
            get { return mList; }
            set { mList = value; }
        }
        
        
        public void addItem(PolygonData m)
        {
            if (mList == null)
            {
                mList = new List<PolygonData>();
            }
            mList.Add(m);
        }
        public string Description
        {
            get { return strDesc; }
            set { strDesc = value; }
        }
   }

    public interface IPolygonData : IPinData
    {
        Color BorderColor { get; }
        DashStyle Style { get; }
        float Thick { get; }
        PointF[] getVertices();
    }

    public class PolygonData : IPolygonData
    {
        Color mBorderColor;
        DashStyle eStyle;
        float fThick;
        List<PointF> mCoords;
        string strDesc;

        public PolygonData()
        {
            mBorderColor = Color.Red;
            eStyle = DashStyle.Solid;
            fThick = 1.0f;
            mCoords = new List<PointF>();
        }

        public string Description
        {
            get { return strDesc; }
            set { strDesc = value; }
        }

        #region IPolygonData Members

        public Color BorderColor
        {
            get { return mBorderColor; }
            set { mBorderColor = value; }
        }

        public DashStyle Style
        {
            get { return eStyle; }
            set { eStyle = value; }
        }

        public float Thick
        {
            get { return fThick; }
            set { fThick = value; }
        }

        public PointF[] getVertices()
        {
            if (mCoords != null)
                return mCoords.ToArray();
            else
                return null;
        }

        public void addVertice(PointF m)
        {
            /*
            try
            {
                if (mCoords == null)
                {
                    mCoords = new List<PointF>();
                }

                lock (mCoords)
                {
                    mCoords.Add(m);
                }
            }
            catch
            {

            }
            */

            lock (mCoords)
            {
                mCoords.Add(m);
            }
                
        }

        public void removeAt(int i)
        {
            if (mCoords != null)
                mCoords.RemoveAt(i);
        }

        public void clearAll()
        {
            if (mCoords != null)
                mCoords.Clear();
        }

        #endregion
    }


    public interface ITimeSpanData : IPinData
    {
        // Properties
        TimeSpan Span { get; }
    }

    public class TimeSpanData
    {
        TimeSpan mSpan;
        string strDesc;

        public string Description
        {
            get { return strDesc; }
            set { strDesc = value; }
        }

        public TimeSpan Span
        {
            get { return mSpan; }
            set { mSpan = value; }
        }
    }


    public interface IFrameInfo : IPinData
    {
        DateTime Time { get; }
        TimeSpan Span { get; }
        int FrameNumber { get; }
    }

    public class FrameInfo : IFrameInfo
    {
        TimeSpan mSpan;
        DateTime mTime;
        int iFrameNo;
        string strDesc = "Current Frame and Time";

        public TimeSpan Span
        {
            get { return mSpan; }
            set { mSpan = value; }
        }

        public string Description
        {
            get { return strDesc; }
            set { strDesc = value; }
        }

        public DateTime Time
        {
            get { return mTime; }
            set { mTime = value; }
        }

        public int FrameNumber
        {
            get { return iFrameNo; }
            set { iFrameNo = value; }
        }
    }


    public interface IAudioData : IPinData
    {
        float[] Data { get; }
        float FrameNumber { get; }
        float StartIndex { get; }
        float EndIndex { get; }
        float SamplesPerSecond { get; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    public class AudioData : IAudioData, IPinData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        protected float[] data;
        protected float frameNumber;
        protected float startIndex;
        protected float endIndex;
        protected float samplesPerSecond;

        public AudioData(float[] setData, float setFrameNumber, float setStartIndex, float setEndIndex, float setSamplesPerSecond)
        {
            frameNumber = setFrameNumber;

            data = new float[setData.Length];
            for (int i = 0; i < setData.Length; i++)
            {
                data[i] = setData[i];
            }

            startIndex = setStartIndex;
            endIndex = setEndIndex;
            samplesPerSecond = setSamplesPerSecond;
        }

        public float FrameNumber
        {
            get
            {
                return frameNumber;
            }
        }

        public float[] Data
        {
            get
            {
                return data;
            }
        }

        public float StartIndex
        {
            get
            {
                return startIndex;
            }
        }

        public float EndIndex
        {
            get
            {
                return endIndex;
            }
        }

        public float SamplesPerSecond
        {
            get
            {
                return samplesPerSecond;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Audio Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }


    public interface IAudioPlayBackData : IPinData
    {
        long StartIndex { get; }
        long EndIndex { get; }
        bool StartPlayback { get; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    public class AudioPlayBackData : IPinData, IAudioPlayBackData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        protected long startIndex;
        protected long endIndex;
        protected bool startPlayback;

        public AudioPlayBackData(long setStartIndex, long setEndIndex, bool setStartPlayback)
        {
            startIndex = setStartIndex;
            endIndex = setEndIndex;
            startPlayback = setStartPlayback;
        }

        public long StartIndex
        {
            get
            {
                return startIndex;
            }
        }

        public long EndIndex
        {
            get
            {
                return endIndex;
            }
        }

        public bool StartPlayback
        {
            get
            {
                return startPlayback;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Audio Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }


    public struct Conversation
    {
        public long start;
        public long end;
    }

    public interface IQAFileData : IPinData
    {
        ArrayList Questions { get; }
        ArrayList Answers { get; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    public class QAFileData : IPinData, IQAFileData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        ArrayList questions;
        ArrayList answers;

        public QAFileData()
        {
            questions = new ArrayList();
            answers = new ArrayList();
        }

        public ArrayList Questions
        {
            get
            {
                return questions;
            }
        }

        public ArrayList Answers
        {
            get
            {
                return answers;
            }
        }

        public void AddQStart(long FrameNumber)
        {
            Conversation Temp;
            Temp.start = FrameNumber;
            Temp.end = -1;

            questions.Add(Temp);
        }

        public void AddQEnd(long FrameNumber)
        {
            if (questions.Count > 0)
            {
                Conversation temp = ((Conversation)questions[questions.Count - 1]);
                temp.end = FrameNumber;
                questions.RemoveAt(questions.Count - 1);
                questions.Add(temp);
            }
        }

        public void AddAStart(long FrameNumber)
        {
            if (answers.Count < questions.Count)
            {
                if ((((Conversation)questions[answers.Count]).end != -1) && (FrameNumber > ((Conversation)questions[answers.Count]).end))
                {
                    Conversation Temp;
                    Temp.start = FrameNumber;
                    Temp.end = -1;

                    answers.Add(Temp);
                }
            }
            else
            {
                Conversation temp = ((Conversation)answers[answers.Count - 1]);
                temp.end = -1;
                answers.RemoveAt(answers.Count - 1);
                answers.Add(temp);
            }
        }

        public void AddAEnd(long FrameNumber)
        {
            if (answers.Count > 0)
            {
                Conversation temp = ((Conversation)answers[answers.Count - 1]);
                temp.end = FrameNumber;
                answers.RemoveAt(answers.Count - 1);
                answers.Add(temp);
            }
        }

        public void ModifyQStart(int index, long FrameNumber)
        {
            if (index > 0)
            {
                Conversation tempQ = ((Conversation)questions[index]);
                Conversation tempPrevQ = ((Conversation)questions[index - 1]);

                if ((FrameNumber > tempPrevQ.end) && (FrameNumber < tempQ.end))
                {
                    tempQ.start = FrameNumber;

                    questions.RemoveAt(index);
                    questions.Insert(index, tempQ);
                }
            }
            else
            {
                Conversation tempQ = ((Conversation)questions[index]);
                if ((FrameNumber < tempQ.end))
                {
                    tempQ.start = FrameNumber;

                    questions.RemoveAt(index);
                    questions.Insert(index, tempQ);
                }
            }
        }

        public void ModifyQEnd(int index, long FrameNumber)
        {
            if (index < questions.Count - 1)
            {
                Conversation tempQ = ((Conversation)questions[index]);
                Conversation tempNextQ = ((Conversation)questions[index + 1]);

                if ((FrameNumber < tempNextQ.start) && (FrameNumber > tempQ.start))
                {
                    tempQ.end = FrameNumber;

                    questions.RemoveAt(index);
                    questions.Insert(index, tempQ);
                }
            }
            else
            {
                Conversation tempQ = ((Conversation)questions[index]);
                if ((FrameNumber > tempQ.start))
                {
                    tempQ.end = FrameNumber;

                    questions.RemoveAt(index);
                    questions.Insert(index, tempQ);
                }
            }
        }

        public void ModifyAStart(int index, long FrameNumber)
        {
            Conversation tempA = ((Conversation)answers[index]);
            Conversation tempQ = ((Conversation)questions[index]);

            if ((FrameNumber > tempQ.end) && (FrameNumber < tempA.end))
            {
                tempA.start = FrameNumber;
                answers.RemoveAt(index);
                answers.Insert(index, tempA);
            }


        }

        public void ModifyAEnd(int index, long FrameNumber)
        {
            if (index < questions.Count - 1)
            {
                Conversation tempA = ((Conversation)answers[index]);
                Conversation tempQ = ((Conversation)questions[index + 1]);

                if ((FrameNumber < tempQ.start) && (FrameNumber > tempA.start))
                {
                    tempA.end = FrameNumber;

                    answers.RemoveAt(index);
                    answers.Insert(index, tempA);
                }
            }
            else
            {
                Conversation tempA = ((Conversation)answers[index]);
                if (FrameNumber > tempA.start)
                {
                    tempA.end = FrameNumber;

                    answers.RemoveAt(index);
                    answers.Insert(index, tempA);
                }
            }
        }

        public void InsertNewQ(Point qPoint, int Index)
        {

            Conversation temp = new Conversation();
            temp.start = qPoint.X;
            temp.end = qPoint.Y;

            if (Index == questions.Count)
                questions.Add(temp);
            else
                questions.Insert(Index, temp);
        }

        public void ClearAs()
        {
            answers.Clear();
        }

        public void DeleteQAt(int Index)
        {
            questions.RemoveAt(Index);

        }


        public void ClearQs()
        {
            questions.Clear();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Audio Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }


    public interface IQChangedData : IPinData
    {
        bool Trigger { get; }
        int Value { get; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////
    public class QChangedData : IPinData, IQChangedData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        bool trigger;
        int intValue;


        public QChangedData(bool setBoolData, int setIntData)
        {
            trigger = setBoolData;
            intValue = setIntData;
        }

        public bool Trigger
        {
            get
            {
                return trigger;
            }
        }

        public int Value
        {
            get
            {
                return intValue;
            }
        }



        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Audio Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }


    // Interface for vessel data that will be sent to 'PulseEstimation' plugin
    public interface IVesselLineData : IPinData
    {
        // Dynamically growing array to hold the vessel line data at each frame
        ArrayList VesselData { get; }
        // Width of user selected vessel line
        int VesselWidth { get; }
        // Height of user selected vessel line
        int VesselHeight { get; }
        // Total number of frames (M+N) to accumulate the vessel data in the ArrayList 
        int NumFrames { get; }
    }

    // Implementation for the interface IVesselLineData
    ///////////////////////////////////////////////////////////////////////////////////////////
    public class VesselLineData : IVesselLineData
    ///////////////////////////////////////////////////////////////////////////////////////////
    {
        // Members for the implementation
        ArrayList vesselData;
        int vesselWidth;
        int vesselHeight;
        int numFrames;

        // Constructor
        public VesselLineData(int SetVesselWidth, int SetVesselHeight, int SetNumFrames)
        {
            vesselData = new ArrayList();
            vesselWidth = SetVesselWidth;
            vesselHeight = SetVesselHeight;
            numFrames = SetNumFrames;
        }

        //////////////////////////////////////////////////////////////////////////////
        // Properties
        //////////////////////////////////////////////////////////////////////////////

        public ArrayList VesselData
        {
            get
            {
                return vesselData;
            }
        }

        public int VesselWidth
        {
            get
            {
                return vesselWidth;
            }
        }

        public int VesselHeight
        {
            get
            {
                return vesselHeight;
            }
        }

        public int NumFrames
        {
            get
            {
                return numFrames;
            }
        }

        // Function to add the new vesseldata from current frame into the ArrayList
        public void AddVesselData(float[] data)
        {
            // Check if M+N sampels are already collected
            //if (vesselData.Count >= NumFrames)
            //{
            //    // If so, remove the history at frame = 0
            //    vesselData.RemoveAt(0);
            //}

            // Add new vessel data from cuurent frame at the end of the array
            vesselData.Add(data);
        }

        // Function to add the new vesseldata from current frame into the ArrayList
        public void ClearVesselData()
        {
            // Check if M+N sampels are already collected
            //if (vesselData.Count >= NumFrames)
            //{
            //    // If so, remove the history at frame = 0
            //    vesselData.RemoveAt(0);
            //}

            // Add new vessel data from cuurent frame at the end of the array
            vesselData.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPinData Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Description
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            get { return "Vessel Data"; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;
using System.Drawing;
using System.Collections;
using System.Drawing.Imaging;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace Display
{
    [Serializable]
    public class Display : IPlugin, ISerializable
    {

        //int q = 0;

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region IPlugin Required Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        //Declarations of all our internal plugin variables
        string myName = "Display";
        string myDescription = "Show the bitmap on a Picture Box";
        string myAuthor = "Pradeep Buddharaju and Avinash";
        string myVersion = "1.0.0";
        IPluginHost myHost = null;
        int myID = -1;
        System.Windows.Forms.UserControl myMainInterface;
        ArrayList inPins = new ArrayList();
        ArrayList outPins = new ArrayList();
        //public event MouseEventDelegate mouseEventDelegate;


        /// <summary>
        /// Description of the Plugin's purpose
        /// </summary>
        public string Description
        {
            get { return myDescription; }
        }

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author
        {
            get { return myAuthor; }

        }

        /// <summary>
        /// Host of the plugin.
        /// </summary>
        public IPluginHost Host
        {
            get { return myHost; }
            set { myHost = value; }
        }

        /// <summary>
        /// Unique ID.
        /// </summary>
        public int MyID
        {
            get { return myID; }
            set { myID = value; }
        }


        public string Name
        {
            get { return myName; }
        }

        public System.Windows.Forms.UserControl MainInterface
        {
            get { return myMainInterface; }
        }

        public string Version
        {
            get { return myVersion; }
        }

        public ArrayList InputPins
        {
            get { return inPins; }
        }

        public ArrayList OutputPins
        {
            get { return outPins; }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region Threshold Plugin Members
        ///////////////////////////////////////////////////////////////////////////////////////////
        IPin inImage = null;
        IPin inOverlay1 = null;
        IPin inOverlay2 = null;
        IPin inImageOverlay = null;
        IPin outMouseData = null;
        IPin outKeyData = null;
        //Hadi
        IPin outImage = null;
        //Hadi
        //  IMouseEventData mouseData;
        public MouseEventData mouseEventData;
        public KeyEventData keyEventData;
        public int display_height = 1, display_width = 1;
        public int image_height = 1, image_width = 1, scale = 1;
        public bool isImagePresent = false;
        public bool isOverlayPresent = false;
        public bool isFirstOverlayPresent = false;
        public bool isSecondOverlayPresent = false;
        public bool isImageOverlayPresent = false;
        public Bitmap img;
        public Bitmap overlayImage;
        public IPolygonData FirstOverlayDataLocalCopy;
        public IPolygonData SecondOverlayDataLocalCopy;

        public event VoidDelegate CurBitmapChanged;
        public event OverlayDelegate Overlay;



        //   public IMouseEventData mouseEventData
        public System.Windows.Forms.MouseEventArgs MouseEventProperty
        {
            get { return mouseEventData.Data; }
            set
            {

                mouseEventData.Data = value;
                //mouseEventData.Data.Y = (mouseEventData.Data.Y * image_height) / display_height;
                //mouseEventData.Data.X = (mouseEventData.Data.X * image_width) / display_width;
                //default value 
                mouseEventData.Category = MouseEventCategory.MouseDown;
                ////Trigger the event
                //if (ThresholdValChanged != null)
                //{
                //    ThresholdValChanged(thresholdval);
                //}
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

        public void Initialize()
        {
            //This is the first Function called by the host...
            //Put anything needed to start with here first

            inImage = Host.LoadOrCreatePin("Image", PinCategory.Critical, new Type[] { typeof(IImageData) });
            inPins.Add(inImage);
            inOverlay1 = Host.LoadOrCreatePin("Overlay 1", PinCategory.Optional, new Type[] { typeof(IPolygonData) });
            inPins.Add(inOverlay1);
            inOverlay2 = Host.LoadOrCreatePin("Overlay 2", PinCategory.Optional, new Type[] { typeof(IPolygonData) });
            inPins.Add(inOverlay2);
            inImageOverlay = Host.LoadOrCreatePin("Image Overlay", PinCategory.Optional, new Type[] { typeof(IImageData) });
            inPins.Add(inImageOverlay);

            outMouseData = Host.LoadOrCreatePin("Mouse", PinCategory.Optional, new Type[] { typeof(IMouseEventData) });
            outPins.Add(outMouseData);
            outKeyData = Host.LoadOrCreatePin("Key", PinCategory.Optional, new Type[] { typeof(IKeyEventData) });
            outPins.Add(outKeyData);
            //Hadi
            outImage = Host.LoadOrCreatePin("Image", PinCategory.Optional, new Type[] { typeof(IBmpData) });
            outPins.Add(outImage);
            //Hadi/
            myMainInterface = new DisplayControl(this);
            //  public event MouseEventDelegate mouseEventDelegate;
            mouseEventData = new MouseEventData();
            keyEventData = new KeyEventData();
            //img = new Bitmap();
            //myMainInterface.mouseEventDelegate += new MouseEventDelegate(OnMouseEventNotify);

        }


        //public void OnMouseEventNotify(System.Windows.Forms.MouseEventArgs e)
        //{
        //    //make sure that this method is only called by the native thread
        //    if (InvokeRequired)
        //    {
        //        Invoke(new MouseEventDelegate(OnMouseEventNotify), e);

        //        return;
        //    }
        //    mouseEventData = e;

        //}

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }

        public void Process(IPin pin, IPinData input)
        {
            //Put process code here

            if (pin == inImage)
            {


                if (input != null)
                {
                    ImageData imgData = (ImageData)input;
                    Bitmap bmp = GetBitmap(imgData.Data, imgData.Width, imgData.Height);
                    if ((imgData.Width < 5) || (imgData.Height < 5))
                    {
                        isFirstOverlayPresent = false;
                        isSecondOverlayPresent = false;
                    }
                    if (bmp != null)
                    {
                        img = bmp;
                        CurBitmapChanged();
                        scale = imgData.Scale;
                        image_height = imgData.Height * scale;
                        image_width = imgData.Width * scale;
                        isImagePresent = true;
                        SendImage(img);
                    }
                }
                else
                {
                    //img = null;
                    //CurBitmapChanged();
                }

                this.Host.SignalCriticalProcessingIsFinished(inImage, this);
            }
            if (pin == inImageOverlay)
            {


                if (input != null)
                {
                    ImageData imgData = (ImageData)input;
                    overlayImage = OverlayBitmap(imgData.Data, imgData.Width, imgData.Height);
                    isImageOverlayPresent = true;
                    CurBitmapChanged();

                    //scale = imgData.Scale;
                    //image_height = imgData.Height * scale;
                    //image_width = imgData.Width * scale;

                }

                //this.Host.SignalCriticalProcessingIsFinished(inImage, this);
            }
            if (pin == inOverlay1)
            {
                if (input != null)
                {
                    IPolygonData overlayData = (IPolygonData)input;
                    isFirstOverlayPresent = true;
                    //isOverlayPresent = true;
                    Overlay(overlayData);
                    //  Pen pen = new Pen(overlayData.BorderColor,overlayData.Thick);
                    ////  Graphics g = MainInterface.pi  //myMainInterface.picBoxDisplay.CreateGraphics();// myMainInterface.CreateGraphics();
                    //  g.DrawPolygon(pen, (PointF[])overlayData.getVertices());                 

                }
                //this.Host.SignalCriticalProcessingIsFinished(inOverlay1, this);
            }
            if (pin == inOverlay2)
            {
                if (input != null)
                {
                    IPolygonData overlayData = (IPolygonData)input;
                    isSecondOverlayPresent = true;
                    //isOverlayPresent = true;
                    Overlay(overlayData);
                    //Pen pen = new Pen(overlayData.BorderColor, overlayData.Thick);
                    //Graphics g = myMainInterface.CreateGraphics();
                    //g.DrawPolygon(pen, (PointF[])overlayData.getVertices());


                }
                //this.Host.SignalCriticalProcessingIsFinished(inOverlay2, this);

            }
            //MouseEventData.Category = MouseEventCategory.MouseDown;
            //MouseEventData.Data = MouseEvent; 
            //this.Host.SendData(outMouseData, (IMouseEventData)MouseEventProperty, this);
            //////////////this.Host.SendData(outMouseData, (IMouseEventData)mouseEventData, this);
            //SendMouseEventData();
            //this.Host.SignalCriticalProcessingIsFinished(inImage, this);

        }
        public void SendMouseEventData()
        {
            if (outMouseData.Connected == true && mouseEventData != null && isImagePresent == true)
            {
                //mouseEventData.NormalizedXLocation = mouseEventData.Data.X;
                //mouseEventData.NormalizedYLocation = mouseEventData.Data.Y;
                mouseEventData.NormalizedXLocation = (int)(mouseEventData.Data.X * image_width) / display_width;
                mouseEventData.NormalizedYLocation = (int)(mouseEventData.Data.Y * image_height) / display_height;
                this.Host.SendData(outMouseData, (IMouseEventData)mouseEventData, this);
                //this.Host.SignalCriticalProcessingIsFinished(inImage, this);
            }
        }


        public void SendKeyEventData()
        {
            if (outKeyData.Connected == true && keyEventData != null)
            {

                this.Host.SendData(outKeyData, keyEventData, this);

            }
        }


        //Hadi
        public void SendImage(Bitmap bmp)
        {
            this.Host.SendData(this.outImage, new BmpData(bmp), this);
        }
        //Hadi

        public Bitmap GetBitmap(byte[] imageArray, int Width, int Height)
        {

            Bitmap bmp = new Bitmap(Width, Height);
            System.Drawing.Imaging.BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

            //set the number of bytes per pixel
            int pixelSize = 4;

            unsafe
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    //get the data from the original image
                    byte* Row = (byte*)bmd.Scan0 +
                       (y * bmd.Stride);

                    for (int x = 0; x < bmd.Width; x++)
                    {
                        //set the image's pixel channels
                        Row[x * pixelSize] = imageArray[y * (Width * pixelSize) + (x * pixelSize + 2)]; //B
                        Row[x * pixelSize + 1] = imageArray[y * (Width * pixelSize) + (x * pixelSize + 1)]; //G
                        Row[x * pixelSize + 2] = imageArray[y * (Width * pixelSize) + (x * pixelSize)]; //R
                        Row[x * pixelSize + 3] = imageArray[y * (Width * pixelSize) + (x * pixelSize + 3)]; //A
                    }
                }
            }

            //unlock the bitmaps
            bmp.UnlockBits(bmd);

            return bmp;
        }

        public Bitmap OverlayBitmap(byte[] imageArray, int Width, int Height)
        {
            //if(Width <= 10)
            Bitmap bmp = new Bitmap(Width, Height);
            System.Drawing.Imaging.BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

            //set the number of bytes per pixel
            int pixelSize = 4;

            unsafe
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    //get the data from the original image
                    byte* Row = (byte*)bmd.Scan0 +
                       (y * bmd.Stride);

                    for (int x = 0; x < bmd.Width; x++)
                    {
                        //set the image's pixel channels
                        Row[x * pixelSize] = imageArray[y * (Width * pixelSize) + (x * pixelSize + 2)]; //B
                        Row[x * pixelSize + 1] = imageArray[y * (Width * pixelSize) + (x * pixelSize + 1)]; //G
                        Row[x * pixelSize + 2] = imageArray[y * (Width * pixelSize) + (x * pixelSize)]; //R
                        Row[x * pixelSize + 3] = imageArray[y * (Width * pixelSize) + (x * pixelSize + 3)]; //A
                    }
                }
            }

            //unlock the bitmaps
            bmp.UnlockBits(bmd);

            /*
            Graphics bitmapGraphics = Graphics.FromImage(img);

            bitmapGraphics.DrawImage(bmp, 0, 0);

            bitmapGraphics.Dispose();
            */

            return bmp;

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #region ISerializable Members
        ///////////////////////////////////////////////////////////////////////////////////////////

        public Display()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public Display(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //ThresholdVal = (int)info.GetInt64("title");

        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        ///////////////////////////////////////////////////////////////////////////////////////////
        {
            //info.AddValue("title", ThresholdVal);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////

    }
}

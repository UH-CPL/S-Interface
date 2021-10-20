using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Display
{
    public partial class DisplayControl : UserControl
    {
        Display plugin;
        PointF selectionPoint1;     ///<summary> The first selected point
        PointF selectionPoint2;     ///<summary> The second selected state 
        PointF[] UnNormalizedVerticesFirstOverlay;
        PointF[] UnNormalizedVerticesSecondOverlay;
       //Bitmap img_prev;

        int q = 0;


        public DisplayControl()
        {
            InitializeComponent();          
           
        }
  

        public DisplayControl(Display setPlugin)
        {
            InitializeComponent();
            plugin = setPlugin;
            this.picBoxDisplay.Resize += new EventHandler(picBoxDisplay_Resize);
           // public event MouseEventDelegate mouseEventDelegate;
            plugin.CurBitmapChanged += new VoidDelegate(OnCurBitmapChanged);
            plugin.Overlay += new OverlayDelegate(OnOverlay);
            //this.picBoxDisplay.MouseClick += new MouseEventHandler(picBoxDisplay_MouseClick);
            this.picBoxDisplay.MouseUp += new MouseEventHandler(picBoxDisplay_MouseUp);
            this.picBoxDisplay.MouseDown += new MouseEventHandler(picBoxDisplay_MouseDown);
            this.picBoxDisplay.MouseMove += new MouseEventHandler(picBoxDisplay_MouseMove);
            
            //this.picBoxDisplay.MouseCaptureChanged += new EventHandler(picBoxDisplay_MouseCaptureChanged);
            //this.picBoxDisplay.MouseDoubleClick += new MouseEventHandler(picBoxDisplay_MouseDoubleClick);
            //this.picBoxDisplay.MouseHover += new EventHandler(picBoxDisplay_MouseHover);

        }

        void picBoxDisplay_Resize(object sender, EventArgs e)
        {
            //Control control = (Control)sender;
            if (plugin != null)
            {
                plugin.display_height = picBoxDisplay.Height;
                plugin.display_width = picBoxDisplay.Width;
                //plugin.display_width = control.Size.Width;         
            }  
                                    
        }


      
        Pen pen1 = new Pen(Color.Red,3);


        void picBoxDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e != null)
            //{
            //    mouseEventDelegate(e);
            //}
            //
            //plugin.mouseEventData.Category = MouseEventCategory.MouseDown;
            //throw new NotImplementedException();

            
           // normalized_MouseEvent.
       
            plugin.mouseEventData.Data = e;
            plugin.mouseEventData.Category = MouseEventCategory.MouseDown;
            selectionPoint1.X = e.X;
            selectionPoint1.Y = e.Y;
            //picBoxDisplay.Image = img_prev;
            //picBoxDisplay.Invalidate();
            plugin.SendMouseEventData();
            
            
        }
        void picBoxDisplay_MouseMove(object sender, MouseEventArgs e)
        {
           
            if (e.Button == MouseButtons.Left)
            {
                selectionPoint2.X = e.X;
                selectionPoint2.Y = e.Y;
                PolygonData selectionOverlay = new PolygonData();
                Pen pen1 = new Pen(Color.Red, 3); 
                selectionOverlay.addVertice(new PointF(selectionPoint1.X, selectionPoint1.Y));
                selectionOverlay.addVertice(new PointF(selectionPoint2.X, selectionPoint1.Y));
                selectionOverlay.addVertice(new PointF(selectionPoint2.X, selectionPoint2.Y));
                selectionOverlay.addVertice(new PointF(selectionPoint1.X, selectionPoint2.Y));
            //    picBoxDisplay.Image = img_prev;
                Graphics g = picBoxDisplay.CreateGraphics();
                g.DrawPolygon(pen1, (PointF[])selectionOverlay.getVertices());
                picBoxDisplay.Invalidate();
                //Overlay(selectionOverlay);
                g.Dispose();

                plugin.mouseEventData.Data = e;
                plugin.mouseEventData.Category = MouseEventCategory.MouseMove;
                plugin.SendMouseEventData();

            }
        }

        //private void Overlay(PolygonData selectionOverlay)
        //{
        //    throw new NotImplementedException();
        //}

        void picBoxDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            

        
            plugin.MouseEventProperty = e;
       

            plugin.mouseEventData.Category = MouseEventCategory.MouseUp;
            
            plugin.SendMouseEventData();
        }

    
        public void OnCurBitmapChanged()
        {
            //img_prev = bmp;
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new VoidDelegate(OnCurBitmapChanged));
                return;
            }
            //picBoxDisplay.Image = bmp;
            //img_prev = bmp;
            plugin.display_height = picBoxDisplay.Size.Height;
            plugin.display_width = picBoxDisplay.Size.Width;
            //plugin.img = bmp;
            if (plugin.img != null)
            {
                plotImagewithOverlay();
            }
            else
            {
                if(picBoxDisplay.Image != null)
                {
                    picBoxDisplay.Image = null;
                }
            }
            
        }
        public void OnOverlay(IPolygonData OverlayData)
        {
            
            //make sure that this method is only called by the native thread
            if (InvokeRequired)
            {
                Invoke(new OverlayDelegate(OnOverlay), OverlayData);
                return;
            }
            if (plugin.isFirstOverlayPresent == true)
            {
                plugin.FirstOverlayDataLocalCopy = OverlayData;
                UnNormalizedVerticesFirstOverlay = OverlayData.getVertices();
                for (int i = 0; i < UnNormalizedVerticesFirstOverlay.Length; i++)
                {
                    UnNormalizedVerticesFirstOverlay[i].X = (int)UnNormalizedVerticesFirstOverlay[i].X / plugin.scale;
                    UnNormalizedVerticesFirstOverlay[i].Y = (int)UnNormalizedVerticesFirstOverlay[i].Y / plugin.scale;
                }
                

            }
            if(plugin.isSecondOverlayPresent == true)
            {
                plugin.SecondOverlayDataLocalCopy = OverlayData;
                UnNormalizedVerticesSecondOverlay = OverlayData.getVertices();
                for (int i = 0; i < UnNormalizedVerticesSecondOverlay.Length; i++)
                {
                    UnNormalizedVerticesSecondOverlay[i].X = (int)UnNormalizedVerticesSecondOverlay[i].X / plugin.scale;
                    UnNormalizedVerticesSecondOverlay[i].Y = (int)UnNormalizedVerticesSecondOverlay[i].Y / plugin.scale;
                }
             
            }
            
            plotImagewithOverlay();
            //Hadi
            Bitmap bmp = plugin.img;
            plugin.SendImage(bmp);
            //Hadi/

        }


         void plotImagewithOverlay()
          {
              if (plugin.isFirstOverlayPresent == false && plugin.isSecondOverlayPresent == false)
              {
                  if (plugin.isImageOverlayPresent && plugin.overlayImage != null && plugin.img!=null)
                  {
                      //picBoxDisplay.Image = plugin.overlayImage;

                      Graphics bitmapGraphics = Graphics.FromImage(plugin.img);



                      bitmapGraphics.DrawImage((Image)plugin.overlayImage, 0, 0);
                      picBoxDisplay.Image = plugin.img;
                      bitmapGraphics.Dispose();
                  }
                  else
                  {

                      //picBoxDisplay.SizeMode = PictureBoxSizeMode.StretchImage;
                      if (plugin.img != null)
                      {
                          picBoxDisplay.Image = plugin.img;
                      }
                  }
              }
              else if (plugin.isFirstOverlayPresent == true && plugin.isSecondOverlayPresent == false)
              {


                  ////Pen  pen = new Pen(OverlayData.BorderColor, OverlayData.Thick);
                  //Pen pen = new Pen(plugin.FirstOverlayDataLocalCopy.BorderColor, plugin.FirstOverlayDataLocalCopy.Thick);
                  //// Create a Graphics object that we can use to draw on the bitmap.
                  //Graphics bitmapGraphics = Graphics.FromImage(plugin.img);

                  ////UnNormalizedVerticesFirstOverlay = plugin.FirstOverlayDataLocalCopy.getVertices();
                  ////for (int i = 0; i < UnNormalizedVerticesFirstOverlay.Length; i++)
                  ////{
                  ////    UnNormalizedVerticesFirstOverlay[i].X = (int)UnNormalizedVerticesFirstOverlay[i].X / 2;
                  ////    UnNormalizedVerticesFirstOverlay[i].Y = (int)UnNormalizedVerticesFirstOverlay[i].Y / 2;
                  ////}

                  //PointF[] UnNormalizedVertices = plugin.FirstOverlayDataLocalCopy.getVertices();

                  //bitmapGraphics.DrawPolygon(pen, (PointF[])UnNormalizedVerticesFirstOverlay);

                  //picBoxDisplay.Image = plugin.img;

                  //bitmapGraphics.Dispose();




                  Pen pen = new Pen(plugin.FirstOverlayDataLocalCopy.BorderColor, plugin.FirstOverlayDataLocalCopy.Thick);
                  //Pen pen = new Pen(UnNormalizedVerticesFirstOverlay.bo, plugin.FirstOverlayDataLocalCopy.Thick);


                  Graphics bitmapGraphics = Graphics.FromImage(plugin.img);


                  //PointF[] UnNormalizedVertices = plugin.FirstOverlayDataLocalCopy.getVertices();

                  //bitmapGraphics.DrawPolygon(pen, (PointF[])UnNormalizedVertices);

                  //Hadi: added try and catch block to prevent exception
                  try
                  {
                      bitmapGraphics.DrawPolygon(pen, (PointF[])UnNormalizedVerticesFirstOverlay);
                  }
                  catch (OverflowException e)
                  { 
                  }
                  //Hadi/

                  picBoxDisplay.Image = plugin.img;
                  bitmapGraphics.Dispose();
                  
                 
              }
              else if (plugin.isFirstOverlayPresent == false && plugin.isSecondOverlayPresent == true)
              {

                  ////Pen  pen = new Pen(OverlayData.BorderColor, OverlayData.Thick);
                  //Pen pen = new Pen(plugin.SecondOverlayDataLocalCopy.BorderColor, plugin.SecondOverlayDataLocalCopy.Thick);
                  //// Create a Graphics object that we can use to draw on the bitmap.
                  //Graphics bitmapGraphics = Graphics.FromImage(plugin.img);

                  ////PointF[] UnNormalizedVertices = plugin.SecondOverlayDataLocalCopy.getVertices();
                  //UnNormalizedVerticesSecondOverlay = plugin.SecondOverlayDataLocalCopy.getVertices();
                  //for (int i = 0; i < UnNormalizedVerticesSecondOverlay.Length; i++)
                  //{
                  //    UnNormalizedVerticesSecondOverlay[i].X = (int)UnNormalizedVerticesSecondOverlay[i].X / 2;
                  //    UnNormalizedVerticesSecondOverlay[i].Y = (int)UnNormalizedVerticesSecondOverlay[i].Y / 2;
                  //}
                  
                  //bitmapGraphics.DrawPolygon(pen, (PointF[])UnNormalizedVerticesSecondOverlay);
                  
                  //picBoxDisplay.Image = plugin.img;
                
                  //bitmapGraphics.Dispose();
                  ////plugin.isSecondOverlayPresent = false;

                  ////g.Dispose();



                  Pen pen = new Pen(plugin.SecondOverlayDataLocalCopy.BorderColor, plugin.SecondOverlayDataLocalCopy.Thick);
                  //Pen pen = new Pen(UnNormalizedVerticesSecondOverlay.bo, plugin.SecondOverlayDataLocalCopy.Thick);


                  Graphics bitmapGraphics = Graphics.FromImage(plugin.img);


                  //PointF[] UnNormalizedVertices = plugin.SecondOverlayDataLocalCopy.getVertices();

                  //bitmapGraphics.DrawPolygon(pen, (PointF[])UnNormalizedVertices);
                  bitmapGraphics.DrawPolygon(pen, (PointF[])UnNormalizedVerticesSecondOverlay);

                  picBoxDisplay.Image = plugin.img;
                  bitmapGraphics.Dispose();

              }
              else if (plugin.isFirstOverlayPresent == true && plugin.isSecondOverlayPresent == true)
              {

                  ////Pen  pen = new Pen(OverlayData.BorderColor, OverlayData.Thick);
                  //Pen pen1 = new Pen(plugin.FirstOverlayDataLocalCopy.BorderColor, plugin.FirstOverlayDataLocalCopy.Thick);
                  //Pen pen2 = new Pen(plugin.SecondOverlayDataLocalCopy.BorderColor, plugin.SecondOverlayDataLocalCopy.Thick);
                  //// Create a Graphics object that we can use to draw on the bitmap.
                  //Graphics bitmapGraphics = Graphics.FromImage(plugin.img);

                  //PointF[] FirstUnNormalizedVertices = plugin.FirstOverlayDataLocalCopy.getVertices();
                  //PointF[] SecondUnNormalizedVertices = plugin.SecondOverlayDataLocalCopy.getVertices();
                  //bitmapGraphics.DrawPolygon(pen1, (PointF[])FirstUnNormalizedVertices);
                  //bitmapGraphics.DrawPolygon(pen2, (PointF[])SecondUnNormalizedVertices);
                  //picBoxDisplay.Image = plugin.img;

                  //bitmapGraphics.Dispose();



                  
                  Pen pen1 = new Pen(plugin.FirstOverlayDataLocalCopy.BorderColor, plugin.FirstOverlayDataLocalCopy.Thick);
                  Pen pen2 = new Pen(plugin.SecondOverlayDataLocalCopy.BorderColor, plugin.SecondOverlayDataLocalCopy.Thick);
                  //Pen pen = new Pen(UnNormalizedVerticesSecondOverlay.bo, plugin.SecondOverlayDataLocalCopy.Thick);


                  Graphics bitmapGraphics = Graphics.FromImage(plugin.img);


                  //PointF[] UnNormalizedVertices = plugin.SecondOverlayDataLocalCopy.getVertices();

                  //bitmapGraphics.DrawPolygon(pen, (PointF[])UnNormalizedVertices);
                  bitmapGraphics.DrawPolygon(pen1, (PointF[])UnNormalizedVerticesFirstOverlay);
                  bitmapGraphics.DrawPolygon(pen2, (PointF[])UnNormalizedVerticesSecondOverlay);

                  picBoxDisplay.Image = plugin.img;
                  bitmapGraphics.Dispose();

              }

            
         }

        private void picBoxDisplay_Click(object sender, EventArgs e)
        {

        }

        private void DisplayControl_Load(object sender, EventArgs e)
        {
            //picBoxDisplay.Height = 512;
            //picBoxDisplay.Width = 640;
            plugin.display_height = picBoxDisplay.Height;
            plugin.display_width = picBoxDisplay.Width;
        }

        private void DisplayControl_KeyDown(object sender, KeyEventArgs e)
        {
            //if (plugin.keyEventData != null)
            //{
            //    plugin.keyEventData.Data = e;

            //    plugin.SendKeyEventData();
            //}
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (plugin.keyEventData != null)
            {
                plugin.keyEventData.Data = new KeyEventArgs(keyData);

                plugin.SendKeyEventData();
            }
            return true;
        }
    }
}

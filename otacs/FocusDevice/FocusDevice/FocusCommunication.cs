using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace FocusDevice
{
    public delegate void FloatDelegate(float value);
    public delegate void IntegerDelegate(int value);

    public class FocusCommunication
    {
        // Fields
        private int baudRate = 0x960;
        private int focusValue = 80;
        private int maxFocusValue = 0x3fe;
        private int minFocusValue = 0;
        private Mutex mutex = new Mutex();
        private SerialPort serialPort;
        private double step = 5.0;

        // Events
        public event FloatDelegate PositionChanged;
        public event FloatDelegate PositionUpdated;
        
        // Methods
        public int FocusIn()
        {
            GotoPosition(GetPosition() + this.step);
            return 0;
        }

        public int FocusOut()
        {
            GotoPosition(GetPosition() - this.step);
            return 0;
        }

        double GetMaxFocusPosition()
        {
            return 100.0;
        }

        double GetMinFocusPosition()
        {
            return 0.0;
        }

        double GetPosition()
        {
            return ((((double)(this.focusValue - this.minFocusValue)) / ((double)(this.maxFocusValue - this.minFocusValue))) * 100.0);
        }

        public int GotoPosition(double position)
        {
            if (this.serialPort != null)
            {
                if (position < 0.0)
                {
                    position = 0.0;
                }
                if (position > 1023.0)
                {
                    position = 1023.0;
                }
                if (this.mutex.WaitOne())
                {
                    int num = ((int)((position / 100.0) * (this.maxFocusValue - this.minFocusValue))) + this.minFocusValue;
                    if (num != this.focusValue)
                    {
                        int num3;
                        this.focusValue = num;
                        string text = "P" + this.focusValue.ToString().PadLeft(4, '0');
                        this.serialPort.Write(text);
                        Thread.Sleep(200);
                        byte[] buffer = new byte[100];
                        this.serialPort.ReadTimeout = 500;
                        int num2 = 0;
                        try
                        {
                            num2 = this.serialPort.Read(buffer, 0, 100);
                        }
                        catch (Exception exception)
                        {
                            this.mutex.ReleaseMutex();
                            throw exception;
                        }
                        string str2 = "";
                        for (num3 = 0; num3 < num2; num3++)
                        {
                            str2 = str2 + ((char)buffer[num3]);
                        }
                        if ((str2[str2.Length - 1] != 'A') || (num2 != (text.Length + 1)))
                        {
                            this.PositionUpdated((float)position);
                            this.mutex.ReleaseMutex();
                            return 0;
                        }
                        for (num3 = 0; num3 < text.Length; num3++)
                        {
                            if (str2[num3] != text[num3])
                            {
                                this.PositionUpdated((float)position);
                                this.mutex.ReleaseMutex();
                                return 0;
                            }
                        }
                        if (this.PositionChanged != null)
                        {
                            this.PositionUpdated((float)position);
                            this.PositionChanged((float)(GetPosition()));
                        }
                    }
                    this.PositionUpdated((float)position);
                    this.mutex.ReleaseMutex();
                }
            }
            // this.PositionUpdated((float)position);
            return 0;
        }

        public int Initialize(int port)
        {
            if (this.mutex.WaitOne())
            {
                if (this.serialPort != null)
                {
                    this.serialPort.Close();
                }
                this.serialPort = new SerialPort("COM" + port.ToString(), this.baudRate, Parity.None, 8, StopBits.One);
                try
                {
                    this.serialPort.Open();
                    if (this.serialPort.IsOpen)
                    {
                        this.focusValue = -1;
                        GotoPosition(0.0);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        this.serialPort.Close();
                    }
                    catch
                    {
                    }
                    this.serialPort = null;
                    this.mutex.ReleaseMutex();
                    throw new Exception("Couldn't connect to focus device on COM" + port.ToString());
                }
                this.mutex.ReleaseMutex();
            }
            return 0;
        }





    }
}

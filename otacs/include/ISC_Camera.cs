using System;
using System.Runtime.InteropServices;

namespace ISC_Camera
{
    public class ISCTypes 
    {
      const int ISC_MAX_INTERFACES = 16;
      const int	ISC_MAX_DEVICES	=	80;
      const int ISC_NAME_STR_SIZE	= 64;

      // Default RS232 bit rates
      const int PHOENIX_RS232_RATE = 19200;
      const int MERLIN_RS232_RATE	= 19200;
      const int CUMULUS_RS232_RATE = 19200;
      const int PHOTON_RS232_RATE = 57600;
      const int OMEGA_RS232_RATE = 57600;
      const int ALPHA_NIR_RS232_RATE = 57600;

      // Frame sizes
      const int PHOTON_MAX_FRAME_WIDTH = 320;
      const int PHOTON_MAX_FRAME_HEIGHT = 124;

      const int PHOTON2_MAX_FRAME_WIDTH = 320;
      const int PHOTON2_MAX_FRAME_HEIGHT = 256;

      const int PHOTON3_MAX_FRAME_WIDTH	= 644;
      const int PHOTON3_MAX_FRAME_HEIGHT = 512;

      const int OMEGA_MAX_FRAME_WIDTH = 164;
      const int OMEGA_MAX_FRAME_HEIGHT = 128;

      const int PHOENIX_DEF_FRAME_WIDTH = 320;
      const int PHOENIX_DEF_FRAME_HEIGHT = 256;

      const int PHOENIX_LARGE_MAX_FRAME_WIDTH = 640;
      const int PHOENIX_LARGE_MAX_FRAME_HEIGHT = 512;

      const int PHOENIX_MID_MAX_FRAME_WIDTH = 320;
      const int PHOENIX_MID_MAX_FRAME_HEIGHT = 256;

      const int MERLIN_DEF_FRAME_WIDTH = 320;
      const int MERLIN_DEF_FRAME_HEIGHT = 240;

      const int MERLIN_II_DEF_FRAME_WIDTH = 320;
      const int MERLIN_II_DEF_FRAME_HEIGHT = 240;

      const int MERLIN_QWIP_DEF_FRAME_WIDTH = 320;
      const int MERLIN_QWIP_DEF_FRAME_HEIGHT = 256;

      const int ALPHA_NIR_MAX_FRAME_WIDTH = 320;
      const int ALPHA_NIR_MAX_FRAME_HEIGHT = 256;

      const int CUMULUS_DEF_FRAME_WIDTH = 640;
      const int CUMULUS_DEF_FRAME_HEIGHT = 512;

      public enum isc_Error 
      {
        eOK = 0,
        eNoDev = 1,
        eNoMem = 2,
        eOutOfRange = 3,
        eNotOpen = 4,
        eNoCam = 5,
        eOperationNotSupported = 6,
        eWrongType = 7,
        eAlreadyOpen = 8,
        eWrongSize = 9,
        eIncompatibleVersion = 10,
        eNotConnected = 11,
        eCannotConfigure = 12,
        eUnableToGetInfo = 13,
        eInvalidPointer = 14,
        eCantRead = 15,
        eCantWrite = 16,
        ePartialConfig = 17,
        eNotImplemented = 18,
        eCantConfigComPort = 19,
        eCantOpenUart = 20,
        eCantOpenDev = 21,
        eBufferTooBig = 22,
        eTimeOut = 23,
        ePartialSuccess = 24,
        eResourceLockTimeout = 25,
        eInvalidState = 26,
        eInvalidName = 27,
        eDeviceSpecificCodes = 1000,
        eUndefined = 9999
      }

      public enum isc_CameraType 
      {
        ISC_UNKNOWN = 0,
        ISC_PHOENIX = 1,	// Legacy
        ISC_OMEGA = 2,		// 164 x 128
        ISC_MERLIN_II = 3,	// 320 x 240
        ISC_PHOTON = 4,		// 320 x 124
        ISC_PHOTON2 = 5,	// 320 x 256
        ISC_MERLIN = 6,		// 320 x 240
        ISC_PHOENIX_IMAGING_LARGE = 7,		// 640 x 512
        ISC_PHOENIX_NONIMAGING_LARGE = 8,	// 640 x 512
        ISC_PHOENIX_IMAGING_MID = 9,		// 320 x 256
        ISC_PHOENIX_NONIMAGING_MID = 10,	// 320 x 256
        ISC_ALPHA_NIR = 11,					// 320 x 256
        ISC_MERLIN_QWIP = 12,				// 320 x 256
        ISC_CUMULUS = 13,					// Manual frame-size definition
        ISC_CUMULUS_320_256 = 14,				
        ISC_CUMULUS_640_512 = 15,
        ISC_CUMULUS_1024_1024 = 16,
        ISC_DTS_320_256	= 17,
        ISC_DTS_640_512 = 18,
        ISC_PHOTON3 = 19,		            // 644 x 512
        ISC_GENERIC_CAMERALINK_CAM = 666,	// Custom settings required
        ISC_GENERIC_LVDS_CAM = 777,			// Custom settings required
        ISC_GENERIC_RS422_CAM = 888			// Custom settings required
      }

      public enum isc_Depth 
      {
        eISC_8BIT= 1,
        eISC_12BIT= 2,
        eISC_14BIT = 3,
        eISC_16BIT = 4,
      }

          //    typedef void * isc_Frame;

      public enum isc_IF_Type 
      {
        eISC_VIDEO = 1,
        eISC_CONTROL = 2,
        eISC_CONFIG = 3,
      }

	//	public unsafe struct isc_Config 
	//	{
	//		fixed char if_name[ISC_NAME_STR_SIZE];	// interface name, ex "iPort"
	//		fixed char device_name[ISC_NAME_STR_SIZE];  // holds the name as used by iPORT
	//		int version;	// typical interpretation: 1 byte each for "Major . Minor . Revision . Build"
	//		int baudrate;
	//		ushort sizeX;
	//		ushort sizeY;
	//		isc_Depth pixel_depth;
	//	}
    } // end class ISCTypes

    public class Discovery
    {
        //Discovery interface functions

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetVideoIF(ref string interfaceName, int index);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetControlIF(ref string interfaceName, int index);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetConfigIF(ref string interfaceName, int index);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetIFDevice(string interfaceName, ref string deviceName, int index);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error IsAvailable(string interfaceName, string fxnName);

    //Optional Discovery functions

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error RefreshDeviceList(string interfaceName);
    }

    public class Video
    {
    //Video interface functions

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error OpenVideo(string interfaceName, string deviceName, ref int id);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error CloseVideo(int id);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error SetFrameSize(int id, int channel, short rows, short cols);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetFrameSize(int id, int channel, out short rows, out short cols);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error StartVideo(int id, int channel);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error StopVideo(int id, int channel);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GrabFrame(int id, int channel, IntPtr buf);

    //Optional Video functions

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error SetPixelDepth(int id, int channel, int depth);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetPixelDepth(int id, int channel, ref int depth);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetVideoChannelCount(int id, ref int count);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error InitVideo(int id, int channel, ISCTypes.isc_CameraType camType);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error SetFrameSkipCount(int id, int channel, int skipCount);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetFrameSkipCount(int id, int channel, ref int skipCount);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error SetOffsetX(int id, int channel, int skipCount);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error SetOffsetY(int id, int channel, int skipCount);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetOffsetX(int id, int channel, ref int skipCount);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetOffsetY(int id, int channel, ref int skipCount);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error CustomVideoFxn(int id, ref int p1, ref int p2, IntPtr obj);
    }


    public class Control
    {
    //Command-and-Control interface functions

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error OpenControl(string interfaceName, string deviceName, ref int id);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error CloseControl(int id);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error SetBaudRate(int id, int baudrate);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetBaudRate(int id, ref int baudrate);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error WriteControl(int id, IntPtr buf, ref int byteCount);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error ReadControl(int id, IntPtr buf, ref int byteCount);

    //Optional Command-and-Control functions

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error FlushControl(int id);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetControlChannelCount(int id, ref int count);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error InitControl(int id, int channel, int port, ISCTypes.isc_CameraType camType);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error CustomControlFxn(int id, ref int p1, ref int p2, IntPtr obj);

	[DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
	public static extern ISCTypes.isc_Error AcquireControl(int id);

	[DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
	public static extern ISCTypes.isc_Error ReleaseControl(int id);
	}

    public class Config
    {
    //Configuration interface functions

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error OpenConfig(string interfaceName, string deviceName, ref int id);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error CloseConfig(int id);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetConfigObjSize(int id, ref int size);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error GetConfig(int id, IntPtr buf, ref int bufSize);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error SetConfig(int id, IntPtr buf, ref int bufSize);

    //Optional configuration functions

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
      public static extern ISCTypes.isc_Error InitConfig(int id, ISCTypes.isc_CameraType camType);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error ExportConfig(int id, string filePath);

    [DllImport("ISC_Camera.dll", CharSet=CharSet.Unicode, CallingConvention=CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error ImportConfig(int id, string filePath);
    
    [DllImport("ISC_Camera.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
    public static extern ISCTypes.isc_Error CustomConfigFxn(int id, ref int p1, ref int p2, IntPtr obj);

	[DllImport("ISC_Camera.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
	public static extern ISCTypes.isc_Error SetIP(int id, string ip);

    }

} // end namespace ISC_Camera

using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace iacfUtil
{
	public enum Unit
	{
		iacfuntCounts			= 0,
		iacfuntObjectSignal		= 1,
		iacfuntTemperatureC		= 2,
		iacfuntTemperatureK		= 3,
		iacfuntTemperatureF		= 4,
		iacfuntTemperatureR		= 5,
	};

	[StructLayout(LayoutKind.Explicit, Size=32)]
	public struct ObjectParameters
	{
		[FieldOffset(0)]	public float	emissivity;
		[FieldOffset(4)]	public float	distance;
		[FieldOffset(8)]	public float	reflectedTemp;
		[FieldOffset(12)]	public float	atmosphereTemp;
		[FieldOffset(16)]	public float	extOpticsTemp;
		[FieldOffset(20)]	public float	extOpticsTransmission;
		[FieldOffset(24)]	public float	estAtmosphericTransmission;
		[FieldOffset(28)]	public float	relativeHumidity;

		public void Default()
		{
			Calib.iacfCalibDefaultObjectParameters(out this);
		}
	};

	public struct CalibrationInfo
	{
		public string	name;
		public string	lens;
		public string	filter;
		public double	tmin;
		public double	tmax;
	};

	public class CalibData
	{
		internal IntPtr	data;

		public CalibData()
		{
			data = Calib.iacfCalibAlloc();
		}

		~CalibData()
		{
			Calib.iacfCalibFree(data);
		}

        public bool setData(byte[] data)
        {
            if (data.Length != Calib.iacfCalibSize())
                return false;

            Marshal.Copy(data, 0, this.data, data.Length);

            return true;
        }


        public byte[] getData()
        {
            uint size = Calib.iacfCalibSize();
            byte[] ret = new byte[size];

            Marshal.Copy(data, ret, 0, (int)size);

            return ret;
        }
	};

	public class Calib
	{
		private int		id;

		public void Init(int id)
		{
			this.id = id;

			IACFSDK.SDKBase.checkError(iacfCalibInit(id));
		}

		public void Fini()
		{
			int		ret = iacfCalibFini(id);
			id = -1;

			IACFSDK.SDKBase.checkError(ret);
		}

		public bool IsFactoryCalibrated
		{
			get
			{
				byte	b;

				IACFSDK.SDKBase.checkError(iacfCalibIsFactoryCalibrated(id, out b));

				return (b != 0);
			}
		}

		public uint Count
		{
			get
			{
				uint		n;

				IACFSDK.SDKBase.checkError(iacfCalibGetCalibrationCount(id, out n));

				return n;
			}
		}

		public CalibrationInfo this[uint i]
		{
			get
			{
				CalibrationInfo		ret = new CalibrationInfo();
				IntPtr				pname, plens, pfilter;

				IACFSDK.SDKBase.checkError(iacfCalibGetCalibrationInfo(id, i, out pname, out plens, out pfilter, out ret.tmin, out ret.tmax));

				ret.name = Marshal.PtrToStringAnsi(pname);
				ret.lens = Marshal.PtrToStringAnsi(plens);
				ret.filter = Marshal.PtrToStringAnsi(pfilter);

				return ret;
			}
		}

		public int GetActiveCalibration(byte preset)
		{
			int		ret;

			IACFSDK.SDKBase.checkError(iacfCalibGetActiveCalibration(id, preset, out ret));

			return ret;
		}

		public void SetActiveCalibration(byte preset, int cal)
		{
			SetActiveCalibration(preset, cal, true);
		}

		public void SetActiveCalibration(byte preset, int cal, bool loadDefault)
		{
			byte	ld = (loadDefault) ? (byte)1 : (byte)0;

			IACFSDK.SDKBase.checkError(iacfCalibSetActiveCalibration(id, preset, cal, ld));
		}

		public CalibData GetCalibration(byte preset)
		{
			CalibData	ret = new CalibData();

			IACFSDK.SDKBase.checkError(iacfCalibGetCalibration(id, preset, ret.data));
			
			return ret; 
		}

		public void Apply(CalibData calib, ref ushort[] frame, ushort width, ushort height, Unit unit, ref ObjectParameters objPars, ref float[] buf)
		{
			GCHandle	gchFrame = GCHandle.Alloc(frame, GCHandleType.Pinned);
			GCHandle	gchBuf = GCHandle.Alloc(buf, GCHandleType.Pinned);
			int			ret;

			ret = iacfCalibApply(calib.data, gchFrame.AddrOfPinnedObject(), width, height, unit, ref objPars, gchBuf.AddrOfPinnedObject(), (uint)buf.Length*4);

			gchFrame.Free();
			gchBuf.Free();

			IACFSDK.SDKBase.checkError(ret);
		}

		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern int iacfCalibInit(int id);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern int iacfCalibFini(int id);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern int iacfCalibIsFactoryCalibrated(int id, out byte calibrated);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern int iacfCalibGetCalibrationCount(int id, out uint n);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern int iacfCalibGetCalibrationInfo(int id, uint i, out IntPtr name, out IntPtr lens, out IntPtr filter, out double tmin, out double tmax);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern int iacfCalibGetActiveCalibration(int id, byte preset, out int cal);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern int iacfCalibSetActiveCalibration(int id, byte preset, int cal, byte loadDefault);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern IntPtr iacfCalibAlloc();
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern void iacfCalibFree(IntPtr calib);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern UInt32 iacfCalibSize();
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern int iacfCalibGetCalibration(int id, byte preset, IntPtr calib);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern int iacfCalibApply(IntPtr calib, IntPtr frame, ushort width, ushort height, Unit unit, ref ObjectParameters objPar, IntPtr buf, uint bufSizeInBytes);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern void iacfCalibDefaultObjectParameters(out ObjectParameters objPar);
	}

	public class File
	{
		private static IntPtr	NullPtr = (IntPtr)0;
		private IntPtr			handle = NullPtr;

		~File()
		{
			Close();
		}

		public bool Open(string fname)
		{
			Close();

			handle = iacfFileOpen(fname);

			return (handle != NullPtr);
		}

		public bool Create(string fname, ushort width, ushort height, CalibData calib)
		{
			Close();

			if (calib == null)
				handle = iacfFileCreate(fname, width, height, NullPtr);
			else
				handle = iacfFileCreate(fname, width, height, calib.data);

			return (handle != NullPtr);
		}

		public void Close()
		{
			if (handle != NullPtr)
				iacfFileClose(handle);

			handle = NullPtr;
		}

		public bool Write(ref ushort[] frame)
		{
			GCHandle	gchFrame = GCHandle.Alloc(frame, GCHandleType.Pinned);
			byte		ret;

			ret = iacfFileWrite(handle, gchFrame.AddrOfPinnedObject());

			gchFrame.Free();

			return (ret == 0) ? false : true;
		}

		public ObjectParameters	ObjectParameters
		{
			get
			{
				ObjectParameters	ret;

				iacfFileGetObjectParameters(handle, out ret);

				return ret;
			}
			set
			{
				iacfFileSetObjectParameters(handle, ref value);
			}
		}

		public bool Info(out UInt16 width, out UInt16 height, out UInt32 numFrames, out bool calibrated)
		{
			byte	b;
			byte	r;

			r = iacfFileInfo(handle, out width, out height, out numFrames, out b);

			calibrated = (b == 0) ? false : true;

			return (r == 0) ? false : true;
		}

		public bool Read(uint frameNumber, ref ushort[] frame, out ulong timestamp)
		{
			GCHandle	gchFrame = GCHandle.Alloc(frame, GCHandleType.Pinned);
			byte		ret;

			ret = iacfFileRead(handle, frameNumber, Unit.iacfuntCounts, gchFrame.AddrOfPinnedObject(), (uint)frame.Length*2, out timestamp);

			gchFrame.Free();

			return (ret == 0) ? false : true;
		}

		public bool Read(uint frameNumber, Unit unit, ref float[] frame, out ulong timestamp)
		{
			timestamp = 0;

			if (unit == Unit.iacfuntCounts)
				return false;

			GCHandle	gchFrame = GCHandle.Alloc(frame, GCHandleType.Pinned);
			byte		ret;

			ret = iacfFileRead(handle, frameNumber, unit, gchFrame.AddrOfPinnedObject(), (uint)frame.Length*4, out timestamp);

			gchFrame.Free();

			return (ret == 0) ? false : true;
		}

		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern IntPtr iacfFileOpen(string fname);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern IntPtr iacfFileCreate(string fname, ushort width, ushort height, IntPtr calib);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern byte iacfFileWrite(IntPtr file, IntPtr frame);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern byte iacfFileSetObjectParameters(IntPtr file, ref ObjectParameters objPar);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern byte iacfFileGetObjectParameters(IntPtr file, out ObjectParameters objPar);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern byte iacfFileInfo(IntPtr file, out UInt16 width, out UInt16 height, out UInt32 numFrames, out byte calibrated);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern byte iacfFileRead(IntPtr file, uint frameNumber, Unit unit, IntPtr buf, uint bufSizeInBytes, out ulong timestamp);
		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern void iacfFileClose(IntPtr file);
	}

	public class Misc
	{
		public static bool SaveTIFF(string fname, ushort width, ushort height, ref ushort[] frame, uint off)
		{
			GCHandle	gchFrame = GCHandle.Alloc(frame, GCHandleType.Pinned);
			byte		ret;

			ret = iacfSaveTIFF(fname, width, height, gchFrame.AddrOfPinnedObject(), off);

			gchFrame.Free();

			return (ret == 0) ? false : true;
		}

		[DllImport("iacfUtil.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		public static extern byte iacfSaveTIFF(string fname, ushort width, ushort height, IntPtr data, uint off);
	}
}

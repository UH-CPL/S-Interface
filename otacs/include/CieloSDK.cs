using System;
using System.Runtime.InteropServices;

namespace IACFSDK
{
	/// <summary>
	/// SDK for communicating with cielo.
	/// </summary>
	public class CieloSDK
	{
		private int						id = -1;

		/// <summary>
		/// ID of the IndigoSDK communications channel, must be set before calling functions.
		/// </summary>
		public int ID
		{
			get
			{
				return id;
			}
			set
			{
				if (id != value)
				{
					cieloConfigFini(id);
					cieloConfigInit(value);
				}

				id = value;
				hostPort.id = id;
				cameraPort.id = id;
				uart1.id = id;
				uart2.id = id;
				phoenixDwellCount.id = id;
				phoenixIntegrationTime.id = id;
				registers.id = id;
				led.id = id;
				serialNumber.id = id;
				irigTime.id = id;
				irigComparator.id = id;
				version.id = id;
				realTimeClock.id = id;
				gpio.id = id;
				gpiad.id = id;
			}
		}

		private static SDKBase.Version	libVersion = new SDKBase.Version();

		/// <summary>
		/// Gets the library version.
		/// </summary>
		public static SDKBase.Version LibraryVersion
		{
			get
			{
				GetLibraryVersion(out libVersion.Major, out libVersion.Minor, out libVersion.Revision, out libVersion.Build);

				return libVersion;
			}
		}

		/// <summary>
		/// Gets the library version.
		/// </summary>
		/// <param name="major">Major.</param>
		/// <param name="minor">Minor (features).</param>
		/// <param name="revision">Revision (bug fixes).</param>
		/// <param name="build">Build number.</param>
		public static void GetLibraryVersion(out ushort major, out ushort minor, out ushort revision, out ushort build)
		{
			SDKBase.checkError(cieloGetLibraryVersion(out major, out minor, out revision, out build));
		}

		/// <summary>
		/// Value to specify for header offset in order to disable the header information.
		/// </summary>
		public const byte	HeaderDisabled = 0xFF;

		/// <summary>
		/// Gets/Sets the byte offset from frame start where cielo inserts it's header.
		/// </summary>
		public byte HeaderOffset
		{
			get
			{
				byte	offset = 0;

				SDKBase.checkError(cieloGetHeaderOffset(id, out offset));

				return offset;
			}
			set
			{
				SDKBase.checkError(cieloSetHeaderOffset(id, value));
			}
		}

		/// <summary>Controls camera link support.</summary>
		public enum CameraLinkSetupType
		{
			/// <summary>Camera link is off.</summary>
			Off				= 0,
			/// <summary>Base mode parallel.</summary>
			BaseParallel	= 1,
			/// <summary>Base mode serial.</summary>
			BaseSerial		= 2,
			/// <summary>Full mode parallel.</summary>
			FullParallel	= 3
		}

		/// <summary>
		/// Controls camera link support.
		/// </summary>
		public CameraLinkSetupType CameraLinkSetup
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetCameraLinkSetup(id, out val));

				return (CameraLinkSetupType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetCameraLinkSetup(id, (byte)value));
			}
		}

		/// <summary>Controls the video mux.</summary>
		public enum VideoMuxSetupType
		{
			/// <summary>Parallel 14-bit image data.</summary>
			Parallel14bit		= 0,
			/// <summary>Serial image data.</summary>
			Serial				= 1,
			/// <summary>Pattern generator image data.</summary>
			PatternGenerator	= 2,
			/// <summary>Only insert telemetry (header)</summary>
			TelemetryOnly		= 3
		}

		/// <summary>
		/// Controls the video mux.
		/// </summary>
		public VideoMuxSetupType VideoMuxSetup
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetVideoMuxSetup(id, out val));

				return (VideoMuxSetupType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetVideoMuxSetup(id, (byte)value));
			}
		}

		/// <summary>Controls the pattern generator.</summary>
		public enum PatternGeneratorModeType
		{
			/// <summary>Static unmoving pattern.</summary>
			Static			= 0,
			/// <summary>Dynamic moving pattern.</summary>
			Dynamic			= 1,
			/// <summary>Static pattern plus CCMT.</summary>
			Static_CCMT		= 2,
			/// <summary>Dynamic pattern plus CCMT.</summary>
			Dynamic_CCMT	= 3,
			/// <summary>CCMT flag OR'd with static and dynamic.</summary>
			CCMT			= 0x02
		}

		/// <summary>
		/// Controls the pattern generator.
		/// </summary>
		public PatternGeneratorModeType PatternGeneratorMode
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetPatternGeneratorMode(id, out val));

				return (PatternGeneratorModeType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetPatternGeneratorMode(id, (byte)value));
			}
		}

		/// <summary>Controls the size of the pattern generator.</summary>
		public enum PatternGeneratorSizeType
		{
			/// <summary>Pattern is 640x512.</summary>
			size640x512			= 0,
			/// <summary>Pattern is 320x256.</summary>
			size320x256			= 1
		}

		/// <summary>
		/// Controls the size of the pattern generator image.
		/// </summary>
		public PatternGeneratorSizeType PatternGeneratorSize
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetPatternGeneratorSize(id, out val));

				return (PatternGeneratorSizeType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetPatternGeneratorSize(id, (byte)value));
			}
		}

		/// <summary>
		/// Clock mux types.
		/// </summary>
		public enum ClockMuxType
		{
			/// <summary>Use 25MHz clock (pat gen/parallel).</summary>
			clk25MHz		= 0,
			/// <summary>Use 50MHz clock (pat gen/parallel).</summary>
			clk50MHz		= 1,
			/// <summary>Use 100MHz clock (pat gen/parallel).</summary>
			clk100MHz		= 2,
			/// <summary>Use 204.8MHz clock (pat gen/parallel).</summary>
			clk204MHz		= 3,
			/// <summary>Use 73.68MHz clock (pat gen/serial).</summary>
			clk73MHz		= 4,
			/// <summary>Use 250MHz clock (pat gen/parallel).</summary>
			clk250MHz		= 5,
			/// <summary>Use 320MHz clock (pat gen/parallel).</summary>
			clk320MHz		= 6,
			/// <summary>Use hotlink clock.</summary>
			HotLink			= 7
		}

		/// <summary>
		/// Controls the clock mux.
		/// </summary>
		public ClockMuxType ClockMux
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetClockMux(id, out val));

				return (ClockMuxType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetClockMux(id, (byte)value));
			}
		}

		/// <summary>
		/// Clock sources.
		/// </summary>
		public enum ClockSourceType
		{
			/// <summary>Use video interface input.</summary>
			VideoIN		= 0,
			/// <summary>Use ClockMux value.</summary>
			ClockMux	= 1
		}

		/// <summary>
		/// Get/set clock source.
		/// </summary>
		public ClockSourceType ClockSource
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetClockSource(id, out val));

				return (ClockSourceType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetClockSource(id, (byte)value));
			}
		}

		/// <summary>
		/// Pleora select pin types.
		/// </summary>
		public enum PleoraSelectPinType
		{
			/// <summary>Pleora video data.</summary>
			NormalInput		= 0,
			/// <summary>Pleora test pattern.</summary>
			TestPattern		= 1
		}

		/// <summary>
		/// Get/set pleora select pin (test pattern).
		/// </summary>
		public PleoraSelectPinType PleoraSelectPin
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetPleoraSelectPin(id, out val));

				return (PleoraSelectPinType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetPleoraSelectPin(id, (byte)value));
			}
		}

		/// <summary>
		/// Pleora board power enable.
		/// </summary>
		public bool PleoraPowerEnabled
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetPleoraPowerEnabled(id, out val));

				return (val != 0);
			}
			set
			{
				byte	val = 0;

				if (value)
					val = 1;

				SDKBase.checkError(cieloSetPleoraPowerEnabled(id, val));
			}
		}

		/// <summary>
		/// Hotlink power enable.
		/// </summary>
		public bool HotLinkPowerEnabled
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetHotLinkPowerEnabled(id, out val));

				return (val != 0);
			}
			set
			{
				byte	val = 0;

				if (value)
					val = 1;

				SDKBase.checkError(cieloSetHotLinkPowerEnabled(id, val));
			}
		}

		/// <summary>
		/// UART type/protocol for ports.
		/// </summary>
		public enum UARTType
		{
			/// <summary>Use RS-232.</summary>
			RS232			= 0,
			/// <summary>Use RS-422.</summary>
			RS422			= 1,
			/// <summary>Use RS-485.</summary>
			RS485			= 2,
			/// <summary>Invet TX/RX signal.</summary>
			Invert			= 0x10,
			/// <summary>Use Inverted RS-232.</summary>
			RS232_Invert	= 0x10,
			/// <summary>Use Inverted RS-422.</summary>
			RS422_Invert	= 0x11,
			/// <summary>Use Inverted RS-485.</summary>
			RS485_Invert	= 0x12,
			/// <summary>Don't change.</summary>
			Default			= 0xFF
		}

		/// <summary>
		/// Host side port type.
		/// </summary>
		public enum HostPortType
		{
			/// <summary>Use Pleora.</summary>
			GigE		= 0,
			/// <summary>Use UART.</summary>
			UART		= 1,
			/// <summary>Use CameraLink.</summary>
			CameraLink	= 2,
			/// <summary>Use High Speed Serial Port.</summary>
			HSSP		= 3,
			/// <summary>Use USB.</summary>
			USB			= 4
		}

		/// <summary>
		/// Container class for host port.
		/// </summary>
		public class HostPortData
		{
			/// <summary>AccessoriesSDK control channel id.</summary>
			public int				id = -1;
			private HostPortType	port;
			private UARTType		uart;
			private uint			baud;
			private bool			cache = false;

			private void Read()
			{
				byte			valPort, valUart;

				SDKBase.checkError(cieloGetHostPort(id, out valPort, out baud, out valUart));

				port = (HostPortType)valPort;
				uart = (UARTType)valUart;
			}

			private void Write()
			{
				SDKBase.checkError(cieloSetHostPort(id, (byte)port, baud, (byte)uart));
			}

			/// <summary>
			/// Read values into cache.
			/// </summary>
			public void CacheRead()
			{
				Read();

				cache = true;
			}

			/// <summary>
			/// Write values from cache, clear cache.
			/// </summary>
			public void CacheWrite()
			{
				Write();

				cache = false;
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void CacheClear()
			{
				cache = false;
			}

			/// <summary>
			/// Host Port type.
			/// </summary>
			public HostPortType Port
			{
				get
				{
					if (!cache)
						Read();

					return port;
				}
				set
				{
					if (!cache)
						Read();

					port = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// UART type/protocol.
			/// </summary>
			public UARTType UartMode
			{
				get
				{
					if (!cache)
						Read();

					return uart;
				}
				set
				{
					if (!cache)
						Read();

					uart = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Baud rate in bps.
			/// </summary>
			public uint BaudRate
			{
				get
				{
					if (!cache)
						Read();

					return baud;
				}
				set
				{
					if (!cache)
						Read();

					baud = value;

					if (!cache)
						Write();
				}
			}
		}

		private HostPortData	hostPort = new HostPortData();

		/// <summary>
		/// Gets host port container.
		/// </summary>
		public HostPortData HostPort
		{
			get
			{
				return hostPort;
			}
		}

		/// <summary>
		/// Camera port type.
		/// </summary>
		public enum CameraPortType
		{
			/// <summary>Use phoenix UART / cielo bus.</summary>
			PhoenixUART		= 0,
			/// <summary>Use High Speed Serial Port.</summary>
			HSSP			= 1
		}

		/// <summary>
		/// Contains camera port information.
		/// </summary>
		public class CameraPortData
		{
			/// <summary>AccessoriesSDK control channel id.</summary>
			public int				id = -1;
			private CameraPortType	port;
			private uint			baud;
			private bool			cache = false;

			private void Read()
			{
				byte	valPort;

				SDKBase.checkError(CieloSDK.cieloGetCameraPort(id, out valPort, out baud));

				port = (CameraPortType)valPort;
			}

			private void Write()
			{
				SDKBase.checkError(CieloSDK.cieloSetCameraPort(id, (byte)port, baud));
			}

			/// <summary>
			/// Read values into cache.
			/// </summary>
			public void CacheRead()
			{
				Read();

				cache = true;
			}

			/// <summary>
			/// Write values from cache, clear cache.
			/// </summary>
			public void CacheWrite()
			{
				Write();

				cache = false;
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void CacheClear()
			{
				cache = false;
			}

			/// <summary>
			/// Camera port.
			/// </summary>
			public CameraPortType Port
			{
				get
				{
					if (!cache)
						Read();

					return port;
				}
				set
				{
					if (!cache)
						Read();

					port = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Baud rate of the camera port.
			/// </summary>
			public uint BaudRate
			{
				get
				{
					if (!cache)
						Read();

					return baud;
				}
				set
				{
					if (!cache)
						Read();

					baud = value;

					if (!cache)
						Write();
				}
			}
		}

		private CameraPortData		cameraPort = new CameraPortData();

		/// <summary>
		/// Camera port that cielo will send packets to.
		/// </summary>
		public CameraPortData CameraPort
		{
			get
			{
				return cameraPort;
			}
		}

		/// <summary>
		/// Determines if the camera needs cielo to remove slip framing before forwarding packets to the camera and add slip framing after reciving packets from the camera.
		/// </summary>
		public bool DeframingEnabled
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetDeframingEnabled(id, out val));

				return (val != 0);
			}
			set
			{
				byte	val = 0;

				if (value)
					val = 1;

				SDKBase.checkError(cieloSetDeframingEnabled(id, val));
			}
		}

		/// <summary>
		/// Determines if bypass mode is enabled.  When enabled cielo forwards bytes and does not listen on the host interface.
		/// Note: Bypass for USB is not supported.
		/// </summary>
		public bool BypassEnabled
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetBypassEnabled(id, out val));

				return (val != 0);
			}
			set
			{
				byte	val = 0;

				if (value)
					val = 1;

				SDKBase.checkError(cieloSetBypassEnabled(id, val));
			}
		}

		/// <summary>
		/// Sync in modes.
		/// </summary>
		public enum SyncInModeType
		{
			/// <summary>Use Pass through ttl.</summary>
			PassThroughTTL				= 0,
			/// <summary>Use Inverted pass through ttl.</summary>
			PassThroughTTLInverted		= 1,
			/// <summary>Use opto-isolated pass through.</summary>
			PassThroughOPTO				= 2,
			/// <summary>Use inverted opto-isolzted pass through.</summary>
			PassThroughOPTOInverted		= 3,
			/// <summary>Use IRIG.</summary>
			IRIG						= 4,
			/// <summary>Use Internal sync.</summary>
			Internal					= 5
		}

		/// <summary>
		/// Sync in mode.
		/// </summary>
		public SyncInModeType SyncInMode
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetSyncInMode(id, out val));

				return (SyncInModeType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetSyncInMode(id, (byte)value));
			}
		}

		/// <summary>
		/// Clock in modes.
		/// </summary>
		public enum ClockInModeType
		{
			/// <summary>Use pass through.</summary>
			PassThrough				= 0,
			/// <summary>Use inverted pass through.</summary>
			PassThroughInverted		= 1
		}

		/// <summary>
		/// Clock in mode.
		/// </summary>
		public ClockInModeType ClockInMode
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetClockInMode(id, out val));

				return (ClockInModeType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetClockInMode(id, (byte)value));
			}
		}

		/// <summary>
		/// Sync out modes.
		/// </summary>
		public enum SyncOutModeType
		{
			/// <summary>Use pass through.</summary>
			PassThrough				= 0,
			/// <summary>Use inverted pass through.</summary>
			PassThroughInverted		= 1,
			/// <summary>Use phoenix sync.</summary>
			PhoenixSync				= 2,
			/// <summary>Use inverted phoenix sync.</summary>
			PhoenixSyncInverted		= 3
		}

		/// <summary>
		/// Sync out mode.
		/// </summary>
		public SyncOutModeType SyncOutMode
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetSyncOutMode(id, out val));

				return (SyncOutModeType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetSyncOutMode(id, (byte)value));
			}
		}

		/// <summary>
		/// Clock out modes.
		/// </summary>
		public enum ClockOutModeType
		{
			/// <summary>Use pass through.</summary>
			PassThrough				= 0,
			/// <summary>Use inverted pass through.</summary>
			PassThroughInverted		= 1
		}

		/// <summary>
		/// Clack out mode.
		/// </summary>
		public ClockOutModeType ClockOutMode
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetClockOutMode(id, out val));

				return (ClockOutModeType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetClockOutMode(id, (byte)value));
			}
		}

		/// <summary>
		/// IRIG latch source types.
		/// </summary>
		public enum IRIGLatchSourceType
		{
			/// <summary>Use manual latch.</summary>
			Software			= 0,
			/// <summary>Use camera latch signal.</summary>
			LatchSignal			= 1,
			/// <summary>Use sync in signal.</summary>
			SyncIn				= 2,
			/// <summary>Use frame valid signal.</summary>
			FrameValid			= 3
		}

		/// <summary>IRIG Latch source.</summary>
		public IRIGLatchSourceType IRIGLatchSource
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetIRIGLatchSource(id, out val));

				return (IRIGLatchSourceType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetIRIGLatchSource(id, (byte)value));
			}
		}

		/// <summary>
		/// IRIG Comparator on/off, match signal goes high when match occurs.
		/// </summary>
		public bool IRIGComparatorEnabled
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetIRIGComparatorEnabled(id, out val));

				return (val != 0);
			}
			set
			{
				byte	val = 0;

				if (value)
					val = 1;

				SDKBase.checkError(cieloSetIRIGComparatorEnabled(id, val));
			}
		}

		/// <summary>
		/// Arm the phoenix sequencer.
		/// </summary>
		public bool PhoenixSequencerArmed
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetPhoenixSequencerArmed(id, out val));

				return (val != 0);
			}
			set
			{
				byte	val = 0;

				if (value)
					val = 1;

				SDKBase.checkError(cieloSetPhoenixSequencerArmed(id, val));
			}
		}

		/// <summary>
		/// Exetended first sync option.
		/// </summary>
		public bool PhoenixExtendedFirstSyncEnabled
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetPhoenixExtendedFirstSyncEnabled(id, out val));

				return (val != 0);
			}
			set
			{
				byte	val = 0;

				if (value)
					val = 1;

				SDKBase.checkError(cieloSetPhoenixExtendedFirstSyncEnabled(id, val));
			}
		}

		/// <summary>
		/// Enable phoenix sequencer.
		/// </summary>
		public bool PhoenixSequencerEnabled
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetPhoenixSequencerEnabled(id, out val));

				return (val != 0);
			}
			set
			{
				byte	val = 0;

				if (value)
					val = 1;

				SDKBase.checkError(cieloSetPhoenixSequencerEnabled(id, val));
			}
		}

		/// <summary>
		/// Control signal paths.
		/// </summary>
		public enum CameraControlSignalPathType
		{
			/// <summary>Send camera control signals to/from camera link.</summary>
			CameraLink		= 0,
			/// <summary>Send camera control signals to/from pleora.</summary>
			GigE			= 1
		}

		/// <summary>
		/// Get/set camera control signal path.
		/// </summary>
		public CameraControlSignalPathType CameraControlSignalPath
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetCameraControlSignalPath(id, out val));

				return (CameraControlSignalPathType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetCameraControlSignalPath(id, (byte)value));
			}
		}

		/// <summary>
		/// Contains information on a uart.
		/// </summary>
		public class UARTData
		{
			/// <summary>AccessoriesSDK control channel id.</summary>
			public int			id = -1;
			private uint		baud;
			private UARTType	uart;
			private bool		cache = false;
			private int			uartnum = 0;

			/// <summary>
			/// Standard constructor.
			/// </summary>
			/// <param name="_uart">UART number (1 or 2).</param>
			public UARTData(int _uart)
			{
				uartnum = _uart;
			}

			private void Read()
			{
				byte	valUart;

				if (uartnum == 1)
					SDKBase.checkError(CieloSDK.cieloGetUART1(id, out baud, out valUart));
				else
					SDKBase.checkError(CieloSDK.cieloGetUART2(id, out baud, out valUart));

				uart = (UARTType)valUart;
			}

			private void Write()
			{
				if (uartnum == 1)
					SDKBase.checkError(CieloSDK.cieloSetUART1(id, baud, (byte)uart));
				else
					SDKBase.checkError(CieloSDK.cieloSetUART2(id, baud, (byte)uart));
			}

			/// <summary>
			/// Read values into cache.
			/// </summary>
			public void CacheRead()
			{
				Read();

				cache = true;
			}

			/// <summary>
			/// Write values from cache, clear cache.
			/// </summary>
			public void CacheWrite()
			{
				Write();

				cache = false;
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void CacheClear()
			{
				cache = false;
			}

			/// <summary>
			/// UART type/protocol.
			/// </summary>
			public UARTType UARTMode
			{
				get
				{
					if (!cache)
						Read();

					return uart;
				}
				set
				{
					if (!cache)
						Read();

					uart = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Baud rate of the UART.
			/// </summary>
			public uint BaudRate
			{
				get
				{
					if (!cache)
						Read();

					return baud;
				}
				set
				{
					if (!cache)
						Read();

					baud = value;

					if (!cache)
						Write();
				}
			}
		}

		private UARTData	uart1 = new UARTData(1);
		private UARTData	uart2 = new UARTData(2);

		/// <summary>
		/// Get uart 1 container class.
		/// </summary>
		public UARTData UART1
		{
			get
			{
				return uart1;
			}
		}

		/// <summary>
		/// Get uart 2 container class.
		/// </summary>
		public UARTData UART2
		{
			get
			{
				return uart2;
			}
		}

		/// <summary>
		/// Number of times to sequence.
		/// </summary>
		public ushort PhoenixSequenceCount
		{
			get
			{
				ushort	val;

				SDKBase.checkError(cieloGetPhoenixSequenceCount(id, out val));

				return val;
			}
			set
			{
				SDKBase.checkError(cieloSetPhoenixSequenceCount(id, value));
			}
		}

		/// <summary>Value to OR to dwell count to get the sequencer to move to the next preset</summary>
		public const ushort		NextPreset = 0x8000;

		/// <summary>
		/// Contains dwell count information.
		/// </summary>
		public class PhoenixDwellCountData
		{
			/// <summary>AccessoriesSDK control channel id.</summary>
			public int		id = -1;
			private ushort	dwellPS0, dwellPS1, dwellPS2, dwellPS3;
			private bool	cache = false;

			private void Read()
			{
				SDKBase.checkError(CieloSDK.cieloGetPhoenixDwellCount(id, out dwellPS0, out dwellPS1, out dwellPS2, out dwellPS3));
			}

			private void Write()
			{
				SDKBase.checkError(CieloSDK.cieloSetPhoenixDwellCount(id, dwellPS0, dwellPS1, dwellPS2, dwellPS3));
			}

			/// <summary>
			/// Read values into cache.
			/// </summary>
			public void CacheRead()
			{
				Read();

				cache = true;
			}

			/// <summary>
			/// Write values from cache, clear cache.
			/// </summary>
			public void CacheWrite()
			{
				Write();

				cache = false;
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void CacheClear()
			{
				cache = false;
			}

			/// <summary>
			/// Dwell count for PS0.
			/// </summary>
			public ushort DwellPS0
			{
				get
				{
					if (!cache)
						Read();

					return dwellPS0;
				}
				set
				{
					if (!cache)
						Read();

					dwellPS0 = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Dwell count for PS1.
			/// </summary>
			public ushort DwellPS1
			{
				get
				{
					if (!cache)
						Read();

					return dwellPS1;
				}
				set
				{
					if (!cache)
						Read();

					dwellPS1 = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Dwell count for PS2.
			/// </summary>
			public ushort DwellPS2
			{
				get
				{
					if (!cache)
						Read();

					return dwellPS2;
				}
				set
				{
					if (!cache)
						Read();

					dwellPS2 = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Dwell count for PS3.
			/// </summary>
			public ushort DwellPS3
			{
				get
				{
					if (!cache)
						Read();

					return dwellPS3;
				}
				set
				{
					if (!cache)
						Read();

					dwellPS3 = value;

					if (!cache)
						Write();
				}
			}
		}

		private PhoenixDwellCountData	phoenixDwellCount = new PhoenixDwellCountData();

		/// <summary>
		/// Get phoenix dwell count container class.
		/// </summary>
		public PhoenixDwellCountData PhoenixDwellCount
		{
			get
			{
				return phoenixDwellCount;
			}
		}

		/// <summary>
		/// Contains information on the raw integration time.
		/// </summary>
		public class PhoenixIntegrationTimeData
		{
			/// <summary>AccessoriesSDK control channel id.</summary>
			public int		id = -1;
			private uint	itimePS0, itimePS1, itimePS2, itimePS3;
			private bool	cache = false;

			private void Read()
			{
				SDKBase.checkError(CieloSDK.cieloGetPhoenixIntegrationTime(id, out itimePS0, out itimePS1, out itimePS2, out itimePS3));
			}

			private void Write()
			{
				SDKBase.checkError(CieloSDK.cieloSetPhoenixIntegrationTime(id, itimePS0, itimePS1, itimePS2, itimePS3));
			}

			/// <summary>
			/// Read values int cache.
			/// </summary>
			public void CacheRead()
			{
				Read();

				cache = true;
			}

			/// <summary>
			/// Write values from cache, clear cache.
			/// </summary>
			public void CacheWrite()
			{
				Write();

				cache = false;
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void CacheClear()
			{
				cache = false;
			}

			/// <summary>
			/// Raw (clocks) integration time for PS0.
			/// </summary>
			public uint ITimePS0
			{
				get
				{
					if (!cache)
						Read();

					return itimePS0;
				}
				set
				{
					if (!cache)
						Read();

					itimePS0 = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Raw (clocks) integration time for PS1.
			/// </summary>
			public uint ITimePS1
			{
				get
				{
					if (!cache)
						Read();

					return itimePS1;
				}
				set
				{
					if (!cache)
						Read();

					itimePS1 = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Raw (clocks) integration time for PS2.
			/// </summary>
			public uint ITimePS2
			{
				get
				{
					if (!cache)
						Read();

					return itimePS2;
				}
				set
				{
					if (!cache)
						Read();

					itimePS2 = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Raw (clocks) integration time for PS3.
			/// </summary>
			public uint ITimePS3
			{
				get
				{
					if (!cache)
						Read();

					return itimePS3;
				}
				set
				{
					if (!cache)
						Read();

					itimePS3 = value;

					if (!cache)
						Write();
				}
			}
		}

		private PhoenixIntegrationTimeData	phoenixIntegrationTime = new PhoenixIntegrationTimeData();

		/// <summary>
		/// Get integration time container class.
		/// </summary>
		public PhoenixIntegrationTimeData PhoenixIntegrationTime
		{
			get
			{
				return phoenixIntegrationTime;
			}
		}

		/// <summary>
		/// Send phoenix init commands to camera on startup.
		/// </summary>
		public bool PhoenixInitEnabled
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetPhoenixInitEnabled(id, out val));

				return (val != 0);
			}
			set
			{
				byte	val = 0;

				if (value)
					val = 1;

				SDKBase.checkError(cieloSetPhoenixInitEnabled(id, val));
			}
		}

		/// <summary>
		/// Initialize cielo the configuration on the given id.
		/// </summary>
		public void ConfigInit()
		{
			SDKBase.checkError(cieloConfigInit(id));
		}

		/// <summary>
		/// Finalize the cielo configuration on the given id.
		/// </summary>
		public void ConfigFini()
		{
			SDKBase.checkError(cieloConfigFini(id));
		}

		/// <summary>
		/// Read information and set cache flag, operations on the structure will use the cached version.
		/// </summary>
		public void CacheRead()
		{
			SDKBase.checkError(cieloCacheRead(id));
		}

		/// <summary>
		/// Write cached information to cielo and clear cache flag.
		/// </summary>
		public void CacheWrite()
		{
			SDKBase.checkError(cieloCacheWrite(id));
		}

		/// <summary>
		/// Clear the cache flag.
		/// </summary>
		public void CacheClear()
		{
			SDKBase.checkError(cieloCacheClear(id));
		}

		/// <summary>
		/// Saves the cielo configuration information to the given bank (usually bank 0).
		/// </summary>
		/// <param name="bank">Bank to save to, either 0 or 1, usually 0.</param>
		public void SaveState(byte bank)
		{
			SDKBase.checkError(cieloSaveState(id, bank));
		}

		/// <summary>
		/// Get the value of a register at address.
		/// </summary>
		/// <param name="address">Address of register.</param>
		/// <param name="reg">Register value.</param>
		public void GetReg(uint address, out uint reg)
		{
			SDKBase.checkError(cieloGetReg(id, address, out reg));
		}

		/// <summary>
		/// Set the value of a register at address.
		/// </summary>
		/// <param name="address">Address of register.</param>
		/// <param name="reg">Register value.</param>
		public void SetReg(uint address, uint reg)
		{
			SDKBase.checkError(cieloSetReg(id, address, reg));
		}

		/// <summary>
		/// Contains register shortcuts.
		/// </summary>
		public class RegistersData
		{
			/// <summary>AccessoriesSDK control channel id.</summary>
			public int		id = -1;

			/// <summary>
			/// Get/set register at address.
			/// </summary>
			public uint this[uint address]
			{
				get
				{
					uint	val;

					SDKBase.checkError(CieloSDK.cieloGetReg(id, address, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(CieloSDK.cieloSetReg(id, address, value));
				}
			}
		}

		private	RegistersData	registers = new RegistersData();

		/// <summary>
		/// Get register container class.
		/// </summary>
		public RegistersData Registers
		{
			get
			{
				return registers;
			}
		}

		/// <summary>
		/// Manually latch irig now.
		/// </summary>
		public void LatchIRIG()
		{
			SDKBase.checkError(cieloLatchIRIG(id));
		}

		/// <summary>
		/// Contains led state shortcut.
		/// </summary>
		public class LEDData
		{
			/// <summary>AccessoriesSDK control channel id.</summary>
			public int		id = -1;

			/// <summary>
			/// Set led state at led index.
			/// </summary>
			public bool this[byte led]
			{
				set
				{
					byte	val;

					val = 0;
					if (value)
						val = 1;

					SDKBase.checkError(cieloSetLEDState(id, led, val));
				}
				get
				{
					byte	val;

					SDKBase.checkError(cieloGetLEDState(id, led, out val));

					return (val != 0);
				}
			}
		}

		private LEDData		led = new LEDData();

		/// <summary>
		/// Get LED stat container class.
		/// </summary>
		public LEDData LEDs
		{
			get
			{
				return led;
			}
		}

		/// <summary>
		/// Set led on/off state for a led.
		/// </summary>
		/// <param name="led">LED index 0-5.</param>
		/// <param name="state">State (0=off,1=on).</param>
		public void SetLEDState(byte led, byte state)
		{
			SDKBase.checkError(cieloSetLEDState(id, led, state));
		}

		/// <summary>
		/// Get led on/off state for a led.
		/// </summary>
		/// <param name="led">LED index 0-5.</param>
		/// <param name="state">State (0=off,1=on).</param>
		public void GetLEDState(byte led, out byte state)
		{
			SDKBase.checkError(cieloGetLEDState(id, led, out state));
		}

		/// <summary>
		/// Genlock types.
		/// </summary>
		public enum GenlockType
		{
			/// <summary>Genlock is free running.</summary>
			FreeRun			= 0,
			/// <summary>Genlock is locked.</summary>
			Locked			= 1
		}

		/// <summary>
		/// Container class for serial number.
		/// </summary>
		public class SerialNumberData
		{
			/// <summary>AccessoriesSDK communication channel id.</summary>
			public int		id = -1;
			private ushort	a, b, c, d;
			private bool	cache = false;

			private void Read()
			{
				SDKBase.checkError(CieloSDK.cieloGetSN(id, out a, out b, out c, out d));
			}

			private void Write()
			{
				SDKBase.checkError(CieloSDK.cieloSetSN(id, a, b, c, d));
			}

			/// <summary>
			/// Read values int cache.
			/// </summary>
			public void CacheRead()
			{
				Read();

				cache = true;
			}

			/// <summary>
			/// Write values from cache, clear cache.
			/// </summary>
			public void CacheWrite()
			{
				Write();

				cache = false;
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void CacheClear()
			{
				cache = false;
			}

			/// <summary>
			/// Serial number first 16 bits.
			/// </summary>
			public ushort A
			{
				get
				{
					if (!cache)
						Read();

					return a;
				}
				set
				{
					if (!cache)
						Read();

					a = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Serial number second 16 bits.
			/// </summary>
			public ushort B
			{
				get
				{
					if (!cache)
						Read();

					return b;
				}
				set
				{
					if (!cache)
						Read();

					b = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Serial number third 16 bits.
			/// </summary>
			public ushort C
			{
				get
				{
					if (!cache)
						Read();

					return c;
				}
				set
				{
					if (!cache)
						Read();

					c = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Serial number last 16 bits.
			/// </summary>
			public ushort D
			{
				get
				{
					if (!cache)
						Read();

					return d;
				}
				set
				{
					if (!cache)
						Read();

					d = value;

					if (!cache)
						Write();
				}
			}
		}

		private SerialNumberData	serialNumber = new SerialNumberData();

		/// <summary>
		/// Get serial number container.
		/// </summary>
		public SerialNumberData SerialNumber
		{
			get
			{
				return serialNumber;
			}
		}

		/// <summary>
		/// IRIG time container class.
		/// </summary>
		public class IRIGTimeData
		{
			/// <summary>AccessoriesSDK communication channel id.</summary>
			public int		id = -1;
			private ushort	day, millisecond, nanosecond;
			private byte	hour, minute, second;
			private bool	cache = false;
			private bool	comparator;

			/// <summary>
			/// Ctandard constructor.
			/// </summary>
			/// <param name="_comparator">True if this refers to the comparator, false is live IRIG.</param>
			public IRIGTimeData(bool _comparator)
			{
				comparator = _comparator;
			}

			private void Read()
			{
				if (comparator)
					SDKBase.checkError(CieloSDK.cieloGetIRIGComparator(id, out day, out hour, out minute, out second, out millisecond, out nanosecond));
				else
					SDKBase.checkError(CieloSDK.cieloGetIRIGTime(id, out day, out hour, out minute, out second, out millisecond, out nanosecond));
			}

			private void Write()
			{
				if (comparator)
					SDKBase.checkError(CieloSDK.cieloSetIRIGComparator(id, day, hour, minute, second, millisecond, nanosecond));
				else
					SDKBase.checkError(CieloSDK.cieloSetIRIGTime(id, day, hour, minute, second, millisecond, nanosecond));
			}

			/// <summary>
			/// Read values int cache.
			/// </summary>
			public void CacheRead()
			{
				Read();

				cache = true;
			}

			/// <summary>
			/// Write values from cache, clear cache.
			/// </summary>
			public void CacheWrite()
			{
				Write();

				cache = false;
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void CacheClear()
			{
				cache = false;
			}

			/// <summary>
			/// Day in year.
			/// </summary>
			public ushort Day
			{
				get
				{
					if (!cache)
						Read();

					return day;
				}
				set
				{
					if (!cache)
						Read();

					day = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Hour in day.
			/// </summary>
			public byte Hour
			{
				get
				{
					if (!cache)
						Read();

					return hour;
				}
				set
				{
					if (!cache)
						Read();

					hour = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Minute in hour.
			/// </summary>
			public byte Minute
			{
				get
				{
					if (!cache)
						Read();

					return minute;
				}
				set
				{
					if (!cache)
						Read();

					minute = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Second in minute.
			/// </summary>
			public byte Second
			{
				get
				{
					if (!cache)
						Read();

					return second;
				}
				set
				{
					if (!cache)
						Read();

					second = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Millisecond in second.
			/// </summary>
			public ushort Millisecond
			{
				get
				{
					if (!cache)
						Read();

					return millisecond;
				}
				set
				{
					if (!cache)
						Read();

					millisecond = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Nanosecond in millisecond
			/// </summary>
			public ushort Nanosecond
			{
				get
				{
					if (!cache)
						Read();

					return nanosecond;
				}
				set
				{
					if (!cache)
						Read();

					nanosecond = value;

					if (!cache)
						Write();
				}
			}

			private ushort BCDFromString(string text, int len)
			{
				int		i;
				ushort	ret;
				byte	digit;

				if (text.Length > len)
					text = text.Substring(0, len);
				else if (text.Length < len)
				{
					while (text.Length != len)
						text = "0" + text;
				}

				ret = 0;
				for (i = 0; i < len; ++i)
				{
					if (char.IsDigit(text[i]))
						digit = (byte)(text[i] - '0');
					else
						digit = 0x0F;

					ret |= (ushort)(digit << ((len - i - 1) * 4));
				}

				return ret;
			}

			private string BCDToString(int len, ushort val)
			{
				string	text = "";
				int		i;
				byte	digit;

				for (i = 0; i < len; ++i)
				{
					digit = (byte)((val >> ((len - i - 1) * 4)) & 0x0F);
					if (digit == 0x0F)
						text += 'X';
					else
						text += (char)('0' + digit);
				}

				return text;
			}

			/// <summary>
			/// Converts the value to an irig formatted string.
			/// </summary>
			/// <returns>irig string</returns>
			public override string ToString()
			{
				if (!cache)
					Read();

				if (comparator)
					return BCDToString(4, day) + "," + BCDToString(2, hour) + ":" + BCDToString(2, minute) +
						":" + BCDToString(2, second) + "." + BCDToString(3, millisecond) + BCDToString(3, nanosecond);
				else
					return day.ToString("000") + "," + hour.ToString("00") + ":" + minute.ToString("00") +
						":" + second.ToString("00") + "." + millisecond.ToString("000") + nanosecond.ToString("000");
			}

			/// <summary>
			/// Sets the value from an irig formated string.
			/// </summary>
			/// <param name="text">string</param>
			/// <returns>true on success</returns>
			public bool FromString(string text)
			{
				if (text.Length != 20)
					return false;

				if (!cache)
					Read();

				//	1234,18:48:33.345654
				if (comparator)
				{
					day = BCDFromString(text.Substring(0, 4), 4);
					hour = (byte)BCDFromString(text.Substring(5, 2), 2);
					minute = (byte)BCDFromString(text.Substring(8, 2), 2);
					second = (byte)BCDFromString(text.Substring(11, 2), 2);
					millisecond = BCDFromString(text.Substring(14, 3), 3);
					nanosecond = BCDFromString(text.Substring(17, 3), 3);
				}
				else
				{
					day = ushort.Parse(text.Substring(0, 4));
					hour = byte.Parse(text.Substring(5, 2));
					minute = byte.Parse(text.Substring(8, 2));
					second = byte.Parse(text.Substring(11, 2));
					millisecond = ushort.Parse(text.Substring(14, 3));
					nanosecond = ushort.Parse(text.Substring(17, 3));
				}

				if (!cache)
					Write();

				return true;
			}
		}

		private IRIGTimeData	irigTime = new IRIGTimeData(false);

		/// <summary>
		/// Get IRIG time container class.
		/// </summary>
		public IRIGTimeData IRIGTime
		{
			get
			{
				return irigTime;
			}
		}

		/// <summary>
		/// Version container class.
		/// </summary>
		public class VersionData
		{
			/// <summary>AccessoriesSDK communication channel id.</summary>
			public int		id = -1;
			private byte	ppcMajor, ppcMinor, upcMajor, upcMinor, ccmMajor, ccmMinor;
			private bool	cache = false;
			private PPCData	ppc;
			private UPCData	upc;
			private CCMData ccm;

			/// <summary>
			/// Standard constructor.
			/// </summary>
			public VersionData()
			{
				ppc = new PPCData(this);
				upc = new UPCData(this);
				ccm = new CCMData(this);
			}

			private void Read()
			{
				SDKBase.checkError(CieloSDK.cieloGetVersion(id, out ppcMajor, out ppcMinor, out ccmMajor, out ccmMinor, out upcMajor, out upcMinor));
			}

			/// <summary>
			/// Read values int cache.
			/// </summary>
			public void CacheRead()
			{
				Read();

				cache = true;
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void CacheClear()
			{
				cache = false;
			}

			/// <summary>
			/// Power PC code version.
			/// </summary>
			public class PPCData
			{
				private VersionData		v;

				internal PPCData(VersionData _v)
				{
					v = _v;
				}

				/// <summary>
				/// Major version.
				/// </summary>
				public byte Major
				{
					get
					{
						if (!v.cache)
							v.Read();

						return v.ppcMajor;
					}
				}

				/// <summary>
				/// Minor version.
				/// </summary>
				public byte Minor
				{
					get
					{
						if (!v.cache)
							v.Read();

						return v.ppcMinor;
					}
				}
			}

			/// <summary>
			/// UPC code version.
			/// </summary>
			public class UPCData
			{
				private VersionData		v;

				internal UPCData(VersionData _v)
				{
					v = _v;
				}

				/// <summary>
				/// Major version.
				/// </summary>
				public byte Major
				{
					get
					{
						if (!v.cache)
							v.Read();

						return v.upcMajor;
					}
				}

				/// <summary>
				/// Minor version.
				/// </summary>
				public byte Minor
				{
					get
					{
						if (!v.cache)
							v.Read();

						return v.upcMinor;
					}
				}
			}

			/// <summary>
			/// CCMT version (telemetry).
			/// </summary>
			public class CCMData
			{
				private VersionData		v;

				internal CCMData(VersionData _v)
				{
					v = _v;
				}

				/// <summary>
				/// Major version.
				/// </summary>
				public byte Major
				{
					get
					{
						if (!v.cache)
							v.Read();

						return v.ccmMajor;
					}
				}

				/// <summary>
				/// Minor version.
				/// </summary>
				public byte Minor
				{
					get
					{
						if (!v.cache)
							v.Read();

						return v.ccmMinor;
					}
				}
			}

			/// <summary>
			/// Get PPC version container.
			/// </summary>
			public PPCData PPC
			{
				get
				{
					return ppc;
				}
			}

			/// <summary>
			/// Get UPC version container.
			/// </summary>
			public UPCData UPC
			{
				get
				{
					return upc;
				}
			}

			/// <summary>
			/// Get CCM version container.
			/// </summary>
			public CCMData CCM
			{
				get
				{
					return ccm;
				}
			}
		}

		private VersionData	version = new VersionData();

		/// <summary>
		/// Get version container class.
		/// </summary>
		public VersionData Version
		{
			get
			{
				return version;
			}
		}

		/// <summary>
		/// General purpose digital inputs state.
		/// </summary>
		public byte GPI
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetGPI(id, out val));

				return val;
			}
		}

		/// <summary>
		/// General purpose digital outputs state.
		/// </summary>
		public byte GPO
		{
			set
			{
				SDKBase.checkError(cieloSetGPO(id, value));
			}
		}

		private IRIGTimeData	irigComparator = new IRIGTimeData(true);

		/// <summary>
		/// IRIG comparator time.
		/// </summary>
		public IRIGTimeData IRIGComparator
		{
			get
			{
				return irigComparator;
			}
		}

		/// <summary>
		/// IRIG latch line state.
		/// </summary>
		public byte IRIGLatchLine
		{
			set
			{
				SDKBase.checkError(cieloSetIRIGLatchLine(id, value));
			}
		}

		/// <summary>
		/// GPIO conatiner class.
		/// </summary>
		public class GPIOData
		{
			/// <summary>AccessoriesSDK communication channel id.</summary>
			public int		id = -1;
			private byte	a, b, c, d;
			private bool	cache = false;

			private void Read()
			{
				SDKBase.checkError(CieloSDK.cieloGetGPIO(id, out a, out b, out c, out d));
			}

			/// <summary>
			/// Read values int cache.
			/// </summary>
			public void CacheRead()
			{
				Read();

				cache = true;
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void CacheClear()
			{
				cache = false;
			}

			/// <summary>
			/// GPIO First 8 bits.
			/// </summary>
			public byte A
			{
				get
				{
					if (!cache)
						Read();

					return a;
				}
			}

			/// <summary>
			/// GPIO second 8 bits.
			/// </summary>
			public byte B
			{
				get
				{
					if (!cache)
						Read();

					return b;
				}
			}

			/// <summary>
			/// GPIO third 8 bits.
			/// </summary>
			public byte C
			{
				get
				{
					if (!cache)
						Read();

					return c;
				}
			}

			/// <summary>
			/// GPIO last 8 bits.
			/// </summary>
			public byte D
			{
				get
				{
					if (!cache)
						Read();

					return d;
				}
			}
		}

		private GPIOData	gpio = new GPIOData();

		/// <summary>
		/// Get GPIO container class.
		/// </summary>
		public GPIOData GPIO
		{
			get
			{
				return gpio;
			}
		}

		/// <summary>
		/// IRIG comparator flag.
		/// </summary>
		public bool IRIGComparatorMatch
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetIRIGComparatorMatch(id, out val));

				return (val != 0);
			}
		}

		/// <summary>
		/// Genlock status.
		/// </summary>
		public GenlockType Genlock
		{
			get
			{
				byte	val;

				SDKBase.checkError(cieloGetGenlock(id, out val));

				return (GenlockType)val;
			}
			set
			{
				SDKBase.checkError(cieloSetGenlock(id, (byte)value));
			}
		}

		/// <summary>
		/// Temperature from Cielo's temperature sensor.
		/// </summary>
		public float Temperature
		{
			get
			{
				float		val;

				SDKBase.checkError(cieloGetTemperature(id, out val));

				return val;
			}
		}

		/// <summary>
		/// Humidity from Cielo's humidity sensor.
		/// </summary>
		public float Humidity
		{
			get
			{
				float		val;

				SDKBase.checkError(cieloGetHumidity(id, out val));

				return val;
			}
		}

		/// <summary>
		/// Pressure from Cielo's pressure sensor.
		/// </summary>
		public ushort Pressure
		{
			get
			{
				ushort		val;

				SDKBase.checkError(cieloGetPressure(id, out val));

				return val;
			}
		}

		/// <summary>
		/// Genera purpose A/D container class.
		/// </summary>
		public class GPIADData
		{
			/// <summary>AccessoriesSDK communication channel id.</summary>
			public int		id = -1;
			private ushort	a, b, c, d, e, f, g, h;
			private bool	cache = false;

			private void Read()
			{
				SDKBase.checkError(CieloSDK.cieloGetGPIAD(id, out a, out b, out c, out d, out e, out f, out g, out h));
			}

			/// <summary>
			/// Read values int cache.
			/// </summary>
			public void CacheRead()
			{
				Read();

				cache = true;
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void CacheClear()
			{
				cache = false;
			}

			/// <summary>
			/// GPIAD a.
			/// </summary>
			public ushort A
			{
				get
				{
					if (!cache)
						Read();

					return a;
				}
			}

			/// <summary>
			/// GPIAD b.
			/// </summary>
			public ushort B
			{
				get
				{
					if (!cache)
						Read();

					return b;
				}
			}

			/// <summary>
			/// GPIAD c.
			/// </summary>
			public ushort C
			{
				get
				{
					if (!cache)
						Read();

					return c;
				}
			}

			/// <summary>
			/// GPIAD d.
			/// </summary>
			public ushort D
			{
				get
				{
					if (!cache)
						Read();

					return d;
				}
			}

			/// <summary>
			/// GPIAD e.
			/// </summary>
			public ushort E
			{
				get
				{
					if (!cache)
						Read();

					return e;
				}
			}

			/// <summary>
			/// GPIAD f.
			/// </summary>
			public ushort F
			{
				get
				{
					if (!cache)
						Read();

					return f;
				}
			}

			/// <summary>
			/// GPIAD g.
			/// </summary>
			public ushort G
			{
				get
				{
					if (!cache)
						Read();

					return g;
				}
			}

			/// <summary>
			/// GPIAD h.
			/// </summary>
			public ushort H
			{
				get
				{
					if (!cache)
						Read();

					return h;
				}
			}
		}

		private GPIADData	gpiad = new GPIADData();

		/// <summary>
		/// Get general purpose A/D container class.
		/// </summary>
		public GPIADData GPIAD
		{
			get
			{
				return gpiad;
			}
		}

		/// <summary>
		/// Write test patter into the header.
		/// </summary>
		public void WriteHeaderTestPattern()
		{
			SDKBase.checkError(cieloWriteHeaderTestPattern(id));
		}

		/// <summary>
		/// Force a phoenix sync.
		/// </summary>
		public void SoftwareSync()
		{
			SDKBase.checkError(cieloSoftwareSync(id));
		}

		/// <summary>
		/// Real time clock container class.
		/// </summary>
		public class RealTimeClockData
		{
			/// <summary>AccessoriesSDK control channel id.</summary>
			public int			id = -1;
			private byte		year, month, day, hour, minute, second;
			private bool		cache = false;

			private void Read()
			{
				SDKBase.checkError(CieloSDK.cieloGetRealTimeClock(id, out year, out month, out day, out hour, out minute, out second));
			}

			private void Write()
			{
				SDKBase.checkError(CieloSDK.cieloSetRealTimeClock(id, year, month, day, hour, minute, second));
			}

			/// <summary>
			/// Read values into cache.
			/// </summary>
			public void CacheRead()
			{
				Read();

				cache = true;
			}

			/// <summary>
			/// Write values from cache, clear cache.
			/// </summary>
			public void CacheWrite()
			{
				Write();

				cache = false;
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void CacheClear()
			{
				cache = false;
			}

			/// <summary>
			/// Year since 2000.
			/// </summary>
			public byte Year
			{
				get
				{
					if (!cache)
						Read();

					return year;
				}
				set
				{
					if (!cache)
						Read();

					year = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Month in year.
			/// </summary>
			public byte Month
			{
				get
				{
					if (!cache)
						Read();

					return month;
				}
				set
				{
					if (!cache)
						Read();

					month = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Day in month.
			/// </summary>
			public byte Day
			{
				get
				{
					if (!cache)
						Read();

					return day;
				}
				set
				{
					if (!cache)
						Read();

					day = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Hour in day.
			/// </summary>
			public byte Hour
			{
				get
				{
					if (!cache)
						Read();

					return hour;
				}
				set
				{
					if (!cache)
						Read();

					hour = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Minute in hour.
			/// </summary>
			public byte Minute
			{
				get
				{
					if (!cache)
						Read();

					return minute;
				}
				set
				{
					if (!cache)
						Read();

					minute = value;

					if (!cache)
						Write();
				}
			}

			/// <summary>
			/// Second in minute.
			/// </summary>
			public byte Second
			{
				get
				{
					if (!cache)
						Read();

					return second;
				}
				set
				{
					if (!cache)
						Read();

					second = value;

					if (!cache)
						Write();
				}
			}
		}

		private RealTimeClockData	realTimeClock = new RealTimeClockData();

		/// <summary>
		/// Get realtime clock container class.
		/// </summary>
		public RealTimeClockData RealTimeClock
		{
			get
			{
				return realTimeClock;
			}
		}

		/// <summary>
		/// Perform IO on the Lens UARTs.
		/// </summary>
		/// <param name="uart">UART index (0,1).</param>
		/// <param name="len">Data length.</param>
		/// <param name="rwFlag">Read or Write flag (0=read,1=write).</param>
		/// <param name="buffer">Buffer to read/write into.</param>
		public void UARTIO(byte uart, byte len, byte rwFlag, ref byte []buffer)
		{
			GCHandle	gch;
			int			Error;

			gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);

			Error = cieloUARTIO(id, uart, len, rwFlag, gch.AddrOfPinnedObject());

			gch.Free();

			SDKBase.checkError(Error);
		}

		/// <summary>
		/// Read len bytes into buffer from UART1.
		/// </summary>
		/// <param name="buffer">Buffer to read into.</param>
		/// <param name="len">Number of bytes to read.</param>
		public void UART1Read(ref byte []buffer, ushort len)
		{
			GCHandle	gch;
			int			Error;

			gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);

			Error = cieloUART1Read(id, gch.AddrOfPinnedObject(), len);

			gch.Free();

			SDKBase.checkError(Error);
		}

		/// <summary>
		/// Write len bytes from buffer to UART1.
		/// </summary>
		/// <param name="buffer">Buffer to write.</param>
		/// <param name="len">Number of bytes to write.</param>
		public void UART1Write(byte []buffer, ushort len)
		{
			GCHandle	gch;
			int			Error;

			gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);

			Error = cieloUART1Write(id, gch.AddrOfPinnedObject(), len);

			gch.Free();

			SDKBase.checkError(Error);
		}

		/// <summary>
		/// Read len bytes into buffer from UART2.
		/// </summary>
		/// <param name="buffer">Buffer to read into.</param>
		/// <param name="len">Number of bytes to read.</param>
		public void UART2Read(ref byte []buffer, ushort len)
		{
			GCHandle	gch;
			int			Error;

			gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);

			Error = cieloUART2Read(id, gch.AddrOfPinnedObject(), len);

			gch.Free();

			SDKBase.checkError(Error);
		}

		/// <summary>
		/// Write len bytes from buffer to UART2.
		/// </summary>
		/// <param name="buffer">Buffer to write.</param>
		/// <param name="len">Number of bytes to write.</param>
		public void UART2Write(byte []buffer, ushort len)
		{
			GCHandle	gch;
			int			Error;

			gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);

			Error = cieloUART2Write(id, gch.AddrOfPinnedObject(), len);

			gch.Free();

			SDKBase.checkError(Error);
		}

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloConfigInit(int id);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloConfigFini(int id);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloCacheRead(int id);	/* Enable cache and read data from cielo */
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloCacheWrite(int id);	/* Invalidate cache and write data to cielo */
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloCacheClear(int id);	/* Invalidate cache without writing to cielo */

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSaveState(int id, byte  bank);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetConfigSize(int id, out ushort size);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetConfig(int id, IntPtr buffer);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetConfig(int id, IntPtr buffer);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetHeaderOffset(int id, out byte offset);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetHeaderOffset(int id, byte  offset);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetCameraLinkSetup(int id, out byte setup);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetCameraLinkSetup(int id, byte  setup);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetVideoMuxSetup(int id, out byte setup);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetVideoMuxSetup(int id, byte  setup);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetPatternGeneratorMode(int id, out byte mode);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetPatternGeneratorMode(int id, byte  mode);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetPatternGeneratorSize(int id, out byte size);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetPatternGeneratorSize(int id, byte  size);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetClockMux(int id, out byte mux);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetClockMux(int id, byte  mux);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetClockSource(int id, out byte source);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetClockSource(int id, byte  source);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetPleoraSelectPin(int id, out byte pin);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetPleoraSelectPin(int id, byte  pin);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetPleoraPowerEnabled(int id, out byte enabled);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetPleoraPowerEnabled(int id, byte  enabled);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetHotLinkPowerEnabled(int id, out byte enabled);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetHotLinkPowerEnabled(int id, byte  enabled);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetHostPort(int id, out byte port, out uint baudRate, out byte uartMode);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetHostPort(int id, byte  port, uint baudRate, byte  uartMode);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetCameraPort(int id, out byte port, out uint baudRate);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetCameraPort(int id, byte  port, uint baudRate);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetDeframingEnabled(int id, out byte enabled);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetDeframingEnabled(int id, byte  enabled);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetBypassEnabled(int id, out byte enabled);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetBypassEnabled(int id, byte  enabled);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetSyncInMode(int id, out byte mode);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetSyncInMode(int id, byte  mode);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetClockInMode(int id, out byte mode);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetClockInMode(int id, byte  mode);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetSyncOutMode(int id, out byte mode);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetSyncOutMode(int id, byte  mode);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetClockOutMode(int id, out byte mode);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetClockOutMode(int id, byte  mode);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetIRIGLatchSource(int id, out byte source);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetIRIGLatchSource(int id, byte  source);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetIRIGComparatorEnabled(int id, out byte enabled);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetIRIGComparatorEnabled(int id, byte  enabled);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetPhoenixSequencerArmed(int id, out byte armed);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetPhoenixSequencerArmed(int id, byte  armed);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetPhoenixExtendedFirstSyncEnabled(int id, out byte enabled);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetPhoenixExtendedFirstSyncEnabled(int id, byte  enabled);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetPhoenixSequencerEnabled(int id, out byte enabled);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetPhoenixSequencerEnabled(int id, byte  enabled);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetCameraControlSignalPath(int id, out byte source);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetCameraControlSignalPath(int id, byte  source);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetUART1(int id, out uint baudRate, out byte uartMode);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetUART1(int id, uint baudRate, byte  uartMode);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetUART2(int id, out uint baudRate, out byte uartMode);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetUART2(int id, uint baudRate, byte  uartMode);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetPhoenixSequenceCount(int id, out ushort count);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetPhoenixSequenceCount(int id, ushort  count);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetPhoenixDwellCount(int id, out ushort dwellPS0, out ushort dwellPS1, out ushort dwellPS2, out ushort dwellPS3);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetPhoenixDwellCount(int id, ushort  dwellPS0, ushort  dwellPS1, ushort  dwellPS2, ushort  dwellPS3);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetPhoenixIntegrationTime(int id, out uint itimePS0, out uint itimePS1, out uint itimePS2, out uint itimePS3);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetPhoenixIntegrationTime(int id, uint itimePS0, uint itimePS1, uint itimePS2, uint itimePS3);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetPhoenixInitEnabled(int id, out byte enabled);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetPhoenixInitEnabled(int id, byte  enabled);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetReg(int id, uint address, out uint reg);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetReg(int id, uint address, uint reg);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetSN(int id, out ushort a, out ushort b, out ushort c, out ushort d);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetSN(int id, ushort a, ushort b, ushort c, ushort d);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetIRIGTime(int id, out ushort day, out byte hour, out byte minute, out byte second, out ushort milisecond, out ushort nanosecond);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetIRIGTime(int id, ushort day, byte hour, byte minute, byte second, ushort milisecond, ushort nanosecond);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetVersion(int id, out byte ppcMajor, out byte ppcMinor, out byte ccmMajor, out byte ccmMinor, out byte upcMajor, out byte upcMinor);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetLEDState(int id, byte led, byte state);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetLEDState(int id, byte led, out byte state);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetGPI(int id, out byte state);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetGPO(int id, byte state);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetIRIGComparator(int id, out ushort day, out byte hour, out byte minute, out byte second, out ushort milisecond, out ushort nanosecond);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetIRIGComparator(int id, ushort day, byte hour, byte minute, byte second, ushort milisecond, ushort nanosecond);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetIRIGLatchLine(int id, byte status);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloLatchIRIG(int id);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetLibraryVersion(out ushort major, out ushort minor, out ushort revision, out ushort build);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetGPIO(int id, out byte a, out byte b, out byte c, out byte d);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetIRIGComparatorMatch(int id, out byte match);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetGenlock(int id, byte status);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetGenlock(int id, out byte status);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetTemperature(int id, out float temp);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetHumidity(int id, out float humidity);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetPressure(int id, out ushort pressure);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetGPIAD(int id, out ushort a, out ushort b, out ushort c, out ushort d, out ushort e, out ushort f, out ushort g, out ushort h);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloWriteHeaderTestPattern(int id);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSoftwareSync(int id);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloGetRealTimeClock(int id, out byte year, out byte month, out byte day, out byte hour, out byte minute, out byte second);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloSetRealTimeClock(int id, byte year, byte month, byte day, byte hour, byte minute, byte second);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloUARTIO(int id, byte uart, byte len, byte rwFlag, IntPtr buffer);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloUART1Read(int id, IntPtr buffer, ushort len);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloUART1Write(int id, IntPtr buffer, ushort len);

		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloUART2Read(int id, IntPtr buffer, ushort len);
		[DllImport("CieloSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int cieloUART2Write(int id, IntPtr buffer, ushort len);
	}
}

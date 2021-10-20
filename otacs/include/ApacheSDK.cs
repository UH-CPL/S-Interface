using System;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Threading;

//	sbyte = 8 bit signed
//	byte = 8 bit unsigned
//	short = 16 bit signed
//	ushort = 16 bit unsigned
//	int = 32 bit signed
//	uint = 32 bit unsigned
//	long = 32 bit signed
//	ulong = 32 bit unsigned
//	float = 32 bit
//	double = 64 bit

namespace IACFSDK
{
	/// <summary>
	/// SDK for communicating with the iacf.
	/// </summary>
	public class ApacheSDK
	{
		[StructLayout(LayoutKind.Explicit, Size=9)]
		public struct IRIG
		{
			[FieldOffset(0)]	public ushort		Day;
			[FieldOffset(2)]	public byte			Hour;
			[FieldOffset(3)]	public byte			Minute;
			[FieldOffset(4)]	public byte			Second;
			[FieldOffset(5)]	public ushort		Millisecond;
			[FieldOffset(7)]	public ushort		Microsecond;
		}

		[StructLayout(LayoutKind.Explicit, Size=53)]
		public struct Header
		{
			[FieldOffset(0)]	public byte			ActivePreset;
			[FieldOffset(1)]	public double		IntegrationTime;
			[FieldOffset(9)]	public ushort		FrameCounter;
			[FieldOffset(11)]	public byte			SaturationFlag;	//	one byte bool
			[FieldOffset(12)]	public uint			Saturation;
			[FieldOffset(16)]	public uint			FPAControlWord;
			[FieldOffset(20)]	public uint			FPAWindowWord;
			[FieldOffset(24)]	public byte			Resolution;
			[FieldOffset(25)]	public IRIG			IRIGTime;
			[FieldOffset(34)]	public byte			IRIGLocked;		//	one byte bool
			[FieldOffset(35)]	public byte			Humidity;
			[FieldOffset(36)]	public byte			Pressure;
			[FieldOffset(37)]	public float		Temp;
			[FieldOffset(41)]	public float		AirGapTemp;
			[FieldOffset(45)]	public float		DewarTemp;
			[FieldOffset(49)]	public float		FrontPanelTemp;
		}

		public void DecodeHeader(ref Header hdr, ref ushort[] frame)
		{
			GCHandle	gch = GCHandle.Alloc(frame, GCHandleType.Pinned);

			iacfDecodeHeader(ref hdr, gch.AddrOfPinnedObject());

			gch.Free();
		}

        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfDecodeHeader(ref Header hdr, IntPtr frame);

		private int						id = -1;
		/// <summary>Handles legacy commands.</summary>
		public Legacy					legacy = new Legacy();
		/// <summary>Handles system commands.</summary>
		public System					system = new System();
		/// <summary>Handles AGC commands.</summary>
		public AutomaticGainControl		automaticGainControl = new AutomaticGainControl();
		/// <summary>Handles calibration(NUC) commands.</summary>
		public Calibration				calibration = new Calibration();
		/// <summary>Handles fpa control commands.</summary>
		public FPAControl				fpaControl = new FPAControl();
		/// <summary>Handles fpa cooler commands.</summary>
		public FPACooler				fpaCooler = new FPACooler();
		/// <summary>Handles configuration commands.</summary>
		public Configuration			configuration = new Configuration();
		/// <summary>Handles video processing commands.</summary>
		public VideoProcessing			videoProcessing = new VideoProcessing();
		/// <summary>Handles video display commands.</summary>
		public Display					display = new Display();
		/// <summary>Handles hidden cooler commands (TE cooler 0002).</summary>
		public HiddenCooler				hiddenCooler = new HiddenCooler();
		/// <summary>Handles file system commands (new interface).</summary>
		public FileSystem				fileSystem = new FileSystem();
        /// <summary>Handles power system commands (new interface).</summary>
        public Power				    power = new Power();
        /// <summary>Handles util system commands (new interface).</summary>
        public Util				        util = new Util();
        /// <summary>Handles bit system commands (new interface).</summary>
        public Bit				        bit = new Bit();

		/// <summary>
		/// IndigoSDK communications channel id, must be set before any functions are used.
		/// </summary>
		public int ID
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
				legacy.id = id;
				system.ID = id;
				automaticGainControl.ID = id;
				calibration.ID = id;
				fpaControl.ID = id;
				fpaCooler.ID = id;
				configuration.ID = id;
				videoProcessing.ID = id;
				display.ID = id;
				hiddenCooler.ID = id;
				fileSystem.ID = id;
				configuration.apache = this;
				fileSystem.apache = this;
                power.ID = id;
                util.ID = id;
                bit.ID = id;
			}
		}

		private static SDKBase.Version	version = new SDKBase.Version();

		/// <summary>
		/// Gets the library version.
		/// </summary>
		public static SDKBase.Version LibraryVersion
		{
			get
			{
				GetLibraryVersion(out version.Major, out version.Minor, out version.Revision, out version.Build);

				return version;
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
			SDKBase.checkError(iacfGetLibraryVersion(out major, out minor, out revision, out build));
		}

		/// <summary>
		/// Initializes the configuration on the control device.
		/// </summary>
		public void ConfigInit()
		{
			SDKBase.checkError(iacfConfigInit(id));
		}

		/// <summary>
		/// Finalizes the configuration on the control device.
		/// </summary>
		public void ConfigFini()
		{
			SDKBase.checkError(iacfConfigFini(id));
		}

		/// <summary>Called incrementally to update the progress of a file transfer.</summary>
		public delegate void CopyProgress(object param, double progress);

		#region Legacy
		/// <summary>
		/// Handles legacy commands.
		/// </summary>
		public class Legacy
		{
			/// <summary>IndigoSDK communications channel id, don't set this directly, use iacfSDK.ID</summary>
			public int	id;

			/// <summary>FPA Detector types.</summary>
			public enum DetectorType
			{
				/// <summary>InSb</summary>
				InSb		= 0,
				/// <summary>InGaAs SUI</summary>
				InGaAsSUI	= 1,
				/// <summary>InGaAs ISC</summary>
				InGaAsISC	= 2
			}

			/// <summary>
			/// Gets the detector type.
			/// </summary>
			public DetectorType Detector
			{
				get
				{
					byte	detector;

					SDKBase.checkError(iacfGetDetector(id, out detector));

					return (DetectorType)detector;
				}
			}

			/// <summary>
			/// Gets the unique identity of the camera.
			/// </summary>
			public string Identity
			{
				get
				{
					string		identity;
					byte		[]identityBytes = new byte[256];
					GCHandle	gch;
					int			len;

					gch = GCHandle.Alloc(identityBytes, GCHandleType.Pinned);
					SDKBase.checkError(iacfGetIdentity(id, gch.AddrOfPinnedObject(), 256));
					gch.Free();

					for (len = 0; len < 256; ++len)
					{
						if (identityBytes[len] == 0)
							break;
					}

					identity = (new ASCIIEncoding()).GetString(identityBytes, 0, len);

					return identity;
				}
			}

			/// <summary>
			/// Gets the FPA temperature.
			/// </summary>
			public float FPATemp
			{
				get
				{
					float	temp;

					SDKBase.checkError(iacfGetFPATemp(id, out temp));

					return temp;
				}
			}

			/// <summary>
			/// Gets the readout number (e.g. 9803).
			/// </summary>
			public ushort Readout
			{
				get
				{
					ushort	readout;

					SDKBase.checkError(iacfGetReadout(id, out readout));

					return readout;
				}
			}

			/// <summary>
			/// Saves state on the given camera file.
			/// </summary>
			/// <param name="filename">Camera file to save.</param>
			public void SaveState(string filename)
			{
				byte[]		filenameBytes;
				GCHandle	gch;

				filenameBytes = (new ASCIIEncoding()).GetBytes(filename);

				gch = GCHandle.Alloc(filenameBytes, GCHandleType.Pinned);
				SDKBase.checkError(iacfSaveState(id, gch.AddrOfPinnedObject()));
				gch.Free();
			}

			/// <summary>
			/// Saves state on all known camera files.
			/// </summary>
			public void SaveState()
			{
				SDKBase.checkError(iacfSaveStateAll(id));
			}

			/// <summary>
			/// Gets all the drives defined in the camera fileysystem.
			/// </summary>
			/// <returns>String array of drive names.</returns>
			public string []GetDrives()
			{
				byte			[]buffer = new byte[256];
				GCHandle		gch;
				ushort			size = 256;
				int				i, iLast;
				ArrayList		ar = new ArrayList();
				string			s;
				string			[]ret;
				ASCIIEncoding	ae = new ASCIIEncoding();

				gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				SDKBase.checkError(iacfGetDriveNames(id, gch.AddrOfPinnedObject(), ref size));
				gch.Free();

				iLast = 0;
				for (i = 0; i < 255; ++i)
				{
					if (buffer[i] == 0)
					{
						s = ae.GetString(buffer, iLast, i - iLast);
						if (s.Length > 0)
							ar.Add(s);

						iLast = i + 1;

						if (buffer[i + 1] == 0)
							break;
					}
				}

				ret = new string[ar.Count];

				for (i = 0; i < ar.Count; ++i)
					ret[i] = (string)ar[i];

				ar.Clear();

				return ret;
			}

			/// <summary>
			/// Get all camera filesystem entries in a given dir.
			/// </summary>
			/// <param name="dir">Directory to list.</param>
			/// <returns>String array of all files in the given directory.</returns>
			public string []GetDirEntries(string dir)
			{
				byte			[]buffer = new byte[4096];
				GCHandle		gch;
				ushort			size = 4096;
				int				i, iLast;
				ArrayList		ar = new ArrayList();
				string			s;
				string			[]ret;
				ASCIIEncoding	ae = new ASCIIEncoding();

				gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				SDKBase.checkError(iacfGetDirEntries(id, dir, gch.AddrOfPinnedObject(), ref size));
				gch.Free();

				iLast = 0;
				for (i = 0; i < 4096; ++i)
				{
					if (buffer[i] == 0)
					{
						s = ae.GetString(buffer, iLast, i - iLast);
						if (s.Length > 0)
							ar.Add(s);

						iLast = i + 1;

						if (buffer[i + 1] == 0)
							break;
					}
				}

				ret = new string[ar.Count];

				for (i = 0; i < ar.Count; ++i)
					ret[i] = (string)ar[i];

				ar.Clear();

				return ret;
			}

			/// <summary>
			/// Get information about a camera filesystem node.
			/// </summary>
			/// <param name="node">Node name.</param>
			/// <param name="size">Node (file) size in bytes.</param>
			/// <param name="driveSize">Drive size in bytes.</param>
			/// <param name="driveFree">Drive free space in bytes.</param>
			/// <param name="IsDirectory">File/Directory flag.</param>
			public void GetNodeInfo(string node, out uint size, out uint driveSize, out uint driveFree, out byte IsDirectory)
			{
				SDKBase.checkError(iacfGetNodeInfo(id, node, out size, out driveSize, out driveFree, out IsDirectory));
			}

			/// <summary>
			/// Returns true if the file exists on the camera.
			/// </summary>
			/// <param name="file">camera file</param>
			/// <returns>true if file exists</returns>
			public bool FileExists(string file)
			{
				uint		size, driveSize, driveFree;
				byte		isDirectory;

				try
				{
					GetNodeInfo(file, out size, out driveSize, out driveFree, out isDirectory);
					return true;
				}
				catch
				{
					return false;
				}
			}

			/// <summary>Size of a transfer block in bytes.</summary>
			public ushort	iacfBlockSize = 128;

			/// <summary>
			/// Send a file to the camera with progress.
			/// </summary>
			/// <param name="localFile">File on the computer.</param>
			/// <param name="cameraFile">File on the camera.</param>
			/// <param name="progress">Copy progress delegate.</param>
			/// <param name="param">Parameter for delegate.</param>
			public void SendFile(string localFile, string cameraFile, CopyProgress progress, object param)
			{
				uint		numBlocks, block, size;
				ushort		thisSize;
				FileStream	fsSrc;
				GCHandle	gch;
				byte		[]data = new byte[iacfBlockSize];

				cameraFile = cameraFile.Replace("/", "\\");

				gch = GCHandle.Alloc(data, GCHandleType.Pinned);

				fsSrc = File.OpenRead(localFile);

				size = (uint)fsSrc.Length;

				numBlocks = size / iacfBlockSize;
				if ((size % iacfBlockSize) != 0)
					++numBlocks;

				try
				{
					SDKBase.checkError(iacfSendFileStart(id, cameraFile, size));
				}
				catch
				{
					gch.Free();
					fsSrc.Close();
					throw;
				}

				for (block = 0; block < numBlocks; ++block)
				{
					thisSize = (ushort)Math.Min(iacfBlockSize, size - (block * iacfBlockSize));
					try
					{
						fsSrc.Read(data, 0, thisSize);
						SDKBase.checkError(iacfSendFileBlock(id, block, gch.AddrOfPinnedObject(), thisSize));
					}
					catch
					{
						gch.Free();
						fsSrc.Close();
						throw;
					}

					progress(param, (block / (double)numBlocks) * 100.0);
				}

				gch.Free();
				fsSrc.Close();
			}

			/// <summary>
			/// Send a file to the camera.
			/// </summary>
			/// <param name="localFile">File on the computer.</param>
			/// <param name="cameraFile">File on the camera.</param>
			public void SendFile(string localFile, string cameraFile)
			{
				byte		[]file;
				uint		size;
				FileStream	fs;
				GCHandle	gch;

				cameraFile = cameraFile.Replace("/", "\\");

				fs = File.Open(localFile, FileMode.Open);

				size = (uint)fs.Length;

				file = new byte[size];

				fs.Read(file, 0, (int)size);
				fs.Close();

				gch = GCHandle.Alloc(file, GCHandleType.Pinned);
				SDKBase.checkError(iacfSendFileMem(id, cameraFile, gch.AddrOfPinnedObject(), size));
				gch.Free();
			}

			/// <summary>
			/// Recieve file from the camera to a local file with progress.
			/// </summary>
			/// <param name="cameraFile">File in the camera.</param>
			/// <param name="localFile">File on the computer.</param>
			/// <param name="progress">Copy progress delegate.</param>
			/// <param name="param">Param for progress delegate.</param>
			public void RecvFile(string cameraFile, string localFile, CopyProgress progress, object param)
			{
				uint		block = 0, sizeRead = 0;
				ushort		thisSize;
				uint		nodeSize, driveSize, driveFree;
				byte		isDirectory;
				FileStream	fsDest;
				byte		[]data = new byte[iacfBlockSize];
				GCHandle	gch;

				cameraFile = cameraFile.Replace("/", "\\");

				SDKBase.checkError(iacfGetNodeInfo(id, cameraFile, out nodeSize, out driveSize, out driveFree, out isDirectory));

				fsDest = File.Create(localFile);

				gch = GCHandle.Alloc(data, GCHandleType.Pinned);

				try
				{
					SDKBase.checkError(iacfRecvFileStart(id, cameraFile));
				}
				catch
				{
					gch.Free();
					fsDest.Close();
					throw;
				}

				while (sizeRead < nodeSize)
				{
					thisSize = iacfBlockSize;

					try
					{
						SDKBase.checkError(iacfRecvFileBlock(id, block, gch.AddrOfPinnedObject(), ref thisSize));
						fsDest.Write(data, 0, thisSize);
					}
					catch
					{
						iacfRecvFileFinish(id);
						gch.Free();
						fsDest.Close();
						throw;
					}

					sizeRead += thisSize;

					progress(param, (sizeRead / (double)nodeSize) * 100.0);

					++block;
				}

				try
				{
					SDKBase.checkError(iacfRecvFileFinish(id));
				}
				catch
				{
					gch.Free();
					fsDest.Close();
					throw;
				}

				gch.Free();
				fsDest.Close();
			}

			/// <summary>
			/// Recive a file from the camera to a local file.
			/// </summary>
			/// <param name="cameraFile">File in the camera.</param>
			/// <param name="localFile">File on the computer</param>
			public void RecvFile(string cameraFile, string localFile)
			{
				uint		size, driveSize, driveFree;
				byte		isDirectory;
				byte		[]file;
				FileStream	fs;
				GCHandle	gch;

				cameraFile = cameraFile.Replace("/", "\\");

				GetNodeInfo(cameraFile, out size, out driveSize, out driveFree, out isDirectory);

				file = new byte[size];

				gch = GCHandle.Alloc(file, GCHandleType.Pinned);
				SDKBase.checkError(iacfRecvFileMem(id, cameraFile, gch.AddrOfPinnedObject(), ref size));
				gch.Free();

				fs = File.Create(localFile);

				fs.Write(file, 0, (int)size);

				fs.Close();
			}

			//	PACK		char[4]
			//	any number of
			//	Filename	len:byte+char[len]
			//	Size		uint
			//	Data		byte[Size]

			/// <summary>
			/// Uploads a pack file to the camera (contains multiple camera files).
			/// </summary>
			/// <param name="packFile">Pack filename on the pc.</param>
			/// <param name="progress">Progress proc.</param>
			/// <param name="param">VAlue to pass to progress proc.</param>
			public void UploadPack(string packFile, CopyProgress progress, object param)
			{
				uint		numBlocks, block, size, magic;
				ushort		thisSize;
				FileStream	fsSrc;
				GCHandle	gch;
				byte		[]data = new byte[iacfBlockSize];
				string		cameraFile;
				ASCIIEncoding	ascii = new ASCIIEncoding();

				gch = GCHandle.Alloc(data, GCHandleType.Pinned);

				fsSrc = File.OpenRead(packFile);

				fsSrc.Read(data, 0, 4);

				magic = BitConverter.ToUInt32(data, 0);

				if (magic != 0x4B434150)
				{
					fsSrc.Close();
					throw new ApplicationException("Not a pack file.");
				}

				while (fsSrc.Position < fsSrc.Length)
				{
					fsSrc.Read(data, 0, 1);
					fsSrc.Read(data, 1, data[0]);
					cameraFile = ascii.GetString(data, 1, data[0]);
					fsSrc.Read(data, 0, 4);

					cameraFile = cameraFile.Replace("/", "\\");

					size = BitConverter.ToUInt32(data, 0);

					numBlocks = size / iacfBlockSize;
					if ((size % iacfBlockSize) != 0)
						++numBlocks;

					try
					{
						SDKBase.checkError(iacfSendFileStart(id, cameraFile, size));
					}
					catch
					{
						gch.Free();
						fsSrc.Close();
						throw;
					}

					for (block = 0; block < numBlocks; ++block)
					{
						thisSize = (ushort)Math.Min(iacfBlockSize, size - (block * iacfBlockSize));
						try
						{
							fsSrc.Read(data, 0, thisSize);
							SDKBase.checkError(iacfSendFileBlock(id, block, gch.AddrOfPinnedObject(), thisSize));
						}
						catch
						{
							gch.Free();
							fsSrc.Close();
							throw;
						}

						progress(param, (fsSrc.Position / (double)fsSrc.Length) * 100.0);
					}
				}

				gch.Free();
				fsSrc.Close();
			}

			/// <summary>
			/// Get the filenames of the files stored in a pack file.
			/// </summary>
			/// <param name="packFile">Pack file name on pc.</param>
			/// <returns>String array of files in the pack file.</returns>
			public string []PackContents(string packFile)
			{
				uint			size, magic;
				FileStream		fsSrc;
				byte			[]data = new byte[256];
				string			cameraFile;
				ASCIIEncoding	ascii = new ASCIIEncoding();
				ArrayList		ar = new ArrayList();
				string			[]ret;
				int				i;

				fsSrc = File.OpenRead(packFile);

				fsSrc.Read(data, 0, 4);

				magic = BitConverter.ToUInt32(data, 0);

				if (magic != 0x4B434150)
				{
					fsSrc.Close();
					throw new ApplicationException("Not a pack file.");
				}

				while (fsSrc.Position < fsSrc.Length)
				{
					fsSrc.Read(data, 0, 1);
					fsSrc.Read(data, 1, data[0]);
					cameraFile = ascii.GetString(data, 1, data[0]);
					fsSrc.Read(data, 0, 4);

					size = BitConverter.ToUInt32(data, 0);

					fsSrc.Seek(size, SeekOrigin.Current);

					ar.Add(cameraFile);
				}

				fsSrc.Close();

				ret = new string[ar.Count];

				for (i = 0; i < ar.Count; ++i)
					ret[i] = (string)ar[i];

				ar.Clear();
				ar = null;

				return ret;
			}

			/// <summary>
			/// Contains information on a pack file entry.
			/// </summary>
			public struct PackEntry
			{
				/// <summary>
				/// name of this entry (full path).
				/// </summary>
				public string	name;
				/// <summary>
				/// size of this entry in bytes.
				/// </summary>
				public uint		size;

				/// <summary>
				/// Standard constructor.
				/// </summary>
				/// <param name="_name">name</param>
				/// <param name="_size">size</param>
				public PackEntry(string _name, uint _size)
				{
					name = _name;
					size = _size;
				}
			}

			/// <summary>
			/// Get a full contents of a pack file.
			/// </summary>
			/// <param name="packFile">pack file</param>
			/// <returns>aray of pack entries</returns>
			public PackEntry []PackContentsEx(string packFile)
			{
				uint			size, magic;
				FileStream		fsSrc;
				byte			[]data = new byte[256];
				string			cameraFile;
				ASCIIEncoding	ascii = new ASCIIEncoding();
				ArrayList		ar = new ArrayList();
				PackEntry		[]ret;
				int				i;

				fsSrc = File.OpenRead(packFile);

				fsSrc.Read(data, 0, 4);

				magic = BitConverter.ToUInt32(data, 0);

				if (magic != 0x4B434150)
				{
					fsSrc.Close();
					throw new ApplicationException("Not a pack file.");
				}

				while (fsSrc.Position < fsSrc.Length)
				{
					fsSrc.Read(data, 0, 1);
					fsSrc.Read(data, 1, data[0]);
					cameraFile = ascii.GetString(data, 1, data[0]);
					fsSrc.Read(data, 0, 4);

					size = BitConverter.ToUInt32(data, 0);

					fsSrc.Seek(size, SeekOrigin.Current);

					ar.Add(new PackEntry(cameraFile, size));
				}

				fsSrc.Close();

				ret = new PackEntry[ar.Count];

				for (i = 0; i < ar.Count; ++i)
					ret[i] = (PackEntry)ar[i];

				ar.Clear();
				ar = null;

				return ret;
			}

			/// <summary>
			/// Download a pack file (multiple camera files in one file).
			/// </summary>
			/// <param name="packFile">Pack filename on PC.</param>
			/// <param name="cameraFiles">Camera files to download.</param>
			/// <param name="progress">Progress proc.</param>
			/// <param name="param">Value to pass to progress proc.</param>
			public void DownloadPack(string packFile, string []cameraFiles, CopyProgress progress, object param)
			{
				uint		block = 0, sizeRead = 0, packSize = 0, packDownloaded = 0;
				ushort		thisSize;
				uint		nodeSize, driveSize, driveFree;
				byte		isDirectory;
				FileStream	fsDest;
				byte		[]data = new byte[iacfBlockSize];
				GCHandle	gch;
				ASCIIEncoding	ascii = new ASCIIEncoding();

				for (int i = 0; i < cameraFiles.Length; ++i)
					cameraFiles[i] = cameraFiles[i].Replace("/", "\\");

				foreach (string cameraFile in cameraFiles)
				{
					SDKBase.checkError(iacfGetNodeInfo(id, cameraFile, out nodeSize, out driveSize, out driveFree, out isDirectory));

					packSize += nodeSize;
				}

				fsDest = File.Create(packFile);

				fsDest.Write(BitConverter.GetBytes((uint)0x4B434150), 0, 4);

				gch = GCHandle.Alloc(data, GCHandleType.Pinned);

				foreach (string cameraFile in cameraFiles)
				{
					try
					{
						SDKBase.checkError(iacfGetNodeInfo(id, cameraFile, out nodeSize, out driveSize, out driveFree, out isDirectory));
						SDKBase.checkError(iacfRecvFileStart(id, cameraFile));
					}
					catch
					{
						gch.Free();
						fsDest.Close();
						throw;
					}

					data[0] = (byte)ascii.GetByteCount(cameraFile);
					ascii.GetBytes(cameraFile, 0, (int)data[0], data, 1);
					fsDest.Write(data, 0, data[0] + 1);
					fsDest.Write(BitConverter.GetBytes(nodeSize), 0, 4);

					block = 0;
					sizeRead = 0;

					while (sizeRead < nodeSize)
					{
						thisSize = iacfBlockSize;

						try
						{
							SDKBase.checkError(iacfRecvFileBlock(id, block, gch.AddrOfPinnedObject(), ref thisSize));
							fsDest.Write(data, 0, thisSize);
						}
						catch
						{
							gch.Free();
							fsDest.Close();
							throw;
						}

						sizeRead += thisSize;
						packDownloaded += thisSize;

						progress(param, (packDownloaded / (double)packSize) * 100.0);

						++block;
					}

					try
					{
						SDKBase.checkError(iacfRecvFileFinish(id));
					}
					catch
					{
						gch.Free();
						fsDest.Close();
						throw;
					}
				}

				gch.Free();
				fsDest.Close();
			}

			/// <summary>
			/// Remove a node (file or dir) from the camera filesystem.
			/// </summary>
			/// <param name="node">Name of the node.</param>
			public void RemoveNode(string node)	//	aka unlink
			{
				node = node.Replace("/", "\\");

				SDKBase.checkError(iacfRemoveNode(id, node));
			}

			/// <summary>
			/// Creates a directory in the camera filesystem.
			/// </summary>
			/// <param name="dir">Name of the directory.</param>
			public void CreateDirectory(string dir)
			{
				dir = dir.Replace("/", "\\");

				SDKBase.checkError(iacfCreateDirectory(id, dir));
			}

			/// <summary>
			/// Renames a node (file or dir).
			/// </summary>
			/// <param name="oldName">Old/existing name.</param>
			/// <param name="newName">New name.</param>
			public void RenameNode(string oldName, string newName)
			{
				oldName = oldName.Replace("/", "\\");
				newName = newName.Replace("/", "\\");

				SDKBase.checkError(iacfRenameNode(id, oldName, newName));
			}

			/// <summary>
			/// Read camera memory bytes at the given address.
			/// </summary>
			/// <param name="address">Address.</param>
			/// <param name="data">Data to read into.</param>
			/// <param name="len">Length to read.</param>
			public void ReadMemory(uint address, ref byte []data, ref uint len)
			{
				GCHandle	gch;

				gch = GCHandle.Alloc(data, GCHandleType.Pinned);
				try
				{
					SDKBase.checkError(iacfReadMemory(id, address, gch.AddrOfPinnedObject(), ref len));
				}
				catch
				{
					gch.Free();
					throw;
				}
				gch.Free();
			}

			/// <summary>
			/// Read camera memory 32-bit words at the given address.
			/// </summary>
			/// <param name="address">Address.</param>
			/// <param name="data">Data to read into.</param>
			/// <param name="len">Number of words to read.</param>
			public void ReadMemory(uint address, ref uint []data, ref uint len)
			{
				byte	[]bytes = new byte[len * 4];
				uint	i;

				len *= 4;
				ReadMemory(address, ref bytes, ref len);
				len /= 4;

				for (i = 0; i < len; ++i)
					data[i] = BitConverter.ToUInt32(bytes, (int)(i * 4));
			}

			/// <summary>
			/// Write camera memory bytes.
			/// </summary>
			/// <param name="address">Address.</param>
			/// <param name="data">Data to write.</param>
			/// <param name="len">Length to write.</param>
			public void WriteMemory(uint address, ref byte []data, ref uint len)
			{
				GCHandle	gch;

				gch = GCHandle.Alloc(data, GCHandleType.Pinned);
				try
				{
					SDKBase.checkError(iacfWriteMemory(id, address, gch.AddrOfPinnedObject(), ref len));
				}
				catch
				{
					gch.Free();
					throw;
				}
				gch.Free();
			}

			/// <summary>
			/// Write camera memory 32-bit words.
			/// </summary>
			/// <param name="address">Address.</param>
			/// <param name="data">Data to write.</param>
			/// <param name="len">Number of words to write.</param>
			public void WriteMemory(uint address, ref uint []data, ref uint len)
			{
				byte	[]bytes = new byte[len * 4], tmpBytes;
				uint	i, j;

				for (i = 0; i < len; ++i)
				{
					tmpBytes = BitConverter.GetBytes(data[i]);
					for (j = 0; j < 4; ++j)
						bytes[i * 4 + j] = tmpBytes[j];
				}

				len *= 4;
				WriteMemory(address, ref bytes, ref len);
				len /= 4;
			}

			float GetDACVoltage(byte dac)
			{
				float	val, sfactor;

                SDKBase.checkError(iacfGetDACVoltage(id, dac, out val, out sfactor));

				return val;
			}

			void SetDACVoltage(byte dac, float val)
			{
				float	scaleFactor, junk;
                // Get the scale factor first
				SDKBase.checkError(iacfGetDACVoltage(id, dac, out junk, out scaleFactor));
                // Lets retry dac voltages at this level because the camera tends to time out 
                // because of our background status qurerying FPA temperature
                if (iacfSetDACVoltage(id, dac, (ushort)(val / scaleFactor)) != 0)
                    if (iacfSetDACVoltage(id, dac, (ushort)(val / scaleFactor)) != 0)
                        SDKBase.checkError(iacfSetDACVoltage(id, dac, (ushort)(val / scaleFactor)));
            }

			/// <summary>
			/// DetCom dac voltage (0002).
			/// </summary>
			public float DetCom
			{
				get
				{
					return GetDACVoltage(4);
				}
				set
				{
					SetDACVoltage(4, value);
				}
			}

			/// <summary>
			/// Skim dac voltage (0002).
			/// </summary>
			public float SkimVoltage
			{
				get
				{
					return GetDACVoltage(7);
				}
				set
				{
					SetDACVoltage(7, value);
				}
			}

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIdentity(int id, IntPtr szIdentity, int maxSize);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPATemp(int id, out float temp);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetReadout(int id, out ushort readout);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetDetector(int id, out byte detector);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSaveState(int id, IntPtr szFile);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSaveStateAll(int id);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetDriveNames(int id, IntPtr szNames, ref ushort size);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetDirEntries(int id, string szDir, IntPtr szEntries, ref ushort size);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetNodeInfo(int id, string szNode, out uint size, out uint driveSize, out uint driveFree, out byte IsDirectory);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfRemoveNode(int id, string szNode);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfRenameNode(int id, string szOld, string szNew);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfCreateDirectory(int id, string szDir);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSendFileStart(int id, string szFile, uint size);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSendFileBlock(int id, uint blockNumber, IntPtr buffer, ushort size);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfRecvFileStart(int id, string szFile);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfRecvFileBlock(int id, uint blockNumber, IntPtr buffer, ref ushort size);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfRecvFileFinish(int id);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSendFileMem(int id, string szFile, IntPtr file, uint size);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfRecvFileMem(int id, string szFile, IntPtr file, ref uint size);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfReadWriteMemory(int id, uint rwFlag, uint address, IntPtr data, ref uint len);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfReadMemory(int id, uint address, IntPtr data, ref uint len);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfWriteMemory(int id, uint address, IntPtr data, ref uint len);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetDACVoltage(int id, byte dac, out float voltage, out float scaleFactor);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetDACVoltage(int id, byte dac, ushort voltage);
		}
		#endregion

		#region AutomaticGainControl
		/// <summary>
		/// Handles AGC functions.
		/// </summary>
		public class AutomaticGainControl
		{
            /// <summary>
            /// Histogram capture more enumeration.
            /// </summary>
            public enum HistogramCaptureMode
            {
				/// <summary>
				/// Capture histogram post bad pixel replacment.
				/// </summary>
                PostBpr,
				/// <summary>
				/// Capture histogram with raw data.
				/// </summary>
                PostRaw
            }
            
            /// <summary>IndigoSDK communications channel id, don't set this directly, use iacfSDK.ID</summary>
			private int	id;

			/// <summary>
			/// Accessories SDK control device id.
			/// </summary>
			public int ID
			{
				set
				{
					id = value;
					activeROI.id = id;
				}
			}

			/// <summary>
			/// Pings for the health/existance of the module.
			/// </summary>
			public void Ping()
			{
				SDKBase.checkError(iacfPingAGC(id));
			}

			/// <summary>
			/// Gets/Sets agc enable update status.
			/// </summary>
			public bool Enabled
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetAGCEnable(id, out val));

					return (val != 0);
				}
				set
				{
					uint	val;

					if (value)
						val = 1;
					else
						val = 0;

					SDKBase.checkError(iacfSetAGCEnable(id, val));
				}
			}

			/// <summary>
			/// Algorithm type.
			/// </summary>
			public enum AlgorithmType
			{
				/// <summary>Use manual contrast/brightness.</summary>
				Manual = 0,
				/// <summary>Use linear scale.</summary>
				Linear,
				/// <summary>Use plateau equalization.</summary>
				Plateau,
				/// <summary>Use logarithmic scale.</summary>
				Logarithmic,
				/// <summary>Use mean sigma scaling.</summary>
				MeanSigma
			};

			/// <summary>
			/// Selects algorithm.
			/// </summary>
			public AlgorithmType Algorithm
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetAGCAlgorithmSelect(id, out val));

					return (AlgorithmType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetAGCAlgorithmSelect(id, (int)value));
				}
			}

			/// <summary>
			/// Bypass AGC enable state.
			/// </summary>
			public bool Bypass
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetAGCBypassITTFilter(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCBypassITTFilter(id, ((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Percent pixels to include (high mark) for linear scale.
			/// </summary>
			public float LinearPercentHigh
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCLinearPctHigh(id, out val));

					return (float)(val * 0.01f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCLinearPctHigh(id, (ushort)Math.Round(value / 0.01f)));
				}
			}

			/// <summary>
			/// Percent pixels to include (low mark) for linear scale.
			/// </summary>
			public float LinearPercentLow
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCLinearPctLow(id, out val));

					return (float)(val * 0.01f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCLinearPctLow(id, (ushort)Math.Round(value / 0.01f)));
				}
			}

			/// <summary>
			/// AGC calculation rate in Hz.
			/// </summary>
			public float CalculationRate
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCCalcRateReq(id, out val));

					return (float)(val * 0.01f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCCalcRateReq(id, (ushort)Math.Round(value / 0.01f)));
				}
			}

			/// <summary>
			/// Actual AGC calculation rate in Hz.
			/// </summary>
			public float CalculationRateActual
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCCalcRateAct(id, out val));

					return (float)(val * 0.01f);
				}
			}

			/// <summary>
			/// Filter rate in Hz.
			/// </summary>
			public float FilterRate
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCFilterRateReq(id, out val));

					return (float)(val * 0.01f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCFilterRateReq(id, (ushort)Math.Round(value / 0.01f)));
				}
			}

			/// <summary>
			/// Actual filter rate in Hz.
			/// </summary>
			public float FilterRateActual
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCFilterRateAct(id, out val));

					return (float)(val * 0.01f);
				}
			}

			/// <summary>
			/// Filter dampening value (new = FD*old+(1-FD)*new).
			/// </summary>
			public byte FilterDampening
			{
				get
				{
					byte	val;

					SDKBase.checkError(iacfGetAGCFilterDampVal(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetAGCFilterDampVal(id, value));
				}
			}

			/// <summary>
			/// Current histogram mean value.
			/// </summary>
			public ushort HistogramMean
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCHistogramMean(id, out val));

					return val;
				}
			}

			/// <summary>
			/// Current histogram standard deviation.
			/// </summary>
			public double HistogramStandardDeviation
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetAGCHistogramStd(id, out val));

					return (double)(val * 0.001);
				}
			}

			/// <summary>
			/// Current histogram low count value.
			/// </summary>
			public ushort HistogramLow
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCHistogramLow(id, out val));

					return val;
				}
			}

			/// <summary>
			/// Current histogram high count value.
			/// </summary>
			public ushort HistogramHigh
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCHistogramHigh(id, out val));

					return val;
				}
			}

			/// <summary>
			/// Maximum allowed gain.
			/// </summary>
			public float MaxGain
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCMaxGainLimit(id, out val));

					return (float)(val * 0.001f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCMaxGainLimit(id, (ushort)Math.Round(value / 0.001f)));
				}
			}

			/// <summary>
			/// Midpoint for transformation table (e.g. 128).
			/// </summary>
			public byte MidpointSetpoint
			{
				get
				{
					byte	val;

					SDKBase.checkError(iacfGetAGCMidpointSetpoint(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetAGCMidpointSetpoint(id, value));
				}
			}

			/// <summary>
			/// Plateau equalization value to use.
			/// </summary>
			public ushort PlateauValue
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCPlateauValReq(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetAGCPlateauValReq(id, value));
				}
			}

			/// <summary>
			/// Actual PE value calculated.
			/// </summary>
			public ushort PlateauValueActual
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCPlateauValAct(id, out val));

					return val;
				}
			}

			/// <summary>
			/// Max algo loops to allow for convergance.
			/// </summary>
			public byte PlateauLoopMax
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCPlateauLoopMax(id, out val));

					return (byte)val;
				}
				set
				{
					SDKBase.checkError(iacfSetAGCPlateauLoopMax(id, (ushort)value));
				}
			}

			/// <summary>
			/// Requested plageau gain value.
			/// </summary>
			public float PlateauGain
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCPlateauGainReq(id, out val));

					return (float)(val * 0.001f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCPlateauGainReq(id, (ushort)Math.Round(value / 0.001f)));
				}
			}

			/// <summary>
			/// Actual plateau gain value.
			/// </summary>
			public float PlateauGainActual
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCPlateauGainAct(id, out val));

					return (float)(val * 0.001f);
				}
			}

			/// <summary>
			/// Mean Sigma lower limit.
			/// </summary>
			public float MeanSigmaLowerLimit
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCMeanSigmaLowerLimit(id, out val));

					return (float)(val * 0.001f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCMeanSigmaLowerLimit(id, (ushort)Math.Round(value / 0.001f)));
				}
			}

			/// <summary>
			/// Mean sigma upper limit.
			/// </summary>
			public float MeanSigmaUpperLimit
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCMeanSigmaUpperLimit(id, out val));

					return (float)(val * 0.001f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCMeanSigmaUpperLimit(id, (ushort)Math.Round(value / 0.001f)));
				}
			}

			/// <summary>
			/// Manual contrast to apply.
			/// </summary>
			public float ManualContrast
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCManualContrast(id, out val));

					return (float)(val * 0.01f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCManualContrast(id, (ushort)Math.Round(value / 0.01f)));
				}
			}

			/// <summary>
			/// Manual brightness to apply.
			/// </summary>
			public short ManualBrightness
			{
				get
				{
					short	val;

					SDKBase.checkError(iacfGetAGCManualBrightness(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetAGCManualBrightness(id, value));
				}
			}

			/// <summary>
			/// Always caluclate histogram standard deviation enable.
			/// </summary>
			public bool HistogramStandardDeviationEnabled
			{
				get	
				{
					int		val;

					SDKBase.checkError(iacfGetAGCHistogramStdEnable(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCHistogramStdEnable(id, ((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Active (save) ROI name.
			/// </summary>
			public string ActiveROIName
			{
				get
				{
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					byte			[]strBytes = new byte[28];
					byte			len;

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfGetAGCActiveRoiName(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();

					for (len = 0; len < 28; ++len)
						if (strBytes[len] == 0)
							break;

					return ascii.GetString(strBytes, 0, len);
				}
				set
				{
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					byte			[]strBytes = new byte[28];

					ascii.GetBytes(value, 0, value.Length, strBytes, 0);

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfSetAGCActiveRoiName(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();
				}
			}

			/// <summary>
			/// Coordinate mode for ROIs.
			/// </summary>
			public enum CoordinateModeType
			{
				/// <summary>
				/// ROI is in FPA space.
				/// </summary>
				FPA,
				/// <summary>
				/// ROI is in Display space.
				/// </summary>
				Display
			}

			/// <summary>
			/// Active ROI properties.
			/// </summary>
			public class ActiveROIData
			{
				/// <summary>Accessories SDK control device to use.</summary>
				public int		id = -1;
				private ushort	xoffset, yoffset, width, height, lifetime;
				private int		coordinateMode;
				private byte	color;
				private bool	cache = false;

				private void Read()
				{
					SDKBase.checkError(iacfGetAGCActiveRoiData(id, out xoffset, out yoffset, out width, out height, out coordinateMode, out lifetime, out color));
				}

				private void Write()
				{
					SDKBase.checkError(iacfSetAGCActiveRoiData(id, xoffset, yoffset, width, height, coordinateMode, lifetime, color));
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// X Offset of the ROI.
				/// </summary>
				public ushort XOffset
				{
					get
					{
						if (!cache)
							Read();

						return xoffset;
					}
					set
					{
						if (!cache)
							Read();

						xoffset = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Y Offset of the ROI.
				/// </summary>
				public ushort YOffset
				{
					get
					{
						if (!cache)
							Read();

						return yoffset;
					}
					set
					{
						if (!cache)
							Read();

						yoffset = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Width of the ROI.
				/// </summary>
				public ushort Width
				{
					get
					{
						if (!cache)
							Read();

						return width;
					}
					set
					{
						if (!cache)
							Read();

						width = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Height of the ROI.
				/// </summary>
				public ushort Height
				{
					get
					{
						if (!cache)
							Read();

						return height;
					}
					set
					{
						if (!cache)
							Read();

						height = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Splash lifetime of the ROI.
				/// </summary>
				public ushort Lifetime
				{
					get
					{
						if (!cache)
							Read();

						return lifetime;
					}
					set
					{
						if (!cache)
							Read();

						lifetime = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Grayscale color.
				/// </summary>
				public byte Color
				{
					get
					{
						if (!cache)
							Read();

						return color;
					}
					set
					{
						if (!cache)
							Read();

						color = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// FPA or Display coordinate space.
				/// </summary>
				public CoordinateModeType CoordinateMode
				{
					get
					{
						if (!cache)
							Read();

						return (CoordinateModeType)coordinateMode;
					}
					set
					{
						if (!cache)
							Read();

						coordinateMode = (int)value;

						if (!cache)
							Write();
					}
				}
			}

			private ActiveROIData	activeROI = new ActiveROIData();

			/// <summary>
			/// Gets the active roi data container class.
			/// </summary>
			public ActiveROIData ActiveROI
			{
				get
				{
					return activeROI;
				}
			}

			/// <summary>
			/// ROI name when using Delete ROI.
			/// </summary>
			public string DeleteROIName
			{
				get
				{
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					byte			[]strBytes = new byte[28];
					byte			len;

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfGetAGCDeleteRoiName(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();

					for (len = 0; len < 28; ++len)
						if (strBytes[len] == 0)
							break;

					return ascii.GetString(strBytes, 0, len);
				}
				set
				{
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					byte			[]strBytes = new byte[28];

					ascii.GetBytes(value, 0, value.Length, strBytes, 0);

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfSetAGCDeleteRoiName(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();
				}
			}

			/// <summary>
			/// Force the ROI to be displayed.
			/// </summary>
			public bool ForceROIDisplay
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetAGCForceRoiDisplay(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCForceRoiDisplay(id, (int)((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Percent include pixels high mark for log scale.
			/// </summary>
			public float LogarithmicPercentHigh
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCLogPctHigh(id, out val));

					return (float)(val * 0.01f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCLogPctHigh(id, (ushort)Math.Round(value / 0.01f)));
				}
			}

			/// <summary>
			/// Percent include pixels low mark for log scale.
			/// </summary>
			public float LogarithmicPercentLow
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCLogPctLow(id, out val));

					return (float)(val * 0.01f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCLogPctLow(id, (ushort)Math.Round(value / 0.01f)));
				}
			}

			/// <summary>
			/// Percent include pixels high mark for plateau equalization scale.
			/// </summary>
			public float PlateauPercentHigh
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCPlateauPctHigh(id, out val));

					return (float)(val * 0.01f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCPlateauPctHigh(id, (ushort)Math.Round(value / 0.01f)));
				}
			}

			/// <summary>
			/// Percent include pixels low mark for plateau equalization scale.
			/// </summary>
			public float PlateauPercentLow
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCPlateauPctLow(id, out val));

					return (float)(val * 0.01f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCPlateauPctLow(id, (ushort)Math.Round(value / 0.01f)));
				}
			}

			/// <summary>
			/// Percent include pixels high mark for mean sigma scale.
			/// </summary>
			public float MeanSigmaPercentHigh
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCMeanSigmaPctHigh(id, out val));

					return (float)(val * 0.01f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCMeanSigmaPctHigh(id, (ushort)Math.Round(value / 0.01f)));
				}
			}

			/// <summary>
			/// percent include pixels low mark for mean sigma scale.
			/// </summary>
			public float MeanSigmaPercentLow
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetAGCMeanSigmaPctLow(id, out val));

					return (float)(val * 0.01f);
				}
				set
				{
					SDKBase.checkError(iacfSetAGCMeanSigmaPctLow(id, (ushort)Math.Round(value / 0.01f)));
				}
			}

			//	TODO:	verify correct function.
			/// <summary>
			/// ROI name to use when with LoadROI.
			/// </summary>
			public string LoadROIName
			{
				get
				{
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					byte			[]strBytes = new byte[28];
					byte			len;

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfGetAGCLoadName(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();

					for (len = 0; len < 28; ++len)
						if (strBytes[len] == 0)
							break;

					return ascii.GetString(strBytes, 0, len);
				}
				set
				{
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					byte			[]strBytes = new byte[28];

					ascii.GetBytes(value, 0, value.Length, strBytes, 0);

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfSetAGCLoadName(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();
				}
			}

            /// <summary>
            /// Specifies the histogram data source for the command: AcquireHistogram
            /// </summary>
            public HistogramCaptureMode AcquireHistogramMode
            {
                get
                {
                    HistogramCaptureMode retVal = HistogramCaptureMode.PostBpr;
                    Int32	val;

                    SDKBase.checkError(iacfGetAGCAcquireHistogramMode(id, out val));

                    switch ( val )
                    {
                        case 0:
                            retVal = HistogramCaptureMode.PostBpr;
                            break;
                        case 1:
                            retVal = HistogramCaptureMode.PostRaw;
                            break;
                        default:
                            retVal = HistogramCaptureMode.PostBpr;
                            break;
                    }

                    return retVal;
                }

                set
                {
                    Int32 val = 0;
                    if (value == HistogramCaptureMode.PostRaw)
                        val = 1;

                    SDKBase.checkError(iacfSetAGCAcquireHistogramMode(id, val));
                }
            }
            
            /// <summary>
			/// Load the ROI at LoadROIName.
			/// </summary>
			public void LoadROI()
			{
				SDKBase.checkError(iacfAgcLoadRoi(id));
			}

			/// <summary>
			/// Load the given ROI.
			/// </summary>
			/// <param name="name">ROI name.</param>
			public void LoadROI(string name)
			{
				LoadROIName = name;
				LoadROI();
			}

			/// <summary>
			/// Save the current ROI.
			/// </summary>
			public void SaveROI()
			{
				SDKBase.checkError(iacfAgcSaveRoi(id));
			}

			/// <summary>
			/// Save the current ROI as name.
			/// </summary>
			/// <param name="name">ROI name.</param>
			public void SaveROI(string name)
			{
				ActiveROIName = name;
				SaveROI();
			}

			/// <summary>
			/// Delete the ROI at DeleteROIName.
			/// </summary>
			public void DeleteROI()
			{
				SDKBase.checkError(iacfAgcDeleteRoi(id));
			}

			/// <summary>
			/// Delege the ROI with the given name.
			/// </summary>
			/// <param name="name">ROI Name.</param>
			public void DeleteROI(string name)
			{
				DeleteROIName = name;
				DeleteROI();
			}


            /// <summary>
            /// Analyze histogram now.
            /// </summary>
            public void AnalyzeHistogram()
            {
                SDKBase.checkError(iacfAgcAnalyzeHistogram(id));
            }
            
            /// <summary>
            /// Acquire histogram now.
            /// </summary>
            public void AcquireHistogram()
            {
                SDKBase.checkError(iacfAgcAcquireHistogram(id));
            }

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfPingAGC(int id);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCEnable(int id, out uint enabled);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCEnable(int id, uint enabled);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCAlgorithmSelect(int id, out int AlgorithmSelect);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCAlgorithmSelect(int id, int AlgorithmSelect);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCBypassITTFilter(int id, out int BypassITTFilter);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCBypassITTFilter(int id, int BypassITTFilter);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCLinearPctHigh(int id, out ushort LinearPctHigh);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCLinearPctHigh(int id, ushort LinearPctHigh);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCLinearPctLow(int id, out ushort LinearPctLow);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCLinearPctLow(int id, ushort LinearPctLow);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCCalcRateReq(int id, out ushort CalcRateReq);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCCalcRateReq(int id, ushort CalcRateReq);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCCalcRateAct(int id, out ushort CalcRateAct);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCCalcRateAct(int id, ushort CalcRateAct);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCFilterRateReq(int id, out ushort FilterRateReq);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCFilterRateReq(int id, ushort FilterRateReq);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCFilterRateAct(int id, out ushort FilterRateAct);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCFilterRateAct(int id, ushort FilterRateAct);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCFilterDampVal(int id, out byte FilterDampVal);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCFilterDampVal(int id, byte FilterDampVal);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCHistogramMean(int id, out ushort HistogramMean);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCHistogramMean(int id, ushort HistogramMean);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCHistogramStd(int id, out uint HistogramStd);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCHistogramStd(int id, uint HistogramStd);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCHistogramHigh(int id, out ushort HistogramLow);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCHistogramHigh(int id, ushort HistogramLow);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetAGCHistogramLow(int id, out ushort HistogramLow);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetAGCHistogramLow(int id, ushort HistogramLow);
            
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCMaxGainLimit(int id, out ushort MaxGainLimit);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCMaxGainLimit(int id, ushort MaxGainLimit);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCMidpointSetpoint(int id, out byte MidpointSetpoint);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCMidpointSetpoint(int id, byte MidpointSetpoint);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCPlateauValReq(int id, out ushort PlateauValReq);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCPlateauValReq(int id, ushort PlateauValReq);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCPlateauValAct(int id, out ushort PlateauValAct);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCPlateauValAct(int id, ushort PlateauValAct);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCPlateauLoopMax(int id, out ushort PlateauLoopMax);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCPlateauLoopMax(int id, ushort PlateauLoopMax);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCPlateauGainReq(int id, out ushort PlateauGainReq);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCPlateauGainReq(int id, ushort PlateauGainReq);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCPlateauGainAct(int id, out ushort PlateauGainAct);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCPlateauGainAct(int id, ushort PlateauGainAct);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCMeanSigmaLowerLimit(int id, out ushort MeansigmaLowerLimit);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCMeanSigmaLowerLimit(int id, ushort MeansigmaLowerLimit);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCMeanSigmaUpperLimit(int id, out ushort MeansigmaUpperLimit);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCMeanSigmaUpperLimit(int id, ushort MeansigmaUpperLimit);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCManualContrast(int id, out ushort ManualContrast);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCManualContrast(int id, ushort ManualContrast);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCManualBrightness(int id, out short ManualBrightness);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCManualBrightness(int id, short ManualBrightness);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCHistogramStdEnable(int id, out int HistogramStdEnable);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCHistogramStdEnable(int id, int HistogramStdEnable);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCActiveRoiName(int id, IntPtr ActiveRoiName);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCActiveRoiName(int id, IntPtr ActiveRoiName);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCDeleteRoiName(int id, IntPtr DeleteRoiName);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCDeleteRoiName(int id, IntPtr DeleteRoiName);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCForceRoiDisplay(int id, out int ForceRoiDisplay);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCForceRoiDisplay(int id, int ForceRoiDisplay);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCLogPctHigh(int id, out ushort LogPctHigh);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCLogPctHigh(int id, ushort LogPctHigh);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCLogPctLow(int id, out ushort LogPctLow);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCLogPctLow(int id, ushort LogPctLow);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCPlateauPctHigh(int id, out ushort PlateauPctHigh);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCPlateauPctHigh(int id, ushort PlateauPctHigh);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCPlateauPctLow(int id, out ushort PlateauPctLow);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCPlateauPctLow(int id, ushort PlateauPctLow);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCMeanSigmaPctHigh(int id, out ushort MeanSigmaPctHigh);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCMeanSigmaPctHigh(int id, ushort MeanSigmaPctHigh);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCMeanSigmaPctLow(int id, out ushort MeanSigmaPctLow);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCMeanSigmaPctLow(int id, ushort MeanSigmaPctLow);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCLoadName(int id, IntPtr LoadName);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCLoadName(int id, IntPtr LoadName);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetAGCActiveRoiData(int id, 
							 out ushort xStart,
							 out ushort yStart,
							 out ushort xSize,
							 out ushort ySize,
							 out int coordinateMode,
							 out ushort boundBoxLifetime,
							 out byte boundBoxColor );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetAGCActiveRoiData(int id, 
							 ushort xStart,
							 ushort yStart,
							 ushort xSize,
							 ushort ySize,
							 int coordinateMode,
							 ushort boundBoxLifetime,
							 byte boundBoxColor );

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetAGCAcquireHistogramMode(int id, out int AcquireHistogramMode);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetAGCAcquireHistogramMode(int id, int AcquireHistogramMode);

            
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfAgcLoadRoi(int id);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfAgcSaveRoi(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfAgcDeleteRoi(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfAgcAnalyzeHistogram(int id);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfAgcAcquireHistogram(int id);

		}
		#endregion

		#region System
		/// <summary>
		/// Handles system functions.
		/// </summary>
		public class System
		{
			private int	id;

			/// <summary>IndigoSDK communications channel id, don't set this directly, use iacfSDK.ID</summary>
			public int ID
			{
				set
				{
					id = value;
					currentStatus.id = id;
					realTimeClock.id = id;
				}
			}

			/// <summary>
			/// Pings for the health/existance of the module.
			/// </summary>
			public void Ping()
			{
				SDKBase.checkError(iacfPingSYS(id));
			}

			/// <summary>
			/// Version for apache modules.
			/// </summary>
			public struct Version
			{
				/// <summary>Major version.</summary>
				public byte		Major;
				/// <summary>Minor version.</summary>
				public byte		Minor;
				/// <summary>Build number.</summary>
				public byte		Build;

				/// <summary>
				/// Converts the version to a string X.Y.Z style.
				/// </summary>
				/// <returns>String representation.</returns>
				public override string ToString()
				{
					return Major.ToString() + "." + Minor.ToString() + "." + Build.ToString();
				}
			}

            /// <summary>
            /// Flags that define system options.
            /// </summary>
            public enum SystemOptionsFlags
            {
                /// <summary>
                /// European Option.
                /// </summary>
                HighGainDisabled    = 1,
                /// <summary>
                /// Place holder.
                /// </summary>
                NextOption          = 2
            }
            
            /// <summary>
			/// board information container class.
			/// </summary>
			public struct BoardInfo
			{
				/// <summary>Board revision number.</summary>
				public ushort	Revision;
				/// <summary>Board assembly number.</summary>
				public ushort	Assembly;
				/// <summary>Board serial number.</summary>
				public string	Serial;

				/// <summary>
				/// Converts the board info to a string.
				/// </summary>
				/// <returns>string representation</returns>
				public override string ToString()
				{
					return Revision.ToString() + "." + Assembly.ToString() + " " + Serial;
				}
			}

			/// <summary>
			/// Holds the versions returned by the info command.
			/// </summary>
			public class InfoData
			{
				/// <summary>Main application version.</summary>
				public Version		MainApp;
				/// <summary>Main boot loader version.</summary>
				public Version		MainBoot;
				/// <summary>FPGA NUC version.</summary>	
				public Version		FPGANUC;
				/// <summary>FPGA Video version.</summary>
				public Version		FPGAVideo;
				/// <summary>Power board application version.</summary>
				public Version		PowerApp;
				/// <summary>Power board boot loader version.</summary>
				public Version		PowerBoot;
				/// <summary>Power board NUC version.</summary>
				public Version		PowerNUC;
				/// <summary>Main board info.</summary>
				public BoardInfo	MainBoard;
				/// <summary>Digitizer board info.</summary>
				public BoardInfo	DigitizerBoard;
				/// <summary>Power board info.</summary>
				public BoardInfo	PowerBoard;
			}

			private InfoData	info = new InfoData();

			/// <summary>
			/// Get versions.
			/// </summary>
			public InfoData Info
			{
				get
				{
					byte			[]sn1Bytes = new byte[16], sn2Bytes = new byte[16], sn3Bytes = new byte[16];
					GCHandle		sn1, sn2, sn3;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					int				len;

					sn1 = GCHandle.Alloc(sn1Bytes, GCHandleType.Pinned);
					sn2 = GCHandle.Alloc(sn2Bytes, GCHandleType.Pinned);
					sn3 = GCHandle.Alloc(sn3Bytes, GCHandleType.Pinned);

					try
					{
						SDKBase.checkError(iacfGetSYSInfo(id, out info.MainApp.Major, out info.MainApp.Minor, out info.MainApp.Build,
							out info.MainBoot.Major, out info.MainBoot.Minor, out info.MainBoot.Build,
							out info.FPGANUC.Major, out info.FPGANUC.Minor, out info.FPGANUC.Build,
							out info.FPGAVideo.Major, out info.FPGAVideo.Minor, out info.FPGAVideo.Build,
							out info.PowerApp.Major, out info.PowerApp.Minor, out info.PowerApp.Build,
							out info.PowerBoot.Major, out info.PowerBoot.Minor, out info.PowerBoot.Build,
							out info.PowerNUC.Major, out info.PowerNUC.Minor, out info.PowerNUC.Build,
							out info.MainBoard.Revision, out info.MainBoard.Assembly, sn1.AddrOfPinnedObject(),
							out info.DigitizerBoard.Revision, out info.DigitizerBoard.Assembly, sn2.AddrOfPinnedObject(),
							out info.PowerBoard.Revision, out info.PowerBoard.Assembly, sn3.AddrOfPinnedObject()));
					}
					catch
					{
						sn1.Free();
						sn2.Free();
						sn3.Free();
						throw;
					}

					for (len = 0; len < 16; ++len)
						if (sn1Bytes[len] == 0)
							break;

					info.MainBoard.Serial = ascii.GetString(sn1Bytes, 0, len);

					for (len = 0; len < 16; ++len)
						if (sn1Bytes[len] == 0)
							break;

					info.DigitizerBoard.Serial = ascii.GetString(sn2Bytes, 0, len);

					for (len = 0; len < 16; ++len)
						if (sn1Bytes[len] == 0)
							break;

					info.PowerBoard.Serial = ascii.GetString(sn3Bytes, 0, len);

					sn1.Free();
					sn2.Free();
					sn3.Free();

					return info;
				}
			}

			/// <summary>
			/// Gets the unique identity of the camera.
			/// </summary>
			public string Identity
			{
				get
				{
					string		identity;
					byte		[]identityBytes = new byte[24];
					GCHandle	gch;
					int			len;

					gch = GCHandle.Alloc(identityBytes, GCHandleType.Pinned);
					SDKBase.checkError(iacfGetSYSIdentity(id, gch.AddrOfPinnedObject()));
					gch.Free();

					for (len = 0; len < 24; ++len)
					{
						if (identityBytes[len] == 0)
							break;
					}

					identity = (new ASCIIEncoding()).GetString(identityBytes, 0, len);

					return identity;
				}
			}

			/// <summary>
			/// Holds the status code and text for the current NUC status.
			/// </summary>
			public class CurrentStatusData
			{
				/// <summary>Accessories SDK control device to use.</summary>
				public int		id = -1;
				private string	message;
				private ushort	code;
                private ushort	maxWaitTime;
                private bool	cache = false;

				private void Read()
				{
					byte		[]messageBytes = new byte[256];
					GCHandle	gch;
					int			len;

					gch = GCHandle.Alloc(messageBytes, GCHandleType.Pinned);
					SDKBase.checkError(iacfGetSYSCurrentStatus(id, gch.AddrOfPinnedObject(), out code, out maxWaitTime));
					gch.Free();

					for (len = 0; len < 256; ++len)
					{
						if (messageBytes[len] == 0)
							break;
					}

					message = (new ASCIIEncoding()).GetString(messageBytes, 0, len);

					if (message.Equals("Ready."))
						message = "Ready";
				}

				private void Write()
				{
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

                /// <summary>
                /// Maximum wait time.
                /// </summary>
                public ushort MaxWaitTime
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return maxWaitTime;
                    }
                }
                
                /// <summary>
				/// Status code.
				/// </summary>
				public ushort Code
				{
					get
					{
						if (!cache)
							Read();

						return code;
					}
				}

				/// <summary>
				/// Status Message.
				/// </summary>
				public string Message
				{
					get
					{
						if (!cache)
							Read();

						return message;
					}
				}
			}

			private CurrentStatusData	currentStatus = new CurrentStatusData();

			/// <summary>
			/// Gets the current status container class.
			/// </summary>
			public CurrentStatusData CurrentStatus
			{
				get
				{
					return currentStatus;
				}
			}

			/// <summary>
			/// Real time clock container class.
			/// </summary>
			public class RealTimeClockData
			{
				/// <summary>AccessoriesSDK control channel id.</summary>
				public int			id = -1;
				private byte		year, month, day, hour, minute, second, weekDay;
				private bool		cache = false;

				private void Read()
				{
					SDKBase.checkError(iacfGetSYSRealTimeData(id, out year, out month, out day, out weekDay, out hour, out minute, out second));
				}

				private void Write()
				{
					SDKBase.checkError(iacfSetSYSRealTimeData(id, year, month, day, weekDay, hour, minute, second));
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

				/// <summary>
				/// Converts the real time clock data to a string.
				/// </summary>
				/// <returns>string representation</returns>
				public override string ToString()
				{
					if (!cache)
						Read();

					return month.ToString("0") + "/" + day.ToString("00") + "/" + year.ToString("00") + " " +
						hour.ToString("0") + ":" + minute.ToString("00") + ":" + second.ToString("00");
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
            /// Get system options (saee enum.
            /// </summary>
            public SystemOptionsFlags SystemOptions
            {
                get
                {
                    uint retVal = 0;
                    SDKBase.checkError(iacfGetSYSOptions(id, out retVal));
                    return (SystemOptionsFlags) retVal;
                }

                set
                {
                    uint dataValue = 0;
                    if ( (value &  SystemOptionsFlags.HighGainDisabled) == SystemOptionsFlags.HighGainDisabled )
                        dataValue = 1;
                    SDKBase.checkError(iacfSetSYSOptions(id, dataValue));
                }
            }

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfPingSYS(int id);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetSYSInfo(int id,
							 out byte mainAppMajor, out byte mainAppMinor, out byte mainAppBuild,
							 out byte mainBootMajor, out byte mainBootMinor, out byte mainBootBuild,
							 out byte fpgaNUCMajor, out byte fpgaNUCMinor, out byte fpgaNUCBuild,
							 out byte fpgaVideoMajor, out byte fpgaVideoMinor, out byte fpgaVideoBuild,
							 out byte pwrAppMajor, out byte pwrAppMinor, out byte pwrAppBuild,
							 out byte pwrBootMajor, out byte pwrBootMinor, out byte pwrBootBuild,
							 out byte pwrNUCMajor, out byte pwrNUCMinor, out byte pwrNUCBuild,
							 out ushort mainBrdRevNum, out ushort mainBrdAssemblyNum, IntPtr mainBrdSerNum,
							 out ushort digiBrdRevNum, out ushort digiBrdAssemblyNum, IntPtr digiBrdSerNum,
							 out ushort powerBrdRevNum, out ushort powerBrdAssemblyNum, IntPtr powerBrdSerNum);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetSYSIdentity(int id, IntPtr sz);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetSYSCurrentStatus (int id, IntPtr sysStatusString, out ushort sysStatusNumber, out ushort sysMaxWaitTime);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetSYSRealTimeData(int id, out byte year, out byte month, out byte day, out byte weekDay, out byte hour, out byte minute, out byte second);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetSYSRealTimeData(int id, byte year, byte month, byte day, byte weekDay, byte hour, byte minute, byte second);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetSYSOptions(int id, out uint systemOptions);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetSYSOptions(int id, uint systemOptions);
        }
		#endregion

		#region Calibration
		/// <summary>
		/// Handles calibration (NUC) functions.
		/// </summary>
		public class Calibration
		{
			/// <summary>
			/// Status codes for CurrentStatus.Code
			/// </summary>
			public enum StatusCode
			{
				///<summary>NotInProgress</summary>
				NotInProgress						= 0,
				///<summary>WaitSource1InFOV</summary>
				WaitSource1InFOV					= 1,
				///<summary>WaitSource2InFOV</summary>
				WaitSource2InFOV					= 2,
				///<summary>WaitSource3InFOV</summary>
				WaitSource3InFOV					= 3,
				///<summary>WaitSetupCamera</summary>
				WaitSetupCamera						= 10,
				///<summary>HereIsCalibrationBadPixelAbort</summary>
				HereIsCalibrationBadPixelAbort		= 11,
				///<summary>FlagIsInFOV</summary>
				FlagIsInFOV							= 12,
				///<summary>FlagComingToTemperature</summary>
				FlagComingToTemperature				= 13,
				///<summary>CurrentTotalBadPixels</summary>
				CurrentTotalBadPixels				= 14,
				///<summary>SavingBadPixelMap</summary>
				SavingBadPixelMap					= 15,
				///<summary>CommandNewNUCActive</summary>
				CommandNewNUCActive					= 16,
				///<summary>SettingUpCameraForCalibration</summary>
				SettingUpCameraForCalibration		= 17,
				///<summary>DisablingMsgQCommands</summary>
				DisablingMsgQCommands				= 18,
				///<summary>SaveActiveNUCPrioirToCalibration</summary>
				SaveActiveNUCPrioirToCalibration	= 19,
				///<summary>NUCAbortInProgress</summary>
				NUCAbortInProgress					= 20,
				///<summary>TooManyBadPixelsCalibrationPaused</summary>
				TooManyBadPixelsCalibrationPaused	= 21,
				///<summary>DisableNUCAbort</summary>
				DisableNUCAbort						= 22,
				///<summary>SelectingAmbientSource</summary>
				SelectingAmbientSource				= 23,
				///<summary>SelectingColdSource</summary>
				SelectingColdSource					= 24,
				///<summary>SelectingHotSource</summary>
				SelectingHotSource					= 25,
				///<summary>SelectingImageFilter</summary>
				SelectingImageFilter				= 26,
				///<summary>SaveToNUCFlash</summary>
				SaveToNUCFlash						= 27,
				///<summary>CalculatingCoefficients</summary>
				CalculatingCoefficients				= 28,
				///<summary>GatheringData</summary>
				GatheringData						= 29,
				///<summary>DoneGatheringData</summary>
				DoneGatheringData					= 30,
				///<summary>SavingCoefficients</summary>
				SavingCoefficients					= 31,
				///<summary>LoadingCoefficients</summary>
				LoadingCoefficients					= 32,
				///<summary>DisablingAGC</summary>
				DisablingAGC						= 33,
				///<summary>DetectingBadPixels</summary>
				DetectingBadPixels					= 34,
				///<summary>NUCAborted</summary>
				NUCAborted							= 35,
				///<summary>SavingNUCConfigToRAMDrive</summary>
				SavingNUCConfigToRAMDrive			= 40,
				///<summary>SavingNUCDataToRAMDrive</summary>
				SavingNUCDataToRAMDrive				= 41,
				///<summary>ApplyingNUCData</summary>
				ApplyingNUCData						= 42,
			}

			//	TODO: CALTimeStamp, CALFlagThermalSetPoints, CALBadPixelStatus, iacfGetCALFlagTecState, iacfGetCALFlagTecSetPointSelect
			private int		id;

			/// <summary>AccessoriesSDK communications channel id.</summary>
			public int ID
			{
				set
				{
					id = value;
					currentStatus.id = id;
					badPixelDetection.id = id;
					badPixelStatus.id = id;
				}
			}

			/// <summary>
			/// Holds the status code and text for the current NUC status.
			/// </summary>
			public class CurrentStatusData
			{
				/// <summary>Accessories SDK control device to use.</summary>
				public int		id = -1;
				private string	message;
				private short	code;
				private bool	cache = false;

				private void Read()
				{
					byte		[]messageBytes = new byte[256];
					GCHandle	gch;
					int			len;

					gch = GCHandle.Alloc(messageBytes, GCHandleType.Pinned);
					SDKBase.checkError(iacfGetCALCurrentStatus(id, gch.AddrOfPinnedObject(), out code));
					gch.Free();

					for (len = 0; len < 256; ++len)
					{
						if (messageBytes[len] == 0)
							break;
					}

					message = (new ASCIIEncoding()).GetString(messageBytes, 0, len);
				}

				private void Write()
				{
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// Status code.
				/// </summary>
				public StatusCode Code
				{
					get
					{
						if (!cache)
							Read();

						return (StatusCode)code;
					}
				}

				/// <summary>
				/// Status Message.
				/// </summary>
				public string Message
				{
					get
					{
						if (!cache)
							Read();

						return message;
					}
				}
			}

			private CurrentStatusData	currentStatus = new CurrentStatusData();

			/// <summary>
			/// Pings for the health/existance of the module.
			/// </summary>
			public void Ping()
			{
				SDKBase.checkError(iacfPingCAL(id));
			}

			/// <summary>Type of NUC to perform.</summary>
			public enum NUCTypeType
			{
				/// <summary>No NUC</summary>
				None			= 0,
				/// <summary>One Point NUC</summary>
				OnePoint		= 1,
				/// <summary>Two Point NUC</summary>
				TwoPoint		= 2,
				/// <summary>Three Point NUC</summary>
				ThreePoint		= 3
			}

			/// <summary>Calibration source for the NUC.</summary>
			public enum CalSourceType
			{
				/// <summary>Use internal flag.</summary>
				Internal		= 0,
				/// <summary>Use external blackbody.</summary>
				External		= 1
			}

			/// <summary>
			/// Gets/Sets the type of NUC to perform.
			/// </summary>
			public NUCTypeType NUCType
			{
				get
				{
					ushort		val;

					SDKBase.checkError(iacfGetCALNucType(id, out val));

					return (NUCTypeType)val;
				}
				set
				{
					ushort		val = (ushort)value;

					SDKBase.checkError(iacfSetCALNucType(id, val));
				}
			}

			/// <summary>
			/// Gets/Sets the calibration source for the NUC.
			/// </summary>
			public CalSourceType CalSource
			{
				get
				{
					ushort		val;

					SDKBase.checkError(iacfGetCALSource(id, out val));

					return (CalSourceType)val;
				}
				set
				{
					ushort		val = (ushort)value;

					SDKBase.checkError(iacfSetCALSource(id, val));
				}
			}

			/// <summary>
			/// Gets the current status code and string during a NUC.
			/// </summary>
			public CurrentStatusData CurrentStatus
			{
				get
				{
					return currentStatus;
				}
			}

			/// <summary>
			/// Version number for the active NUC.
			/// </summary>
			public ushort Version
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetCALVersion(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetCALVersion(id, value));
				}
			}

			/// <summary>
			/// Name of the current NUC.
			/// </summary>
			public string Tag
			{
				get
				{
					byte		[]tagBytes = new byte[28];
					GCHandle	gch;
					int			len;

					gch = GCHandle.Alloc(tagBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfGetCALTag(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();

					for (len = 0; len < 28; ++len)
					{
						if (tagBytes[len] == 0)
							break;
					}

					return (new ASCIIEncoding()).GetString(tagBytes, 0, len);
				}
				set
				{
					byte			[]tagBytes = new byte[28];
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					int				i;

					if (ascii.GetByteCount(value) > 28)
						SDKBase.checkError(-1);

					for (i = 0; i < 28; ++i)
						tagBytes[i] = 0;

					ascii.GetBytes(value, 0, value.Length, tagBytes, 0);

					gch = GCHandle.Alloc(tagBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfSetCALTag(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();
				}
			}

			/// <summary>
			/// Holds information on bad pixel status.
			/// </summary>
			public class BadPixelStatusData
			{
				/// <summary>Accessories SDK control device to use.</summary>
				public int		id = -1;
				private uint	counterBadAtHighAdcLimit, counterBadAtLowAdcLimit, counterBadAtHighPercent, counterBadAtLowPercent,
								counterTotalBadDetected, counterAlt1, counterAlt2, counterAlt3, counterAlt4, counterTotalAltDetected;
				private bool	cache = false;

				private void Read()
				{
					SDKBase.checkError(iacfGetCALBadPixelStatus(id, out counterBadAtHighAdcLimit, out counterBadAtLowAdcLimit, out counterBadAtHighPercent, out counterBadAtLowPercent,
																	out counterTotalBadDetected, out counterAlt1, out counterAlt2, out counterAlt3, out counterAlt4, out counterTotalAltDetected));
				}

				/// <summary>
				/// Read all.
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Discard previously read.
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// Bad due to ADC high limit.
				/// </summary>
				public uint BadAtADCLimitHigh
				{
					get
					{
						if (!cache)
							Read();

						return counterBadAtHighAdcLimit;
					}
				}

				/// <summary>
				/// Bad due to ADC low limit.
				/// </summary>
				public uint BadAtADCLimitLow
				{
					get
					{
						if (!cache)
							Read();

						return counterBadAtLowAdcLimit;
					}
				}

				/// <summary>
				/// Bad due to response low percent.
				/// </summary>
				public uint BadAtResponseLow
				{
					get
					{
						if (!cache)
							Read();

						return counterBadAtLowPercent;
					}
				}

				/// <summary>
				/// Bad due to response high percent.
				/// </summary>
				public uint BadAtResponseHigh
				{
					get
					{
						if (!cache)
							Read();

						return counterBadAtHighPercent;
					}
				}

				/// <summary>
				/// Total bad pixels detected.
				/// </summary>
				public uint TotalBad
				{
					get
					{
						if (!cache)
							Read();

						return counterTotalBadDetected;
					}
				}

				/// <summary>
				/// Alternate counter 1.
				/// </summary>
				public uint Alt1
				{
					get
					{
						if (!cache)
							Read();

						return counterAlt1;
					}
				}

				/// <summary>
				/// Alternate counter 2.
				/// </summary>
				public uint Alt2
				{
					get
					{
						if (!cache)
							Read();

						return counterAlt2;
					}
				}

				/// <summary>
				/// Alternate counter 3.
				/// </summary>
				public uint Alt3
				{
					get
					{
						if (!cache)
							Read();

						return counterAlt3;
					}
				}

				/// <summary>
				/// Alternate counter 4.
				/// </summary>
				public uint Alt4
				{
					get
					{
						if (!cache)
							Read();

						return counterAlt4;
					}
				}

				/// <summary>
				/// Total alternate counter.
				/// </summary>
				public uint TotalAlt
				{
					get
					{
						if (!cache)
							Read();

						return counterTotalAltDetected;
					}
				}
			}

			private BadPixelStatusData	badPixelStatus = new BadPixelStatusData();

			/// <summary>
			/// Gets bad pixel status container class.
			/// </summary>
			public BadPixelStatusData BadPixelStatus
			{
				get
				{
					return badPixelStatus;
				}
			}

			/// <summary>
			/// Holds information on bad pixel detection.
			/// </summary>
			public class BadPixelDetectionData
			{
				/// <summary>Accessories SDK control device to use.</summary>
				public int		id = -1;
				private uint	limitAdcHigh, limitAdcLow, responseHighPercent, responseLowPercent, percentBadBeforePause;
				private bool	cache = false;

				private void Read()
				{
					SDKBase.checkError(iacfGetCALBadPixelDetection(id, out limitAdcHigh, out limitAdcLow, out responseHighPercent, out responseLowPercent, out percentBadBeforePause));
				}

				private void Write()
				{
					SDKBase.checkError(iacfSetCALBadPixelDetection(id, limitAdcHigh, limitAdcLow, responseHighPercent, responseLowPercent, percentBadBeforePause));
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// Analog Digital Converter high limit (counts).
				/// </summary>
				public uint ADCLimitHigh
				{
					get
					{
						if (!cache)
							Read();

						return limitAdcHigh;
					}
					set
					{
						if (!cache)
							Read();

						limitAdcHigh = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Analog Digital Convert low limit (counts).
				/// </summary>
				public uint ADCLimitLow
				{
					get
					{
						if (!cache)
							Read();

						return limitAdcLow;
					}
					set
					{
						if (!cache)
							Read();

						limitAdcLow = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Response low percent.
				/// </summary>
				public uint ResponseLow
				{
					get
					{
						if (!cache)
							Read();

						return responseLowPercent;
					}
					set
					{
						if (!cache)
							Read();

						responseLowPercent = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Response high percent.
				/// </summary>
				public uint ResponseHigh
				{
					get
					{
						if (!cache)
							Read();

						return responseHighPercent;
					}
					set
					{
						if (!cache)
							Read();

						responseHighPercent = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Percent of bad pixels before pausing the calibration.
				/// </summary>
				public float PercentBadBeforePause
				{
					get
					{
						if (!cache)
							Read();

						return percentBadBeforePause * 0.01f;
					}
					set
					{
						if (!cache)
							Read();

						percentBadBeforePause = (uint)Math.Round(value / 0.01);

						if (!cache)
							Write();
					}
				}
			}

			private BadPixelDetectionData	badPixelDetection = new BadPixelDetectionData();

			/// <summary>
			/// Gets bad pixel detection container class.
			/// </summary>
			public BadPixelDetectionData BadPixelDetection
			{
				get
				{
					return badPixelDetection;
				}
			}

			/// <summary>
			/// Bad pixel in file.
			/// </summary>
			public string BadPixelIngressFile1
			{
				get
				{
					byte		[]strBytes = new byte[32];
					GCHandle	gch;
					int			len;

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfGetCALBadPixelIngressFile1(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();

					for (len = 0; len < 32; ++len)
					{
						if (strBytes[len] == 0)
							break;
					}

					return (new ASCIIEncoding()).GetString(strBytes, 0, len);
				}
				set
				{
					byte			[]strBytes = new byte[32];
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					int				i;

					if (ascii.GetByteCount(value) > 32)
						SDKBase.checkError(-1);

					for (i = 0; i < 32; ++i)
						strBytes[i] = 0;

					ascii.GetBytes(value, 0, value.Length, strBytes, 0);

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfSetCALBadPixelIngressFile1(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();
				}
			}

			/// <summary>
			/// Bad pixel in file.
			/// </summary>
			public string BadPixelIngressFile2
			{
				get
				{
					byte		[]strBytes = new byte[32];
					GCHandle	gch;
					int			len;

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfGetCALBadPixelIngressFile2(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();

					for (len = 0; len < 32; ++len)
					{
						if (strBytes[len] == 0)
							break;
					}

					return (new ASCIIEncoding()).GetString(strBytes, 0, len);
				}
				set
				{
					byte			[]strBytes = new byte[32];
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					int				i;

					if (ascii.GetByteCount(value) > 32)
						SDKBase.checkError(-1);

					for (i = 0; i < 32; ++i)
						strBytes[i] = 0;

					ascii.GetBytes(value, 0, value.Length, strBytes, 0);

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfSetCALBadPixelIngressFile2(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();
				}
			}

			/// <summary>
			/// Bad pixel in file.
			/// </summary>
			public string BadPixelIngressFile3
			{
				get
				{
					byte		[]strBytes = new byte[32];
					GCHandle	gch;
					int			len;

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfGetCALBadPixelIngressFile3(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();

					for (len = 0; len < 32; ++len)
					{
						if (strBytes[len] == 0)
							break;
					}

					return (new ASCIIEncoding()).GetString(strBytes, 0, len);
				}
				set
				{
					byte			[]strBytes = new byte[32];
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					int				i;

					if (ascii.GetByteCount(value) > 32)
						SDKBase.checkError(-1);

					for (i = 0; i < 32; ++i)
						strBytes[i] = 0;

					ascii.GetBytes(value, 0, value.Length, strBytes, 0);

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfSetCALBadPixelIngressFile3(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();
				}
			}

			/// <summary>
			/// Bad pixel out file.
			/// </summary>
			public string BadPixelEgressFile
			{
				get
				{
					byte		[]strBytes = new byte[32];
					GCHandle	gch;
					int			len;

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfGetCALBadPixelEgressFile(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();

					for (len = 0; len < 32; ++len)
					{
						if (strBytes[len] == 0)
							break;
					}

					return (new ASCIIEncoding()).GetString(strBytes, 0, len);
				}
				set
				{
					byte			[]strBytes = new byte[32];
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					int				i;

					if (ascii.GetByteCount(value) > 32)
						SDKBase.checkError(-1);

					for (i = 0; i < 32; ++i)
						strBytes[i] = 0;

					ascii.GetBytes(value, 0, value.Length, strBytes, 0);

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfSetCALBadPixelEgressFile(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();
				}
			}

			/// <summary>
			/// Enable bad pixel detection.
			/// </summary>
			public bool BadPixelDetectionEnabled
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCALBadPixelDetectionState(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCALBadPixelDetectionState(id, (int)((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Enable bad pixel replacment.
			/// </summary>
			public bool BadPixelReplacementEnabled
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCALBadPixelReplacementState(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCALBadPixelReplacementState(id, (int)((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Number of frames to collect and average for BP detection.
			/// </summary>
			public ushort NumFramesToAverage
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetCALNumFramesToAverage(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetCALNumFramesToAverage(id, value));
				}
			}

			/// <summary>
			/// Automatically save NUC when performed flag.
			/// </summary>
			public bool AutoSaveNUCEnabled
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCALAutoSaveNucToFlashState(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCALAutoSaveNucToFlashState(id, (int)((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// NUC name for Load NUC.
			/// </summary>
			public string LoadNUCName
			{
				get
				{
					byte		[]strBytes = new byte[28];
					GCHandle	gch;
					int			len;

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfGetCALLoadNucName(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();

					for (len = 0; len < 28; ++len)
					{
						if (strBytes[len] == 0)
							break;
					}

					return (new ASCIIEncoding()).GetString(strBytes, 0, len);
				}
				set
				{
					byte			[]strBytes = new byte[28];
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					int				i;

					if (ascii.GetByteCount(value) > 28)
						SDKBase.checkError(-1);

					for (i = 0; i < 28; ++i)
						strBytes[i] = 0;

					ascii.GetBytes(value, 0, value.Length, strBytes, 0);

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfSetCALLoadNucName(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();
				}
			}

			/// <summary>
			/// Preset for Load NUC.
			/// </summary>
			public byte LoadNUCPreset
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCALLoadNucPresetSlot(id, out val));

					return (byte)val;
				}
				set
				{
					SDKBase.checkError(iacfSetCALLoadNucPresetSlot(id, (uint)value));
				}
			}

			/// <summary>
			/// Preset for Unload NUC.
			/// </summary>
			public byte UnloadNUCPreset
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCALUnloadNucPresetSlot(id, out val));

					return (byte)val;
				}
				set
				{
					SDKBase.checkError(iacfSetCALUnloadNucPresetSlot(id, (uint)value));
				}
			}

			/// <summary>
			/// Preset for Save NUC.
			/// </summary>
			public byte SaveNUCPreset
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCALSaveNucPresetSlot(id, out val));

					return (byte)val;
				}
				set
				{
					SDKBase.checkError(iacfSetCALSaveNucPresetSlot(id, (uint)value));
				}
			}

			/// <summary>
			/// NUC name for Delete NUC.
			/// </summary>
			public string DeleteNUCName
			{
				get
				{
					byte		[]strBytes = new byte[28];
					GCHandle	gch;
					int			len;

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfGetCALDeleteNucName(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();

					for (len = 0; len < 28; ++len)
					{
						if (strBytes[len] == 0)
							break;
					}

					return (new ASCIIEncoding()).GetString(strBytes, 0, len);
				}
				set
				{
					byte			[]strBytes = new byte[28];
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					int				i;

					if (ascii.GetByteCount(value) > 28)
						SDKBase.checkError(-1);

					for (i = 0; i < 28; ++i)
						strBytes[i] = 0;

					ascii.GetBytes(value, 0, value.Length, strBytes, 0);

					gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfSetCALDeleteNucName(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();
				}
			}
			
			/// <summary>
			/// Starts performing a NUC.  Periodically refresh the status to see if the NUC has been interrupted or is complete.
			/// </summary>
			public void StartNUC()
			{
				SDKBase.checkError(iacfCalStartNuc(id));
			}

			/// <summary>
			/// Continue performing a NUC after setting up a blackbody source or after an interruption.
			/// </summary>
			public void ContinueNUC()
			{
				SDKBase.checkError(iacfCalContinueNuc(id));
			}

			/// <summary>
			/// Abort performing a NUC.
			/// </summary>
			public void AbortNUC()
			{
				SDKBase.checkError(iacfCalAbortNuc(id));
			}

			/// <summary>
			/// Load LoadNUCName into current preset.
			/// </summary>
			public void LoadNUCIntoCurrentPreset()
			{
				SDKBase.checkError(iacfCalLoadNucCurrentPreset(id));
			}

			/// <summary>
			/// Load LoadNUCName into LoadNUCPreset.
			/// </summary>
			public void LoadNUCIntoPreset()
			{
				SDKBase.checkError(iacfCalLoadNucAtPreset(id));
			}

			/// <summary>
			/// Load the given nuc into the given preset.
			/// </summary>
			/// <param name="name">NUC name.</param>
			/// <param name="preset">Preset.</param>
			public void LoadNUCIntoPreset(string name, byte preset)
			{
				LoadNUCName = name;
				LoadNUCPreset = preset;
				LoadNUCIntoPreset();
			}

			/// <summary>
			/// Unload NUC from current preset.
			/// </summary>
			public void UnloadNUCFromCurrentPreset()
			{
				SDKBase.checkError(iacfCalUnloadNucCurrentPreset(id));
			}

			/// <summary>
			/// Unload NUC from UnloadNUCPreset.
			/// </summary>
			public void UnloadNUCFromPreset()
			{
				SDKBase.checkError(iacfCalUnloadNucAtPreset(id));
			}

			/// <summary>
			/// Unload NUC from given preset.
			/// </summary>
			/// <param name="preset">Preset.</param>
			public void UnloadNUCFromPreset(byte preset)
			{
				UnloadNUCPreset = preset;
				UnloadNUCFromPreset();
			}

			/// <summary>
			/// Save NUC at current preset.
			/// </summary>
			public void SaveNUCAtCurrentPreset()
			{
				SDKBase.checkError(iacfCalSaveNucCurrentPreset(id));
			}

			/// <summary>
			/// Save NUC at SaveNUCPreset.
			/// </summary>
			public void SaveNUCAtPreset()
			{
				SDKBase.checkError(iacfCalSaveNucAtPreset(id));
			}

			/// <summary>
			/// Save NUC at given preset.
			/// </summary>
			/// <param name="preset">Preset.</param>
			public void SaveNUCAtPreset(byte preset)
			{
				SaveNUCPreset = preset;
				SaveNUCAtPreset();
			}

			/// <summary>
			/// Delete DeleteNUCName from the camera.
			/// </summary>
			public void DeleteNUC()
			{
				SDKBase.checkError(iacfCalDeleteNuc(id));
			}

			/// <summary>
			/// Delete the given NUc from the camera.
			/// </summary>
			/// <param name="name">NUC name.</param>
			public void DeleteNUC(string name)
			{
				DeleteNUCName = name;
				DeleteNUC();
			}

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfPingCAL(int id);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALTag(int id, IntPtr Tag);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALTag(int id, IntPtr Tag);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALVersion(int id, out ushort version);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALVersion(int id, ushort version);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALNucType(int id, out ushort type);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALNucType(int id, ushort type);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALSource(int id, out ushort source);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALSource(int id, ushort source);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALBadPixelDetection(int id, out uint limitAdcHigh, out uint limitAdcLow, out uint responseHighPercent, out uint responseLowPercent, out uint percentBadBeforePause);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALBadPixelDetection(int id, uint limitAdcHigh, uint limitAdcLow, uint responseHighPercent, uint responseLowPercent, uint percentBadBeforePause);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALBadPixelIngressFile1( int id, IntPtr BadPixelIngressFile1 );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALBadPixelIngressFile1( int id, IntPtr BadPixelIngressFile1 );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALBadPixelIngressFile2( int id, IntPtr BadPixelIngressFile2 );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALBadPixelIngressFile2( int id, IntPtr BadPixelIngressFile2 );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALBadPixelIngressFile3( int id, IntPtr BadPixelIngressFile3 );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALBadPixelIngressFile3( int id, IntPtr BadPixelIngressFile3 );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALBadPixelEgressFile( int id, IntPtr BadPixelEgressFile );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALBadPixelEgressFile( int id, IntPtr BadPixelEgressFile );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALBadPixelDetectionState( int id, out int BadPixelDetectionState );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALBadPixelDetectionState( int id, int BadPixelDetectionState );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALBadPixelReplacementState( int id, out int BadPixelReplacementState );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALBadPixelReplacementState( int id, int BadPixelReplacementState );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALNumFramesToAverage( int id, out ushort NumFramesToAverage );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALNumFramesToAverage( int id, ushort NumFramesToAverage );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALCurrentStatus(int id, IntPtr message, out short code);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALAutoSaveNucToFlashState( int id, out int AutoSaveNucToFlashState );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALAutoSaveNucToFlashState( int id, int AutoSaveNucToFlashState );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALLoadNucName( int id, IntPtr LoadNucName );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALLoadNucName( int id, IntPtr LoadNucName );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALLoadNucPresetSlot( int id, out uint LoadNucPresetSlot );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALLoadNucPresetSlot( int id, uint LoadNucPresetSlot );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALUnloadNucPresetSlot( int id, out uint UnloadNucPresetSlot );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALUnloadNucPresetSlot( int id, uint UnloadNucPresetSlot );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALSaveNucPresetSlot( int id, out uint SaveNucPresetSlot );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALSaveNucPresetSlot( int id, uint SaveNucPresetSlot );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALDeleteNucName( int id, IntPtr DeleteNucName );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCALDeleteNucName( int id, IntPtr DeleteNucName );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCALBadPixelStatus(int id, 
											out uint counterBadAtHighAdcLimit,
											out uint counterBadAtLowAdcLimit,
											out uint counterBadAtHighPercent,
											out uint counterBadAtLowPercent,
											out uint counterTotalBadDetected,
											out uint counterAlt1,
											out uint counterAlt2,
											out uint counterAlt3,
											out uint counterAlt4,
											out uint counterTotalAltDetected);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfCalStartNuc(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfCalContinueNuc(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfCalAbortNuc(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfCalLoadNucCurrentPreset(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfCalLoadNucAtPreset(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfCalUnloadNucCurrentPreset(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfCalUnloadNucAtPreset(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfCalSaveNucCurrentPreset(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfCalSaveNucAtPreset(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfCalDeleteNuc(int id);
		}
		#endregion

		#region FPAControl
		/// <summary>
		/// Contains functions for controlling the FPA.
		/// </summary>
		public class FPAControl
		{
			/// <summary>Accessories SDK id to use when communicating.</summary>
			private int		id;
			/// <summary>
			/// Integration time scale factor
			/// </summary>
			private uint intTimeSF = 1000;
			/// <summary>
			/// Special integration time scale factor
			/// </summary>
			private uint specialIntTimeSF = 1000;
			/// <summary>
			/// Frame rate scale factor
			/// </summary>
			private ushort frameRateSF = 10;

			/// <summary>Integration mode.</summary>
			public enum IntSyncModeType
			{
				/// <summary>Async Triggered FPA Read</summary>
				ATFR		= 0,
				/// <summary>Async Integrate Then Read</summary>
				AITR		= 1,
				/// <summary>Async Integrate While Read</summary>
				AIWR		= 2,
				/// <summary>Special Integrate Then Read</summary>
				SpITR		= 3,
				/// <summary>Special Async Triggered FPA Read</summary>
				SpATFR		= 4
			}

			/// <summary>
			/// AccessoriesSDK control channel id.
			/// </summary>
			public int ID
			{
				set
				{
					id = value;
					windowSize.id = value;
					windowOrigin.id = value;
                    maxWindowSize.id = value;
				}
			}

			/// <summary>
			/// Ping the module for health status.
			/// </summary>
			public void Ping()
			{
				SDKBase.checkError(iacfPingFPA(id));
			}

			/// <summary>
			/// Integration mode.
			/// </summary>
			public IntSyncModeType IntSyncMode
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetFPAIntSyncMode(id, out val));

					return (IntSyncModeType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAIntSyncMode(id, (int)value));
				}
			}

			/// <summary>
			/// Get/set integration time in TODO: Get units.
			/// </summary>
			public double IntegrationTime
			{
				get
				{
					uint	val = 0;

					SDKBase.checkError(iacfGetFPAIntTime(id, out val));
					return (val / (double)intTimeSF);
				}
				set
				{
					uint scaledValue = (uint)Math.Round((value * (double)intTimeSF));
					SDKBase.checkError(iacfSetFPAIntTime(id, scaledValue));
				}
			}

			/// <summary>
			/// Get/set special (short) integration time in TODO: Get units.
			/// </summary>
			public double SpecialIntegrationTime
			{
				get
				{
					uint	val = 0;

					SDKBase.checkError(iacfGetFPASpecialIntTime(id, out val));

					return (Double)((Double)val / (Double)specialIntTimeSF);
				}
				set
				{
					uint scaledValue = (uint) Math.Round (value * (double)specialIntTimeSF);
					SDKBase.checkError(iacfSetFPASpecialIntTime(id, scaledValue));
				}
			}

			/// <summary>
			/// Get/set image invert (flip vertically) flag.
			/// </summary>
			public bool Invert
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetFPARowReadoutOrder(id, out val));

					return (val != 0);
				}
				set
				{
					int		val = 0;

					if (value)
						val = 1;

					SDKBase.checkError(iacfSetFPARowReadoutOrder(id, val));
				}
			}

			/// <summary>
			/// Get/set image revert (flip horizontally) flag.
			/// </summary>
			public bool Revert
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetFPAColReadoutOrder(id, out val));

					return (val != 0);
				}
				set
				{
					int		val = 0;

					if (value)
						val = 1;

					SDKBase.checkError(iacfSetFPAColReadoutOrder(id, val));
				}
			}

			/// <summary>
			/// Get/set the frame rate in Hz.
			/// </summary>
			public double FrameRate
			{
				get
				{
					uint	val = 0;

					SDKBase.checkError(iacfGetFPAFrameRate(id, out val));

					return (val / (double)frameRateSF);
				}
				set
				{
					uint scaledValue = (uint) Math.Round ((value * (double)frameRateSF));
					SDKBase.checkError(iacfSetFPAFrameRate(id, scaledValue));
				}
			}

			/// <summary>
			/// Holds the size of the window.
			/// </summary>
			public class WindowSizeData
			{
				/// <summary>Accessories SDK control device to use.</summary>
				public int		id = -1;
				protected short	width;
				protected short	height;
				protected bool	cache = false;

				virtual protected void Read()
				{
					SDKBase.checkError(ApacheSDK.FPAControl.iacfGetFPAWindowSize(id, out height, out width));
				}

				virtual protected void Write()
				{
					SDKBase.checkError(ApacheSDK.FPAControl.iacfSetFPAWindowSize(id, height, width));
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// Width.
				/// </summary>
				public short Width
				{
					get
					{
						if (!cache)
							Read();

						return width;
					}
					set
					{
						if (!cache)
							Read();

						width = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Height.
				/// </summary>
				public short Height
				{
					get
					{
						if (!cache)
							Read();

						return height;
					}
					set
					{
						if (!cache)
							Read();

						height = value;

						if (!cache)
							Write();
					}
				}
			}

			private WindowSizeData	windowSize = new WindowSizeData();

			/// <summary>
			/// Gets the window size container class.
			/// </summary>
			public WindowSizeData WindowSize
			{
				get
				{
					return windowSize;
				}
			}

			/// <summary>
			/// Holds the window origin information.
			/// </summary>
			public class WindowOriginData
			{
				/// <summary>Accessories SDK control device to use.</summary>
				public int		id = -1;
				protected short	xoffset;
				protected short	yoffset;
				private bool	cache = false;

				virtual protected void Read()
				{
					SDKBase.checkError(ApacheSDK.FPAControl.iacfGetFPAWindowOrigin(id, out yoffset, out xoffset));
				}

				virtual protected void Write()
				{
					SDKBase.checkError(ApacheSDK.FPAControl.iacfSetFPAWindowOrigin(id, yoffset, xoffset));
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// X offset.
				/// </summary>
				public short XOffset
				{
					get
					{
						if (!cache)
							Read();

						return xoffset;
					}
					set
					{
						if (!cache)
							Read();

						xoffset = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Y offset.
				/// </summary>
				public short YOffset
				{
					get
					{
						if (!cache)
							Read();

						return yoffset;
					}
					set
					{
						if (!cache)
							Read();

						yoffset = value;

						if (!cache)
							Write();
					}
				}
			}

			private WindowOriginData	windowOrigin = new WindowOriginData();

			/// <summary>
			/// Gets the window origin container class.
			/// </summary>
			public WindowOriginData WindowOrigin
			{
				get
				{
					return windowOrigin;
				}
			}

			/// <summary>
			/// Returns true if the FPA is initialized.
			/// </summary>
			public bool FPAInitialized
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetFPAInitialized(id, out val));

					return (val != 0);
				}
			}

			/// <summary>
			/// FPA gain level.
			/// </summary>
			public byte Gain
			{
				get
				{
					byte	val;

					SDKBase.checkError(iacfGetFPAGain(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAGain(id, value));
				}
			}

			/// <summary>
			/// Detecotr bias in millivolts.
			/// </summary>
			public uint DetectorBias
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetFPADetectorBiasMv(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPADetectorBiasMv(id, value));
				}
			}

			/// <summary>
			/// FPA control word.
			/// </summary>
			public uint ControlWord
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetFPAControlWord(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAControlWord(id, value));
				}
			}

			/// <summary>
			/// Number of FPA readout channels.
			/// </summary>
			public ushort ReadoutChannels
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetFPAReadoutChannels(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAReadoutChannels(id, value));
				}
			}

			/// <summary>
			/// Dead rows in the readout.
			/// </summary>
			public ushort ReadoutDeadRows
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetFPAReadoutDeadRows(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAReadoutDeadRows(id, value));
				}
			}

			/// <summary>
			/// Dead columns in the readout.
			/// </summary>
			public ushort ReadoutDeadColumns
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetFPAReadoutDeadColumns(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAReadoutDeadColumns(id, value));
				}
			}

			/// <summary>
			/// Overhead per row in ticks.
			/// </summary>
			public ushort OverheadPerRow
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetFPAOverheadPerRowTicks(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAOverheadPerRowTicks(id, value));
				}
			}

			/// <summary>
			/// Overhead per frame in rows.
			/// </summary>
			public ushort OverheadPerFrameRows
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetFPAOverheadPerFrameRows(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAOverheadPerFrameRows(id, value));
				}
			}

			/// <summary>
			/// Overhead per frame in ticks.
			/// </summary>
			public ushort OverheadPerFrameTicks
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetFPAOverheadPerFrameTicks(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAOverheadPerFrameTicks(id, value));
				}
			}

			/// <summary>
			/// Internal clock delay.
			/// </summary>
			public ushort InternalClockDelay
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetFPAInternalClockDelayTicks(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAInternalClockDelayTicks(id, value));
				}
			}

			/// <summary>
			/// Extra analog time.
			/// </summary>
			public ushort ExtraAnalogTime
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetFPAExtraAnalogTimeNsecs(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAExtraAnalogTimeNsecs(id, value));
				}
			}

			/// <summary>
			/// Minimum # of clocks for FSync to remain high.
			/// </summary>
			public ushort MinFSyncHigh
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetFPAMinFsyncHighTicks(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAMinFsyncHighTicks(id, value));
				}
			}

			/// <summary>
			/// Image generation priority.
			/// </summary>
			public enum ImageGenerationPriorityType
			{
				/// <summary>No priority.</summary>
				None,
				/// <summary>Integration time has priority, drives frame rate if overlap.</summary>
				IntegrationTime,
				/// <summary>Frame rate has priority, drives integration time if overlap.</summary>
				FrameRate
			};

			/// <summary>
			/// Image generation priority, the given value with drive the others.
			/// </summary>
			public ImageGenerationPriorityType ImageGenerationPriority
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetFPAImageGenPriority(id, out val));

					return (ImageGenerationPriorityType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAImageGenPriority(id, (int)value));
				}
			}

			/// <summary>
			/// Readout mode.
			/// </summary>
			public enum ReadoutModeType
			{
				/// <summary>Readout is normal order.</summary>
				Normal,
				/// <summary>Readout is interlaced.</summary>
				Interlaced
			};

			/// <summary>
			/// Readout mode (normal / interlaced).
			/// </summary>
			public ReadoutModeType ReadoutMode
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetFPAReadoutMode(id, out val));

					return (ReadoutModeType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetFPAReadoutMode(id, (int)value));
				}
			}

            /// <summary>
            /// FPA State).
            /// </summary>
            public enum FPAStateType
            {
                /// <summary>Focal Plan Array is off.</summary>
                FPAOff = 0,
                /// <summary>Focal Plan Array is on.</summary>
                FPAOn  = 1,
                /// <summary>Focal Plan Array is in auto off/on temperature limit mode.</summary>
                FPAAuto = 2
            };
            
            /// <summary>
			/// FPA enable flag.
			/// </summary>
			public FPAStateType State
			{
				get
				{
					uint		val;

					SDKBase.checkError(iacfGetFPAState(id, out val));

					return (FPAStateType)val;
				}
                set
                {
                    uint val = 0;
                    if (value == FPAStateType.FPAAuto)
                        val = 2;
                    else if (value == FPAStateType.FPAOn)
                        val = 1;
                    if ( iacfSetFPAState(id, val)  == (int)SDKBase.sdkError.errSemaphoreTimeout )
                    {
                        Thread.Sleep(100);
                        SDKBase.checkError(iacfSetFPAState(id, val));
                    }
                }
                   
			}

			/// <summary>
			/// Skim voltage enable status.
			/// </summary>
			public bool SkimVoltageEnabled
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetFPASkimMode(id, out val));

					return ((val & 0xFF) != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetFPASkimMode(id, ((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Window row granularity.
			/// </summary>
			public byte WindowRowPrecision
			{
				get
				{
					byte	val;

					SDKBase.checkError(iacfGetFPAWindowRowPrecision(id, out val));

					return val;
				}
			//	set
			//	{
			//		SDKBase.checkError(iacfSetFPAWindowRowPrecision(id, value));
			//	}
			}

			/// <summary>
			/// Window column granularity.
			/// </summary>
			public byte WindowColPrecision
			{
				get
				{
					byte	val;

					SDKBase.checkError(iacfGetFPAWindowColPrecision(id, out val));

					return val;
				}
			//	set
			//	{
			//		SDKBase.checkError(iacfSetFPAWindowColPrecision(id, value));
			//	}
			}

			/// <summary>
			/// Available detector types.
			/// </summary>
			public enum DetectorType
			{
				/// <summary>InSb</summary>
				InSb,
				/// <summary>InGaAs (pre Indigo)</summary>
				InGaAs,
				/// <summary>InGaAs (Indigo)</summary>
				InGaAsISC,
				/// <summary>VisGaAs</summary>
				VisGaAs,
                /// <summary>QWIP detector</summary>
                QWIP,
                /// <summary>Unknown detector</summary>
				Unknown
			}

			/// <summary>
			/// Gets the detector in the camera.
			/// </summary>
			public DetectorType Detector
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetFPADetectorType(id, out val));

					return (DetectorType)val;
				}
			}

			/// <summary>
			/// Gets the readout in the camera.
			/// </summary>
			public ushort Readout
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetFPADeviceType(id, out val));

					return (ushort)val;
				}
			}


            /// <summary>
            /// Holds the maximum size of the window.
            /// </summary>
            public class MaxWindowSizeData : WindowSizeData
            {
                /// <summary>Accessories SDK data object to use.</summary>

                override protected void Read()
                {
                    SDKBase.checkError(ApacheSDK.FPAControl.iacfGetFPAMaximumWindowSize(id, out height, out width));
                }

                override protected void Write()
                {
                    SDKBase.checkError(ApacheSDK.FPAControl.iacfSetFPAMaximumWindowSize(id, height, width));
                }
            }


            private MaxWindowSizeData	maxWindowSize = new MaxWindowSizeData();

            /// <summary>
            /// Gets the window size container class.
            /// </summary>
            public MaxWindowSizeData MaxWindowSize
            {
                get
                {
                    return maxWindowSize;
                }
            }

			/// <summary>
            /// Holds the minimum size of the window.
            /// </summary>
            public class MinWindowSizeData : WindowSizeData
            {
                /// <summary>Accessories SDK data object to use.</summary>

                override protected void Read()
                {
                    SDKBase.checkError(ApacheSDK.FPAControl.iacfGetFPAMinimumWindowSize(id, out height, out width));
                }

                override protected void Write()
                {
                    SDKBase.checkError(ApacheSDK.FPAControl.iacfSetFPAMinimumWindowSize(id, height, width));
                }
            }


            private MinWindowSizeData	minWindowSize = new MinWindowSizeData();

            /// <summary>
            /// Gets the min window size container class.
            /// </summary>
            public MinWindowSizeData MinWindowSize
            {
                get
                {
                    return minWindowSize;
                }
            }
            
            
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPADeviceType(int id, out int deviceType);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPADetectorType(int id, out int detectorType);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfPingFPA(int id);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAActivePreset(int id, out uint preset);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAActivePreset(int id, uint preset);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAVersion(int id, out int version);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAColReadoutOrder(int id, out int colReadoutOrder);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAColReadoutOrder(int id, int colReadoutOrder);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPARowReadoutOrder(int id, out int mode);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPARowReadoutOrder(int id, int mode);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAWindowSize(int id, out short height, out short width);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAWindowSize(int id, short height, short width);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAWindowOrigin(int id, out short xoffset, out short yoffset);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAWindowOrigin(int id, short xoffset, short yoffset);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAIntSyncMode(int id, out int mode);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAIntSyncMode(int id, int mode);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPASpecialIntTime(int id, out uint itime);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPASpecialIntTime(int id, uint itime);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAControlWord(int id, out uint value);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAControlWord(int id, uint value);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPADetectorBiasMv(int id, out uint bias);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPADetectorBiasMv(int id, uint bias);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAState(int id, out uint state);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAState(int id, uint state);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAIntTime(int id, out uint itime);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAIntTime(int id, uint itime);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAFrameRate(int id, out uint rate);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAFrameRate(int id, uint rate);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAInitialized(int id, out ushort rate);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAGain(int id, out byte gain);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAGain(int id, byte gain);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAReadoutDeadRows(int id, out ushort ReadoutDeadRows);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAReadoutDeadRows(int id, ushort ReadoutDeadRows);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAReadoutDeadColumns(int id, out ushort ReadoutDeadColumns);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAReadoutDeadColumns(int id, ushort ReadoutDeadColumns);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAOverheadPerRowTicks(int id, out ushort OverheadPerRowTicks);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAOverheadPerRowTicks(int id, ushort OverheadPerRowTicks);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAOverheadPerFrameRows(int id, out ushort OverheadPerFrameRows);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAOverheadPerFrameRows(int id, ushort OverheadPerFrameRows);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAOverheadPerFrameTicks(int id, out ushort OverheadPerFrameTicks);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAOverheadPerFrameTicks(int id, ushort OverheadPerFrameTicks);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAInternalClockDelayTicks(int id, out ushort InternalClockDelayTicks);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAInternalClockDelayTicks(int id, ushort InternalClockDelayTicks);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAExtraAnalogTimeNsecs(int id, out ushort ExtraAnalogTimeNsecs);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAExtraAnalogTimeNsecs(int id, ushort ExtraAnalogTimeNsecs);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAMinFsyncHighTicks(int id, out ushort MinFsyncHighTicks);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAMinFsyncHighTicks(int id, ushort MinFsyncHighTicks);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAReadoutChannels(int id, out ushort ReadoutChannels);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAReadoutChannels(int id, ushort ReadoutChannels);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAImageGenPriority(int id, out int ImageGenPriority);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAImageGenPriority(int id, int ImageGenPriority);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAReadoutMode(int id, out int ReadoutMode);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAReadoutMode(int id, int ReadoutMode);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPASkimMode( int id, out int SkimMode );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPASkimMode( int id, int SkimMode );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAWindowColPrecision(int id, out byte WindowColPrecision);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAWindowColPrecision(int id, byte WindowColPrecision);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetFPAWindowRowPrecision(int id, out byte WindowRowPrecision);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetFPAWindowRowPrecision(int id, byte WindowRowPrecision);
        
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetFPAMaximumWindowSize(int id, out short nRows, out short nCols);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetFPAMaximumWindowSize(int id, short nRows, short nCols);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetFPAMinimumWindowSize(int id, out short nRows, out short nCols);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetFPAMinimumWindowSize(int id, short nRows, short nCols);
        }
		#endregion

		#region FPACooler
		/// <summary>
		/// Contains functions for controlling the FPA cooler.
		/// </summary>
		public class FPACooler
		{
			private int		id = -1;

            /// <summary>
            /// FPA Auto off/on flag
            /// </summary>
            public enum FPAAutoOffOnType
            {
                /// <summary>Automatic FPA turn off/on at temp limit is OFF</summary>
                AutoOff,
                /// <summary>Automatic FPA turn off/on at temp limit is ON</summary>
                AutoOn
            }
            
            public enum FpaCoolerStateType
            {
                /// <summary>Cooler state is off</summary>
                CoolerOff,
                /// <summary>Cooler state is ON</summary>
                CoolerOn
            }

            /// <summary>
			/// Accessories SDK control device to use.
			/// </summary>
			public int ID
			{
				set
				{
					id = value;
				}
			}

			/// <summary>
			/// Ping the module for health status.
			/// </summary>
			public void Ping()
			{
				SDKBase.checkError(iacfPingCLR(id));
			}

			/// <summary>
			/// FPA cooler enable status.
			/// </summary>
			public bool Enabled
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCLRCoolerState(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCLRCoolerState(id, ((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Temperature setpoint.
			/// </summary>
			public float FPATemperatureSetpoint
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCLRTempSetpoint(id, out val));

					return val * 0.001f;
				}
				set
				{
					SDKBase.checkError(iacfSetCLRTempSetpoint(id, (uint)(value / 0.001)));
				}
			}

			/// <summary>
			/// Get the current FPA Temp.
			/// </summary>
			public float FPATemp
			{
				get
				{
					uint	val = 0;

					SDKBase.checkError(iacfGetCLRTempActual(id, out val));
					return (float)(val / (float)1000.0);
				}
			}

			/// <summary>
			/// High temperature setpoint.
			/// </summary>
			public float HighTemperatureSetpoint
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCLRHighTempSetpoint(id, out val));

					return val * 0.001f;
				}
				set
				{
					SDKBase.checkError(iacfSetCLRHighTempSetpoint(id, (uint)(value / 0.001)));
				}
			}

			/// <summary>
			/// Low temperature setpoint.
			/// </summary>
			public float LowTemperatureSetpoint
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCLRLowTempSetpoint(id, out val));

					return val * 0.001f;
				}
				set
				{
					SDKBase.checkError(iacfSetCLRLowTempSetpoint(id, (uint)(value / 0.001)));
				}
			}

			/// <summary>
			/// Gets elapsed time (running time) of the cooler.
			/// </summary>
			public uint ElapsedTime
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCLRCoolerElapsedTime(id, out val));

					return val;
				}
                set
                {
                    SDKBase.checkError(iacfSetCLRCoolerElapsedTime(id, value));
                }
			}

			/// <summary>
			/// Cooler idle status.
			/// </summary>
			public bool Idle
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCLRCoolerRunMode(id, out val));

					return (val == 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCLRCoolerRunMode(id, ((value) ? 0 : 1)));
				}
			}

			/// <summary>
			/// Cooler types.
			/// </summary>
			public enum CoolerTypeType
			{
				/// <summary>No cooler</summary>
				None,
				/// <summary>TE Cooler</summary>
				TE,
				/// <summary>Mechanical Cooler (Closed Cycle Cooler)</summary>
				Mechanical
			}

			/// <summary>
			/// Gets the cooler type in the camera.
			/// </summary>
			public CoolerTypeType CoolerType
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCLRCoolerType(id, out val));

					return (CoolerTypeType)val;
				}
			}

            /// <summary>
            /// Get the current FPA Auto Turn On Temperature Limit
            /// </summary>
            public float FPAAutoTurnOnTempLimit
            {
                get
                {
                    uint	val = 0;

                    SDKBase.checkError(iacfGetCLRFpaAutoTurnOnTempLimit(id, out val));
                    return (float)(val / (float)1000.0);
                }
                set
                {
                    SDKBase.checkError(iacfSetCLRFpaAutoTurnOnTempLimit(id, (uint)(value * 1000.0)));
                }
            }

            /// <summary>
            /// Gets/sets FPA cooler state 
            /// </summary>
            public FpaCoolerStateType CoolerState
            {
                get
                {
                    int val = 0;
                    SDKBase.checkError(iacfGetCLRCoolerState(id, out val));
                    return (FpaCoolerStateType)val;
                }
                set
                {
                    SDKBase.checkError(iacfSetCLRCoolerState(id, (int)value));
                }
            }
            
            /// <summary>
            /// Get the current FPA Auto Turn Off Temperature Limit
            /// </summary>
            public float FPAAutoTurnOffTempLimit
            {
                get
                {
                    uint	val = 0;

                    SDKBase.checkError(iacfGetCLRFpaAutoTurnOffTempLimit(id, out val));
                    return (float)(val / (float)1000.0);
                }
                set
                {
                    SDKBase.checkError(iacfSetCLRFpaAutoTurnOffTempLimit(id, (uint)(value * 1000.0)));
                }
            }

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfPingCLR(int id);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCLRCoolerState( int id, out int CoolerState );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCLRCoolerState( int id, int CoolerState );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCLRTempSetpoint( int id, out uint FpaTempSetPoint );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCLRTempSetpoint( int id, uint FpaTempSetPoint );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCLRTempActual(int id, out uint FpaTempActual);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCLRHighTempSetpoint( int id, out uint HighTempSetPoint );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCLRHighTempSetpoint( int id, uint HighTempSetPoint );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCLRLowTempSetpoint( int id, out uint LowTempSetPoint );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCLRLowTempSetpoint( int id, uint LowTempSetPoint );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCLRCoolerElapsedTime( int id, out uint ElapsedTime );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCLRCoolerElapsedTime( int id, uint ElapsedTime );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCLRCoolerType( int id, out int CoolerType );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCLRCoolerRunMode(int id, out int CoolerRunMode);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCLRCoolerRunMode(int id, int CoolerRunMode);
        
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCLRFpaAutoTurnOnTempLimit( int id, out uint FpaAutoTurnOnTempLimit );
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCLRFpaAutoTurnOnTempLimit( int id, uint FpaAutoTurnOnTempLimit );

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCLRFpaAutoTurnOffTempLimit( int id, out uint FpaAutoTurnOffTempLimit );
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCLRFpaAutoTurnOffTempLimit( int id, uint FpaAutoTurnOffTempLimit );

        }
		#endregion

		#region HiddenCooler
		/// <summary>
		/// Contains functions for controlling the Hidden FPA cooler.
		/// </summary>
		public class HiddenCooler
		{
			private int		id = -1;
            private const float lm50AdcFullScale = (float)2.5;
            private const float flagFullScale24Bits = (float)0xffffff;

			/// <summary>
			/// Accessories SDK control device to use.
			/// </summary>
			public int ID
			{
				set
				{
					id = value;
					teControl.id = value;
                    flagPosition.id = value;
                    flagMotorControl.id = value;
				}
			}

			/// <summary>
			/// Ping the module for health status.
			/// </summary>
			public void Ping()
			{
				SDKBase.checkError(iacfPingHCLR(id));
			}


            /// <summary>
            /// Defines the flag motor control container class.
            /// </summary>
            public class FlagMotorControlData
            {
                /// <summary>AccessoriesSDK control channel id.</summary>
                public int		    id = -1;
                private short	    accelRate = 0;
                private short       minMotorSpeed = 0;
                private short       posnMoveMaxMotorSpeed = 0;
                private short       findHomeMaxSpeed = 0;
                private short       maxPossibleSteps = 0;
                private short       maxBackupSteps = 0;
                private bool	    cache = false;

                private void Read()
                {
                    SDKBase.checkError(iacfGetHCLRFlagMotorControl(id, out accelRate, out minMotorSpeed, out posnMoveMaxMotorSpeed, out findHomeMaxSpeed, out maxPossibleSteps, out maxBackupSteps));
                }

                private void Write()
                {
                    SDKBase.checkError(iacfSetHCLRFlagMotorControl(id, accelRate, minMotorSpeed, posnMoveMaxMotorSpeed, findHomeMaxSpeed, maxPossibleSteps, maxBackupSteps));
                }

                /// <summary>
                /// Read all dwell counts (struct).
                /// </summary>
                public void CacheRead()
                {
                    Read();

                    cache = true;
                }

                /// <summary>
                /// Write previously read dwell counts (struct).
                /// </summary>
                public void CacheWrite()
                {
                    cache = false;

                    Write();
                }

                /// <summary>
                /// Discard previously read dwell counts (struct).
                /// </summary>
                public void CacheClear()
                {
                    cache = false;
                }

                /// <summary>
                /// Acceleration rate
                /// </summary>
                public Int16 AccelRate
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return accelRate;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        accelRate = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Minimum motor speed
                /// </summary>
                public Int16 MinMotorSpeed
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return minMotorSpeed;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        minMotorSpeed = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Position move max motor speed
                /// </summary>
                public Int16 PosnMoveMaxMotorSpeed
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return posnMoveMaxMotorSpeed;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        posnMoveMaxMotorSpeed = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Minimum motor spee for find home
                /// </summary>
                public Int16 FindHomeMaxSpeed
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return findHomeMaxSpeed;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        findHomeMaxSpeed = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Maximum backup steps
                /// </summary>
                public Int16 MaxBackupSteps
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return maxBackupSteps;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        maxBackupSteps = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                ///Maximum possible steps
                /// </summary>
                public Int16 MaxPossibleSteps
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return maxPossibleSteps;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        maxPossibleSteps = value;

                        if (!cache)
                            Write();
                    }
                }

            }

            private FlagMotorControlData	flagMotorControl = new FlagMotorControlData();

            /// <summary>
            /// Gets the flag postion control container class.
            /// </summary>
            public FlagMotorControlData FlagMotorControl
            {
                get
                {
                    return flagMotorControl;
                }
            }
            
            
            
            /// <summary>
            /// Defines the flag position data container class.
            /// </summary>
            public class FlagPositionData
            {
                /// <summary>AccessoriesSDK control channel id.</summary>
                public int		    id = -1;
                private short	    seeSpotFilter = 0;
                private short       imagingFilter = 0;
                private short       coldSrc = 0;
                private short       hotSrc = 0;
                private short       sensorCal = 0;
                private bool	    cache = false;

                private void Read()
                {
                    SDKBase.checkError(iacfGetHCLRFlagPositions(id, out seeSpotFilter, out imagingFilter, out coldSrc, out hotSrc, out sensorCal));
                }

                private void Write()
                {
                    SDKBase.checkError(iacfSetHCLRFlagPositions(id, seeSpotFilter, imagingFilter, coldSrc, hotSrc, sensorCal));
                }

                /// <summary>
                /// Read all dwell counts (struct).
                /// </summary>
                public void CacheRead()
                {
                    Read();

                    cache = true;
                }

                /// <summary>
                /// Write previously read dwell counts (struct).
                /// </summary>
                public void CacheWrite()
                {
                    cache = false;

                    Write();
                }

                /// <summary>
                /// Discard previously read dwell counts (struct).
                /// </summary>
                public void CacheClear()
                {
                    cache = false;
                }

                /// <summary>
                /// See spot filter flag position
                /// </summary>
                public Int16 SeeSpotFilter
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return seeSpotFilter;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        seeSpotFilter = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Imaging filter flag position
                /// </summary>
                public Int16 ImagingFilter
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return imagingFilter;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        imagingFilter = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Cold source flag position
                /// </summary>
                public Int16 ColdSrc
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return coldSrc;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        coldSrc = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Hot source flag position
                /// </summary>
                public Int16 HotSrc
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return hotSrc;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        hotSrc = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                ///Sensor Calibration flag position
                /// </summary>
                public Int16 SensorCal
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return sensorCal;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        sensorCal = value;

                        if (!cache)
                            Write();
                    }
                }

            }

            private FlagPositionData	flagPosition = new FlagPositionData();

            /// <summary>
            /// Gets the flag postion control container class.
            /// </summary>
            public FlagPositionData FlagPosition
            {
                get
                {
                    return flagPosition;
                }
            }
            
            /// <summary>
			/// Gets the TE control container class.
			/// </summary>
			public class TEControlData
			{
				/// <summary>AccessoriesSDK control channel id.</summary>
				public int		id = -1;
				private int		propGain, intGain, diffGain;
				private uint	clampHigh, clampLow;
				private uint	tempSetpoint, currentTemp, currentOutput;
				private uint	pterm, iterm, dterm;
				private bool	cache = false;

				private void Read()
				{
					SDKBase.checkError(iacfGetHCLRTECControlParams(id, out propGain, out intGain, out diffGain, out clampHigh, out clampLow, out tempSetpoint, out currentTemp, out currentOutput, out pterm, out iterm, out dterm));
				}

				private void Write()
				{
					SDKBase.checkError(iacfSetHCLRTECControlParams(id, propGain, intGain, diffGain, clampHigh, clampLow, tempSetpoint, currentTemp, currentOutput, pterm, iterm, dterm));
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// p term to use.
				/// </summary>
				public int ProportionalGain
				{
					get
					{
						if (!cache)
							Read();

						return propGain;
					}
					set
					{
						if (!cache)
							Read();

						propGain = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// i term to use.
				/// </summary>
				public int IntegralGain
				{
					get
					{
						if (!cache)
							Read();

						return intGain;
					}
					set
					{
						if (!cache)
							Read();

						intGain = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// d term to use.
				/// </summary>
				public int DerivativeGain
				{
					get
					{
						if (!cache)
							Read();

						return diffGain;
					}
					set
					{
						if (!cache)
							Read();

						diffGain = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// High clamp.
				/// </summary>
				public uint ClampHigh
				{
					get
					{
						if (!cache)
							Read();

						return clampHigh;
					}
					set
					{
						if (!cache)
							Read();

						clampHigh = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Low clamp.
				/// </summary>
				public uint ClampLow
				{
					get
					{
						if (!cache)
							Read();

						return clampLow;
					}
					set
					{
						if (!cache)
							Read();

						clampLow = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Temperature setpoint.
				/// </summary>
				public uint TemperatureSetpoint
				{
					get
					{
						if (!cache)
							Read();

						return tempSetpoint;
					}
					set
					{
						if (!cache)
							Read();

						tempSetpoint = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Current temperature (read only).
				/// </summary>
				public float CurrentTemperature
				{
					get
					{
                        float retVal = 0;
                        float fvolts = (float)0.0;
                        if (!cache)
							Read();

						retVal = 100 * ((lm50AdcFullScale * (float)currentTemp / flagFullScale24Bits) - (float)0.5);
                        return retVal;
					}
					set
					{
						float tmpValue = (float)0;
                        if (!cache)
							Read();

						tmpValue = value / (float)100 * ((flagFullScale24Bits / lm50AdcFullScale) - (float) 0.5);
                        currentTemp = (uint)tmpValue;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Current output (read only).
				/// </summary>
				public uint CurrentOutput
				{
					get
					{
						if (!cache)
							Read();

						return currentOutput;
					}
					set
					{
						if (!cache)
							Read();

						currentOutput = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// current p term (read only).
				/// </summary>
				public uint ProportionalTerm
				{
					get
					{
						if (!cache)
							Read();

						return pterm;
					}
					set
					{
						if (!cache)
							Read();

						pterm = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// current i term (read only).
				/// </summary>
				public uint IntegralTerm
				{
					get
					{
						if (!cache)
							Read();

						return iterm;
					}
					set
					{
						if (!cache)
							Read();

						iterm = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// current d term (read only).
				/// </summary>
				public uint DerivativeTerm
				{
					get
					{
						if (!cache)
							Read();

						return dterm;
					}
					set
					{
						if (!cache)
							Read();

						dterm = value;

						if (!cache)
							Write();
					}
				}
			}

			private TEControlData		teControl = new TEControlData();

			/// <summary>
			/// Gets the TE control container class.
			/// </summary>
			public TEControlData TEControl
			{
				get
				{
					return teControl;
				}
			}

			/// <summary>
			/// Temperature sensors
			/// </summary>
			public enum TemperatureSenseSourceType
			{
				/// <summary>Use readout temperature.</summary>
				Readout		= 0,
				/// <summary>Use diode temperature.</summary>
				Diode		= 1
			}

			/// <summary>
			/// Temperature sensor to use.
			/// </summary>
			public TemperatureSenseSourceType TemperatureSenseSource
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetHCLRTempSenseSource(id, out val));

					return (TemperatureSenseSourceType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetHCLRTempSenseSource(id, (int)value));
				}
			}

			/// <summary>
			/// Readout sense source slope.
			/// </summary>
			public float ReadoutTemperatureSenseSlope
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetHCLRTempSenseSlope(id, out val));

					return val * 0.001f;
				}
				set
				{
					SDKBase.checkError(iacfSetHCLRTempSenseSlope(id, (int)Math.Round(value / 0.001f)));
				}
			}

			/// <summary>
			/// Readout sense source offset.
			/// </summary>
			public float ReadoutTemperatureSenseOffset
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetHCLRTempSenseOffset(id, out val));

					return val * 0.001f;
				}
				set
				{
					SDKBase.checkError(iacfSetHCLRTempSenseOffset(id, (int)Math.Round(value / 0.001f)));
				}
			}

			/// <summary>
			/// Diode sense mode slope.
			/// </summary>
			public float DiodeTemperatureSenseSlope
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetHCLRTempSenseDiodeSlope(id, out val));

					return val * 0.001f;
				}
				set
				{
					SDKBase.checkError(iacfSetHCLRTempSenseDiodeSlope(id, (int)Math.Round(value / 0.001f)));
				}
			}

			/// <summary>
			/// Diode sense source offset.
			/// </summary>
			public float DiodeTemperatureSenseOffset
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetHCLRTempSenseDiodeOffset(id, out val));

					return val * 0.001f;
				}
				set
				{
					SDKBase.checkError(iacfSetHCLRTempSenseDiodeOffset(id, (int)Math.Round(value / 0.001f)));
				}
			}

			/// <summary>
			/// Seconds per tick in elapsed time.
			/// </summary>
			public uint ElapsedTimerTick
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetHCLRFPACoolerElapsedTimerTick(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetHCLRFPACoolerElapsedTimerTick(id, value));
				}
			}

			/// <summary>
			/// Start the TE cooler.
			/// </summary>
			public void TECStart()
			{
				SDKBase.checkError(iacfHclrTecStart(id));
			}

			/// <summary>
			/// Stop the TE cooler.
			/// </summary>
			public void TECStop()
			{
				SDKBase.checkError(iacfHclrTecStop(id));
			}

			/// <summary>
			/// Resume the TE cooler.
			/// </summary>
			public void TECResume()
			{
				SDKBase.checkError(iacfHclrTecResume(id));
			}

			/// <summary>
			/// Reset the elapsed time to 0.
			/// </summary>
			public void ResetTimer()
			{
				SDKBase.checkError(iacfHclrTimerReset(id));
			}

            /// <summary>
            /// Pre defined states of the flag postion
            /// </summary>
            public enum   FlagStateType
            {   
                /// <summary>Image Position.</summary>
                ImageFilter,
                /// <summary>Sea Spot position.</summary>
                SeaSpotFilter,
                /// <summary>Cold source position.</summary>
                ColdSourceFilter,
                /// <summary>Hot source position.</summary>
                HotSourceFilter,
                /// <summary>Home position.</summary>
                Home,
                /// <summary>Calibrate position.</summary>
                CalibrateSensor,
                /// <summary>Find home position.</summary>
                FindHome,
                /// <summary>Current state position.</summary>
                CurrentState = -1,
                /// <summary>Flag not present state.</summary>
                FlagNotPresent = -2,
                /// <summary>Position is inknown.</summary>
                PositionUnknown = -3
             };

            /// <summary>
            /// Sets the position of the flag to an enumerated position
            /// </summary>
            public FlagStateType FlagPositionState
            {
                get
                {
                    int val = 0;
                    // Since we may not have a hidden cooler module for some cameras
                    // return FlagNotPresent on error
                    if (iacfGetHCLRFlagPositionState(id, out val) != 0)
                        return FlagStateType.FlagNotPresent;
                    else
                        return (FlagStateType)val;
                }
                set
                {
                    SDKBase.checkError(iacfSetHCLRFlagPositionState(id, (int)value));
                }
            }

            /// <summary>
            /// Pre defined states of the flag setpoints
            /// </summary>
            public enum   FlagSetpointType
            {   
                /// <summary>Current setpoint.</summary>
                FlagCurrentSetpoint = -1,
                /// <summary>Use ambient temperature for setpoint.</summary>
                FlagAmbient,
                /// <summary>Use average value for hot and cold temperatures for setpoint.</summary>
                FlagHotAndCold
            };

            /// <summary>
            /// Sets the position of the flag to an enumerated position
            /// </summary>
            public FlagSetpointType FlagSetpoint
            {
                get
                {
                    int val = 0;
                    SDKBase.checkError(iacfGetHCLRFlagSetpoint(id, out val));
                    return (FlagSetpointType)val;
                }
                set
                {
                    SDKBase.checkError(iacfSetHCLRFlagSetpoint(id, (int)value));
                }
            }

            
            /// <summary>
            /// Gets/Sets the current temperature of the flag in degrees Celsius 
            /// </summary>
            public float FlagCurrentTemperature
            {
                get
                {
                    short val = 0;
                    SDKBase.checkError(iacfGetHCLRFlagCurrentTemperature(id, out val));
                    return (float)((float)val/(float)100);
                }
                set
                {
                    short val = (short)(value * 100);
                    SDKBase.checkError(iacfSetHCLRFlagCurrentTemperature(id, val));
                }
            }

            /// <summary>
            /// Gets/Sets the current ambient temperature in degrees Celsius 
            /// </summary>
            public float AmbientTemperature
            {
                get
                {
                    short val = 0;
                    SDKBase.checkError(iacfGetHCLRAmbientTemperature(id, out val));
                    return (float)((float)val/(float)100);
                }
                set
                {
                    short val = (short)(value * 100);
                    SDKBase.checkError(iacfSetHCLRAmbientTemperature(id, val));
                }
            }


			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfPingHCLR(int id);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetHCLRFlagPositions(int id, out short seeSpotFilter, 
                out short imagingFilter, out short coldSrc, out short hotSrc, out short sensorCal);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetHCLRFlagPositions(int id, short seeSpotFilter, short imagingFilter,
                int coldSrc, short hotSrc, short sensorCal);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetHCLRFlagSetpoint(int id, out int flagSetpoint);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetHCLRFlagSetpoint(int id, int flagSetpoint);
            
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetHCLRFPACoolerElapsedTimerTick(int id, out uint FPACoolerElapsedTimerTick);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetHCLRFPACoolerElapsedTimerTick(int id, uint FPACoolerElapsedTimerTick);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetHCLRTECControlParams(int id, out int propGain, out int intGain, out int diffGain, out uint clampHigh,
				out uint clampLow, out uint tempSetpoint, out uint currentTemp, out uint currentOutput, out uint pterm,
				out uint iterm, out uint dterm);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetHCLRTECControlParams(int id, int propGain, int intGain, int diffGain, uint clampHigh,
				uint clampLow, uint tempSetpoint, uint currentTemp, uint currentOutput, uint pterm, uint iterm, uint dterm);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetHCLRTempSenseSource(int id, out int TempSenseSource);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetHCLRTempSenseSource(int id, int TempSenseSource);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetHCLRTempSenseSlope(int id, out int TempSenseSlope);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetHCLRTempSenseSlope(int id, int TempSenseSlope);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetHCLRTempSenseOffset(int id, out int TempSenseOffset);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetHCLRTempSenseOffset(int id, int TempSenseOffset);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetHCLRTempSenseDiodeSlope( int id, out int TempSenseSlope );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetHCLRTempSenseDiodeSlope( int id, int TempSenseSlope );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetHCLRTempSenseDiodeOffset(int id, out int TempSenseOffset);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetHCLRTempSenseDiodeOffset(int id, int TempSenseOffset);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetHCLRFlagPositionState(int id, out int PositionState);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetHCLRFlagPositionState(int id, int PositionState);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetHCLRFlagCurrentPosition(int id, out int CurrentPosition);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetHCLRFlagCurrentPosition(int id, int CurrentPosition);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetHCLRFlagCurrentTemperature(int id, out short CurrentTemp);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetHCLRFlagCurrentTemperature(int id, short CurrentTemp);
            
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetHCLRAmbientTemperature(int id, out short CurrentTemp);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetHCLRAmbientTemperature(int id, short CurrentTemp);
    
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetHCLRFlagMotorControl(int id, out short shortAccelRate, 
                out short MinMotorSpeed, out short PosnMoveMaxMotorSpeed, out short finHomeMaxMotorSpeed, 
                out short MaxPossibleSteps, out short MaxBackupSteps);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetHCLRFlagMotorControl(int id, short AccelRate, short MinMotorSpeed,
                short PosnMoveMaxMotorSpeed, short finHomeMaxMotorSpeed, short MaxPossibleSteps, short MaxBackupSteps);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfHclrTecStart(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfHclrTecStop(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfHclrTecResume(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfHclrTimerReset(int id);

        
        }
		#endregion

		#region Configuration
		/// <summary>
		/// Handles configuration commands.
		/// </summary>
		public class Configuration
		{
			private int			id = -1;
            private byte totalPresets = 4;
			/// <summary>
			/// Reference to the SDK to allow this module to call functions in other modules.
			/// </summary>
			public ApacheSDK	apache = null;

			/// <summary>
			/// Accessories SDK ID.
			/// </summary>
			public int ID
			{
				set
				{
					id = value;
					saturationThreshold.id = id;
					dwellCount.id = id;
					restorePreset = totalPresets;
				}
			}

			/// <summary>
			/// Ping the module for health status.
			/// </summary>
			public void Ping()
			{
				SDKBase.checkError(iacfPingCFG(id));
			}

            /// <summary>
            /// Specifies the total number of presets for the Configuration object.
            /// </summary>
            public byte TotalPresets
            {
                set
                {
                    totalPresets = value;
                }

                get
                {
                    return totalPresets;
                }
            }
            /*
			public string ActiveSaveState
			{
				get
				{
					string	text = apache.fileSystem.RecvFileText("pgm\\activestate.ini");

					if (text == null)
						text = "Default.sav";

					return text;
				}
				set
				{
					apache.fileSystem.SendFileText("pgm\\activestate.ini", value);
					apache.fileSystem.SendFileText("nuc\\Default.sav", apache.fileSystem.RecvFileText("nuc\\" + value));
				}
			}
			*/

			/// <summary>
			/// Gets/Sets Active preset.
			/// </summary>
			public byte ActivePreset
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCFGActivePreset(id, out val));

					return (byte)val;
				}
				set
				{
					ConfigPreset = value;
					SDKBase.checkError(iacfSetCFGActivePreset(id, value));
				}
			}

			private byte		restorePreset = 4;

			/// <summary>
			/// Preset used to configure preset dependant settings.
			/// </summary>
			public byte ConfigPreset
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCFGConfigPreset(id, out val));

					return (byte)val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGConfigPreset(id, (uint)value));
				}
			}

			/// <summary>
			/// Preset used for configuration.
			/// </summary>
			public byte ConfigurationPreset
			{
				get
				{
					return ConfigPreset;
				}
				set
				{
					if (restorePreset == totalPresets)
						throw new ApplicationException("Configuration Preset modified outside of Start/End.");
					ConfigPreset = value;
				}
			}

			private Mutex	configMutex = new Mutex();

			/// <summary>
			/// Start performing configuration on any given preset (must be called before configuration preset is modified).
			/// </summary>
			public void StartPresetConfiguration()
			{
				Utility.safeWait(configMutex);
			//	try {	configMutex.WaitOne(10000, false);	} catch {}

				if (restorePreset == totalPresets)
					restorePreset = ActivePreset;
			}

			/// <summary>
			/// End performing configuration on any given preset (must not modify configuration preset after this call).
			/// </summary>
			public void EndPresetConfiguration()
			{
			//	try {	configMutex.ReleaseMutex();	} catch {}

				if (restorePreset != totalPresets)
					ConfigPreset = restorePreset;
			//	else
			//		throw new ApplicationException("EndPresetConfiguration called with no corresponding StartPresetConfiguration");

				restorePreset = totalPresets;

				configMutex.ReleaseMutex();
			}

			/// <summary>
			/// Preset to be displayed on analog video when preset sequencing.
			/// </summary>
			public byte AnalogVideoPreset
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCFGAnalogVideoPreset(id, out val));

					return (byte)val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGAnalogVideoPreset(id, value));
				}
			}

			/// <summary>
			/// Number of sequences to perform in repeated sequence mode.
			/// </summary>
			public uint CycleCount
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCFGPscCycleCount(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGPscCycleCount(id, value));

					//	TODO: remove after fixed in camera.
					PresetSequenceTriggerMode = PresetSequenceTriggerMode;
				}
			}

			/// <summary>
			/// Contains the dwell counts for each preset.
			/// </summary>
			public class DwellCountData
			{
				/// <summary>Accessories SDK control device to use.</summary>
				public int		id = -1;
				private ushort	dwellPS0;
				private ushort	dwellPS1;
				private ushort	dwellPS2;
				private ushort	dwellPS3;
				private bool	cache = false;

				private void Read()
				{
					SDKBase.checkError(iacfGetCFGPscDwellCounts(id, out dwellPS0, out dwellPS1, out dwellPS2, out dwellPS3));
				}

				private void Write()
				{
					SDKBase.checkError(iacfSetCFGPscDwellCounts(id, dwellPS0, dwellPS1, dwellPS2, dwellPS3));
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// Dwell count for ps 0.
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
				/// Dwell count for ps 1.
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
				/// Dwell count for ps 2.
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
				/// Dwell count for ps 3.
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

			private	DwellCountData	dwellCount = new DwellCountData();

			/// <summary>
			/// Get the dwell counts.
			/// </summary>
			public DwellCountData DwellCount
			{
				get
				{
					return dwellCount;
				}
			}

			/// <summary>
			/// Determins of preset sequencing (cycling) is enabled.
			/// </summary>
			public bool CyclingEnabled
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCFGPscEnableCycling(id, out val));

					return (val != 0);
				}
				set
				{
					uint	val = 0;

					if (value)
						val = 1;

					SDKBase.checkError(iacfSetCFGPscEnableCycling(id, val));
				}
			}

			/// <summary>
			/// Preset sequencing trigger modes.
			/// </summary>
			public enum PresetSequenceTriggerModeType
			{
				/// <summary>Trigger active preset.</summary>
				SinglePresetTriggerd				= 0,
				/// <summary>Trigger sequence</summary>
				SinglePresetSequenceTriggerd		= 1,
				/// <summary>Trigger N sequences (see cycle count)</summary>
				RepeatedPresetSequenceTriggerd		= 2,
			}

			/// <summary>
			/// Sync trigger mode, not used.
			/// </summary>
			public enum SyncTriggerModeType
			{
				/// <summary>Sync for each frame in sequence (not used).</summary>
				MultipleSyncTriggered			= 0,
				/// <summary>Sync for entire sequence (not used).</summary>
				SingleSyncTriggered				= 1,
			}

			/// <summary>
			/// Sync trigger mode, not used.
			/// </summary>
			public SyncTriggerModeType SyncTriggerMode
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGPscSyncTriggerMode(id, out val));

					return (SyncTriggerModeType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGPscSyncTriggerMode(id, (int)value));
				}
			}

			/// <summary>
			/// Preset sequencing trigger mode (active preset, single sequence, repeated sequence).
			/// </summary>
			public PresetSequenceTriggerModeType PresetSequenceTriggerMode
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGPscSeqMode(id, out val));

					return (PresetSequenceTriggerModeType)val;
				}
				set
				{
				/*	if (value == PresetSequenceTriggerModeType.RepeatedPresetSequenceTriggerd)
					{
						uint	[]data = new uint[1];
						uint	len = 1;

						apache.legacy.ReadMemory(0x40000180, ref data, ref len);
						data[0] |= 0x00008000;
						apache.legacy.WriteMemory(0x40000180, ref data, ref len);
					}
					else
					{
						uint	[]data = new uint[1];
						uint	len = 1;

						apache.legacy.ReadMemory(0x40000180, ref data, ref len);
						data[0] &= ~((uint)0x00008000);
						apache.legacy.WriteMemory(0x40000180, ref data, ref len);
					}*/

					SDKBase.checkError(iacfSetCFGPscSeqMode(id, (int)value));
				}
			}

			/// <summary>
			/// Global gain value.
			/// </summary>
			public float GlobalGain
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetCFGGlobalGain(id, out val));

					return val / 10000.0f;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGGlobalGain(id, (ushort)(value * 10000.0f)));
				}
			}

			/// <summary>
			/// Global offset value.
			/// </summary>
			public short GlobalOffset
			{
				get
				{
					short	val;

					SDKBase.checkError(iacfGetCFGGlobalOffset(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGGlobalOffset(id, value));
				}
			}

			/// <summary>
			/// Saturation threshold container class.
			/// </summary>
			public class SaturationThresholdData
			{
				/// <summary>AccessoriesSDK control channel id.</summary>
				public int		id = -1;
				private ushort	intensityLimit;
				private uint	pixelCountLimit;
				private bool	cache = false;

				private void Read()
				{
					SDKBase.checkError(iacfGetCFGSaturationThreshold(id, out intensityLimit, out pixelCountLimit));
				}

				private void Write()
				{
					SDKBase.checkError(iacfSetCFGSaturationThreshold(id, intensityLimit, pixelCountLimit));
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// Intensity limit.
				/// </summary>
				public ushort IntensityLimit
				{
					get
					{
						if (!cache)
							Read();

						return intensityLimit;
					}
					set
					{
						if (!cache)
							Read();

						intensityLimit = value;

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Pixel count limit.
				/// </summary>
				public uint PixelCountLimit
				{
					get
					{
						if (!cache)
							Read();

						return pixelCountLimit;
					}
					set
					{
						if (!cache)
							Read();

						pixelCountLimit = value;

						if (!cache)
							Write();
					}
				}
			}

			private SaturationThresholdData		saturationThreshold = new SaturationThresholdData();

			/// <summary>
			/// Gets saturation threshold container class.
			/// </summary>
			public SaturationThresholdData SaturationThreshold
			{
				get
				{
					return saturationThreshold;
				}
			}

			/// <summary>
			/// Current saturation count in pixels.
			/// </summary>
			public uint CurrentSaturationCount
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCFGCurrentSaturationCount(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGCurrentSaturationCount(id, value));
				}
			}

			/// <summary>
			/// IRIG Sync out status.
			/// </summary>
			public bool IRIGSyncOutEnabled
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGIrigSyncOut(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCFGIrigSyncOut(id, ((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Sync out status, note this should always be true, not modifiable.
			/// </summary>
			public bool SyncOutEnabled
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGSyncOut(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCFGSyncOut(id, ((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Sync source list.
			/// </summary>
			public enum SyncSourceType
			{
				/// <summary>
				/// internal free run
				/// </summary>
				Internal	= 0,
				/// <summary>
				/// external from cielo
				/// </summary>
				External	= 1,
				/// <summary>
				/// external from genlock
				/// </summary>
				Genlock		= 2,
				/// <summary>
				/// internal software triggered
				/// </summary>
				CPU			= 3,
			}

			/// <summary>
			/// Sync generation source.
			/// </summary>
			public SyncSourceType SyncSource
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGSyncSource(id, out val));

					return (SyncSourceType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGSyncSource(id, (int)value));
				}
			}

			/// <summary>
			/// Integrate then read start delay enable status.
			/// </summary>
			public bool ITRStartDelayEnabled
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGItrStartDelayEnable(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCFGItrStartDelayEnable(id, ((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Integrate then read start delay (ticks).
			/// </summary>
			public uint ITRStartDelay
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCFGItrStartDelay(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGItrStartDelay(id, value));
				}
			}

			/// <summary>
			/// External sync delay enable status.
			/// </summary>
			public bool ExternalSyncDelayEnabled
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGExtSyncDelayEnable(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCFGExtSyncDelayEnable(id, ((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// External sync delay (ticks).
			/// </summary>
			public uint ExternalSyncDelay
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCFGExtSyncDelay(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGExtSyncDelay(id, value));
				}
			}

			/// <summary>
			/// Signal polarity.
			/// </summary>
			public enum PolarityType
			{
				/// <summary>
				/// Signal on falling edge
				/// </summary>
				FallingEdge		= 0,
				/// <summary>
				/// Signal on rising edge
				/// </summary>
				RisingEdge		= 1,
			}

			/// <summary>
			/// External sync polarity.
			/// </summary>
			public PolarityType ExternaSyncPolarity
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGExtSyncPolarity(id, out val));

					return (PolarityType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGExtSyncPolarity(id, (int)value));
				}
			}

			/// <summary>
			/// Trigger sources, not used.
			/// </summary>
			public enum TriggerSourceType
			{
				/// <summary>
				/// Not used.
				/// </summary>
				Hardware = 0,
				/// <summary>
				/// Not used.
				/// </summary>
				Software = 1,
			}

			/// <summary>
			/// Trigger source, not used.
			/// </summary>
			public TriggerSourceType TriggerSource
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGTriggerSource(id, out val));

					return (TriggerSourceType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGTriggerSource(id, (int)value));
				}
			}

			/// <summary>
			/// Trigger amred, not used.
			/// </summary>
			public bool TriggerArmed
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGTriggerArm(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCFGTriggerArm(id, ((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Trigger activeate, set to true to cause a trigger.
			/// </summary>
			public bool TriggerActivate
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGTriggerActivate(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCFGTriggerActivate(id, ((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Use full window NUCs when windowed.
			/// </summary>
			public bool FullWindowNUC
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGFullWindowNuc(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCFGFullWindowNuc(id, ((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Enable genloc signal.
			/// </summary>
			public bool GenlockEnabled
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGGenlock(id, out val));

					return (val != 0);
				}
				set
				{
					SDKBase.checkError(iacfSetCFGGenlock(id, ((value) ? 1 : 0)));
				}
			}

			/// <summary>
			/// Genloc sync delay in ticks.
			/// </summary>
			public uint GenlockDelay
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetCFGGenlockDelay(id, out val));

					return val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGGenlockDelay(id, value));
				}
			}

			/// <summary>
			/// Genlock signal polarity.
			/// </summary>
			public PolarityType GenlockPolarity
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGGenlockPolarity(id, out val));

					return (PolarityType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGGenlockPolarity(id, (int)value));
				}
			}

			/// <summary>
			/// Display clock sources.
			/// </summary>
			public enum DisplayClockSourceType
			{
				/// <summary>
				/// Master video clock.
				/// </summary>
				MasterVideo		= 0,
				/// <summary>
				/// Input genlock clock.
				/// </summary>
				Genlock			= 1,
			}

			/// <summary>
			/// Clock source for the display.
			/// </summary>
			public DisplayClockSourceType DisplayClockSource
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetCFGDisplayClockSource(id, out val));

					return (DisplayClockSourceType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetCFGDisplayClockSource(id, (int)value));
				}
			}

			/// <summary>
			/// Save restore tag name (no dir or extension).
			/// </summary>
			public string SaveRestoreTag
			{
				get
				{
					byte		[]tagBytes = new byte[32];
					GCHandle	gch;
					int			len;

					gch = GCHandle.Alloc(tagBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfGetCFGSaveRestoreTag(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();

					for (len = 0; len < 32; ++len)
					{
						if (tagBytes[len] == 0)
							break;
					}

					return (new ASCIIEncoding()).GetString(tagBytes, 0, len);
				}
				set
				{
					byte			[]tagBytes = new byte[32];
					GCHandle		gch;
					ASCIIEncoding	ascii = new ASCIIEncoding();
					int				i;

					if (ascii.GetByteCount(value) > 32)
						SDKBase.checkError(-1);

					for (i = 0; i < 32; ++i)
						tagBytes[i] = 0;

					ascii.GetBytes(value, 0, value.Length, tagBytes, 0);

					gch = GCHandle.Alloc(tagBytes, GCHandleType.Pinned);
					try
					{
						SDKBase.checkError(iacfSetCFGSaveRestoreTag(id, gch.AddrOfPinnedObject()));
					}
					catch
					{
						gch.Free();
						throw;
					}
					gch.Free();
				}
			}

            /// <summary>
            /// Saves state of all user data to file specified in SaveRestoreTag property
            /// </summary>
            public void SaveState()
			{
				SDKBase.checkError(iacfCfgCameraSave(id));
			}

            /// <summary>
            /// Saves state of all user data to file specified in argument
            /// </summary>
            public void SaveState(string name)
			{
				SaveRestoreTag = name;
				SaveState();
			}

            /// <summary>
            /// Restores state of all user data from file specified in SaveRestoreTag argument
            /// </summary>
            public void RestoreState()
			{
				SDKBase.checkError(iacfCfgCameraRestore(id));
			}

            /// <summary>
            /// Restores state of all user data from file specified in 1st argument
            /// </summary>
            public void RestoreState(string name)
			{
				SaveRestoreTag = name;
				RestoreState();
			}

            /// <summary>
            /// Saves state of all factory data.
            /// </summary>
            public void SaveStateFactory()
            {
                SDKBase.checkError(iacfCfgCameraSaveFactory(id));
            }
            
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfPingCFG(int id);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCFGActivePreset(int id, out uint index);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCFGActivePreset(int id, uint index);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCFGAnalogVideoPreset(int id, out uint AnalogVideoPreset);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCFGAnalogVideoPreset(int id, uint AnalogVideoPreset);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGPscStartid(int id, out int PscStartId);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGPscStartid(int id, int PscStartId);
            
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGPscEndid(int id, out int PscEndId);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGPscEndid(int id, int PscEndId);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGPscCycleCount(int id, out uint PscCycleCount);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGPscCycleCount(int id, uint PscCycleCount);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCFGPscDwellCounts(int id, out ushort dwellPS0, out ushort dwellPS1, out ushort dwellPS2, out ushort dwellPS3);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCFGPscDwellCounts(int id, ushort dwellPS0, ushort dwellPS1, ushort dwellPS2, ushort dwellPS3);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCFGPscEnableCycling(int id, out uint enabled);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCFGPscEnableCycling(int id, uint enabled);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGPscSyncTriggerMode(int id, out int mode);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGPscSyncTriggerMode(int id, int mode);
            
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGPscSeqMode(int id, out int SeqMode);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGPscSeqMode(int id, int SeqMode);
            
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCFGGlobalGain(int id, out ushort GlobalGain);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCFGGlobalGain(int id, ushort GlobalGain);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCFGGlobalOffset(int id, out short GlobalOffset);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCFGGlobalOffset(int id, short GlobalOffset);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCFGSaturationThreshold(int id, out ushort intensityLimit, out uint pixelCountLimit);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCFGSaturationThreshold(int id, ushort intensityLimit, uint pixelCountLimit);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetCFGCurrentSaturationCount(int id, out uint CurrentSaturationCount);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetCFGCurrentSaturationCount(int id, uint CurrentSaturationCount);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGIrigSyncOut(int id, out int SyncOut);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGIrigSyncOut(int id, int SyncOut);
        
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGSyncOut(int id, out int SyncOut);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGSyncOut(int id, int SyncOut);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGSyncSource(int id, out int SyncSource);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGSyncSource(int id, int SyncSource);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGItrStartDelayEnable(int id, out int ItrStartDelayEnable);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGItrStartDelayEnable(int id, int ItrStartDelayEnable);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGItrStartDelay(int id, out uint ItrStartDelay);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGItrStartDelay(int id, uint ItrStartDelay);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGExtSyncDelayEnable(int id, out int ExtSyncDelayEnable);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGExtSyncDelayEnable(int id, int ExtSyncDelayEnable);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGExtSyncDelay(int id, out uint ExtSyncDelay);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGExtSyncDelay(int id, uint ExtSyncDelay);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGExtSyncPolarity(int id, out int ExtSyncPolarity);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGExtSyncPolarity(int id, int ExtSyncPolarity);
        
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGTriggerSource(int id, out int TriggerSource);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGTriggerSource(int id, int TriggerSource);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGTriggerArm(int id, out int TriggerArm);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGTriggerArm(int id, int TriggerArm);
        
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGTriggerActivate(int id, out int TriggerActivate);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGTriggerActivate(int id, int TriggerActivate);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGFullWindowNuc(int id, out int FullWindowNuc);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGFullWindowNuc(int id, int FullWindowNuc);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGGenlock(int id, out int Genlock);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGGenlock(int id, int Genlock);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGGenlockPolarity(int id, out int GenlockPolarity);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGGenlockPolarity(int id, int GenlockPolarity);
        
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGGenlockDelay(int id, out uint GenlockDelay);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGGenlockDelay(int id, uint GenlockDelay);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGDisplayClockSource(int id, out int DisplayClockSource);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGDisplayClockSource(int id, int DisplayClockSource);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGConfigPreset(int id, out uint ConfigPreset);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGConfigPreset(int id, uint ConfigPreset);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetCFGSaveRestoreTag(int id, IntPtr SaveRestoreTag);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetCFGSaveRestoreTag(int id, IntPtr SaveRestoreTag);
            
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfCfgCameraSave(int id);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfCfgCameraRestore(int id);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfCfgCameraSaveFactory(int id);
            
        }
		#endregion

		#region VideoProcessing
		/// <summary>
		/// Handles video processing commands.
		/// </summary>
		public class VideoProcessing
		{
			private int		id = -1;

			/// <summary>
			/// AccessoriesSDK control channel id.
			/// </summary>
			public int ID
			{
				set
				{
					id = value;
					digitalDataPath.id = id;
					analogDataPath.id = id;
				}
			}

			/// <summary>
			/// Pings the module for health.
			/// </summary>
			public void Ping()
			{
				SDKBase.checkError(iacfPingVID(id));
			}

			/// <summary>
			/// Type of polarity.
			/// </summary>
			public enum PolarityType
			{
				/// <summary>White is hot, black is cold.</summary>
				WhiteHot	= 0,
				/// <summary>Black is hot, white is cold.</summary>
				BlackHot	= 1,
			}

            /// <summary>
            /// Video data source enumeration.
            /// </summary>
            public enum DataSourceType
            {
                /// <summary>Video source is FPA.</summary>
                SourceFPA       = 0,
                /// <summary>Video source is test pattern ramp up.</summary>
                SourceRampUp    = 1,
                /// <summary>Video source is specified constant.</summary>
                SourceConstant  = 2,
                /// <summary>Video source is test pattern ramp down.</summary>
                SourceRampDown  = 3,
            }

            /// <summary>
            /// Data Source property
            /// </summary>
            /// 
            public DataSourceType DataSource
            {
                get
                {
                    uint	val = 0;
                    SDKBase.checkError(iacfGetVIDDataSourceSelect(id, out val));
                    return (DataSourceType)val;
                }
                
                set
                {
                    SDKBase.checkError(iacfSetVIDDataSourceSelect(id, (uint)value));
                }
            }
            
            /// <summary>
            /// Video data constant value property.
            /// </summary>
            public UInt16 DataConstant
            {
                get
                {
                    UInt16 val = 0;
                    SDKBase.checkError(iacfGetVIDVideoDataConstant(id, out val));
                    return val;
                }
                set
                {
                    SDKBase.checkError(iacfSetVIDVideoDataConstant(id, value));
                }
            }
            
            /// <summary>
            /// Video enable.
            /// </summary>
            public bool Enabled
            {
                get
                {
                    int	val = 0;

                    SDKBase.checkError(iacfGetVIDEnable(id, out val));

                    return (val != 0);
                }
                set
                {
                    int	val = 0;
                    if (value)
                        val = 1;
                    else
                        val = 0;

                    SDKBase.checkError(iacfSetVIDEnable(id, val));
                }
            }

            /// <summary>
            /// Get/set the analog video polarity.
            /// </summary>
            public PolarityType AnalogVideoPolarity
            {
                get
                {
                    uint	val;

                    SDKBase.checkError(iacfGetVIDAnalogVideoPolarity(id, out val));

                    return (PolarityType)val;
                }
                set
                {
                    SDKBase.checkError(iacfSetVIDAnalogVideoPolarity(id, (uint)value));
                }
            }

			/// <summary>
			/// Get/set the digital video polarity.
			/// </summary>
			public PolarityType DigitalVideoPolarity
			{
				get
				{
					uint	val;

					SDKBase.checkError(iacfGetVIDDigitalVideoPolarity(id, out val));

					return (PolarityType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetVIDDigitalVideoPolarity(id, (uint)value));
				}
			}

			/// <summary>
			/// Contains the gain, offset, bad pixel flags for a data path.
			/// </summary>
			public class DataPathData
			{
				/// <summary>AccessoriesSDK control channel id.</summary>
				public int		id = -1;
				private bool	analog;
				private uint	gain, offset, badPixel, eightBit;
				private bool	cache = false;

				/// <summary>
				/// Standard constructor.
				/// </summary>
				/// <param name="_analog">True if this data path container is for the analog path, false is digital.</param>
				public DataPathData(bool _analog)
				{
					analog = _analog;
				}

				private void Read()
				{
					if (analog)
						SDKBase.checkError(ApacheSDK.VideoProcessing.iacfGetVIDAnalogDataPath(id, out gain, out offset, out badPixel));
					else
						SDKBase.checkError(ApacheSDK.VideoProcessing.iacfGetVIDDigitalDataPath(id, out gain, out offset, out badPixel, out eightBit));
				}

				private void Write()
				{
					if (analog)
						SDKBase.checkError(ApacheSDK.VideoProcessing.iacfSetVIDAnalogDataPath(id, gain, offset, badPixel));
					else
						SDKBase.checkError(ApacheSDK.VideoProcessing.iacfSetVIDDigitalDataPath(id, gain, offset, badPixel, eightBit));
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// Apply gain.
				/// </summary>
				public bool Gain
				{
					get
					{
						if (!cache)
							Read();

						return (gain == 0) ? false : true;
					}
					set
					{
						if (!cache)
							Read();

						gain = (uint)((value) ? 1 : 0);

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Apply Offset.
				/// </summary>
				public bool Offset
				{
					get
					{
						if (!cache)
							Read();

						return (offset == 0) ? false : true;
					}
					set
					{
						if (!cache)
							Read();

						offset = (uint)((value) ? 1 : 0);

						if (!cache)
							Write();
					}
				}

				/// <summary>
				/// Apply bad pixels.
				/// </summary>
				public bool BadPixel
				{
					get
					{
						if (!cache)
							Read();

						return (badPixel == 0) ? false : true;
					}
					set
					{
						if (!cache)
							Read();

						badPixel = (uint)((value) ? 1 : 0);

						if (!cache)
							Write();
					}
				}

                /// <summary>
                /// Read/write video eight bit data path.
                /// </summary>
                public bool EightBit
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return (eightBit == 0) ? false : true;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        eightBit = (uint)((value) ? 1 : 0);

                        if (!cache)
                            Write();
                    }
                }

			}

			DataPathData		analogDataPath = new DataPathData(true);
			DataPathData		digitalDataPath = new DataPathData(false);

			/// <summary>
			/// Gets the analog data path container.
			/// </summary>
			public DataPathData AnalogDataPath
			{
				get
				{
					return analogDataPath;
				}
			}

			/// <summary>
			/// Gets the digital data path container.
			/// </summary>
			public DataPathData DigitalDataPath
			{
				get
				{
					return digitalDataPath;
				}
			}

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfPingVID(int id);
            
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetVIDEnable(int id, out int Enable);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetVIDEnable(int id, int Enable);
            
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetVIDAnalogDataPath(int id, out uint gain, out uint offset, out uint badPixel);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetVIDAnalogDataPath(int id, uint gain, uint offset, uint badPixel);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetVIDDigitalDataPath(int id, out uint gain, out uint offset, out uint badPixel, out uint vidEightBitDataSelect);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetVIDDigitalDataPath(int id, uint gain, uint offset, uint badPixel, uint vidEightBitDataSelect);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetVIDAnalogVideoPolarity(int id, out uint polarity);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetVIDAnalogVideoPolarity(int id, uint polarity);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetVIDDigitalVideoPolarity(int id, out uint polarity);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetVIDDigitalVideoPolarity(int id, uint polarity);
        
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetVIDDataSourceSelect(int id, out uint dataSourceSelect);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetVIDDataSourceSelect(int id, uint dataSourceSelect);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetVIDVideoDataConstant(int id, out ushort dataConstant);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetVIDVideoDataConstant(int id, ushort dataConstant);
        }
		#endregion

		#region Display
		/// <summary>
		/// Handles video dislay commands.
		/// </summary>
        public class Display
        {
            private int		id = -1;

            /// <summary>
            /// AccessoriesSDK control channel id.
            /// </summary>
            public int ID
            {
                set
                {
                    id = value;
                    window.id = id;
                    boresight.id = id;
                    extents.id = id;
                    centerShift.id = id;
                }
            }

            /// <summary>
            /// Pings the module for health.
            /// </summary>
            public void Ping()
            {
                SDKBase.checkError(iacfPingDIS(id));
            }

            /// <summary>
            /// Container class for display window.
            /// </summary>
            public class WindowData
            {
                /// <summary>AccessoriesSDK control channel id.</summary>
                public int		id = -1;
                protected ushort	xoffset, yoffset, width, height;
                protected bool	cache = false;

                virtual protected void Read()
                {
                    SDKBase.checkError(ApacheSDK.Display.iacfGetDISDisplayWindow(id, out xoffset, out yoffset, out width, out height));
                }

                virtual protected void Write()
                {
                    SDKBase.checkError(ApacheSDK.Display.iacfSetDISDisplayWindow(id, xoffset, yoffset, width, height));
                }

                /// <summary>
                /// Read all dwell counts (struct).
                /// </summary>
                public void CacheRead()
                {
                    Read();

                    cache = true;
                }

                /// <summary>
                /// Write previously read dwell counts (struct).
                /// </summary>
                public void CacheWrite()
                {
                    cache = false;

                    Write();
                }

                /// <summary>
                /// Discard previously read dwell counts (struct).
                /// </summary>
                public void CacheClear()
                {
                    cache = false;
                }

                /// <summary>
                /// X offset.
                /// </summary>
                public ushort XOffset
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return xoffset;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        xoffset = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Y offset.
                /// </summary>
                public ushort YOffset
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return yoffset;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        yoffset = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Width.
                /// </summary>
                public ushort Width
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return width;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        width = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Height.
                /// </summary>
                public ushort Height
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return height;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        height = value;

                        if (!cache)
                            Write();
                    }
                }
            }

            private WindowData		window = new WindowData();

            /// <summary>
            /// Gets the display window container class.
            /// </summary>
            public WindowData Window
            {
                get
                {
                    return window;
                }
            }

            /// <summary>
            /// Container class for display boresight.
            /// </summary>
            public class BoresightData
            {
                /// <summary>AccessoriesSDK control channel id.</summary>
                public int		id = -1;
                /// <summary>boresight x,y offsets.</summary>
                protected short	xoffset, yoffset;
                protected bool	cache = false;

                virtual protected void Read()
                {
                    SDKBase.checkError(ApacheSDK.Display.iacfGetDISBoresightShift(id, out xoffset, out yoffset));
                }

                virtual protected void Write()
                {
                    SDKBase.checkError(ApacheSDK.Display.iacfSetDISBoresightShift(id, xoffset, yoffset));
                }

                /// <summary>
                /// Read boresight offsets.
                /// </summary>
                public void CacheRead()
                {
                    Read();

                    cache = true;
                }

                /// <summary>
                /// Write previously read boresight data (struct).
                /// </summary>
                public void CacheWrite()
                {
                    cache = false;

                    Write();
                }

                /// <summary>
                /// Discard previously read boresoght data (struct).
                /// </summary>
                public void CacheClear()
                {
                    cache = false;
                }

                /// <summary>
                /// X offset.
                /// </summary>
                public short XOffset
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return xoffset;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        xoffset = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Y offset.
                /// </summary>
                public short YOffset
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return yoffset;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        yoffset = value;

                        if (!cache)
                            Write();
                    }
                }

            }

            private BoresightData boresight = new BoresightData();

            /// <summary>
            /// Gets the Boresight container class.
            /// </summary>
            public BoresightData Boresight
            {
                get
                {
                    return boresight;
                }
            }
            


            
            /// <summary>
            /// Container class for display extents.
            /// </summary>
            public class ExtentsData
            {
                /// <summary>AccessoriesSDK control channel id.</summary>
                public int		id = -1;
                /// <summary>extents width and height.</summary>
                private short	width, height;
                private bool	cache = false;

                private void Read()
                {
                    SDKBase.checkError(ApacheSDK.Display.iacfGetDISDisplayExtents(id, out width, out height));
                }

                private void Write()
                {
                    SDKBase.checkError(ApacheSDK.Display.iacfSetDISDisplayExtents(id, width, height));
                }

                /// <summary>
                /// Read extents width and height (struct).
                /// </summary>
                public void CacheRead()
                {
                    Read();

                    cache = true;
                }

                /// <summary>
                /// Write previously read extents data (struct).
                /// </summary>
                public void CacheWrite()
                {
                    cache = false;

                    Write();
                }

                /// <summary>
                /// Discard previously read extents data (struct).
                /// </summary>
                public void CacheClear()
                {
                    cache = false;
                }

                /// <summary>
                /// Width.
                /// </summary>
                public short Width
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return width;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        width = value;

                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Height.
                /// </summary>
                public short Height
                {
                    get
                    {
                        if (!cache)
                            Read();

                        return height;
                    }
                    set
                    {
                        if (!cache)
                            Read();

                        height = value;

                        if (!cache)
                            Write();
                    }
                }

            }

            private ExtentsData extents = new ExtentsData();

            /// <summary>
            /// Gets the Extents container class.
            /// </summary>
            public ExtentsData Extents
            {
                get
                {
                    return extents;
                }
            }

            /// <summary>
            /// Gets the zoom factor data 0 = NO ZOOM, 1 = DOUBLE, etc..
            /// </summary>
            public UInt16 ZoomFactor
            {
                get
                {
                    ushort val = 0;
                    SDKBase.checkError(iacfGetDISVideoZoomFactor(id, out val));
                    return val;
                }

                set
                {
                    SDKBase.checkError(iacfSetDISVideoZoomFactor(id, value));
                }
            }
            
            /// <summary>
            /// Contains data to describe shift of FPA to center of display.
            /// </summary>
            public class CenterShiftData : BoresightData
            {
                override protected void Read()
                {
                    SDKBase.checkError(ApacheSDK.Display.iacfGetDISCenterShift(id, out xoffset, out yoffset));
                }

                override protected void Write()
                {
                    SDKBase.checkError(ApacheSDK.Display.iacfSetDISCenterShift(id, xoffset, yoffset));
                }
            }

            private CenterShiftData	centerShift = new CenterShiftData();

            /// <summary>
            /// Gets the window center shift container class.
            /// </summary>
            public CenterShiftData CenterShift
            {
                get
                {
                    return centerShift;
                }
            }

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfPingDIS(int id);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetDISPreset(int id, out uint Preset);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetDISPreset(int id, uint Preset);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetDISDisplayWindow(int id, out ushort xStart, out ushort yStart, out ushort xSize, out ushort ySize);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetDISDisplayWindow(int id, ushort xStart, ushort yStart, ushort xSize, ushort ySize);
        
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetDISBoresightShift(int id, out short xShift, out short yShift);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetDISBoresightShift(int id, short xShift, short yShift);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetDISDisplayExtents(int id, out short xSize, out short ySize);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetDISDisplayExtents(int id, short xSize, short ySize);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetDISVideoZoomFactor(int id, out ushort zoomFactor);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetDISVideoZoomFactor(int id, ushort zoomFactor);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetDISCenterShift(int id, out short xShift, out short yShift);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetDISCenterShift(int id, short xShift, short yShift);
            
        }
		#endregion

		#region FileSystem
		/// <summary>
		/// Handles system functions.
		/// </summary>
		public class FileSystem
		{
			/// <summary>
			/// Reference to parent SDK so this module can use functions in other modules.
			/// </summary>
			public ApacheSDK	apache;
			private int	id = -1;
			private const uint iacfIFSBlockSize = 128;

			/// <summary>IndigoSDK communications channel id, don't set this directly, use iacfSDK.ID</summary>
			public int ID
			{
				set
				{
					id = value;
					driveInfo.id = id;
					fileInfo.id = id;
					directoryInfo.id = id;
				}
			}

			/// <summary>
			/// Pings for the health/existance of the module.
			/// </summary>
			public void Ping()
			{
				SDKBase.checkError(iacfPingIFS(id));
			}

			/// <summary>
			/// Drive types.
			/// </summary>
			public enum DriveType
			{
				/// <summary>ram</summary>
				Memory		= 0,
				/// <summary>pgm</summary>
				Program		= 1,
				/// <summary>nuc</summary>
				Data		= 2,
				/// <summary>not used</summary>
				Reserved	= 3,
			}

			/// <summary>
			/// Active drive for functions that use a drive.
			/// </summary>
			public DriveType ActiveDrive
			{
				get
				{
					int		val;

					SDKBase.checkError(iacfGetIFSActiveDrive(id, out val));

					return (DriveType)val;
				}
				set
				{
					SDKBase.checkError(iacfSetIFSActiveDrive(id, (int)value));
				}
			}

			private delegate int GenericStringProperty(int id, IntPtr sz);

			string GenericGetString(GenericStringProperty fn)
			{
				byte		[]strBytes = new byte[512];
				GCHandle	gch;
				int			len;

				gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
				try
				{
					SDKBase.checkError(fn(id, gch.AddrOfPinnedObject()));
				}
				catch
				{
					gch.Free();
					throw;
				}
				gch.Free();

				for (len = 0; len < 512; ++len)
				{
					if (strBytes[len] == 0)
						break;
				}

				return (new ASCIIEncoding()).GetString(strBytes, 0, len);
			}

			void GenericSetString(string val, GenericStringProperty fn)
			{
				byte			[]strBytes = new byte[512];
				GCHandle		gch;
				ASCIIEncoding	ascii = new ASCIIEncoding();
				int				i;

				if (ascii.GetByteCount(val) > 512)
					SDKBase.checkError(-1);

				for (i = 0; i < 512; ++i)
					strBytes[i] = 0;

				ascii.GetBytes(val, 0, val.Length, strBytes, 0);

				gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
				try
				{
					SDKBase.checkError(fn(id, gch.AddrOfPinnedObject()));
				}
				catch
				{
					gch.Free();
					throw;
				}
				gch.Free();
			}

			/// <summary>
			/// Active directory name for functions that use it.
			/// </summary>
			public string ActiveDirectory
			{
				get
				{
					return GenericGetString(new GenericStringProperty(iacfGetIFSActiveDirectory));
				}
				set
				{
					GenericSetString(value, new GenericStringProperty(iacfSetIFSActiveDirectory));
				}
			}

			/// <summary>
			/// Active file name for functions that use it (full path).
			/// </summary>
			public string ActiveFile
			{
				get
				{
					return GenericGetString(new GenericStringProperty(iacfGetIFSActiveFile));
				}
				set
				{
					GenericSetString(value, new GenericStringProperty(iacfSetIFSActiveFile));
				}
			}

			/// <summary>
			/// Source for copy function (full path).
			/// </summary>
			public string SourceName
			{
				get
				{
					return GenericGetString(new GenericStringProperty(iacfGetIFSSourceName));
				}
				set
				{
					GenericSetString(value, new GenericStringProperty(iacfSetIFSSourceName));
				}
			}

			/// <summary>
			/// Dest for copy function (full path).
			/// </summary>
			public string DestinationName
			{
				get
				{
					return GenericGetString(new GenericStringProperty(iacfGetIFSDestinationName));
				}
				set
				{
					GenericSetString(value, new GenericStringProperty(iacfSetIFSDestinationName));
				}
			}

			/// <summary>
			/// Max transfer block size allowed, must be -.
			/// </summary>
			public ushort MaxBlockSize
			{
				get
				{
					ushort	val;

					SDKBase.checkError(iacfGetIFSMaxBlockSize(id, out val));

					return val;
				}
			}

			/// <summary>
			/// Power board software filename for powerboard update.
			/// </summary>
            public string PwrBrdSWFilename
            {
                get
                {
                    return GenericGetString(new GenericStringProperty(iacfGetIFSPowerBoardSwFilename));
                }
                set
                {
                    GenericSetString(value, new GenericStringProperty(iacfSetIFSPowerBoardSwFilename));
                }
            }
            
			/*
            public string ActiveFilePath
			{
				get
				{
					return ActiveDrive.ToString() + "\\" + ActiveFile;
				}
				set
				{
					string	drive, file;

					drive = value.Substring(0, value.IndexOf('\\'));
					file = value.Substring(value.IndexOf('\\') + 1);

					switch (drive)
					{
						case "Memory":
							ActiveDrive = DriveType.Memory;
							break;
						case "Program":
							ActiveDrive = DriveType.Program;
							break;
						case "Data":
							ActiveDrive = DriveType.Data;
							break;
					}

					ActiveFile = file;
				}
			}*/

			private DriveType DriveFromPath(string path)
			{
				string	drive, file;

				drive = path.Substring(0, path.IndexOf('\\'));
				file = path.Substring(path.IndexOf('\\') + 1);

				switch (drive)
				{
					case "ram":
						return DriveType.Memory;
					case "pgm":
						return DriveType.Program;
					case "nuc":
						return DriveType.Data;
				}

				return DriveType.Memory;
			}

			private string FileFromPath(string path)
			{
				string	drive, file;

				drive = path.Substring(0, path.IndexOf('\\'));
				file = path.Substring(path.IndexOf('\\') + 1);

				return file;
			}

			internal static void Zero(byte[] data, ushort size)
			{
				for (int i = 0; i < size; ++i)
					data[i] = 0;
			}

			/// <summary>
			/// Force a drive to recycle unused sectors. "Fixes" bad free space issue.
			/// </summary>
			/// <param name="drive">drive name, can be pgm/nuc/ram</param>
			public void Recycle(string drive)
			{
				if (drive.CompareTo("pgm") == 0)
					ActiveDrive = DriveType.Program;
				else if (drive.CompareTo("ram") == 0)
					ActiveDrive = DriveType.Memory;
				else if (drive.CompareTo("nuc") == 0)
					ActiveDrive = DriveType.Data;
				else
					SDKBase.checkError(-1);

				UnmountDrive();

				while (apache.system.CurrentStatus.Code != 0)
					Thread.Sleep(500);

				MountDrive();

				while (apache.system.CurrentStatus.Code != 0)
					Thread.Sleep(500);
			}

			/// <summary>
			/// Erase the active drive, REMOVES BOOT LOADER if active drive is program.
			/// </summary>
			public void EraseDrive()
			{
				SDKBase.checkError(iacfIfsCmdEraseDrive(id));
			}

			/// <summary>
			/// Formats the active drive, may remove the boot loader if active drive is program.
			/// </summary>
			public void FormatDrive()
			{
				SDKBase.checkError(iacfIfsCmdFormatDrive(id));
			}

			/// <summary>
			/// Mounts the active drive.
			/// </summary>
			public void MountDrive()
			{
				SDKBase.checkError(iacfIfsCmdMountDrive(id));
			}

			/// <summary>
			/// Unmounts the active drive.
			/// </summary>
			public void UnmountDrive()
			{
				SDKBase.checkError(iacfIfsCmdUnmountDrive(id));
			}

			/// <summary>
			/// Crates a directory using active directory.
			/// </summary>
			public void CreateDirectory()
			{
				SDKBase.checkError(iacfIfsCmdCreateDirectory(id));
			}

			/// <summary>
			/// Copys source to destination.
			/// </summary>
			public void CopyDirectory()
			{
				SDKBase.checkError(iacfIfsCmdCopyDirectory(id));
			}

			/// <summary>
			/// Renames source to destination.
			/// </summary>
			public void RenameDirectory()
			{
				SDKBase.checkError(iacfIfsCmdRenameDirectory(id));
			}

			/// <summary>
			/// Celetes active directory.
			/// </summary>
			public void DeleteDirectory()
			{
				SDKBase.checkError(iacfIfsCmdDeleteDirectory(id));
			}

			/// <summary>
			/// Copies source to destination.
			/// </summary>
			public void CopyFile()
			{
				SDKBase.checkError(iacfIfsCmdCopyFile(id));
			}

			/// <summary>
			/// Renames source to destination.
			/// </summary>
			public void RenameFile()
			{
				SDKBase.checkError(iacfIfsCmdRenameFile(id));
			}

			/// <summary>
			/// Deletes active file.
			/// </summary>
			public void DeleteFile()
			{
				SDKBase.checkError(iacfIfsCmdDeleteFile(id));
			}

			internal static int ByteStrLen(byte []bytes, int maxLen)
			{
				int i;

				for (i = 0; i < maxLen; ++i)
					if (bytes[i] == 0)
						break;

				return i;
			}

			/// <summary>
			/// Gets the drive names.
			/// </summary>
			public string[] DriveNames
			{
				get
				{
				//	return new string[3] { "ram", "pgm", "nuc" };

					byte[]			ramBytes, pgmBytes, nucBytes, resBytes;
					GCHandle		gchRam, gchPgm, gchNuc, gchRes;
					string[]		ret = new string[4];
					ASCIIEncoding	ascii = new ASCIIEncoding();

					ramBytes = new byte[256];
					pgmBytes = new byte[256];
					nucBytes = new byte[256];
					resBytes = new byte[256];

					Zero(ramBytes, 256);
					Zero(pgmBytes, 256);
					Zero(nucBytes, 256);
					Zero(resBytes, 256);

					gchRam = GCHandle.Alloc(ramBytes, GCHandleType.Pinned);
					gchPgm = GCHandle.Alloc(pgmBytes, GCHandleType.Pinned);
					gchNuc = GCHandle.Alloc(nucBytes, GCHandleType.Pinned);
					gchRes = GCHandle.Alloc(resBytes, GCHandleType.Pinned);

					try
					{
						SDKBase.checkError(iacfGetIFSDriveNames(id, gchRam.AddrOfPinnedObject(), gchPgm.AddrOfPinnedObject(), gchNuc.AddrOfPinnedObject(), gchRes.AddrOfPinnedObject()));
					}
					catch
					{
						throw;
					}
					finally
					{
						gchRam.Free();
						gchPgm.Free();
						gchNuc.Free();
						gchRes.Free();
					}

					ret[0] = ascii.GetString(ramBytes, 0, ByteStrLen(ramBytes, 256));
					ret[1] = ascii.GetString(pgmBytes, 0, ByteStrLen(pgmBytes, 256));
					ret[2] = ascii.GetString(nucBytes, 0, ByteStrLen(nucBytes, 256));
                    ret[3] = ascii.GetString(resBytes, 0, ByteStrLen(resBytes, 256));
					//	note: no longer ommiting reserved drive name.

					return ret;
				}
			}

			const int	SendToCamera = 0;
			const int	RecvFromCamera = 1;

			internal int GetIFSFileHostTransferProlog(out uint fileSizeInBytes, out uint maxBlockSizeToSend,
				out uint numTransferBlocks, out int xmitRcvFlag, out string fileFullName)
			{
				byte[]			strBytes;
				GCHandle		gchStr;
				ASCIIEncoding	ascii = new ASCIIEncoding();
				int				ret;

				strBytes = new byte[256];

				Zero(strBytes, 256);

				gchStr = GCHandle.Alloc(strBytes, GCHandleType.Pinned);

				ret = iacfGetIFSFileHostTransferProlog(id, out fileSizeInBytes, out maxBlockSizeToSend, out numTransferBlocks, out xmitRcvFlag, gchStr.AddrOfPinnedObject());

				gchStr.Free();

				fileFullName = ascii.GetString(strBytes, 0, ByteStrLen(strBytes, 256));

				return ret;
			}

			internal int SetIFSFileHostTransferProlog(uint fileSizeInBytes, uint maxBlockSizeToSend,
				uint numTransferBlocks, int xmitRcvFlag, string fileFullName)
			{
				byte[]			strBytes;
				GCHandle		gchStr;
				ASCIIEncoding	ascii = new ASCIIEncoding();
				int				ret;

				strBytes = new byte[256];

				Zero(strBytes, 256);

				ascii.GetBytes(fileFullName, 0, fileFullName.Length, strBytes, 0);

				gchStr = GCHandle.Alloc(strBytes, GCHandleType.Pinned);

				ret = iacfSetIFSFileHostTransferProlog(id, fileSizeInBytes, maxBlockSizeToSend, numTransferBlocks, xmitRcvFlag, gchStr.AddrOfPinnedObject());

				gchStr.Free();

				return ret;
			}

			internal int GetIFSFileHostTransferDataBlock(out uint blockSize, out uint blockNumber,
				byte []dataBlock)
			{
				GCHandle	gchData;
				int			ret;

				gchData = GCHandle.Alloc(dataBlock, GCHandleType.Pinned);

				ret = iacfGetIFSFileHostTransferDataBlock(id, out blockSize, out blockNumber, gchData.AddrOfPinnedObject());

				gchData.Free();

				return ret;
			}

			internal int SetIFSFileHostTransferDataBlock(uint blockSize, uint blockNumber,
				byte []dataBlock)
			{
				GCHandle	gchData;
				int			ret;

				gchData = GCHandle.Alloc(dataBlock, GCHandleType.Pinned);

				ret = iacfSetIFSFileHostTransferDataBlock(id, blockSize, blockNumber, gchData.AddrOfPinnedObject());

				gchData.Free();

				return ret;
			}

			internal int GetIFSFileHostTransferEpilog(out uint totalBytesTransfered, out uint totalBlocksTransfered)
			{
				return iacfGetIFSFileHostTransferEpilog(id, out totalBytesTransfered, out totalBlocksTransfered);
			}

			internal int SetIFSFileHostTransferEpilog(uint totalBytesTransfered, uint totalBlocksTransfered)
			{
				return iacfSetIFSFileHostTransferEpilog(id, totalBytesTransfered, totalBlocksTransfered);
			}

			internal int GetIFSDirListHostTransferProlog(out uint requestedBlockSize, out uint numTransferBlocks,
				out string directoryFullName)
			{
				byte[]			strBytes;
				GCHandle		gchStr;
				ASCIIEncoding	ascii = new ASCIIEncoding();
				int				ret;

				strBytes = new byte[256];

				Zero(strBytes, 256);

				gchStr = GCHandle.Alloc(strBytes, GCHandleType.Pinned);

				ret = iacfGetIFSDirListHostTransferProlog(id, out requestedBlockSize, out numTransferBlocks, gchStr.AddrOfPinnedObject());

				gchStr.Free();

				directoryFullName = ascii.GetString(strBytes, 0, ByteStrLen(strBytes, 256));

				return ret;
			}

			internal int SetIFSDirListHostTransferProlog (uint requestedBlockSize, uint numTransferBlocks,
				string directoryFullName)
			{
				byte[]			strBytes;
				GCHandle		gchStr;
				ASCIIEncoding	ascii = new ASCIIEncoding();
				int				ret;

				strBytes = new byte[256];

				Zero(strBytes, 256);

				ascii.GetBytes(directoryFullName, 0, directoryFullName.Length, strBytes, 0);

				gchStr = GCHandle.Alloc(strBytes, GCHandleType.Pinned);

				ret = iacfSetIFSDirListHostTransferProlog(id, requestedBlockSize, numTransferBlocks, gchStr.AddrOfPinnedObject());

				gchStr.Free();

				return ret;
			}

			internal int GetIFSDirListHostTransferDataBlock (out uint blockSize, out uint blockNumber,
				byte []dataBlock)
			{
				GCHandle	gchData;
				int			ret;

				gchData = GCHandle.Alloc(dataBlock, GCHandleType.Pinned);

				ret = iacfGetIFSDirListHostTransferDataBlock(id, out blockSize, out blockNumber, gchData.AddrOfPinnedObject());

				gchData.Free();

				return ret;
			}

			internal int SetIFSDirListHostTransferDataBlock(uint blockSize, uint blockNumber,
				byte []dataBlock)
			{
				GCHandle	gchData;
				int			ret;

				gchData = GCHandle.Alloc(dataBlock, GCHandleType.Pinned);

				ret = iacfSetIFSDirListHostTransferDataBlock(id, blockSize, blockNumber, gchData.AddrOfPinnedObject());

				gchData.Free();

				return ret;
			}

			internal int GetIFSDirListHostTransferEpilog(out uint totalBytesTransfered, out uint totalBlocksTransfered)
			{
				return iacfGetIFSDirListHostTransferEpilog(id, out totalBytesTransfered, out totalBlocksTransfered);
			}

			internal int SetIFSDirListHostTransferEpilog(uint totalBytesTransfered, uint totalBlocksTransfered)
			{
				return iacfSetIFSDirListHostTransferEpilog(id, totalBytesTransfered, totalBlocksTransfered);
			}

			/// <summary>
			/// Gets drive names.
			/// </summary>
			/// <returns>string array of drive names</returns>
			public string []GetDrives()
			{
				return DriveNames;
			}

			/// <summary>
			/// Gets directory entries that are children of dir (full path).
			/// </summary>
			/// <param name="dir">directory to list</param>
			/// <returns>string array of dir entries</returns>
			public string []GetDirEntries(string dir)
			{
				uint			blockSize, numBlocks, totalBytes, totalBlocks, iBlock, thisSize, start, end;
				byte			[]data, block;
				ArrayList		ar = new ArrayList();
				string			[]ret;
				ASCIIEncoding	ascii = new ASCIIEncoding();
				int				error = 0, tryNum;

				dir = dir.Replace("/", "\\");

				if (!dir.EndsWith("\\"))
					dir += "\\";

				ActiveDirectory = dir;
				SDKBase.checkError(SetIFSDirListHostTransferProlog(128, 0, dir));
				SDKBase.checkError(GetIFSDirListHostTransferProlog(out blockSize, out numBlocks, out dir));

				data = new byte[numBlocks * blockSize];
				block = new byte[128];

				for (iBlock = 0; iBlock < numBlocks; ++iBlock)
				{
					thisSize = 128;
					try
					{
						for (tryNum = 0; tryNum < 3; ++tryNum)
						{
							if ((error = SetIFSDirListHostTransferDataBlock(thisSize, iBlock, block)) == 0)
								error = GetIFSDirListHostTransferDataBlock(out thisSize, out iBlock, block);

							if (error == 0)
								break;
						}

						SDKBase.checkError(error);
					}
					catch
					{
						GetIFSDirListHostTransferEpilog(out totalBytes, out totalBlocks);
						throw;
					}
					block.CopyTo(data, iBlock * blockSize);
				}

				SDKBase.checkError(GetIFSDirListHostTransferEpilog(out totalBytes, out totalBlocks));

				for (start = 0, end = 0; end < totalBytes; ++end)
				{
					if (data[end] == 0)
					{
						ar.Add(ascii.GetString(data, (int)start, (int)(end - start)));
						start = end + 1;
					}
				}

				ret = new string[ar.Count];

				for (int i = 0; i < ar.Count; ++i)
					ret[i] = (string)ar[i];

				return ret;
			}

			/// <summary>
			/// Recieves a file from the camera.
			/// </summary>
			/// <param name="cameraFile">camera file name</param>
			/// <param name="fsDest">filestream to recieve to</param>
			/// <param name="progress">progress delegate</param>
			/// <param name="param">progress parameter</param>
			public void RecvFile(string cameraFile, FileStream fsDest, CopyProgress progress, object param)
			{
				uint		block = 0, sizeRead = 0, totalBytes, totalBlocks;
				uint		thisSize = 0;
				uint		nodeSize;
				byte		[]data = new byte[iacfIFSBlockSize];
				int			intJunk1;
				string		strJunk1;
				uint		blockSize, numBlocks;
                uint        blockReturned = 0;
				int			tryNum, error = 0;

				cameraFile = cameraFile.Replace("/", "\\");

				SDKBase.checkError(SetIFSFileHostTransferProlog(0, iacfIFSBlockSize, 0, RecvFromCamera, cameraFile));
				SDKBase.checkError(GetIFSFileHostTransferProlog(out nodeSize, out blockSize, out numBlocks, out intJunk1, out strJunk1));

				if (blockSize > iacfIFSBlockSize)
					blockSize = iacfIFSBlockSize;

				for (block = 0; block < numBlocks; ++block)
				{
					try
					{
						for (tryNum = 0; tryNum < 4; ++tryNum)
						{
							if ((error = SetIFSFileHostTransferDataBlock(blockSize, block, data)) == 0)
								error = GetIFSFileHostTransferDataBlock(out thisSize, out blockReturned, data);

							if (blockReturned != block)
								error = (int)SDKBase.sdkError.errFileUploadBlockNumber;

							if (error == 0)
								break;

							if (tryNum == 3)
								Thread.Sleep(500);
						}

						SDKBase.checkError(error);
						fsDest.Write(data, 0, (int)thisSize);
					}
					catch
					{
						GetIFSFileHostTransferEpilog(out totalBytes, out totalBlocks);
						throw;
					}

					sizeRead += thisSize;

					if (progress != null)
						progress(param, (sizeRead / (double)nodeSize) * 100.0);
				}

				SDKBase.checkError(GetIFSFileHostTransferEpilog(out totalBytes, out totalBlocks));
			}

			/// <summary>
			/// Recieves a file from the camera.
			/// </summary>
			/// <param name="cameraFile">camera file name</param>
			/// <param name="localFile">local file name</param>
			/// <param name="progress">progress delegate</param>
			/// <param name="param">progress parameter</param>
			public void RecvFile(string cameraFile, string localFile, CopyProgress progress, object param)
			{
				FileStream	fsDest;

				fsDest = File.Create(localFile);

				try
				{
					RecvFile(cameraFile, fsDest, progress, param);
				}
				catch
				{
					throw;
				}
				finally
				{
					fsDest.Close();
				}
			}

			/// <summary>
			/// Sends a file to the camera.
			/// </summary>
			/// <param name="fsSrc">source filestream</param>
			/// <param name="size">size of the file</param>
			/// <param name="cameraFile">file in camera filesystem</param>
			/// <param name="progress">progress delegate</param>
			/// <param name="param">progress parameter</param>
			public void SendFile(FileStream fsSrc, uint size, string cameraFile, CopyProgress progress, object param)
			{
				uint		numBlocks, block, totalBytes, totalBlocks;
				ushort		thisSize;
				byte		[]data = new byte[iacfIFSBlockSize];
				int			error = 0, tryNum;

				cameraFile = cameraFile.Replace("/", "\\");

				numBlocks = size / iacfIFSBlockSize;
				if ((size % iacfIFSBlockSize) != 0)
					++numBlocks;

				try
				{
					SDKBase.checkError(SetIFSFileHostTransferProlog(size, iacfIFSBlockSize, numBlocks, SendToCamera, cameraFile));
				}
				catch
				{
					fsSrc.Close();
					throw;
				}

				for (block = 0; block < numBlocks; ++block)
				{
					thisSize = (ushort)Math.Min(iacfIFSBlockSize, size - (block * iacfIFSBlockSize));
					try
					{
						fsSrc.Read(data, 0, thisSize);
						for (tryNum = 0; tryNum < 3; ++tryNum)
						{
							error = SetIFSFileHostTransferDataBlock(thisSize, block, data);

							if (error == 0)
								break;
						}
						SDKBase.checkError(error);
					}
					catch
					{
						GetIFSFileHostTransferEpilog(out totalBytes, out totalBlocks);
						throw;
					}

					if (progress != null)
						progress(param, (block / (double)numBlocks) * 100.0);
				}

				SDKBase.checkError(GetIFSFileHostTransferEpilog(out totalBytes, out totalBlocks));
			}

			/// <summary>
			/// Sends a file to the camera.
			/// </summary>
			/// <param name="localFile">file in computer filesystem</param>
			/// <param name="cameraFile">file in camera filesystem</param>
			/// <param name="progress">progress delegate</param>
			/// <param name="param">progress parameter</param>
			public void SendFile(string localFile, string cameraFile, CopyProgress progress, object param)
			{
				uint		size;
				FileStream	fsSrc;

				fsSrc = File.OpenRead(localFile);
				size = (uint)fsSrc.Length;

				try
				{
					SendFile(fsSrc, size, cameraFile, progress, param);
				}
				catch
				{
					throw;
				}
				finally
				{
					fsSrc.Close();
				}
			}

			/// <summary>
			/// Write a text file to the camera.
			/// </summary>
			/// <param name="cameraFile">camera file name</param>
			/// <param name="text">text to write</param>
			public void SendFileText(string cameraFile, string text)
			{
				string			localFile = Path.GetTempFileName();
				FileStream		fs = File.OpenWrite(localFile);
				ASCIIEncoding	ascii = new ASCIIEncoding();
				byte			[]bytes = ascii.GetBytes(text);

				fs.Write(bytes, 0, bytes.Length);
				fs.Close();

				try
				{
					SendFile(localFile, cameraFile, null, null);
				}
				finally
				{
					File.Delete(localFile);
				}
			}

			/// <summary>
			/// Read a text file from the camera.
			/// </summary>
			/// <param name="cameraFile">camera file to read</param>
			/// <returns>text in camera file</returns>
			public string RecvFileText(string cameraFile)
			{
				string			localFile = Path.GetTempFileName();

				try
				{
					RecvFile(cameraFile, localFile, null, null);
				}
				catch
				{
					return null;
				}

				FileStream		fs = File.OpenRead(localFile);
				ASCIIEncoding	ascii = new ASCIIEncoding();
				byte			[]bytes = new byte[fs.Length];
				string			ret;

				fs.Read(bytes, 0, bytes.Length);
				ret = ascii.GetString(bytes, 0, bytes.Length);
				fs.Close();

				File.Delete(localFile);

				return ret;
			}

			struct packProgressStruct
			{
				public object			param;
				public double			weight;
				public double			offset;
				public CopyProgress		innerProgress;
			}

			void packProgress(object param, double progress)
			{
				packProgressStruct	pps = (packProgressStruct)param;

				pps.innerProgress(pps.param, pps.offset + pps.weight * progress);
			}

			/// <summary>
			/// Uploads a pack file (composite file).
			/// </summary>
			/// <param name="packFile">pack filename on pc</param>
			/// <param name="progress">progress delegate</param>
			/// <param name="param">progress parameter</param>
			public void UploadPack(string packFile, CopyProgress progress, object param)
			{
				uint				size, magic;
				FileStream			fsSrc;
				byte				[]data = new byte[iacfIFSBlockSize];
				string				cameraFile;
				ASCIIEncoding		ascii = new ASCIIEncoding();
				packProgressStruct	pps = new packProgressStruct();

				pps.param = param;
				pps.innerProgress = progress;

				fsSrc = File.OpenRead(packFile);

				fsSrc.Read(data, 0, 4);

				magic = BitConverter.ToUInt32(data, 0);

				if (magic != 0x4B434150)
				{
					fsSrc.Close();
					throw new ApplicationException("Not a pack file.");
				}

				while (fsSrc.Position < fsSrc.Length)
				{
					fsSrc.Read(data, 0, 1);
					fsSrc.Read(data, 1, data[0]);
					cameraFile = ascii.GetString(data, 1, data[0]);
					fsSrc.Read(data, 0, 4);

					size = BitConverter.ToUInt32(data, 0);

					pps.offset = (fsSrc.Position / (double)fsSrc.Length) * 100.0;
					pps.weight = size / (double)fsSrc.Length;

					try
					{
						SendFile(fsSrc, size, cameraFile, new CopyProgress(packProgress), pps);
					}
					catch
					{
						fsSrc.Close();
						throw;
					}
				}

				fsSrc.Close();
			}

			/// <summary>
			/// Downloads a pack file (composite file).
			/// </summary>
			/// <param name="packFile">pack file on pc</param>
			/// <param name="cameraFiles">string array of camera files to download</param>
			/// <param name="progress">progress delegate</param>
			/// <param name="param">progress parameter</param>
			public void DownloadPack(string packFile, string []cameraFiles, CopyProgress progress, object param)
			{
				uint				packSize = 0, size;
				FileStream			fsDest;
				ASCIIEncoding		ascii = new ASCIIEncoding();
				byte				[]data = new byte[iacfIFSBlockSize];
				packProgressStruct	pps = new packProgressStruct();

				pps.param = param;
				pps.innerProgress = progress;

				for (int i = 0; i < cameraFiles.Length; ++i)
					cameraFiles[i] = cameraFiles[i].Replace("/", "\\");

				foreach (string cameraFile in cameraFiles)
				{
					ActiveFile = cameraFile;

					packSize += FileInfo.Size;
				}

				fsDest = File.Create(packFile);

				fsDest.Write(BitConverter.GetBytes((uint)0x4B434150), 0, 4);

				foreach (string cameraFile in cameraFiles)
				{
					ActiveFile = cameraFile;

					size = FileInfo.Size;

					data[0] = (byte)ascii.GetByteCount(cameraFile);
					ascii.GetBytes(cameraFile, 0, (int)data[0], data, 1);
					fsDest.Write(data, 0, data[0] + 1);
					fsDest.Write(BitConverter.GetBytes(size), 0, 4);

					pps.offset = (fsDest.Position / (double)packSize) * 100.0;
					pps.weight = size / (double)packSize;

					try
					{
						RecvFile(cameraFile, fsDest, new CopyProgress(packProgress), pps);
					}
					catch
					{
						fsDest.Close();
						throw;
					}
				}

				fsDest.Close();
			}

			/// <summary>
			/// Rename a file.
			/// </summary>
			/// <param name="oldName">existing file</param>
			/// <param name="newName">new file</param>
			public void RenameNode(string oldName, string newName)
			{
				oldName = oldName.Replace("/", "\\");
				newName = newName.Replace("/", "\\");

				SourceName = oldName;
				DestinationName = newName;

				RenameFile();
			}

			/// <summary>
			/// Remove a file.
			/// </summary>
			/// <param name="node">file to remove</param>
			public void RemoveNode(string node)
			{
				node = node.Replace("/", "\\");

				ActiveFile = node;

				DeleteFile();
			}

			/// <summary>
			/// Create a directory.
			/// </summary>
			/// <param name="dir">directory to create</param>
			public void CreateDirectory(string dir)
			{
				dir = dir.Replace("/", "\\");

				if (!dir.EndsWith("\\"))
					dir += "\\";

				ActiveDirectory = dir;

				CreateDirectory();
			}

			/// <summary>
			/// Contains directory information for active directory.
			/// </summary>
			public class DirectoryInfoData
			{
				/// <summary>Accessories SDK control device to use.</summary>
				public int		id = -1;
				private uint	numFiles, numBytes, modTime, accessTime;
				private bool	cache = false;

				private void Read()
				{
					SDKBase.checkError(iacfGetIFSDirectoryInfo(id, out numFiles, out numBytes, out modTime, out accessTime));
				}

				private void Write()
				{
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// Number of files in the directory.
				/// </summary>
				public uint NumFiles
				{
					get
					{
						if (!cache)
							Read();

						return numFiles;
					}
				}

				/// <summary>
				/// Number of bytes in the directory.
				/// </summary>
				public uint NumBytes
				{
					get
					{
						if (!cache)
							Read();

						return numBytes;
					}
				}

				/// <summary>
				/// Last modified date time.
				/// </summary>
				public DateTime ModifyTime
				{
					get
					{
						if (!cache)
							Read();

						return Utility.DateTimeFromTime_t(modTime);
					}
				}

				/// <summary>
				/// Last access date time.
				/// </summary>
				public DateTime AccessTime
				{
					get
					{
						if (!cache)
							Read();

						return Utility.DateTimeFromTime_t(accessTime);
					}
				}
			}

			private DirectoryInfoData directoryInfo = new DirectoryInfoData();

			/// <summary>
			/// Gets directory info container class.
			/// </summary>
			public DirectoryInfoData DirectoryInfo
			{
				get
				{
					return directoryInfo;
				}
			}

			/// <summary>
			/// Contains file info from active file.
			/// </summary>
			public class FileInfoData
			{
				/// <summary>Accessories SDK control device to use.</summary>
				public int		id = -1;
				private uint	size, modTime, accessTime;
				private bool	cache = false;

				private void Read()
				{
					SDKBase.checkError(iacfGetIFSFileInfo(id, out size, out modTime, out accessTime));
				}

				private void Write()
				{
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// File size in bytes.
				/// </summary>
				public uint Size
				{
					get
					{
						if (!cache)
							Read();

						return size;
					}
				}

				/// <summary>
				/// Last modify time.
				/// </summary>
				public DateTime ModifyTime
				{
					get
					{
						if (!cache)
							Read();

						return Utility.DateTimeFromTime_t(modTime);
					}
				}

				/// <summary>
				/// Last access time.
				/// </summary>
				public DateTime AccessTime
				{
					get
					{
						if (!cache)
							Read();

						return Utility.DateTimeFromTime_t(accessTime);
					}
				}
			}

			private FileInfoData fileInfo = new FileInfoData();

			/// <summary>
			/// Gets file info container class.
			/// </summary>
			public FileInfoData FileInfo
			{
				get
				{
					return fileInfo;
				}
			}

			/// <summary>
			/// Contains information on the active drive.
			/// </summary>
			public class DriveInfoData
			{
				/// <summary>Accessories SDK control device to use.</summary>
				public int		id = -1;
				private string	name;
				private uint	size, volType, sectorSize, numSectors, availableSectors, numUsedSectors, recycleSectors, flashType, wearCount, freeSpace;
				private bool	cache = false;

				private void Read()
				{
					byte[]			strBytes = new byte[256];
					GCHandle		gchStr;
					ASCIIEncoding	ascii = new ASCIIEncoding();

					Zero(strBytes, 256);

					gchStr = GCHandle.Alloc(strBytes, GCHandleType.Pinned);

					try
					{
						SDKBase.checkError(iacfGetIFSDriveInfo(id, gchStr.AddrOfPinnedObject(), out volType, out sectorSize, out numSectors, out availableSectors, out numUsedSectors, out recycleSectors, out flashType, out wearCount));
						size = sectorSize * numSectors;
						freeSpace = sectorSize * (availableSectors/* + recycleSectors*/);
					}
					catch
					{
						throw;
					}
					finally
					{
						gchStr.Free();
					}

					name = ascii.GetString(strBytes, 0, ByteStrLen(strBytes, 256));
                }

				private void Write()
				{
				}

				/// <summary>
				/// Read all dwell counts (struct).
				/// </summary>
				public void CacheRead()
				{
					Read();

					cache = true;
				}

				/// <summary>
				/// Write previously read dwell counts (struct).
				/// </summary>
				public void CacheWrite()
				{
					cache = false;

					Write();
				}

				/// <summary>
				/// Discard previously read dwell counts (struct).
				/// </summary>
				public void CacheClear()
				{
					cache = false;
				}

				/// <summary>
				/// Name of the drive.
				/// </summary>
				public string Name
				{
					get
					{
						if (!cache)
							Read();

						return name;
					}
				}

				/// <summary>
				/// Volume type.
				/// </summary>
				public uint VolumeType
				{
					get
					{
						if (!cache)
							Read();

						return volType;
					}
				}

				/// <summary>
				/// Sector size in bytes.
				/// </summary>
				public uint SectorSize
				{
					get
					{
						if (!cache)
							Read();

						return sectorSize;
					}
				}

				/// <summary>
				/// Total number of sectors in drive.
				/// </summary>
				public uint NumSectors
				{
					get
					{
						if (!cache)
							Read();

						return numSectors;
					}
				}

				/// <summary>
				/// Free sectors in drive.
				/// </summary>
				public uint AvailableSectors
				{
					get
					{
						if (!cache)
							Read();

						return availableSectors;
					}
				}

				/// <summary>
				/// Sectors to be recycled.
				/// </summary>
				public uint RecycleSectors
				{
					get
					{
						if (!cache)
							Read();

						return recycleSectors;
					}
				}

				/// <summary>
				/// Flas type.
				/// </summary>
				public uint FlashType
				{
					get
					{
						if (!cache)
							Read();

						return flashType;
					}
				}

				/// <summary>
				/// Wear count / lifetime.
				/// </summary>
				public uint WearCount
				{
					get
					{
						if (!cache)
							Read();

						return wearCount;
					}
				}

				/// <summary>
				/// Size of the drive in bytes.
				/// </summary>
				public uint Size
				{
					get
					{
						if (!cache)
							Read();

						return size;
					}
				}

				/// <summary>
				/// Free space in bytes.
				/// </summary>
				public uint FreeSpace
				{
					get
					{
						if (!cache)
							Read();

						return freeSpace;
					}
				}
			}

			private DriveInfoData driveInfo = new DriveInfoData();

			/// <summary>
			/// Gets drive info container class.
			/// </summary>
			public DriveInfoData DriveInfo
			{
				get
				{
					return driveInfo;
				}
			}

			/// <summary>
			/// Returns true if the file at path exists on the camera.
			/// </summary>
			/// <param name="path">camera file</param>
			/// <returns>true if the file exists</returns>
			public bool FileExists(string path)
			{
				ActiveFile = path;

				try
				{
					uint	size = FileInfo.Size;
					return true;
				}
				catch
				{
					return false;
				}
			}

			/// <summary>
			///	Execute power board software update process, use current status to wait for completion.
			/// </summary>
            public void UpdatePowerBrdSoftware()
            {
                SDKBase.checkError(iacfIfsCmdUpdatePwrbrdSw(id));
            }

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfPingIFS(int id);

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSActiveDrive( int id, out int ActiveDrive );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSActiveDrive( int id, int ActiveDrive );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSActiveDirectory( int id, IntPtr ActiveDirectory );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSActiveDirectory( int id, IntPtr ActiveDirectory );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSActiveFile( int id, IntPtr ActiveFile );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSActiveFile( int id, IntPtr ActiveFile );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSSourceName( int id, IntPtr SourceName );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSSourceName( int id, IntPtr SourceName );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSDestinationName( int id, IntPtr DestinationName );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSDestinationName( int id, IntPtr DestinationName );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSDriveNames ( int id, 
							 IntPtr ramDrive,
							 IntPtr programDrive,
							 IntPtr dataDrive,
							 IntPtr reservedDrive );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSFileHostTransferProlog ( int id, 
							 out uint fileSizeInBytes,
							 out uint maxBlockSizeToSend,
							 out uint numTransferBlocks,
							 out int xmitRcvFlag,
							 IntPtr fileFullName );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSFileHostTransferProlog ( int id, 
							 uint fileSizeInBytes,
							 uint maxBlockSizeToSend,
							 uint numTransferBlocks,
							 int xmitRcvFlag,
							 IntPtr fileFullName );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSFileHostTransferDataBlock ( int id, 
							 out uint blockSize,
							 out uint blockNumber,
							 IntPtr dataBlock );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSFileHostTransferDataBlock ( int id, 
							 uint blockSize,
							 uint blockNumber,
							 IntPtr dataBlock );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSFileHostTransferEpilog ( int id, 
							 out uint totalBytesTransfered,
							 out uint totalBlocksTransfered );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSFileHostTransferEpilog ( int id, 
							 uint totalBytesTransfered,
							 uint totalBlocksTransfered );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSDirListHostTransferProlog ( int id, 
							 out uint requestedBlockSize,
							 out uint numTransferBlocks,
							 IntPtr directoryFullName );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSDirListHostTransferProlog ( int id, 
							 uint requestedBlockSize,
							 uint numTransferBlocks,
							 IntPtr directoryFullName );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSDirListHostTransferDataBlock ( int id, 
							 out uint blockSize,
							 out uint blockNumber,
							 IntPtr dataBlock );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSDirListHostTransferDataBlock ( int id, 
							 uint blockSize,
							 uint blockNumber,
							 IntPtr dataBlock );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSDirListHostTransferEpilog ( int id, 
							 out uint totalBytesTransfered,
							 out uint totalBlocksTransfered );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSDirListHostTransferEpilog ( int id, 
							 uint totalBytesTransfered,
							 uint totalBlocksTransfered );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSDriveInfo ( int id, 
							 IntPtr volName,
							 out uint volType,
							 out uint sectorSize,
							 out uint numberOfSectors,
							 out uint availableSectors,
                             out uint usedSectors,
							 out uint sectorToRecycle,
							 out uint flashType,
							 out uint wearCount );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSDriveInfo ( int id, 
							 IntPtr volName,
							 uint volType,
							 uint sectorSize,
							 uint numberOfSectors,
							 uint availableSectors,
                             uint usedSectors,
                             uint sectorToRecycle,
							 uint flashType,
							 uint wearCount );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSDirectoryInfo ( int id, 
							 out uint totalBytesTransfered,
							 out uint totalBlocksTransfered,
							 out uint timeLastModified,
							 out uint timeLastAccessed );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSDirectoryInfo ( int id, 
							 uint totalBytesTransfered,
							 uint totalBlocksTransfered,
							 uint timeLastModified,
							 uint timeLastAccessed );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSFileInfo ( int id, 
							 out uint fileSizeInBytes,
							 out uint timeLastModified,
							 out uint timeLastAccessed );
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfSetIFSFileInfo ( int id, 
							 uint fileSizeInBytes,
							 uint timeLastModified,
							 uint timeLastAccessed );

			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfGetIFSMaxBlockSize( int id, out ushort MaxBlockSize );

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetIFSPowerBoardSwFilename( int id, IntPtr PowerBoardSwFilename );
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetIFSPowerBoardSwFilename( int id, IntPtr PowerBoardSwFilename );


			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfIfsCmdEraseDrive(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfIfsCmdFormatDrive(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfIfsCmdMountDrive(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfIfsCmdUnmountDrive(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfIfsCmdCreateDirectory(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfIfsCmdCopyDirectory(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfIfsCmdRenameDirectory(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfIfsCmdDeleteDirectory(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfIfsCmdCopyFile(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfIfsCmdRenameFile(int id);
			[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
			private static extern int iacfIfsCmdDeleteFile(int id);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfIfsCmdUpdatePwrbrdSw(int id);

			//	TODO: iacfGetIFSDriveNames, iacfGetIFSDriveInfo, iacfGetIFSDirectoryInfo, iacfGetIFSFileInfo
		}
		#endregion

        #region Power
        /// <summary>
        /// Handles power functions.
        /// </summary>
        public class Power
        {
            /// <summary>
            /// Reference to parent SDK so this module can use functions in other modules.
            /// </summary>
            public ApacheSDK	apache;
            private int	id = -1;
            private const uint iacfIFSBlockSize = 128;
            /// <summary>
            /// FPA power switch definitions.
            /// </summary>
            public enum PowerModeType
            {
                /// <summary>Off = 0.</summary>
                PowerOff = 0,
                /// <summary>On = 1</summary>
                PowerOn	= 1,
            }


            /// <summary>IndigoSDK communications channel id, don't set this directly, use iacfSDK.ID</summary>
            public int ID
            {
                set
                {
                    id = value;
                    dacvoltage.id = id;
                }
            }

            /// <summary>
            /// Pings for the health/existance of the module.
            /// </summary>
            public void Ping()
            {
                SDKBase.checkError(iacfPingPWR(id));
            }

            /// <summary>
            /// Turns off/on power for FPA (0 = Off;  1 = On)
            /// </summary>
            public PowerModeType VoltageSwitch
            {
                get
                {
                    Int32 val = 0;
                    SDKBase.checkError(iacfGetPWRFpaPowerSwitch(id, out val));
                    return (PowerModeType)val;
                }

                set
                {
                    int setVal =  0;
                    if (value == PowerModeType.PowerOff)
                        setVal = 0;
                    else
                        setVal = 1;

                    SDKBase.checkError(iacfSetPWRFpaPowerSwitch(id, setVal));
                }
            }

            /// <summary>
            /// Get the total number of DACs available For non-manufacturing user, this is read only)
            /// </summary>
            public UInt32 TotalDACs
            {
                get
                {
                    UInt32 val = 0;
                    SDKBase.checkError(iacfGetPWRMaxVoltageDacs(id, out val));
                    return val;
                }

                set
                {
                    SDKBase.checkError(iacfSetPWRMaxVoltageDacs(id, value));
                }
            }

            /// <summary>
            /// Gets or sets bit flags representing which dacs are available for use)
            /// </summary>
            public UInt32 AvailableDacsMap
            {
                get
                {
                    UInt32 val = 0;
                    SDKBase.checkError(iacfGetPWRAvailPwrMap(id, out val));
                    return val;
                }

                set
                {
                    SDKBase.checkError(iacfSetPWRAvailPwrMap(id, value));
                }
            }

        
            /// <summary>
            /// Container class for DAC voltage data.
            /// </summary>
            public class DacVoltageData
            {
                /// <summary>AccessoriesSDK control channel id.</summary>
                public int		id = -1;
                private string    voltageId;
                private ushort    dacIndex = 0;
                private int    maxLimit, minLimit, readBackVoltage, actualVoltage, gainSF, offsetSF, biasOffset, readBackSF;
                private uint biasDelay = 0;
                private ushort numBits = 0;
                private byte powerOnSeq, powerOffSeq;
                private bool	cache = false;
                private string[] dacIds = new string[10];

                private void Read(ushort dacId)
                {
                    byte		[]messageBytes = new byte[32];
                    GCHandle	gch;
                    int			len = 0;

                    gch = GCHandle.Alloc(messageBytes, GCHandleType.Pinned);
                    dacIndex = dacId;

//                    try
//                    {
//                        if ( iacfGetPWRDac(id, out dacIndex, gch.AddrOfPinnedObject(), out maxLimit, out minLimit,
//                            out readBackVoltage, out actualVoltage, out gainSF, out offsetSF,
//                            out biasOffset, out numBits, out powerOnSeq, out powerOffSeq, out biasDelay, out readBackSF ) == (int)SDKBase.sdkError.errSemaphoreTimeout )
//                        {
//                            // retry if first attempt fails due to semaphore timeout
//                            Thread.Sleep(100);
                            SDKBase.checkError(iacfGetPWRDac(id, out dacIndex, gch.AddrOfPinnedObject(), out maxLimit, out minLimit,
                                out readBackVoltage, out actualVoltage, out gainSF, out offsetSF,
                                out biasOffset, out numBits, out powerOnSeq, out powerOffSeq, out biasDelay, out readBackSF ));
//                        }
//                    }
//                    catch (exception ex)
//                    {
                        
//                        gch.Free();
//                        throw;
//                    }
                    gch.Free();
                    for (len = 0; len < 32; ++len)
                    {
                        if (messageBytes[len] == 0)
                            break;
                    }

                    voltageId = (new ASCIIEncoding()).GetString(messageBytes, 0, len);
                    dacIds[dacIndex] = voltageId;
                }

                private void Write(ushort dacId)
                {
                    byte			[]tagBytes = new byte[32];
                    GCHandle		gch;
                    ASCIIEncoding	ascii = new ASCIIEncoding();
                    int				i;

                    if (ascii.GetByteCount(voltageId) > 32)
                        SDKBase.checkError(-1);

                    for (i = 0; i < 32; ++i)
                        tagBytes[i] = 0;

                    ascii.GetBytes(voltageId, 0, voltageId.Length, tagBytes, 0);

                    gch = GCHandle.Alloc(tagBytes, GCHandleType.Pinned);
                    dacIndex = dacId;
                    try
                    {
                        if ( iacfSetPWRDac(id, dacIndex,  gch.AddrOfPinnedObject(), maxLimit, minLimit, readBackVoltage, actualVoltage, 
                            gainSF, offsetSF, biasOffset, numBits, powerOnSeq, powerOffSeq, biasDelay, readBackSF ) == (int)SDKBase.sdkError.errSemaphoreTimeout )
                        {
                            // retry if first attempt fails due to semaphore timeout
                            Thread.Sleep(100);
                            SDKBase.checkError(iacfSetPWRDac(id, dacIndex,  gch.AddrOfPinnedObject(), maxLimit, minLimit, readBackVoltage, actualVoltage, 
                                gainSF, offsetSF, biasOffset, numBits, powerOnSeq, powerOffSeq, biasDelay, readBackSF));
                        }
                    }
                    catch
                    {
                        gch.Free();
                        throw;
                    }
                    gch.Free();
                    
                }

                /// <summary>
                /// Read all dac data (struct).
                /// </summary>
                public void CacheRead(ushort dacId)
                {
                    Read(dacId);

                    cache = true;
                }

                /// <summary>
                /// Write previously read dac data(struct).
                /// </summary>
                public void CacheWrite(ushort dacId)
                {
                    cache = false;

                    Write(dacId);
                }

                /// <summary>
                /// Discard previously read dwell counts (struct).
                /// </summary>
                public void CacheClear()
                {
                    cache = false;
                }

                /// <summary>
                /// Sets the dac index 
                /// </summary>
                public ushort DacIndex
                {
                    get
                    {
                        return dacIndex;
                    }
                    set
                    {
                        dacIndex = value;
                    }
                }

                /// <summary>
                /// String dexcriptor for voltage id
                /// </summary>
                public string VoltageID
                {
                    get
                    {
                        if (!cache)
                            Read(dacIndex);

                        return voltageId;
                    }
                    set
                    {
                        if (!cache)
                            Read(dacIndex);

                        voltageId = value;

                        if (!cache)
                            Write(dacIndex);
                    }
                }

                
                /// <summary>
                /// String dexcriptor for previously read, specified voltage id
                /// </summary>
                public string GetDacDescription(int dacIndex)
                {
                    return dacIds[dacIndex];
                }

                /// <summary>
                /// Maximum voltage for dac id
                /// </summary>
                public Int32 MaxLimit
                {
                    get
                    {
                        if (!cache)
                            Read(dacIndex);

                        return maxLimit;
                    }
                    set
                    {
                        if (!cache)
                            Read(dacIndex);

                        maxLimit = value;

                        if (!cache)
                            Write(dacIndex);
                    }
                }
                /// <summary>
                /// Miminmun voltage for dac id
                /// </summary>
                public Int32 MinLimit
                {
                    get
                    {
                        if (!cache)
                            Read(dacIndex);

                        return minLimit;
                    }
                    set
                    {
                        if (!cache)
                            Read(dacIndex);

                        minLimit = value;

                        if (!cache)
                            Write(dacIndex);
                    }
                }

                /// <summary>
                /// Factory default voltage for dac id
                /// </summary>
                public Int32 ReadbackVoltage
                {
                    get
                    {
                        if (!cache)
                            Read(dacIndex);

                        return readBackVoltage;
                    }
                    set
                    {
                        if (!cache)
                            Read(dacIndex);

                        readBackVoltage = value;

                        if (!cache)
                            Write(dacIndex);
                    }
                }

                /// <summary>
                /// Actual voltage for dac id
                /// </summary>
                public Int32 Voltage
                {
                    get
                    {
                        if (!cache)
                            Read(dacIndex);

                        return actualVoltage;
                    }
                    set
                    {
                        if (!cache)
                            Read(dacIndex);

                        actualVoltage = value;

                        if (!cache)
                            Write(dacIndex);
                    }
                }
                /// <summary>
                /// Gain scale factor for voltage value for this dac id
                /// </summary>
                public Int32 GainScaleFactor
                {
                    get
                    {
                        if (!cache)
                            Read(dacIndex);

                        return gainSF;
                    }
                    set
                    {
                        if (!cache)
                            Read(dacIndex);

                        gainSF = value;

                        if (!cache)
                            Write(dacIndex);
                    }
                }

                /// <summary>
                /// Offset scale factor for voltage value for this dac id
                /// </summary>
                public Int32 OffsetScaleFactor
                {
                    get
                    {
                        if (!cache)
                            Read(dacIndex);

                        return OffsetScaleFactor;
                    }
                    set
                    {
                        if (!cache)
                            Read(dacIndex);

                        OffsetScaleFactor = value;

                        if (!cache)
                            Write(dacIndex);
                    }
                }
            
                /// <summary>
                /// Bias offset for voltage value for this dac id
                /// </summary>
                public Int32 BiasOffset
                {
                    get
                    {
                        if (!cache)
                            Read(dacIndex);

                        return biasOffset;
                    }
                    set
                    {
                        if (!cache)
                            Read(dacIndex);

                        biasOffset = value;

                        if (!cache)
                            Write(dacIndex);
                    }
                }

                /// <summary>
                /// Number of bits for voltage value for this dac id
                /// </summary>
                public UInt16 NumBits
                {
                    get
                    {
                        if (!cache)
                            Read(dacIndex);

                        return numBits;
                    }
                    set
                    {
                        if (!cache)
                            Read(dacIndex);

                        numBits = value;

                        if (!cache)
                            Write(dacIndex);
                    }
                }

                /// <summary>
                /// Flag representing the power on sequennce for this dac id
                /// </summary>
                public byte PowerOnSequence
                {
                    get
                    {
                        if (!cache)
                            Read(dacIndex);

                        return powerOnSeq;
                    }
                    set
                    {
                        if (!cache)
                            Read(dacIndex);

                        powerOnSeq = value;

                        if (!cache)
                            Write(dacIndex);
                    }
                }

                /// <summary>
                /// Flag representing the power off sequennce for this dac id
                /// </summary>
                public byte PowerOffSequence
                {
                    get
                    {
                        if (!cache)
                            Read(dacIndex);

                        return powerOffSeq;
                    }
                    set
                    {
                        if (!cache)
                            Read(dacIndex);

                        powerOffSeq = value;

                        if (!cache)
                            Write(dacIndex);
                    }
                }
            }

            private DacVoltageData	dacvoltage = new DacVoltageData();

            /// <summary>
            /// Gets the Dac voltage container class.
            /// </summary>
            public DacVoltageData DacVoltage
            {
                get
                {
                    return dacvoltage;
                }
            }

            /// <summary>
            /// Sets the dac index 
            /// </summary>
            public ushort DacIndex
            {
                get
                {
                    uint retVal = 0;
                    SDKBase.checkError(iacfGetPWRConfigDac(id, out retVal)); 
                    DacVoltage.DacIndex = (ushort)retVal;
                    return DacVoltage.DacIndex;
                }
                set
                {
                    dacvoltage.DacIndex = value;
                    SDKBase.checkError(iacfSetPWRConfigDac(id, (uint)DacVoltage.DacIndex)); 
                }
            }
        }

        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfPingPWR(int id);

        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfGetPWRFpaPowerSwitch(int id, out int switchOffOn);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfSetPWRFpaPowerSwitch(int id, int switchOffOn);

        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfGetPWRMaxVoltageDacs(int id, out uint MaxVoltageDacs);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfSetPWRMaxVoltageDacs(int id, uint MaxVoltageDacs);

        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfGetPWRAvailPwrMap(int id, out uint AvailPwrMap);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfSetPWRAvailPwrMap(int id, uint AvailPwrMap);

        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfGetPWRConfigDac(int id, out uint ConfigDac);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfSetPWRConfigDac(int id, uint ConfigDac);

        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfGetPWRConfigVoltage(int id, out uint ConfigVoltage);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfSetPWRConfigVoltage(int id, uint ConfigVoltage);

        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfGetPWRDac(int id, out ushort DacIndex, IntPtr voltageIdString, 
            out int maxLimit, out int minLimit, out int readBackVoltage, out int actualVoltage, 
            out int gainSF, out int offsetSF, out int basiOffset, out ushort numBits, out byte pwrOnSeq,
            out byte pwrOffSeq, out uint biasDelay, out int readBackSF );
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfSetPWRDac(int id, ushort DacIndex, IntPtr voltageIdString, 
            int maxLimit, int minLimit, int readBackVoltage, int actualVoltage, int gainSF, int offsetSF, 
            int basiOffset, ushort numBits, byte pwrOnSeq, byte pwrOffSeq, uint biasDelay, int readBackSF);

        #endregion
        
        #region Bit
        /// <summary>
        /// Handles bit functions.
        /// </summary>
        /// 
        public class Bit
        {
            /// <summary>
            /// Built in self test mode for hotlink (0=disabled 1=enabled)
            /// </summary>
            public enum HotlinkBistType
            {
                ///<summary>Built in test disabled</summary>
                HotlinkBistDisabled 			        = 0,
                ///<summary>Built in test enabled</summary>
                HotlinkBistEnabled  					= 1
            }

            /// <summary>
            /// Internal / external mode for hotlink (0=internal, 1=external)
            /// </summary>
            public enum HotlinkInternExternType
            {
                ///<summary>External Mode</summary>
                HotlinkModeExternal 					= 0,
                ///<summary>Local loopback mode</summary>
                HotlinkModeInternal 			        = 1
            }

            /// <summary>
            /// Reference to parent SDK so this module can use functions in other modules.
            /// </summary>
            /// 
            public ApacheSDK	apache;
            private int	id = -1;

            /// <summary>IndigoSDK communications channel id, don't set this directly, use iacfSDK.ID</summary>
            public int ID
            {
                set
                {
                    id = value;
                    bitObject.id = id;
                    bitStatus.id = id;
                }
            }

            /// <summary>
            /// Pings for the health/existance of the module.
            /// </summary>
            /// 
            public void Ping()
            {
                SDKBase.checkError(iacfPingBIT(id));
            }
        
            /// <summary>
            /// Built in self test mode for hotlink
            /// </summary>
            /// 
            public HotlinkBistType HotLinkBIST
            {
                set
                {
                    SDKBase.checkError(iacfSetBITHotLinkBist(id, (int)value));
                }
                get
                {
                    int retVal = 0;
                    SDKBase.checkError(iacfGetBITHotLinkBist(id, out retVal));
                    return (HotlinkBistType)retVal;
                }
            }

            /// <summary>
            /// Built in self test mode for hotlink
            /// </summary>
            /// 
            public HotlinkInternExternType HotLinkInternExternMode
            {
                set
                {
                    SDKBase.checkError(iacfSetBITHotLinkMode(id, (int)value));
                }
                get
                {
                    int retVal = 0;
                    SDKBase.checkError(iacfGetBITHotLinkMode(id, out retVal));
                    return (HotlinkInternExternType)retVal;
                }
            }

            /// <summary>
            /// Index for Bit Object
            /// </summary>
            /// 
            public byte Index
            {
                set
                {
                    SDKBase.checkError(iacfSetBITIndex(id, value));
                }
                get
                {
                    byte retVal = 0;
                    SDKBase.checkError(iacfGetBITIndex(id, out retVal));
                    return retVal;
                }
            }

            /// <summary>
            /// Read bit status.
            /// </summary>
            /// <param name="data">Data to read into.</param>
            public void GetStatus(ref byte []data)
            {
                GCHandle	gch;
                gch = GCHandle.Alloc(data, GCHandleType.Pinned);
                try
                {
                    SDKBase.checkError(iacfGetBITStatus(id, gch.AddrOfPinnedObject()));
                }
                catch
                {
                    gch.Free();
                    throw;
                }
                gch.Free();
            }

            /// <summary>
            /// Read camera memory 32-bit words at the given address.
            /// </summary>
            /// <param name="data">Data to read into.</param>
            public void GetStatus(ref uint []data)
            {
                int len = (int)Bit.BitObjectData.BitTestIndex.NumBitTests;
                byte	[]bytes = new byte[len * 4];
                uint	i;

                GetStatus(ref bytes);

                for (i = 0; i < len; ++i)
                    data[i] = BitConverter.ToUInt32(bytes, (int)(i * 4));
            }


            /// <summary>
            /// Holds the status of all bit tests
            /// </summary>
            public class BitStatusData
            {
                /// <summary>Accessories SDK control device to use.</summary>
                public int		id = -1;
                ///<summary>Status array to store raw status data in.</summary>
                protected uint [] statusArray = new uint[(int)Bit.BitObjectData.BitTestIndex.NumBitTests];
                ///<summary>Cache flag.</summary>
                protected bool	cache = false;
                ///<summary>NUC FPGA firmware flash test.</summary>
                protected uint nucFpgaFirmwareFlash = 0;
                ///<summary>NUC FPGA flash test.</summary>
                protected uint nucFpgaFlash = 0;
                ///<summary>NUC FPGA SDRAM test.</summary>
                protected uint nucFpgaSdram = 0;
                ///<summary>NUC FPGA SRAM test.</summary>
                protected uint nucFpgaSram = 0;
                ///<summary>NUC FPGA register IO test.</summary>
                protected uint nucFpgaReg = 0;
                ///<summary>Video FPGA firmware flash test.</summary>
                protected uint vidFpgaFirmwareFlash = 0;
                ///<summary>Video FPGA firmware load 1 test.</summary>
                protected uint vidFpgaFirmwareLoad1 = 0;
                ///<summary>Video FPGA firmware load 2 test.</summary>
                protected uint vidFpgaFirmwareLoad2 = 0;
                ///<summary>Video FPGA symbology test.</summary>
                protected uint vidFpgaSymbology = 0;
                ///<summary>Video FPGA DAC test.</summary>
                protected uint vidFpgaDac = 0;
                ///<summary>Video FPGA register IO test.</summary>
                protected uint vidFpgaReg = 0;
                ///<summary>DSP flash test.</summary>
                protected uint dspFlash = 0;           // Main Board Tests. DSP centric Tests. 
                ///<summary>DSP SDRAM test.</summary>
                protected uint dspSdram = 0;
                ///<summary>DSP internal RAM test.</summary>
                protected uint dspInternalRam = 0;
                ///<summary>Bias DAC tests.</summary>
                protected uint biasDac = 0;            // Digitizer Board Tests.
                ///<summary>FPA Voltage swing test.</summary>
                protected uint fpaVoltageSwing = 0;
                ///<summary>Preamplifier channel tests.</summary>
                protected uint preampChannel = 0;
                ///<summary>Preamplifier reference tests.</summary>
                protected uint preampReference = 0;
                ///<summary>Flag TE Cooler setpoint test.</summary>
                protected uint flagTESetpoint = 0;     // Power Board Tests.
                ///<summary>Dewar cool down test.</summary>
                protected uint dewarCoolDown = 0;      // Dewar Tests.
                ///<summary>Dewar steady state test.</summary>
                protected uint dewarSteadyState = 0;
                ///<summary>Serial communication test.</summary>
                protected uint SerialCom = 0;
                ///<summary>External sync test.</summary>
                protected uint externalSync = 0;

                /// <summary>
                /// Read bit status.
                /// </summary>
                /// <param name="data">Data to read into.</param>
                public void GetStatus(ref byte []data)
                {
                    GCHandle	gch;
                    gch = GCHandle.Alloc(data, GCHandleType.Pinned);
                    try
                    {
                        SDKBase.checkError(iacfGetBITStatus(id, gch.AddrOfPinnedObject()));
                    }
                    catch
                    {
                        gch.Free();
                        throw;
                    }
                    gch.Free();
                }

                /// <summary>
                /// Read camera bit status.
                /// </summary>
                /// <param name="data">Data to read into.</param>
                public void GetStatus(ref uint []data)
                {
                    int len = (int)Bit.BitObjectData.BitTestIndex.NumBitTests;
                    byte	[]bytes = new byte[len * 4];
                    uint	i;

                    GetStatus(ref bytes);

                    for (i = 0; i < len; ++i)
                        data[i] = BitConverter.ToUInt32(bytes, (int)(i * 4));
                }

                /// <summary>
                /// Read camera bit status in array of 32-bit words
                /// </summary>
                virtual protected void Read()
                {
                    GetStatus(ref statusArray);

                    // RJT - Later, we can loop through the status array and put the data in descriptive member vars
                }

                /// <summary>
                /// Returns StatusArray
                /// </summary>
                /// 
                public UInt32 [] StatusArray
                {
                    get
                    {
                        return statusArray;
                    }
                }
                
                /// <summary>
                /// Read all ststus (struct).
                /// </summary>
                public void CacheRead()
                {
                    Read();
                    cache = true;
                }

                /// <summary>
                /// Discard previously read status (struct).
                /// </summary>
                public void CacheClear()
                {
                    cache = false;
                }
            }

            private BitStatusData	bitStatus = new BitStatusData();

            /// <summary>
            /// Gets the BIY status container class.
            /// </summary>
            public BitStatusData BitStatus
            {
                get
                {
                    return bitStatus;
                }
            }
            
            /// <summary>
            /// Holds the status of all bit tests
            /// </summary>
            public class BitObjectData
            {
                /// <summary>Accessories SDK control device to use.</summary>
                public int		id = -1;

                /// <summary>
                /// Enumeration for any BIT test status
                /// </summary>
                public enum BitTestIndex
                {
                    ///<summary>Main board firmware flash test</summary>
                    NucFpgaFirmwareFlash,   
                    ///<summary>Main board FPGA flash test</summary>
                    NucFpgaFlash,      
                    ///<summary>Main board FPGA SDRAM test</summary>
                    NucFpgaSdram,
                    ///<summary>Main board FPGA SRAM test</summary>
                    NucFpgaSram,
                    ///<summary>Main board FPGA register test</summary>
                    NucFpgaReg,
                    ///<summary>Video FPGA firmwareflash test</summary>
                    VidFpgaFirmwareFlash,  
                    ///<summary>Video FPGA firmware buffer 1</summary>
                    VidFpgaFrmBuf1,
                    ///<summary>Video firmware buffer 2</summary>
                    VidFpgaFrmBuf2,
                    ///<summary>Video firmware symbology test</summary>
                    VidFpgaSymbology,
                    ///<summary>Video DAC test</summary>
                    VidFpgaDac,
                    ///<summary>Video register test</summary>
                    VidFpgaReg,
                    ///<summary>Main board DSP flash test</summary>
                    DspFlash,           
                    ///<summary>DAP SDRAM test</summary>
                    DspSdram,
                    ///<summary>DAP internal RAM test</summary>
                    DspInternalRam,
                    ///<summary>Digitizer board DAC bias test</summary>
                    BiasDac,            
                    ///<summary>FPA voltage swing  test</summary>
                    FpaVoltSwing,
                    ///<summary>Preamp channel test test</summary>
                    PreampChannel,
                    ///<summary>Preamp voltage test</summary>
                    PreampReference,
                    ///<summary>Flag Temperature Setpoint test</summary>
                    FlagTESetpoint,   /* Power Board Tests. */
                    ///<summary>Dewar cool down test</summary>
                    DewarCooldown,      
                    ///<summary>Dewar steady state test</summary>
                    DewarSteadyState,
                    ///<summary>Serial communications test</summary>
                    SerialCom,
                    ///<summary>External sync test</summary>
                    ExternalSync,
                    ///<summary>Hotling Built In Test Statust</summary>
                    HotlinkBist,
                    ///<summary>total enumerations</summary>
                    NumBitTests
                };

                /// <summary>
                /// Internal / external mode for hotlink (0=internal, 1=external)
                /// </summary>
                public enum BitTestStatus
                {                              
                    ///<summary>No Status</summary>
                    NoStatus,
                    ///<summary>Not a members</summary>
                    NotAMember,           
                    ///<summary>Not Yet Run</summary>
                    NotYetRun,                  
                    ///<summary>Running Status</summary>
                    Running,                               
                    ///<summary>Aborting status</summary>
                    Aborting,                            
                    ///<summary>Passed status</summary>
                    Pass,                                               
                    ///<summary>Fail</summary>
                    Fail,                                                  
                    ///<summary>Aborted by user</summary>
                    AbortedByUser,               
                    ///<summary>Aborted by the system</summary>
                    AbortedBySystem,        
                    ///<summary>total enumerations</summary>
                    NumBitTestStatus
                };
                
                ///<summary>Bit control parameter - needs to be masked to get PBC data.</summary>
                protected UInt32 bitControlParameters = 0;
                protected UInt32 bitMonitorParameters = 0;
                ///<summary>Cache flag.</summary>
                protected bool	cache = false;
                protected byte powerCommandBackgroundMemberships;    
                protected byte powerCommandedBasckgroundExclude;       
                protected byte powerOnTestCount;
                protected byte commandedCount; 
                protected byte  backgroundCount;           
                protected byte powerOnTestAbort;         
                protected byte commandedAbort;           
                protected byte backgroundAbort;          

                protected int  tmpTestIndex;     /* Unique id (index in the array of objs) of test. */
                protected int tmpPowerOnStatus;     /* Status of the Power-On's   execution of test. */
                protected int  tmpCommandedStatus;     /* Status of the Commanded's  execution of test. */
                protected int tmpBackgroundStatus;     /* Status of the Background's execution of test. */
                protected byte[]  histStatus = new byte[12];
                protected int globalIndex = 0;
                protected byte gSpare1 = 0;
                protected byte gSpare2 = 0;
                protected byte gSpare3 = 0;


                /// <summary>
                /// Read camera BIT object data
                /// </summary>
                virtual protected void Read()
                {
                    GCHandle	gch;
                    gch = GCHandle.Alloc(histStatus, GCHandleType.Pinned);
                    try
                    {
                        SDKBase.checkError(iacfGetBITS(id, out powerCommandBackgroundMemberships,out powerCommandedBasckgroundExclude, out powerOnTestCount,
                            out commandedCount,out backgroundCount,out powerOnTestAbort,out commandedAbort,out backgroundAbort,out tmpTestIndex,out tmpPowerOnStatus,
                            out tmpCommandedStatus,out tmpBackgroundStatus,gch.AddrOfPinnedObject(),out globalIndex,out gSpare1,out gSpare2,out gSpare3));
                    }
                    catch
                    {
                        gch.Free();
                        throw;
                    }
                    gch.Free();
                }

                /// <summary>
                /// Write camera BIT object data
                /// </summary>
                virtual protected void Write()
                {
                    GCHandle	gch;
                    gch = GCHandle.Alloc(histStatus, GCHandleType.Pinned);
                    try
                    {
                        SDKBase.checkError(iacfSetBITS(id, powerCommandBackgroundMemberships,powerCommandedBasckgroundExclude,powerOnTestCount,
                            commandedCount,backgroundCount,powerOnTestAbort,commandedAbort,backgroundAbort,tmpTestIndex,tmpPowerOnStatus,
                            tmpCommandedStatus,tmpBackgroundStatus,gch.AddrOfPinnedObject(),globalIndex,gSpare1,gSpare2,gSpare3));
                    }
                    catch
                    {
                        gch.Free();
                        throw;
                    }
                    gch.Free();
                }

                /// <summary>
                /// Returns which tests are included in power on, commanded, and background BIT tests
                /// </summary>
                /// 
                public byte PowerCommandBackgroundMemberships
                {
                    get
                    {
                        if (!cache)
                            Read();
                        return powerCommandBackgroundMemberships;
                    }
                    set
                    {
                        if (!cache)
                            Read();
                        powerCommandBackgroundMemberships = value;
                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Sets/getswhich tests are excluded in power on, commanded, and background BIT tests
                /// </summary>
                /// 
                public byte PowerCommandedBasckgroundExclude
                {
                    get
                    {
                        if (!cache)
                            Read();
                        return powerCommandedBasckgroundExclude;
                    }
                    set
                    {
                        if (!cache)
                            Read();
                        powerCommandedBasckgroundExclude = value;
                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Sets/gets the count for power on tests
                /// </summary>
                /// 
                public byte PowerOnTestCount
                {
                    get
                    {
                        if (!cache)
                            Read();
                        return powerOnTestCount;
                    }
                    set
                    {
                        if (!cache)
                            Read();
                        powerOnTestCount = value;
                        if (!cache)
                            Write();
                    }
                }


                /// <summary>
                /// Sets/gets the count for commanded tests
                /// </summary>
                /// 
                public byte CommandedCount
                {
                    get
                    {
                        if (!cache)
                            Read();
                        return commandedCount;
                    }
                    set
                    {
                        if (!cache)
                            Read();
                        commandedCount = value;
                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Sets/gets the count for background tests
                /// </summary>
                /// 
                public byte BackgroundCount
                {
                    get
                    {
                        if (!cache)
                            Read();
                        return backgroundCount;
                    }
                    set
                    {
                        if (!cache)
                            Read();
                        backgroundCount = value;
                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Sets/gets the power on test abort flag
                /// </summary>
                /// 
                public byte PowerOnTestAbort
                {
                    get
                    {
                        if (!cache)
                            Read();
                        return powerOnTestAbort;
                    }
                    set
                    {
                        if (!cache)
                            Read();
                        powerOnTestAbort = value;
                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Sets/gets the commanded test abort flag
                /// </summary>
                /// 
                public byte CommandedAbort
                {
                    get
                    {
                        if (!cache)
                            Read();
                        return commandedAbort;
                    }
                    set
                    {
                        if (!cache)
                            Read();
                        commandedAbort = value;
                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Sets/gets the background test abort flag
                /// </summary>
                /// 
                public byte BackgroundAbort
                {
                    get
                    {
                        if (!cache)
                            Read();
                        return backgroundAbort;
                    }
                    set
                    {
                        if (!cache)
                            Read();
                        backgroundAbort = value;
                        if (!cache)
                            Write();
                    }
                }

                public BitTestIndex TestIndex
                {
                    get
                    {
                        if (!cache)
                            Read();
                        return (BitTestIndex)tmpTestIndex;
                    }
                    set
                    {
                        if (!cache)
                            Read();
                        tmpTestIndex = (int)value;
                        if (!cache)
                            Write();
                    }
                }

                public BitTestStatus powerOnStatus
                {
                    get
                    {
                        if (!cache)
                            Read();
                        return (BitTestStatus)tmpPowerOnStatus;
                    }
                    set
                    {
                        if (!cache)
                            Read();
                        tmpPowerOnStatus = (int)value;
                        if (!cache)
                            Write();
                    }
                }
                
                public BitTestStatus CommandedStatus
                {
                    get
                    {
                        if (!cache)
                            Read();
                        return (BitTestStatus)tmpCommandedStatus;
                    }
                    set
                    {
                        if (!cache)
                            Read();
                        tmpCommandedStatus = (int)value;
                        if (!cache)
                            Write();
                    }
                }

                public BitTestStatus BackgroundStatus
                {
                    get
                    {
                        if (!cache)
                            Read();
                        return (BitTestStatus)tmpBackgroundStatus;
                    }
                    set
                    {
                        if (!cache)
                            Read();
                        tmpBackgroundStatus = (int)value;
                        if (!cache)
                            Write();
                    }
                }

                /// <summary>
                /// Discard previously read object.
                /// </summary>
                public void CacheClear()
                {
                    cache = false;
                }
            }

            private BitObjectData	bitObject = new BitObjectData();

            /// <summary>
            /// Gets the boy object container class.
            /// </summary>
            public BitObjectData BitObject
            {
                get
                {
                    return bitObject;
                }
            }
            
            /// <summary>
            /// Starts Bit 
            /// </summary>
            public void StartBit()
            {
                SDKBase.checkError(iacfBitCmdInitiate(id));
            }

            /// <summary>
            /// Abort Bit 
            /// </summary>
            public void AbortBit()
            {
                SDKBase.checkError(iacfBitCmdAbort(id));
            }
        
        }

        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfPingBIT(int id);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfGetBITHotLinkBist(int id, out int hotlinkBist);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfSetBITHotLinkBist(int id, int hotlinkBist);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfGetBITHotLinkMode(int id, out int hotlinkMode);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfSetBITHotLinkMode(int id, int hotlinkMode);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfGetBITIndex(int id, out byte index);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfSetBITIndex(int id, byte index);

        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfGetBITS( int id, out byte powerCommandBackgroundMembershipsBitCtlParms,
            out byte powerCommandedBasckgroundExcludeBitCtlParms,
            out byte powerOnTestCountBitCtlParms,
            out byte commandedCountBitCtlParms,
            out byte backgroundCountBitCtlParms,
            out byte powerOnTestAbortBitCtlParms,
            out byte commandedAbortBitCtlParms,
            out byte backgroundAbortBitCtlParms,
            out int  testIndexBitMonParms,
            out int  powerOnStatusBitMonParms,
            out int  commandedStatusBitMonParms, 
            out int  backgroundStatusBitMonParms, 
            IntPtr   bitStatusHistory,
            out int  globalIndex,
            out byte gSpare1,
            out byte gSpare2,
            out byte gSpare3);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfSetBITS( int id, byte powerCommandBackgroundMembershipsBitCtlParms,
            byte powerCommandedBasckgroundExcludeBitCtlParms,
            byte powerOnTestCountBitCtlParms,
            byte commandedCountBitCtlParms,
            byte backgroundCountBitCtlParms,
            byte powerOnTestAbortBitCtlParms,
            byte commandedAbortBitCtlParms,
            byte backgroundAbortBitCtlParms,
            int  testIndexBitMonParms,
            int  powerOnStatusBitMonParms,
            int  commandedStatusBitMonParms, 
            int  backgroundStatusBitMonParms, 
            IntPtr   bitStatusHistory,
            int  globalIndex,
            byte gSpare1,
            byte gSpare2,
            byte gSpare3);
        
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfGetBITStatus(int id, IntPtr bitStatus);

        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfBitCmdInitiate(int id);
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfBitCmdAbort(int id);
        
        #endregion

        #region Util
        /// <summary>
        /// Handles power functions.
        /// </summary>
        /// 
        public class Util
        {
            /// <summary>
            /// Reference to parent SDK so this module can use functions in other modules.
            /// </summary>
            /// 
            public ApacheSDK	apache;
            private int	id = -1;
            private const uint iacfIFSBlockSize = 128;

            private delegate int GenericStringProperty(int id, IntPtr sz);

            string GenericGetString(GenericStringProperty fn)
            {
                byte		[]strBytes = new byte[512];
                GCHandle	gch;
                int			len;

                gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
                try
                {
                    SDKBase.checkError(fn(id, gch.AddrOfPinnedObject()));
                }
                catch
                {
                    gch.Free();
                    throw;
                }
                gch.Free();

                for (len = 0; len < 512; ++len)
                {
                    if (strBytes[len] == 0)
                        break;
                }

                return (new ASCIIEncoding()).GetString(strBytes, 0, len);
            }

            void GenericSetString(string val, GenericStringProperty fn)
            {
                byte			[]strBytes = new byte[512];
                GCHandle		gch;
                ASCIIEncoding	ascii = new ASCIIEncoding();
                int				i;

                if (ascii.GetByteCount(val) > 512)
                    SDKBase.checkError(-1);

                for (i = 0; i < 512; ++i)
                    strBytes[i] = 0;

                ascii.GetBytes(val, 0, val.Length, strBytes, 0);

                gch = GCHandle.Alloc(strBytes, GCHandleType.Pinned);
                try
                {
                    SDKBase.checkError(fn(id, gch.AddrOfPinnedObject()));
                }
                catch
                {
                    gch.Free();
                    throw;
                }
                gch.Free();
            }

            /// <summary>IndigoSDK communications channel id, don't set this directly, use iacfSDK.ID</summary>
            public int ID
            {
                set
                {
                    id = value;
                }
            }

            /// <summary>
            /// Pings for the health/existance of the module.
            /// </summary>
            public void Ping()
            {
                SDKBase.checkError(iacfPingUTL(id));
            }
            
            /// <summary>
            /// Factory save flag 1 = save to system.ini
            /// </summary>
            public UInt32 SaveFactoryConfigFlag
            {
                set
                {
                    SDKBase.checkError(iacfSetUTLSaveFactoryConfig(id, value));
                }
                get
                {
                    uint val = 0;
                    SDKBase.checkError(iacfGetUTLSaveFactoryConfig(id, out val));
                    return val;
                }
            }
        
            /// <summary>
            /// Read camera memory bytes at the given address.
            /// </summary>
            /// <param name="address">Address.</param>
            /// <param name="data">Data to read into.</param>
            /// <param name="len">Length to read.</param>
            public void ReadMemory(uint address, ref byte []data, uint len)
            {
                GCHandle	gch;

                gch = GCHandle.Alloc(data, GCHandleType.Pinned);
                try
                {
                    SDKBase.checkError(iacfSetUTLMemSize(id, len));
                    SDKBase.checkError(iacfSetUTLMemAddress(id, address));
                    SDKBase.checkError(iacfGetUTLMemData(id,gch.AddrOfPinnedObject()));
                }
                catch
                {
                    gch.Free();
                    throw;
                }
                gch.Free();
            }

            /// <summary>
            /// Read camera memory 32-bit words at the given address.
            /// </summary>
            /// <param name="address">Address.</param>
            /// <param name="data">Data to read into.</param>
            /// <param name="len">Number of words to read.</param>
            public void ReadMemory(uint address, ref uint []data, uint len)
            {
                byte	[]bytes = new byte[len * 4];
                uint	i;

                len *= 4;
                ReadMemory(address, ref bytes, len);
                len /= 4;

                for (i = 0; i < len; ++i)
                    data[i] = BitConverter.ToUInt32(bytes, (int)(i * 4));
            }

            /// <summary>
            /// Write camera memory bytes.
            /// </summary>
            /// <param name="address">Address.</param>
            /// <param name="data">Data to write.</param>
            /// <param name="len">Length to write.</param>
            public void WriteMemory(uint address, ref byte []data, uint len)
            {
                GCHandle	gch;

                gch = GCHandle.Alloc(data, GCHandleType.Pinned);
                try
                {
                    SDKBase.checkError(iacfSetUTLMemSize(id, len));
                    SDKBase.checkError(iacfSetUTLMemAddress(id, address));
                    SDKBase.checkError(iacfSetUTLMemData(id, gch.AddrOfPinnedObject()));
                }
                catch
                {
                    gch.Free();
                    throw;
                }
                gch.Free();
            }

            /// <summary>
            /// Write camera memory 32-bit words.
            /// </summary>
            /// <param name="address">Address.</param>
            /// <param name="data">Data to write.</param>
            /// <param name="len">Number of words to write.</param>
            public void WriteMemory(uint address, ref uint []data, uint len)
            {
                byte	[]bytes = new byte[len * 4], tmpBytes;
                uint	i, j;

                for (i = 0; i < len; ++i)
                {
                    tmpBytes = BitConverter.GetBytes(data[i]);
                    for (j = 0; j < 4; ++j)
                        bytes[i * 4 + j] = tmpBytes[j];
                }

                len *= 4;
                WriteMemory(address, ref bytes, len);
                len /= 4;
            }
            
            /// <summary>
            /// Idnetifier for memory to be read/written
            /// </summary>
            public String MemName
            {
                get
                {
                    return GenericGetString(new GenericStringProperty(iacfGetUTLMemName));
                }
                set
                {
                    GenericSetString(value, new GenericStringProperty(iacfSetUTLMemName));
                }
            }

            
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetUTLMemAddress(int id, out uint MemAddress);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetUTLMemAddress(int id, uint MemAddress);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetUTLMemData(int id, IntPtr MemData);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetUTLMemData(int id, IntPtr MemData);

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetUTLMemName( int id, IntPtr MemName );
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetUTLMemName( int id, IntPtr MemName );

            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetUTLMemSize( int id, out uint MemSize );
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetUTLMemSize(int id, uint MemAddress);
           
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfGetUTLSaveFactoryConfig(int id, out uint SaveFactoryConfig);
            [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
            private static extern int iacfSetUTLSaveFactoryConfig(int id, uint SaveFactoryConfig);
        }


        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
        private static extern int iacfPingUTL(int id);
        
        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int iacfGetLibraryVersion(out ushort major, out ushort minor, out ushort revision, out ushort build);

        [DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int iacfConfigInit(int id);

		[DllImport("IACFSDK.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
		private static extern int iacfConfigFini(int id);
        #endregion
    }
}

using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace IACFSDK
{
	/// <summary>
	/// Interface to the accessories SDK.
	/// </summary>
	/// <example>
	/// AccessoriesSDK		accessories = new AccessoriesSDK();
	/// 
	/// accessories.ControlInterfaces.Discover();
	/// foreach (AccessoriesSDK.ControlInterface cif in accessories.ControlInterfaces)
	/// {
	///		cif.Discover();
	///		foreach(AccessoriesSDK.ControlDevice cd in cif)
	///		{
	///			print(cd.InterfaceName + cd.DeviceName);
	///		}
	/// }
	/// 
	/// AccessoriesSDK.ControlDevice		control = new AccessoriesSDK.ControlDevice();
	/// 
	/// control.InterfaceName = "MSCOMM";
	/// control.DeviceName = "COM1";
	/// control.BaudRate = 38400;
	/// 
	/// control.Open();
	/// 
	/// someSDK.ID = control.ID;
	/// someSDK.DoSomething();
	/// 
	/// control.Close();
	/// </example>
	public class AccessoriesSDK
	{
		/// <summary>
		/// Represents a device, which can be either control or video.
		/// </summary>
		public class Device
		{
			internal string		interfaceName;
			internal string		deviceName;
			internal int		id = -1;
			internal bool		open = false;

			/// <summary>
			/// Determins if the device is connected (open).
			/// </summary>
			public bool Connected
			{
				get
				{
					return open;
				}
			}

			/// <summary>
			/// Interface name of this device.
			/// </summary>
			public string InterfaceName
			{
				get
				{
					return interfaceName;
				}
				set
				{
					interfaceName = value;
				}
			}

			/// <summary>
			/// Name of this device.
			/// </summary>
			public string DeviceName
			{
				get
				{
					return deviceName;
				}
				set
				{
					deviceName = value;
				}
			}

			/// <summary>
			/// ID of this device.
			/// </summary>
			public int ID
			{
				get
				{
					return id;
				}
			}
		}

		/// <summary>
		/// Represents a device used to control the camera.
		/// </summary>
		public class ControlDevice : Device
		{
			internal int		baudRate = 0;
			internal string		ip = "";

			/// <summary>
			///	Baud rate of the control device.
			/// </summary>
			public int BaudRate
			{
				get
				{
					return baudRate;
				}
				set
				{
					baudRate = value;
					if (open)
						ISC_Camera.Control.SetBaudRate(id, baudRate);
				}
			}

			/// <summary>
			/// IP to configure the device for if the device needs an IP.
			/// </summary>
			public string IP
			{
				get
				{
					return ip;
				}
				set
				{
					ip = value;
				//	if (open)
				//		ISC_Camera.Config.SetIP(id, ip);
				}
			}

			/// <summary>
			/// Opens the control device with the interface name, device name, and baud stored in the object.
			/// </summary>
			/// <returns>true if successfull.</returns>
			public bool Open()
			{
				ISC_Camera.ISCTypes.isc_Error		Error;

				if (ip.Length > 0)
				{
					if (ISC_Camera.Config.OpenConfig(interfaceName, deviceName, ref id) == ISC_Camera.ISCTypes.isc_Error.eOK)
					{
						ISC_Camera.Config.SetIP(id, ip);

						ISC_Camera.Config.CloseConfig(id);
					}
				}

				if ((Error = ISC_Camera.Control.OpenControl(interfaceName, deviceName, ref id)) != ISC_Camera.ISCTypes.isc_Error.eOK)
					return false;

				if (baudRate == 0)
					ISC_Camera.Control.GetBaudRate(id, ref baudRate);
				else
					ISC_Camera.Control.SetBaudRate(id, baudRate);

		//		if (ip.Length > 0)
		//			ISC_Camera.Config.SetIP(id, ip);

			//	ISC_Camera.Control.InitControl(id, 0, 4, 13);

				open = true;

				return true;
			}

			/// <summary>
			/// Closes the control device if currently open.
			/// </summary>
			/// <returns>true if successfull.</returns>
			public bool Close()
			{
				if (open)
				{
					ISC_Camera.Control.AcquireControl(id);
					ISC_Camera.Control.CloseControl(id);
				}

				open = false;

				return true;
			}

			public bool Init(int port, ISC_Camera.ISCTypes.isc_CameraType type)
			{
				return (ISC_Camera.Control.InitControl(id, 0, port, type) == ISC_Camera.ISCTypes.isc_Error.eOK);
			}

			/// <summary>
			/// Acquire's exclusive access to the control device in a multithreaded enviroment.
			/// Device must be acquired before a transaction can take place in order to prevent
			/// "interlaced" commands.  Low level SDKs are required to do this automatically.
			/// </summary>
			public void Acquire()
			{
				ISC_Camera.Control.AcquireControl(id);
			}

			/// <summary>
			/// Release the device if it has been exclusively acquired (see Acquire).
			/// </summary>
			public void Release()
			{
				ISC_Camera.Control.ReleaseControl(id);
			}
		}

		/// <summary>
		/// Sub class for a video device.
		/// </summary>
		public class VideoDevice : Device
		{
			internal string		ip = "";

			/// <summary>
			/// IP to configure the device for if the device needs an IP.
			/// </summary>
			public string IP
			{
				get
				{
					return ip;
				}
				set
				{
					ip = value;
				}
			}

			/// <summary>
			/// Opens the control device with the interface name, device name, and baud stored in the object.
			/// </summary>
			/// <returns>true if successfull.</returns>
			public bool Open()
			{
				ISC_Camera.ISCTypes.isc_Error		Error;

				if (ip.Length > 0)
				{
					if (ISC_Camera.Config.OpenConfig(interfaceName, deviceName, ref id) == ISC_Camera.ISCTypes.isc_Error.eOK)
					{
						ISC_Camera.Config.SetIP(id, ip);

						ISC_Camera.Config.CloseConfig(id);
					}
				}

				Error = ISC_Camera.Video.OpenVideo(interfaceName, deviceName, ref id);

				if (Error != ISC_Camera.ISCTypes.isc_Error.eOK && 
					Error != ISC_Camera.ISCTypes.isc_Error.eAlreadyOpen)
					return false;

				open = true;

				return true;
			}

			/// <summary>
			/// Closes the control device if currently open.
			/// </summary>
			/// <returns>true if successfull.</returns>
			public bool Close()
			{
				if (open)
					ISC_Camera.Video.CloseVideo(id);

				open = false;

				return true;
			}

			public bool SetFrameSize(short width, short height)
			{
				return (ISC_Camera.Video.SetFrameSize(id, 0, height, width) == ISC_Camera.ISCTypes.isc_Error.eOK);
			}

			public bool GetFrameSize(out short width, out short height)
			{
				return (ISC_Camera.Video.GetFrameSize(id, 0, out height, out width) == ISC_Camera.ISCTypes.isc_Error.eOK);
			}

			public bool SetPixelDepth(ISC_Camera.ISCTypes.isc_Depth depth)
			{
				return (ISC_Camera.Video.SetPixelDepth(id, 0, (int)depth) == ISC_Camera.ISCTypes.isc_Error.eOK);
			}

			public bool GetPixelDepth(out ISC_Camera.ISCTypes.isc_Depth depth)
			{
				depth = ISC_Camera.ISCTypes.isc_Depth.eISC_14BIT;

				int		tmp = (int)depth;

				if (ISC_Camera.Video.GetPixelDepth(id, 0, ref tmp) != ISC_Camera.ISCTypes.isc_Error.eOK)
					return false;

				depth = (ISC_Camera.ISCTypes.isc_Depth)tmp;

				return true;
			}

			public bool Start()
			{
				return (ISC_Camera.Video.StartVideo(id, 0) == ISC_Camera.ISCTypes.isc_Error.eOK);
			}

			public bool Stop()
			{
				return (ISC_Camera.Video.StopVideo(id, 0) == ISC_Camera.ISCTypes.isc_Error.eOK);
			}

			public bool Init(ISC_Camera.ISCTypes.isc_CameraType type)
			{
				return (ISC_Camera.Video.InitVideo(id, 0, type) == ISC_Camera.ISCTypes.isc_Error.eOK);
			}

			public bool GrabFrame(ref ushort[] frame)
			{
				GCHandle	gch = GCHandle.Alloc(frame, GCHandleType.Pinned);
				bool		ret = true;

				if (ISC_Camera.Video.GrabFrame(id, 0, gch.AddrOfPinnedObject()) != ISC_Camera.ISCTypes.isc_Error.eOK)
					ret = false;

				gch.Free();

				return ret;
			}
		}

		/// <summary>
		/// Represents a control interface, also a list of devices on that control interface.
		/// </summary>
		public class ControlInterface : ArrayList
		{
			internal string		name;

			public ControlInterface(string n)
			{
				name = n;
			}

			/// <summary>
			/// Name of this interface.
			/// </summary>
			public string Name
			{
				get
				{
					return name;
				}
			}

			/// <summary>
			/// Discovers all devices on this interface.
			/// </summary>
			public void Discover()
			{
				Clear();

				string		s = "";
				int			i = 0;

				ISC_Camera.Discovery.RefreshDeviceList(name);

				while (ISC_Camera.Discovery.GetIFDevice(name, ref s, i) == 0)
				{
					ControlDevice		c = new ControlDevice();

					c.interfaceName = name;
					c.deviceName = s;

					Add(c);

					++i;
				}
			}

			/// <summary>
			/// Provides random access to the devices on this interface.
			/// </summary>
			public new ControlDevice this[int index]
			{
				get
				{
					return (ControlDevice)base[index];
				}
				set
				{
					base[index] = value;
				}
			}

		}

		/// <summary>
		/// Represents all control interfaces that are present.
		/// </summary>
		public class ControlInterfaceList : ArrayList
		{
			/// <summary>
			/// Discovers all interfaces.
			/// </summary>
			public void Discover()
			{
				Clear();

				string		s = "";
				int			i = 0;

				while (ISC_Camera.Discovery.GetControlIF(ref s, i) == 0)
				{
					ControlInterface	cif = new ControlInterface(s);

					Add(cif);

					++i;
				}
			}

			/// <summary>
			/// Provides random access to the control interfaces.
			/// </summary>
			public new ControlInterface this[int index]
			{
				get
				{
					return (ControlInterface)base[index];
				}
				set
				{
					base[index] = value;
				}
			}
		}

		/// <summary>
		/// Represents a video interface, also a list of devices on that video interface.
		/// </summary>
		public class VideoInterface : ArrayList
		{
			internal string		name;

			public VideoInterface(string n)
			{
				name = n;
			}

			/// <summary>
			/// Name of this interface.
			/// </summary>
			public string Name
			{
				get
				{
					return name;
				}
			}

			/// <summary>
			/// Discovers all devices on this interface.
			/// </summary>
			public void Discover()
			{
				Clear();

				string		s = "";
				int			i = 0;

				ISC_Camera.Discovery.RefreshDeviceList(name);

				while (ISC_Camera.Discovery.GetIFDevice(name, ref s, i) == 0)
				{
					VideoDevice		v = new VideoDevice();

					v.interfaceName = name;
					v.deviceName = s;

					Add(v);

					++i;
				}
			}

			/// <summary>
			/// Provides random access to the devices on this interface.
			/// </summary>
			public new VideoDevice this[int index]
			{
				get
				{
					return (VideoDevice)base[index];
				}
				set
				{
					base[index] = value;
				}
			}
		}

		/// <summary>
		/// Represents all video interfaces that are present.
		/// </summary>
		public class VideoInterfaceList : ArrayList
		{
			/// <summary>
			/// Discovers all interfaces.
			/// </summary>
			public void Discover()
			{
				Clear();

				string		s = "";
				int			i = 0;

				while (ISC_Camera.Discovery.GetVideoIF(ref s, i) == 0)
				{
					VideoInterface	vif = new VideoInterface(s);

					Add(vif);

					++i;
				}
			}

			/// <summary>
			/// Provides random access to the control interfaces.
			/// </summary>
			public new VideoInterface this[int index]
			{
				get
				{
					return (VideoInterface)base[index];
				}
				set
				{
					base[index] = value;
				}
			}
		}

		private ControlInterfaceList	controlInterfaces = new ControlInterfaceList();

		/// <summary>
		/// Returns the list of control interfaces.
		/// </summary>
		public ControlInterfaceList ControlInterfaces
		{
			get
			{
				return controlInterfaces;
			}
		}

		private VideoInterfaceList		videoInterfaces = new VideoInterfaceList();

		/// <summary>
		/// Returns the list of control interfaces.
		/// </summary>
		public VideoInterfaceList VideoInterfaces
		{
			get
			{
				return videoInterfaces;
			}
		}
	}
}

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace IACFSDK
{
	/// <summary>
	/// Various utility functions.
	/// </summary>
	public class Utility
	{
		/// <summary>
		/// Default constructor for the Utility class.
		/// </summary>
		public Utility()
		{
		}

		/// <summary>
		/// Retrieves an embedded bitmap from the assembly resource.
		/// </summary>
		/// <param name="name">The name of the file.</param>
		/// <returns>The bitmap found. Null if none was found.</returns>
		static public System.Drawing.Bitmap GetEmbeddedBitmap(string name)
		{
		//	string						fullName;
			System.Drawing.Bitmap		ret = null;

		//	fullName = System.Reflection.Assembly.GetCallingAssembly().GetName().Name + "." + name;

			try
			{
				ret = new System.Drawing.Bitmap(System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream(name));
			}
			catch (Exception e)
			{
				string	s = e.ToString();
			}

			return ret;
		}

		/// <summary>
		/// Retrieves an embedded icon from the assembly resource.
		/// </summary>
		/// <param name="name">The name of the file.</param>
		/// <returns>The Icon found. Null if none was found.</returns>
		static public System.Drawing.Icon GetEmbeddedIcon(string name)
		{
			System.Drawing.Icon		ret = null;

			try
			{
				ret = new System.Drawing.Icon(System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream(name));
			}
			catch (Exception e)
			{
				string	s = e.ToString();
			}

			return ret;
		}

		/// <summary>
		/// Retrieves an embedded bitmap from the assembly resource.
		/// </summary>
		/// <param name="name">The name of the file.</param>
		/// <param name="clr">Color to use for transparent pixels.</param>
		/// <returns>The bitmap found. Null if none was found.</returns>
		static public System.Drawing.Bitmap GetEmbeddedBitmap(string name, System.Drawing.Color clr)
		{
		//	string						fullName;
			System.Drawing.Bitmap		ret = null;

			//fullName = System.Reflection.Assembly.GetCallingAssembly().GetName().Name + "." + name;

			/*
			string		sWhat = "";
			foreach (string s in System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceNames())
			{
				sWhat += s + "\n";
			}
			*/

			try
			{
				ret = new System.Drawing.Bitmap(System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream(name));
			} 
			catch (Exception e)
			{
				string	s = e.ToString();
			}

			if (ret != null)
				ret.MakeTransparent(clr);

			return ret;
		}

		/// <summary>
		/// The bitmap used to draw the frame.
		/// </summary>
		static public System.Drawing.Bitmap		frameBitmap = GetEmbeddedBitmap("CameraControllerCore.Frame.bmp");

		/// <summary>
		/// Draws a fancy frame in the given rectangle.
		/// </summary>
		/// <param name="g">Graphics object to draw the frame on.</param>
		/// <param name="r">Rectangle to draw the frame in.</param>
		/// <param name="frameHeight">Height of the "title bar" of the frame in pixels.</param>
		static public void DrawFrame(System.Drawing.Graphics g, System.Drawing.Rectangle r, int frameHeight)
		{
			System.Drawing.Rectangle	destRect;

		//	g.Clear(System.Drawing.Color.FromArgb(242, 242, 242));

			int		titleHeight = 20;

			// top left corner
			destRect=new Rectangle(r.X + 0, r.Y + 0, 10, frameHeight);
			g.DrawImage(frameBitmap, destRect, 0, 0, 10, titleHeight, GraphicsUnit.Pixel);

			// top middle
			destRect=new Rectangle(r.X + 10, r.Y + 0, r.Width - 20, frameHeight);
			g.DrawImage(frameBitmap, destRect, 10, 0, 90, titleHeight, GraphicsUnit.Pixel);

			// top right corner
			destRect=new Rectangle(r.X + r.Width - 10, r.Y + 0, 10, frameHeight);
			g.DrawImage(frameBitmap, destRect, 100, 0, 10, titleHeight, GraphicsUnit.Pixel);

			// bottom left corner
			destRect=new Rectangle(r.X + 0, r.Y + r.Height - 5, 5, 5);
			g.DrawImage(frameBitmap, destRect, 0, 45, 5, 5, GraphicsUnit.Pixel);

			// bottom middle
			destRect=new Rectangle(r.X + 5, r.Y + r.Height - 5, r.Width - 10, 5);
			g.DrawImage(frameBitmap, destRect, 5, 45, 100, 5, GraphicsUnit.Pixel);

			// bottom right corner
			destRect=new Rectangle(r.X + r.Width - 5, r.Y + r.Height - 5, 5, 5);
			g.DrawImage(frameBitmap, destRect, 105, 45, 5, 5, GraphicsUnit.Pixel);

			// left side
			destRect=new Rectangle(r.X + 0, r.Y + frameHeight, 5, r.Height - frameHeight - 5);
			g.DrawImage(frameBitmap, destRect, 0, titleHeight, 5, 10, GraphicsUnit.Pixel);

			// right side
			destRect=new Rectangle(r.X + r.Width - 5, r.Y + frameHeight, 5, r.Height - frameHeight - 5);
			g.DrawImage(frameBitmap, destRect, 105, titleHeight, 5, 10, GraphicsUnit.Pixel);

			// center
			destRect = new Rectangle(r.X + 5, r.Y + frameHeight, r.Width - 10, r.Height - frameHeight - 5);
			SolidBrush	brush = new SolidBrush(Color.FromArgb(242, 242, 242));
			g.FillRectangle(brush, destRect);
			brush.Dispose();
		}

		/// <summary>Image list containing the system image list for filesystem objects.</summary>
		static public System.Windows.Forms.ImageList systemImageList = new System.Windows.Forms.ImageList();
		/// <summary>Hash of types to system image list indexes.</summary>
		static public System.Collections.Hashtable typeHash = new System.Collections.Hashtable();

		/// <summary>
		/// Gets the system image list index for the filesystem object of type.
		/// </summary>
		/// <param name="type">Type of filesystem object.</param>
		/// <returns>System image list index.</returns>
		static public int GetTypeImage(string type)
		{
			if (typeHash.Contains(type))
				return (int)typeHash[type];
			else
				return -1;
		}

		/// <summary>
		/// Adds an icon to the (local) system image list for the given type.
		/// </summary>
		/// <param name="type">Type of filesystem object.</param>
		/// <param name="icon">Icont o add to the image list.</param>
		/// <returns>System image list index.</returns>
		static public int AddTypeImage(string type, System.Drawing.Icon icon)
		{
			int		id;

			if ((id = GetTypeImage(type)) != -1)
				return id;

			id = systemImageList.Images.Count;

			if (id == 0)
				systemImageList.ImageSize = new Size(16, 16);

			systemImageList.Images.Add(icon);

			typeHash.Add(type, id);

			return id;
		}

		/// <summary>
		/// Gets the default file image index from the system image list.
		/// </summary>
		/// <param name="file">Filename to retrieve the icon for.</param>
		/// <returns>System image list index.</returns>
		static public int GetFileImage(string file)
		{
			return GetFileImage(file, false);
		}

		/// <summary>
		/// Gets the file image index of a file from the system image list.
		/// </summary>
		/// <param name="file">Filename to retrive the icon for.</param>
		/// <param name="open">Open flag, true if the item is selected (e.g open folder).</param>
		/// <returns>System image list index.</returns>
		static public int GetFileImage(string file, bool open)
		{
			SHFILEINFO		fi = new SHFILEINFO();
			Icon			icon;

			SHGetFileInfo(file, 0, ref fi, (uint)Marshal.SizeOf(fi), SHGFI_TYPENAME | SHGFI_ICON | SHGFI_SMALLICON | ((open) ? SHGFI_OPENICON : 0));

			if (fi.hIcon == (IntPtr)0)
				return -1;

			icon = Icon.FromHandle(fi.hIcon);

			return AddTypeImage(fi.szTypeName + ((open) ? " (Open)" : ""), icon);
		}

		/// <summary>Get Icon.</summary>
		public const uint SHGFI_ICON				= 0x000000100;
		/// <summary>Get display name.</summary>
		public const uint SHGFI_DISPLAYNAME			= 0x000000200;
		/// <summary>Get type name.</summary>
		public const uint SHGFI_TYPENAME			= 0x000000400;
		/// <summary>Get attributes.</summary>
		public const uint SHGFI_ATTRIBUTES			= 0x000000800;
		/// <summary>Get icon location.</summary>
		public const uint SHGFI_ICONLOCATION		= 0x000001000;
		/// <summary>Get executable type.</summary>
		public const uint SHGFI_EXETYPE				= 0x000002000;
		/// <summary>Get sytem image list index.</summary>
		public const uint SHGFI_SYSICONINDEX		= 0x000004000;
		/// <summary>Get link overlay icon.</summary>
		public const uint SHGFI_LINKOVERLAY			= 0x000008000;
		/// <summary>Get icon in selected state.</summary>
		public const uint SHGFI_SELECTED			= 0x000010000;
		/// <summary>Get icon with addributes specified.</summary>
		public const uint SHGFI_ATTR_SPECIFIED		= 0x000020000;
		/// <summary>Get large icon.</summary>
		public const uint SHGFI_LARGEICON			= 0x000000000;
		/// <summary>Get small icon.</summary>
		public const uint SHGFI_SMALLICON			= 0x000000001;
		/// <summary>Get icon in open state.</summary>
		public const uint SHGFI_OPENICON			= 0x000000002;
		/// <summary>Get icon size.</summary>
		public const uint SHGFI_SHELLICONSIZE		= 0x000000004;
		/// <summary>Use PIDLs instead of strings.</summary>
		public const uint SHGFI_PIDL				= 0x000000008;
		/// <summary>Use file attributes.</summary>
		public const uint SHGFI_USEFILEATTRIBUTES	= 0x000000010;
		/// <summary>Add overlays to icon (e.g. link overlay).</summary>
		public const uint SHGFI_ADDOVERLAYS			= 0x000000020;
		/// <summary>Get overlay icon index.</summary>
		public const uint SHGFI_OVERLAYINDEX		= 0x000000040;

		/// <summary>
		/// Win32 Shell file info struct.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct SHFILEINFO
		{
			/// <summary>Icon handle.</summary>
			public IntPtr hIcon;
			/// <summary>Icon index.</summary>
			public IntPtr iIcon;
			/// <summary>Attributes.</summary>
			public uint dwAttributes;
			/// <summary>Display name.</summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			/// <summary>Object type.</summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

		/// <summary>Win32 Shell file info getter function.</summary>
		[DllImport("shell32.dll")]
		extern static public IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
		/// <summary>Sends a Win32 message to a control.</summary>
		[DllImport("user32.dll")]
		extern static public int SendMessage(IntPtr hwnd, uint msg, int wParam, int lParam);

		/// <summary>
		/// Rich edit line scroll message.
		/// </summary>
		public const uint EM_LINESCROLL				= 0x000000B6;
		/// <summary>
		/// Rich edit get first visible line message.
		/// </summary>
		public const uint EM_GETFIRSTVISIBLELINE	= 0x000000CE;
		/// <summary>
		/// Rich edit get line count message.
		/// </summary>
		public const uint EM_GETLINECOUNT			= 0x000000BA;

		internal class nfaNode
		{
			public char	next;
			public char	cycle;

			internal nfaNode()
			{
				next = cycle = (char)0;
			}
		}

		//	I keep thinking this should be in the API somewhere, but I can never find it.  Ohh well, no matter.
		/// <summary>
		/// Returns true if the string matches the patter after wildcard expansion.
		/// </summary>
		/// <param name="pattern">pattern</param>
		/// <param name="str">string</param>
		/// <returns>true on match</returns>
		public static bool WildcardMatch(string pattern, string str)
		{
			int		iPat, lenPat, iNFA, iStr, strLen, iState, iNew;
			bool	add;
			nfaNode	[]nfa;
			int		[]states, newStates;
			int		nStates = 0, nNewStates;

			string	[]patterns = pattern.Split(new char[] { '|' });

			if (patterns.Length != 1)
			{
				foreach (string s in patterns)
				{
					if (WildcardMatch(s, str))
						return true;
				}

				return false;
			}

			lenPat = pattern.Length;

			nfa = new nfaNode[lenPat + 1];
			for (iNFA = 0; iNFA < nfa.Length; ++iNFA)
				nfa[iNFA] = new nfaNode();
			states = new int[lenPat + 1];
			newStates = new int[lenPat + 1];

			for (iNFA = 0, iPat = 0; iPat < lenPat; ++iPat)
			{
				switch (pattern[iPat])
				{
				case '*':
					nfa[iNFA].cycle = '*';
					break;
				case '?':
					nfa[iNFA++].next = '?';
					break;
				default:
					nfa[iNFA++].next = pattern[iPat];
					break;
				}
			}

			states[0] = 0;
			nStates = 1;

			strLen = str.Length;

			for (iStr = 0; iStr < strLen; ++iStr)
			{
				nNewStates = 0;

				for (iState = 0; iState < nStates; ++iState)
				{
					if (nfa[states[iState]].cycle == '*')
					{
						add = true;
						for (iNew = 0; iNew < nNewStates; ++iNew)
						{
							if (newStates[iNew] == states[iState])
							{
								add = false;
								break;
							}
						}

						if (add)
							newStates[nNewStates++] = states[iState];
					}
					if ((nfa[states[iState]].next == str[iStr]) ||
						(nfa[states[iState]].next == '?'))
					{
						add = true;
						for (iNew = 0; iNew < nNewStates; ++iNew)
						{
							if (newStates[iNew] == states[iState] + 1)
							{
								add = false;
								break;
							}
						}

						if (add)
							newStates[nNewStates++] = states[iState] + 1;
					}
				}

				nStates = nNewStates;
				newStates.CopyTo(states, 0);
			}

			for (iState = 0; iState < nStates; ++iState)
				if (states[iState] == iNFA)
					return true;

			return false;
		}

		/// <summary>
		/// Creates a date time object from a timestamp (1900AD based).
		/// </summary>
		/// <param name="time_t">timestamp</param>
		/// <returns>date time object</returns>
		public static DateTime DateTimeFromTime_t(uint time_t) 
		{
		//	DateTime	dt = new DateTime(1900, 1, 1, 0, 0, 0, 0);
		//	long		offset = dt.ToFileTimeUtc();

			return DateTime.FromFileTimeUtc(10000000 * (long)time_t + 94354848000000000);
		}

		private delegate void setTextDelegate(Control c, string text);

		private static void SetText(System.Windows.Forms.Control c, string text)
		{
			c.Text = text;
		}

		/// <summary>
		/// Sets the text of a control, delegates to the UI thread if necisary.
		/// </summary>
		/// <param name="c">control</param>
		/// <param name="text">text</param>
		public static void safeSetText(System.Windows.Forms.Control c, string text)
		{
			if (c.InvokeRequired)
				c.Invoke(new setTextDelegate(SetText), new object [] { c, text });
			else
				SetText(c, text);
		}

		private delegate void setEnabledDelegate(System.Windows.Forms.Control c, bool enabled);

		private static void SetEnabled(System.Windows.Forms.Control c, bool enabled)
		{
			c.Enabled = enabled;
		}

		/// <summary>
		/// Sets the enabled state of a control, delegates to the UI thread if necisary.
		/// </summary>
		/// <param name="c">control</param>
		/// <param name="enable">enable state</param>
		public static void safeSetEnabled(System.Windows.Forms.Control c, bool enable)
		{
			if (c.InvokeRequired)
				c.Invoke(new setEnabledDelegate(SetEnabled), new object [] { c, enable });
			else
				SetEnabled(c, enable);
		}

		/// <summary>
		/// Waits for a mutex without indefinitely blocking the thread (so the UI thread can respond to invoke calls).
		/// </summary>
		/// <param name="m">mutex to wait for</param>
		public static void safeWait(System.Threading.Mutex m)
		{
			while (!m.WaitOne(10, false))
			{
				if ((!Thread.CurrentThread.IsBackground) &&
					(!Thread.CurrentThread.IsThreadPoolThread))
				{
					Application.DoEvents();
				}
			}
		}

		/// <summary>
		/// Returns a string array of an array list containing strings.
		/// </summary>
		/// <param name="ar">array list containing strings</param>
		/// <returns>string array</returns>
		public static string[] ArrayListToStringArray(ArrayList ar)
		{
			string	[]ret = new string[ar.Count];
			int		i;

			for (i = 0; i < ret.Length; ++i)
				ret[i] = (string)ar[i];

			return ret;
		}
	}
}

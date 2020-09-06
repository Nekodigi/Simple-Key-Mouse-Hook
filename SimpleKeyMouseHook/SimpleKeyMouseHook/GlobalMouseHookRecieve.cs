using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GlobalHook
{
    partial class GlobalMouseHookRecieve
    {
		#region Constant, Structure and Delegate Definitions
		/// <summary>
		/// defines the callback type for the hook
		/// </summary>
		public delegate int mouseHookProc(int code, int wParam, ref MSLLHOOKSTRUCT lParam);

		public static POINT mousePosition;

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MSLLHOOKSTRUCT
		{
			public POINT pt;
			public uint mouseData, flags, time;
			public int dwExtraInfo;
		}

		const int WH_MOUSE_LL = 14;
		#endregion

		#region Constants
		public const int MAGIC_NUMBER = 0x10209;
		#endregion

		#region Instance Variables
		/// <summary>
		/// The collections of keys to watch for
		/// </summary>
		/// <summary>
		/// Handle to the hook, need this to unhook and call the next hook
		/// </summary>
		IntPtr hhook = IntPtr.Zero;
		#endregion

		#region Constructors and Destructors
		/// <summary>
		/// Initializes a new instance of the <see cref="globalKeyboardHook"/> class and installs the keyboard hook.
		/// </summary>
		public GlobalMouseHookRecieve()
		{
			hook();
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="globalKeyboardHook"/> is reclaimed by garbage collection and uninstalls the keyboard hook.
		/// </summary>
		~GlobalMouseHookRecieve()
		{
			unhook();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Installs the global hook
		/// </summary>
		public void hook()
		{
			IntPtr hInstance = LoadLibrary("User32");
			hhook = SetWindowsHookEx(WH_MOUSE_LL, hookProc, hInstance, 0);
		}

		/// <summary>
		/// Uninstalls the global hook
		/// </summary>
		public void unhook()
		{
			UnhookWindowsHookEx(hhook);
		}

		/// <summary>
		/// The callback for the keyboard hook
		/// </summary>
		/// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
		/// <param name="wParam">The event type</param>
		/// <param name="lParam">The keyhook event information</param>
		/// <returns></returns>
		
		#endregion

		#region DLL imports
		/// <summary>
		/// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
		/// </summary>
		/// <param name="idHook">The id of the event you want to hook</param>
		/// <param name="callback">The callback.</param>
		/// <param name="hInstance">The handle you want to attach the event to, can be null</param>
		/// <param name="threadId">The thread you want to attach the event to, can be null</param>
		/// <returns>a handle to the desired hook</returns>
		[DllImport("user32.dll")]
		static extern IntPtr SetWindowsHookEx(int idHook, mouseHookProc callback, IntPtr hInstance, uint threadId);

		/// <summary>
		/// Unhooks the windows hook.
		/// </summary>
		/// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
		/// <returns>True if successful, false otherwise</returns>
		[DllImport("user32.dll")]
		static extern bool UnhookWindowsHookEx(IntPtr hInstance);

		/// <summary>
		/// Calls the next hook.
		/// </summary>
		/// <param name="idHook">The hook id</param>
		/// <param name="nCode">The hook code</param>
		/// <param name="wParam">The wparam.</param>
		/// <param name="lParam">The lparam.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref MSLLHOOKSTRUCT lParam);

		/// <summary>
		/// Loads the library.
		/// </summary>
		/// <param name="lpFileName">Name of the library</param>
		/// <returns>A handle to the library</returns>
		[DllImport("kernel32.dll")]
		static extern IntPtr LoadLibrary(string lpFileName);
		#endregion
	}
}

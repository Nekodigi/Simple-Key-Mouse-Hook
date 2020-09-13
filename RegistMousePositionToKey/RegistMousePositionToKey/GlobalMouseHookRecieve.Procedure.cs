using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//based on this site https://www.codeproject.com/Articles/7294/Processing-Global-Mouse-and-Keyboard-Hooks-in-C
namespace GlobalHook
{
	partial class GlobalMouseHookRecieve
    {
		#region Windows constants

		/// <summary>
		/// Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure. 
		/// </summary>
		private const int WH_MOUSE = 7;

		/// <summary>
		/// Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure. 
		/// </summary>
		private const int WH_KEYBOARD = 2;

		/// <summary>
		/// The WM_MOUSEMOVE message is posted to a window when the cursor moves. 
		/// </summary>
		private const int WM_MOUSEMOVE = 0x200;

		/// <summary>
		/// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button 
		/// </summary>
		private const int WM_LBUTTONDOWN = 0x201;

		/// <summary>
		/// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
		/// </summary>
		private const int WM_RBUTTONDOWN = 0x204;

		/// <summary>
		/// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button 
		/// </summary>
		private const int WM_MBUTTONDOWN = 0x207;

		/// <summary>
		/// The WM_LBUTTONUP message is posted when the user releases the left mouse button 
		/// </summary>
		private const int WM_LBUTTONUP = 0x202;

		/// <summary>
		/// The WM_RBUTTONUP message is posted when the user releases the right mouse button 
		/// </summary>
		private const int WM_RBUTTONUP = 0x205;

		/// <summary>
		/// The WM_MBUTTONUP message is posted when the user releases the middle mouse button 
		/// </summary>
		private const int WM_MBUTTONUP = 0x208;

		/// <summary>
		/// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button 
		/// </summary>
		private const int WM_LBUTTONDBLCLK = 0x203;

		/// <summary>
		/// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button 
		/// </summary>
		private const int WM_RBUTTONDBLCLK = 0x206;

		/// <summary>
		/// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button 
		/// </summary>
		private const int WM_MBUTTONDBLCLK = 0x209;

		/// <summary>
		/// The WM_MOUSEWHEEL message is posted when the user presses the mouse wheel. 
		/// </summary>
		private const int WM_MOUSEWHEEL = 0x020A;

		/// <summary>
		/// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem 
		/// key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
		/// </summary>
		private const int WM_KEYDOWN = 0x100;

		/// <summary>
		/// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem 
		/// key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, 
		/// or a keyboard key that is pressed when a window has the keyboard focus.
		/// </summary>
		private const int WM_KEYUP = 0x101;

		/// <summary>
		/// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user 
		/// presses the F10 key (which activates the menu bar) or holds down the ALT key and then 
		/// presses another key. It also occurs when no window currently has the keyboard focus; 
		/// in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that 
		/// receives the message can distinguish between these two contexts by checking the context 
		/// code in the lParam parameter. 
		/// </summary>
		private const int WM_SYSKEYDOWN = 0x104;

		/// <summary>
		/// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user 
		/// releases a key that was pressed while the ALT key was held down. It also occurs when no 
		/// window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent 
		/// to the active window. The window that receives the message can distinguish between 
		/// these two contexts by checking the context code in the lParam parameter. 
		/// </summary>
		private const int WM_SYSKEYUP = 0x105;

		private const byte VK_SHIFT = 0x10;
		private const byte VK_CAPITAL = 0x14;
		private const byte VK_NUMLOCK = 0x90;

        #endregion
        #region Event
        public event MouseEventHandler s_MouseUp;
		public event MouseEventHandler s_MouseDown;
		public event MouseEventHandler s_MouseClick;
		public event MouseEventHandler s_MouseDoubleClick;
		public event MouseEventHandler s_MouseWheel;
        #endregion

        public int hookProc(int code, int wParam, ref MSLLHOOKSTRUCT lParam)
		{
			if (code >= 0 && lParam.dwExtraInfo != MAGIC_NUMBER)//use dwExtraInfo to check input by user or software
			{
				mousePosition = lParam.pt;

				MouseButtons button = MouseButtons.None;
				short mouseDelta = 0;
				int clickCount = 0;
				bool mouseDown = false;
				bool mouseUp = false;

				switch (wParam)
				{
					case WM_LBUTTONDOWN:
						mouseDown = true;
						button = MouseButtons.Left;
						clickCount = 1;
						break;
					case WM_LBUTTONUP:
						mouseUp = true;
						button = MouseButtons.Left;
						clickCount = 1;
						break;
					case WM_LBUTTONDBLCLK:
						button = MouseButtons.Left;
						clickCount = 2;
						break;
					case WM_RBUTTONDOWN:
						mouseDown = true;
						button = MouseButtons.Right;
						clickCount = 1;
						break;
					case WM_RBUTTONUP:
						mouseUp = true;
						button = MouseButtons.Right;
						clickCount = 1;
						break;
					case WM_RBUTTONDBLCLK:
						button = MouseButtons.Right;
						clickCount = 2;
						break;
					case WM_MOUSEWHEEL:
						//If the message is WM_MOUSEWHEEL, the high-order word of MouseData member is the wheel delta. 
						//One wheel click is defined as WHEEL_DELTA, which is 120. 
						//(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
						mouseDelta = (short)((lParam.mouseData >> 16) & 0xffff);

						//TODO: X BUTTONS (I havent them so was unable to test)
						//If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, 
						//or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
						//and the low-order word is reserved. This value can be one or more of the following values. 
						//Otherwise, MouseData is not used. 
						break;
				}

				//generate event //nekodigi replaced mouseEventExArgs 
				MouseEventArgs e = new MouseEventArgs(
												   button,
												   clickCount,
												   mousePosition.x,
												   mousePosition.y,
												   mouseDelta);

				//Mouse up
				if (s_MouseUp != null && mouseUp)
				{
					s_MouseUp.Invoke(null, e);
				}

				//Mouse down
				if (s_MouseDown != null && mouseDown)
				{
					s_MouseDown.Invoke(null, e);
				}

				//If someone listens to click and a click is heppened
				if (s_MouseClick != null && clickCount > 0)
				{
					s_MouseClick.Invoke(null, e);
				}

				//If someone listens to double click and a click is heppened
				if (s_MouseDoubleClick != null && clickCount == 2)
				{
					s_MouseDoubleClick.Invoke(null, e);
				}

				//Wheel was moved
				if (s_MouseWheel != null && mouseDelta != 0)
				{
					s_MouseWheel.Invoke(null, e);
				}

				//if (e.Handled)
				//{
				//	return -1;
				//}
			}
			return CallNextHookEx(hhook, code, wParam, ref lParam);
		}
	}
}

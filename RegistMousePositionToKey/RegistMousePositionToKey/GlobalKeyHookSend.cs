﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

//based on this site https://github.com/AonaSuzutsuki/KimamaKeyConverter
namespace GlobalHook
{
    class GlobalKeyHookSend
    {
        #region Win32API Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public MOUSEINPUT no;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        };
        #endregion

        #region Win32API Methods
        [DllImport("user32.dll")]
        private static extern void SendInput(int nInputs, ref INPUT pInputs, int cbsize);
        [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
        private static extern int MapVirtualKey(int wCode, int wMapType);
        #endregion

        #region Win32API Constants
        private const int INPUT_MOUSE = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int KEYEVENTF_KEYDOWN = 0x0;
        private const int KEYEVENTF_KEYUP = 0x2;
        private const int KEYEVENTF_EXTENDEDKEY = 0x1;

        #endregion

        #region Constants
        public const int MAGIC_NUMBER = 0x10209;
        #endregion

        public void KeyOnce(int key, bool isExtend = false)
        {
            INPUT input = KeyDown(key, isExtend);
            KeyUp(input, isExtend);
        }

        public INPUT KeyDown(int key, bool isExtend = false)
        {
            INPUT input = new INPUT
            {
                type = INPUT_KEYBOARD,
                ki = new KEYBDINPUT()
                {
                    wVk = (short)key,
                    wScan = (short)MapVirtualKey((short)key, 0),
                    dwFlags = ((isExtend) ? (KEYEVENTF_EXTENDEDKEY) : 0x0) | KEYEVENTF_KEYDOWN,
                    time = 0,
                    dwExtraInfo = MAGIC_NUMBER
                },
            };

            SendInput(1, ref input, Marshal.SizeOf(input));
            return input;
        }

        public void KeyUp(INPUT input, bool isExtend = false)
        {
            input.ki.dwFlags = ((isExtend) ? (KEYEVENTF_EXTENDEDKEY) : 0x0) | KEYEVENTF_KEYUP;
            SendInput(1, ref input, Marshal.SizeOf(input));
        }

        public void MouseSend(int dx, int dy, int flags, bool isExtend = false)
        {
            INPUT input = new INPUT
            {
                type = INPUT_MOUSE,
                no = new MOUSEINPUT()
                {
                    dx = dx,
                    dy = dy,
                    mouseData = 0,
                    //dwFlags = ((isExtend) ? (KEYEVENTF_EXTENDEDKEY) : 0x0) | KEYEVENTF_KEYDOWN,
                    dwFlags = flags,//dx, dy means moved pixel
                    time = 0,
                    dwExtraInfo = MAGIC_NUMBER
                },
            };
            //input.ki.dwFlags = ((isExtend) ? (KEYEVENTF_EXTENDEDKEY) : 0x0) | KEYEVENTF_KEYUP;
            SendInput(1, ref input, Marshal.SizeOf(input));
        }

        //[DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]

        //static public extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
    }
}
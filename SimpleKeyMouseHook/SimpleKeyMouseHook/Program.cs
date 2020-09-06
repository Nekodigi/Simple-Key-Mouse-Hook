using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using GlobalHook;
using System.Drawing;
using static GlobalHook.GlobalKeyHookSend;

namespace Globalkey
{
    static class Program
    {
        static GlobalKeyHookSend gkhs;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            gkhs = new GlobalKeyHookSend();
            GlobalKeyHookRecieve gkhr = new GlobalKeyHookRecieve();
            gkhr.KeyDown += gkhr_KeyDown;
            gkhr.KeyUp += gkhr_KeyUp;
            GlobalMouseHookRecieve gmhr = new GlobalMouseHookRecieve();
            gmhr.s_MouseUp += gmhr_MouseAct;
            Application.Run();
        }

        static void gkhr_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Up" + e.KeyCode.ToString());
            e.Handled = false;//if handled, I can't type.
            
            //gkhs.KeyOnce((int)'U');//software input for test
        }

        static void gkhr_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Down" + e.KeyCode.ToString());
            e.Handled = false;
            //gkhs.KeyOnce((int)'D');//software input for test
            if(e.KeyCode == Keys.LControlKey)//select word with mouse when Left Control key Pressed
            {
                Point firstPoint = Cursor.Position;//unit is not pixels
                gkhs.MouseSend(0, 0, MOUSEEVENTF_LEFTDOWN);//dx, dy unit is pixel
                gkhs.MouseSend(200, 0, MOUSEEVENTF_MOVE);//please move mouse when you want to drag and drop
                gkhs.MouseSend(200, 0, MOUSEEVENTF_LEFTUP);
                Cursor.Position = firstPoint;
            }
        }

        const int MOUSEEVENTF_MOVE = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        const int MOUSEEVENTF_VIRTUALDESK = 0x4000;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        static void gmhr_MouseAct(object sender, MouseEventArgs e)//never click console anyway
        {
            //Console.WriteLine(GlobalMouseHookRecieve.mousePosition.x);
            Console.WriteLine("X:"+Cursor.Position.X+ "Y:" + Cursor.Position.Y);
            //gkhs.MouseSet(100, 0);//move with delta x (px)
            //GlobalKeyHookSend.mouse_event(MOUSEEVENTF_LEFTDOWN, Cursor.Position.X, Cursor.Position.Y, 0, MAGIC_NUMBER);//I can use instead of that code but, we have to set MAGIC_NUMBER to check software input 
            //GlobalKeyHookSend.mouse_event(MOUSEEVENTF_LEFTUP, Cursor.Position.X, Cursor.Position.Y, 0, 0);
            //Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y);
        }


    }
}
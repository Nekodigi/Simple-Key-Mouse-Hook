using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using GlobalHook;


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
            Application.Run();
        }

        static void gkhr_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Up" + e.KeyCode.ToString());
            e.Handled = false;//if handled, I can't type.
            
            gkhs.KeyDown((int)'U');//software input for test
        }

        static void gkhr_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Down" + e.KeyCode.ToString());
            e.Handled = false;
            gkhs.KeyDown((int)'D');//software input for test
        }
    }
}
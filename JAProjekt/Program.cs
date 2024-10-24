using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//prrzekazać wskażnik do asm

namespace JAProjekt
{
    static class Program
    {
        /*
        // [STAThread]
        static void Main()
        {            
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        */
        
        [DllImport(@"C:\Users\asia3\OneDrive\Pulpit\STUDIA\3_ROK_2024-2025\SEMESTR_5\JA\Filtr_Gornoprzepustowy_HP1\JAProjekt\x64\Debug\JAAsm.dll")]
        public static extern int count_asm(int a, int b);

        [DllImport(@"C:\Users\asia3\OneDrive\Pulpit\STUDIA\3_ROK_2024-2025\SEMESTR_5\JA\Filtr_Gornoprzepustowy_HP1\JAProjekt\x64\Debug\JACpp.dll")]
        public static extern int count_cpp(int a, int b);

        static void Main()
        {

            int x = 3, y = 5;
            int retValAsm = 0;
            Thread threadAsm = new Thread(() => { retValAsm = count_asm(x, y); });
            threadAsm.Start();
            threadAsm.Join();
            Console.WriteLine("Wartość obliczona w asm to: " + retValAsm);
            //Console.ReadLine();

            int retValCpp = 0;
            Thread threadCpp = new Thread(() => { retValCpp = count_cpp(x, y); });
            threadCpp.Start();
            threadCpp.Join();
            Console.WriteLine("Wartość obliczona w cpp to: " + retValCpp);
            Console.ReadLine();

        }    
    }
}
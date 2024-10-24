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
        public static extern unsafe int count_asm(int* result, int a, int b);

        [DllImport(@"C:\Users\asia3\OneDrive\Pulpit\STUDIA\3_ROK_2024-2025\SEMESTR_5\JA\Filtr_Gornoprzepustowy_HP1\JAProjekt\x64\Debug\JACpp.dll")]
        public static extern int count_cpp(int a, int b);
        static int retValAsm = 0;

        static unsafe void Main()
        {

            int x = 3, y = 5;
            // Utworzenie wątku dla wywołania funkcji count_asm
            Thread threadAsm = new Thread(() =>
            {
                // Wywołanie funkcji asm w wątku
                fixed (int* ptr = &retValAsm) // Użycie 'fixed' dla bezpiecznego przekazania wskaźnika
                {
                    count_asm(ptr, x, y);
                }
            });
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
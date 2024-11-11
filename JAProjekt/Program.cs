using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
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

        // Dodaj HighPassFilter jako pole statyczne
        private static int[] HighPassFilter = { 0, -1, 0, -1, 5, -1, 0, -1, 0 };

        [DllImport(@"C:\Users\asia3\OneDrive\Pulpit\STUDIA\3_ROK_2024-2025\SEMESTR_5\JA\Filtr_Gornoprzepustowy_HP1\JAProjekt\x64\Debug\JAAsm.dll")]
        public static extern unsafe int count_asm(IntPtr imagePtr, int width, int height, int yStart, int stripHeight);

        [DllImport(@"C:\Users\asia3\OneDrive\Pulpit\STUDIA\3_ROK_2024-2025\SEMESTR_5\JA\Filtr_Gornoprzepustowy_HP1\JAProjekt\x64\Debug\JADLL.dll")]
        public static extern int count_c(IntPtr dataPtr, int width, int height, int yStart, int stripHeight);

        static void ProcessImageInThreads(Bitmap bitmap, int threadCount)
        {
            int width = bitmap.Width;   // Capture width once
            int height = bitmap.Height; // Capture height once

            // Create an array to store pixel data
            int[] imageArray = new int[width * height];

            // Lock the bitmap and copy data to the array
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* ptr = (byte*)bitmapData.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * width + x;
                        imageArray[index] = ptr[2] << 16 | ptr[1] << 8 | ptr[0]; // Format RGB, 24-bit color value
                        ptr += 3; // Move to the next pixel (3 bytes per pixel)
                    }
                }
            }
            bitmap.UnlockBits(bitmapData);

            // Split the processing across threads
            int stripHeight = height / threadCount;
            int remainingHeight = height % threadCount;

            Thread[] threads = new Thread[threadCount];

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < threadCount; i++)
            {
                int yStart = i * stripHeight;
                int currentStripHeight = stripHeight + (i == threadCount - 1 ? remainingHeight : 0);

                // Pass width and height as captured values to ProcessStrip
                threads[i] = new Thread(() => ProcessStrip(imageArray, yStart, currentStripHeight, width, height));
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            stopwatch.Stop();
            Console.WriteLine($"Processing time: {stopwatch.ElapsedMilliseconds} ms");
            Console.ReadLine();
            // Write the updated data back to the bitmap
            bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* ptr = (byte*)bitmapData.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * width + x;
                        ptr[2] = (byte)((imageArray[index] >> 16) & 0xFF); // R
                        ptr[1] = (byte)((imageArray[index] >> 8) & 0xFF);  // G
                        ptr[0] = (byte)(imageArray[index] & 0xFF);         // B
                        ptr += 3; // Move to the next pixel
                    }
                }
            }
            bitmap.UnlockBits(bitmapData);
        }

        static void ProcessStrip(int[] imageArray, int yStart, int stripHeight, int width, int height)
        {
            // Pin the array in memory
            GCHandle handle = GCHandle.Alloc(imageArray, GCHandleType.Pinned);
            IntPtr imagePtr = handle.AddrOfPinnedObject();

            // Call count_c using width and height passed as parameters
            count_c(imagePtr, width, height, yStart, stripHeight);
            count_asm(imagePtr, width, height, yStart, stripHeight);

            handle.Free();
        }

        static void Main(string[] args)
        {

            Bitmap bitmap = new Bitmap("C:\\Users\\asia3\\OneDrive\\Pulpit\\STUDIA\\3_ROK_2024-2025\\SEMESTR_5\\JA\\Filtr_Gornoprzepustowy_HP1\\JAProjekt\\sample.bmp");
            int threadCount = 8; // Ustalona liczba wątków
            ProcessImageInThreads(bitmap, threadCount);
            bitmap.Save("C:\\Users\\asia3\\OneDrive\\Pulpit\\STUDIA\\3_ROK_2024-2025\\SEMESTR_5\\JA\\Filtr_Gornoprzepustowy_HP1\\JAProjekt\\sampleAfter.bmp");


        }
    }
}
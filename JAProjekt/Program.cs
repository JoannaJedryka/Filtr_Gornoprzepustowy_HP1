using System;
using System.Collections.Generic;
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

        [DllImport(@"C:\Users\asia3\OneDrive\Pulpit\STUDIA\3_ROK_2024-2025\SEMESTR_5\JA\Filtr_Gornoprzepustowy_HP1\JAProjekt\x64\Debug\JAAsm.dll")]
        public static extern unsafe int count_asm(IntPtr dataPtr, int length);

        [DllImport(@"C:\Users\asia3\OneDrive\Pulpit\STUDIA\3_ROK_2024-2025\SEMESTR_5\JA\Filtr_Gornoprzepustowy_HP1\JAProjekt\x64\Debug\JACpp.dll")]
        public static extern int count_cpp(int a, int b);

        static void ProcessImageInThreads(Bitmap bitmap, int threadCount)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Tworzenie tablicy do przechowywania pikseli
            int[] imageArray = new int[width * height];

            // Blokowanie bitmapy i kopiowanie danych do tablicy
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* ptr = (byte*)bitmapData.Scan0.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * width + x;
                        imageArray[index] = ptr[2] << 16 | ptr[1] << 8 | ptr[0]; // Format RGB, wartości pikseli jako 24-bitowe
                        ptr += 3; // Przechodzenie do następnego piksela (3 bajty na piksel)
                    }
                }
            }
            bitmap.UnlockBits(bitmapData);

            // Procesowanie obrazów w wątkach
            int stripHeight = height / threadCount;
            int remainingHeight = height % threadCount;

            Thread[] threads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int yStart = i * stripHeight;
                int currentStripHeight = stripHeight + (i == threadCount - 1 ? remainingHeight : 0);

                threads[i] = new Thread(() => ProcessStrip(imageArray, yStart, currentStripHeight, width));
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            // Zapis zaktualizowanych danych z powrotem do bitmapy
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
                        ptr += 3; // Przechodzenie do następnego piksela
                    }
                }
            }
            bitmap.UnlockBits(bitmapData);
        }

        static void ProcessStrip(int[] imageArray, int yStart, int stripHeight, int width)
        {
            for (int y = yStart; y < yStart + stripHeight; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;

                    // Wyciągnięcie wartości piksela jako 4 bajtów RGBA
                    byte r = (byte)((imageArray[index] >> 16) & 0xFF);
                    byte g = (byte)((imageArray[index] >> 8) & 0xFF);
                    byte b = (byte)(imageArray[index] & 0xFF);
                    byte a = 255; // Przyjmujemy pełną przezroczystość

                    byte[] pixelData = { r, g, b, a };
                    GCHandle handle = GCHandle.Alloc(pixelData, GCHandleType.Pinned);
                    IntPtr pixelPtr = handle.AddrOfPinnedObject();

                    count_asm(pixelPtr, 5);

                    // Aktualizacja tablicy z przetworzonymi wartościami
                    imageArray[index] = (pixelData[0] << 16) | (pixelData[1] << 8) | pixelData[2]; // Zaktualizuj piksel
                    handle.Free();
                }
            }
        }

        static void Main(string[] args)
        {
            Bitmap bitmap = new Bitmap("C:\\Users\\asia3\\OneDrive\\Pulpit\\STUDIA\\3_ROK_2024-2025\\SEMESTR_5\\JA\\Filtr_Gornoprzepustowy_HP1\\JAProjekt\\sample.bmp");
            int threadCount = 4; // Ustalona liczba wątków
            ProcessImageInThreads(bitmap, threadCount);
            bitmap.Save("C:\\Users\\asia3\\OneDrive\\Pulpit\\STUDIA\\3_ROK_2024-2025\\SEMESTR_5\\JA\\Filtr_Gornoprzepustowy_HP1\\JAProjekt\\sampleKopia.bmp");
        }
    }
}
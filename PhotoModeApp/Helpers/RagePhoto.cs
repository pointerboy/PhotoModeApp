using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PhotoModeApp.Helpers
{
    internal static class RagePhoto
    {
        [DllImport("libragephoto.dll")]
        private static extern IntPtr ragephoto_open();

        [DllImport("libragephoto.dll")]
        private static extern void ragephoto_close(IntPtr instance);

        [DllImport("libragephoto.dll", CharSet = CharSet.Ansi)]
        private static extern int ragephoto_loadfile(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string filename);

        [DllImport("libragephoto.dll")]
        private static extern byte ragephoto_error(IntPtr instance);

        [DllImport("libragephoto.dll")]
        private static extern int ragephoto_getphotosize(IntPtr instance);

        [DllImport("libragephoto.dll")]
        private static extern IntPtr ragephoto_getphotojpeg(IntPtr instance);


        public static void Convert(string input)
        {
            IntPtr instance = ragephoto_open();

            int result = ragephoto_loadfile(instance, input);

            if (result != 1)
            {
                if (ragephoto_error(instance) == 0)
                {
                    throw new FileNotFoundException($"File {input} could not be found.");
                }

                else if (ragephoto_getphotosize(instance) <= 0)
                {
                    throw new InvalidDataException($"Unable to load photo {input}");
                }
            }

            byte[] bytes = new byte[ragephoto_getphotosize(instance)];

            IntPtr ptr = ragephoto_getphotojpeg(instance);

            Marshal.Copy(ptr, bytes, 0, ragephoto_getphotosize(instance));

            FileStream fileStream = File.Create(input + ".jpg");

            fileStream.Write(bytes, 0, bytes.Length);

            fileStream.Close();

            ragephoto_close(instance);
        }
    }
}

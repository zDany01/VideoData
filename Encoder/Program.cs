using Accord.Video.FFMPEG;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Encoder
{
    internal class Program
    {
        private const int VIDEO_FRAME_RATE = 30;
        private const int VIDEO_WIDTH = 320;
        private const int VIDEO_HEIGHT = 240;
        private const int BYTES_PER_FRAME = VIDEO_WIDTH * VIDEO_HEIGHT / 8;
        private static readonly Color nullControlBit = Color.Wheat;
        private static readonly Color skipControlBit = Color.Red;
        private static readonly Color fullDataControlBit = Color.Lime;
        private static readonly Color voidBit = Color.Gainsboro;
        /*     Information
         * ---------------------
         *      Bit Legend
         * ---------------------
         *  White             Bit - Bit that represent a binary 1
         *  Black             Bit - Bit that represent a binary 0
         *  Void              Bit - Bit that specify that the current pixel doesn't contain any data and shouldn't be conceived as zero
         *  Null Control      Bit - Bit that specify that specify absence of data.
         *  Skip Control      Bit - Bit that specify that the current frame shouldn't be decoded and it is only present to complete full-frame-rate seconds.
         *  Full Data Control Bit - Bit that specify that the current frame is full-filled of data.
         * 
         * ---------------------
         *      Bit Index
         * ---------------------
         * [Data Range] Black, White, Void
         * 0 Null
         * 1 Null
         * 2 Null
         * 3 Null
         * 4 Null
         * 5 Null
         * 6 Null
         * 7 Frame Data Related - Skip, Full Data, Null
         */
        private static string ConvertToMultiple(ulong byteSize)
        {
            if (byteSize > 1099511627775) return (byteSize / 1099511627776) + "TiB";
            else if (byteSize > 1073741823) return (byteSize / 1073741824) + "GiB";
            else if (byteSize > 1048575) return (byteSize / 1048576) + "MiB";
            else return byteSize > 1023 ? (byteSize / 1024) + "KiB" : byteSize + "B";
        }

        private static Bitmap ConvertToFrame(byte[] values, bool fullFrame)
        {
            Bitmap frame = new Bitmap(VIDEO_WIDTH, VIDEO_HEIGHT);
            for (int i = 0; i < VIDEO_HEIGHT; i++) for (int j = 0; j < VIDEO_WIDTH - 8; j++) frame.SetPixel(j, i, Color.White); //riempi bitmap bianca
            frame.Save("C:\\users\\danyb\\Desktop\\test.bmp");
            /*for (int i = 7; i > -1; i--)
            {
                if (value - Math.Pow(2, i) > -1)
                {
                    frame.SetPixel(i, 0, Color.White);
                    value -= (byte)Math.Pow(2, i);
                }
                else frame.SetPixel(i, 0, Color.Black);
            }*/
            return frame;
        }

        private static void Main(string[] args)
        {
#if DEBUG
            Debug.WriteLine($"[Debug-Info] Video Information: {VIDEO_WIDTH}x{VIDEO_HEIGHT}@{VIDEO_FRAME_RATE}p");
            Debug.WriteLine($"[Debug-Info] Constant Byte Information: {BYTES_PER_FRAME}B/F at {ConvertToMultiple(BYTES_PER_FRAME * VIDEO_FRAME_RATE)}/s");
#endif

            /*byte[] fileToConvert = File.ReadAllBytes(@"C:\users\danyb\Desktop\file.pdf");
            long frameToFill = VIDEO_FRAME_RATE - (fileToConvert.LongLength % VIDEO_FRAME_RATE);
            VideoFileWriter videoConverter = new VideoFileWriter();
            videoConverter.Open($"C:\\users\\danyb\\desktop\\video.mp4", VIDEO_WIDTH, VIDEO_HEIGHT, VIDEO_FRAME_RATE, VideoCodec.H264);

            for (int i = 0; i < fileToConvert.Length; i++)
            {
                Bitmap frame = ConvertToFrame(fileToConvert[i]);
                videoConverter.WriteVideoFrame(frame);
                frame.Dispose();
            }
            for (int i = 0; i < frameToFill; i++)
            {
                Bitmap fullBlack = new Bitmap(VIDEO_WIDTH, VIDEO_HEIGHT);
                for (int j = 0; i < VIDEO_HEIGHT; i++) for (int k = 0; j < VIDEO_WIDTH; j++) fullBlack.SetPixel(j, k, Color.Black);
                videoConverter.WriteVideoFrame(fullBlack);
                fullBlack.Dispose();
            }
            videoConverter.Close();*/
            Console.ReadKey();
        }
    }
}

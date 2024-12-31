using Accord.Collections;
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
        private const int VIDEO_WIDTH = 3840;
        private const int VIDEO_HEIGHT = 2160;
        private const int BYTES_PER_FRAME = VIDEO_WIDTH * VIDEO_HEIGHT / 8;
        private static readonly Color nullControlBit = Color.Wheat;
        private static readonly Color skipControlBit = Color.Red;
        private static readonly Color fullDataControlBit = Color.LimeGreen;
        private static readonly Color voidBit = Color.Gainsboro;
        /*      Information
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

        private static Bitmap ConvertToFrame(byte[] values)
        {
            if (values.Length > BYTES_PER_FRAME || values.Length < 0) throw new ArgumentOutOfRangeException(nameof(values));

            Bitmap frame = new Bitmap(VIDEO_WIDTH, VIDEO_HEIGHT);
            for (int i = 0; i < VIDEO_HEIGHT; i++) for (int j = 0; j < VIDEO_WIDTH; j++) frame.SetPixel(j, i, voidBit);

            #region "Control Bit Management"
            for (int i = VIDEO_WIDTH - 8; i < VIDEO_WIDTH; i++) frame.SetPixel(i, VIDEO_HEIGHT - 1, nullControlBit);
            if (values.Length == BYTES_PER_FRAME - 1) frame.SetPixel(VIDEO_WIDTH - 1, VIDEO_HEIGHT - 1, fullDataControlBit);
            #endregion

            for (int column = 0, bIndex = 0; column < VIDEO_HEIGHT; column++)
            {
                for (int offset = 0; bIndex < values.Length && offset < VIDEO_WIDTH; offset += 8, bIndex++)
                {
                    for (int i = 7; i > -1; i--)
                    {
                        if (values[bIndex] - Math.Pow(2, i) > -1)
                        {
                            frame.SetPixel(offset + i, column, Color.White);
                            values[bIndex] -= (byte)Math.Pow(2, i);
                        }
                        else frame.SetPixel(offset + i, column, Color.Black);
                    }
                }
            }

            return frame;
        }

        private static void Main(string[] args)
        {
#if DEBUG
            Debug.WriteLine($"[Debug-Info] Video Information: {VIDEO_WIDTH}x{VIDEO_HEIGHT}@{VIDEO_FRAME_RATE}p");
            Debug.WriteLine($"[Debug-Info] Constant Byte Information: {BYTES_PER_FRAME}B/F({ConvertToMultiple(BYTES_PER_FRAME)}) at {ConvertToMultiple(BYTES_PER_FRAME * VIDEO_FRAME_RATE)}/s");
#endif
            byte[] fileToConvert = File.ReadAllBytes(@"C:\users\danyb\Desktop\file.pdf");
            VideoFileWriter videoConverter = new VideoFileWriter();
            videoConverter.Open("C:\\users\\danyb\\desktop\\video.vp9", VIDEO_WIDTH, VIDEO_HEIGHT, VIDEO_FRAME_RATE, VideoCodec.VP9);

            byte[][] fileFrames;
            if (fileToConvert.LongLength > BYTES_PER_FRAME - 1)
            {
                uint noArray = (uint)(fileToConvert.LongLength / BYTES_PER_FRAME - 1);
                uint leftOverSize = (uint)(fileToConvert.LongLength % BYTES_PER_FRAME - 1);

                fileFrames = new byte[leftOverSize == 0 ? noArray : noArray + 1][];
                for (uint i = 0; i < fileFrames.Length - 1; i++) fileFrames[i] = new byte[BYTES_PER_FRAME - 1];
                fileFrames[fileFrames.Length - 1] = new byte[leftOverSize != 0 ? leftOverSize : BYTES_PER_FRAME - 1];

                for (int i = 0, bIndex = 0; i < fileFrames.Length; i++)
                {
                    for (int j = 0; j < fileFrames[i].Length; j++, bIndex++)
                    {
                        fileFrames[i][j] = fileToConvert[bIndex];
                    }
                }
            }
            else fileFrames = new byte[1][] { fileToConvert };

            for (int i = 0; i < fileFrames.Length; i++)
            {
                Bitmap frame = ConvertToFrame(fileFrames[i]);
                frame.Save($"C:\\users\\danyb\\desktop\\frame{i}.bmp");
                videoConverter.WriteVideoFrame(frame);
                frame.Dispose();
            }



            for (int i = 0; i < VIDEO_FRAME_RATE - (fileToConvert.LongLength % VIDEO_FRAME_RATE); i++)
            {
                Bitmap skipFrame = new Bitmap(VIDEO_WIDTH, VIDEO_HEIGHT);
                for (int j = 0; i < VIDEO_HEIGHT; i++) for (int k = 0; j < VIDEO_WIDTH; j++) skipFrame.SetPixel(j, k, Color.Black);
                skipFrame.SetPixel(VIDEO_WIDTH - 1, VIDEO_HEIGHT - 1, skipControlBit);
                videoConverter.WriteVideoFrame(skipFrame);
                skipFrame.Dispose();
            }

            videoConverter.Close();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Finished!");
            Console.ReadKey();
        }
    }
}

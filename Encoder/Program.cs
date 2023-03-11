using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using Accord.Video.FFMPEG;
using Accord.Diagnostics;
using System.Diagnostics;

namespace Encoder
{
    internal class Program
    {
        private const int VIDEO_FRAME_RATE = 30;
        private const int VIDEO_WIDTH = 320;
        private const int VIDEO_HEIGHT = 240;

        private static Bitmap ConvertToFrame(byte value)
        {
            Bitmap frame = new Bitmap(VIDEO_WIDTH, VIDEO_HEIGHT);
            for (int i = 0; i < VIDEO_HEIGHT; i++) for (int j = 0; j < VIDEO_WIDTH; j++) frame.SetPixel(j, i, Color.White); //riempi bitmap bianca
            for (int i = 7; i > -1; i--)
            {
                if (value - Math.Pow(2, i) > -1)
                {
                    frame.SetPixel(i, 0, Color.White);
                    value -= (byte)Math.Pow(2, i);
                }
                else frame.SetPixel(i, 0, Color.Black);
            }
            return frame;
        }

        static void Main(string[] args)
        {
            byte[] fileToConvert = File.ReadAllBytes(@"C:\users\danyb\Desktop\file.pdf");
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
            videoConverter.Close();
            Console.ReadKey();
        }
    }
}

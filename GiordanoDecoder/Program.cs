using System;
using System.IO;
using System.Drawing;
using Accord.Video.FFMPEG;

namespace Decoder
{
    internal class Program
    {
        private static byte ObtainValue(Bitmap frame)
        {
            byte tempdata = 0;
            for (int i = 0; i < 8; i++) tempdata += Math.Round(frame.GetPixel(i, 0).GetBrightness(), 0) == 1 ? (byte)Math.Pow(2, i) : (byte)0;
            return tempdata;
        }
        static void Main(string[] args)
        {
            VideoFileReader video = new VideoFileReader();
            video.Open(@"C:\Users\danyb\Desktop\video.mp4");
            int lastFrame = 0;
            for (int i = 0; i < video.FrameCount; i++) if (Math.Round(video.ReadVideoFrame().GetPixel(319, 239).GetBrightness(), 0) == 1) lastFrame++;
            byte[] convertedFile = new byte[lastFrame];
            for (int i = 0; i < lastFrame; i++)
            {
                convertedFile[i] = ObtainValue(video.ReadVideoFrame(i));
                Console.WriteLine($"Frame {i}: {convertedFile[i]}");
            }
            File.WriteAllBytes("C:\\Users\\danyb\\Desktop\\output.pdf", convertedFile);
            video.Close();
            Console.ReadKey();
        }
    }
}

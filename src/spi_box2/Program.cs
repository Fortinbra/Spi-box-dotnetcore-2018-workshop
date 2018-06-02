using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace spi_box2
{
    class Program
    {
        static void Main(string[] args)
        {
            var pin = Pi.Gpio.Pin07;
            pin.PinMode = GpioPinDriveMode.Input;
            pin.RegisterInterruptCallback(EdgeDetection.FallingEdge, MotionDetected);
            Console.ReadKey();
        }
        public static void MotionDetected()
        {
            Console.WriteLine("Motion Detected");
            TakePicture();
        }
        public static void TakePicture()
        {
            try
            {
                var pictureBytes = Pi.Camera.CaptureImageJpegAsync(1920, 1080).Result;
                var targetPath = $"/home/pi/picture_{DateTime.Now.ToFileTime()}.jpg";
                if (File.Exists(targetPath)) File.Delete(targetPath);
                Console.WriteLine($"Took picture -- Byte count: {pictureBytes.Length}");
                File.WriteAllBytesAsync(targetPath,pictureBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}

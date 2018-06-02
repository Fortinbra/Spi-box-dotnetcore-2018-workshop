using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace spi_box2_email
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DisplayQueue();
                FileWatcher();
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong, have a stack trace \r\n {e.StackTrace}");
            }
        }
        public static void DisplayQueue()
        {
            foreach (string file in Directory.GetFiles("/home/pi/share/pictures"))
            {

                Console.WriteLine(new FileInfo(file).FullName);
            }
        }
        public static void FileWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher($"/home/pi/share/pictures");
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.jpg";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            watcher.EnableRaisingEvents = true;
        }
        public static void SendEmail(string picturePath)
        {
            SmtpClient client = new SmtpClient("smpt.mailtrap.io");
            client.Credentials = new NetworkCredential("", "");
            client.Port = 25;
            using (var message = new MailMessage("", "")
            {
                Subject = "Danger Will Robinson",
                Body = "Motion Detected"
            })
            {
                message.Attachments.Add(new Attachment(picturePath));
                client.Send(message);
            }

        }
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} - {e.ChangeType}");
            SendEmail(e.FullPath);
        }
        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath} - {e.ChangeType}");
        }
    }
}

using System;
using System.IO;
using System.Net;

namespace FTPDownloader
{
    internal class Program
    {
        // Usage: FTPDownloader.exe <FTP-Server> <FTP-Directory> <Local-Path> <Username> <Password> <MaxDays> <Prefix> <delete>
        private static void Main(string[] args)
        {
            if (args.Length != 8)
            {
                Console.WriteLine(
                    "Usage: FTPDownloader.exe <FTP-Server> <FTP-Directory> <Local-Path> <Username> <Password> <MaxDays> <Prefix> <delete>");
                Console.WriteLine(
                    "Example: FTPDownloader.exe ftp://192.168.173.100 /output/ c:\\temp\\download\\ ftpuser \"\" 30 \"SCANNER0_\" n");
                return;
            }

            var ftpServer = args[0];
            var ftpFolder = args[1];
            var localPath = args[2];
            var userName = args[3];
            var password = args[4];

            if (!int.TryParse(args[5], out var maxDays))
            {
                Console.WriteLine("MaxDays argument must be a positive integer.");
                return;
            }

            var prefix = args[6];    
            var delete = args[7];    

            try
            {
                var request = (FtpWebRequest) WebRequest.Create(new Uri(ftpServer + ftpFolder));
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(userName, password);

                using (var response = (FtpWebResponse) request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    while (!reader.EndOfStream)
                    {
                        var fileName = reader.ReadLine();
                        if (!string.IsNullOrEmpty(fileName))
                            DownloadAndDeleteFile(ftpServer, ftpFolder + fileName, localPath, userName, password,
                                prefix, delete.ToLower()=="y" || delete.ToLower() == "yes");
                    }
                }

                if (maxDays > 0) CleanLocalFolder(localPath, maxDays, prefix);
            }
            catch (WebException e)
            {
                Console.WriteLine("Error connecting to FTP-Server: " + e.Message);
            }
        }

        private static void DownloadAndDeleteFile(string ftpServer, string filePath, string localFolder,
            string userName, string password, string prefix, bool delete)
        {
            try
            {
                var downloadRequest = (FtpWebRequest) WebRequest.Create(new Uri(ftpServer + filePath));
                downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                downloadRequest.Credentials = new NetworkCredential(userName, password);

                using (var downloadResponse = (FtpWebResponse) downloadRequest.GetResponse())
                using (var responseStream = downloadResponse.GetResponseStream())
                using (var localFile = File.Create(Path.Combine(localFolder, prefix + Path.GetFileName(filePath))))
                {
                    var buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        localFile.Write(buffer, 0, bytesRead);
                }

                // Delete file on FTP-Server
                if (delete)
                {
                    var deleteRequest = (FtpWebRequest)WebRequest.Create(new Uri(ftpServer + filePath));
                    deleteRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                    deleteRequest.Credentials = new NetworkCredential(userName, password);
                    deleteRequest.GetResponse();
                } 
            }
            catch (WebException e)
            {
                Console.WriteLine("Error downloading and deleting file: " + e.Message);
            }
        }

        private static void CleanLocalFolder(string localPath, int maxDays, string prefix)
        {
            try
            {
                var dirInfo = new DirectoryInfo(localPath);
                var files = dirInfo.GetFiles();

                var cutoffDate = DateTime.Now.AddDays(-maxDays);

                foreach (var file in files)
                    if (file.LastWriteTime < cutoffDate && file.Name.StartsWith(prefix))
                    {
                        file.Delete();
                        Console.WriteLine($"Deleted file: {file.Name}");
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error cleaning the local folder: " + e.Message);
            }
        }
    }
}
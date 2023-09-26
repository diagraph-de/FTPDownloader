using System;
using System.IO;
using System.Net;

namespace FTPDownloader
{
    internal class Program
    {
        //example FTPDownloader.exe ftp://192.168.173.100 /output/ C:\temp ftpuser ""
        private static void Main(string[] args)
        {
            if (args.Length != 5)
            {
                Console.WriteLine(
                    "Verwendung: FTPDownloader.exe <FTP-Server> <FTP-Verzeichnis> <Lokaler-Pfad> <Benutzername> <Passwort>");
                return;
            }

            var ftpServer = args[0];
            var ftpFolder = args[1];
            var localFolder = args[2];
            var userName = args[3];
            var password = args[4];

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
                            DownloadAndDeleteFile(ftpServer, ftpFolder + fileName, localFolder, userName, password);
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine("Fehler beim Verbinden mit dem FTP-Server: " + e.Message);
            }
        }

        private static void DownloadAndDeleteFile(string ftpServer, string filePath, string localFolder,
            string userName, string password)
        {
            try
            {
                var downloadRequest = (FtpWebRequest) WebRequest.Create(new Uri(ftpServer + filePath));
                downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                downloadRequest.Credentials = new NetworkCredential(userName, password);

                using (var downloadResponse = (FtpWebResponse) downloadRequest.GetResponse())
                using (var responseStream = downloadResponse.GetResponseStream())
                using (var localFile = File.Create(Path.Combine(localFolder, Path.GetFileName(filePath))))
                {
                    var buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        localFile.Write(buffer, 0, bytesRead);
                }

                // Datei auf dem FTP-Server löschen
                var deleteRequest = (FtpWebRequest) WebRequest.Create(new Uri(ftpServer + filePath));
                deleteRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                deleteRequest.Credentials = new NetworkCredential(userName, password);
                deleteRequest.GetResponse();
            }
            catch (WebException e)
            {
                Console.WriteLine("Fehler beim Herunterladen und Löschen der Datei: " + e.Message);
            }
        }
    }
}
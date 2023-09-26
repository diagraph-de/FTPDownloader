# FTPDownloader
This GitHub repository contains a simple C# application that allows you to download files from an FTP server and subsequently delete them from the FTP server. 
The application has been developed to automate file management on an FTP server and streamline the process.

This can be used for camera systems i.e. WENGLOR WeCube to transfer captured images.

Usage Instructions

Follow these steps to use the application:

    Clone the repository to your local computer:

shell

·  git clone https://github.com/YourUsername/FTP-Downloader.git

·  Navigate to the directory of the cloned repository:

·  cd FTP-Downloader

·  Compile the application:

·  csc Program.cs

·  Run the application and provide the required parameters via the command line:

 FTPDownloader.exe <FTP-Server> <FTP-Directory> <Local-Path> <Username> <Password>   

        <FTP-Server>: The URL of the FTP server, e.g., ftp://192.168.173.100.
        <FTP-Directory>: The directory on the FTP server from which files should be downloaded and deleted, e.g., /output/.
        <Local-Path>: The local path on your computer where files should be downloaded, e.g., C:\temp.
        <Username>: The username for the FTP server.
        <Password>: The password for the FTP server. If the password is empty, use an empty pair of double quotes ("").

The application will download the files from the FTP server and subsequently delete them from the server. 
License

This project is licensed under the MIT License. For more information, please see the License File.

 

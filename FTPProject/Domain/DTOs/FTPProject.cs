namespace FTPProject.Domain.DTOs
{
    public class FtpSettings
    {
        public string Protocol { get; set; } // e.g., "ftp" or "ftps"
        public string Host { get; set; }
        public int Port { get; set; } 
        public string Username { get; set; }
        public string Password { get; set; }
        public string DestinationFolder { get; set; }
    }

    public class FtpFolderSettings
    {
        public string FolderPath { get; set; }
        public string ArchivePath { get; set; }
        public FtpSettings FtpSettings { get; set; }
    }
    public class AppSettings
    {
        public List<FtpFolderSettings> FtpFolders { get; set; }
    }
}
